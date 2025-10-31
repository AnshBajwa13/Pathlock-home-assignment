# PathLock Project Manager

A modern, enterprise-grade project management application demonstrating **Clean Architecture**, **CQRS pattern**, and **React + TypeScript** best practices. Built with scalability, maintainability, and user experience as top priorities.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-blue)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.x-blue)](https://www.typescriptlang.org/)

---

## 📑 Table of Contents
- [Quick Start](#-quick-start)
- [Architecture Overview](#️-architecture-overview)
- [Why These Technologies?](#-why-these-technologies)
- [Architecture Decisions & Rationale](#-architecture-decisions--rationale)
- [Features](#-features)
- [API Documentation](#-api-documentation)
- [Database Schema](#️-database-schema)
- [Security & Validation](#-security--validation)
- [Testing](#-testing)
- [Project Structure](#-project-structure)

---

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- Git

### Backend Setup & Run
```bash
cd Assignment2
dotnet restore
dotnet run --project src/API/API.csproj
```
**API:** `http://localhost:5001`  
**Swagger UI:** `http://localhost:5001/swagger`

### Frontend Setup & Run
```bash
cd project-manager-frontend
npm install
npm run dev
```
**App:** `http://localhost:5175`

---

## 🏗️ Architecture Overview

This application implements **Clean Architecture** with **CQRS** (Command Query Responsibility Segregation) to ensure clear separation of concerns, high testability, and enterprise-level maintainability.

### System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                      FRONTEND (React + TypeScript)                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────────┐  │
│  │   Pages      │  │  Features    │  │  Components              │  │
│  │              │  │  - Auth      │  │  - Layout, Navbar        │  │
│  │ - Dashboard  │  │  - Projects  │  │  - Toast, Modals         │  │
│  │ - Projects   │  │  - Tasks     │  │                          │  │
│  └──────────────┘  └──────────────┘  └──────────────────────────┘  │
│         │                  │                        │               │
│         └──────────────────┼────────────────────────┘               │
│                            │                                        │
│         ┌─────────────────────────────────────┐                    │
│         │  State Management                   │                    │
│         │  - Zustand (Auth State)             │                    │
│         │  - TanStack Query (Server Cache)    │                    │
│         │  - React Hook Form + Zod (Forms)    │                    │
│         └─────────────────────────────────────┘                    │
└─────────────────────────────────────────────────────────────────────┘
                                 │
                      HTTP/REST API (JSON)
                                 │
┌─────────────────────────────────────────────────────────────────────┐
│                         BACKEND (.NET 8)                             │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                      API Layer                                │  │
│  │  Controllers → Middleware → JWT Auth → Swagger               │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                 │                                   │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                   Application Layer (CQRS)                    │  │
│  │                                                               │  │
│  │   Commands (Write)          Queries (Read)                   │  │
│  │   ┌─────────────────┐       ┌─────────────────┐             │  │
│  │   │ CreateProject   │       │ GetProjects     │             │  │
│  │   │ UpdateProject   │       │ GetProjectById  │             │  │
│  │   │ DeleteProject   │       │ GetTasksByProj  │             │  │
│  │   │ CreateTask      │       └─────────────────┘             │  │
│  │   │ UpdateTask      │              │                        │  │
│  │   │ ToggleTask      │              │                        │  │
│  │   └─────────────────┘              │                        │  │
│  │          │                          │                        │  │
│  │          └──────────┬───────────────┘                        │  │
│  │                     │                                        │  │
│  │            ┌────────────────────┐                           │  │
│  │            │  MediatR Pipeline  │                           │  │
│  │            │  - Validation      │                           │  │
│  │            │  - Logging         │                           │  │
│  │            │  - Error Handling  │                           │  │
│  │            └────────────────────┘                           │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                 │                                   │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                      Domain Layer                             │  │
│  │   Entities: User, Project, TaskItem, RefreshToken            │  │
│  │   Interfaces: IRepository<T>, IAuditableEntity               │  │
│  │   Exceptions: Business domain exceptions                     │  │
│  │   ⭐ NO DEPENDENCIES - Pure Business Logic                   │  │
│  └──────────────────────────────────────────────────────────────┘  │
│                                 │                                   │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │                   Infrastructure Layer                        │  │
│  │   - ApplicationDbContext (EF Core)                           │  │
│  │   - Repository<T> Implementations                            │  │
│  │   - JwtService (Token generation/validation)                 │  │
│  │   - PasswordHasher (BCrypt)                                  │  │
│  │   - Database Migrations                                      │  │
│  └──────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                                 │
                          ┌──────────────┐
                          │   SQLite DB  │
                          │ miniproject  │
                          │ manager.db   │
                          └──────────────┘
```

### CQRS Flow Diagram

```
User Action → Frontend
                │
                ├─ CREATE/UPDATE/DELETE (Command)
                │    │
                │    ├→ POST/PUT/PATCH/DELETE Request
                │    │
                │    ├→ [API Controller]
                │    │
                │    ├→ [MediatR] → CommandHandler
                │    │                    │
                │    │             FluentValidation
                │    │                    │
                │    │             Business Logic
                │    │                    │
                │    │             Repository.Add/Update/Delete
                │    │                    │
                │    │             DbContext.SaveChanges()
                │    │                    │
                │    └←──────────  Return Result<DTO>
                │
                └─ READ (Query)
                     │
                     ├→ GET Request
                     │
                     ├→ [API Controller]
                     │
                     ├→ [MediatR] → QueryHandler
                     │                    │
                     │             Repository.GetAll/GetById
                     │                    │
                     │             Map to DTO
                     │                    │
                     └←──────────  Return Result<DTO>
```

### Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|----------------|--------------|
| **Domain** | Core business entities, interfaces, domain exceptions | ❌ None (Pure C#) |
| **Application** | Use cases (Commands/Queries), DTOs, Validation, Business rules | Domain, MediatR, FluentValidation |
| **Infrastructure** | Database, EF Core, External services (JWT, BCrypt), Logging | Domain, Application, EF Core |
| **API** | Controllers, Middleware, DI setup, Swagger config | All layers |

---

## 💡 Why These Technologies?

### Backend Architecture Choices

#### ✅ **Clean Architecture**
**Why?** 
- **Independence**: Business logic (Domain) has zero dependencies on frameworks, UI, or database
- **Testability**: Can test domain and application logic without infrastructure concerns
- **Flexibility**: Easy to swap databases, change UI frameworks, or migrate to microservices
- **Maintainability**: Clear boundaries make code easier to understand and modify

**When Required?**
- When building enterprise applications expected to evolve over years
- When multiple teams work on different layers simultaneously
- When you need to swap infrastructure (e.g., SQLite → PostgreSQL)

#### ✅ **CQRS (Command Query Responsibility Segregation)**
**Why?**
- **Separation**: Write operations (Commands) separated from read operations (Queries)
- **Performance**: Can optimize reads and writes independently
- **Scalability**: Easy to scale read replicas separately from write databases
- **Clarity**: Business operations are explicit (CreateProjectCommand vs GetProjectsQuery)

**Example:**
```csharp
// Command (Write) - Validates and changes state
public class CreateTaskCommand : IRequest<Result<TaskDto>>
{
    public string Title { get; set; }  // Required
    public string? Description { get; set; }  // Optional
    // Validator ensures Title is 1-200 chars
}

// Query (Read) - No validation needed, just retrieval
public class GetTasksByProjectQuery : IRequest<Result<List<TaskDto>>>
{
    public Guid ProjectId { get; set; }
}
```

#### ✅ **MediatR (Mediator Pattern)**
**Why?**
- **Decoupling**: Controllers don't know about handler implementations
- **Single Responsibility**: Each handler does ONE thing (e.g., create task)
- **Pipeline**: Automatic validation, logging, and error handling via behaviors

**Without MediatR:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskDto dto)
{
    // Controller has too many responsibilities
    var validator = new CreateTaskValidator();
    var validationResult = validator.Validate(dto);
    if (!validationResult.IsValid) return BadRequest();
    
    var task = new TaskItem { Title = dto.Title, ... };
    await _repository.AddAsync(task);
    return Ok(MapToDto(task));
}
```

**With MediatR:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskCommand command)
{
    // Clean! Validation, logging happens in pipeline
    var result = await _mediator.Send(command);
    return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Errors);
}
```

#### ✅ **FluentValidation**
**Why?**
- **Expressive**: Readable validation rules
- **Reusable**: Same validator for API and potential CLI/Background jobs
- **Testable**: Can unit test validators independently
- **Composable**: Can chain rules, create custom validators

**Example:**
```csharp
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        // Title is required, 1-200 characters
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        
        // Description is optional, but if provided max 1000 chars
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        
        // DueDate can't be in the past
        When(x => x.DueDate.HasValue, () =>
        {
            RuleFor(x => x.DueDate!.Value)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Due date cannot be in the past");
        });
    }
}
```

**When Required?**
- When validation logic is complex (e.g., password regex, cross-field validation)
- When you want validation logic separate from business logic
- When the same validation needs to be reused across different entry points

#### ✅ **JWT (JSON Web Tokens) + Refresh Tokens**
**Why?**
- **Stateless**: Server doesn't store session data (scales horizontally)
- **Short-lived Access Tokens (60 min)**: Limits damage if token is stolen
- **Long-lived Refresh Tokens (7 days)**: User doesn't re-login frequently
- **Secure**: Stored in httpOnly cookies (frontend version uses localStorage with XSS protection awareness)

**Flow:**
```
1. Login → Server returns AccessToken (60 min) + RefreshToken (7 days)
2. Client stores both tokens
3. Each API call: Authorization: Bearer {AccessToken}
4. AccessToken expires → Client calls /auth/refresh with RefreshToken
5. Server validates RefreshToken → Issues new AccessToken
6. Repeat until RefreshToken expires (7 days) → Force re-login
```

**When Required?**
- When building SPAs (Single Page Applications) with separate backend
- When you need mobile app support (mobile apps can't use cookies easily)
- When horizontal scaling is needed (no server-side sessions)

#### ✅ **SQLite for Development**
**Why?**
- **Zero Config**: No database server installation required
- **File-Based**: Easy to reset (just delete .db file)
- **Portable**: Can share with .db file
- **EF Core Support**: Can swap to SQL Server/PostgreSQL with 1 line change

**Production Swap:**
```csharp
// SQLite (Development)
options.UseSqlite("Data Source=miniprojectmanager.db");

// PostgreSQL (Production) - Same code, just change this line
options.UseNpgsql(connectionString);
```

---

### Frontend Architecture Choices

#### ✅ **React + TypeScript**
**Why?**
- **Type Safety**: Catch errors at compile-time, not runtime
- **IntelliSense**: Auto-completion for props, state, API responses
- **Refactor-Friendly**: Rename variables safely across entire codebase
- **Documentation**: Types serve as inline documentation

**Example:**
```typescript
// TypeScript prevents mistakes
interface Task {
  id: string;
  title: string;  // Required
  description?: string;  // Optional
  dueDate: string | null;
}

function TaskCard({ task }: { task: Task }) {
  // TypeScript knows task.title exists
  // TypeScript knows task.description might be undefined
  return <div>{task.title}</div>;
}
```

#### ✅ **TanStack Query (React Query)**
**Why?**
- **Automatic Caching**: Fetch once, use everywhere
- **Background Refetching**: Keep data fresh automatically
- **Optimistic Updates**: Update UI before server responds
- **Less Boilerplate**: No manual loading/error/success states

**Without React Query:**
```typescript
const [projects, setProjects] = useState([]);
const [loading, setLoading] = useState(false);
const [error, setError] = useState(null);

useEffect(() => {
  setLoading(true);
  fetch('/api/projects')
    .then(res => res.json())
    .then(setProjects)
    .catch(setError)
    .finally(() => setLoading(false));
}, []);
```

**With React Query:**
```typescript
const { data: projects, isLoading, error } = useQuery({
  queryKey: ['projects'],
  queryFn: () => projectsApi.getProjects()
});
// Automatic caching, refetching, and background updates!
```

**When Required?**
- When building data-heavy applications with lots of API calls
- When you need real-time-ish data (automatic background refetch)
- When you want to eliminate 80% of state management boilerplate

#### ✅ **Zustand for Auth State**
**Why?**
- **Lightweight**: 1KB vs 20KB (Redux)
- **Simple API**: No boilerplate reducers/actions
- **TypeScript-First**: Excellent TypeScript support
- **Persistent**: Easy localStorage integration

**Example:**
```typescript
export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      setAuth: (user, token) => set({ user, token }),
      logout: () => set({ user: null, token: null }),
    }),
    { name: 'auth-storage' }  // Auto-saves to localStorage
  )
);
```

**When Required?**
- When you need global client-side state (like authentication)
- When Redux feels too heavy for simple state
- When you want TypeScript-first state management

#### ✅ **React Hook Form + Zod**
**Why?**
- **Performance**: Uncontrolled inputs (fewer re-renders)
- **Type-Safe Validation**: Zod generates TypeScript types from schema
- **Less Boilerplate**: No manual onChange handlers
- **Consistent Validation**: Same schema validates on client and can mirror server

**Example:**
```typescript
const taskSchema = z.object({
  title: z.string()
    .min(1, 'Title is required')
    .max(200, 'Title must be less than 200 characters'),
  description: z.string()
    .max(1000, 'Description must be less than 1000 characters')
    .optional(),
});

type TaskFormData = z.infer<typeof taskSchema>;  // Auto-generated type!

const { register, handleSubmit, formState: { errors } } = useForm<TaskFormData>({
  resolver: zodResolver(taskSchema),
});
```

**When Required?**
- When building forms with complex validation
- When you want type-safe form data
- When performance matters (large forms)

#### ✅ **Tailwind CSS**
**Why?**
- **Utility-First**: Build custom designs without leaving HTML
- **No CSS Bloat**: Only ships CSS you actually use (tree-shaking)
- **Consistent Design**: Predefined spacing, colors, breakpoints
- **Fast Iteration**: Change styles without switching files

**Example:**
```tsx
// Tailwind (Inline, Fast Iteration)
<button className="bg-primary hover:bg-primary/90 text-white px-4 py-2 rounded-lg transition">
  Save
</button>

// Traditional CSS (Separate file, context switching)
<button className="save-button">Save</button>
// In styles.css: .save-button { background: ...; padding: ...; }
```

**When Required?**
- When you want rapid UI development
- When consistency across components is important
- When bundle size matters (Tailwind purges unused styles)

---

## 🎯 Architecture Decisions & Rationale

### **Why CQRS for Assignment 2 (But NOT Assignment 1)?**

**Assignment 1: Simple Service Layer** ✅
```
User → Controller → TaskService → Repository → Database
```
- Single aggregate (Tasks only)
- Simple CRUD operations
- No complex authorization
- No multi-entity workflows
- **Verdict**: CQRS would be over-engineering

**Assignment 2: CQRS Pattern** ✅
```
User → Controller → MediatR → Commands/Queries → Handlers → Database
```
- **Multiple aggregates**: User → Project → Task (hierarchical)
- **Complex authorization**: User owns Project, Project contains Tasks
- **Different write/read needs**: 
  - Writes: Validate ownership, enforce business rules
  - Reads: Optimize with includes, pagination, filtering
- **Cross-aggregate operations**: Can't delete project with tasks
- **Verdict**: CQRS provides clear boundaries for complex domain

**Key Differences:**

| Aspect | Assignment 1 | Assignment 2 |
|--------|-------------|-------------|
| Aggregates | 1 (Tasks) | 3 (Users, Projects, Tasks) |
| Authorization | None | Multi-level (user → project → task) |
| Business Rules | Minimal | Complex (ownership, soft delete, constraints) |
| Write/Read Split | Not needed | Beneficial (different optimization needs) |
| Pattern Choice | Service Layer | CQRS |

**Source**: Martin Fowler - "CQRS is useful when the domain is complex, but it's overkill for simple CRUD" ([martinfowler.com/bliki/CQRS.html](https://martinfowler.com/bliki/CQRS.html))

---

### **Assignment 3 Integrates WITH Assignment 2 (Not Separate)?**

**Evidence from Requirements:**

1. **Title**: "Required Enhancements **Linked with** Mini Project Manager"
   - "Linked with" = integrated, not separate

2. **Endpoint Pattern**: `/api/v1/projects/{projectId}/schedule`
   - Uses `{projectId}` from Assignment 2's database
   - Not a standalone `/api/schedule` endpoint

3. **Time Estimate**: "8-12 hours for Assignment 2 & 3"
   - Combined estimate suggests single codebase

4. **Reuses Existing Infrastructure**:
   - Same JWT authentication
   - Same database (tasks, projects)
   - Same Clean Architecture layers
   - Same user authorization

**Integration Strategy:**
```
Assignment 2 (Base):
- Domain: TaskItem, Project entities
- Application: CQRS commands/queries
- Infrastructure: DbContext, JWT

Assignment 3 (Enhancement):
- Domain: TaskDependency entity, EstimatedHours field
- Application: SchedulingAlgorithm service, GenerateScheduleCommand
- API: ScheduleController
- Tests: 8 new scheduling algorithm tests
```

**Result**: Single cohesive application, not microservices

---

### **Scheduling Algorithm: Why Topological Sort + CPM?**

**Problem**: Order tasks automatically based on dependencies and time estimates.

**Solution**: Combine two complementary algorithms.

#### **1. Topological Sort (Kahn's Algorithm)**

**Purpose**: Order tasks respecting dependencies  
**Time Complexity**: O(V + E) where V = tasks, E = dependencies  
**Space Complexity**: O(V + E)

**How it works:**
1. Calculate in-degree (# of dependencies) for each task
2. Start with tasks that have zero dependencies (in-degree = 0)
3. Process task, reduce in-degree of its dependents
4. Repeat until all tasks processed
5. If tasks remain, circular dependency exists ⚠️

**Example:**
```
Dependencies:
Task A → Task B → Task D
Task A → Task C → Task D

Topological Order: A, B, C, D (or A, C, B, D)
```

**Why Kahn's Algorithm?**
- ✅ Natural cycle detection (tasks remain = cycle exists)
- ✅ BFS-based (easy to understand)
- ✅ O(V+E) optimal time complexity

**Industry Usage**: Git (commit ordering), Gradle/Maven (build dependencies), Make (target dependencies)

#### **2. Critical Path Method (CPM)**

**Purpose**: Identify scheduling bottlenecks and calculate flexibility  
**Time Complexity**: O(V + E)  
**Invented**: 1957 by DuPont and Remington Rand for project management

**How it works:**

**Forward Pass** (Calculate Earliest Times):
```
ES (Earliest Start) = max(EF of all predecessors)
EF (Earliest Finish) = ES + duration
```

**Backward Pass** (Calculate Latest Times):
```
LF (Latest Finish) = min(LS of all successors)
LS (Latest Start) = LF - duration
```

**Slack Calculation**:
```
Slack = LS - ES (or LF - EF)
```

**Critical Path**: All tasks with Slack = 0

**Example:**
```
Task A (3h) → Task B (2h) → Task D (4h)
Task A (3h) → Task C (5h) → Task D (4h)

Forward Pass:
A: ES=0, EF=3
B: ES=3, EF=5
C: ES=3, EF=8
D: ES=8, EF=12  (waits for C, the longer path)

Backward Pass:
D: LF=12, LS=8
C: LF=8, LS=3
B: LF=8, LS=6   (can start later without delaying D)
A: LF=3, LS=0

Slack:
A: 0 hours (critical)
B: 3 hours (can delay by 3h)
C: 0 hours (critical)
D: 0 hours (critical)

Critical Path: A → C → D (9 hours total)
```

**Why CPM?**
- ✅ Identifies bottlenecks (tasks that can't be delayed)
- ✅ Calculates scheduling flexibility (slack time)
- ✅ Used by Microsoft Project, Jira, Asana for Gantt charts
- ✅ Industry standard since 1950s

**Real-World Applications**:
- Construction project management
- Software release planning
- Manufacturing scheduling
- Event planning

---

### **Smart Defaults: Tasks Without Time Estimates**

**Problem**: User creates task but forgets to add estimated hours.

**Bad Solution**: Crash or skip the task ❌

**Our Solution**: Default to 1 hour + warning ✅

**Code:**
```csharp
int GetEstimate(TaskItem task) => task.EstimatedHours ?? 1;

if (tasksWithoutEstimates.Any())
{
    result.Warnings.Add(
        $"{tasksWithoutEstimates.Count} task(s) have no estimated hours. " +
        "Assuming 1 hour for scheduling."
    );
}
```

**Why 1 hour?**
- ✅ Conservative estimate (most tasks take 1-4 hours)
- ✅ Better than 0 (schedule still works)
- ✅ User gets **transparent warning**
- ✅ Industry standard (Microsoft Project, Jira do the same)

**Principle**: **Graceful degradation** - system works with incomplete data

**Source**: Nielsen Norman Group - "Always provide a way out" (error prevention principle)

---

### **Edge Cases Handled in Scheduling Algorithm**

✅ **Empty Task Lists**
```csharp
if (tasks.Count == 0) {
    result.Warnings.Add("No tasks to schedule.");
    return result;
}
```

✅ **Circular Dependencies** (A → B → C → A)
```csharp
if (result.Count != tasks.Count) {
    return null; // Cycle detected
}
// Handler returns warning: "Circular dependency detected!"
```

✅ **Disconnected Task Graphs**
```
Task A → Task B
Task C → Task D  (separate chain)
```
Algorithm handles both chains independently.

✅ **Single Task Projects**
```
Order: [Task A]
Critical Path: [Task A]
Slack: 0
```

✅ **Parallel Tasks** (No dependencies)
```
Task A (5h), Task B (5h), Task C (5h)
All are critical (slack = 0)
```

✅ **Overdue Tasks**
```csharp
var overdueTasks = tasks.Where(t => 
    t.DueDate.HasValue && t.DueDate < DateTime.UtcNow
).ToList();

if (overdueTasks.Any()) {
    result.Warnings.Add($"{overdueTasks.Count} task(s) have past due dates.");
}
```

---

### **Test Strategy & Coverage**

**40 Tests Organized by Layer:**

**Domain Layer** (Implicit - via Application tests):
- Entity validation
- Business rules enforcement
- Relationship integrity

**Application Layer** (32 tests):
- **Authentication** (8 tests):
  - Login command handler
  - Register command handler
  - Login validator (email format, password length)
  - Register validator (password complexity, duplicate email)
  
- **Projects** (10 tests):
  - Create/Update/Delete command handlers
  - Get projects query (pagination, filtering)
  - Get project by ID query
  - Authorization checks (user owns project)
  - Validators (title length, description)

- **Tasks** (14 tests):
  - Create/Update/Delete/Toggle command handlers
  - Get tasks by project query
  - Authorization checks (user owns project owns task)
  - Validators (title, description, due date)

**Scheduling Algorithm** (8 tests - Assignment 3):
- ✅ Empty task list handling
- ✅ Single task scheduling
- ✅ Linear dependency chain (A → B → C)
- ✅ Circular dependency detection
- ✅ Parallel tasks (all critical)
- ✅ Tasks without estimates (smart defaults + warnings)
- ✅ Complex diamond dependency graph with CPM validation
- ✅ Overdue task warnings

**Coverage**: ~80% (focused on business logic, not DTOs/entities)

**Test Philosophy**:
- Unit tests for complex logic (handlers, validators, algorithms)
- Integration tests would test API endpoints (not included yet)
- E2E tests would test full workflows (future work)

**Why xUnit + FluentAssertions?**
- xUnit: Industry standard for .NET (used by .NET Core itself)
- FluentAssertions: Readable assertions (`result.Should().BeOfType<ErrorResult>()`)

---

### **Security Considerations**

**JWT Token Security:**
```csharp
// Short-lived access tokens (60 min)
var accessToken = _tokenService.GenerateAccessToken(user);

// Long-lived refresh tokens (7 days)
var refreshToken = await _tokenService.GenerateRefreshToken(user);

// Refresh token stored in database (can revoke)
await _context.RefreshTokens.AddAsync(refreshToken);
```

**Why?**
- Access token stolen? Expires in 60 min ✅
- Refresh token stolen? Can revoke in database ✅
- User logout? Revoke all refresh tokens ✅

**Password Hashing:**
```csharp
// BCrypt with salt (cost factor: 12)
var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, 12);
```

**Why BCrypt?**
- ✅ Adaptive (can increase cost over time)
- ✅ Salted automatically (prevents rainbow tables)
- ✅ Slow by design (prevents brute force)

**Authorization:**
```csharp
// Every command verifies user owns the resource
var userId = _currentUserService.UserId;
var project = await _context.Projects
    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.UserId == userId);

if (project == null)
    throw new KeyNotFoundException("Project not found or access denied");
```

**Soft Delete:**
```csharp
// Don't actually delete from database
task.IsDeleted = true;
task.DeletedAt = DateTime.UtcNow;

// Queries filter out deleted items
var tasks = await _context.Tasks
    .Where(t => !t.IsDeleted)
    .ToListAsync();
```

**Why?**
- ✅ Data recovery possible
- ✅ Audit trail maintained
- ✅ Accidental deletes reversible

---

## ✨ Features

### Core Functionality
- ✅ **User Authentication** - JWT-based secure login/register
- ✅ **Projects Management** - Full CRUD with soft delete
- ✅ **Tasks Management** - Create, update, delete, toggle completion
- ✅ **Task Attributes**:
  - **Title** (required, 1-200 chars) - Short scannable task name
  - **Description** (optional, max 1000 chars) - Detailed task notes
  - **Due Date** (optional) - Track deadlines
  - **Completion Status** - Mark tasks done/undone
- ✅ **Pagination** - Efficient data loading (9 projects per page)
- ✅ **Progress Tracking** - Real-time completion percentages

### Advanced UX Features
- 🎯 **Quick Tasks Preview** - Expandable task list on project cards (Jira-style)
- 🔄 **Smart Task Sorting** - Overdue → Due Soon → Normal → Completed
- 🚨 **Visual Indicators**:
  - Red border + ⚠️ icon for overdue tasks
  - Orange border for tasks due within 3 days
  - Green checkmarks for completed tasks
- 🎨 **Status Badges** - "To Do", "Done", "Overdue", "Due Soon"
- 🔔 **Toast Notifications** - Instant feedback for all actions
- ⏳ **Loading States** - Spinners during async operations
- 📱 **Responsive Design** - Mobile, tablet, desktop optimized
- 🎭 **Empty States** - Helpful prompts when no data exists
- 🔒 **Protected Routes** - Automatic redirect to login if unauthenticated

### 🧠 Smart Scheduler (Assignment 3 Enhancement)
- 📊 **Intelligent Task Scheduling** - Automatic work planning with dependency analysis
- 🔗 **Task Dependencies** - Define which tasks must be completed before others
- ⏱️ **Time Estimation** - Add estimated hours to tasks for accurate scheduling
- 🎯 **Critical Path Analysis** - Identifies tasks that can't be delayed without affecting project completion
- 📈 **CPM Algorithm** - Uses Critical Path Method for optimal task sequencing
- 🔄 **Topological Sorting** - Automatically orders tasks respecting all dependencies
- ⚠️ **Circular Dependency Detection** - Prevents and warns about invalid task relationships
- 📅 **Slack Time Calculation** - Shows flexibility window for each task
- 🛡️ **Smart Defaults** - Tasks without time estimates default to 1 hour with warning
- 📋 **Comprehensive Warnings** - Alerts for circular deps, missing estimates, overdue tasks

**Scheduling Algorithm Features:**
- **Earliest Start/Finish Times** - When tasks can begin at the earliest
- **Latest Start/Finish Times** - Latest possible start without delaying project
- **Float/Slack Calculation** - How much delay a task can tolerate
- **Recommended Execution Order** - Optimal sequence based on dependencies
- **Total Project Duration** - Sum of critical path task hours

---

## 📚 API Documentation

### Swagger UI
Access interactive API documentation at: **http://localhost:5001/swagger**

### Authentication Endpoints (Public)

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response 200 OK:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "dGhpc2lz...",
  "user": {
    "id": "guid",
    "fullName": "John Doe",
    "email": "john@example.com"
  }
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response 200 OK:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "dGhpc2lz...",
  "user": { "id": "...", "fullName": "...", "email": "..." }
}
```

#### Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "dGhpc2lz..."
}

