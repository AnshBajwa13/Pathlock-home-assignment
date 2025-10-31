# GitHub Push & Deployment Guide

## 📤 Step 1: Create GitHub Repository

### Option A: Via GitHub Website (Recommended - Easiest)
1. Go to https://github.com/new
2. **Repository name:** `pathlock-home-assignment`
3. **Description:** `Full-stack project management system for PathLock - Clean Architecture, CQRS, React, TypeScript`
4. **Visibility:** ✅ **Public** (so PathLock can access it)
5. **DO NOT** initialize with README (we already have one)
6. Click **"Create repository"**

### Option B: Via GitHub CLI
```bash
gh repo create pathlock-home-assignment --public --source=. --remote=origin
```

---

## 📤 Step 2: Push to GitHub

After creating the repository on GitHub, copy the commands they show you. It will look like this:

```bash
# Set the remote repository
git remote add origin https://github.com/YOUR_USERNAME/pathlock-home-assignment.git

# OR if you use SSH:
git remote add origin git@github.com:YOUR_USERNAME/pathlock-home-assignment.git

# Push your code
git branch -M main
git push -u origin main
```

**Replace `YOUR_USERNAME` with your actual GitHub username!**

---

## ✅ Verification

After pushing, verify on GitHub:
1. Go to `https://github.com/YOUR_USERNAME/pathlock-home-assignment`
2. You should see:
   - ✅ 192 files
   - ✅ README.md displayed
   - ✅ All folders (Assignment1, Assignment2, project-manager-frontend)
   - ✅ All commits

---

## 🚀 Step 3: Deployment Options

### Option 1: Railway.app (FASTEST - 10 minutes) ⚡ **RECOMMENDED**

#### Backend Deployment
1. Go to https://railway.app
2. Sign in with GitHub
3. Click "New Project" → "Deploy from GitHub repo"
4. Select `pathlock-home-assignment`
5. Railway will detect .NET project
6. **Settings to configure:**
   - **Root Directory:** `Assignment2/src/API`
   - **Build Command:** `dotnet publish -c Release -o /app`
   - **Start Command:** `dotnet API.dll`
   - **Environment Variables:**
     ```
     ASPNETCORE_ENVIRONMENT=Production
     ASPNETCORE_URLS=http://0.0.0.0:$PORT
     ```
7. Click "Deploy"
8. Copy the generated URL (e.g., `https://pathlock-api.up.railway.app`)

#### Frontend Deployment
1. In Railway, click "New" → "Deploy from GitHub repo"
2. Select `pathlock-home-assignment` again
3. **Settings:**
   - **Root Directory:** `project-manager-frontend`
   - **Build Command:** `npm install && npm run build`
   - **Start Command:** `npm run preview`
   - **Environment Variables:**
     ```
     VITE_API_BASE_URL=https://pathlock-api.up.railway.app/api
     ```
4. Click "Deploy"
5. You'll get a URL like `https://pathlock-frontend.up.railway.app`

