# Vercel Deployment Fix Guide

## 🔴 Problem
Frontend shows **404 NOT_FOUND** when trying to login because it can't reach the backend API.

## ✅ Solution
Configure the API URL in Vercel's environment variables.

---

## 📝 Steps to Fix

### 1. Go to Vercel Dashboard
Visit: https://vercel.com/dashboard

### 2. Select Your Project
Click on `pathlock-project-manager`

### 3. Go to Settings
Click **Settings** tab → **Environment Variables**

### 4. Add Environment Variable
**Variable Name:** `VITE_API_URL`  
**Value:** `https://pathlock-home-assignment-production-344b.up.railway.app/api`  
**Environments:** Check all (Production, Preview, Development)

Click **Save**

### 5. Redeploy
Go to **Deployments** tab → Click on latest deployment → **Redeploy**

---

## 🧪 Test After Deployment

1. Visit: https://pathlock-project-manager.vercel.app/login
2. Open browser console (F12)
3. Look for: `🔗 API Base URL: https://pathlock-home-assignment-production-344b.up.railway.app/api`
4. Try logging in with wrong password
5. Should show: **"Invalid email or password. Please try again."**
6. Should NOT show: **404 NOT_FOUND**

---

## 🔍 Error Messages Explained

| Error | Meaning | Solution |
|-------|---------|----------|
| **404 NOT_FOUND** | Frontend can't find backend endpoint | Set `VITE_API_URL` in Vercel |
| **401 Unauthorized** | Wrong credentials (expected!) | Enter correct email/password |
| **Network Error** | No internet or CORS issue | Check connection/CORS |
| **Invalid email or password** | Login failed (expected!) | Use correct credentials |

---

## ✨ What We Fixed

### Backend (Already Done ✅)
- CORS configured for all Vercel deployments
- Swagger UI at root URL
- Flexible origin handling

### Frontend (Just Fixed ✅)
- Better error messages
- 404 detection
- Environment variable logging
- Proper error handling for login/register

---

## 🚀 After Fix
Your app will show user-friendly error messages:
- ✅ "Invalid email or password. Please try again."
- ✅ "Network error. Please check your internet connection."
- ✅ "Unable to connect to server."

Instead of technical errors like:
- ❌ "404: NOT_FOUND"
- ❌ "bom1::2w6bs-1761916634548-8220edc3d6d0"
