import axios from 'axios';
import type { Task, CreateTaskRequest, UpdateTaskRequest } from '../types/task';

// API base URL - matches our backend
const API_BASE_URL = 'http://localhost:5039/api/v1';

// Create axios instance with base configuration
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Task API service
 * Provides methods to interact with the backend API
 */
export const taskApi = {
  /**
   * Get all tasks
   */
  async getAll(): Promise<Task[]> {
    const response = await api.get<Task[]>('/tasks');
    return response.data;
  },

  /**
   * Get a single task by ID
   */
  async getById(id: string): Promise<Task> {
    const response = await api.get<Task>(`/tasks/${id}`);
    return response.data;
  },

  /**
   * Create a new task
   */
  async create(request: CreateTaskRequest): Promise<Task> {
    const response = await api.post<Task>('/tasks', request);
    return response.data;
  },

  /**
   * Update an existing task
   */
  async update(id: string, request: UpdateTaskRequest): Promise<Task> {
    const response = await api.put<Task>(`/tasks/${id}`, request);
    return response.data;
  },

  /**
   * Delete a task
   */
  async delete(id: string): Promise<void> {
    await api.delete(`/tasks/${id}`);
  },

  /**
   * Toggle task completion status
   */
  async toggleCompletion(id: string): Promise<Task> {
    const response = await api.patch<Task>(`/tasks/${id}/toggle`);
    return response.data;
  },
};
