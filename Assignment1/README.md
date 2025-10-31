# Task Manager - PathLock Home Assignment# Task Manager API - Assignment 1



A full-stack task management application built with .NET 8 and React + TypeScript, demonstrating enterprise-grade development practices.




This is Assignment 1 of the PathLock home assignment series. It's a simple yet robust CRUD application that showcases clean architecture, modern frontend patterns, and professional UI design.**Live API**: http://localhost:5039 (Swagger UI at root)



**Live Features:**A production-ready RESTful API for managing tasks, built with .NET 8 using clean architecture principles.

- Create, read, update, and delete tasks

- Mark tasks as completed/active---

- Filter by status (All/Active/Completed)

- Search tasks in real-time## 🎯 Project Overview

- Bulk select and delete

- Offline persistence with localStorageThis is Assignment 1 for the PathLock home assignment - a **Basic Task Manager** demonstrating:

- ✅ Clean, simple Service Layer architecture (NO CQRS - intentionally)

## Tech Stack- ✅ Rich domain models with encapsulation

- ✅ Production-ready error handling and logging

### Backend (.NET 8)- ✅ Comprehensive API documentation with Swagger

- **ASP.NET Core Web API** - RESTful endpoints- ✅ FluentValidation for request validation

- **FluentValidation** - Request validation- ✅ In-memory storage (as per requirements)

- **Serilog** - Structured logging

- **Swagger/OpenAPI** - API documentation---

- **In-memory storage** - ConcurrentDictionary for thread-safety

## ✨ Features

### Frontend (React)

- **React 18 + TypeScript** - Type-safe UI components### Core Functionality

- **TanStack Query (React Query)** - Server state management- ✅ Create new tasks with descriptions

- **Axios** - HTTP client- ✅ View all tasks

- **Tailwind CSS** - Utility-first styling- ✅ View individual task by ID

- **Vite** - Fast build tool- ✅ Update task description and completion status

- **Lucide React** - Icon library- ✅ Delete tasks

- ✅ Toggle task completion status

## Architecture Decisions

### Technical Features

I chose a **Simple Service Layer** pattern over CQRS because:- ✅ RESTful API design with proper HTTP verbs and status codes

- Single aggregate (Task)- ✅ Global exception handling middleware

- Simple CRUD operations- ✅ Structured logging with Serilog

- No complex business rules- ✅ Request validation with FluentValidation

- Microsoft and Martin Fowler recommend this for straightforward scenarios- ✅ OpenAPI/Swagger documentation

- ✅ CORS configuration for frontend integration

The backend follows a clean 3-layer architecture:- ✅ Health check endpoint

```- ✅ API versioning (`/api/v1/tasks`)

Controller → Service → Repository → Domain Model

```---



## Getting Started## 🏗️ Architecture



### Prerequisites### Simple Service Layer (NOT CQRS)

- .NET 8 SDK

- Node.js 18+ and npm```

- A code editor (VS Code recommended)┌─────────────────────────────────────────────────────────────┐

│                     HTTP Request                            │

### Running the Backend└─────────────────────┬───────────────────────────────────────┘

                      │

```bash                      ▼

cd Assignment1/TaskManagerAPI┌─────────────────────────────────────────────────────────────┐

dotnet run│                  TasksController                            │

```│           (Thin - orchestration only)                       │

└─────────────────────┬───────────────────────────────────────┘

Backend runs on `http://localhost:5039`                      │

- Swagger UI: `http://localhost:5039` (root path)                      ▼

- API Base: `http://localhost:5039/api/v1`┌─────────────────────────────────────────────────────────────┐

│                    TaskService                              │

### Running the Frontend│            (Business logic lives here)                      │

└─────────────────────┬───────────────────────────────────────┘

```bash                      │

cd Assignment1/task-manager-frontend                      ▼

npm install┌─────────────────────────────────────────────────────────────┐

npm run dev│                ITaskRepository                              │

```│         (InMemoryTaskRepository impl)                       │

