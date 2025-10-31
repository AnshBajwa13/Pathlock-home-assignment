# 🎯 QUICK DEPLOYMENT OVERVIEW

## What You're Deploying (2 Separate Apps)

```
┌─────────────────────────────────────────────────────────────┐
│                    Assignment 1                             │
│              (Simple Task Manager)                          │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Frontend (Vercel)              Backend (Render)            │
│  ┌─────────────┐               ┌──────────────┐            │
│  │   React     │──── API ────▶ │  .NET 8 API  │            │
│  │ TypeScript  │   Calls       │  In-Memory   │            │
│  └─────────────┘               └──────────────┘            │
│                                                             │
│  assignment1-task-manager      assignment1-task-api        │
│  .vercel.app                   .onrender.com               │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                 Assignment 2 + 3                            │
│         (Project Manager with CQRS)                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Frontend (Vercel)              Backend (Render)            │
│  ┌─────────────┐               ┌──────────────┐            │
│  │   React     │──── API ────▶ │  .NET 8 API  │            │
│  │ TypeScript  │   Calls       │  + SQLite    │            │
│  │ TanStack Q. │               │  (Disk)      │            │
│  └─────────────┘               └──────────────┘            │
│                                                             │
│  pathlock-project-manager      pathlock-project-api        │
│  .vercel.app                   .onrender.com               │
└─────────────────────────────────────────────────────────────┘
```

---

## 🚀 Deployment Order

**TOTAL: 4 deployments (2 backends + 2 frontends)**

### Do This Order:

1. **Assignment 2+3 Backend** → Render (10 min)
   - Get URL: `https://pathlock-project-api.onrender.com`
   
2. **Assignment 2+3 Frontend** → Vercel (5 min)
   - Use backend URL from step 1
   
3. **Assignment 1 Backend** → Render (5 min)
   - Get URL: `https://assignment1-task-api.onrender.com`
   
4. **Assignment 1 Frontend** → Vercel (5 min)
   - Use backend URL from step 3

---

## ⚙️ Settings Summary

### Assignment 1 Backend (Render)
```
Root Directory: Assignment1/TaskManagerAPI
Build: dotnet restore && dotnet publish -c Release -o out
Start: dotnet out/TaskManagerAPI.dll
Disk: NONE (in-memory)
```

### Assignment 1 Frontend (Vercel)
```
Root Directory: Assignment1/task-manager-frontend
Framework: Vite
Env: VITE_API_URL=https://assignment1-task-api.onrender.com
```

### Assignment 2+3 Backend (Render)
```
Root Directory: Assignment2/src/API
Build: dotnet restore && dotnet publish -c Release -o out
Start: dotnet out/API.dll
Disk: YES (/var/data, 1GB for SQLite)
```

### Assignment 2+3 Frontend (Vercel)
```
Root Directory: project-manager-frontend
Framework: Vite
Env: VITE_API_URL=https://pathlock-project-api.onrender.com
```

---

## 🎯 Quick Links

**Full Guide:** `DEPLOY_RENDER_VERCEL.md`

**Start Here:**
1. https://render.com/ (Sign in with GitHub)
2. https://vercel.com/ (Sign in with GitHub)

---

**Ready? Follow DEPLOY_RENDER_VERCEL.md step-by-step!** 🚀