Response 200 OK:
{
  "accessToken": "new_token_here",
  "refreshToken": "new_refresh_token"
}
```

### Projects Endpoints (Authenticated)

**Authentication Required**: Add header `Authorization: Bearer {your_access_token}`

#### Get All Projects (Paginated)
```http
GET /api/projects?pageNumber=1&pageSize=10
Authorization: Bearer {token}

Response 200 OK:
{
  "items": [
    {
      "id": "guid",
      "title": "Website Redesign",
      "description": "Redesign company website with modern UI",
      "taskCount": 5,
      "completedTaskCount": 2,
      "createdAt": "2024-01-15T10:00:00Z",
      "updatedAt": null
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3,
  "totalCount": 25
}
```

#### Get Project by ID (with Tasks Summary)
```http
GET /api/projects/{id}
Authorization: Bearer {token}

Response 200 OK:
{
  "id": "guid",
  "title": "Website Redesign",
  "description": "...",
  "tasks": [
    {
      "id": "task_guid",
      "title": "Design homepage mockup",
      "description": "Create Figma mockups for new homepage",
      "isCompleted": false,
      "dueDate": "2024-02-01T00:00:00Z",
      "createdAt": "2024-01-15T10:00:00Z"
    }
  ],
  "taskCount": 5,
  "completedTaskCount": 2,
  "createdAt": "2024-01-15T10:00:00Z"
}
```

#### Create Project
```http
POST /api/projects
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Mobile App Development",
  "description": "Build iOS and Android apps"
}

Response 201 Created:
{
  "id": "new_guid",
  "title": "Mobile App Development",
  "description": "Build iOS and Android apps",
  "taskCount": 0,
  "completedTaskCount": 0,
  "createdAt": "2024-01-20T14:30:00Z",
  "updatedAt": null
}
```

#### Update Project
```http
PUT /api/projects/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": "guid",
  "title": "Updated Project Title",
  "description": "Updated description"
}

