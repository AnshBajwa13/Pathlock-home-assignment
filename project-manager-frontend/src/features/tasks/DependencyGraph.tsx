import { useEffect, useRef, useState } from 'react';
import { X, ZoomIn, ZoomOut, Maximize2 } from 'lucide-react';
import type { Task } from '../../types';

interface DependencyGraphProps {
  tasks: Task[];
  onClose: () => void;
}

interface Node {
  id: string;
  title: string;
  x: number;
  y: number;
  estimatedHours?: number;
  isCompleted: boolean;
}

interface Edge {
  from: string;
  to: string;
}

export function DependencyGraph({ tasks, onClose }: DependencyGraphProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [zoom, setZoom] = useState(1);
  const [nodes, setNodes] = useState<Node[]>([]);
  const [edges, setEdges] = useState<Edge[]>([]);
  const [hoveredNode, setHoveredNode] = useState<string | null>(null);

  // Calculate node positions using a simple hierarchical layout
  useEffect(() => {
    if (tasks.length === 0) return;

    // Build adjacency list for topological sort
    const adjList = new Map<string, string[]>();
    const inDegree = new Map<string, number>();

    tasks.forEach(task => {
      adjList.set(task.id, []);
      inDegree.set(task.id, 0);
    });

    const edgesList: Edge[] = [];

    tasks.forEach(task => {
      if (task.dependencyIds && task.dependencyIds.length > 0) {
        task.dependencyIds.forEach(depId => {
          if (adjList.has(depId)) {
            adjList.get(depId)!.push(task.id);
            inDegree.set(task.id, (inDegree.get(task.id) || 0) + 1);
            edgesList.push({ from: depId, to: task.id });
          }
        });
      }
    });

    setEdges(edgesList);

    // Topological sort to determine levels
    const levels = new Map<string, number>();
    const queue: string[] = [];

    // Start with nodes that have no dependencies
    tasks.forEach(task => {
      if ((inDegree.get(task.id) || 0) === 0) {
        queue.push(task.id);
        levels.set(task.id, 0);
      }
    });

    while (queue.length > 0) {
      const current = queue.shift()!;
      const currentLevel = levels.get(current) || 0;

      adjList.get(current)?.forEach(neighbor => {
        const newDegree = (inDegree.get(neighbor) || 0) - 1;
        inDegree.set(neighbor, newDegree);

        const neighborLevel = levels.get(neighbor) || 0;
        levels.set(neighbor, Math.max(neighborLevel, currentLevel + 1));

        if (newDegree === 0) {
          queue.push(neighbor);
        }
      });
    }

    // Group nodes by level
    const levelGroups = new Map<number, string[]>();
    tasks.forEach(task => {
      const level = levels.get(task.id) || 0;
      if (!levelGroups.has(level)) {
        levelGroups.set(level, []);
      }
      levelGroups.get(level)!.push(task.id);
    });

    // Calculate positions
    const horizontalSpacing = 250;
    const verticalSpacing = 150;

    const nodesList: Node[] = [];

    levelGroups.forEach((taskIds, level) => {
      const yOffset = level * verticalSpacing + 100;
      const totalWidth = (taskIds.length - 1) * horizontalSpacing;
      const startX = 400 - totalWidth / 2; // Center horizontally

      taskIds.forEach((taskId, index) => {
        const task = tasks.find(t => t.id === taskId);
        if (task) {
          nodesList.push({
            id: task.id,
            title: task.title,
            x: startX + index * horizontalSpacing,
            y: yOffset,
            estimatedHours: task.estimatedHours ?? undefined,
            isCompleted: task.isCompleted,
          });
        }
      });
    });

    setNodes(nodesList);
  }, [tasks]);

  // Draw the graph
  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    // Set canvas size
    canvas.width = 800;
    canvas.height = 600;

    // Clear canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Apply zoom
    ctx.save();
    ctx.scale(zoom, zoom);

    // Draw edges first
    edges.forEach(edge => {
      const fromNode = nodes.find(n => n.id === edge.from);
      const toNode = nodes.find(n => n.id === edge.to);

      if (fromNode && toNode) {
        ctx.beginPath();
        ctx.moveTo(fromNode.x + 90, fromNode.y + 40); // Center bottom of from node
        
        // Draw curved arrow
        const controlX = (fromNode.x + toNode.x) / 2 + 90;
        const controlY = (fromNode.y + toNode.y) / 2 + 40;
        
        ctx.quadraticCurveTo(controlX, controlY, toNode.x + 90, toNode.y);
        
        ctx.strokeStyle = '#9333ea';
        ctx.lineWidth = 2;
        ctx.stroke();

        // Draw arrowhead
        const arrowSize = 10;
        const angle = Math.atan2(toNode.y - controlY, (toNode.x + 90) - controlX);
        
        ctx.beginPath();
        ctx.moveTo(toNode.x + 90, toNode.y);
        ctx.lineTo(
          toNode.x + 90 - arrowSize * Math.cos(angle - Math.PI / 6),
          toNode.y - arrowSize * Math.sin(angle - Math.PI / 6)
        );
        ctx.lineTo(
          toNode.x + 90 - arrowSize * Math.cos(angle + Math.PI / 6),
          toNode.y - arrowSize * Math.sin(angle + Math.PI / 6)
        );
        ctx.closePath();
        ctx.fillStyle = '#9333ea';
        ctx.fill();
      }
    });

    // Draw nodes
    nodes.forEach(node => {
      const isHovered = hoveredNode === node.id;
      
      // Draw shadow for hover effect
      if (isHovered) {
        ctx.shadowColor = 'rgba(0, 0, 0, 0.2)';
        ctx.shadowBlur = 10;
        ctx.shadowOffsetX = 0;
        ctx.shadowOffsetY = 4;
      }

      // Draw node rectangle
      ctx.fillStyle = node.isCompleted ? '#f0fdf4' : '#ffffff';
      ctx.strokeStyle = node.isCompleted ? '#22c55e' : (isHovered ? '#0F1555' : '#d1d5db');
      ctx.lineWidth = isHovered ? 3 : 2;
      
      const nodeWidth = 180;
      const nodeHeight = 70;
      const radius = 8;

      // Rounded rectangle
      ctx.beginPath();
      ctx.moveTo(node.x + radius, node.y);
      ctx.lineTo(node.x + nodeWidth - radius, node.y);
      ctx.quadraticCurveTo(node.x + nodeWidth, node.y, node.x + nodeWidth, node.y + radius);
      ctx.lineTo(node.x + nodeWidth, node.y + nodeHeight - radius);
      ctx.quadraticCurveTo(node.x + nodeWidth, node.y + nodeHeight, node.x + nodeWidth - radius, node.y + nodeHeight);
      ctx.lineTo(node.x + radius, node.y + nodeHeight);
      ctx.quadraticCurveTo(node.x, node.y + nodeHeight, node.x, node.y + nodeHeight - radius);
      ctx.lineTo(node.x, node.y + radius);
      ctx.quadraticCurveTo(node.x, node.y, node.x + radius, node.y);
      ctx.closePath();
      ctx.fill();
      ctx.stroke();

      // Reset shadow
      ctx.shadowColor = 'transparent';
      ctx.shadowBlur = 0;
      ctx.shadowOffsetX = 0;
      ctx.shadowOffsetY = 0;

      // Draw title
      ctx.fillStyle = '#111827';
      ctx.font = 'bold 14px Inter, system-ui, sans-serif';
      ctx.textAlign = 'center';
      ctx.fillText(
        node.title.length > 20 ? node.title.substring(0, 20) + '...' : node.title,
        node.x + 90,
        node.y + 30
      );

      // Draw estimated hours if available
      if (node.estimatedHours) {
        ctx.fillStyle = '#6b7280';
        ctx.font = '12px Inter, system-ui, sans-serif';
        ctx.fillText(`${node.estimatedHours}h`, node.x + 90, node.y + 50);
      }

      // Draw completion checkmark
      if (node.isCompleted) {
        ctx.fillStyle = '#22c55e';
        ctx.font = 'bold 16px Inter, system-ui, sans-serif';
        ctx.fillText('✓', node.x + 165, node.y + 20);
      }
    });

    ctx.restore();
  }, [nodes, edges, zoom, hoveredNode]);

  // Handle mouse hover
  const handleMouseMove = (e: React.MouseEvent<HTMLCanvasElement>) => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const rect = canvas.getBoundingClientRect();
    const x = (e.clientX - rect.left) / zoom;
    const y = (e.clientY - rect.top) / zoom;

    const hoveredNode = nodes.find(
      node => x >= node.x && x <= node.x + 180 && y >= node.y && y <= node.y + 70
    );

    setHoveredNode(hoveredNode?.id || null);
  };

  return (
    <div className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-2xl shadow-2xl max-w-4xl w-full max-h-[90vh] flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-purple-100 rounded-lg">
              <Maximize2 className="w-5 h-5 text-purple-600" />
            </div>
            <div>
              <h2 className="text-xl font-bold text-gray-900">Task Dependency Graph</h2>
              <p className="text-sm text-gray-600 mt-1">Visual representation of task dependencies</p>
            </div>
          </div>
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-100 rounded-lg transition"
          >
            <X className="w-5 h-5 text-gray-500" />
          </button>
        </div>

        {/* Canvas Container */}
        <div className="flex-1 p-6 overflow-auto">
          <div className="flex justify-center mb-4 gap-2">
            <button
              onClick={() => setZoom(Math.max(0.5, zoom - 0.1))}
              className="flex items-center gap-2 px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-lg transition"
            >
              <ZoomOut className="w-4 h-4" />
              Zoom Out
            </button>
            <span className="px-4 py-2 bg-gray-100 rounded-lg font-medium">
              {Math.round(zoom * 100)}%
            </span>
            <button
              onClick={() => setZoom(Math.min(2, zoom + 0.1))}
              className="flex items-center gap-2 px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-lg transition"
            >
              <ZoomIn className="w-4 h-4" />
              Zoom In
            </button>
          </div>

          {tasks.length === 0 ? (
            <div className="flex items-center justify-center h-96 text-gray-500">
              <p>No tasks to display</p>
            </div>
          ) : (
            <div className="flex justify-center">
              <canvas
                ref={canvasRef}
                onMouseMove={handleMouseMove}
                className="border border-gray-200 rounded-lg shadow-inner cursor-pointer"
              />
            </div>
          )}
        </div>

        {/* Legend */}
        <div className="p-6 bg-gray-50 border-t border-gray-200 rounded-b-2xl">
          <div className="flex items-center justify-center gap-8 text-sm">
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-white border-2 border-gray-300 rounded"></div>
              <span className="text-gray-700">Incomplete Task</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-4 h-4 bg-green-50 border-2 border-green-500 rounded"></div>
              <span className="text-gray-700">Completed Task</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-8 h-0.5 bg-purple-600"></div>
              <span className="text-gray-700">Dependency Arrow</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
