# PathLock Home Assignment - Full Stack Project Management System

**Candidate:** Anshdeep singh  
**Email:** anshdeepsingh686@gmail.com  
**Submission Date:** October 31, 2025

---

## 🌐 Live Deployments

### Assignment 1: Simple Task Manager ✅
- **🚀 Live Demo:** https://assignment1-task-manager.vercel.app
- **📖 API Docs:** https://pathlock-home-assignment-production.up.railway.app/index.html
- **Backend:** Railway (.NET 8 API)
- **Frontend:** Vercel (React + TypeScript)

### Assignment 2+3: Full Project Manager (CQRS + Scheduling) ✅
- **🚀 Live Demo:** https://pathlock-project-manager.vercel.app
- **📖 API Docs:** https://pathlock-home-assignment-production-344b.up.railway.app/
- **Backend:** Railway (.NET 8 API with Clean Architecture + CQRS)
- **Frontend:** Vercel (React + TypeScript + Vite)

---

## 🎯 Project Overview

A comprehensive full-stack project management system built with **.NET 8** (backend) and **React 18 + TypeScript** (frontend). This application demonstrates advanced software engineering principles including Clean Architecture, CQRS pattern, task dependency management, and Critical Path Method (CPM) scheduling algorithm.

### 🏆 Key Achievements
- ✅ **100% Feature Complete** - All 3 assignments delivered
- ✅ **40/40 Tests Passing** - Comprehensive unit test coverage
- ✅ **Production-Ready** - Security, validation, and error handling
- ✅ **Advanced Features** - Interactive dependency graph, decimal precision
- ✅ **Clean Code** - SOLID principles, proper architecture
- ✅ **Fully Deployed** - Both assignments live on Railway + Vercel
- ✅ **CORS Configured** - Flexible origin handling for any frontend URL changes

---

## 📋 Assignments Completed

### Assignment 1: Simple Task Manager ✅
- In-memory task storage (thread-safe ConcurrentDictionary)
- RESTful API with proper HTTP verbs and status codes
- Task CRUD operations (Create, Read, Update, Delete)
- Task completion tracking
- Responsive UI with Tailwind CSS
- **DEPLOYED:** https://assignment1-task-manager.vercel.app

### Assignment 2: Authentication & Project Management ✅
- JWT-based authentication (15-min access token, 7-day refresh token)
- User registration with strong password validation
- Project CRUD operations (Create, Read, Update, Delete)
- Task management with completion tracking
- Responsive UI with Tailwind CSS
- Auto-refresh token mechanism
- **DEPLOYED:** https://pathlock-project-manager.vercel.app

### Assignment 3: Task Dependencies ✅
- Many-to-many task relationships
- Circular dependency detection
- Self-dependency prevention
- Database design with junction tables
- **BONUS:** Interactive dependency graph with Canvas API
- **BONUS:** Visual task relationships with zoom functionality
- **DEPLOYED:** https://pathlock-project-manager.vercel.app

### Assignment 4: Project Scheduling Algorithm ✅
- Critical Path Method (CPM) implementation
- Topological sort for task ordering
- Forward/backward pass calculations
- Slack time and critical path identification
- **40/40 unit tests passing** (100% coverage)
- Schedule visualization UI
- **DEPLOYED:** https://pathlock-project-manager.vercel.app

---

## 🛠️ Technology Stack

### Backend
- **.NET 8.0** - Latest LTS framework
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 9.0** - ORM with SQLite
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **BCrypt.Net** - Password hashing
- **JWT Bearer** - Token-based authentication

### Frontend
- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **TanStack Query (React Query)** - Data fetching & caching
- **React Router** - Client-side routing
- **React Hook Form** - Form management
- **Zod** - Schema validation
- **Tailwind CSS** - Styling
- **Lucide React** - Icon library
- **Zustand** - State management

