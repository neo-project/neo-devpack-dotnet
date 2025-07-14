# Docker Deployment Guide for R3E WebGUI Service

This guide explains how to deploy the R3E WebGUI Service using Docker and Docker Compose.

## Prerequisites

- Docker Engine 20.10 or later
- Docker Compose 2.0 or later
- At least 4GB of available RAM
- Ports 8888 and 1433 available (can be changed in docker-compose.yml)

## Quick Start

1. **Start the service with the helper script:**
   ```bash
   cd /home/neo/git/neo-devpack-dotnet/src/R3E.WebGUI.Service
   ./scripts/start-service.sh
   ```

2. **For development mode with debug logging:**
   ```bash
   ./scripts/start-service.sh dev
   ```

3. **To follow logs after startup:**
   ```bash
   ./scripts/start-service.sh dev logs
   ```

## Manual Docker Compose Commands

### Start Services
```bash
# Production mode
docker-compose up -d

# Development mode
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d

# Build and start
docker-compose up -d --build
```

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f r3e-webgui-service
docker-compose logs -f sqlserver
```

### Stop Services
```bash
docker-compose down

# Remove volumes (WARNING: This deletes all data!)
docker-compose down -v
```

## Service URLs

- **Web Interface**: http://localhost:8888
- **Health Check**: http://localhost:8888/health
- **API Documentation**: http://localhost:8888/swagger
- **SQL Server**: localhost:1433 (sa / R3E_Strong_Pass_2024!)

## Configuration

### Environment Variables

The service is configured through environment variables in `docker-compose.yml`:

```yaml
- R3EWebGUI__BaseDomain=localhost           # Base domain for subdomains
- R3EWebGUI__StorageBasePath=/app/webgui-storage  # Storage path
- R3EWebGUI__MaxFileSizeKB=5120            # Max file size (5MB)
- R3EWebGUI__AllowedNetworks=["testnet", "mainnet"]
- NEO_RPC_TESTNET=https://test1.neo.coz.io:443
- NEO_RPC_MAINNET=https://mainnet1.neo.coz.io:443
```

### Volumes

- `webgui-storage`: Stores uploaded WebGUI files
- `webgui-logs`: Application logs
- `sqlserver-data`: SQL Server database files

## Database Management

### Access SQL Server
```bash
# Connect to SQL Server container
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024!

# From host using sqlcmd or Azure Data Studio
Server: localhost,1433
Username: sa
Password: R3E_Strong_Pass_2024!
Database: R3EWebGUI
```

### Run Migrations Manually
```bash
docker exec r3e-webgui-service dotnet ef database update
```

## Troubleshooting

### Service Won't Start

1. Check if ports are in use:
   ```bash
   sudo lsof -i :8888
   sudo lsof -i :1433
   ```

2. Check container logs:
   ```bash
   docker-compose logs r3e-webgui-service
   docker-compose logs sqlserver
   ```

3. Verify SQL Server is healthy:
   ```bash
   docker exec r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024! -Q "SELECT 1"
   ```

### Database Connection Issues

1. Ensure SQL Server is running:
   ```bash
   docker ps | grep sqlserver
   ```

2. Check SQL Server logs:
   ```bash
   docker logs r3e-webgui-sqlserver
   ```

3. Recreate database:
   ```bash
   docker exec r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024! -Q "DROP DATABASE IF EXISTS R3EWebGUI; CREATE DATABASE R3EWebGUI;"
   ```

### Reset Everything

To completely reset the service and database:
```bash
docker-compose down -v
docker-compose up -d --build
```

## Production Deployment

For production deployment:

1. Change passwords in `docker-compose.yml`
2. Use HTTPS with proper SSL certificates
3. Configure firewall rules
4. Set up monitoring and backups
5. Use Docker secrets for sensitive data

## Monitoring

### Health Checks
```bash
# Service health
curl http://localhost:8888/health

# Database health
docker exec r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024! -Q "SELECT 1"
```

### Resource Usage
```bash
# Container stats
docker stats r3e-webgui-service r3e-webgui-sqlserver

# Disk usage
docker system df
```