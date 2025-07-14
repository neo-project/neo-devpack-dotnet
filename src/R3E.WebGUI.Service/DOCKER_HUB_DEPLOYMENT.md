# R3E WebGUI Service - Docker Hub Deployment

🚀 **Revolutionary hosting service for Neo smart contracts** - Deploy professional web interfaces from contract manifests in minutes!

## 🏗️ Architecture

The R3E WebGUI Service consists of three main components:

- **R3E WebGUI Service**: ASP.NET Core 9.0 application with signature-based authentication
- **SQL Server**: Microsoft SQL Server 2022 for data persistence  
- **Nginx**: Reverse proxy with SSL/TLS termination and subdomain routing

## 📦 Docker Hub Images

The service is available on Docker Hub:

- **Main Service**: `r3enetwork/r3e-webgui-service:latest`
- **Tagged Version**: `r3enetwork/r3e-webgui-service:0.0.2`

## 🚀 Quick Start

### Prerequisites

- Docker Engine 20.10 or higher
- Docker Compose 2.0 or higher
- 4GB+ RAM available
- Ports 80, 443, 1433, 8888 available

### One-Command Deployment

```bash
# Download the deployment script
curl -o deploy-docker-hub.sh https://raw.githubusercontent.com/neo-project/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/deploy-docker-hub.sh

# Make it executable
chmod +x deploy-docker-hub.sh

# Deploy the complete stack
./deploy-docker-hub.sh deploy
```

### Manual Deployment

1. **Download Docker Compose file**:
```bash
curl -o docker-compose.hub.yml https://raw.githubusercontent.com/neo-project/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/docker-compose.hub.yml
```

2. **Create environment file**:
```bash
cat > .env << EOF
BASE_DOMAIN=localhost
DB_PASSWORD=R3E_Strong_Pass_2024!
EOF
```

3. **Deploy services**:
```bash
docker-compose -f docker-compose.hub.yml up -d
```

4. **Wait for services to be ready**:
```bash
# Wait for health check
curl http://localhost:8888/health
# Should return: Healthy
```

## 🔧 Configuration

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `BASE_DOMAIN` | `localhost` | Base domain for WebGUI hosting |
| `DB_PASSWORD` | `R3E_Strong_Pass_2024!` | SQL Server SA password |
| `NEO_RPC_TESTNET` | `https://test1.neo.coz.io:443` | Neo TestNet RPC endpoint |
| `NEO_RPC_MAINNET` | `https://mainnet1.neo.coz.io:443` | Neo MainNet RPC endpoint |

### Custom Configuration

```bash
# Deploy with custom domain and password
BASE_DOMAIN=mycompany.com DB_PASSWORD=MySecurePass123! ./deploy-docker-hub.sh deploy
```

## 🌐 Access Points

After deployment, the service provides several access points:

| Service | URL | Description |
|---------|-----|-------------|
| **Health Check** | http://localhost:8888/health | Service health status |
| **API Base** | http://localhost:8888/api | REST API endpoints |
| **Contract WebGUIs** | http://CONTRACT.localhost:8888 | Dynamic contract interfaces |
| **SQL Server** | localhost:1433 | Database (sa/DB_PASSWORD) |

## 🔐 Authentication

The service uses **signature-based authentication** - no user registration required:

1. **Contract Deployment**: Requires signature from contract deployer's address
2. **Plugin Upload**: Validates signature for authenticity
3. **WebGUI Access**: Public access to deployed interfaces

## 📡 API Usage

### Deploy Contract WebGUI

```bash
curl -X POST http://localhost:8888/api/webgui/deploy-from-manifest \
  -H "Content-Type: application/json" \
  -d '{
    "contractAddress": "0x1234...abcd",
    "contractName": "MyContract",
    "network": "testnet",
    "deployerAddress": "NXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx",
    "description": "My awesome contract WebGUI",
    "timestamp": 1640995200,
    "signature": "signature_from_neo_wallet"
  }'
```

### Access Contract WebGUI

After deployment, your contract WebGUI will be available at:
```
http://CONTRACT_ADDRESS.localhost:8888
```

