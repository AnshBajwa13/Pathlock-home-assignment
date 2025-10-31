import { useMutation } from '@tanstack/react-query';
import { tasksApi } from './tasksApi';
import { X, Calendar, AlertCircle, CheckCircle2, Clock } from 'lucide-react';
import type { ScheduleResponse } from '../../types';
import { useState } from 'react';

interface ScheduleModalProps {
  projectId: string;
  onClose: () => void;
}

export function ScheduleModal({ projectId, onClose }: ScheduleModalProps) {
  const [scheduleData, setScheduleData] = useState<ScheduleResponse | null>(null);

  const generateMutation = useMutation({
    mutationFn: () => tasksApi.generateSchedule(projectId),
    onSuccess: (data) => {
      setScheduleData(data);
    },
  });

  const handleGenerate = () => {
    generateMutation.mutate();
  };

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-2xl w-full max-w-3xl transform transition-all max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-5 border-b border-gray-200">
          <div className="flex items-center gap-3">
            <Calendar className="w-6 h-6 text-primary" />
            <h2 className="text-xl font-bold text-gray-900">Smart Schedule Generator</h2>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition p-1 rounded-lg hover:bg-gray-100"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 overflow-y-auto flex-1">
          {!scheduleData && !generateMutation.isPending && !generateMutation.error && (
            <div className="text-center py-12">
              <Calendar className="w-16 h-16 text-gray-300 mx-auto mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                Generate Optimized Schedule
              </h3>
              <p className="text-gray-600 mb-6 max-w-md mx-auto">
                Uses Critical Path Method (CPM) and Topological Sort to determine the optimal task execution order based on dependencies and estimated hours.
              </p>
              <button
                onClick={handleGenerate}
                className="bg-primary hover:bg-primary/90 text-white px-6 py-3 rounded-lg transition font-semibold shadow-sm hover:shadow-md"
              >
                Generate Schedule
              </button>
            </div>
          )}

          {generateMutation.isPending && (
            <div className="text-center py-12">
              <div className="animate-spin rounded-full h-12 w-12 border-4 border-primary border-t-transparent mx-auto mb-4"></div>
              <p className="text-gray-600">Analyzing dependencies and calculating critical path...</p>
            </div>
          )}

          {generateMutation.error && (
            <div className="bg-red-50 border-l-4 border-red-500 p-6 rounded-lg">
              <div className="flex items-center gap-3">
                <AlertCircle className="w-6 h-6 text-red-600 flex-shrink-0" />
                <div>
                  <h3 className="font-semibold text-red-900">Failed to generate schedule</h3>
                  <p className="text-red-700 text-sm mt-1">
                    {(generateMutation.error as any)?.response?.data?.message || 'An error occurred. Please try again.'}
                  </p>
                </div>
              </div>
              <button
                onClick={handleGenerate}
                className="mt-4 text-red-700 hover:text-red-900 font-medium"
              >
                Try Again
              </button>
            </div>
          )}

          {scheduleData && (
            <div>
              {/* Warnings */}
              {scheduleData.warnings && scheduleData.warnings.length > 0 && (
                <div className="bg-orange-50 border-l-4 border-orange-500 p-4 rounded-lg mb-6">
                  <div className="flex items-start gap-3">
                    <AlertCircle className="w-5 h-5 text-orange-600 flex-shrink-0 mt-0.5" />
                    <div className="flex-1">
                      <h4 className="font-semibold text-orange-900 mb-2">Warnings</h4>
                      <ul className="list-disc list-inside space-y-1 text-sm text-orange-800">
                        {scheduleData.warnings.map((warning, index) => (
                          <li key={index}>{warning}</li>
                        ))}
                      </ul>
                    </div>
                  </div>
                </div>
              )}

              {/* Schedule Table */}
              <div className="bg-gray-50 rounded-lg p-4 mb-4">
                <div className="flex items-center gap-2 mb-3">
                  <CheckCircle2 className="w-5 h-5 text-green-600" />
                  <h3 className="font-semibold text-gray-900">
                    Optimized Task Order ({scheduleData.scheduledTasks?.length || 0} tasks)
                  </h3>
                </div>
                <div className="bg-white rounded-lg overflow-hidden border border-gray-200">
                  <table className="w-full">
                    <thead className="bg-gray-100 border-b border-gray-200">
                      <tr>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                          Order
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                          Task
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                          Hours
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                          Critical
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">
                          Slack
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                      {scheduleData.scheduledTasks?.map((task) => (
                        <tr key={task.taskId} className="hover:bg-gray-50">
                          <td className="px-4 py-3 text-sm font-medium text-gray-900">
                            #{task.order}
                          </td>
                          <td className="px-4 py-3 text-sm text-gray-900">
                            {task.title}
                          </td>
                          <td className="px-4 py-3 text-sm text-gray-600">
                            <div className="flex items-center gap-1">
                              <Clock className="w-4 h-4 text-gray-400" />
                              {task.estimatedHours || 1}h
                            </div>
                          </td>
                          <td className="px-4 py-3 text-sm">
                            {task.isCritical ? (
                              <span className="inline-flex items-center px-2 py-1 bg-red-100 text-red-700 text-xs font-semibold rounded-full">
                                Critical Path
                              </span>
                            ) : (
                              <span className="text-gray-400">—</span>
                            )}
                          </td>
                          <td className="px-4 py-3 text-sm text-gray-600">
                            {task.slack > 0 ? (
                              <span className="text-green-600 font-medium">{task.slack}h</span>
                            ) : (
                              <span className="text-gray-400">0h</span>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* Info Box */}
              <div className="bg-blue-50 border-l-4 border-blue-500 p-4 rounded-lg">
                <div className="flex items-start gap-3">
                  <AlertCircle className="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" />
                  <div className="text-sm text-blue-800">
                    <p className="font-semibold mb-1">How to read this schedule:</p>
                    <ul className="list-disc list-inside space-y-1">
                      <li><strong>Order:</strong> Suggested execution sequence</li>
                      <li><strong>Critical Path:</strong> Tasks that directly impact project duration</li>
                      <li><strong>Slack Time:</strong> How much delay this task can tolerate without affecting the deadline</li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Footer */}
        <div className="px-6 py-4 border-t border-gray-200 bg-gray-50">
          <button
            onClick={onClose}
            className="w-full px-4 py-2.5 bg-white border-2 border-gray-200 rounded-lg hover:bg-gray-50 transition font-semibold text-gray-700"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
