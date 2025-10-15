# EC2 Deployment Checklist

Use this checklist to ensure successful deployment to AWS EC2.

---

## ☑️ Pre-Deployment Checklist

### AWS Infrastructure
- [ ] EC2 instance is running (Ubuntu 22.04 LTS recommended)
- [ ] EC2 has public IP address assigned
- [ ] EC2 security group allows:
  - [ ] Port 22 (SSH) from your IP
  - [ ] Port 5000 (API) - from frontend IP or 0.0.0.0/0 for public access
- [ ] RDS MSSQL Server instance is running
- [ ] RDS security group allows:
  - [ ] Port 1433 from EC2 security group
- [ ] You have the EC2 private key (.pem file)

### EC2 Setup
- [ ] .NET 8.0 Runtime installed
- [ ] Application directory created (`/var/www/dotnetapi`)
- [ ] Directory permissions set correctly
- [ ] Systemd service file installed
- [ ] Systemd service enabled

### GitHub Configuration
- [ ] Repository has Actions enabled
- [ ] All 6 secrets configured:
  - [ ] `AWS_EC2_HOST`
  - [ ] `AWS_EC2_USER`
  - [ ] `EC2_SSH_KEY`
  - [ ] `RDS_CONNECTION_STRING`
  - [ ] `JWT_SECRET_KEY`
  - [ ] `FRONTEND_URL`

### Security
- [ ] RDS password changed (if exposed in Git)
- [ ] JWT secret key is strong (32+ characters)
- [ ] SSH key is secure and not shared
- [ ] GitHub repository is private (if needed)

---

## 🚀 Deployment Steps

### 1. EC2 Initial Setup
```bash
# Connect to EC2
ssh -i your-key.pem ubuntu@your-ec2-ip

# Install .NET 8
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/bin

# Verify installation
dotnet --version
```
- [ ] .NET 8 installed successfully
- [ ] Version shows 8.0.x

### 2. Create Application Directory
```bash
sudo mkdir -p /var/www/dotnetapi
sudo chown -R ubuntu:ubuntu /var/www/dotnetapi
```
- [ ] Directory created
- [ ] Permissions set

### 3. Install Systemd Service
```bash
sudo nano /etc/systemd/system/microjobboard-api.service
```
Copy content from `microjobboard-api.service` file

```bash
sudo systemctl daemon-reload
sudo systemctl enable microjobboard-api
```
- [ ] Service file created
- [ ] Service enabled

### 4. Configure GitHub Secrets
Go to: GitHub → Settings → Secrets and variables → Actions

- [ ] `AWS_EC2_HOST` = `your-ec2-ip-or-dns`
- [ ] `AWS_EC2_USER` = `ubuntu`
- [ ] `EC2_SSH_KEY` = `<content of .pem file>`
- [ ] `RDS_CONNECTION_STRING` = `Server=...;Database=...`
- [ ] `JWT_SECRET_KEY` = `<32+ character random string>`
- [ ] `FRONTEND_URL` = `https://yourdomain.com`

### 5. Test SSH Connection
```bash
ssh -i your-key.pem ubuntu@your-ec2-ip
```
- [ ] SSH connection successful
- [ ] No permission errors

### 6. Deploy
```bash
git add .
git commit -m "Setup EC2 deployment"
git push origin main
```
- [ ] Code pushed to GitHub
- [ ] GitHub Actions workflow triggered

### 7. Monitor Deployment
- [ ] Go to GitHub → Actions tab
- [ ] Watch workflow execution
- [ ] Build step completes successfully
- [ ] Deploy step completes successfully

---

## ✅ Post-Deployment Verification

### 1. Check Service Status
```bash
ssh -i your-key.pem ubuntu@your-ec2-ip
sudo systemctl status microjobboard-api
```
- [ ] Service is active (running)
- [ ] No error messages

### 2. View Logs
```bash
sudo journalctl -u microjobboard-api -n 50
```
- [ ] Application started successfully
- [ ] Database connection successful
- [ ] No critical errors

