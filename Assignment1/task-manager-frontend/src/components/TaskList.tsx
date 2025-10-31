import { TaskItem } from './TaskItem';
import { ClipboardCheck } from 'lucide-react';
import type { Task } from '../types/task';

interface TaskListProps {
  tasks: Task[];
  isLoading: boolean;
  onToggle: (id: string) => Promise<void>;
  onDelete: (id: string) => Promise<void>;
  onUpdate: (id: string, description: string, isCompleted: boolean) => Promise<void>;
}

export function TaskList({ tasks, isLoading, onToggle, onDelete, onUpdate }: TaskListProps) {
  if (isLoading) {
    return (
      <div className="space-y-3">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-20 bg-gray-100 rounded-lg animate-pulse" />
        ))}
      </div>
    );
  }

  if (tasks.length === 0) {
    return (
      <div className="text-center py-16">
        <ClipboardCheck className="w-20 h-20 text-[#24B770] mx-auto mb-4" />
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          No tasks yet
        </h3>
        <p className="text-gray-500">
          Get started by adding your first task above
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      {tasks.map((task) => (
        <TaskItem key={task.id} task={task} onToggle={onToggle} onDelete={onDelete} onUpdate={onUpdate} />
      ))}
    </div>
  );
}
