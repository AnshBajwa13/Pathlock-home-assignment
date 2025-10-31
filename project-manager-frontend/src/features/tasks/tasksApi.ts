import { api } from '../../lib/api';
import type { Task, CreateTaskRequest, UpdateTaskRequest, ScheduleResponse } from '../../types';

export const tasksApi = {
  getTasksByProject: async (projectId: string): Promise<Task[]> => {
    const { data } = await api.get(`/tasks/project/${projectId}`);
    return data;
  },

  createTask: async (projectId: string, taskData: CreateTaskRequest): Promise<Task> => {
    const { data } = await api.post(`/projects/${projectId}/tasks`, taskData);
    return data;
  },

  updateTask: async (taskId: string, taskData: UpdateTaskRequest): Promise<Task> => {
    const { data } = await api.put(`/tasks/${taskId}`, taskData);
    return data;
  },

  toggleTaskCompletion: async (taskId: string): Promise<void> => {
    await api.patch(`/tasks/${taskId}/toggle`);
  },

  deleteTask: async (taskId: string): Promise<void> => {
    await api.delete(`/tasks/${taskId}`);
  },

  generateSchedule: async (projectId: string): Promise<ScheduleResponse> => {
    const { data } = await api.post(`/scheduling/generate/${projectId}`);
    return data;
  },
};
