# PathLock Home Assignment - Comprehensive Analysis



---



### Overview
Built a complete RESTful API with **Clean Architecture**, **CQRS pattern**, and **MediatR** for managing projects and tasks.

### Architecture
- **Domain Layer:** Entities, interfaces, exceptions (10 files)
- **Application Layer:** CQRS commands/queries, validation (45 files)
- **Infrastructure Layer:** DbContext, security services, migrations (9 files)
- **API Layer:** Controllers, configuration (4 files)

### Features Implemented
✅ **Authentication System**
- User registration with password validation
- Login with JWT token generation
- Refresh token mechanism
- BCrypt password hashing (work factor: 12)

✅ **Projects Management (5 endpoints)**
- Create, Read, Update, Delete projects
- Pagination support
- Ownership-based access control
- Soft delete pattern

✅ **Tasks Management (6 endpoints)**
- Create, Read, Update, Delete tasks
- Toggle completion status
- Filter by completion status
- Project ownership validation

✅ **Security & Data Management**
- JWT Bearer authentication
- Role-based authorization
- Automatic audit trail (CreatedBy, UpdatedBy)
- Soft delete with timestamps
- SQLite database with EF Core 9.0.10

### Technology Stack
- .NET 8.0.415
- Entity Framework Core 9.0.10
- MediatR 13.1.0
- FluentValidation 12.0.0
- BCrypt.Net-Next 4.0.3
- Serilog.AspNetCore 8.0.3
- SQLite Database

### API Endpoints (14 total)

**Authentication (3 endpoints):**
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh

**Projects (5 endpoints):**
- GET /api/projects (paginated)
- GET /api/projects/{id}
- POST /api/projects
- PUT /api/projects/{id}
- DELETE /api/projects/{id}

**Tasks (6 endpoints):**
- GET /api/tasks/project/{projectId}
- GET /api/tasks/{id}
- POST /api/tasks
- PUT /api/tasks/{id}
- PATCH /api/tasks/{id}/toggle
- DELETE /api/tasks/{id}

### Build Status
✅ Build: Successful (0 errors, 0 warnings)
✅ Database: Created and migrated
✅ API: Running on https://localhost:5001
✅ Swagger UI: Available at /swagger

### Documentation
See `Assignment2/IMPLEMENTATION_SUMMARY.md` for detailed documentation including:
- Complete API endpoint specifications
- Database schema
- CQRS pattern explanation
- Security features
- Code structure
- Next steps for frontend integration

### Total Deliverables
- **Files Created:** 68 files
- **Database Tables:** 4 tables (Users, Projects, Tasks, RefreshTokens)
- **Endpoints:** 14 RESTful endpoints
- **Lines of Code:** ~3,500 lines

---

## Summary

### Assignment 1
- ✅ Fixed sidebar border visibility
- ✅ Simple CSS modification

### Assignment 2
- ✅ Complete Clean Architecture implementation
- ✅ CQRS pattern with MediatR
- ✅ JWT authentication with refresh tokens
- ✅ 14 fully functional API endpoints
- ✅ Database with migrations
- ✅ Comprehensive documentation

**Status:** Both assignments completed successfully! 🎉

---

## Assignment 3: Smart Scheduler ✅
**Status:** COMPLETED

### Overview
Implemented an intelligent task scheduling system using **Critical Path Method (CPM)** and **Topological Sort** to optimize project timelines and identify critical tasks.

### Features Implemented