### 3. Test API Locally (on EC2)
```bash
curl http://localhost:5000/swagger/index.html
```
- [ ] Returns HTML content
- [ ] No connection errors

### 4. Test API Remotely
Open in browser: `http://your-ec2-ip:5000/swagger`
- [ ] Swagger UI loads
- [ ] API endpoints are visible

### 5. Test Authentication
1. Register a new user via Swagger
2. Login to get JWT token
3. Test protected endpoint with token
- [ ] Registration works
- [ ] Login returns token
- [ ] Protected endpoints accessible with token

### 6. Test Database Connection
1. Create a job via API
2. Retrieve the job
3. Update the job
- [ ] Data persists in RDS
- [ ] CRUD operations work

---

## 🔍 Troubleshooting Checklist

### If Deployment Fails

#### GitHub Actions Error
- [ ] Check Actions logs for specific error
- [ ] Verify all secrets are set
- [ ] Check secret values are correct (no extra spaces)
- [ ] Ensure EC2 security group allows SSH

#### SSH Connection Error
- [ ] Verify `EC2_SSH_KEY` includes BEGIN/END markers
- [ ] Check `AWS_EC2_USER` is correct for your AMI
- [ ] Confirm EC2 security group allows port 22
- [ ] Test SSH manually: `ssh -i key.pem ubuntu@ec2-ip`

#### Service Won't Start
```bash
# Check logs
sudo journalctl -u microjobboard-api -n 100

# Check files
ls -la /var/www/dotnetapi/

# Check .NET runtime
dotnet --list-runtimes
```
- [ ] All files copied to EC2
- [ ] .NET runtime is installed
- [ ] Permissions are correct

#### Database Connection Error
```bash
# Test RDS connectivity
telnet your-rds-endpoint 1433
```
- [ ] RDS is accessible from EC2
- [ ] Connection string is correct
- [ ] RDS security group allows EC2

---

## 🔄 Redeployment Checklist

For subsequent deployments:

- [ ] Make code changes
- [ ] Test locally
- [ ] Commit changes
- [ ] Push to main branch
- [ ] Monitor GitHub Actions
- [ ] Verify service restarted
- [ ] Test API functionality

---

## 📊 Health Check Commands

Run these regularly to monitor your deployment:

```bash
# Service status
sudo systemctl status microjobboard-api

# Recent logs
sudo journalctl -u microjobboard-api -n 50

# Follow logs in real-time
sudo journalctl -u microjobboard-api -f

# Check disk space
df -h

# Check memory
free -h

# Check process
ps aux | grep dotnet

# Test API
curl http://localhost:5000/swagger/index.html
```

---

## 🎯 Success Criteria

Your deployment is successful when:

- [x] GitHub Actions workflow completes without errors
- [x] Systemd service is active and running
- [x] API responds at `http://ec2-ip:5000/swagger`
- [x] Authentication endpoints work
- [x] Database operations succeed
- [x] Logs show no critical errors
- [x] Service auto-restarts after reboot

---

## 📝 Notes

**Date Deployed:** _________________

**EC2 Instance ID:** _________________

**EC2 Public IP:** _________________

**RDS Endpoint:** _________________

**Issues Encountered:** 
_________________________________________________
_________________________________________________
_________________________________________________

**Resolution:**
_________________________________________________
_________________________________________________
_________________________________________________

---

## 📚 Reference Documents

- [DEPLOYMENT.md](./DEPLOYMENT.md) - Complete deployment guide
- [GITHUB_SECRETS_SETUP.md](./GITHUB_SECRETS_SETUP.md) - Secrets configuration
- [QUICK_DEPLOY.md](./QUICK_DEPLOY.md) - Quick reference
- [DEPLOYMENT_CHANGES.md](./DEPLOYMENT_CHANGES.md) - What changed

---

**Ready to deploy? Start with the Pre-Deployment Checklist!** ✅
