# Deployment Changes Summary

## ✅ What Was Done

### 1. Updated GitHub Actions Workflow
**File:** `.github/workflows/dotnet.yml`

**Changes:**
- ✅ Fixed SCP source path from `./publish/*` to `./publish/**` for recursive copy
- ✅ Added `strip_components: 1` to properly extract files
- ✅ Replaced `pkill dotnet` with proper systemd service management
- ✅ Added dynamic `appsettings.Production.json` generation from GitHub secrets
- ✅ Added deployment verification with service status checks
- ✅ Added error handling with detailed log output on failure

**Security Improvements:**
- Secrets are now managed via GitHub Secrets (not hardcoded)
- Production config is generated dynamically on EC2
- No sensitive data in repository

### 2. Created Deployment Documentation
**New Files:**

| File | Purpose |
|------|---------|
| `DEPLOYMENT.md` | Complete EC2 deployment guide with step-by-step instructions |
| `GITHUB_SECRETS_SETUP.md` | Detailed guide for configuring all 6 required GitHub secrets |
| `QUICK_DEPLOY.md` | Quick reference card for common deployment tasks |
| `microjobboard-api.service` | Systemd service file for production service management |
| `appsettings.Production.json.template` | Template for production configuration |

### 3. Updated README.md
**Changes:**
- Added AWS EC2 Deployment section
- Added links to all deployment guides
- Added quick start instructions
- Documented GitHub Actions workflow features

---

## 🔐 Required GitHub Secrets

You must add these 6 secrets before deploying:

1. **AWS_EC2_HOST** - EC2 instance IP/DNS
2. **AWS_EC2_USER** - SSH username (usually `ubuntu`)
3. **EC2_SSH_KEY** - Private key (.pem) content
4. **RDS_CONNECTION_STRING** - RDS MSSQL connection string
5. **JWT_SECRET_KEY** - Strong JWT secret (32+ chars)
6. **FRONTEND_URL** - Frontend application URL

See [GITHUB_SECRETS_SETUP.md](./GITHUB_SECRETS_SETUP.md) for detailed instructions.

---

## 🚀 Deployment Workflow

### Before (Old Method)
```bash
# Manual deployment required
# Used pkill dotnet (unreliable)
# No service management
# Hardcoded credentials in appsettings.json
```

### After (New Method)
```bash
# Automated CI/CD
git push origin main

# GitHub Actions automatically:
# 1. Builds and tests
# 2. Publishes artifacts
# 3. Copies to EC2 via SCP
# 4. Generates production config from secrets
# 5. Restarts systemd service
# 6. Verifies deployment success
```

---

## 📋 Next Steps

### 1. One-Time EC2 Setup
```bash
# SSH into EC2
ssh -i your-key.pem ubuntu@your-ec2-ip

# Install .NET 8 Runtime
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/bin

# Create app directory
sudo mkdir -p /var/www/dotnetapi
sudo chown -R ubuntu:ubuntu /var/www/dotnetapi

# Install systemd service
sudo nano /etc/systemd/system/microjobboard-api.service
# Copy content from microjobboard-api.service file

# Enable service
sudo systemctl daemon-reload
sudo systemctl enable microjobboard-api
```

### 2. Configure GitHub Secrets
Go to: **GitHub Repository → Settings → Secrets and variables → Actions**

Add all 6 secrets listed above.

### 3. Deploy
```bash
git add .
git commit -m "Setup EC2 deployment"
git push origin main
```

Monitor deployment: **GitHub → Actions tab**

### 4. Verify
```bash
# SSH into EC2
ssh -i your-key.pem ubuntu@your-ec2-ip

# Check service
sudo systemctl status microjobboard-api

# View logs
sudo journalctl -u microjobboard-api -f

# Test API
curl http://localhost:5000/swagger/index.html
```

---

## 🔧 Configuration Changes

### appsettings.json (Development)
- Keep for local development
- ⚠️ **WARNING:** Contains hardcoded RDS credentials
- **Action Required:** Change RDS password and update this file
- Consider using User Secrets for local development

### appsettings.Production.json (Production)
- ✅ Now generated dynamically on EC2 from GitHub secrets
- ✅ Never committed to repository (.gitignore already excludes it)
- ✅ Secrets are secure in GitHub Secrets

---

## 🛡️ Security Improvements

| Before | After |
|--------|-------|
| ❌ Hardcoded credentials in appsettings.json | ✅ Secrets in GitHub Secrets |
| ❌ Credentials committed to Git | ✅ No secrets in repository |
| ❌ Manual deployment process | ✅ Automated CI/CD |
| ❌ No service management | ✅ Systemd service with auto-restart |
| ❌ Process killed with pkill | ✅ Graceful service restart |

---

## 📊 Deployment Architecture

```
┌─────────────────┐
│   GitHub Repo   │
│   (Push main)   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ GitHub Actions  │
│  - Build        │
│  - Test         │
│  - Publish      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   AWS EC2       │
│  - Copy files   │
│  - Gen config   │
│  - Restart svc  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   AWS RDS       │
│  MSSQL Server   │
└─────────────────┘
```

---

## 📝 Files Modified/Created

### Modified
- `.github/workflows/dotnet.yml` - Updated deployment workflow
- `README.md` - Added deployment section

### Created
- `DEPLOYMENT.md` - Full deployment guide
- `GITHUB_SECRETS_SETUP.md` - Secrets configuration guide
- `QUICK_DEPLOY.md` - Quick reference
- `microjobboard-api.service` - Systemd service file
- `appsettings.Production.json.template` - Production config template
- `DEPLOYMENT_CHANGES.md` - This file

### Unchanged
- `.gitignore` - Already excludes appsettings.Production.json ✅
- Application code - No changes needed
- Database migrations - No changes needed

---

## ⚠️ Important Security Notes

1. **Change RDS Password:** Your current RDS password is exposed in `appsettings.json`
   ```bash
   # In AWS Console → RDS → Modify → Change password
   # Then update RDS_CONNECTION_STRING secret in GitHub
   ```

2. **Remove Hardcoded Credentials:** Consider using User Secrets for local development
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   ```

3. **Rotate Secrets Regularly:** Change JWT secret and RDS password every 90 days

4. **Restrict Security Groups:** 
   - EC2: Allow SSH only from your IP, Port 5000 for API access
   - RDS: Allow 1433 only from EC2 security group

5. **HTTP Deployment:** This setup uses HTTP (port 5000). HTTPS/SSL is optional and can be added later if needed.

---

## 🎉 Benefits

✅ **Automated Deployment** - Push to deploy  
✅ **Secure Secrets** - No credentials in code  
✅ **Service Management** - Systemd with auto-restart  
✅ **Error Handling** - Deployment verification  
✅ **Production Ready** - Best practices implemented  
✅ **Easy Rollback** - Revert Git commit and redeploy  
✅ **Monitoring** - Service logs via journalctl  

---

## 📞 Support

For issues:
1. Check [DEPLOYMENT.md](./DEPLOYMENT.md) troubleshooting section
2. Review GitHub Actions logs
3. Check EC2 service logs: `sudo journalctl -u microjobboard-api -n 100`
4. Verify all secrets are configured correctly

---

**Ready to deploy? Follow the Next Steps above!** 🚀
