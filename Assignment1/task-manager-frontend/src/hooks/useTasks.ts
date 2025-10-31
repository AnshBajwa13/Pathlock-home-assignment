import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { taskApi } from '../services/taskApi';
import { taskStorage } from '../services/taskStorage';
import type { CreateTaskRequest, UpdateTaskRequest, Task } from '../types/task';

/**
 * Custom hook for task operations using React Query
 * 
 * Benefits:
 * - Automatic caching and background refetching
 * - Optimistic updates for better UX
 * - Error handling and retry logic
 * - localStorage synchronization
 */
export function useTasks() {
  const queryClient = useQueryClient();

  // Fetch all tasks
  const { data: tasks = [], isLoading, error, refetch } = useQuery({
    queryKey: ['tasks'],
    queryFn: async () => {
      try {
        const data = await taskApi.getAll();
        console.log('Fetched tasks from API:', data);
        // Sync with localStorage
        taskStorage.save(data);
        return data;
      } catch (error) {
        // Fallback to localStorage if API fails
        console.warn('API failed, loading from localStorage', error);
        return taskStorage.load();
      }
    },
    staleTime: 0, // Always refetch when needed
    retry: 2,
  });

  // Create task mutation
  const createMutation = useMutation({
    mutationFn: (request: CreateTaskRequest) => taskApi.create(request),
    onSuccess: () => {
      // Invalidate and refetch tasks
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    },
    onError: (error) => {
      console.error('Failed to create task:', error);
      alert('Failed to create task. Please try again.');
    },
  });

  // Update task mutation
  const updateMutation = useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateTaskRequest }) =>
      taskApi.update(id, request),
    onMutate: async ({ id, request }) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ queryKey: ['tasks'] });

      // Snapshot previous value
      const previous = queryClient.getQueryData<Task[]>(['tasks']);

      // Optimistically update
      queryClient.setQueryData<Task[]>(['tasks'], (old = []) => {
        const updated = old.map((task) =>
          task.id === id
            ? { ...task, description: request.description, isCompleted: request.isCompleted }
            : task
        );
        taskStorage.save(updated);
        return updated;
      });

      return { previous };
    },
    onError: (_err, _variables, context) => {
      // Rollback on error
      if (context?.previous) {
        queryClient.setQueryData(['tasks'], context.previous);
        taskStorage.save(context.previous);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    },
  });

  // Delete task mutation
  const deleteMutation = useMutation({
    mutationFn: (id: string) => taskApi.delete(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: ['tasks'] });

      const previous = queryClient.getQueryData<Task[]>(['tasks']);

      queryClient.setQueryData<Task[]>(['tasks'], (old = []) => {
        const updated = old.filter((task) => task.id !== id);
        taskStorage.save(updated);
        return updated;
      });

      return { previous };
    },
    onError: (_err, _id, context) => {
      if (context?.previous) {
        queryClient.setQueryData(['tasks'], context.previous);
        taskStorage.save(context.previous);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    },
  });

  // Toggle completion mutation
  const toggleMutation = useMutation({
    mutationFn: (id: string) => taskApi.toggleCompletion(id),
    onMutate: async (id) => {
      await queryClient.cancelQueries({ queryKey: ['tasks'] });

      const previous = queryClient.getQueryData<Task[]>(['tasks']);

      queryClient.setQueryData<Task[]>(['tasks'], (old = []) => {
        const updated = old.map((task) =>
          task.id === id
            ? {
                ...task,
                isCompleted: !task.isCompleted,
                completedAt: !task.isCompleted ? new Date().toISOString() : null,
              }
            : task
        );
        taskStorage.save(updated);
        return updated;
      });

      return { previous };
    },
    onError: (_err, _id, context) => {
      if (context?.previous) {
        queryClient.setQueryData(['tasks'], context.previous);
        taskStorage.save(context.previous);
      }
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    },
  });

  // Clear completed tasks
  const clearCompleted = () => {
    queryClient.setQueryData<Task[]>(['tasks'], (old = []) => {
      const updated = old.filter((task) => !task.isCompleted);
      taskStorage.save(updated);
      
      // Delete from API (fire and forget)
      old
        .filter((task) => task.isCompleted)
        .forEach((task) => {
          taskApi.delete(task.id).catch(console.error);
        });
      
      return updated;
    });
  };

  return {
    tasks,
    isLoading,
    error,
    refetch,
    createTask: createMutation,
    updateTask: updateMutation,
    deleteTask: deleteMutation,
    toggleTask: toggleMutation,
    clearCompleted,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isDeleting: deleteMutation.isPending,
    isToggling: toggleMutation.isPending,
  };
}