Response 200 OK:
{
  "id": "guid",
  "title": "Updated Project Title",
  "description": "Updated description",
  "taskCount": 5,
  "completedTaskCount": 2,
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-20T15:00:00Z"
}
```

#### Delete Project (Soft Delete)
```http
DELETE /api/projects/{id}
Authorization: Bearer {token}

Response 204 No Content
```

### Tasks Endpoints (Authenticated)

#### Get Tasks by Project
```http
GET /api/tasks/project/{projectId}
Authorization: Bearer {token}

Response 200 OK:
[
  {
    "id": "guid",
    "title": "Design homepage mockup",
    "description": "Create Figma mockups for new homepage",
    "isCompleted": false,
    "completedAt": null,
    "dueDate": "2024-02-01T00:00:00Z",
    "projectId": "project_guid",
    "createdAt": "2024-01-15T10:00:00Z",
    "updatedAt": null
  }
]
```

#### Create Task
```http
POST /api/tasks
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Implement user authentication",
  "description": "Add JWT-based auth with refresh tokens",
  "dueDate": "2024-02-15T00:00:00Z",
  "projectId": "project_guid"
}

Response 201 Created:
{
  "id": "new_task_guid",
  "title": "Implement user authentication",
  "description": "Add JWT-based auth with refresh tokens",
  "isCompleted": false,
  "completedAt": null,
  "dueDate": "2024-02-15T00:00:00Z",
  "projectId": "project_guid",
  "createdAt": "2024-01-20T16:00:00Z",
  "updatedAt": null
}
```

#### Update Task
```http
PUT /api/tasks/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": "task_guid",
  "title": "Updated task title",
  "description": "Updated description",
  "dueDate": "2024-02-20T00:00:00Z"
}

