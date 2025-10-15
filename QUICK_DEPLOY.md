# Quick Deployment Reference

## 🚀 One-Time Setup (Do Once)

### 1. EC2 Setup
```bash
# SSH into EC2
ssh -i your-key.pem ubuntu@your-ec2-ip

# Install .NET 8
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/bin

# Create app directory
sudo mkdir -p /var/www/dotnetapi
sudo chown -R ubuntu:ubuntu /var/www/dotnetapi

# Copy and install systemd service
sudo cp microjobboard-api.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable microjobboard-api
```

### 2. GitHub Secrets
Add these 6 secrets in GitHub → Settings → Secrets and variables → Actions:

| Secret | Value |
|--------|-------|
| `AWS_EC2_HOST` | Your EC2 IP/DNS |
| `AWS_EC2_USER` | `ubuntu` |
| `EC2_SSH_KEY` | Content of your .pem file |
| `RDS_CONNECTION_STRING` | Your RDS connection string |
| `JWT_SECRET_KEY` | Run: `openssl rand -base64 32` |
| `FRONTEND_URL` | Your frontend URL |

### 3. Security Groups
- **EC2:** Allow port 22 (SSH), 5000 (API)
- **RDS:** Allow port 1433 from EC2 security group

---

## 📦 Deploy (Every Time)

```bash
git add .
git commit -m "Your commit message"
git push origin main
```

Monitor at: GitHub → Actions tab

---

## 🔍 Verify Deployment

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

## 🛠️ Common Commands

```bash
# Restart service
sudo systemctl restart microjobboard-api

# Stop service
sudo systemctl stop microjobboard-api

# Start service
sudo systemctl start microjobboard-api

# View logs (last 100 lines)
sudo journalctl -u microjobboard-api -n 100

# View logs (follow mode)
sudo journalctl -u microjobboard-api -f

# Check service status
sudo systemctl status microjobboard-api
```

---

## ⚠️ Troubleshooting

### Deployment fails
1. Check GitHub Actions logs
2. Verify all 6 secrets are set correctly
3. Ensure EC2 security group allows SSH

### Service won't start
```bash
# Check logs
sudo journalctl -u microjobboard-api -n 50

# Check permissions
ls -la /var/www/dotnetapi/

# Verify .NET runtime
dotnet --list-runtimes
```

### Database connection fails
1. Check RDS security group allows EC2
2. Verify connection string in secrets
3. Test connectivity: `telnet your-rds-endpoint 1433`

---

## 📚 Full Documentation

- [DEPLOYMENT.md](./DEPLOYMENT.md) - Complete deployment guide
- [GITHUB_SECRETS_SETUP.md](./GITHUB_SECRETS_SETUP.md) - Detailed secrets guide