### Architecture Patterns
- **Clean Architecture** - Separation of concerns
- **CQRS** - Command Query Responsibility Segregation
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **MVC Pattern** - Frontend component structure

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Git](https://git-scm.com/)

### 1️⃣ Clone Repository
```bash
git clone <repository-url>
cd pathlock_home_assignment
```

### 2️⃣ Backend Setup
```bash
# Navigate to API project
cd Assignment2/src/API

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run backend (http://localhost:5000)
dotnet run
```

**Backend URL:** `http://localhost:5000`  
**Swagger UI:** `http://localhost:5000/swagger`

### 3️⃣ Frontend Setup
```bash
# Navigate to frontend (from root)
cd project-manager-frontend

# Install dependencies
npm install

# Run development server (http://localhost:5173)
npm run dev
```

**Frontend URL:** `http://localhost:5173`

### 4️⃣ Run Tests
```bash
# Navigate to test project
cd Assignment2/tests/Application.Tests

# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed
```

**Expected Result:** ✅ 40/40 tests passing

---

## 📁 Project Structure

```
pathlock_home_assignment/
│
├── Assignment1/                    # Assignment 1: Basic Task Manager
│   ├── TaskManagerAPI/            # .NET 8 Web API (Simple Service Layer)
│   │   ├── Controllers/           # REST endpoints
│   │   ├── Services/              # Business logic
│   │   ├── Repositories/          # In-memory storage
│   │   ├── Models/                # Domain & DTOs
│   │   ├── Validators/            # FluentValidation
│   │   └── Middleware/            # Global error handling
│   │
│   ├── task-manager-frontend/     # React + TypeScript frontend
│   │   ├── src/
│   │   │   ├── components/        # UI components
│   │   │   ├── hooks/             # Custom hooks
│   │   │   └── App.tsx
│   │   └── package.json
│   │
│   └── README.md                  # Assignment 1 documentation
│
├── Assignment2/                    # Assignments 2 & 3: Project Manager + Scheduling
│   ├── src/
│   │   ├── API/                   # ASP.NET Core Web API
│   │   ├── Application/           # CQRS (Commands & Queries)
│   │   ├── Domain/                # Domain entities & interfaces
│   │   └── Infrastructure/        # Database, JWT, services
│   │
│   ├── tests/
│   │   └── Application.Tests/     # 40 unit tests (CPM algorithm)
│   │
│   ├── ProjectManager.sln         # Solution file
│   └── README.md                  # Assignment 2 & 3 documentation
│
├── project-manager-frontend/       # Frontend for Assignment 2 & 3
│   ├── src/
│   │   ├── components/            # Shared UI components
│   │   ├── features/              # Auth, Projects, Tasks modules
│   │   │   ├── auth/              # Login, Register, JWT handling
│   │   │   ├── projects/          # Project CRUD, Dashboard
│   │   │   └── tasks/             # Tasks, Dependencies, Graph, Scheduling
│   │   ├── lib/                   # API client, auth store, utils
│   │   └── App.tsx
│   │
│   ├── package.json
│   ├── vite.config.ts
│   └── README.md                  # Frontend documentation
│
├── README.md                       # This file - Project overview
├── FINAL_SUBMISSION_CHECKLIST.md  # Comprehensive submission checklist
└── COMPREHENSIVE_ANALYSIS.md      # Technical deep-dive & decisions
```

**Key Points:**
- **Assignment 1** is self-contained with its own backend (`TaskManagerAPI`) and frontend (`task-manager-frontend`)
- **Assignment 2 & 3** share the same backend (`Assignment2/src`) and frontend (`project-manager-frontend/`)
- Frontend for Assignments 2 & 3 is at the **root level** (`project-manager-frontend/`), NOT inside `Assignment2/`
```

---

## 🔑 Key Features

### 1. Authentication & Security
- **JWT Tokens:** 15-minute access token, 7-day refresh token
- **Auto-Refresh:** Seamless session management
- **Password Requirements:**
  - Minimum 8 characters
  - At least 1 uppercase letter
  - At least 1 lowercase letter
  - At least 1 number
  - At least 1 special character (!@#$%^&*)
- **Secure Storage:** Bcrypt password hashing (cost factor: 12)
- **CORS Protection:** Configured for frontend origin

### 2. Project Management
- Create, update, and delete projects
- Project listing with search/filter
- Task creation with dependencies
- Task completion tracking
- Estimated hours (decimal support: 0.5, 2.5, etc.)

### 3. Task Dependencies
- Visual dependency selection (checkboxes)
- Circular dependency prevention
- Dependency names shown on task cards
- **Interactive Dependency Graph:**
  - HTML5 Canvas rendering
  - Topological layout algorithm
  - Zoom (50% - 200%)
  - Hover effects with task details
  - Color coding (green = completed, white = incomplete)

### 4. Project Scheduling (CPM Algorithm)
- Critical Path Method implementation
- Topological sort for task ordering
- Forward pass: Calculate earliest start/finish times
- Backward pass: Calculate latest start/finish times
- Slack calculation: Identify float time
- Critical path: Tasks with 0 slack highlighted
- **Schedule Modal UI:**
  - Timeline visualization
  - Color-coded tasks (red/orange for critical path)
  - Start/end times per task
  - Total project duration

---

## 🧪 Testing

### Unit Tests (40/40 Passing) ✅
Located in `Assignment2/tests/Application.Tests/SchedulingAlgorithmTests.cs`

**Test Categories:**
1. **Topological Sort Tests (6)** - Graph ordering validation
2. **Forward Pass Tests (7)** - Earliest start/finish calculations
3. **Backward Pass Tests (7)** - Latest start/finish calculations
4. **Slack Calculation Tests (6)** - Float time accuracy
5. **Critical Path Tests (7)** - Path identification
6. **Integration Tests (7)** - End-to-end scenarios

### Run Tests
```bash
cd Assignment2/tests/Application.Tests
dotnet test --verbosity detailed
```

### Test Coverage
- ✅ Simple linear paths
- ✅ Parallel tasks
- ✅ Complex dependency graphs
- ✅ Multiple critical paths
- ✅ Edge cases (no dependencies, circular detection)

---

## 📊 API Endpoints

### Authentication
- `POST /api/auth/register` - Create new user
- `POST /api/auth/login` - User login (returns JWT)
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Invalidate refresh token

### Projects
- `GET /api/projects` - List all projects (with pagination)
- `GET /api/projects/{id}` - Get project details
- `POST /api/projects` - Create project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

### Tasks
- `GET /api/projects/{id}/tasks` - List tasks with dependencies
- `POST /api/tasks` - Create task with dependencies
- `PUT /api/tasks/{id}` - Update task and dependencies
- `DELETE /api/tasks/{id}` - Delete task
- `PATCH /api/tasks/{id}/complete` - Mark task complete
- `PATCH /api/tasks/{id}/incomplete` - Mark task incomplete

### Scheduling
- `POST /api/projects/{id}/schedule` - Generate CPM schedule

**API Documentation:** Available via Swagger at `http://localhost:5000/swagger`

---


---

## 🔒 Security Features

### Backend
- ✅ JWT token-based authentication
- ✅ Bcrypt password hashing (cost: 12)
- ✅ Refresh token rotation
- ✅ CORS configuration
- ✅ Input validation (FluentValidation)
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Token expiration handling
- ✅ Secure password storage

### Frontend
- ✅ Auto token refresh before expiry
- ✅ Protected routes (redirect to login)
- ✅ XSS prevention (React auto-escaping)
- ✅ Form validation (Zod schemas)
- ✅ Error boundary implementation
- ✅ Secure state management (Zustand)
- ✅ Browser autocomplete icons disabled

---

## ⚙️ Configuration

### Backend (appsettings.json)
```json
{
  "Jwt": {
    "SecretKey": "YourSecretKeyHere_MinimumLength32Characters_ChangeInProduction",
    "Issuer": "ProjectManagerAPI",
    "Audience": "ProjectManagerClient",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=projectmanager.db"
  }
}
```

### Frontend (.env)
```env
VITE_API_BASE_URL=http://localhost:5000/api
```

---

## 📈 Performance Optimizations

### Backend
- ✅ Async/await throughout
- ✅ Eager loading for dependencies (Include)
- ✅ Database indexes on foreign keys
- ✅ Efficient LINQ queries
- ✅ MediatR pipeline caching potential

### Frontend
- ✅ TanStack Query caching (5-minute default)
- ✅ React.memo for expensive components
- ✅ Debounced search inputs
- ✅ Lazy loading for routes (code splitting ready)
- ✅ Canvas optimization (requestAnimationFrame)

---




### Test Quality
- ✅ **40/40 Tests Passing** (100%)
- ✅ **Arrange-Act-Assert Pattern:** Clear test structure
- ✅ **Edge Cases Covered:** Empty graphs, circular dependencies
- ✅ **Integration Tests:** End-to-end scenarios
- ✅ **Descriptive Names:** Self-documenting test methods

---

## 🎓 Advanced Features (Bonus)

### 1. Interactive Dependency Graph 🎨
- **Technology:** HTML5 Canvas API
- **Algorithm:** Topological sort for hierarchical layout
- **Features:**
  - Zoom (50% - 200%)
  - Pan (drag to move)
  - Hover tooltips
  - Color coding (green = done, white = pending)
  - Smooth curved arrows
  - Responsive resizing

### 2. Decimal Estimated Hours 🔢
- **Database:** Changed from `int?` to `decimal?`
- **Migration:** Automated EF Core migration
- **Precision:** Support for 0.5, 2.5, 7.25 hours, etc.
- **Validation:** Must be > 0 if provided

### 3. Enhanced UX 🎯
- Dependency names on cards (not just count)
- Loading spinners for all async operations
- Success/error toast notifications
- Form auto-focus
- Keyboard navigation support
- Password visibility toggle (single icon)

---


---

## 🔮 Future Enhancements (Post-Submission)

### Potential Improvements
- [ ] Email verification for registration
- [ ] Password reset flow
- [ ] Real-time collaboration (SignalR)
- [ ] File attachments for tasks
- [ ] Task comments/discussions
- [ ] Gantt chart view
- [ ] Export schedule to PDF/Excel
- [ ] Mobile app (React Native)
- [ ] Dark mode theme
- [ ] Multi-language support (i18n)

---

## 👨‍💻 Developer Information

**Name:** Anshdeep Singh  
**Email:** anshdeepsingh686@gmail.com  
**LinkedIn:** https://linkedin.com/in/anshdeep-singh-306336256
**GitHub:** https://github.com/AnshBajwa13  

#
**Total Development Time:** ~7 days  
**Final Status:** 100% Complete, Production-Ready

---

## � Production Deployment

### Assignment 1 (Simple Task Manager)
- **Frontend:** https://assignment1-task-manager.vercel.app
- **Backend API:** https://pathlock-home-assignment-production.up.railway.app/index.html
- **Platform:** Vercel + Railway
- **Status:** ✅ Live & Running

### Assignment 2+3 (Full Project Manager)
- **Frontend:** https://pathlock-project-manager.vercel.app
- **Backend API:** https://pathlock-home-assignment-production-344b.up.railway.app/
- **Platform:** Vercel + Railway
- **Status:** ✅ Live & Running

### Deployment Features
- ✅ Automatic CI/CD from GitHub
- ✅ CORS configured for production
- ✅ Environment variables secured
- ✅ Database persistence with SQLite
- ✅ Swagger UI enabled for API testing
- ✅ JWT authentication in production
- ✅ Flexible origin handling for frontend URL changes

### How to Test
1. Visit frontend URLs above
2. Register a new account or use test credentials
3. Create projects and tasks
4. Add task dependencies and view interactive graph
5. Generate project schedules with CPM algorithm
6. All features work end-to-end in production!

---

## �📞 Support & Contact

For questions or issues:
1. **Email:** anshdeepsingh686@gmail.com

---

## 📄 License

This project is submitted as part of PathLock's home assignment evaluation.  
© 2025 ANshdeep singh. All rights reserved.

---

## 🙏 Acknowledgments

- PathLock team for the detailed assignment
- .NET community for excellent documentation
- React community for modern best practices
- Open-source contributors for amazing libraries




**Thank you for reviewing my submission!** 🚀




