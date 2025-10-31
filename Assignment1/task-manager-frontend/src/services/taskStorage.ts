import type { Task } from '../types/task';

const STORAGE_KEY = 'task-manager-tasks';

/**
 * LocalStorage utility for persisting tasks
 * This provides offline persistence as an enhancement
 */
export const taskStorage = {
  /**
   * Load tasks from localStorage
   */
  load(): Task[] {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      return stored ? JSON.parse(stored) : [];
    } catch (error) {
      console.error('Failed to load tasks from localStorage:', error);
      return [];
    }
  },

  /**
   * Save tasks to localStorage
   */
  save(tasks: Task[]): void {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
    } catch (error) {
      console.error('Failed to save tasks to localStorage:', error);
    }
  },

  /**
   * Clear all tasks from localStorage
   */
  clear(): void {
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch (error) {
      console.error('Failed to clear tasks from localStorage:', error);
    }
  },
};