✅ **Backend Scheduling Algorithm**
- Critical Path Method (CPM) for project timeline optimization
- Topological Sort (Kahn's Algorithm) for dependency resolution
- Earliest/Latest start time calculations
- Slack time computation
- Circular dependency detection
- O(V+E) time complexity
- 40/40 unit tests passing

✅ **Frontend Smart Scheduler UI**
- Beautiful schedule generation modal
- Color-coded warnings (orange) for tasks without estimated hours
- Task order table with execution sequence
- Critical path visualization (red badges)
- Slack time display (green for flexible tasks)
- Info box explaining schedule metrics
- Dependency names shown on task cards (e.g., "Depends on: hello, hey")
- Estimated hours badges on all tasks

✅ **Advanced Dependency Graph Visualization**
- Interactive canvas-based graph rendering
- Hierarchical topological layout
- Zoom controls (50% - 200%)
- Hover effects on nodes
- Curved arrows showing dependencies
- Color-coded nodes (green for completed, white for incomplete)
- Node shadows and smooth animations
- Legend explaining graph symbols

✅ **Task Management Enhancements**
- EstimatedHours property on tasks (nullable decimal)
- DependencyIds array for task relationships
- TaskDependency entity for many-to-many relationships
- Create/Update commands support dependencies
- Query handlers populate dependency data
- Validation for scheduling parameters

### Technology Stack
- **Algorithm:** Critical Path Method + Kahn's Topological Sort
- **Backend:** .NET 8, CQRS, MediatR
- **Frontend:** React 18, TypeScript, HTML5 Canvas
- **Database:** SQLite with EF Core migrations
- **UI:** TailwindCSS, Lucide Icons

### API Endpoints (1 new endpoint)

**Scheduling:**
- POST /api/scheduling/generate/{projectId}
  - Request: None
  - Response: ScheduleResult with warnings, ordered tasks, critical path
  - Returns: 200 OK with schedule data
  - Handles: Missing estimated hours (defaults to 1 hour)

### Algorithm Details

**Critical Path Method:**
- Calculates earliest start time (EST) for each task
- Computes latest start time (LST) via backward pass
- Identifies slack time: LST - EST
- Critical path tasks have 0 slack

**Topological Sort (Kahn's Algorithm):**
- Builds dependency graph
- Uses in-degree tracking
- Queue-based BFS traversal
- Detects circular dependencies

### UI Components

**ScheduleModal:**
- Warnings section with task count
- Optimized task order table (ORDER, TASK, HOURS, CRITICAL, SLACK)
- Critical path badges (red)
- Slack time (green text)
- Info box with schedule reading guide
- Close button

**DependencyGraph:**
- Interactive canvas (800x600px)
- Zoom controls (±10% increments)
- Hierarchical node positioning
- Curved dependency arrows with arrowheads
- Node hover effects
- Completion status indicators (✓)
- Legend (incomplete/completed/arrows)

### Build & Test Status
✅ Backend Tests: 40/40 passing
✅ Frontend Build: No errors
✅ TypeScript: No type errors
✅ Algorithm Verified: Correct critical path calculation
✅ UI Tested: Schedule generation working end-to-end

### User Experience Highlights
- **1-hour default** for tasks without estimated hours (industry standard)
- **Clear warnings** inform users about assumptions
- **Professional UI** rated 8.5/10 by user
- **Dependency names** displayed on task cards (not just count)
- **Visual graph** shows project structure at a glance
- **Interactive zoom** for complex dependency chains

### Total Deliverables
- **Files Created:** 5 new files
  - SchedulingController.cs
  - SchedulingService.cs
  - ScheduleModal.tsx
  - DependencyGraph.tsx
  - TaskDependency entity
- **Algorithm Implementation:** CPM + Topological Sort
- **Tests:** 40 comprehensive unit tests
- **UI Components:** 2 advanced visualizations

---

## Summary

### Assignment 1
- ✅ Fixed sidebar border visibility
- ✅ Simple CSS modification

### Assignment 2
- ✅ Complete Clean Architecture implementation
- ✅ CQRS pattern with MediatR
- ✅ JWT authentication with refresh tokens
- ✅ 14 fully functional API endpoints
- ✅ Database with migrations
- ✅ Comprehensive documentation

### Assignment 3
- ✅ Critical Path Method scheduling algorithm
- ✅ Topological Sort for dependency resolution
- ✅ Interactive dependency graph visualization
- ✅ Smart schedule generator with warnings
- ✅ Enhanced task cards with dependency names
- ✅ 40/40 unit tests passing
- ✅ Professional UI with advanced features

```
