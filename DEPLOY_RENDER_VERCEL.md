# 🚀 COMPLETE DEPLOYMENT GUIDE - Render + Vercel

**Total Time:** ~20 minutes  
**Cost:** FREE forever  
**Result:** 2 live apps with professional URLs

---

## 📦 What We're Deploying

### **Assignment 1** (Simple Task Manager)
- **Backend:** Assignment1/TaskManagerAPI → Render
- **Frontend:** Assignment1/task-manager-frontend → Vercel
- **Database:** In-memory (no setup needed)

### **Assignment 2+3** (Full Project Manager)
- **Backend:** Assignment2/src/API → Render
- **Frontend:** project-manager-frontend → Vercel
- **Database:** SQLite (auto-created on Render)

---

## 🎯 PART 1: Deploy Assignment 2+3 Backend (Render)

### Step 1: Go to Render
1. Open: https://render.com/
2. Click **"Get Started"**
3. Sign up with GitHub
4. Authorize Render to access your repos

### Step 2: Create Web Service
1. Click **"New +"** → **"Web Service"**
2. Connect your GitHub repo: `AnshBajwa13/Pathlock-home-assignment`
3. Click **"Connect"** next to your repo

### Step 3: Configure Backend (Assignment 2+3)
Fill in these settings:

| Field | Value |
|-------|-------|
| **Name** | `pathlock-project-api` |
| **Region** | Oregon (US West) |
| **Branch** | `main` |
| **Root Directory** | `Assignment2/src/API` |
| **Runtime** | `.NET` |
| **Build Command** | `dotnet restore && dotnet publish -c Release -o out` |
| **Start Command** | `dotnet out/API.dll` |
| **Instance Type** | **Free** |

### Step 4: Add Environment Variables
Click **"Advanced"** → **"Add Environment Variable"**

Add these 3 variables:

```
ASPNETCORE_ENVIRONMENT = Production
ASPNETCORE_URLS = http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection = Data Source=/var/data/miniprojectmanager.db
```

### Step 5: Add Persistent Disk (for SQLite)
1. Scroll to **"Disk"** section
2. Click **"Add Disk"**
3. Configure:
   - **Name:** `sqlite-data`
   - **Mount Path:** `/var/data`
   - **Size:** 1 GB
4. Click **"Save"**

### Step 6: Deploy!
1. Click **"Create Web Service"**
2. Wait 5-10 minutes (Render will build your .NET app)
3. Once deployed, you'll get a URL like: `https://pathlock-project-api.onrender.com`

### Step 7: Test the API
Open in browser: `https://pathlock-project-api.onrender.com/swagger`

You should see the Swagger API documentation! ✅

---

## 🎯 PART 2: Deploy Assignment 2+3 Frontend (Vercel)

### Step 1: Go to Vercel
1. Open: https://vercel.com/
2. Click **"Start Deploying"**
3. Sign up with GitHub
4. Authorize Vercel

### Step 2: Import Project
1. Click **"Add New..."** → **"Project"**
2. Import: `AnshBajwa13/Pathlock-home-assignment`
3. Click **"Import"**

### Step 3: Configure Frontend
Fill in these settings:

| Field | Value |
|-------|-------|
| **Project Name** | `pathlock-project-manager` |
| **Framework Preset** | Vite |
| **Root Directory** | `project-manager-frontend` |
| **Build Command** | `npm run build` |
| **Output Directory** | `dist` |

### Step 4: Add Environment Variable
Click **"Environment Variables"**

Add this variable:

```
VITE_API_URL = https://pathlock-project-api.onrender.com
```

⚠️ **IMPORTANT:** Use the URL from Part 1, Step 6!

### Step 5: Deploy!
1. Click **"Deploy"**
2. Wait 2-3 minutes
3. You'll get a URL like: `https://pathlock-project-manager.vercel.app`

### Step 6: Test the App
1. Open the Vercel URL
2. Try to register a new user
3. Login and create a project

---

## 🎯 PART 3: Deploy Assignment 1 Backend (Render)

### Repeat Part 1, but with these settings:

| Field | Value |
|-------|-------|
| **Name** | `assignment1-task-api` |
| **Root Directory** | `Assignment1/TaskManagerAPI` |
| **Build Command** | `dotnet restore && dotnet publish -c Release -o out` |
| **Start Command** | `dotnet out/TaskManagerAPI.dll` |

**Environment Variables:**
```
ASPNETCORE_ENVIRONMENT = Production
ASPNETCORE_URLS = http://0.0.0.0:$PORT
```

**NO DISK NEEDED** (in-memory storage)

---

## 🎯 PART 4: Deploy Assignment 1 Frontend (Vercel)

### Repeat Part 2, but with these settings:

| Field | Value |
|-------|-------|
| **Project Name** | `assignment1-task-manager` |
| **Root Directory** | `Assignment1/task-manager-frontend` |

**Environment Variable:**
```
VITE_API_URL = https://assignment1-task-api.onrender.com
```

⚠️ Use the URL from Part 3!

---

## ✅ FINAL CHECKLIST

After all 4 deployments, you should have:

- [ ] Assignment 1 Backend: `https://assignment1-task-api.onrender.com/swagger`
- [ ] Assignment 1 Frontend: `https://assignment1-task-manager.vercel.app`
- [ ] Assignment 2+3 Backend: `https://pathlock-project-api.onrender.com/swagger`
- [ ] Assignment 2+3 Frontend: `https://pathlock-project-manager.vercel.app`

---

## 🎯 UPDATE YOUR README

Add this section to your main README.md:

```markdown
## 🌐 Live Deployments

### Assignment 1: Task Manager
- **Frontend:** https://assignment1-task-manager.vercel.app
- **API Docs:** https://assignment1-task-api.onrender.com/swagger

### Assignment 2+3: Project Manager (CQRS + Scheduling)
- **Frontend:** https://pathlock-project-manager.vercel.app
- **API Docs:** https://pathlock-project-api.onrender.com/swagger

### Test Credentials
- **Email:** test@example.com
- **Password:** Test123!@#
```

---

## 🚨 TROUBLESHOOTING

### Backend Won't Start?
**Check Render logs:**
1. Go to Render dashboard
2. Click your service
3. Click "Logs" tab
4. Look for errors

**Common fixes:**
- Make sure `Root Directory` is correct
- Check environment variables spelling
- Wait 10 minutes for first deploy

### Frontend API Connection Failed?
**Check:**
1. `VITE_API_URL` matches your Render backend URL
2. Backend is running (check Swagger)
3. No typos in environment variable name

### Database Not Working (Assignment 2+3)?
**Check:**
1. Disk is attached in Render
2. Mount path is `/var/data`
3. Connection string uses `/var/data/miniprojectmanager.db`

---

## 📝 COMMIT DEPLOYMENT FILES

Before deploying, commit the render.yaml files:

```bash
cd c:\Users\Anshd\OneDrive\Desktop\pathlock_home_assignment

git add Assignment1/TaskManagerAPI/render.yaml
git add Assignment2/src/API/render.yaml
git commit -m "Add Render deployment configuration"
git push origin main
```

---

## 🎉 YOU'RE DONE!

After deployment:
1. ✅ Update README with live URLs
2. ✅ Test both apps
3. ✅ Submit form with GitHub link
4. ✅ Email PathLock with live demo links

**Estimated Total Time:** 20-25 minutes

---

**Need help during deployment? I'm here!** 🚀
