# R3E WebGUI Service - Server Installation Guide

## 🚀 One-Line Installation

The easiest way to install the complete R3E WebGUI Service on your server:

```bash
curl -sSL https://raw.githubusercontent.com/r3e-network/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/quick-install.sh | sudo bash
```

This command will:
- ✅ Install Docker and Docker Compose
- ✅ Configure firewall rules
- ✅ Download all necessary files
- ✅ Set up the complete service stack
- ✅ Create management scripts
- ✅ Configure automatic backups
- ✅ Start all services

## 📋 Prerequisites

### System Requirements
- **OS**: Ubuntu 20.04+, Debian 11+, CentOS 8+, RHEL 8+, Amazon Linux 2
- **RAM**: Minimum 4GB (8GB recommended)
- **Storage**: Minimum 20GB free space
- **CPU**: 2+ cores (4+ recommended)
- **Network**: Public IP with ports 80, 443, 8888 available

### Required Ports
| Port | Service | Description |
|------|---------|-------------|
| 22 | SSH | Remote access (configure as needed) |
| 80 | HTTP | Web traffic (redirects to HTTPS) |
| 443 | HTTPS | Secure web traffic |
| 8888 | API | R3E WebGUI Service API |
| 1433 | SQL | Database (restrict in production) |

## 🔧 Manual Installation

If you prefer to install manually or customize the process:

### 1. Download Setup Script

```bash
# Download the setup script
wget https://raw.githubusercontent.com/r3e-network/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/setup-r3e-webgui.sh

# Make it executable
chmod +x setup-r3e-webgui.sh

# Run with sudo
sudo ./setup-r3e-webgui.sh
```

### 2. Interactive Configuration

The script will prompt you for:
- **Domain name**: Your server's domain (or use localhost)
- **Admin email**: For SSL certificates
- **Database password**: Secure password for SQL Server

### 3. Installation Process

The script automatically:
1. Detects your operating system
2. Installs Docker and Docker Compose
3. Configures firewall rules
4. Creates directory structure in `/opt/r3e-webgui`
5. Downloads configuration files
6. Sets up environment variables
7. Creates SSL certificates (self-signed initially)
8. Configures systemd service
9. Deploys all containers
10. Sets up automated backups

## 📁 Directory Structure

After installation, your server will have:

```
/opt/r3e-webgui/
├── configs/           # Configuration files
│   ├── docker-compose.yml
│   ├── nginx.conf
│   └── .env          # Environment variables
├── ssl/              # SSL certificates
├── data/             # Application data
├── logs/             # Service logs
├── backups/          # Automated backups
└── scripts/          # Management scripts
    ├── start.sh      # Start services
    ├── stop.sh       # Stop services
    ├── restart.sh    # Restart services
    ├── status.sh     # Check status
    ├── logs.sh       # View logs
    ├── backup.sh     # Manual backup
    └── update.sh     # Update services
```

## 🎮 Service Management

### Using Systemd

```bash
# Start service
sudo systemctl start r3e-webgui

# Stop service
sudo systemctl stop r3e-webgui

# Restart service
sudo systemctl restart r3e-webgui

# Check status
sudo systemctl status r3e-webgui

# Enable auto-start on boot
sudo systemctl enable r3e-webgui
```

### Using Management Scripts

```bash
# Check service status
/opt/r3e-webgui/scripts/status.sh

# View logs
/opt/r3e-webgui/scripts/logs.sh

# Restart services
/opt/r3e-webgui/scripts/restart.sh

# Create manual backup
/opt/r3e-webgui/scripts/backup.sh

# Update to latest version
/opt/r3e-webgui/scripts/update.sh
```

## 🔒 SSL/TLS Configuration

### Self-Signed Certificates (Default)

The installer creates self-signed certificates for initial testing.

### Let's Encrypt (Production)

For production deployments with a real domain:

```bash
# Run Let's Encrypt setup
/opt/r3e-webgui/scripts/setup-letsencrypt.sh
```

This will:
- Install certbot
- Request certificates for your domain
- Configure automatic renewal
- Restart nginx with new certificates

### Custom Certificates

To use your own certificates:

```bash
# Copy your certificates
sudo cp /path/to/fullchain.pem /opt/r3e-webgui/ssl/
sudo cp /path/to/privkey.pem /opt/r3e-webgui/ssl/

# Restart nginx
docker restart r3e-webgui-nginx
```

## 🌐 DNS Configuration

### Basic Setup

Point your domain to your server's IP:
- A record: `yourdomain.com` → `YOUR_SERVER_IP`
- A record: `*.yourdomain.com` → `YOUR_SERVER_IP`

### Example with Cloudflare

1. Add A record for `@` pointing to your server IP
2. Add A record for `*` pointing to your server IP
3. Enable "Proxy" for DDoS protection
4. Set SSL/TLS to "Full" or "Full (strict)"

## 🔐 Security Hardening

### 1. Change Default Passwords

```bash
# Edit configuration
sudo nano /opt/r3e-webgui/configs/.env

# Update DB_PASSWORD
# Restart services
sudo /opt/r3e-webgui/scripts/restart.sh
```

### 2. Restrict Database Access

```bash
# Only allow local connections
sudo ufw delete allow 1433/tcp
```

### 3. Enable HTTPS Only