Response 200 OK:
{
  "id": "task_guid",
  "title": "Updated task title",
  "description": "Updated description",
  "isCompleted": false,
  "dueDate": "2024-02-20T00:00:00Z",
  "updatedAt": "2024-01-21T10:00:00Z"
}
```

#### Toggle Task Completion
```http
PATCH /api/tasks/{id}/toggle
Authorization: Bearer {token}

Response 200 OK:
{
  "id": "task_guid",
  "title": "Task title",
  "isCompleted": true,
  "completedAt": "2024-01-21T11:00:00Z",
  "updatedAt": "2024-01-21T11:00:00Z"
}
```

#### Delete Task
```http
DELETE /api/tasks/{id}
Authorization: Bearer {token}

Response 204 No Content
```

### 📅 Schedule Endpoint (Authenticated - Assignment 3 Enhancement)

#### Generate Optimized Task Schedule
```http
POST /api/v1/projects/{projectId}/schedule
Authorization: Bearer {token}

Response 200 OK:
{
  "scheduledTasks": [
    {
      "taskId": "a1b2c3d4-...",
      "title": "Setup Environment",
      "order": 1,
      "estimatedHours": 3,
      "earliestStart": 0,
      "earliestFinish": 3,
      "latestStart": 0,
      "latestFinish": 3,
      "slack": 0,
      "isCritical": true,
      "dependencies": [],
      "dueDate": "2025-11-15T00:00:00Z"
    },
    {
      "taskId": "e5f6g7h8-...",
      "title": "Write Tests",
      "order": 2,
      "estimatedHours": 5,
      "earliestStart": 3,
      "earliestFinish": 8,
      "latestStart": 5,
      "latestFinish": 10,
      "slack": 2,
      "isCritical": false,
      "dependencies": ["a1b2c3d4-..."],
      "dueDate": "2025-11-20T00:00:00Z"
    }
  ],
  "criticalPath": ["a1b2c3d4-...", "i9j0k1l2-..."],
  "totalEstimatedHours": 18,
  "warnings": [
    "2 task(s) have no estimated hours. Assuming 1 hour for scheduling.",
    "1 task(s) have past due dates."
  ]
}
```

**Response Fields Explained:**
- **order**: Recommended execution sequence (1, 2, 3...)
- **earliestStart**: Earliest time (in hours) this task can begin
- **earliestFinish**: Earliest time this task can complete
- **latestStart**: Latest time this task can start without delaying project
- **latestFinish**: Latest time this task can finish without delaying project
- **slack**: Float time - how much delay this task can tolerate (hours)
- **isCritical**: If true, this task is on the critical path (cannot be delayed)
- **criticalPath**: List of task IDs that form the longest dependency chain
- **totalEstimatedHours**: Total duration of the critical path

**Algorithm Details:**
- Uses **Topological Sort (Kahn's Algorithm)** for dependency ordering
- Uses **Critical Path Method (CPM)** for schedule optimization
- Detects circular dependencies and returns warnings
- Tasks without estimates default to 1 hour

### Error Responses

All endpoints may return these error responses:

```http
400 Bad Request:
{
  "errors": {
    "Title": ["Title is required", "Title must not exceed 200 characters"]
  }
}

