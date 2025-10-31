# Task Manager API - Assignment 1

A production-ready RESTful API for managing tasks, built with .NET 8 using clean Service Layer architecture.

**API URL:** `http://localhost:5039/api/v1`  
**Swagger UI:** `http://localhost:5039`

---

## 🎯 Project Overview

This is **Assignment 1** for the PathLock home assignment - a **Basic Task Manager** demonstrating:

- ✅ Clean, simple Service Layer architecture (NO CQRS - intentionally simple)
- ✅ Rich domain models with encapsulation
- ✅ Production-ready error handling and logging
- ✅ Comprehensive API documentation with Swagger
- ✅ FluentValidation for request validation
- ✅ Thread-safe in-memory storage (as per requirements)

---

## ✨ Features

### Core Functionality
- ✅ Create new tasks with descriptions
- ✅ View all tasks
- ✅ View individual task by ID
- ✅ Update task description and completion status
- ✅ Delete tasks
- ✅ Toggle task completion status

### Technical Features
- ✅ RESTful API design with proper HTTP verbs and status codes
- ✅ Global exception handling middleware
- ✅ Structured logging with Serilog
- ✅ Request validation with FluentValidation
- ✅ OpenAPI/Swagger documentation
- ✅ CORS configuration for frontend integration
- ✅ Health check endpoint
- ✅ API versioning (`/api/v1/tasks`)

---

## 🏗️ Architecture

### Why Simple Service Layer (NOT CQRS)?

I chose a **Simple Service Layer** pattern over CQRS because:
- ✅ Single aggregate (Task) - no complex domain
- ✅ Simple CRUD operations - no command/query distinction needed
- ✅ No complex business rules
- ✅ Microsoft and Martin Fowler recommend this for straightforward scenarios

> **Quote from Martin Fowler:**  
> *"CQRS should only be used on specific portions of a system, and not the whole system. We should only use it for complex domains where we need the flexibility."*

### Architecture Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                     HTTP Request                             │
│                  (POST, GET, PUT, DELETE)                    │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                    TasksController                           │
│                                                              │
│  • Route handling                                            │
│  • HTTP status codes                                         │
│  • DTO validation                                            │
│  • Delegates to Service                                      │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                      TaskService                             │
│                  (Business Logic Layer)                      │
│                                                              │
│  • Orchestrates business rules                               │
│  • Calls repository                                          │
│  • Maps between Domain ↔ DTOs                                │
│  • Returns responses                                         │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                   ITaskRepository                            │
│             (InMemoryTaskRepository impl)                    │
│                                                              │
│  • CRUD operations                                           │
│  • Uses ConcurrentDictionary<Guid, TaskItem>                 │
│  • Thread-safe storage                                       │
└─────────────────────────┬────────────────────────────────────┘
                          │
                          ▼
┌──────────────────────────────────────────────────────────────┐
│                  ConcurrentDictionary                        │
│              (Thread-safe in-memory storage)                 │
│                                                              │
│  • Key: Guid (Task ID)                                       │
│  • Value: TaskItem (Rich domain model)                       │
└──────────────────────────────────────────────────────────────┘
```

### Clean 3-Layer Architecture

```
┌─────────────┐
│ Controller  │  ← Thin (routing, validation, HTTP concerns)
└─────────────┘
       ↓
┌─────────────┐
│  Service    │  ← Business logic lives here
└─────────────┘
       ↓
