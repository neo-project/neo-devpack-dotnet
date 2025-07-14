# R3E WebGUI Service - Deployment Guide

## 🎯 Choose Your Deployment Method

### For Testing/Development: Option A (Simple)
### For Production: Option B (Docker)
### For Cloud: Option C (Cloud Provider)

---

## Option A: Simple Local Development

### Prerequisites
- .NET 9.0 SDK
- SQL Server Express or LocalDB (optional)

### Steps

1. **Navigate to the service directory**:
   ```bash
   cd /home/neo/git/neo-devpack-dotnet/src/R3E.WebGUI.Service
   ```

2. **Configure the database** (optional - uses in-memory by default):
   ```bash
   # Edit appsettings.Development.json if you want to use a real database
   # Otherwise, it will use in-memory database
   ```

3. **Run the service**:
   ```bash
   dotnet run --environment Development
   ```

4. **Access the service**:
   - API: http://localhost:5000
   - Swagger Documentation: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health

### Quick Test
```bash
# Test the health endpoint
curl http://localhost:5000/health

# Test the API documentation
open http://localhost:5000/swagger
```

---

## Option B: Docker Deployment (Recommended)

### Prerequisites
- Docker and Docker Compose
- 4GB+ available RAM
- 10GB+ available disk space

### Development Deployment

1. **Run the deployment script**:
   ```bash
   cd /home/neo/git/neo-devpack-dotnet/src/R3E.WebGUI.Service
   chmod +x scripts/deploy.sh
   ./scripts/deploy.sh development
   ```

2. **Wait for services to start** (this may take 2-3 minutes):
   ```bash
   # Check service status
   docker-compose ps
   ```

3. **Access the services**:
   - API: http://localhost:8080
   - Swagger: http://localhost:8080/swagger
   - Health: http://localhost:8080/health
   - Database: localhost:1433 (sa/YourStrong@Passw0rd)

### Production Deployment

1. **Configure production settings**:
   ```bash
   # Edit docker-compose.yml and update:
   # - Database passwords
   # - Domain names
   # - SSL certificates
   # - API keys (if needed)
   ```

2. **Deploy to production**:
   ```bash
   ./scripts/deploy.sh production
   ```

3. **Verify deployment**:
   ```bash
   # Check all services are healthy
   docker-compose ps
   
   # Test API endpoints
   curl -f http://localhost:8080/health
   ```

### Docker Commands Reference
```bash
# View logs
docker-compose logs -f r3e-webgui-service

# Stop services
docker-compose down

# Restart specific service
docker-compose restart r3e-webgui-service

# Access database
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa

# View storage files
docker exec -it r3e-webgui-service ls -la /app/webgui-storage
```

---

## Option C: Cloud Provider Deployment

### Azure Container Instances

1. **Build and push image**:
   ```bash
   # Build the image
   docker build -t r3e-webgui-service .
   
   # Tag for Azure Container Registry
   docker tag r3e-webgui-service yourregistry.azurecr.io/r3e-webgui-service:latest
   
   # Push to registry
   docker push yourregistry.azurecr.io/r3e-webgui-service:latest
   ```

2. **Deploy with Azure CLI**:
   ```bash
   az container create \
     --resource-group myResourceGroup \
     --name r3e-webgui-service \
     --image yourregistry.azurecr.io/r3e-webgui-service:latest \
     --ports 8080 \
     --environment-variables \
       ASPNETCORE_ENVIRONMENT=Production \
       ConnectionStrings__DefaultConnection="your-db-connection"
   ```

### AWS ECS/Fargate

1. **Create task definition**:
   ```json
   {
     "family": "r3e-webgui-service",
     "networkMode": "awsvpc",
     "requiresCompatibilities": ["FARGATE"],
     "cpu": "512",
     "memory": "1024",
     "containerDefinitions": [
       {
         "name": "r3e-webgui-service",
         "image": "your-account.dkr.ecr.region.amazonaws.com/r3e-webgui-service:latest",
         "portMappings": [
           {
             "containerPort": 8080,
             "protocol": "tcp"
           }
         ],
         "environment": [
           {
             "name": "ASPNETCORE_ENVIRONMENT",
             "value": "Production"
           }
         ]
       }
     ]
   }
   ```

2. **Deploy with AWS CLI**:
   ```bash
   aws ecs create-service \
     --cluster your-cluster \
     --service-name r3e-webgui-service \
     --task-definition r3e-webgui-service:1 \
     --desired-count 2
   ```

### Google Cloud Run

1. **Deploy directly**:
   ```bash
   gcloud run deploy r3e-webgui-service \
     --image gcr.io/your-project/r3e-webgui-service \
     --platform managed \
     --region us-central1 \
     --allow-unauthenticated \
     --port 8080
   ```

---

## 🔧 Configuration Guide

### Environment Variables

Set these environment variables for production:

```bash
# Database
export ConnectionStrings__DefaultConnection="Server=your-server;Database=R3EWebGUIService;User Id=sa;Password=your-password"

# Storage
export Storage__LocalPath="/app/webgui-storage"

# Security
export Security__RequireHttps=true
export Security__EnableApiKey=true
export WEBGUI_API_KEY="your-secret-api-key"

# Rate Limiting
export RateLimit__MaxRequestsPerMinute=100
export RateLimit__MaxDeploymentsPerDay=100

# Domain
export R3EWebGUI__BaseDomain="yourdomain.com"
```

### Database Setup

For production, you'll need a proper database:

```sql
-- Create database
CREATE DATABASE R3EWebGUIService;

-- Create user (recommended)
CREATE LOGIN r3ewebgui WITH PASSWORD = 'YourSecurePassword123!';
USE R3EWebGUIService;
CREATE USER r3ewebgui FOR LOGIN r3ewebgui;
ALTER ROLE db_owner ADD MEMBER r3ewebgui;
```

### SSL/HTTPS Setup

For production with custom domain:

1. **Get SSL certificate** (Let's Encrypt recommended):
   ```bash
   # Using certbot
   certbot certonly --webroot -w /var/www/html -d yourdomain.com -d *.yourdomain.com
   ```

2. **Update nginx.conf**:
   ```nginx
   server {
       listen 443 ssl;
       server_name api.yourdomain.com;
       
       ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
       ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
       
       # ... rest of configuration
   }
   ```

---

## 🧪 Testing Your Deployment

### Basic Health Check
```bash
# Test API health
curl -f http://localhost:8080/health

# Expected response:
# "Healthy"
```

### Deploy a Test WebGUI
```bash
# Create test files
mkdir test-webgui
echo '<html><body><h1>Test Contract</h1></body></html>' > test-webgui/index.html
echo 'body { font-family: Arial; }' > test-webgui/style.css

# Deploy using curl
curl -X POST "http://localhost:8080/api/webgui/deploy" \
  -F "contractAddress=0x1234567890abcdef1234567890abcdef12345678" \
  -F "contractName=TestContract" \
  -F "network=testnet" \
  -F "deployerAddress=0xabcdef1234567890abcdef1234567890abcdef12" \
  -F "description=Test deployment" \
  -F "webGUIFiles=@test-webgui/index.html" \
  -F "webGUIFiles=@test-webgui/style.css"

# Expected response:
# {
#   "success": true,
#   "subdomain": "testcontract",
#   "url": "https://testcontract.r3e-gui.com",
#   "contractAddress": "0x1234567890abcdef1234567890abcdef12345678"
# }
```

### Test Contract Search
```bash
# Search for the deployed contract
curl "http://localhost:8080/api/webgui/search?contractAddress=0x1234567890abcdef1234567890abcdef12345678&network=testnet"
```

---

## 🚨 Troubleshooting

### Common Issues

#### Service Won't Start
```bash
# Check logs
docker-compose logs r3e-webgui-service

# Common causes:
# 1. Database connection issues
# 2. Port conflicts
# 3. Missing environment variables
```

#### Database Connection Errors
```bash
# Test database connectivity
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1"

# Check connection string format
# Should be: Server=sqlserver;Database=R3EWebGUIService;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
```

#### File Upload Issues
```bash
# Check storage permissions
docker exec -it r3e-webgui-service ls -la /app/webgui-storage

# Check disk space
df -h

# Check file size limits in configuration
```

#### Subdomain Routing Issues
```bash
# For local testing, add to /etc/hosts:
echo "127.0.0.1 testcontract.r3e-gui.local" >> /etc/hosts
echo "127.0.0.1 api.r3e-gui.local" >> /etc/hosts

# Test subdomain access
curl -H "Host: testcontract.r3e-gui.local" http://localhost:8080/
```

### Getting Help

1. **Check logs**: Always start with the logs
2. **Verify configuration**: Ensure all environment variables are set
3. **Test components**: Test database, storage, and network separately
4. **Check documentation**: Refer to the comprehensive docs in `/docs/`

---

## 📋 Production Checklist

Before going live:

- [ ] Database backup strategy configured
- [ ] SSL certificates installed and renewed automatically
- [ ] Monitoring and alerting set up
- [ ] Rate limits configured appropriately
- [ ] Security headers configured
- [ ] Firewall rules configured
- [ ] Domain DNS configured for wildcards
- [ ] Load testing completed
- [ ] Disaster recovery plan documented

---

## 🎯 Next Steps

After deployment:

1. **Monitor the service**: Watch logs and metrics
2. **Test all features**: Deploy WebGUIs, search contracts
3. **Set up monitoring**: Configure alerts and dashboards
4. **Document your setup**: Keep deployment notes
5. **Plan for scaling**: Consider load balancing and redundancy

Your R3E WebGUI Service is now ready to host Neo smart contract interfaces! 🚀