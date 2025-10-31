export interface User {
  userId: string;
  email: string;
  fullName: string;
}

export interface AuthResponse {
  userId: string;
  email: string;
  fullName: string;
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
}

export interface Project {
  id: string;
  title: string;
  description: string;
  taskCount: number;
  completedTaskCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface ProjectDetail {
  id: string;
  title: string;
  description: string;
  userId: string;
  taskCount: number;
  completedTaskCount: number;
  createdAt: string;
  createdBy: string;
  updatedAt: string | null;
  updatedBy: string | null;
  tasks: TaskSummary[];
}

export interface TaskSummary {
  id: string;
  title: string;
  description?: string;
  isCompleted: boolean;
  completedAt: string | null;
  dueDate: string | null;
  estimatedHours?: number | null;
  createdAt: string;
}

export interface Task {
  id: string;
  title: string;
  description?: string;
  isCompleted: boolean;
  completedAt: string | null;
  dueDate: string | null;
  estimatedHours?: number | null;
  dependencyIds?: string[];
  projectId: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateProjectRequest {
  title: string;
  description: string;
}

export interface UpdateProjectRequest {
  id: string;
  title: string;
  description: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  dueDate: string | null;
  estimatedHours?: number | null;
  dependencyIds?: string[];
  projectId: string;
}

export interface UpdateTaskRequest {
  id: string;
  title: string;
  description?: string;
  dueDate: string | null;
  estimatedHours?: number | null;
  dependencyIds?: string[];
}

export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiError {
  message: string | string[];
}

export interface ScheduledTask {
  taskId: string;
  title: string;
  order: number;
  estimatedHours: number | null;
  earliestStart: number;
  latestStart: number;
  earliestFinish: number;
  latestFinish: number;
  slack: number;
  isCritical: boolean;
  dependencies: string[];
  dueDate: string | null;
}

export interface ScheduleResponse {
  scheduledTasks: ScheduledTask[];
  criticalPath: string[];
  totalEstimatedHours: number;
  warnings: string[];
}
