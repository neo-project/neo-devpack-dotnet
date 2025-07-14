# 🚀 Quick Setup Guide - R3E WebGUI Service

This guide will help you deploy the R3E WebGUI Service so contracts are accessible at `contract.yourdomain.com`.

## 📋 What You'll Get

After deployment, you'll have:
- **API Endpoint**: `https://api.yourdomain.com`
- **Contract WebGUIs**: `https://[contract-name].yourdomain.com`
- **Documentation**: `https://api.yourdomain.com/swagger`
- **Health Monitoring**: `https://api.yourdomain.com/health`

## 🔧 Prerequisites

1. **Server with Docker**:
   - Ubuntu 20.04+ or similar Linux distribution
   - Docker and Docker Compose installed
   - At least 2GB RAM, 20GB disk space
   - Ports 80 and 443 open

2. **Domain Name**:
   - A domain you control (e.g., `yourdomain.com`)
   - Access to DNS settings

## ⚡ Quick Deployment (5 minutes)

### Step 1: Clone and Navigate
```bash
git clone https://github.com/r3e-network/neo-devpack-dotnet.git
cd neo-devpack-dotnet/src/R3E.WebGUI.Service
```

### Step 2: Run Deployment Script
```bash
# Make script executable
chmod +x scripts/deploy-production.sh

# Run deployment (will prompt for domain and email)
./scripts/deploy-production.sh
```

The script will ask you for:
- **Your domain** (e.g., `yourdomain.com`)
- **Your email** (for SSL certificates)
- **Database password** (or use default)

### Step 3: Configure DNS

Add these DNS records to your domain:

| Type | Name | Value |
|------|------|-------|
| A | `yourdomain.com` | `YOUR_SERVER_IP` |
| A | `api.yourdomain.com` | `YOUR_SERVER_IP` |
| A | `*.yourdomain.com` | `YOUR_SERVER_IP` |

### Step 4: Test the Service

```bash
# Test health endpoint
curl https://api.yourdomain.com/health

# Should return: "Healthy"
```

## 🧪 Deploy Your First Contract WebGUI

### Using curl:
```bash
# Create test WebGUI files
mkdir test-contract
echo '<html><body><h1>My Neo Contract</h1><p>Contract Address: 0x1234...</p></body></html>' > test-contract/index.html

# Deploy to service
curl -X POST "https://api.yourdomain.com/api/webgui/deploy" \
  -F "contractAddress=0x1234567890abcdef1234567890abcdef12345678" \
  -F "contractName=MyContract" \
  -F "network=testnet" \
  -F "deployerAddress=0xabcdef1234567890abcdef1234567890abcdef12" \
  -F "description=My first contract WebGUI" \
  -F "webGUIFiles=@test-contract/index.html"

# Response will include the subdomain URL
# Access your contract at: https://mycontract.yourdomain.com
```

### Using the R3E Compiler (Integrated):
```bash
# Install R3E compiler tool
dotnet tool install -g R3E.Compiler.CSharp.Tool

# Compile and deploy WebGUI in one command
r3e-compiler MyContract.csproj \
  --generate-webgui \
  --deploy-webgui \
  --contract-address 0x1234567890abcdef1234567890abcdef12345678 \
  --deployer-address 0xabcdef1234567890abcdef1234567890abcdef12 \
  --network testnet \
  --webgui-service-url https://api.yourdomain.com
```

## 🔧 Advanced Configuration

### Custom Environment Variables

Create a `.env` file:
```bash
cp .env.example .env
# Edit .env with your settings
```

### Custom Rate Limits

Edit `docker-compose.production.yml`:
```yaml
environment:
  - RateLimit__MaxRequestsPerMinute=200
  - RateLimit__MaxDeploymentsPerDay=500
```

### Enable API Key Authentication

```yaml
environment:
  - Security__EnableApiKey=true
  - WEBGUI_API_KEY=your-secret-key
```

Then include the API key in requests:
```bash
curl -H "X-API-Key: your-secret-key" https://api.yourdomain.com/api/webgui/deploy
```

## 📊 Monitoring

### Check Service Status
```bash
# View all services
docker-compose -f docker-compose.production.yml ps

# View logs
docker-compose -f docker-compose.production.yml logs -f
```

### Health Checks
```bash
# API health
curl https://api.yourdomain.com/health

# Database health
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -Q "SELECT 1"
```

### View Metrics
- Access logs: `docker-compose logs nginx`
- API metrics: `docker-compose logs r3e-webgui-service`
- Database logs: `docker-compose logs sqlserver`

## 🔒 Security Checklist

- [ ] Change default database password
- [ ] Enable firewall (allow only 80, 443, 22)
- [ ] Set up SSL certificate auto-renewal
- [ ] Configure API key authentication (optional)
- [ ] Set up log monitoring
- [ ] Regular backups of database

## 🛠️ Maintenance

### Update the Service
```bash
# Pull latest changes
git pull origin main

# Rebuild and restart
docker-compose -f docker-compose.production.yml up -d --build
```

### Backup Database
```bash
# Create backup
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -Q "BACKUP DATABASE R3EWebGUIService TO DISK = '/var/opt/mssql/backup/webgui-backup.bak'"

# Copy backup to host
docker cp r3e-webgui-sqlserver:/var/opt/mssql/backup/webgui-backup.bak ./backups/
```

### SSL Certificate Renewal
```bash
# Renew certificates (automatic with Let's Encrypt)
docker-compose -f docker-compose.production.yml run --rm certbot renew

# Restart nginx
docker-compose -f docker-compose.production.yml restart nginx
```

## 🆘 Troubleshooting

### Service Won't Start
```bash
# Check logs
docker-compose -f docker-compose.production.yml logs

# Common issues:
# 1. Port conflicts (check if 80/443 are in use)
# 2. DNS not configured
# 3. SSL certificate issues
```

### SSL Certificate Issues
```bash
# Check certificate status
docker-compose -f docker-compose.production.yml exec nginx ls -la /etc/letsencrypt/live/

# Regenerate certificates
docker-compose -f docker-compose.production.yml run --rm certbot certonly --force-renewal
```

### Database Connection Issues
```bash
# Test database
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa

# Reset database
docker-compose -f docker-compose.production.yml down -v
docker-compose -f docker-compose.production.yml up -d
```

## 📞 Support

- **Documentation**: See `/docs/R3E_WEBGUI_SERVICE.md` for complete documentation
- **Issues**: Report bugs at [GitHub Issues](https://github.com/r3e-network/neo-devpack-dotnet/issues)
- **Community**: Join the R3E Discord server

---

## 🎉 You're Ready!

Your R3E WebGUI Service is now running and ready to host Neo smart contract interfaces at `contract.yourdomain.com`!

### Next Steps:
1. Deploy your first contract WebGUI
2. Set up monitoring and alerting
3. Configure automatic backups
4. Share with the Neo developer community

**Happy hosting! 🚀**