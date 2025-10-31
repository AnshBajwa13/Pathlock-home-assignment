import { useState, useMemo } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { TaskForm } from './components/TaskForm';
import { TaskList } from './components/TaskList';
import { FilterBar } from './components/FilterBar';
import { useTasks } from './hooks/useTasks';
import { Trash2, Search } from 'lucide-react';
import type { TaskFilter } from './types/task';

// Create a client for React Query
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60, // 1 minute
      refetchOnWindowFocus: false,
    },
  },
});

/**
 * Inner App component (needs QueryClientProvider context)
 */
function TaskManagerApp() {
  const [filter, setFilter] = useState<TaskFilter>('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedTaskIds, setSelectedTaskIds] = useState<Set<string>>(new Set());
  
  const {
    tasks,
    isLoading,
    createTask,
    updateTask,
    deleteTask,
    toggleTask,
    clearCompleted,
  } = useTasks();

  // Filter tasks based on current filter and search
  const filteredTasks = useMemo(() => {
    let result = tasks;
    
    // Apply search filter
    if (searchQuery.trim()) {
      result = result.filter((task) =>
        task.description.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }
    
    // Apply status filter
    switch (filter) {
      case 'active':
        return result.filter((task) => !task.isCompleted);
      case 'completed':
        return result.filter((task) => task.isCompleted);
      default:
        return result;
    }
  }, [tasks, filter, searchQuery]);

  // Calculate task counts
  const taskCounts = useMemo(() => ({
    all: tasks.length,
    active: tasks.filter((task) => !task.isCompleted).length,
    completed: tasks.filter((task) => task.isCompleted).length,
  }), [tasks]);

  const handleCreateTask = async (description: string) => {
    await createTask.mutateAsync({ description });
  };

  const handleToggleTask = async (id: string) => {
    await toggleTask.mutateAsync(id);
  };

  const handleUpdateTask = async (id: string, description: string, isCompleted: boolean) => {
    await updateTask.mutateAsync({ id, request: { description, isCompleted } });
  };

  const handleDeleteTask = async (id: string) => {
    await deleteTask.mutateAsync(id);
  };

  const handleClearCompleted = () => {
    if (taskCounts.completed === 0) return;
    
    if (window.confirm(`Are you sure you want to delete ${taskCounts.completed} completed task(s)?`)) {
      clearCompleted();
    }
  };

  const handleSelectAll = () => {
    if (selectedTaskIds.size === filteredTasks.length) {
      setSelectedTaskIds(new Set());
    } else {
      setSelectedTaskIds(new Set(filteredTasks.map(t => t.id)));
    }
  };

  const handleDeleteSelected = async () => {
    if (selectedTaskIds.size === 0) return;
    
    if (window.confirm(`Are you sure you want to delete ${selectedTaskIds.size} task(s)?`)) {
      for (const id of selectedTaskIds) {
        await deleteTask.mutateAsync(id);
      }
      setSelectedTaskIds(new Set());
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8 max-w-4xl">
        {/* Header with PathLock Branding */}
        <div className="text-center mb-8">
          <div className="flex flex-col md:flex-row items-center justify-center gap-4 mb-4">
            {/* PathLock Logo - FINAL PERFECT VERSION */}
            <svg 
              width="64" 
              height="64" 
              viewBox="0 0 120 120" 
              fill="none"
              className="flex-shrink-0"
            >
              {/* LEFT VERTICAL STROKE - The "P" bar that goes DOWN from vertex */}
              <line
                x1="20"
                y1="60"
                x2="20"
                y2="95"
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
              />
              
              {/* OUTER hexagon ring - OPEN at bottom left */}
              <path 
                d="
                  M 20 60
                  L 20 30
                  L 50 10
                  L 80 30
                  L 80 70
                  L 50 90
                  L 35 82
                "
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
                fill="none"
              />
              
              {/* Small vertical stroke at bottom-right (6th edge extension) */}
              <line
                x1="35"
                y1="82"
                x2="35"
                y2="95"
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
              />
              
              {/* INNER hexagon ring - OPEN at bottom left (smaller) */}
              <path 
                d="
                  M 32 58
                  L 32 38
                  L 50 26
                  L 68 38
                  L 68 62
                  L 50 74
                  L 42 69
                "
                stroke="#24B770"
                strokeWidth="8"
                strokeLinecap="round"
                fill="none"
              />
            </svg>
            
            <h1 className="text-2xl md:text-3xl font-bold text-gray-900">
              Task Manager
            </h1>
          </div>
          
          <p className="text-sm md:text-base text-gray-600">
            Enterprise-grade task management with compliance-focused design
          </p>
        </div>

        {/* Task Form */}
        <div className="mb-6">
          <TaskForm 
            onSubmit={handleCreateTask} 
            isLoading={createTask.isPending || false}
          />
        </div>

        {/* Search Bar */}
        {tasks.length > 0 && (
          <div className="mb-4">
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-3">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Search tasks..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg
                           bg-white text-gray-900
                           focus:ring-2 focus:ring-[#24B770] focus:border-[#24B770]
                           placeholder-gray-400 transition-all"
                />
              </div>
            </div>
          </div>
        )}

        {/* Filter Bar and Bulk Actions */}
        {tasks.length > 0 && (
          <div className="mb-6 space-y-3">
            <FilterBar
              currentFilter={filter}
              onFilterChange={setFilter}
              taskCounts={taskCounts}
            />
            
            {/* Bulk Actions */}
            {filteredTasks.length > 0 && (
              <div className="flex items-center justify-between bg-white rounded-lg shadow-sm border border-gray-200 p-3">
                <div className="flex items-center gap-3">
                  <button
                    onClick={handleSelectAll}
                    className="flex items-center gap-2 px-3 py-1.5 text-sm font-medium
                             text-gray-700 hover:text-[#24B770] transition-colors"
                  >
                    <input
                      type="checkbox"
                      checked={selectedTaskIds.size === filteredTasks.length && filteredTasks.length > 0}
                      onChange={handleSelectAll}
                      className="w-4 h-4 rounded border-gray-300 text-[#24B770] 
                               focus:ring-[#24B770] cursor-pointer"
                    />
                    Select All ({filteredTasks.length})
                  </button>
                  {selectedTaskIds.size > 0 && (
                    <span className="text-sm text-gray-600">
                      {selectedTaskIds.size} selected
                    </span>
                  )}
                </div>
                {selectedTaskIds.size > 0 && (
                  <button
                    onClick={handleDeleteSelected}
                    className="flex items-center gap-2 px-4 py-1.5 bg-red-50 text-red-600
                             rounded-lg hover:bg-red-100 transition-colors text-sm font-medium
                             border border-red-200"
                  >
                    <Trash2 className="w-4 h-4" />
                    Delete Selected
                  </button>
                )}
              </div>
            )}
          </div>
        )}

        {/* Task List */}
        <div className="mb-6 bg-white rounded-xl shadow-md border border-gray-200 p-6">
          <TaskList
            tasks={filteredTasks}
            isLoading={isLoading}
            onToggle={handleToggleTask}
            onDelete={handleDeleteTask}
            onUpdate={handleUpdateTask}
          />
        </div>

        {/* Footer Actions */}
        {taskCounts.completed > 0 && (
          <div className="flex justify-center">
            <button
              onClick={handleClearCompleted}
              className="flex items-center gap-2 px-4 py-2 bg-red-50 
                       text-red-600 rounded-lg hover:bg-red-100 
                       transition-colors border border-red-200"
            >
              <Trash2 className="w-4 h-4" />
              Clear {taskCounts.completed} completed task{taskCounts.completed !== 1 ? 's' : ''}
            </button>
          </div>
        )}

        {/* Stats Footer */}
        <div className="mt-8 text-center text-sm text-gray-600">
          {taskCounts.active} active • {taskCounts.completed} completed
        </div>
      </div>
    </div>
  );
}

/**
 * Root App component with QueryClientProvider
 */
function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <TaskManagerApp />
    </QueryClientProvider>
  );
}

export default App;
