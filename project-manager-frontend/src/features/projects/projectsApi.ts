import { api } from '../../lib/api';
import type { Project, ProjectDetail, CreateProjectRequest, UpdateProjectRequest, PaginatedList } from '../../types';

export const projectsApi = {
  // Get all projects with pagination
  getProjects: async (pageNumber = 1, pageSize = 10) => {
    const response = await api.get<PaginatedList<Project>>('/projects', {
      params: { pageNumber, pageSize },
    });
    return response.data;
  },

  // Get single project by ID
  getProjectById: async (id: string) => {
    const response = await api.get<ProjectDetail>(`/projects/${id}`);
    return response.data;
  },

  // Create new project
  createProject: async (data: CreateProjectRequest) => {
    const response = await api.post<Project>('/projects', data);
    return response.data;
  },

  // Update existing project
  updateProject: async (id: string, data: UpdateProjectRequest) => {
    const response = await api.put<Project>(`/projects/${id}`, data);
    return response.data;
  },

  // Delete project (soft delete)
  deleteProject: async (id: string) => {
    await api.delete(`/projects/${id}`);
  },
};
