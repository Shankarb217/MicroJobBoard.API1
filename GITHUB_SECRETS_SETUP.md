# GitHub Secrets Setup Guide

This document lists all required GitHub secrets for the CI/CD pipeline.

## How to Add Secrets

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add each secret below

---

## Required Secrets (6 Total)

### 1. AWS_EC2_HOST
**Description:** Your EC2 instance public IP address or DNS name

**Example:**
```
ec2-13-233-123-456.ap-south-1.compute.amazonaws.com
```
or
```
13.233.123.456
```

**How to find:**
```bash
# In AWS Console → EC2 → Instances → Select your instance
# Copy "Public IPv4 address" or "Public IPv4 DNS"
```

---

### 2. AWS_EC2_USER
**Description:** SSH username for your EC2 instance

**Value depends on AMI:**
- Ubuntu: `ubuntu`
- Amazon Linux: `ec2-user`
- Debian: `admin`
- RHEL: `ec2-user`

**Example:**
```
ubuntu
```

---

### 3. EC2_SSH_KEY
**Description:** Complete content of your EC2 private key (.pem file)

**How to get:**
```bash
# On your local machine
cat /path/to/your-key.pem
```

**Important:** Copy the ENTIRE content including:
```
-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA...
... (all lines) ...
-----END RSA PRIVATE KEY-----
```

**⚠️ Security Warning:** Never commit this key to your repository!

---

### 4. RDS_CONNECTION_STRING
**Description:** Complete connection string for AWS RDS MSSQL Server

**Format:**
```
Server=<RDS_ENDPOINT>,1433;Database=<DB_NAME>;User Id=<USERNAME>;Password=<PASSWORD>;TrustServerCertificate=True;Encrypt=True;
```

**Your current RDS endpoint:**
```
database-1.cbko6ouwuxuh.ap-south-1.rds.amazonaws.com
```

**Example:**
```
Server=database-1.cbko6ouwuxuh.ap-south-1.rds.amazonaws.com,1433;Database=MicroJobBoardDb;User Id=admin;Password=YourSecurePassword123!;TrustServerCertificate=True;Encrypt=True;
```

**⚠️ Important:** 
- Replace `YourSecurePassword123!` with your actual RDS password
- DO NOT use the password from appsettings.json if it's committed to Git
- Change your RDS password if it's been exposed

---

### 5. JWT_SECRET_KEY
**Description:** Secret key for JWT token generation (minimum 32 characters)

**Generate a strong key:**
```bash
# Option 1: Using OpenSSL (recommended)
openssl rand -base64 32

# Option 2: Using PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))

# Option 3: Online generator
# Visit: https://generate-random.org/api-token-generator
```

**Example output:**
```
8K7L9M2N4P6Q8R0S2T4U6V8W0X2Y4Z6A8B0C2D4E6F8G0H=
```

**⚠️ Important:** 
- Must be at least 32 characters
- Use a cryptographically secure random generator
- Never reuse keys across environments

---

### 6. FRONTEND_URL
**Description:** URL of your frontend application (for CORS configuration)

**Examples:**

Production:
```
https://yourdomain.com
```

Development/Testing:
```
http://localhost:3000
```

Multiple origins (if needed, modify workflow):
```
https://yourdomain.com,https://www.yourdomain.com
```

**Note:** Do not include trailing slash

---

## Verification Checklist

Before deploying, verify:

- [ ] All 6 secrets are added to GitHub
- [ ] `EC2_SSH_KEY` includes BEGIN and END markers
- [ ] `RDS_CONNECTION_STRING` has correct endpoint and credentials
- [ ] `JWT_SECRET_KEY` is at least 32 characters
- [ ] `AWS_EC2_HOST` is accessible from internet
- [ ] EC2 security group allows SSH (port 22)
- [ ] RDS security group allows EC2 access (port 1433)
- [ ] Systemd service is created on EC2 (see DEPLOYMENT.md)

---

## Testing Secrets

After adding secrets, test by:

1. Push a commit to `main` branch
2. Go to **Actions** tab in GitHub
3. Watch the workflow run
4. Check for any authentication or connection errors

---

## Security Best Practices

✅ **DO:**
- Rotate secrets regularly (every 90 days)
- Use different secrets for dev/staging/production
- Limit GitHub Actions permissions
- Enable 2FA on GitHub account
- Use AWS IAM roles when possible

❌ **DON'T:**
- Share secrets via email or chat
- Commit secrets to repository
- Use weak or default passwords
- Reuse secrets across projects
- Store secrets in plain text files

---

## Troubleshooting

### "Permission denied (publickey)" error
- Check `EC2_SSH_KEY` is complete and correct
- Verify `AWS_EC2_USER` matches your AMI type
- Ensure EC2 security group allows SSH

### "Could not connect to database" error
- Verify `RDS_CONNECTION_STRING` is correct
- Check RDS security group allows EC2
- Confirm RDS is running and accessible

### "Invalid JWT token" error
- Ensure `JWT_SECRET_KEY` is at least 32 characters
- Verify secret is properly encoded (no extra spaces)

---

## Need Help?

Refer to:
- [DEPLOYMENT.md](./DEPLOYMENT.md) - Full deployment guide
- [GitHub Actions Logs](../../actions) - Workflow execution logs
- [AWS Console](https://console.aws.amazon.com) - EC2 and RDS management