┌─────────────┐
│ Repository  │  ← Data access abstraction
└─────────────┘
```

---

## 📁 Project Structure

<<<<<<< Updated upstream
**Request/Response Examples:**
# create POST /api/v1/tasks
{
  "description": "Complete PathLock assignment"
}
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Complete PathLock assignment",
  "isCompleted": false,
  "createdAt": "2025-10-31T08:04:17Z",
  "completedAt": null
}

=======
```
Assignment1/
│
├── TaskManagerAPI/                       # Backend (.NET 8)
│   ├── Controllers/
│   │   └── TasksController.cs            # REST endpoints
│   │
│   ├── Services/
│   │   ├── ITaskService.cs               # Service interface
│   │   └── TaskService.cs                # Business logic
│   │
│   ├── Repositories/
│   │   ├── ITaskRepository.cs            # Repository interface
│   │   └── InMemoryTaskRepository.cs     # ConcurrentDictionary storage
│   │
│   ├── Models/
│   │   ├── Domain/
│   │   │   └── TaskItem.cs               # Rich domain model
│   │   ├── DTOs/
│   │   │   └── TaskDtos.cs               # Request/Response models
│   │   └── TaskMappingExtensions.cs      # Domain ↔ DTO mapping
│   │
│   ├── Validators/
│   │   └── TaskValidators.cs             # FluentValidation rules
│   │
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs # Global error handling
│   │
│   ├── Program.cs                         # DI & app configuration
│   └── appsettings.json                   # Configuration
│
├── task-manager-frontend/                 # Frontend (React + TypeScript)
│   ├── src/
│   │   ├── components/
│   │   ├── hooks/
│   │   └── App.tsx
│   └── package.json
│
└── README.md                               # This file
>>>>>>> Stashed changes
```
### Get All Tasks
**GET** `/api/v1/tasks`
**Response (200 OK):**
{
"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
"description": "Complete PathLock assignment",
"isCompleted": false,
"createdAt": "2025-10-31T08:04:17Z",
"completedAt": null
## Update Task
**PUT** `/api/v1/tasks/{id}`

<<<<<<< Updated upstream
**Request:**
{
"description": "Updated task description",
"isCompleted": true
}
**Response (200 OK):**
{
"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
"description": "Updated task description",
"isCompleted": true,
"createdAt": "2025-10-31T08:04:17Z",
"completedAt": "2025-10-31T08:15:32Z"
}
### Delete Task
**DELETE** `/api/v1/tasks/{id}`

**Response (204 No Content)**
---

 ### Project Structure
TaskManagerAPI/
├── Controllers/
│   └── TasksController.cs           # Thin REST controllers (GET, POST, PUT, DELETE)
│
├── Services/
│   ├── ITaskService.cs              # Service interface
│   └── TaskService.cs               # Business logic implementation
│
├── Repositories/
│   ├── ITaskRepository.cs           # Repository interface
│   └── InMemoryTaskRepository.cs    # In-memory storage (ConcurrentDictionary)
│
├── Models/
│   ├── Domain/
│   │   └── TaskItem.cs              # Rich domain model (private setters, behavior)
│   │
│   └── DTOs/
│       ├── CreateTaskRequest.cs     # Request DTOs
│       ├── UpdateTaskRequest.cs
│       ├── TaskResponse.cs          # Response DTOs
│       └── TaskMappingExtensions.cs # Domain ↔ DTO mapping
│
├── Validators/
│   ├── CreateTaskValidator.cs       # FluentValidation validators
│   └── UpdateTaskValidator.cs
│
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs  # Global error handling
│
├── Program.cs                        # App configuration & DI setup
├── appsettings.json                  # Configuration
└── TaskManagerAPI.csproj             # Project file



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
=======
---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (8.0.415+)
- [Node.js 18+](https://nodejs.org/) (for frontend)

### Backend Setup
>>>>>>> Stashed changes

1. **Navigate to the API folder**
   ```bash
   cd Assignment1/TaskManagerAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the API**
   - **API Base:** `http://localhost:5039/api/v1`
   - **Swagger UI:** `http://localhost:5039`
   - **Health Check:** `http://localhost:5039/health`

### Frontend Setup

1. **Navigate to the frontend folder**
   ```bash
   cd Assignment1/task-manager-frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Run development server**
   ```bash
   npm run dev
   ```

4. **Access the app**
   - **Frontend:** `http://localhost:5173`

---

## 📊 API Endpoints

### Task Operations

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| **GET** | `/api/v1/tasks` | Get all tasks | - |
| **GET** | `/api/v1/tasks/{id}` | Get task by ID | - |
| **POST** | `/api/v1/tasks` | Create new task | `{ "description": "string" }` |
| **PUT** | `/api/v1/tasks/{id}` | Update task | `{ "description": "string", "isCompleted": bool }` |
| **DELETE** | `/api/v1/tasks/{id}` | Delete task | - |
| **PATCH** | `/api/v1/tasks/{id}/toggle` | Toggle completion | - |

### Request/Response Examples

#### Create Task
```http
POST /api/v1/tasks
Content-Type: application/json

{
  "description": "Complete PathLock assignment"
}
```

**Response (201 Created):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Complete PathLock assignment",
  "isCompleted": false,
  "createdAt": "2025-10-31T10:00:00Z",
  "completedAt": null
}
```

#### Get All Tasks
```http
GET /api/v1/tasks
```

**Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "description": "Complete PathLock assignment",
    "isCompleted": false,
    "createdAt": "2025-10-31T10:00:00Z",
    "completedAt": null
  }
]
```

#### Toggle Task Completion
```http
PATCH /api/v1/tasks/{id}/toggle
```

