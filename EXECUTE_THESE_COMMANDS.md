# Execute These Commands - Step by Step

Follow these steps in order. Copy and paste each command.

---

## STEP 1: Connect to Your EC2 Instance

```bash
# Replace with your actual key file and EC2 IP
ssh -i your-key.pem ubuntu@your-ec2-ip
```

**Example:**
```bash
ssh -i ~/Downloads/my-ec2-key.pem ubuntu@13.233.123.456
```

---

## STEP 2: Install .NET 8 Runtime on EC2

Copy and paste these commands one by one:

```bash
# Download .NET install script
wget https://dot.net/v1/dotnet-install.sh

# Make it executable
chmod +x dotnet-install.sh

# Install .NET 8 runtime
sudo ./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/bin

# Verify installation
dotnet --version
```

**Expected output:** Should show version 8.0.x

---

## STEP 3: Create Application Directory

```bash
# Create directory
sudo mkdir -p /var/www/dotnetapi

# Set ownership to ubuntu user
sudo chown -R ubuntu:ubuntu /var/www/dotnetapi

# Verify
ls -la /var/www/
```

---

## STEP 4: Create Systemd Service File

```bash
# Create the service file
sudo nano /etc/systemd/system/microjobboard-api.service
```

**Paste this content into nano:**

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

**Save and exit nano:**
- Press `Ctrl + O` (save)
- Press `Enter` (confirm)
- Press `Ctrl + X` (exit)

---

## STEP 5: Enable the Service

```bash
# Reload systemd
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable microjobboard-api

# Check if service is enabled
sudo systemctl is-enabled microjobboard-api
```

**Expected output:** `enabled`

---

## STEP 6: Exit EC2 (Return to Your Local Machine)

```bash
exit
```

---

## STEP 7: Configure GitHub Secrets

Go to your browser:

1. Open: `https://github.com/YOUR_USERNAME/YOUR_REPO/settings/secrets/actions`
2. Click **"New repository secret"**
3. Add each secret below:

### Secret 1: AWS_EC2_HOST
- **Name:** `AWS_EC2_HOST`
- **Value:** Your EC2 public IP or DNS
- **Example:** `13.233.123.456` or `ec2-13-233-123-456.ap-south-1.compute.amazonaws.com`

### Secret 2: AWS_EC2_USER
- **Name:** `AWS_EC2_USER`
- **Value:** `ubuntu`

### Secret 3: EC2_SSH_KEY
- **Name:** `EC2_SSH_KEY`
- **Value:** Full content of your .pem file

**To get .pem content (on your local machine):**
```bash
cat your-key.pem
```
Copy everything including `-----BEGIN RSA PRIVATE KEY-----` and `-----END RSA PRIVATE KEY-----`

### Secret 4: RDS_CONNECTION_STRING
- **Name:** `RDS_CONNECTION_STRING`
- **Value:** Your RDS connection string
- **Example:** 
```
Server=database-1.cbko6ouwuxuh.ap-south-1.rds.amazonaws.com,1433;Database=MicroJobBoardDb;User Id=admin;Password=YOUR_PASSWORD;TrustServerCertificate=True;Encrypt=True;
```

### Secret 5: JWT_SECRET_KEY
- **Name:** `JWT_SECRET_KEY`
- **Value:** Generate a strong key

**Generate on your local machine:**
```bash
# On Linux/Mac
openssl rand -base64 32

# On Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```
Copy the output and use it as the secret value.

### Secret 6: FRONTEND_URL
- **Name:** `FRONTEND_URL`
- **Value:** Your frontend URL
- **Example:** `http://localhost:3000` or `http://your-frontend-ip:3000`

---

## STEP 8: Update EC2 Security Group

Go to AWS Console:

1. Go to **EC2 → Security Groups**
2. Select your EC2 instance's security group
3. Click **"Edit inbound rules"**
4. Add these rules:

