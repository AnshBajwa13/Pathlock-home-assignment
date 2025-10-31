import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Layout } from '../../components/Layout';
import { projectsApi } from './projectsApi';
import { tasksApi } from '../tasks/tasksApi';
import { Plus, Folder, Calendar, CheckCircle2, Circle, Pencil, Trash2, Loader2, ChevronRight } from 'lucide-react';
import { CreateProjectModal } from './CreateProjectModal';
import { EditProjectModal } from './EditProjectModal';
import { Toast } from '../../components/Toast';
import { useToast } from '../../hooks/useToast';
import type { Project, Task } from '../../types';

export function ProjectsList() {
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editingProject, setEditingProject] = useState<Project | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [expandedProjects, setExpandedProjects] = useState<Set<string>>(new Set());
  const pageSize = 9;

  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { toast, showToast, hideToast } = useToast();

  const toggleProjectExpanded = (projectId: string) => {
    setExpandedProjects((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(projectId)) {
        newSet.delete(projectId);
      } else {
        newSet.add(projectId);
      }
      return newSet;
    });
  };

  // Fetch projects
  const { data, isLoading, error } = useQuery({
    queryKey: ['projects', currentPage],
    queryFn: () => projectsApi.getProjects(currentPage, pageSize),
  });

  // Delete mutation
  const deleteMutation = useMutation({
    mutationFn: projectsApi.deleteProject,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    },
  });

  const handleDelete = async (id: string, title: string) => {
    if (window.confirm(`Are you sure you want to delete "${title}"?`)) {
      await deleteMutation.mutateAsync(id);
    }
  };

  // Toggle task mutation
  const toggleTaskMutation = useMutation({
    mutationFn: tasksApi.toggleTaskCompletion,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      showToast('Task updated!', 'success');
    },
  });

  if (isLoading) {
    return (
      <Layout>
        <div className="flex items-center justify-center h-64">
          <div className="w-12 h-12 border-4 border-primary border-t-transparent rounded-full animate-spin" />
        </div>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
          Failed to load projects. Please try again.
        </div>
      </Layout>
    );
  }

  const projects = data?.items || [];
  const totalPages = data?.totalPages || 1;

  // Quick Tasks Preview Component
  const QuickTasksPreview = ({ project }: { project: Project }) => {
    const isExpanded = expandedProjects.has(project.id);
    
    const { data: tasks, isLoading: tasksLoading } = useQuery({
      queryKey: ['tasks', project.id],
      queryFn: () => tasksApi.getTasksByProject(project.id),
      enabled: isExpanded,
    });

    if (!isExpanded) {
      return project.taskCount > 0 ? (
        <div className="mt-4 pt-4 border-t border-gray-100">
          <button
            onClick={(e) => {
              e.stopPropagation();
              toggleProjectExpanded(project.id);
            }}
            className="w-full flex items-center justify-between text-sm text-gray-600 hover:text-primary transition py-2 px-3 rounded-lg hover:bg-gray-50"
          >
            <span className="font-medium">Quick view tasks ({project.taskCount})</span>
            <ChevronRight className="w-4 h-4" />
          </button>
        </div>
      ) : null;
    }

    if (tasksLoading) {
      return (
        <div className="mt-4 pt-4 border-t border-gray-100">
          <div className="flex items-center justify-center py-4">
            <Loader2 className="w-5 h-5 animate-spin text-primary" />
          </div>
        </div>
      );
    }

    const tasksList = tasks || [];
    
    // Sort tasks: incomplete tasks first, then by due date (soonest first), then by creation date
    const sortedTasks = [...tasksList].sort((a, b) => {
      // Completed tasks go to the end
      if (a.isCompleted !== b.isCompleted) {
        return a.isCompleted ? 1 : -1;
      }
      
      // For incomplete tasks, prioritize by due date
      if (!a.isCompleted && !b.isCompleted) {
        // Tasks with due dates come first
        if (a.dueDate && !b.dueDate) return -1;
        if (!a.dueDate && b.dueDate) return 1;
        
        // Both have due dates - sort by soonest first
        if (a.dueDate && b.dueDate) {
          return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime();
        }
      }
      
      // Fall back to creation date (newest first)
      return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    });
    
    const displayTasks = sortedTasks.slice(0, 3);
    const remainingCount = tasksList.length - 3;

    // Helper to check if task is overdue
    const isOverdue = (task: Task) => {
      if (!task.dueDate || task.isCompleted) return false;
      return new Date(task.dueDate) < new Date();
    };

    // Helper to check if task is due soon (within 3 days)
    const isDueSoon = (task: Task) => {
      if (!task.dueDate || task.isCompleted) return false;
      const daysUntilDue = Math.ceil(
        (new Date(task.dueDate).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24)
      );
      return daysUntilDue >= 0 && daysUntilDue <= 3;
    };

    return (
      <div className="mt-4 pt-4 border-t border-gray-100">
        <div className="flex items-center justify-between mb-3">
          <span className="text-sm font-semibold text-gray-700">Quick Tasks</span>
          <button
            onClick={(e) => {
              e.stopPropagation();
              toggleProjectExpanded(project.id);
            }}
            className="text-xs text-gray-500 hover:text-gray-700 transition"
          >
            Hide
          </button>
        </div>
        
        {displayTasks.length === 0 ? (
          <p className="text-sm text-gray-500 py-2">No tasks yet</p>
        ) : (
          <div className="space-y-2">
            {displayTasks.map((task) => (
              <div
                key={task.id}
                className={`flex items-start gap-2 py-2 px-2 rounded-lg hover:bg-gray-50 transition group ${
                  isOverdue(task) ? 'border-l-2 border-l-red-500 bg-red-50/30' :
                  isDueSoon(task) ? 'border-l-2 border-l-orange-500 bg-orange-50/30' : ''
                }`}
              >
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    toggleTaskMutation.mutate(task.id);
                  }}
                  disabled={toggleTaskMutation.isPending}
                  className="flex-shrink-0 transition-transform hover:scale-110 mt-0.5"
                >
                  {toggleTaskMutation.isPending ? (
                    <Loader2 className="w-4 h-4 animate-spin text-primary" />
                  ) : task.isCompleted ? (
                    <CheckCircle2 className="w-4 h-4 text-green-600" />
                  ) : (
                    <Circle className="w-4 h-4 text-gray-400 group-hover:text-primary" />
                  )}
                </button>
                <div className="flex-1 min-w-0">
                  <div
                    className={`font-medium text-sm mb-0.5 ${
                      task.isCompleted ? 'line-through text-gray-500' : 'text-gray-900'
                    }`}
                  >
                    {task.title}
                  </div>
                  {task.description && (
                    <div className={`text-xs truncate ${
                      task.isCompleted ? 'text-gray-400' : 'text-gray-600'
                    }`}>
                      {task.description}
                    </div>
                  )}
                  {task.dueDate && (
                    <div className={`flex items-center gap-1 mt-1 text-xs ${
                      isOverdue(task) ? 'text-red-600 font-semibold' :
                      isDueSoon(task) ? 'text-orange-600 font-semibold' :
                      'text-gray-500'
                    }`}>
                      <Calendar className="w-3 h-3" />
                      <span>
                        {isOverdue(task) && '⚠️ '}
                        {new Date(task.dueDate).toLocaleDateString('en-US', {
                          month: 'short',
                          day: 'numeric'
                        })}
                        {isOverdue(task) && ' (overdue)'}
                        {isDueSoon(task) && !isOverdue(task) && ' (due soon)'}
                      </span>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}

        {remainingCount > 0 && (
          <button
            onClick={() => navigate(`/projects/${project.id}`)}
            className="w-full mt-3 text-xs text-primary hover:text-primary/80 font-medium py-2 text-center hover:bg-primary/5 rounded-lg transition"
          >
            View all {tasksList.length} tasks →
          </button>
        )}
      </div>
    );
  };

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Your Projects</h1>
            <p className="text-gray-600 mt-1">
              Manage your projects and track progress
            </p>
          </div>
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="flex items-center gap-2 bg-primary hover:bg-primary/90 text-white px-4 py-2 rounded-lg transition font-semibold"
          >
            <Plus className="w-5 h-5" />
            New Project
          </button>
        </div>

        {/* Projects Grid */}
        {projects.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm p-16 text-center border border-gray-200">
            <div className="inline-flex items-center justify-center w-20 h-20 rounded-full bg-gray-100 mb-6">
              <Folder className="w-10 h-10 text-gray-400" />
            </div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">
              No projects yet
            </h3>
            <p className="text-gray-600 mb-8 max-w-md mx-auto">
              Get started by creating your first project to organize and track your tasks
            </p>
            <button
              onClick={() => setIsCreateModalOpen(true)}
              className="inline-flex items-center gap-2 bg-primary hover:bg-primary/90 text-white px-6 py-3 rounded-lg transition font-semibold shadow-sm hover:shadow-md"
            >
              <Plus className="w-5 h-5" />
              Create Your First Project
            </button>
          </div>
        ) : (
          <>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {projects.map((project) => (
                <div
                  key={project.id}
                  className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-lg hover:border-primary/30 transition-all duration-200 group"
                >
                  {/* Project Header */}
                  <div className="flex items-start justify-between mb-3">
                    <div className="flex-1">
                      <h3 className="font-bold text-gray-900 text-lg mb-1 group-hover:text-primary transition">
                        {project.title}
                      </h3>
                    </div>
                    <div className="flex gap-1 ml-2">
                      <button
                        onClick={() => setEditingProject(project)}
                        className="p-2 text-gray-400 hover:text-primary hover:bg-primary/5 rounded-lg transition"
                        title="Edit project"
                      >
                        <Pencil className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => handleDelete(project.id, project.title)}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                        title="Delete project"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </div>
                  </div>

                  {/* Project Description */}
                  <p className="text-gray-600 text-sm mb-4 line-clamp-2 min-h-[40px]">
                    {project.description || 'No description provided'}
                  </p>

                  {/* Progress Bar */}
                  {project.taskCount > 0 && (
                    <div className="mb-4">
                      <div className="flex justify-between items-center mb-1.5">
                        <span className="text-xs font-medium text-gray-600">Progress</span>
                        <span className="text-xs font-semibold text-primary">
                          {Math.round((project.completedTaskCount / project.taskCount) * 100)}%
                        </span>
                      </div>
                      <div className="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
                        <div
                          className="h-full bg-gradient-to-r from-primary to-primary/80 rounded-full transition-all duration-500"
                          style={{
                            width: `${(project.completedTaskCount / project.taskCount) * 100}%`,
                          }}
                        ></div>
                      </div>
                    </div>
                  )}

                  {/* Divider */}
                  <div className="border-t border-gray-100 my-4"></div>

                  {/* Project Stats */}
                  <div className="flex items-center justify-between text-sm mb-4">
                    <div className="flex items-center gap-3">
                      <div className="flex items-center gap-1.5">
                        <div className="flex items-center justify-center w-5 h-5 rounded-full bg-green-100">
                          <CheckCircle2 className="w-3 h-3 text-green-600" />
                        </div>
                        <span className="text-gray-700 font-medium">{project.completedTaskCount}</span>
                        <span className="text-gray-400 text-xs">done</span>
                      </div>
                      <div className="flex items-center gap-1.5">
                        <div className="flex items-center justify-center w-5 h-5 rounded-full bg-gray-100">
                          <Circle className="w-3 h-3 text-gray-400" />
                        </div>
                        <span className="text-gray-700 font-medium">{project.taskCount}</span>
                        <span className="text-gray-400 text-xs">total</span>
                      </div>
                    </div>
                  </div>

                  {/* Created Date */}
                  <div className="flex items-center gap-2 text-xs text-gray-500 mb-4">
                    <Calendar className="w-3.5 h-3.5" />
                    <span>Created {new Date(project.createdAt).toLocaleDateString('en-US', { 
                      month: 'short', 
                      day: 'numeric', 
                      year: 'numeric' 
                    })}</span>
                  </div>

                  {/* Quick Tasks Preview */}
                  <QuickTasksPreview project={project} />

                  {/* View Details Button */}
                  <button 
                    onClick={() => navigate(`/projects/${project.id}`)}
                    className="w-full mt-4 text-center text-primary hover:text-primary/80 font-semibold py-2.5 rounded-lg border-2 border-primary/20 hover:border-primary/40 hover:bg-primary/5 transition-all duration-200 flex items-center justify-center gap-2"
                  >
                    <span>View Project</span>
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                    </svg>
                  </button>
                </div>
              ))}
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="flex items-center justify-center gap-3 mt-8">
                <button
                  onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                  className="px-5 py-2.5 border-2 border-gray-200 rounded-lg hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed font-medium text-gray-700 transition disabled:hover:bg-white"
                >
                  ← Previous
                </button>
                <span className="px-5 py-2.5 text-gray-700 font-medium bg-gray-50 rounded-lg border-2 border-gray-200">
                  Page {currentPage} of {totalPages}
                </span>
                <button
                  onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                  className="px-5 py-2.5 border-2 border-gray-200 rounded-lg hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed font-medium text-gray-700 transition disabled:hover:bg-white"
                >
                  Next →
                </button>
              </div>
            )}
          </>
        )}
      </div>

      {/* Modals */}
      {isCreateModalOpen && (
        <CreateProjectModal onClose={() => setIsCreateModalOpen(false)} />
      )}
      {editingProject && (
        <EditProjectModal
          project={editingProject}
          onClose={() => setEditingProject(null)}
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
