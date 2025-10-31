import { Check, X, Trash2, Edit2 } from 'lucide-react';
import { useState } from 'react';
import type { Task } from '../types/task';

interface TaskItemProps {
  task: Task;
  onToggle: (id: string) => Promise<void>;
  onDelete: (id: string) => Promise<void>;
  onUpdate: (id: string, description: string, isCompleted: boolean) => Promise<void>;
}

export function TaskItem({ task, onToggle, onDelete, onUpdate }: TaskItemProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState(task.description);
  const [isLoading, setIsLoading] = useState(false);

  const handleToggle = async () => {
    setIsLoading(true);
    try {
      await onToggle(task.id);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async () => {
    if (window.confirm('Delete this task?')) {
      setIsLoading(true);
      try {
        await onDelete(task.id);
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleUpdate = async () => {
    if (editValue.trim().length < 3) {
      alert('Task must be at least 3 characters');
      return;
    }
    setIsLoading(true);
    try {
      await onUpdate(task.id, editValue.trim(), task.isCompleted);
      setIsEditing(false);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    setEditValue(task.description);
    setIsEditing(false);
  };

  return (
    <div
      className={`flex items-center gap-3 p-4 border rounded-lg transition-all duration-200
                 ${task.isCompleted
                   ? 'bg-gray-50 border-gray-200'
                   : 'bg-white border-gray-300 shadow-sm hover:shadow-md'
                 }`}
    >
      {/* Checkbox */}
      <button
        onClick={handleToggle}
        disabled={isLoading}
        className={`flex-shrink-0 w-6 h-6 rounded border-2 flex items-center justify-center
                   transition-colors duration-200
                   ${task.isCompleted
                     ? 'bg-[#24B770] border-[#24B770]'
                     : 'border-gray-300 hover:border-[#24B770]'
                   }
                   disabled:opacity-50`}
      >
        {task.isCompleted && <Check className="w-4 h-4 text-white" />}
      </button>
      {/* Task content */}
      <div className="flex-1 min-w-0">
        {isEditing ? (
          <input
            type="text"
            value={editValue}
            onChange={(e) => setEditValue(e.target.value)}
            className="w-full px-2 py-1 border border-gray-300 rounded
                     bg-white text-gray-900
                     focus:ring-2 focus:ring-[#24B770] focus:border-[#24B770]"
            autoFocus
            onKeyDown={(e) => {
              if (e.key === 'Enter') handleUpdate();
              if (e.key === 'Escape') handleCancel();
            }}
          />
        ) : (
          <div>
            <p
              className={`text-base transition-all duration-200
                         ${task.isCompleted
                           ? 'line-through text-gray-500'
                           : 'text-gray-900'
                         }`}
            >
              {task.description}
            </p>
            <p className="text-xs text-gray-500 mt-1">
              Created: {new Date(task.createdAt).toLocaleString()}
              {task.completedAt && (
                <> • Completed: {new Date(task.completedAt).toLocaleString()}</>
              )}
            </p>
          </div>
        )}
      </div>
      {/* Action buttons */}
      <div className="flex gap-2">
        {isEditing ? (
          <>
            <button
              onClick={handleUpdate}
              disabled={isLoading}
              className="p-2 text-[#24B770] hover:bg-green-50 rounded transition-colors"
              title="Save"
            >
              <Check className="w-5 h-5" />
            </button>
            <button
              onClick={handleCancel}
              className="p-2 text-gray-600 hover:bg-gray-100 rounded transition-colors"
              title="Cancel"
            >
              <X className="w-5 h-5" />
            </button>
          </>
        ) : (
          <>
            <button
              onClick={() => setIsEditing(true)}
              disabled={isLoading}
              className="p-2 text-[#0F1555] hover:bg-blue-50 rounded transition-colors
                       disabled:opacity-50"
              title="Edit"
            >
              <Edit2 className="w-5 h-5" />
            </button>
            <button
              onClick={handleDelete}
              disabled={isLoading}
              className="p-2 text-red-600 hover:bg-red-50 rounded transition-colors
                       disabled:opacity-50"
              title="Delete"
            >
              <Trash2 className="w-5 h-5" />
            </button>
          </>
        )}
      </div>
    </div>
  );
}
