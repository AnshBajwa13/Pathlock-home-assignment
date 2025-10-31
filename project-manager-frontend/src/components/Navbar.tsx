import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../lib/authStore';
import { LogOut } from 'lucide-react';

export function Navbar() {
  const navigate = useNavigate();
  const { user, logout } = useAuthStore();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-secondary text-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo/Brand */}
          <div className="flex items-center gap-3">
            {/* PathLock Logo */}
            <svg 
              width="40" 
              height="40" 
              viewBox="0 0 120 120" 
              fill="none"
              className="flex-shrink-0"
            >
              {/* LEFT VERTICAL STROKE */}
              <line
                x1="15"
                y1="60"
                x2="15"
                y2="95"
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
              />
              
              {/* OUTER hexagon ring - OPEN at bottom left - WIDER */}
              <path 
                d="
                  M 15 60
                  L 15 25
                  L 55 5
                  L 95 25
                  L 95 75
                  L 55 95
                  L 35 82
                "
                stroke="#24B770"
                strokeWidth="9"
                strokeLinecap="round"
                fill="none"
              />
              
              {/* Small vertical stroke at bottom-right */}
              <line
                x1="35"
                y1="82"
                x2="35"
                y2="95"
                stroke="#24B770"
                strokeWidth="10"
                strokeLinecap="round"
              />
              
              {/* INNER hexagon ring - OPEN at bottom left - WIDER */}
              <path 
                d="
                  M 28 58
                  L 28 35
                  L 55 20
                  L 82 35
                  L 82 65
                  L 55 80
                  L 42 73
                "
                stroke="#24B770"
                strokeWidth="8"
                strokeLinecap="round"
                fill="none"
              />
            </svg>
            <div>
              <h1 className="text-xl font-bold">PathLock</h1>
              <p className="text-xs text-gray-300">Project Manager</p>
            </div>
          </div>

          {/* User Info & Logout */}
          <div className="flex items-center gap-4">
            <div className="text-right hidden sm:block">
              <p className="text-sm font-medium">{user?.fullName}</p>
              <p className="text-xs text-gray-300">{user?.email}</p>
            </div>
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 bg-white/10 hover:bg-white/20 px-4 py-2 rounded-lg transition"
            >
              <LogOut className="w-4 h-4" />
              <span className="hidden sm:inline">Logout</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}
