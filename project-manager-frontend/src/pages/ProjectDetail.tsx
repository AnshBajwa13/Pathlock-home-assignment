import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Layout } from '../components/Layout';
import { Toast } from '../components/Toast';
import { useToast } from '../hooks/useToast';
import { projectsApi } from '../features/projects/projectsApi';
import { tasksApi } from '../features/tasks/tasksApi';
import { 
  ArrowLeft, 
  Plus, 
  CheckCircle2, 
  Circle,
  Pencil,
  Trash2,
  Calendar,
  Loader2,
  AlertCircle,
  Clock,
  CalendarClock,
  Network
} from 'lucide-react';
import { CreateTaskModal } from '../features/tasks/CreateTaskModal';
import { EditTaskModal } from '../features/tasks/EditTaskModal';
import { ScheduleModal } from '../features/tasks/ScheduleModal';
import { DependencyGraph } from '../features/tasks/DependencyGraph';
import type { Task } from '../types';

export function ProjectDetail() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { toast, showToast, hideToast } = useToast();

  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<Task | null>(null);
  const [isScheduleModalOpen, setIsScheduleModalOpen] = useState(false);
  const [isDependencyGraphOpen, setIsDependencyGraphOpen] = useState(false);
  const [filter, setFilter] = useState<'all' | 'completed' | 'incomplete'>('all');

  // Fetch project details
  const { data: project, isLoading: projectLoading, error: projectError } = useQuery({
    queryKey: ['project', projectId],
    queryFn: () => projectsApi.getProjectById(projectId!),
    enabled: !!projectId,
  });

  // Fetch tasks for this project
  const { data: tasks = [], isLoading: tasksLoading } = useQuery({
    queryKey: ['tasks', projectId],
    queryFn: () => tasksApi.getTasksByProject(projectId!),
    enabled: !!projectId,
  });

  // Toggle task completion
  const toggleMutation = useMutation({
    mutationFn: (taskId: string) => tasksApi.toggleTaskCompletion(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks', projectId] });
      queryClient.invalidateQueries({ queryKey: ['project', projectId] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      showToast('Task status updated!', 'success');
    },
  });

  // Delete task
  const deleteMutation = useMutation({
    mutationFn: (taskId: string) => tasksApi.deleteTask(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks', projectId] });
      queryClient.invalidateQueries({ queryKey: ['project', projectId] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      showToast('Task deleted successfully!', 'success');
    },
  });

  const handleToggleTask = (taskId: string) => {
    toggleMutation.mutate(taskId);
  };

  const handleDeleteTask = (taskId: string, description: string) => {
    if (window.confirm(`Delete task "${description}"?`)) {
      deleteMutation.mutate(taskId);
    }
  };

  // Filter tasks
  const filteredTasks = tasks.filter((task) => {
    if (filter === 'completed') return task.isCompleted;
    if (filter === 'incomplete') return !task.isCompleted;
    return true;
  });

  // Helper functions for task status
  const isOverdue = (task: Task) => {
    if (!task.dueDate || task.isCompleted) return false;
    return new Date(task.dueDate) < new Date();
  };

  const isDueSoon = (task: Task) => {
    if (!task.dueDate || task.isCompleted) return false;
    const daysUntilDue = Math.ceil(
      (new Date(task.dueDate).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24)
    );
    return daysUntilDue >= 0 && daysUntilDue <= 3;
  };

  const getTaskBorderColor = (task: Task) => {
    if (task.isCompleted) return 'border-green-200 bg-green-50/30';
    if (isOverdue(task)) return 'border-red-300 bg-red-50/20 border-l-4 border-l-red-500';
    if (isDueSoon(task)) return 'border-orange-300 bg-orange-50/20 border-l-4 border-l-orange-500';
    return 'border-gray-200 hover:border-primary/30';
  };

  const getStatusBadge = (task: Task) => {
    if (task.isCompleted) {
      return (
        <span className="inline-flex items-center gap-1 px-2 py-1 bg-green-100 text-green-700 text-xs font-semibold rounded-full">
          <CheckCircle2 className="w-3 h-3" />
          Done
        </span>
      );
    }
    if (isOverdue(task)) {
      return (
        <span className="inline-flex items-center gap-1 px-2 py-1 bg-red-100 text-red-700 text-xs font-semibold rounded-full">
          <AlertCircle className="w-3 h-3" />
          Overdue
        </span>
      );
    }
    if (isDueSoon(task)) {
      return (
        <span className="inline-flex items-center gap-1 px-2 py-1 bg-orange-100 text-orange-700 text-xs font-semibold rounded-full">
          <Clock className="w-3 h-3" />
          Due Soon
        </span>
      );
    }
    return (
      <span className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-700 text-xs font-semibold rounded-full">
        <Circle className="w-3 h-3" />
        To Do
      </span>
    );
  };

  if (projectLoading || tasksLoading) {
    return (
      <Layout>
        <div className="max-w-5xl mx-auto py-8 px-4">
          <div className="flex items-center justify-center py-20">
            <Loader2 className="w-8 h-8 animate-spin text-primary" />
          </div>
        </div>
      </Layout>
    );
  }

  if (projectError || !project) {
    return (
      <Layout>
        <div className="max-w-5xl mx-auto py-8 px-4">
          <div className="bg-red-50 border-l-4 border-red-500 p-6 rounded-lg">
            <div className="flex items-center gap-3">
              <AlertCircle className="w-6 h-6 text-red-600" />
              <div>
                <h3 className="font-semibold text-red-900">Project not found</h3>
                <p className="text-red-700 text-sm mt-1">
                  The project you're looking for doesn't exist or has been deleted.
                </p>
              </div>
            </div>
            <button
              onClick={() => navigate('/dashboard')}
              className="mt-4 text-red-700 hover:text-red-900 font-medium flex items-center gap-2"
            >
              <ArrowLeft className="w-4 h-4" />
              Back to Projects
            </button>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-5xl mx-auto py-8 px-4">
        {/* Back Button */}
        <button
          onClick={() => navigate('/dashboard')}
          className="flex items-center gap-2 text-gray-600 hover:text-primary font-medium mb-6 transition"
        >
          <ArrowLeft className="w-4 h-4" />
          Back to Projects
        </button>

        {/* Project Header */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-start justify-between mb-4">
            <div className="flex-1">
              <h1 className="text-3xl font-bold text-gray-900 mb-2">{project.title}</h1>
              <p className="text-gray-600">{project.description}</p>
            </div>
          </div>

          {/* Project Stats */}
          <div className="flex items-center gap-6 pt-4 border-t border-gray-100">
            <div className="flex items-center gap-2">
              <div className="flex items-center justify-center w-8 h-8 rounded-full bg-green-100">
                <CheckCircle2 className="w-4 h-4 text-green-600" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">{project.completedTaskCount}</p>
                <p className="text-xs text-gray-600">Completed</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <div className="flex items-center justify-center w-8 h-8 rounded-full bg-gray-100">
                <Circle className="w-4 h-4 text-gray-400" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">{project.taskCount}</p>
                <p className="text-xs text-gray-600">Total Tasks</p>
              </div>
            </div>
            <div className="flex items-center gap-2">
              <Calendar className="w-5 h-5 text-gray-400" />
              <div>
                <p className="text-sm font-medium text-gray-900">
                  {new Date(project.createdAt).toLocaleDateString('en-US', {
                    month: 'long',
                    day: 'numeric',
                    year: 'numeric',
                  })}
                </p>
                <p className="text-xs text-gray-600">Created</p>
              </div>
            </div>
          </div>
        </div>

        {/* Tasks Header */}
        <div className="flex items-center justify-between mb-6">
          <div>
            <h2 className="text-2xl font-bold text-gray-900">Tasks</h2>
            <p className="text-gray-600 text-sm mt-1">
              Manage and track your project tasks
            </p>
          </div>
          <div className="flex items-center gap-3">
            <button
              onClick={() => setIsDependencyGraphOpen(true)}
              disabled={tasks.length === 0}
              className="flex items-center gap-2 bg-purple-600 hover:bg-purple-700 text-white px-5 py-2.5 rounded-lg transition font-semibold shadow-sm hover:shadow-md disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Network className="w-5 h-5" />
              View Graph
            </button>
            <button
              onClick={() => setIsScheduleModalOpen(true)}
              disabled={tasks.length === 0}
              className="flex items-center gap-2 text-white px-5 py-2.5 rounded-lg transition font-semibold shadow-sm hover:shadow-md disabled:opacity-50 disabled:cursor-not-allowed"
              style={{ backgroundColor: '#0F1555' }}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#0a0d3d'}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = '#0F1555'}
            >
              <CalendarClock className="w-5 h-5" />
              Generate Schedule
            </button>
            <button
              onClick={() => setIsCreateModalOpen(true)}
              className="flex items-center gap-2 bg-primary hover:bg-primary/90 text-white px-5 py-2.5 rounded-lg transition font-semibold shadow-sm hover:shadow-md"
            >
              <Plus className="w-5 h-5" />
              Add Task
            </button>
          </div>
        </div>

        {/* Filter Tabs */}
        <div className="flex gap-2 mb-6">
          <button
            onClick={() => setFilter('all')}
            className={`px-4 py-2 rounded-lg font-medium transition ${
              filter === 'all'
                ? 'bg-primary text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            All ({tasks.length})
          </button>
          <button
            onClick={() => setFilter('incomplete')}
            className={`px-4 py-2 rounded-lg font-medium transition ${
              filter === 'incomplete'
                ? 'bg-primary text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            Incomplete ({tasks.filter((t) => !t.isCompleted).length})
          </button>
          <button
            onClick={() => setFilter('completed')}
            className={`px-4 py-2 rounded-lg font-medium transition ${
              filter === 'completed'
                ? 'bg-primary text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            Completed ({tasks.filter((t) => t.isCompleted).length})
          </button>
        </div>

        {/* Tasks List */}
        {filteredTasks.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-16 text-center">
            <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gray-100 mb-6">
              <CheckCircle2 className="w-10 h-10 text-gray-400" />
            </div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">
              {filter === 'all' ? 'No tasks yet' : `No ${filter} tasks`}
            </h3>
            <p className="text-gray-600 mb-8 max-w-md mx-auto">
              {filter === 'all'
                ? 'Get started by creating your first task for this project'
                : `There are no ${filter} tasks in this project`}
            </p>
            {filter === 'all' && (
              <button
                onClick={() => setIsCreateModalOpen(true)}
                className="inline-flex items-center gap-2 bg-primary hover:bg-primary/90 text-white px-6 py-3 rounded-lg transition font-semibold shadow-sm hover:shadow-md"
              >
                <Plus className="w-5 h-5" />
                Create Your First Task
              </button>
            )}
          </div>
        ) : (
          <div className="space-y-3">
            {filteredTasks.map((task) => (
              <div
                key={task.id}
                className={`bg-white rounded-xl shadow-sm border-2 p-5 transition-all duration-200 hover:shadow-md ${getTaskBorderColor(task)}`}
              >
                <div className="flex items-start gap-4">
                  {/* Checkbox */}
                  <button
                    onClick={() => handleToggleTask(task.id)}
                    className="mt-1 flex-shrink-0 transition-transform hover:scale-110"
                    disabled={toggleMutation.isPending}
                  >
                    {toggleMutation.isPending ? (
                      <Loader2 className="w-6 h-6 animate-spin text-primary" />
                    ) : task.isCompleted ? (
                      <CheckCircle2 className="w-6 h-6 text-green-600" />
                    ) : (
                      <Circle className="w-6 h-6 text-gray-400 hover:text-primary" />
                    )}
                  </button>

                  {/* Task Content */}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <p
                        className={`text-gray-900 font-semibold flex-1 ${
                          task.isCompleted ? 'line-through text-gray-500' : ''
                        }`}
                      >
                        {task.title}
                      </p>
                      {getStatusBadge(task)}
                    </div>
                    
                    {/* Description */}
                    {task.description && (
                      <p className={`text-sm mb-2 ${
                        task.isCompleted ? 'text-gray-400 line-through' : 'text-gray-600'
                      }`}>
                        {task.description}
                      </p>
                    )}
                    
                    {/* Task Metadata */}
                    <div className="flex items-center gap-4 flex-wrap">
                      {/* Due Date */}
                      {task.dueDate && (
                        <div className="flex items-center gap-2">
                          <Clock className={`w-4 h-4 ${
                            isOverdue(task) ? 'text-red-500' : 
                            isDueSoon(task) ? 'text-orange-500' : 
                            'text-gray-400'
                          }`} />
                          <span className={`text-sm font-medium ${
                            isOverdue(task) ? 'text-red-600' : 
                            isDueSoon(task) ? 'text-orange-600' : 
                            'text-gray-600'
                          }`}>
                            Due: {new Date(task.dueDate).toLocaleDateString('en-US', {
                              month: 'short',
                              day: 'numeric',
                              year: 'numeric',
                            })}
                            {isOverdue(task) && (
                              <span className="ml-2 text-xs">
                                ({Math.ceil((new Date().getTime() - new Date(task.dueDate).getTime()) / (1000 * 60 * 60 * 24))} days ago)
                              </span>
                            )}
                            {isDueSoon(task) && !isOverdue(task) && (
                              <span className="ml-2 text-xs">
                                (in {Math.ceil((new Date(task.dueDate).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24))} days)
                              </span>
                            )}
                          </span>
                        </div>
                      )}

                      {/* Estimated Hours */}
                      {task.estimatedHours && (
                        <div className="flex items-center gap-2">
                          <span className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-700 text-xs font-semibold rounded-full">
                            <Clock className="w-3 h-3" />
                            {task.estimatedHours}h
                          </span>
                        </div>
                      )}

                      {/* Dependencies with Names */}
                      {task.dependencyIds && task.dependencyIds.length > 0 && (
                        <div className="flex items-center gap-2">
                          <span className="inline-flex items-center gap-1 px-2 py-1 bg-purple-100 text-purple-700 text-xs font-semibold rounded-full">
                            Depends on: {task.dependencyIds.map((depId) => {
                              const depTask = tasks.find(t => t.id === depId);
                              return depTask?.title || 'Unknown';
                            }).join(', ')}
                          </span>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Actions */}
                  <div className="flex gap-1">
                    <button
                      onClick={() => setEditingTask(task)}
                      className="p-2 text-gray-400 hover:text-primary hover:bg-primary/5 rounded-lg transition"
                      title="Edit task"
                    >
                      <Pencil className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleDeleteTask(task.id, task.title)}
                      disabled={deleteMutation.isPending}
                      className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition disabled:opacity-50"
                      title="Delete task"
                    >
                      {deleteMutation.isPending ? (
                        <Loader2 className="w-4 h-4 animate-spin" />
                      ) : (
                        <Trash2 className="w-4 h-4" />
                      )}
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Modals */}
      {isCreateModalOpen && projectId && (
        <CreateTaskModal
          projectId={projectId}
          onClose={() => setIsCreateModalOpen(false)}
        />
      )}
      {editingTask && (
        <EditTaskModal
          task={editingTask}
          onClose={() => setEditingTask(null)}
        />
      )}
      {isScheduleModalOpen && projectId && (
        <ScheduleModal
          projectId={projectId}
          onClose={() => setIsScheduleModalOpen(false)}
        />
      )}
      {isDependencyGraphOpen && (
        <DependencyGraph
          tasks={tasks}
          onClose={() => setIsDependencyGraphOpen(false)}
        />
      )}

      {/* Toast Notification */}
      {toast.isVisible && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={hideToast}
        />
      )}
    </Layout>
  );
}
