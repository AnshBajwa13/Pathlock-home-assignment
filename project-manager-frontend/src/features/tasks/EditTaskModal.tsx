import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { tasksApi } from './tasksApi';
import { X } from 'lucide-react';
import type { Task } from '../../types';
import { useState } from 'react';

const taskSchema = z.object({
  title: z.string().min(1, 'Title is required').max(200, 'Title must be less than 200 characters'),
  description: z.string().max(1000, 'Description must be less than 1000 characters').optional(),
  dueDate: z.string().optional().refine((date) => {
    if (!date) return true;
    return new Date(date) >= new Date(new Date().setHours(0, 0, 0, 0));
  }, 'Due date must be today or in the future'),
  estimatedHours: z.string().optional(),
  dependencyIds: z.array(z.string()).optional(),
});

type TaskFormData = z.infer<typeof taskSchema>;

interface EditTaskModalProps {
  task: Task;
  onClose: () => void;
}

export function EditTaskModal({ task, onClose }: EditTaskModalProps) {
  const queryClient = useQueryClient();
  const [selectedDeps, setSelectedDeps] = useState<string[]>(task.dependencyIds || []);

  // Fetch existing tasks for dependencies dropdown
  const { data: existingTasks = [] } = useQuery<Task[]>({
    queryKey: ['tasks', task.projectId],
    queryFn: () => tasksApi.getTasksByProject(task.projectId),
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<TaskFormData>({
    resolver: zodResolver(taskSchema),
    defaultValues: {
      title: task.title,
      description: task.description || '',
      dueDate: task.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : '',
      estimatedHours: task.estimatedHours?.toString() || '',
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: TaskFormData) => {
      const taskData = {
        id: task.id,
        title: data.title,
        description: data.description || undefined,
        dueDate: data.dueDate || null,
        estimatedHours: data.estimatedHours ? parseFloat(data.estimatedHours) : null,
        dependencyIds: selectedDeps,
      };
      return tasksApi.updateTask(task.id, taskData);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks', task.projectId] });
      queryClient.invalidateQueries({ queryKey: ['project', task.projectId] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      onClose();
    },
  });

  const onSubmit = (data: TaskFormData) => {
    updateMutation.mutate(data);
  };

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-md transform transition-all">
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-5 border-b border-gray-200">
          <h2 className="text-xl font-bold text-gray-900">Edit Task</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition p-1 rounded-lg hover:bg-gray-100"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-5">
          {updateMutation.error && (
            <div className="bg-red-50 border-l-4 border-red-500 text-red-700 px-4 py-3 rounded-lg text-sm font-medium">
              Failed to update task. Please try again.
            </div>
          )}

          {/* Title Field */}
          <div>
            <label htmlFor="title" className="block text-sm font-semibold text-gray-700 mb-2">
              Task Title <span className="text-red-500">*</span>
            </label>
            <input
              {...register('title')}
              type="text"
              id="title"
              className="w-full px-4 py-2.5 border-2 border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/50 focus:border-primary transition"
              placeholder="Enter task title"
            />
            {errors.title && (
              <p className="mt-1.5 text-sm text-red-600 flex items-center gap-1">
                <span className="text-red-500">●</span>
                {errors.title.message}
              </p>
            )}
          </div>

          {/* Description Field */}
          <div>
            <label htmlFor="description" className="block text-sm font-semibold text-gray-700 mb-2">
              Description (Optional)
            </label>
            <textarea
              {...register('description')}
              id="description"
              rows={3}
              className="w-full px-4 py-2.5 border-2 border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/50 focus:border-primary transition resize-none"
              placeholder="Add details about this task (optional)"
            />
            {errors.description && (
              <p className="mt-1.5 text-sm text-red-600 flex items-center gap-1">
                <span className="text-red-500">●</span>
                {errors.description.message}
              </p>
            )}
          </div>

          {/* Due Date Field */}
          <div>
            <label htmlFor="dueDate" className="block text-sm font-semibold text-gray-700 mb-2">
              Due Date (Optional)
            </label>
            <input
              {...register('dueDate')}
              type="date"
              id="dueDate"
              min={new Date().toISOString().split('T')[0]}
              className="w-full px-4 py-2.5 border-2 border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/50 focus:border-primary transition"
            />
            {errors.dueDate && (
              <p className="mt-1.5 text-sm text-red-600 flex items-center gap-1">
                <span className="text-red-500">●</span>
                {errors.dueDate.message}
              </p>
            )}
          </div>

          {/* Estimated Hours Field */}
          <div>
            <label htmlFor="estimatedHours" className="block text-sm font-semibold text-gray-700 mb-2">
              Estimated Hours (Optional)
            </label>
            <input
              {...register('estimatedHours')}
              type="number"
              id="estimatedHours"
              min="0.5"
              step="0.5"
              className="w-full px-4 py-2.5 border-2 border-gray-200 rounded-lg focus:ring-2 focus:ring-primary/50 focus:border-primary transition"
              placeholder="e.g., 2.5"
            />
            <p className="mt-1 text-xs text-gray-500">
              Leave empty to use default (1 hour)
            </p>
          </div>

          {/* Dependencies Field */}
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Dependencies (Optional)
            </label>
            {existingTasks.filter(t => t.id !== task.id).length > 0 ? (
              <div className="max-h-32 overflow-y-auto border-2 border-gray-200 rounded-lg p-3 space-y-2">
                {existingTasks
                  .filter(t => t.id !== task.id) // Can't depend on itself
                  .map((t) => (
                    <label key={t.id} className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 p-1 rounded">
                      <input
                        type="checkbox"
                        checked={selectedDeps.includes(t.id)}
                        onChange={(e) => {
                          if (e.target.checked) {
                            setSelectedDeps([...selectedDeps, t.id]);
                          } else {
                            setSelectedDeps(selectedDeps.filter(id => id !== t.id));
                          }
                        }}
                        className="w-4 h-4 text-primary border-gray-300 rounded focus:ring-primary"
                      />
                      <span className="text-sm text-gray-700">{t.title}</span>
                    </label>
                  ))}
              </div>
            ) : (
              <p className="text-sm text-gray-500 italic">No other tasks available</p>
            )}
            <p className="mt-1 text-xs text-gray-500">
              Selected: {selectedDeps.length}
            </p>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 px-4 py-2.5 border-2 border-gray-200 rounded-lg hover:bg-gray-50 transition font-semibold text-gray-700"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={updateMutation.isPending}
              className="flex-1 bg-primary hover:bg-primary/90 text-white px-4 py-2.5 rounded-lg transition font-semibold disabled:opacity-50 disabled:cursor-not-allowed shadow-sm hover:shadow-md"
            >
              {updateMutation.isPending ? 'Saving...' : 'Save Changes'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
