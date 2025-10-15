# HTTP-Only Deployment Configuration

## Overview

This project is configured for **HTTP-only deployment** on AWS EC2. No HTTPS, SSL certificates, or reverse proxy (Nginx) are required.

---

## Configuration Details

### API Access
- **Protocol:** HTTP
- **Port:** 5000
- **URL Format:** `http://your-ec2-ip:5000`
- **Swagger UI:** `http://your-ec2-ip:5000/swagger`

### What's Disabled
- ✅ HTTPS redirection is commented out in `Program.cs`
- ✅ No SSL certificate required
- ✅ No Nginx reverse proxy needed
- ✅ Direct HTTP access on port 5000

---

## Security Group Configuration

### EC2 Security Group (Inbound Rules)

| Type | Protocol | Port | Source | Description |
|------|----------|------|--------|-------------|
| SSH | TCP | 22 | Your IP | SSH access |
| Custom TCP | TCP | 5000 | 0.0.0.0/0 | API HTTP access (public) |

**Note:** For better security, restrict port 5000 to specific IPs (your frontend server IP) instead of 0.0.0.0/0

### RDS Security Group (Inbound Rules)

| Type | Protocol | Port | Source | Description |
|------|----------|------|--------|-------------|
| MSSQL | TCP | 1433 | EC2 Security Group | Database access from EC2 |

---

## Testing Your Deployment

### From Browser
```
http://your-ec2-ip:5000/swagger
```

### From Command Line
```bash
# Test from EC2 instance
curl http://localhost:5000/swagger/index.html

# Test from your local machine
curl http://your-ec2-ip:5000/swagger/index.html
```

### From Frontend Application
```javascript
// Update your frontend API base URL to:
const API_BASE_URL = 'http://your-ec2-ip:5000';
```

---

## CORS Configuration

Make sure your `FRONTEND_URL` GitHub secret includes the HTTP protocol:

**Correct:**
```
http://your-frontend-domain.com
```

**Incorrect:**
```
https://your-frontend-domain.com  ❌ (if frontend is also HTTP)
your-frontend-domain.com          ❌ (missing protocol)
```

---

## Code Changes Made

### Program.cs
```csharp
// HTTPS redirection disabled for HTTP-only deployment
// app.UseHttpsRedirection();
```

This line is commented out to allow HTTP-only access without redirecting to HTTPS.

---

## When to Add HTTPS (Optional)

Consider adding HTTPS if:
- You have a domain name
- You need encrypted traffic
- You're handling sensitive data beyond authentication
- Your frontend requires HTTPS

### How to Add HTTPS Later (Optional)

If you decide to add HTTPS later:

1. **Get a domain name** and point it to your EC2 IP
2. **Install Nginx** as reverse proxy
3. **Get SSL certificate** from Let's Encrypt
4. **Configure Nginx** to proxy to port 5000
5. **Uncomment** `app.UseHttpsRedirection()` in Program.cs
6. **Update security group** to allow port 443

---

## Advantages of HTTP-Only Setup

✅ **Simpler deployment** - No SSL certificate management  
✅ **Faster setup** - No Nginx configuration needed  
✅ **Direct access** - API runs directly on port 5000  
✅ **Easier debugging** - No proxy layer to troubleshoot  
✅ **Lower complexity** - Fewer moving parts  

---

## Considerations

⚠️ **Traffic is not encrypted** - Data travels in plain text  
⚠️ **Browser warnings** - Some browsers may show "Not Secure"  
⚠️ **API keys visible** - JWT tokens can be intercepted  
⚠️ **Not recommended for production** - If handling sensitive data  

---

## Best Practices for HTTP Deployment

1. **Use strong JWT secrets** - Even without HTTPS
2. **Restrict security groups** - Limit access to known IPs
3. **Use VPC** - Keep RDS in private subnet
4. **Monitor logs** - Watch for suspicious activity
5. **Rotate credentials** - Change passwords regularly
6. **Consider VPN** - For admin access

---

## Summary

Your deployment is configured for:
- ✅ HTTP on port 5000
- ✅ No HTTPS/SSL required
- ✅ No Nginx needed
- ✅ Direct API access
- ✅ Simple and straightforward

**API will be accessible at:** `http://your-ec2-ip:5000`