```bash
# Edit .env file
sudo nano /opt/r3e-webgui/configs/.env

# Set REQUIRE_HTTPS=true
# Restart services
```

### 4. Configure Firewall

```bash
# Ubuntu/Debian
sudo ufw status
sudo ufw allow from YOUR_IP to any port 22
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 8888/tcp
sudo ufw enable

# CentOS/RHEL
sudo firewall-cmd --list-all
sudo firewall-cmd --permanent --add-service=http
sudo firewall-cmd --permanent --add-service=https
sudo firewall-cmd --permanent --add-port=8888/tcp
sudo firewall-cmd --reload
```

## 📊 Monitoring

### Check Service Health

```bash
# Health endpoint
curl http://localhost:8888/health

# Container status
docker ps --filter "name=r3e-webgui"

# Resource usage
docker stats --no-stream
```

### View Logs

```bash
# All services
/opt/r3e-webgui/scripts/logs.sh

# Specific service
docker logs r3e-webgui-service
docker logs r3e-webgui-sqlserver
docker logs r3e-webgui-nginx
```

### Database Queries

```bash
# Connect to SQL Server
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P YOUR_PASSWORD -C

# Example queries
SELECT COUNT(*) FROM WebGUIs;
GO
```

## 🔄 Backup & Recovery

### Automated Backups

Backups run daily at 2 AM via cron:
- Database backup to `.bak` file
- Volume data to compressed archives
- Stored in `/opt/r3e-webgui/backups/`

### Manual Backup

```bash
/opt/r3e-webgui/scripts/backup.sh
```

### Restore from Backup

```bash
# Stop services
/opt/r3e-webgui/scripts/stop.sh

# Restore database
docker exec -i r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P YOUR_PASSWORD -C \
    -Q "RESTORE DATABASE [R3EWebGUI] FROM DISK = N'/backup/R3EWebGUI_20231215_020000.bak' WITH REPLACE"

# Restore volumes
docker run --rm -v r3ewebguiservice_webgui-storage:/data \
    -v /opt/r3e-webgui/backups:/backup \
    alpine tar xzf /backup/webgui-storage_20231215_020000.tar.gz -C /data

# Start services
/opt/r3e-webgui/scripts/start.sh
```

## 🚀 Scaling for Production

### Resource Allocation

Edit `/opt/r3e-webgui/configs/docker-compose.yml`:

```yaml
services:
  r3e-webgui-service:
    deploy:
      resources:
        limits:
          cpus: '4.0'
          memory: 8G
        reservations:
          cpus: '2.0'
          memory: 4G
```

### Multiple Instances

For high availability, deploy multiple instances behind a load balancer:

```bash
# Instance 1
CONTAINER_NAME_PREFIX=r3e-webgui-1 docker-compose up -d

# Instance 2  
CONTAINER_NAME_PREFIX=r3e-webgui-2 docker-compose up -d
```

### External Database

For production, consider using managed database services:
- Azure SQL Database
- AWS RDS SQL Server
- Google Cloud SQL

Update connection string in `.env`:
```
ConnectionStrings__DefaultConnection=Server=your-db-server.database.windows.net;Database=R3EWebGUI;User Id=yourusername;Password=yourpassword;
```

## 🆘 Troubleshooting

### Service Won't Start

```bash
# Check logs
docker logs r3e-webgui-service

# Check if ports are in use
sudo netstat -tulpn | grep -E "(80|443|8888|1433)"

# Restart Docker
sudo systemctl restart docker
```

### Database Connection Issues

```bash
# Test SQL Server connection
docker exec r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P YOUR_PASSWORD -Q "SELECT 1" -C

# Check SQL Server logs
docker logs r3e-webgui-sqlserver
```

### Reset Everything

```bash
# Stop services
sudo systemctl stop r3e-webgui

# Remove containers and volumes
cd /opt/r3e-webgui/configs
docker-compose down -v

# Start fresh
sudo systemctl start r3e-webgui
```

## 📞 Support

- **GitHub Issues**: [Report bugs or request features](https://github.com/r3e-network/neo-devpack-dotnet/issues)
- **Documentation**: [Full documentation](https://github.com/r3e-network/neo-devpack-dotnet/tree/r3e/src/R3E.WebGUI.Service)
- **Docker Hub**: [r3enetwork/r3e-webgui-service](https://hub.docker.com/r/r3enetwork/r3e-webgui-service)

## 🎯 Quick Start After Installation

1. **Check service status**:
   ```bash
   /opt/r3e-webgui/scripts/status.sh
   ```

2. **Deploy your first WebGUI**:
   ```bash
   curl -X POST http://YOUR_DOMAIN:8888/api/webgui/deploy-from-manifest \
     -H "Content-Type: application/json" \
     -d '{
       "contractAddress": "0x1234...abcd",
       "contractName": "MyContract",
       "network": "testnet",
       "deployerAddress": "NXXXxxxXXXxxxXXXxxxXXXxxxXXXxxxXXXxx",
       "timestamp": 1700000000,
       "signature": "signature_from_wallet"
     }'
   ```

3. **Access your WebGUI**:
   ```
   http://CONTRACT_ADDRESS.YOUR_DOMAIN:8888
   ```

---

**🎉 Your R3E WebGUI Service is ready to revolutionize Neo smart contract interfaces!**