## 🛠️ Management Commands

The deployment script provides several management commands:

```bash
# Check status
./deploy-docker-hub.sh status

# View logs
./deploy-docker-hub.sh logs

# Restart services  
./deploy-docker-hub.sh restart

# Stop services
./deploy-docker-hub.sh stop

# Clean up (removes containers and optionally data)
./deploy-docker-hub.sh cleanup
```

## 🔍 Monitoring

### Health Checks

All services include health checks:

```bash
# Service health
curl http://localhost:8888/health

# Database connectivity  
docker exec r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YOUR_PASSWORD -Q "SELECT 1" -C

# Container status
docker ps --filter "name=r3e-webgui"
```

### Logs

```bash
# All services
docker-compose -f docker-compose.hub.yml logs

# Specific service
docker logs r3e-webgui-service
docker logs r3e-webgui-sqlserver  
docker logs r3e-webgui-nginx
```

## 🚨 Troubleshooting

### Common Issues

**1. Service returns 503 (Service Unavailable)**
```bash
# Check if SQL Server is ready
docker logs r3e-webgui-sqlserver | grep "Recovery is complete"

# Wait for database startup (can take 60+ seconds)
sleep 60 && curl http://localhost:8888/health
```

**2. Port conflicts**
```bash
# Check what's using the ports
sudo netstat -tulpn | grep -E "(80|443|1433|8888)"

# Use different ports in docker-compose.hub.yml
ports:
  - "8080:8080"  # Instead of 8888
```

**3. Permission issues**
```bash
# Ensure Docker has sufficient permissions
sudo chmod 666 /var/run/docker.sock

# Or run with sudo
sudo ./deploy-docker-hub.sh deploy
```

**4. Database connection failures**
```bash
# Reset SQL Server container
docker-compose -f docker-compose.hub.yml restart sqlserver

# Check database logs
docker logs r3e-webgui-sqlserver
```

### Reset Everything

```bash
# Complete cleanup and fresh start
./deploy-docker-hub.sh cleanup
docker system prune -f
./deploy-docker-hub.sh deploy
```

## 🔒 Production Deployment

### Security Considerations

1. **Change default passwords**:
```bash
DB_PASSWORD=YourSecurePassword123! ./deploy-docker-hub.sh deploy
```

2. **Use proper domain**:
```bash
BASE_DOMAIN=yourdomain.com ./deploy-docker-hub.sh deploy
```

3. **SSL/TLS Setup**:
   - Configure SSL certificates in `nginx.conf`
   - Use Let's Encrypt for automatic certificates
   - Update firewall rules

4. **Database Security**:
   - Use strong SA password
   - Consider external managed database
   - Regular backups

### Scaling

For production workloads:

```yaml
# docker-compose.hub.yml - Add resource limits
services:
  r3e-webgui-service:
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 4G
        reservations:
          cpus: '1.0'
          memory: 2G
```

## 📊 Performance

### Resource Requirements

| Component | CPU | Memory | Storage |
|-----------|-----|--------|---------|
| **WebGUI Service** | 1-2 cores | 2-4GB | 1GB |
| **SQL Server** | 2-4 cores | 4-8GB | 10-50GB |
| **Nginx** | 0.5 cores | 512MB | 100MB |
| **Total Recommended** | 4+ cores | 8GB+ | 20GB+ |

### Optimization

- Use SSD storage for database
- Configure SQL Server memory limits
- Enable gzip compression in Nginx
- Use Redis for caching (future enhancement)

## 🤝 Support

- **Documentation**: [GitHub Repository](https://github.com/neo-project/neo-devpack-dotnet)
- **Issues**: [GitHub Issues](https://github.com/neo-project/neo-devpack-dotnet/issues)
- **Docker Hub**: [r3enetwork/r3e-webgui-service](https://hub.docker.com/r/r3enetwork/r3e-webgui-service)

## 📄 License

This project is licensed under the MIT License.

---

**🎯 Deploy professional Neo smart contract interfaces in minutes, not weeks!**