| Type | Protocol | Port | Source | Description |
|------|----------|------|--------|-------------|
| SSH | TCP | 22 | My IP | SSH access |
| Custom TCP | TCP | 5000 | 0.0.0.0/0 | API access |

5. Click **"Save rules"**

---

## STEP 9: Update RDS Security Group

1. Go to **RDS → Databases → Your database**
2. Click on the **VPC security group**
3. Click **"Edit inbound rules"**
4. Add this rule:

| Type | Protocol | Port | Source | Description |
|------|----------|------|--------|-------------|
| MSSQL/Aurora | TCP | 1433 | EC2 Security Group | Database access |

5. Click **"Save rules"**

---

## STEP 10: Deploy Your Application

On your local machine (in your project directory):

```bash
# Check current branch
git branch

# Add all changes
git add .

# Commit changes
git commit -m "Setup EC2 deployment with HTTP"

# Push to main branch
git push origin main
```

---

## STEP 11: Monitor Deployment

1. Go to: `https://github.com/YOUR_USERNAME/YOUR_REPO/actions`
2. Click on the latest workflow run
3. Watch the **build** and **deploy** steps
4. Wait for green checkmarks ✅

---

## STEP 12: Verify Deployment

### Check Service Status (SSH into EC2):

```bash
# Connect to EC2
ssh -i your-key.pem ubuntu@your-ec2-ip

# Check service status
sudo systemctl status microjobboard-api

# View logs
sudo journalctl -u microjobboard-api -n 50

# Test API locally
curl http://localhost:5000/swagger/index.html
```

### Test from Browser:

Open: `http://your-ec2-ip:5000/swagger`

You should see the Swagger UI! 🎉

---

## STEP 13: Test API Endpoints

In Swagger UI:

1. **Register a user:**
   - Expand `POST /api/auth/register`
   - Click "Try it out"
   - Fill in the details
   - Click "Execute"

2. **Login:**
   - Expand `POST /api/auth/login`
   - Click "Try it out"
   - Enter email and password
   - Click "Execute"
   - Copy the token from response

3. **Authorize:**
   - Click the green "Authorize" button at top
   - Enter: `Bearer YOUR_TOKEN`
   - Click "Authorize"

4. **Test protected endpoints:**
   - Try any endpoint that requires authentication

---

## Troubleshooting Commands

### If service fails to start:

```bash
# Check detailed logs
sudo journalctl -u microjobboard-api -n 100 --no-pager

# Check if files exist
ls -la /var/www/dotnetapi/

# Check .NET runtime
dotnet --list-runtimes

# Restart service manually
sudo systemctl restart microjobboard-api
```

### If database connection fails:

```bash
# Test RDS connectivity
telnet database-1.cbko6ouwuxuh.ap-south-1.rds.amazonaws.com 1433

# If telnet not installed
sudo apt install telnet -y
```

### View real-time logs:

```bash
sudo journalctl -u microjobboard-api -f
```

Press `Ctrl + C` to stop following logs.

---

## Useful Commands for Later

```bash
# Restart API
sudo systemctl restart microjobboard-api

# Stop API
sudo systemctl stop microjobboard-api

# Start API
sudo systemctl start microjobboard-api

# Check status
sudo systemctl status microjobboard-api

# View recent logs
sudo journalctl -u microjobboard-api -n 100

# Follow logs in real-time
sudo journalctl -u microjobboard-api -f
```

---

## Redeploy After Code Changes

```bash
# On your local machine
git add .
git commit -m "Your changes"
git push origin main

# GitHub Actions will automatically deploy!
# Monitor at: https://github.com/YOUR_USERNAME/YOUR_REPO/actions
```

---

## Summary

✅ EC2 setup complete  
✅ .NET 8 installed  
✅ Systemd service configured  
✅ GitHub secrets added  
✅ Security groups configured  
✅ Application deployed  
✅ API accessible at `http://your-ec2-ip:5000`  

**You're done! 🎉**
