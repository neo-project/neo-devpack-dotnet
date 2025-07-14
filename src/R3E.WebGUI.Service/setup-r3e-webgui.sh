#!/bin/bash

# ============================================================================
# R3E WebGUI Service - Complete Server Setup Script
# 
# This script will download and setup the complete R3E WebGUI Service
# from Docker Hub on your server with all dependencies and configurations.
#
# Usage: curl -sSL https://raw.githubusercontent.com/neo-project/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service/setup-r3e-webgui.sh | bash
# ============================================================================

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configuration
INSTALL_DIR="/opt/r3e-webgui"
GITHUB_RAW_URL="https://raw.githubusercontent.com/neo-project/neo-devpack-dotnet/r3e/src/R3E.WebGUI.Service"
DOCKER_HUB_IMAGE="r3enetwork/r3e-webgui-service:latest"

# Default configuration values
DEFAULT_BASE_DOMAIN="localhost"
DEFAULT_DB_PASSWORD="R3E_Strong_Pass_2024!"
DEFAULT_ADMIN_EMAIL="admin@localhost"

# Functions
print_header() {
    echo -e "${BLUE}===================================================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}===================================================================${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

print_step() {
    echo -e "${PURPLE}🔄 $1${NC}"
}

# Detect OS
detect_os() {
    if [ -f /etc/os-release ]; then
        . /etc/os-release
        OS=$NAME
        VER=$VERSION_ID
    elif [ -f /etc/redhat-release ]; then
        OS="Red Hat"
        VER=$(rpm -q --qf "%{VERSION}" $(rpm -qa '(redhat|centos|fedora)*release*'))
    else
        OS=$(uname -s)
        VER=$(uname -r)
    fi
    
    print_info "Detected OS: $OS $VER"
}

# Check if running as root
check_root() {
    if [ "$EUID" -ne 0 ]; then 
        print_error "This script must be run as root or with sudo"
        exit 1
    fi
}

# Install Docker
install_docker() {
    print_header "Installing Docker"
    
    if command -v docker >/dev/null 2>&1; then
        print_info "Docker is already installed"
        docker --version
        return
    fi
    
    print_step "Installing Docker..."
    
    # Install Docker based on OS
    if [[ "$OS" == "Ubuntu" ]] || [[ "$OS" == "Debian"* ]]; then
        apt-get update
        apt-get install -y \
            ca-certificates \
            curl \
            gnupg \
            lsb-release
        
        # Add Docker's official GPG key
        mkdir -m 0755 -p /etc/apt/keyrings
        curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
        
        # Set up repository
        echo \
          "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
          $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
        
        # Install Docker Engine
        apt-get update
        apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
        
    elif [[ "$OS" == "CentOS"* ]] || [[ "$OS" == "Red Hat"* ]] || [[ "$OS" == "Fedora"* ]]; then
        yum install -y yum-utils
        yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
        yum install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
        
    elif [[ "$OS" == "Amazon Linux"* ]]; then
        yum update -y
        amazon-linux-extras install docker -y
        
    else
        print_error "Unsupported OS for automatic Docker installation"
        print_info "Please install Docker manually and run this script again"
        exit 1
    fi
    
    # Start and enable Docker
    systemctl start docker
    systemctl enable docker
    
    print_success "Docker installed successfully"
}

# Install Docker Compose
install_docker_compose() {
    print_header "Installing Docker Compose"
    
    if command -v docker-compose >/dev/null 2>&1 || docker compose version >/dev/null 2>&1; then
        print_info "Docker Compose is already installed"
        return
    fi
    
    print_step "Installing Docker Compose standalone..."
    
    # Install standalone docker-compose
    COMPOSE_VERSION=$(curl -s https://api.github.com/repos/docker/compose/releases/latest | grep -oP '"tag_name": "\K(.*)(?=")')
    curl -L "https://github.com/docker/compose/releases/download/${COMPOSE_VERSION}/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    chmod +x /usr/local/bin/docker-compose
    
    print_success "Docker Compose installed successfully"
}

# Install additional utilities
install_utilities() {
    print_header "Installing Required Utilities"
    
    print_step "Installing system utilities..."
    
    if [[ "$OS" == "Ubuntu" ]] || [[ "$OS" == "Debian"* ]]; then
        apt-get update
        apt-get install -y \
            curl \
            wget \
            git \
            jq \
            htop \
            net-tools \
            vim \
            ufw
    elif [[ "$OS" == "CentOS"* ]] || [[ "$OS" == "Red Hat"* ]] || [[ "$OS" == "Fedora"* ]]; then
        yum install -y \
            curl \
            wget \
            git \
            jq \
            htop \
            net-tools \
            vim \
            firewalld
    fi
    
    print_success "Utilities installed"
}

# Configure firewall
configure_firewall() {
    print_header "Configuring Firewall"
    
    print_step "Setting up firewall rules..."
    
    if [[ "$OS" == "Ubuntu" ]] || [[ "$OS" == "Debian"* ]]; then
        # UFW firewall
        if command -v ufw >/dev/null 2>&1; then
            ufw allow 22/tcp    # SSH
            ufw allow 80/tcp    # HTTP
            ufw allow 443/tcp   # HTTPS
            ufw allow 8888/tcp  # WebGUI Service
            ufw allow 1433/tcp  # SQL Server (consider restricting this)
            ufw --force enable
            print_success "UFW firewall configured"
        fi
    elif [[ "$OS" == "CentOS"* ]] || [[ "$OS" == "Red Hat"* ]] || [[ "$OS" == "Fedora"* ]]; then
        # Firewalld
        if command -v firewall-cmd >/dev/null 2>&1; then
            systemctl start firewalld
            systemctl enable firewalld
            firewall-cmd --permanent --add-port=22/tcp
            firewall-cmd --permanent --add-port=80/tcp
            firewall-cmd --permanent --add-port=443/tcp
            firewall-cmd --permanent --add-port=8888/tcp
            firewall-cmd --permanent --add-port=1433/tcp
            firewall-cmd --reload
            print_success "Firewalld configured"
        fi
    fi
}

# Create installation directory
create_directories() {
    print_header "Creating Installation Directories"
    
    print_step "Creating directory structure..."
    
    mkdir -p $INSTALL_DIR/{configs,ssl,data,logs,scripts,backups}
    
    print_success "Directories created at $INSTALL_DIR"
}

# Download configuration files
download_configs() {
    print_header "Downloading Configuration Files"
    
    cd $INSTALL_DIR
    
    print_step "Downloading docker-compose.yml..."
    curl -sSL "$GITHUB_RAW_URL/docker-compose.hub.yml" -o configs/docker-compose.yml
    
    print_step "Downloading nginx configuration..."
    curl -sSL "$GITHUB_RAW_URL/nginx.conf" -o configs/nginx.conf
    
    print_step "Downloading production compose file..."
    curl -sSL "$GITHUB_RAW_URL/docker-compose.production.hub.yml" -o configs/docker-compose.production.yml
    
    print_step "Downloading deployment script..."
    curl -sSL "$GITHUB_RAW_URL/deploy-docker-hub.sh" -o scripts/deploy.sh
    chmod +x scripts/deploy.sh
    
    print_step "Downloading environment template..."
    curl -sSL "$GITHUB_RAW_URL/.env.production.template" -o configs/.env.template
    
    print_success "Configuration files downloaded"
}

# Configure environment
configure_environment() {
    print_header "Configuring Environment"
    
    cd $INSTALL_DIR
    
    # Collect configuration from user
    echo ""
    read -p "Enter your domain name (default: $DEFAULT_BASE_DOMAIN): " BASE_DOMAIN
    BASE_DOMAIN=${BASE_DOMAIN:-$DEFAULT_BASE_DOMAIN}
    
    read -p "Enter admin email for SSL certificates (default: $DEFAULT_ADMIN_EMAIL): " ADMIN_EMAIL
    ADMIN_EMAIL=${ADMIN_EMAIL:-$DEFAULT_ADMIN_EMAIL}
    
    read -sp "Enter database password (default: $DEFAULT_DB_PASSWORD): " DB_PASSWORD
    echo ""
    DB_PASSWORD=${DB_PASSWORD:-$DEFAULT_DB_PASSWORD}
    
    # Create .env file
    print_step "Creating environment configuration..."
    cat > configs/.env << EOF
# R3E WebGUI Service Configuration
# Generated on $(date)

# Domain Configuration
BASE_DOMAIN=$BASE_DOMAIN
ADMIN_EMAIL=$ADMIN_EMAIL

# Database Configuration
DB_PASSWORD=$DB_PASSWORD
SQL_PORT=1433

# Service Configuration
REQUIRE_HTTPS=false
CONTAINER_NAME_PREFIX=r3e-webgui

# Neo RPC Endpoints
NEO_RPC_TESTNET=https://test1.neo.coz.io:443
NEO_RPC_MAINNET=https://mainnet1.neo.coz.io:443

# Rate Limiting
RATE_LIMIT_PER_MINUTE=200
RATE_LIMIT_PER_HOUR=10000

# File Upload Limits
MAX_FILE_SIZE_KB=10240
MAX_TOTAL_SIZE_KB=102400
EOF
    
    print_success "Environment configured"
}

# Create SSL certificates
setup_ssl() {
    print_header "Setting Up SSL Certificates"
    
    cd $INSTALL_DIR
    
    if [ "$BASE_DOMAIN" != "localhost" ]; then
        print_step "Generating self-signed certificates for initial setup..."
        
        # Create self-signed certificate for initial testing
        openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
            -keyout ssl/privkey.pem \
            -out ssl/fullchain.pem \
            -subj "/C=US/ST=State/L=City/O=Organization/CN=$BASE_DOMAIN"
        
        print_info "Self-signed certificates created"
        print_info "To use Let's Encrypt, run: $INSTALL_DIR/scripts/setup-letsencrypt.sh"
    else
        print_info "Using localhost - SSL not required"
    fi
}

# Create systemd service
create_systemd_service() {
    print_header "Creating Systemd Service"
    
    print_step "Creating systemd service file..."
    
    cat > /etc/systemd/system/r3e-webgui.service << EOF
[Unit]
Description=R3E WebGUI Service
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=$INSTALL_DIR/configs
ExecStart=/usr/local/bin/docker-compose -f docker-compose.yml up -d
ExecStop=/usr/local/bin/docker-compose -f docker-compose.yml down
ExecReload=/usr/local/bin/docker-compose -f docker-compose.yml restart

[Install]
WantedBy=multi-user.target
EOF
    
    systemctl daemon-reload
    systemctl enable r3e-webgui.service
    
    print_success "Systemd service created"
}

# Create management scripts
create_management_scripts() {
    print_header "Creating Management Scripts"
    
    cd $INSTALL_DIR/scripts
    
    # Start script
    cat > start.sh << 'EOF'
#!/bin/bash
cd /opt/r3e-webgui/configs
docker-compose up -d
echo "R3E WebGUI Service started"
EOF
    
    # Stop script
    cat > stop.sh << 'EOF'
#!/bin/bash
cd /opt/r3e-webgui/configs
docker-compose down
echo "R3E WebGUI Service stopped"
EOF
    
    # Restart script
    cat > restart.sh << 'EOF'
#!/bin/bash
cd /opt/r3e-webgui/configs
docker-compose restart
echo "R3E WebGUI Service restarted"
EOF
    
    # Status script
    cat > status.sh << 'EOF'
#!/bin/bash
echo "=== R3E WebGUI Service Status ==="
docker ps --filter "name=r3e-webgui" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
echo ""
echo "=== Health Check ==="
curl -s http://localhost:8888/health || echo "Service not healthy"
echo ""
EOF
    
    # Logs script
    cat > logs.sh << 'EOF'
#!/bin/bash
cd /opt/r3e-webgui/configs
docker-compose logs -f --tail=100
EOF
    
    # Backup script
    cat > backup.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/opt/r3e-webgui/backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

echo "Starting backup..."

# Backup database
docker exec r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P $DB_PASSWORD -C \
    -Q "BACKUP DATABASE [R3EWebGUI] TO DISK = N'/var/opt/mssql/backup/R3EWebGUI_$TIMESTAMP.bak' WITH FORMAT, INIT, COMPRESSION"

# Copy backup from container
docker cp r3e-webgui-sqlserver:/var/opt/mssql/backup/R3EWebGUI_$TIMESTAMP.bak $BACKUP_DIR/

# Backup volumes
docker run --rm -v r3ewebguiservice_webgui-storage:/data -v $BACKUP_DIR:/backup \
    alpine tar czf /backup/webgui-storage_$TIMESTAMP.tar.gz -C /data .

echo "Backup completed: $BACKUP_DIR/*_$TIMESTAMP.*"
EOF
    
    # Update script
    cat > update.sh << 'EOF'
#!/bin/bash
echo "Updating R3E WebGUI Service..."
cd /opt/r3e-webgui/configs

# Pull latest images
docker-compose pull

# Restart services
docker-compose up -d

echo "Update completed"
EOF
    
    # Let's Encrypt setup script
    cat > setup-letsencrypt.sh << 'EOF'
#!/bin/bash
source /opt/r3e-webgui/configs/.env

echo "Setting up Let's Encrypt SSL certificates..."

# Install certbot
if ! command -v certbot >/dev/null 2>&1; then
    if [ -f /etc/debian_version ]; then
        apt-get update && apt-get install -y certbot
    elif [ -f /etc/redhat-release ]; then
        yum install -y certbot
    fi
fi

# Stop nginx to free port 80
docker stop r3e-webgui-nginx

# Get certificates
certbot certonly --standalone -d $BASE_DOMAIN -d *.$BASE_DOMAIN \
    --non-interactive --agree-tos --email $ADMIN_EMAIL

# Copy certificates
cp /etc/letsencrypt/live/$BASE_DOMAIN/fullchain.pem /opt/r3e-webgui/ssl/
cp /etc/letsencrypt/live/$BASE_DOMAIN/privkey.pem /opt/r3e-webgui/ssl/

# Start nginx
docker start r3e-webgui-nginx

echo "SSL certificates installed"
EOF
    
    # Make all scripts executable
    chmod +x *.sh
    
    print_success "Management scripts created in $INSTALL_DIR/scripts"
}

# Deploy services
deploy_services() {
    print_header "Deploying R3E WebGUI Services"
    
    cd $INSTALL_DIR/configs
    
    print_step "Pulling Docker images..."
    docker pull $DOCKER_HUB_IMAGE
    docker pull mcr.microsoft.com/mssql/server:2022-latest
    docker pull nginx:alpine
    
    print_step "Starting services..."
    docker-compose up -d
    
    print_info "Waiting for services to start (this may take 60-120 seconds)..."
    
    # Wait for SQL Server
    for i in {1..120}; do
        if docker exec r3e-webgui-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$DB_PASSWORD" -Q "SELECT 1" -C >/dev/null 2>&1; then
            print_success "SQL Server is ready"
            break
        fi
        if [ $i -eq 120 ]; then
            print_error "SQL Server failed to start"
            exit 1
        fi
        sleep 1
    done
    
    # Wait for WebGUI Service
    for i in {1..60}; do
        if curl -f http://localhost:8888/health >/dev/null 2>&1; then
            print_success "R3E WebGUI Service is ready"
            break
        fi
        if [ $i -eq 60 ]; then
            print_error "R3E WebGUI Service failed to start"
            exit 1
        fi
        sleep 1
    done
    
    print_success "All services deployed successfully"
}

# Create cron jobs
setup_cron() {
    print_header "Setting Up Automated Tasks"
    
    print_step "Creating cron jobs..."
    
    # Create cron job for backups
    (crontab -l 2>/dev/null; echo "0 2 * * * $INSTALL_DIR/scripts/backup.sh >> $INSTALL_DIR/logs/backup.log 2>&1") | crontab -
    
    # Create cron job for Let's Encrypt renewal
    if [ "$BASE_DOMAIN" != "localhost" ]; then
        (crontab -l 2>/dev/null; echo "0 0 * * 0 certbot renew --quiet && docker restart r3e-webgui-nginx") | crontab -
    fi
    
    print_success "Cron jobs configured"
}

# Display final information
show_completion_info() {
    print_header "🎉 Installation Complete! 🎉"
    
    echo -e "${CYAN}R3E WebGUI Service has been successfully installed!${NC}"
    echo ""
    echo -e "${GREEN}Installation Details:${NC}"
    echo "• Installation Directory: $INSTALL_DIR"
    echo "• Configuration: $INSTALL_DIR/configs/.env"
    echo "• Logs: $INSTALL_DIR/logs/"
    echo "• Backups: $INSTALL_DIR/backups/"
    echo ""
    echo -e "${GREEN}Service URLs:${NC}"
    echo "• Health Check: http://$BASE_DOMAIN:8888/health"
    echo "• API Endpoint: http://$BASE_DOMAIN:8888/api"
    echo "• Contract WebGUIs: http://CONTRACT.$BASE_DOMAIN:8888"
    echo ""
    echo -e "${GREEN}Database Access:${NC}"
    echo "• Host: localhost:1433"
    echo "• Username: sa"
    echo "• Password: [configured in .env]"
    echo ""
    echo -e "${GREEN}Management Commands:${NC}"
    echo "• Start:   $INSTALL_DIR/scripts/start.sh"
    echo "• Stop:    $INSTALL_DIR/scripts/stop.sh"
    echo "• Restart: $INSTALL_DIR/scripts/restart.sh"
    echo "• Status:  $INSTALL_DIR/scripts/status.sh"
    echo "• Logs:    $INSTALL_DIR/scripts/logs.sh"
    echo "• Backup:  $INSTALL_DIR/scripts/backup.sh"
    echo "• Update:  $INSTALL_DIR/scripts/update.sh"
    echo ""
    echo -e "${GREEN}Systemd Service:${NC}"
    echo "• systemctl start r3e-webgui"
    echo "• systemctl stop r3e-webgui"
    echo "• systemctl restart r3e-webgui"
    echo "• systemctl status r3e-webgui"
    echo ""
    
    if [ "$BASE_DOMAIN" != "localhost" ]; then
        echo -e "${YELLOW}SSL Setup:${NC}"
        echo "To enable Let's Encrypt SSL certificates, run:"
        echo "  $INSTALL_DIR/scripts/setup-letsencrypt.sh"
        echo ""
    fi
    
    echo -e "${BLUE}Documentation:${NC}"
    echo "• GitHub: https://github.com/neo-project/neo-devpack-dotnet"
    echo "• Docker Hub: https://hub.docker.com/r/r3enetwork/r3e-webgui-service"
    echo ""
    
    echo -e "${GREEN}Next Steps:${NC}"
    echo "1. Check service status: $INSTALL_DIR/scripts/status.sh"
    echo "2. View logs: $INSTALL_DIR/scripts/logs.sh"
    echo "3. Configure your domain DNS if not using localhost"
    echo "4. Deploy your first contract WebGUI using the API"
    echo ""
    
    print_success "R3E WebGUI Service is ready to use!"
}

# Main installation flow
main() {
    print_header "R3E WebGUI Service - Complete Server Setup"
    echo -e "${CYAN}This script will install and configure the complete R3E WebGUI Service${NC}"
    echo -e "${CYAN}from Docker Hub on your server.${NC}"
    echo ""
    
    # Check prerequisites
    check_root
    detect_os
    
    # Confirm installation
    read -p "Continue with installation? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Installation cancelled"
        exit 0
    fi
    
    # Run installation steps
    install_docker
    install_docker_compose
    install_utilities
    configure_firewall
    create_directories
    download_configs
    configure_environment
    setup_ssl
    create_systemd_service
    create_management_scripts
    deploy_services
    setup_cron
    show_completion_info
}

# Run main function
main "$@"