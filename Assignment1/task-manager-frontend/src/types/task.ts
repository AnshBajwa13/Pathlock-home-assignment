/**
 * Task model matching the backend API response
 */
export interface Task {
  id: string;
  description: string;
  isCompleted: boolean;
  createdAt: string;
  completedAt: string | null;
}

/**
 * Request model for creating a new task
 */
export interface CreateTaskRequest {
  description: string;
}

/**
 * Request model for updating a task
 */
export interface UpdateTaskRequest {
  description: string;
  isCompleted: boolean;
}

/**
 * Filter options for task list
 */
export type TaskFilter = 'all' | 'active' | 'completed';
