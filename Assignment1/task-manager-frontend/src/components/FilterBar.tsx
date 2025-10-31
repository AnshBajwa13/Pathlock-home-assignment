import type { TaskFilter } from '../types/task';

interface FilterBarProps {
  currentFilter: TaskFilter;
  onFilterChange: (filter: TaskFilter) => void;
  taskCounts: {
    all: number;
    active: number;
    completed: number;
  };
}

/**
 * Filter bar component for task visibility
 * Features:
 * - All/Active/Completed filters
 * - Task count badges
 * - Active state highlighting
 * - Responsive layout
 */
export function FilterBar({ currentFilter, onFilterChange, taskCounts }: FilterBarProps) {
  const filters: Array<{ value: TaskFilter; label: string; count: number }> = [
    { value: 'all', label: 'All', count: taskCounts.all },
    { value: 'active', label: 'Active', count: taskCounts.active },
    { value: 'completed', label: 'Completed', count: taskCounts.completed },
  ];

  return (
    <div className="flex gap-2 p-1 bg-white rounded-lg border border-gray-200 shadow-sm">
      {filters.map((filter) => (
        <button
          key={filter.value}
          onClick={() => onFilterChange(filter.value)}
          className={`flex-1 px-4 py-2.5 rounded-md font-semibold text-sm transition-all duration-200
                     ${currentFilter === filter.value
                       ? 'bg-[#24B770] text-white shadow-md'
                       : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                     }`}
        >
          {filter.label}
          <span
            className={`ml-2 px-2.5 py-0.5 rounded-full text-xs font-bold
                       ${currentFilter === filter.value
                         ? 'bg-white/20 text-white'
                         : 'bg-gray-100 text-gray-600'
                       }`}
          >
            {filter.count}
          </span>
        </button>
      ))}
    </div>
  );
}
