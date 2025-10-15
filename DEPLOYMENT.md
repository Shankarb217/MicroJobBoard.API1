# EC2 Deployment Guide - MicroJobBoard API

This guide covers deploying the MicroJobBoard .NET API to AWS EC2 with RDS MSSQL Server.

## Prerequisites

- AWS EC2 instance (Ubuntu 22.04 LTS recommended)
- AWS RDS MSSQL Server instance
- .NET 8.0 Runtime installed on EC2
- GitHub repository with Actions enabled

---

## 1. EC2 Instance Setup

### Install .NET 8.0 Runtime

```bash
# SSH into your EC2 instance
ssh -i your-key.pem ubuntu@your-ec2-ip

# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 8.0 Runtime
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/bin

# Verify installation
dotnet --version
```

### Create Application Directory

```bash
sudo mkdir -p /var/www/dotnetapi
sudo chown -R ubuntu:ubuntu /var/www/dotnetapi
```

### Configure Security Group

Ensure your EC2 security group allows:
- **Port 22** (SSH) - from your IP
- **Port 5000** (API) - from your application/frontend (or 0.0.0.0/0 for public access)

---

## 2. Create Systemd Service

Create the service file:

```bash
sudo nano /etc/systemd/system/microjobboard-api.service
```

Add the following content:

```ini
[Unit]
Description=MicroJobBoard .NET API
After=network.target

[Service]
Type=notify
User=ubuntu
Group=ubuntu
WorkingDirectory=/var/www/dotnetapi
ExecStart=/usr/bin/dotnet /var/www/dotnetapi/MicroJobBoard.API.dll

# Environment
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

# Restart policy
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=microjobboard-api

# Security
NoNewPrivileges=true
PrivateTmp=true

# Limits
LimitNOFILE=65536

[Install]
WantedBy=multi-user.target
```

Enable and start the service:

```bash
# Reload systemd
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable microjobboard-api

# Start the service (after first deployment)
sudo systemctl start microjobboard-api

# Check status
sudo systemctl status microjobboard-api

# View logs
sudo journalctl -u microjobboard-api -f
```

---

## 3. Configure GitHub Secrets

Go to your GitHub repository → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**

Add the following secrets:

### Required Secrets

| Secret Name | Description | Example |
|-------------|-------------|----------|
| `AWS_EC2_HOST` | EC2 instance public IP or DNS | `ec2-13-233-xxx-xxx.ap-south-1.compute.amazonaws.com` |
| `AWS_EC2_USER` | SSH username | `ubuntu` (for Ubuntu) or `ec2-user` (for Amazon Linux) |
| `EC2_SSH_KEY` | Private key (.pem) content | Copy entire content of your `.pem` file |
| `RDS_CONNECTION_STRING` | RDS MSSQL connection string | `Server=your-rds.amazonaws.com,1433;Database=MicroJobBoardDb;User Id=admin;Password=YourPassword;TrustServerCertificate=True;` |
| `JWT_SECRET_KEY` | JWT secret key (min 32 chars) | Generate using: `openssl rand -base64 32` |
| `FRONTEND_URL` | Frontend application URL | `https://yourdomain.com` or `http://your-frontend-ip:3000` |

### How to Add EC2_SSH_KEY

```bash
# On your local machine, copy the private key content
cat your-key.pem

# Copy the entire output including:
# -----BEGIN RSA PRIVATE KEY-----
# ... key content ...
# -----END RSA PRIVATE KEY-----
```

Paste this entire content into the `EC2_SSH_KEY` secret.

---

## 4. RDS MSSQL Configuration

### Security Group Settings

Ensure RDS security group allows:
- **Port 1433** (MSSQL) - from EC2 security group

### Connection String Format

```
Server=<RDS_ENDPOINT>,1433;Database=MicroJobBoardDb;User Id=<USERNAME>;Password=<PASSWORD>;TrustServerCertificate=True;Encrypt=True;
```

**Important:** Never commit connection strings with credentials to your repository!

---

## 5. Deployment Workflow

The GitHub Actions workflow (`.github/workflows/dotnet.yml`) will:

1. **Build** - Compile and test the application
2. **Publish** - Create deployment artifacts
3. **Deploy** - Copy files to EC2 and restart service

### Trigger Deployment

```bash
# Push to main branch
git add .
git commit -m "Deploy to EC2"
git push origin main
```

### Monitor Deployment

- Go to **Actions** tab in GitHub
- Click on the running workflow
- Monitor build and deploy steps

---

## 6. Post-Deployment Verification

### Check Service Status

```bash
ssh -i your-key.pem ubuntu@your-ec2-ip

# Check if service is running
sudo systemctl status microjobboard-api

# View recent logs
sudo journalctl -u microjobboard-api -n 100

# Test API endpoint
curl http://localhost:5000/swagger/index.html
```

### Test API from Browser

```
http://your-ec2-ip:5000/swagger
```

---

## 7. Troubleshooting

### Service Won't Start

```bash
# Check detailed logs
sudo journalctl -u microjobboard-api -n 100 --no-pager

# Check file permissions
ls -la /var/www/dotnetapi/

# Verify .NET runtime
dotnet --list-runtimes
```

### Database Connection Issues

```bash
# Test RDS connectivity from EC2
telnet your-rds-endpoint.amazonaws.com 1433

# Check security groups
# - RDS security group must allow EC2 security group on port 1433
# - EC2 must have outbound rules for port 1433
```

### GitHub Actions Deployment Fails

- Verify all secrets are correctly set
- Check EC2 SSH key has correct permissions
- Ensure EC2 security group allows SSH from GitHub Actions IPs
- Review workflow logs in GitHub Actions tab

---

## 9. Security Best Practices

✅ **DO:**
- Use GitHub Secrets for sensitive data
- Use systemd for service management
- Restrict security group rules to specific IPs when possible
- Regularly update EC2 instance and packages
- Use strong JWT secret keys (32+ characters)
- Enable RDS encryption at rest
- Consider adding HTTPS/SSL in production (optional)

❌ **DON'T:**
- Hardcode credentials in appsettings.json
- Commit secrets to repository
- Use weak passwords
- Leave default ports open to 0.0.0.0/0
- Run application as root user

---

## 10. Useful Commands

```bash
# Restart service
sudo systemctl restart microjobboard-api

# Stop service
sudo systemctl stop microjobboard-api

# View logs (follow mode)
sudo journalctl -u microjobboard-api -f

# Check disk space
df -h

# Check memory usage
free -h

# Monitor process
top -p $(pgrep -f MicroJobBoard.API)
```

---

## Support

For issues or questions, check:
- Application logs: `sudo journalctl -u microjobboard-api`
- GitHub Actions logs
- RDS CloudWatch logs
- EC2 system logs: `/var/log/syslog`