**Total Time:** ~10 minutes  
**Cost:** FREE (with Railway's free tier)

---

### Option 2: Azure (30 minutes) - Enterprise Grade

#### Prerequisites
```bash
# Install Azure CLI
winget install Microsoft.AzureCLI

# Login
az login
```

#### Backend to Azure App Service
```bash
# Create resource group
az group create --name pathlock-rg --location eastus

# Create App Service plan
az appservice plan create --name pathlock-plan --resource-group pathlock-rg --sku B1 --is-linux

# Create web app
az webapp create --resource-group pathlock-rg --plan pathlock-plan --name pathlock-api-2025 --runtime "DOTNETCORE:8.0"

# Configure app settings
az webapp config appsettings set --resource-group pathlock-rg --name pathlock-api-2025 --settings ASPNETCORE_ENVIRONMENT=Production

# Deploy from local (from Assignment2 folder)
cd Assignment2
az webapp up --name pathlock-api-2025 --resource-group pathlock-rg
```

#### Frontend to Azure Static Web Apps
```bash
# Build frontend
cd project-manager-frontend
npm install
npm run build

# Create static web app
az staticwebapp create --name pathlock-frontend --resource-group pathlock-rg --location eastus --source ./dist

# Or use Azure Static Web Apps CLI
npm install -g @azure/static-web-apps-cli
swa deploy --app-location ./dist --api-location ""
```

**Total Time:** ~30 minutes  
**Cost:** ~$10-15/month (B1 tier)

---

### Option 3: Docker + DigitalOcean (20 minutes)

#### Create Dockerfiles

**Backend Dockerfile** (`Assignment2/Dockerfile`):
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/API/API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
```

**Frontend Dockerfile** (`project-manager-frontend/Dockerfile`):
```dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

#### Deploy to DigitalOcean
1. Create account at https://digitalocean.com
2. Create droplet (Ubuntu 22.04, $6/month)
3. Install Docker
4. Push images and run

**Total Time:** ~20 minutes  
**Cost:** ~$6/month

---

## 📊 Deployment Comparison

| Platform | Time | Cost/Month | Ease | Best For |
|----------|------|------------|------|----------|
| **Railway** | 10 min | FREE | ⭐⭐⭐⭐⭐ | Quick demos, assignments |
| **Azure** | 30 min | $10-15 | ⭐⭐⭐⭐ | Enterprise, production |
| **DigitalOcean** | 20 min | $6 | ⭐⭐⭐ | Cost-effective hosting |
| **Heroku** | 15 min | $7 | ⭐⭐⭐⭐ | Simple apps |

---

## 🎯 Recommendation for PathLock Submission

**Use Railway.app:**
1. ✅ Fastest deployment (10 minutes)
2. ✅ FREE tier available
3. ✅ Auto-deploys on git push
4. ✅ Provides HTTPS URLs
5. ✅ No credit card required
6. ✅ Perfect for code reviews

**Include in your README:**
```markdown
## 🌐 Live Demo

- **Backend API:** https://pathlock-api.up.railway.app
- **Frontend App:** https://pathlock-frontend.up.railway.app
- **API Documentation:** https://pathlock-api.up.railway.app/swagger

**Test Credentials:**
- Email: demo@pathlock.com
- Password: Demo@123456
```

---

## 🔧 Post-Deployment Checklist

After deployment:
- [ ] Backend health check works: `GET /health`
- [ ] Swagger UI accessible: `GET /swagger`
- [ ] Frontend loads and connects to backend
- [ ] Login/Register works
- [ ] Can create projects and tasks
- [ ] Dependency graph renders
- [ ] Schedule generation works
- [ ] Update GitHub README with live URLs

---

## 📝 What to Tell PathLock

In your submission email:

```
Subject: PathLock Home Assignment - ANshdeep Singh

Hi PathLock Team,

I've completed the home assignment and pushed the code to GitHub:

📦 Repository: https://github.com/YOUR_USERNAME/pathlock-home-assignment

🌐 Live Demo:
- Frontend: https://pathlock-frontend.up.railway.app
- Backend API: https://pathlock-api.up.railway.app/swagger

✅ All 3 Assignments Complete:
- Assignment 1: Task Manager (Service Layer)
- Assignment 2: Project Dependencies (Clean Architecture + CQRS)  
- Assignment 3: CPM Scheduling (40/40 tests passing)

🔑 Test Account:
- Email: demo@pathlock.com
- Password: Demo@123456

📚 Documentation:
- Main README: Comprehensive overview
- Assignment1/README: Architecture & API docs
- Assignment2/README: Clean Architecture deep-dive

Looking forward to discussing the implementation!

Best regards,
ANshdeep Singh
```

---

## 🚀 Ready to Deploy!

**Next Steps:**
1. Create GitHub repository
2. Push code (commands above)
3. Deploy to Railway (10 minutes)
4. Update README with live URLs
5. Email PathLock with links

Good luck! 🎉
