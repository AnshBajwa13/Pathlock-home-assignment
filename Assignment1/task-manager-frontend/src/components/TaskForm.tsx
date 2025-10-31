import { useState } from 'react';
import type { FormEvent } from 'react';
import { Plus } from 'lucide-react';

interface TaskFormProps {
  onSubmit: (description: string) => Promise<void>;
  isLoading: boolean;
}

/**
 * Task creation form component
 * Features:
 * - Input validation (3-500 characters)
 * - Character counter
 * - Loading state
 * - Error handling
 */
export function TaskForm({ onSubmit, isLoading }: TaskFormProps) {
  const [description, setDescription] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');

    // Validation
    if (description.trim().length < 3) {
      setError('Task description must be at least 3 characters');
      return;
    }

    if (description.trim().length > 500) {
      setError('Task description must be less than 500 characters');
      return;
    }

    try {
      await onSubmit(description.trim());
      setDescription('');
    } catch (err) {
      setError('Failed to create task. Please try again.');
    }
  };

  const charCount = description.length;
  const isValid = charCount >= 3 && charCount <= 500;

  return (
    <form onSubmit={handleSubmit} className="w-full">
      <div className="bg-white rounded-xl shadow-md border border-gray-200 p-6">
        <div className="flex gap-3">
          <div className="flex-1">
            <input
              type="text"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="What needs to be done?"
              className="w-full px-4 py-3 border border-gray-200 rounded-lg
                       bg-white text-gray-900
                       focus:ring-2 focus:ring-[#24B770] focus:border-[#24B770]
                       placeholder-gray-400
                       transition-all"
              disabled={isLoading}
            />
            <div className="flex justify-between items-center mt-2">
              {error ? (
                <p className="text-sm text-red-600">{error}</p>
              ) : (
                <p className="text-sm text-gray-500">
                  {charCount > 0 && (
                    <span className={isValid ? 'text-[#24B770]' : 'text-red-600'}>
                      {charCount}/500 characters
                    </span>
                  )}
                </p>
              )}
            </div>
          </div>
          <button
            type="submit"
            disabled={isLoading || !isValid || description.trim().length === 0}
            className="px-6 py-3 bg-[#24B770] hover:bg-[#1e9960] text-white rounded-lg
                     font-medium transition-colors flex items-center gap-2
                     disabled:opacity-50 disabled:cursor-not-allowed shadow-sm"
          >
            <Plus className="w-5 h-5" />
            {isLoading ? 'Adding...' : 'Add Task'}
          </button>
        </div>
      </div>
    </form>
  );
}