401 Unauthorized:
{
  "message": "Invalid token"
}

404 Not Found:
{
  "message": "Project not found"
}

500 Internal Server Error:
{
  "message": "An unexpected error occurred"
}
```

---

## 🗄️ Database Schema

### Tables

**Users**
```sql
CREATE TABLE Users (
  Id TEXT PRIMARY KEY,
  FullName TEXT NOT NULL,
  Email TEXT NOT NULL UNIQUE,
  PasswordHash TEXT NOT NULL,
  CreatedAt TEXT NOT NULL,
  UpdatedAt TEXT,
  IsDeleted INTEGER NOT NULL DEFAULT 0
);
```

**Projects**
```sql
CREATE TABLE Projects (
  Id TEXT PRIMARY KEY,
  Title TEXT NOT NULL,
  Description TEXT,
  UserId TEXT NOT NULL,
  CreatedAt TEXT NOT NULL,
  UpdatedAt TEXT,
  IsDeleted INTEGER NOT NULL DEFAULT 0,
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

**Tasks**
```sql
CREATE TABLE Tasks (
  Id TEXT PRIMARY KEY,
  Title TEXT NOT NULL,              -- Required (1-200 chars)
  Description TEXT,                  -- Optional (max 1000 chars)
  IsCompleted INTEGER NOT NULL DEFAULT 0,
  CompletedAt TEXT,
  DueDate TEXT,
  EstimatedHours INTEGER,            -- NEW: For scheduling (Assignment 3)
  ProjectId TEXT NOT NULL,
  CreatedAt TEXT NOT NULL,
  CreatedBy TEXT NOT NULL,
  UpdatedAt TEXT,
  UpdatedBy TEXT,
  IsDeleted INTEGER NOT NULL DEFAULT 0,
  DeletedAt TEXT,
  FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
);
```

**TaskDependencies** (Assignment 3)
```sql
CREATE TABLE TaskDependencies (
  TaskId TEXT NOT NULL,              -- Task that has the dependency
  DependsOnTaskId TEXT NOT NULL,     -- Task that must be completed first
  CreatedAt TEXT NOT NULL,
  PRIMARY KEY (TaskId, DependsOnTaskId),
  FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE RESTRICT,
  FOREIGN KEY (DependsOnTaskId) REFERENCES Tasks(Id) ON DELETE RESTRICT
);
```

**RefreshTokens**
```sql
CREATE TABLE RefreshTokens (
  Id TEXT PRIMARY KEY,
  Token TEXT NOT NULL UNIQUE,
  UserId TEXT NOT NULL,
  ExpiresAt TEXT NOT NULL,
  CreatedAt TEXT NOT NULL,
  IsRevoked INTEGER NOT NULL DEFAULT 0,
  FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

### Entity Relationships

```
User (1) ──────┬───────→ (Many) Projects
               │
               └───────→ (Many) RefreshTokens

Project (1) ───────────→ (Many) Tasks

Task (Many) ←──────────→ (Many) Task
         (via TaskDependencies join table)
         One task can depend on many tasks,
         One task can be required by many tasks
```

---

## 🔐 Security & Validation

### Password Security
- **Hashing**: BCrypt with cost factor 12 (2^12 iterations)
- **Requirements**: 
  - Minimum 8 characters
  - At least 1 uppercase letter
  - At least 1 lowercase letter
  - At least 1 number
  - At least 1 special character

### JWT Configuration
- **Access Token**: 60 minutes expiry
- **Refresh Token**: 7 days expiry
- **Algorithm**: HS256 (HMAC SHA-256)
- **Issuer/Audience**: Validated on each request

### Input Validation

#### Backend (FluentValidation)
```csharp
// CreateTaskCommandValidator
RuleFor(x => x.Title)
    .NotEmpty().WithMessage("Title is required")
    .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

RuleFor(x => x.Description)
    .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

When(x => x.DueDate.HasValue, () =>
{
    RuleFor(x => x.DueDate!.Value)
        .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
        .WithMessage("Due date cannot be in the past");
});
```

#### Frontend (Zod)
```typescript
const taskSchema = z.object({
  title: z.string()
    .min(1, 'Title is required')
    .max(200, 'Title must be less than 200 characters'),
  description: z.string()
    .max(1000, 'Description must be less than 1000 characters')
    .optional(),
  dueDate: z.string().optional()
});
```

### API Security
- All endpoints except `/api/auth/*` require JWT authentication
- User can only access their own projects/tasks (UserId filtering)
- SQL injection protection via EF Core parameterized queries
- XSS protection via React's automatic escaping

---

## 📁 Project Structure

```
Assignment2/
├── src/
│   ├── API/                          # 🌐 Entry point & HTTP layer
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs     # Login, Register, Refresh
│   │   │   ├── ProjectsController.cs # Projects CRUD
│   │   │   └── TasksController.cs    # Tasks CRUD
│   │   ├── Middleware/
│   │   │   └── ExceptionHandlerMiddleware.cs
│   │   └── Program.cs                # DI, Swagger, Auth config
│   │
│   ├── Application/                  # 🎯 CQRS Use Cases
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   ├── Login/
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   ├── LoginCommandHandler.cs
│   │   │   │   │   └── LoginCommandValidator.cs
│   │   │   │   ├── Register/
│   │   │   │   └── RefreshToken/
│   │   │   └── DTOs/
│   │   │       └── AuthResponseDto.cs
│   │   ├── Projects/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateProject/
│   │   │   │   ├── UpdateProject/
│   │   │   │   └── DeleteProject/
│   │   │   ├── Queries/
│   │   │   │   ├── GetProjects/
│   │   │   │   └── GetProjectById/
│   │   │   └── DTOs/
│   │   │       ├── ProjectDto.cs
│   │   │       └── ProjectDetailDto.cs
│   │   └── Tasks/
│   │       ├── Commands/
│   │       │   ├── CreateTask/
│   │       │   ├── UpdateTask/
│   │       │   ├── DeleteTask/
│   │       │   └── ToggleTaskCompletion/
│   │       ├── Queries/
│   │       │   └── GetTasksByProject/
│   │       └── DTOs/
│   │           └── TaskDto.cs
│   │
│   ├── Domain/                       # 💎 Core Business Logic
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs
│   │   │   ├── Project.cs
│   │   │   ├── TaskItem.cs
│   │   │   └── RefreshToken.cs
│   │   ├── Repositories/
│   │   │   └── IRepository.cs
│   │   └── Exceptions/
│   │       └── NotFoundException.cs
│   │
│   └── Infrastructure/               # 🔧 External Services
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/
│       │   │   ├── UserConfiguration.cs
│       │   │   ├── ProjectConfiguration.cs
│       │   │   └── TaskConfiguration.cs
│       │   └── Migrations/
│       ├── Repositories/
│       │   └── Repository.cs
│       └── Security/
│           ├── IJwtService.cs
│           ├── JwtService.cs
│           ├── IPasswordHasher.cs
│           └── PasswordHasher.cs
│
└── project-manager-frontend/        # ⚛️ React Frontend
    └── src/
        ├── components/               # Reusable UI
        │   ├── Layout.tsx
        │   ├── Navbar.tsx
        │   └── Toast.tsx
        ├── features/                 # Domain features
        │   ├── auth/
        │   │   ├── Login.tsx
        │   │   ├── Register.tsx
        │   │   └── authApi.ts
        │   ├── projects/
        │   │   ├── ProjectsList.tsx
        │   │   ├── CreateProjectModal.tsx
        │   │   ├── EditProjectModal.tsx
        │   │   └── projectsApi.ts
        │   └── tasks/
        │       ├── CreateTaskModal.tsx
        │       ├── EditTaskModal.tsx
        │       └── tasksApi.ts
        ├── lib/                      # Core utilities
        │   ├── api.ts                # Axios client with JWT
        │   ├── authStore.ts          # Zustand auth state
        │   └── queryClient.ts        # TanStack Query config
        ├── pages/
        │   ├── ProjectDetail.tsx     # /projects/:id
        │   └── ProtectedRoute.tsx    # Auth guard
        ├── hooks/
        │   └── useToast.ts
        └── types/
            └── index.ts              # TypeScript interfaces
```

---

## 🎯 Design Decisions

### Why Task has Title AND Description?

**Original Requirement:** "Each task includes: Title (required)"

**Implementation:** Title (required) + Description (optional)

**Rationale:**
1. **Industry Standard**: Jira, Asana, Trello all use Title + Description pattern
2. **Better UX**: Short scannable titles, detailed optional descriptions
3. **Flexibility**: Users can choose detail level per task
4. **Visual Hierarchy**: Title prominent, description secondary

**Validation:**
- Title: Required, 1-200 characters (scannable)
- Description: Optional, max 1000 characters (detailed notes)

### Why Soft Delete Instead of Hard Delete?

**Implementation:** Projects and Tasks have `IsDeleted` flag

**Rationale:**
1. **Data Recovery**: Can restore accidentally deleted items
2. **Audit Trail**: Maintain historical records
3. **Referential Integrity**: Avoid cascade deletion issues
4. **Compliance**: Some regulations require data retention

### Why Separate Auth Store (Zustand) from Server State (React Query)?

**Rationale:**
1. **Different Lifecycles**: Auth persists across sessions, server data is ephemeral
2. **Different Concerns**: Auth is client-side state, projects/tasks are server state
3. **Performance**: Don't refetch auth on every query invalidation
4. **Simplicity**: Each tool does what it's best at

---

## 🧪 Testing

### Backend Tests ✅
**Current Status:** **40 tests passing** (100% success rate)

**Test Coverage:**
- ✅ **Authentication Tests** (8 tests)
  - Login command handler
  - Register command handler  
  - Login validator (email, password rules)
  - Register validator (email, password, name rules)
  
- ✅ **Projects Tests** (10 tests)
  - Create project command handler
  - Update project command handler
  - Delete project command handler
  - Get projects query handler
  - Get project by ID query handler
  - Validators for all commands

- ✅ **Tasks Tests** (14 tests)
  - Create task command handler
  - Update task command handler
  - Delete task command handler
  - Toggle completion command handler
  - Get tasks by project query handler
  - Validators for all commands

- ✅ **Scheduling Algorithm Tests** (8 tests - Assignment 3)
  - Empty task list handling
  - Single task scheduling
  - Linear dependency chain ordering
  - Circular dependency detection
  - Parallel tasks (all critical)
  - Tasks without estimates (smart defaults)
  - Complex diamond dependency graph with CPM validation
  - Overdue task warnings

**Framework:** xUnit + FluentAssertions + Moq

**Run Tests:**
```bash
cd Assignment2
dotnet test
```

**Output:**
```
Passed!  - Failed: 0, Passed: 40, Skipped: 0, Total: 40, Duration: 147 ms
```
---


## 👨‍💻 Author

**Anshdeep Singh**

Built with attention to architecture, user experience, and industry best practices.

---

## 📄 License

Created as a portfolio demonstration project for PathLock.
