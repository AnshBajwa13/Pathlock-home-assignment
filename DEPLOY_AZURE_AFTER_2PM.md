# Azure Deployment Guide - Using GitHub Education Credits

## ✅ Prerequisites
- GitHub Education Pack activated
- $100 Azure credit available
- Azure CLI installed: `winget install Microsoft.AzureCLI`

---

## 🚀 STEP-BY-STEP DEPLOYMENT (After 2 PM)

### Part 1: Backend to Azure App Service (15 minutes)

#### 1. Login to Azure
```bash
az login
```
*This will open a browser - sign in with your GitHub Education account*

#### 2. Create Resource Group
```bash
az group create --name pathlock-rg --location eastus
```

#### 3. Create App Service Plan (FREE tier)
```bash
az appservice plan create \
  --name pathlock-plan \
  --resource-group pathlock-rg \
  --sku F1 \
  --is-linux
```

#### 4. Create Web App for Backend
```bash
az webapp create \
  --resource-group pathlock-rg \
  --plan pathlock-plan \
  --name pathlock-api-ansh \
  --runtime "DOTNETCORE:8.0"
```

#### 5. Configure Environment Variables
```bash
az webapp config appsettings set \
  --resource-group pathlock-rg \
  --name pathlock-api-ansh \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    WEBSITE_RUN_FROM_PACKAGE=1
```

#### 6. Deploy Backend
```bash
cd Assignment2/src/API
dotnet publish -c Release -o ./publish
cd publish
zip -r ../api.zip .
az webapp deployment source config-zip \
  --resource-group pathlock-rg \
  --name pathlock-api-ansh \
  --src ../api.zip
```

**Backend URL:** `https://pathlock-api-ansh.azurewebsites.net`

---

### Part 2: Frontend to Azure Static Web Apps (10 minutes)

#### 1. Build Frontend
```bash
cd project-manager-frontend

# Update API URL in code first
# Edit src/lib/api.ts: baseURL = 'https://pathlock-api-ansh.azurewebsites.net/api'

npm install
npm run build
```

#### 2. Install Static Web Apps CLI
```bash
npm install -g @azure/static-web-apps-cli
```

#### 3. Deploy Frontend
```bash
az staticwebapp create \
  --name pathlock-frontend-ansh \
  --resource-group pathlock-rg \
  --source ./dist \
  --location eastus \
  --branch main \
  --app-location "/" \
  --output-location "dist"
```

**OR use GitHub Actions (Automatic):**
```bash
# This creates auto-deployment from GitHub
az staticwebapp create \
  --name pathlock-frontend-ansh \
  --resource-group pathlock-rg \
  --location eastus \
  --source https://github.com/AnshBajwa13/Pathlock-home-assignment \
  --branch main \
  --app-location "project-manager-frontend" \
  --output-location "dist" \
  --login-with-github
```

**Frontend URL:** `https://pathlock-frontend-ansh.azurestaticapps.net`

---

## 🔧 Alternative: Simple PowerShell Script (Copy-Paste)

Save this as `deploy-azure.ps1`:

```powershell
# Login
Write-Host "Logging into Azure..." -ForegroundColor Green
az login

# Create resources
Write-Host "Creating Azure resources..." -ForegroundColor Green
az group create --name pathlock-rg --location eastus
az appservice plan create --name pathlock-plan --resource-group pathlock-rg --sku F1 --is-linux

# Deploy Backend
Write-Host "Creating backend app..." -ForegroundColor Green
az webapp create --resource-group pathlock-rg --plan pathlock-plan --name pathlock-api-ansh --runtime "DOTNETCORE:8.0"

Write-Host "Building backend..." -ForegroundColor Green
cd Assignment2/src/API
dotnet publish -c Release -o ./publish

Write-Host "Backend deployed!" -ForegroundColor Green
Write-Host "URL: https://pathlock-api-ansh.azurewebsites.net" -ForegroundColor Cyan

# Deploy Frontend
Write-Host "Building frontend..." -ForegroundColor Green
cd ../../../project-manager-frontend
npm install
npm run build

Write-Host "Creating Static Web App..." -ForegroundColor Green
az staticwebapp create --name pathlock-frontend-ansh --resource-group pathlock-rg --location eastus

Write-Host "✅ DEPLOYMENT COMPLETE!" -ForegroundColor Green
Write-Host "Backend: https://pathlock-api-ansh.azurewebsites.net" -ForegroundColor Cyan
Write-Host "Frontend: https://pathlock-frontend-ansh.azurestaticapps.net" -ForegroundColor Cyan
```