└─────────────────────┬───────────────────────────────────────┘

Frontend runs on `http://localhost:5173` (or next available port)                      │

                      ▼

## API Endpoints┌─────────────────────────────────────────────────────────────┐

│              ConcurrentDictionary                           │

| Method | Endpoint | Description |│             (Thread-safe in-memory storage)                 │

|--------|----------|-------------|└─────────────────────────────────────────────────────────────┘

| GET | `/api/v1/tasks` | Get all tasks |```

| GET | `/api/v1/tasks/{id}` | Get task by ID |

| POST | `/api/v1/tasks` | Create new task |**3 clean layers** - Controller → Service → Repository

| PUT | `/api/v1/tasks/{id}` | Update task |

| DELETE | `/api/v1/tasks/{id}` | Delete task |---

| PATCH | `/api/v1/tasks/{id}/toggle` | Toggle completion |

## 📁 Project Structure

**Request/Response Examples:**

```

Create Task:TaskManagerAPI/

```json├── Controllers/

POST /api/v1/tasks│   └── TasksController.cs          # Thin REST controllers

{├── Services/

  "description": "Complete PathLock assignment"│   ├── ITaskService.cs              # Service interface

}│   └── TaskService.cs               # Business logic implementation

```├── Repositories/

│   ├── ITaskRepository.cs           # Repository interface

Response:│   └── InMemoryTaskRepository.cs    # In-memory storage (ConcurrentDictionary)

```json├── Models/

{│   ├── Domain/

  "id": "guid-here",│   │   └── TaskItem.cs              # Rich domain model (private setters, behavior methods)

  "description": "Complete PathLock assignment",│   ├── DTOs/

  "isCompleted": false,│   │   └── TaskDtos.cs              # Request/Response models

  "createdAt": "2025-10-30T04:14:17Z",│   └── TaskMappingExtensions.cs     # Domain ↔ DTO mapping

  "completedAt": null├── Validators/

}│   └── TaskValidators.cs            # FluentValidation validators

```├── Middleware/

│   └── ExceptionHandlingMiddleware.cs  # Global error handling

## Project Structure└── Program.cs                        # App configuration & DI setup

```

### Backend

```---

TaskManagerAPI/

├── Controllers/          # API endpoints## 🚀 Getting Started

├── Services/            # Business logic

├── Repositories/        # Data access### Prerequisites

├── Models/- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (version 8.0.415+)

│   ├── Domain/         # Rich domain models

│   └── DTOs/           # Data transfer objects### Running the API

├── Validators/         # FluentValidation rules

├── Middleware/         # Global exception handling1. **Clone the repository**

└── Program.cs          # App configuration   ```bash

```   cd Assignment1/TaskManagerAPI

   ```

### Frontend

```2. **Run the application**

task-manager-frontend/   ```bash

├── src/   dotnet run

│   ├── components/     # React components   ```

│   ├── hooks/          # Custom React hooks

│   ├── services/       # API client & storage3. **Access Swagger UI**

│   ├── types/          # TypeScript interfaces   - Open browser: http://localhost:5039

│   └── App.tsx         # Main application   - Swagger UI is served at the root path

└── tailwind.config.js  # Tailwind configuration   - Interactive API documentation with "Try it out" feature

```

4. **Health Check**

## Key Features   ```bash

   curl http://localhost:5039/health

### Backend Highlights   ```

- **Rich Domain Model**: Encapsulated properties with behavior methods

- **Validation**: FluentValidation for request validation (3-500 characters)---

- **Logging**: Serilog with structured logging to console and file

- **Error Handling**: Global middleware for consistent error responses## 📖 API Endpoints

- **CORS**: Configured for frontend integration

- **Thread-Safe**: ConcurrentDictionary for in-memory storageAll endpoints are under `/api/v1/tasks`:



### Frontend Highlights| Method | Endpoint | Description | Status Codes |

- **React Query**: Automatic caching, background refetching, optimistic updates|--------|----------|-------------|--------------|

- **Search**: Real-time task filtering| `GET` | `/api/v1/tasks` | Get all tasks | 200 OK |

- **Bulk Operations**: Select all and delete multiple tasks| `GET` | `/api/v1/tasks/{id}` | Get task by ID | 200 OK, 404 Not Found |

- **LocalStorage**: Offline persistence with API sync| `POST` | `/api/v1/tasks` | Create new task | 201 Created, 400 Bad Request |

- **PathLock Branding**: Custom logo and brand colors (#24B770 green, #0F1555 navy)| `PUT` | `/api/v1/tasks/{id}` | Update task | 200 OK, 400 Bad Request, 404 Not Found |

- **Responsive**: Mobile-friendly design| `DELETE` | `/api/v1/tasks/{id}` | Delete task | 204 No Content, 404 Not Found |

- **Accessibility**: Proper ARIA labels and keyboard navigation| `PATCH` | `/api/v1/tasks/{id}/toggle` | Toggle completion | 200 OK, 404 Not Found |



## Design Decisions### Request/Response Examples



**Why React Query?**#### Create Task

- Automatic caching reduces unnecessary API calls```bash

- Optimistic updates improve perceived performancePOST /api/v1/tasks

- Built-in error handling and retry logicContent-Type: application/json

- Better developer experience than manual state management

{

**Why Tailwind CSS?**  "description": "Complete Assignment 1"

- Rapid development with utility classes}

- Consistent design system```

- Small production bundle (unused CSS purged)

- Easy to maintain and customize**Response (201 Created):**

```json

**Why In-Memory Storage?**{

- Assignment requirement (no database needed)  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

- Fast and simple for demo purposes  "description": "Complete Assignment 1",

- Thread-safe implementation for production-like behavior  "isCompleted": false,

  "createdAt": "2025-10-30T01:14:00Z",

## Testing the Application  "completedAt": null

}

1. **Start Backend**: Run the .NET API (localhost:5039)```

2. **Start Frontend**: Run the React app (localhost:5173)

3. **Create Tasks**: Add tasks using the input form#### Update Task

4. **Filter**: Switch between All/Active/Completed views```bash

5. **Search**: Use the search bar to find tasksPUT /api/v1/tasks/3fa85f64-5717-4562-b3fc-2c963f66afa6

6. **Bulk Delete**: Select multiple tasks and delete themContent-Type: application/json

7. **Check Logs**: Watch the backend console for structured logs

{

## What I Learned  "description": "Complete Assignment 1 - Backend",

  "isCompleted": true

Building this assignment helped me understand:}

- Clean architecture principles in .NET```

- Domain-driven design basics

- React Query for server state management**Response (200 OK):**

- TypeScript type safety benefits```json

- Professional API design patterns{

- The importance of validation and error handling  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

  "description": "Complete Assignment 1 - Backend",

## Future Improvements  "isCompleted": true,

  "createdAt": "2025-10-30T01:14:00Z",

If I were to extend this (not required for the assignment):  "completedAt": "2025-10-30T01:15:30Z"

- Add unit tests (xUnit for backend, Vitest for frontend)}

- Implement persistent database (PostgreSQL/SQL Server)```

- Add authentication/authorization

- Implement real-time updates with SignalR---

- Deploy to Azure/AWS

- Add Docker containerization## 🎨 Architecture Decisions



## Assignment Requirements Checklist### Why Simple Service Layer Instead of CQRS?



**Core Requirements:**I deliberately chose a straightforward **Service Layer pattern** over **CQRS/MediatR** for this assignment because:

- ✅ RESTful API with .NET 8

- ✅ In-memory data storage#### 1. **Single Aggregate**

- ✅ Task model with required properties- Only one entity (`TaskItem`) with no complex relationships or invariants

- ✅ All CRUD endpoints- No need for separate read/write models

- ✅ React + TypeScript frontend

- ✅ Task list display#### 2. **No Read/Write Divergence**

- ✅ Add/toggle/delete functionality- Queries and commands have identical structure and authorization

- ✅ Axios for API integration- Same data returned from GET as sent to POST/PUT

- ✅ React Hooks

#### 3. **KISS Principle (Keep It Simple, Stupid)**

**Enhancements:**- Adding CQRS/MediatR would create this chain: 

- ✅ Task filtering (All/Active/Completed)  - `Controller → Mediator → CommandHandler → Service → Repository` (5+ hops)

- ✅ Tailwind CSS styling- Current architecture:

- ✅ LocalStorage persistence  - `Controller → Service → Repository` (3 hops)

- **3x less complexity** with zero benefit for this use case

**Bonus Features:**

- ✅ Search functionality#### 4. **Industry Guidance**

- ✅ Bulk operations- **Microsoft**: *"CQRS should only be used where there is clear benefit"*

- ✅ React Query- **Martin Fowler**: *"CQRS adds risky complexity for most systems"*

- ✅ FluentValidation- **Greg Young (CQRS creator)**: *"CQRS is not a top-level architecture"*

- ✅ Serilog logging

- ✅ Professional UI/UX#### 5. **Maintainability**

- Easier to debug (clear call stack)

- Less code to maintain (~500 LOC vs ~1200+ with CQRS)

Built by **Anshdeep Singh** as part of the PathLock technical assessment.



### Trade-off Acknowledged

If requirements grow to include:
- Multi-user collaboration
- Complex authorization rules
- Event sourcing
- Different read/write optimization needs
- Audit trails and command logging

...then refactoring to CQRS would be justified. The current architecture is designed to make such evolution straightforward.

### References
- [Microsoft CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Martin Fowler on CQRS](https://martinfowler.com/bliki/CQRS.html)
- [Ardalis Clean Architecture](https://github.com/ardalis/CleanArchitecture)

---

## 🔧 Technologies Used

### Backend
- **.NET 8.0** - Latest LTS release
- **ASP.NET Core Web API** - RESTful API framework
- **FluentValidation 11.3.0** - Request validation (better than DataAnnotations)
- **Serilog 8.0.0** - Structured logging
- **Swashbuckle (Swagger) 6.6.2** - API documentation

### Design Patterns
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Service Layer** - Business logic separation
- ✅ **Dependency Injection** - Loose coupling
- ✅ **Rich Domain Model** - Encapsulation with private setters
- ✅ **DTO Pattern** - API contract separation

### Code Quality
- ✅ **SOLID Principles** throughout
- ✅ **Clean Code** practices
- ✅ **Async/Await** for scalability
- ✅ **Thread-safe** in-memory storage (ConcurrentDictionary)

---

### 3. Clean Separation of Concerns
- **Controllers**: HTTP orchestration only
- **Services**: Business logic
- **Repositories**: Data access
- **Models**: Domain logic encapsulation
- **Validators**: Input validation rules

---

## 🧪 Testing Strategy

### Unit Tests (Planned)
- TaskService business logic
- TaskItem domain model behavior
- Validators

### Integration Tests (Planned)
- Full API endpoints with in-memory storage
- Middleware pipeline
- Error handling flows

---



## 📝 Development Process

### Decisions Made
1. ✅ **No CQRS** - Justified by single aggregate, simple CRUD
2. ✅ **FluentValidation** over DataAnnotations - More flexible
3. ✅ **Serilog** over built-in logging - Structured logging
4. ✅ **ConcurrentDictionary** - Thread-safe in-memory storage
5. ✅ **Rich domain models** - Encapsulation over anemic models




## 🙏 Acknowledgments

- Microsoft for excellent .NET 8 documentation
- Martin Fowler for architecture guidance
- Clean Architecture community for best practices