**Response (200 OK):**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Complete PathLock assignment",
  "isCompleted": true,
  "createdAt": "2025-10-31T10:00:00Z",
  "completedAt": "2025-10-31T11:30:00Z"
}
```

---

## 🛠️ Tech Stack

### Backend
- **.NET 8.0** - Latest LTS framework
- **ASP.NET Core Web API** - RESTful endpoints
- **FluentValidation** - Request validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **ConcurrentDictionary** - Thread-safe in-memory storage

### Frontend
- **React 18** - UI library
- **TypeScript** - Type safety
- **TanStack Query (React Query)** - Server state management
- **Axios** - HTTP client
- **Tailwind CSS** - Utility-first styling
- **Vite** - Build tool
- **Lucide React** - Icon library

---

## 🎨 Frontend Features

### UI Capabilities
- ✅ Create, update, delete tasks
- ✅ Toggle task completion with checkboxes
- ✅ Search tasks in real-time
- ✅ Filter by status (All/Active/Completed)
- ✅ Bulk select and delete
- ✅ Offline persistence with localStorage
- ✅ Clean, modern UI with Tailwind CSS
- ✅ Loading states and error handling
- ✅ Responsive design

### State Management
- **Server State:** TanStack Query (caching, auto-refetch)
- **Client State:** React hooks
- **Persistence:** localStorage backup

---

## 🔒 Error Handling & Validation

### Global Exception Handling
All exceptions are caught by `ExceptionHandlingMiddleware` and return consistent error responses:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Task with ID '...' not found",
  "traceId": "00-123abc..."
}
```

### FluentValidation Rules
- **Description:**
  - Required
  - Maximum 500 characters
  - Not empty/whitespace

### HTTP Status Codes
- **200 OK** - Successful GET, PUT, PATCH
- **201 Created** - Successful POST
- **204 No Content** - Successful DELETE
- **400 Bad Request** - Validation failure
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Unexpected errors

---

## 📝 Logging

Structured logging with **Serilog** to console:

```
[10:00:00 INF] Creating new task: Complete PathLock assignment
[10:00:01 INF] Task created successfully: 3fa85f64-5717-4562-b3fc-2c963f66afa6
[10:00:10 INF] Toggling task completion: 3fa85f64-5717-4562-b3fc-2c963f66afa6
```

---

## 🎯 Why This Architecture?

### Service Layer Benefits
1. **Simplicity** - Easy to understand and maintain
2. **Testability** - Services are easily mockable
3. **Separation of Concerns** - Clear boundaries between layers
4. **Right-sized** - Not over-engineered for simple CRUD

### When to Use CQRS Instead?
CQRS would be overkill for this because:
- ❌ No complex read models needed
- ❌ No event sourcing requirements
- ❌ No separate read/write scaling needs
- ❌ Simple domain with single aggregate

**CQRS is used in Assignment 2 & 3** where complexity justifies it:
- ✅ Multiple aggregates (Projects, Tasks, Users)
- ✅ Complex queries (task dependencies, scheduling)
- ✅ Command validation pipelines
- ✅ Scalability requirements

---

## 🚀 Production Considerations

### What's Included
- ✅ Global exception handling
- ✅ Request validation
- ✅ Structured logging
- ✅ CORS configuration
- ✅ API documentation (Swagger)
- ✅ Thread-safe storage
- ✅ Health checks

### What Would Be Added for Production
- [ ] Database (SQL Server/PostgreSQL)
- [ ] Authentication & Authorization (JWT)
- [ ] Rate limiting
- [ ] API key management
- [ ] Distributed caching (Redis)
- [ ] Monitoring & metrics (Application Insights)
- [ ] Containerization (Docker)
- [ ] CI/CD pipeline

---

## ✅ Assignment 1 Checklist

- [x] Clean Service Layer architecture
- [x] Rich domain models (TaskItem)
- [x] Thread-safe in-memory storage
- [x] FluentValidation for requests
- [x] Global exception handling
- [x] Structured logging with Serilog
- [x] Swagger API documentation
- [x] CORS configuration
- [x] Health check endpoint
- [x] RESTful best practices
- [x] Frontend integration (React + TypeScript)
- [x] Production-ready error responses

---

**Assignment 1 Status:** ✅ **Complete and Production-Ready**

For Assignments 2 & 3 (Project Manager with Dependencies & Scheduling), see:
- **Backend:** `../Assignment2/README.md`
- **Frontend:** `../project-manager-frontend/README.md`
- **Main Overview:** `../README.md`