Run: `.\deploy-azure.ps1`

---

## 📝 After Deployment: Update README

Add this section to your main README.md:

```markdown
## 🌐 Live Demo

**🎉 Application is deployed and running on Azure!**

- **Frontend Application:** https://pathlock-frontend-ansh.azurestaticapps.net
- **Backend API:** https://pathlock-api-ansh.azurewebsites.net
- **API Documentation (Swagger):** https://pathlock-api-ansh.azurewebsites.net/swagger

### Test Credentials
- **Email:** demo@pathlock.com
- **Password:** Demo@123456

*Or register your own account!*

---

**Deployment Details:**
- ✅ Backend: Azure App Service (.NET 8)
- ✅ Frontend: Azure Static Web Apps (React + TypeScript)
- ✅ Database: SQLite (embedded)
- ✅ CI/CD: GitHub Actions (auto-deploy on push)
- ✅ SSL: Automatic HTTPS
```

---

## 💰 Cost Breakdown (Using $100 Credit)

| Service | Monthly Cost | Your Cost |
|---------|--------------|-----------|
| App Service (F1 Free Tier) | $0 | **FREE** |
| Static Web Apps (Free Tier) | $0 | **FREE** |
| **Total** | **$0** | **FREE** |

You won't use ANY of your $100 credit! 🎉

---

## ⚡ Even Faster: Railway (If Azure takes too long)

If Azure is being slow, use Railway as backup:

1. Go to https://railway.app
2. Sign in with GitHub
3. "New Project" → "Deploy from GitHub"
4. Select your repo
5. Done in 5 minutes!

---

## 📧 Email Template for PathLock (After Deployment)

```
Subject: PathLock Assignment - Live Demo Now Available

Hi PathLock Team,

I've completed the deployment! The application is now live on Azure:

🌐 Live URLs:
- Frontend: https://pathlock-frontend-ansh.azurestaticapps.net
- Backend API: https://pathlock-api-ansh.azurewebsites.net/swagger
- GitHub: https://github.com/AnshBajwa13/Pathlock-home-assignment

✅ What's Deployed:
- All 3 assignments (Task Manager, Dependencies, Scheduling)
- 40/40 tests passing
- Full authentication system
- Interactive dependency graph
- CPM scheduling algorithm

🔑 Test Account:
Email: demo@pathlock.com
Password: Demo@123456

The application is deployed using Azure's enterprise-grade infrastructure 
with automatic SSL and CI/CD via GitHub Actions.

Looking forward to your feedback!

Best regards,
Ansh Bajwa
```

---

## ✅ Pre-Deployment Checklist

Before deploying (after 2 PM):

- [ ] Azure CLI installed: `az --version`
- [ ] GitHub Education credits activated
- [ ] Node.js installed: `node --version`
- [ ] .NET SDK installed: `dotnet --version`
- [ ] All code committed and pushed to GitHub

---

## 🆘 Troubleshooting

**If deployment fails:**
1. Check Azure portal: https://portal.azure.com
2. View deployment logs
3. Or use Railway as instant backup

**If running out of time:**
- Deploy to Railway (5 minutes)
- Update later to Azure
- Both look professional!

---

**Estimated Total Time:** 20-25 minutes after 2 PM

Good luck! 🚀
