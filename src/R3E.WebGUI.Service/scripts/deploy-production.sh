#!/bin/bash

# R3E WebGUI Service Production Deployment Script
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

print_status $BLUE "🚀 R3E WebGUI Service - Production Deployment"
echo "=============================================="
echo

# Get domain from user
if [ -z "$DOMAIN" ]; then
    read -p "Enter your domain (e.g., yourdomain.com): " DOMAIN
    if [ -z "$DOMAIN" ]; then
        print_status $RED "❌ Domain is required"
        exit 1
    fi
fi

# Get email for Let's Encrypt
if [ -z "$EMAIL" ]; then
    read -p "Enter your email for SSL certificates: " EMAIL
    if [ -z "$EMAIL" ]; then
        print_status $RED "❌ Email is required for SSL certificates"
        exit 1
    fi
fi

# Get database password
if [ -z "$DB_PASSWORD" ]; then
    read -s -p "Enter database password (press Enter for default): " DB_PASSWORD
    echo
    if [ -z "$DB_PASSWORD" ]; then
        DB_PASSWORD="YourStrong@Passw0rd"
        print_status $YELLOW "⚠️  Using default database password. Change this for production!"
    fi
fi

# Export environment variables
export DOMAIN
export EMAIL
export DB_PASSWORD

print_status $BLUE "📋 Deployment Configuration:"
echo "   Domain: $DOMAIN"
echo "   Email: $EMAIL"
echo "   API URL: https://api.$DOMAIN"
echo "   Contract URLs: https://[contract-name].$DOMAIN"
echo

# Check prerequisites
print_status $BLUE "🔍 Checking prerequisites..."

if ! command -v docker &> /dev/null; then
    print_status $RED "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    print_status $RED "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

print_status $GREEN "✅ Prerequisites check passed"
echo

# Create necessary directories
print_status $BLUE "📁 Creating directories..."
mkdir -p ./ssl
mkdir -p ./webroot
mkdir -p ./data
echo

# Generate self-signed certificate for default server
print_status $BLUE "🔐 Generating default SSL certificate..."
if [ ! -f "./ssl/default.crt" ]; then
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout ./ssl/default.key \
        -out ./ssl/default.crt \
        -subj "/C=US/ST=State/L=City/O=R3E/CN=default"
    print_status $GREEN "✅ Default SSL certificate generated"
else
    print_status $GREEN "✅ Default SSL certificate already exists"
fi

# Update nginx configuration with actual domain
print_status $BLUE "⚙️  Configuring NGINX for domain $DOMAIN..."
sed "s/DOMAIN_PLACEHOLDER/$DOMAIN/g" nginx.production.conf > nginx.conf.tmp
mv nginx.conf.tmp nginx.production.conf
print_status $GREEN "✅ NGINX configuration updated"

# Stop any existing services
print_status $BLUE "🛑 Stopping existing services..."
docker-compose -f docker-compose.production.yml down --remove-orphans 2>/dev/null || true

# Build and start services (without SSL first)
print_status $BLUE "🏗️  Building and starting services..."
docker-compose -f docker-compose.production.yml up -d --build r3e-webgui-service sqlserver nginx

# Wait for services to be ready
print_status $BLUE "⏳ Waiting for services to start..."
sleep 30

# Check service health
print_status $BLUE "🏥 Checking service health..."
max_attempts=30
attempt=1

while [ $attempt -le $max_attempts ]; do
    if docker-compose -f docker-compose.production.yml ps | grep -q "Up.*healthy"; then
        print_status $GREEN "✅ Services are healthy"
        break
    fi
    
    if [ $attempt -eq $max_attempts ]; then
        print_status $RED "❌ Services failed to become healthy"
        print_status $BLUE "📋 Service logs:"
        docker-compose -f docker-compose.production.yml logs --tail=20
        exit 1
    fi
    
    print_status $BLUE "   ⏳ Waiting for services (attempt $attempt/$max_attempts)..."
    sleep 5
    ((attempt++))
done

# Test basic connectivity
print_status $BLUE "🧪 Testing basic connectivity..."
if curl -f -s "http://localhost:8080/health" > /dev/null; then
    print_status $GREEN "✅ API health check passed"
else
    print_status $RED "❌ API health check failed"
    exit 1
fi

# Generate SSL certificates with Let's Encrypt
print_status $BLUE "🔒 Generating SSL certificates with Let's Encrypt..."

# Create a temporary HTTP server for domain verification
print_status $BLUE "   Setting up temporary HTTP server for domain verification..."

# Run certbot for initial certificate generation
if docker-compose -f docker-compose.production.yml run --rm certbot; then
    print_status $GREEN "✅ SSL certificates generated successfully"
else
    print_status $YELLOW "⚠️  SSL certificate generation failed. Continuing with HTTP only."
    print_status $YELLOW "   You can set up SSL certificates manually later."
fi

# Restart nginx with SSL configuration
print_status $BLUE "🔄 Restarting NGINX with SSL configuration..."
docker-compose -f docker-compose.production.yml restart nginx

# Final health check
print_status $BLUE "🏥 Final health check..."
sleep 10

if curl -f -s "http://localhost:80" > /dev/null; then
    print_status $GREEN "✅ HTTP endpoint is responding"
else
    print_status $YELLOW "⚠️  HTTP endpoint check failed"
fi

# Test HTTPS if certificates exist
if [ -d "./ssl" ] && [ "$(ls -A ./ssl 2>/dev/null)" ]; then
    if curl -f -k -s "https://localhost:443" > /dev/null; then
        print_status $GREEN "✅ HTTPS endpoint is responding"
    else
        print_status $YELLOW "⚠️  HTTPS endpoint check failed"
    fi
fi

echo
print_status $GREEN "🎉 Deployment completed successfully!"
echo
print_status $BLUE "📋 Service Information:"
echo "   Domain: $DOMAIN"
echo "   API Endpoint: https://api.$DOMAIN (or http://api.$DOMAIN)"
echo "   Health Check: https://api.$DOMAIN/health"
echo "   Documentation: https://api.$DOMAIN/swagger"
echo "   Contract URLs: https://[contract-name].$DOMAIN"
echo
print_status $BLUE "📊 Service Status:"
docker-compose -f docker-compose.production.yml ps

echo
print_status $BLUE "📝 Important Next Steps:"
echo
echo "1. 🌐 DNS Configuration:"
echo "   Add these DNS records to your domain:"
echo "   A     $DOMAIN              → YOUR_SERVER_IP"
echo "   A     api.$DOMAIN          → YOUR_SERVER_IP"
echo "   A     *.$DOMAIN            → YOUR_SERVER_IP"
echo
echo "2. 🔐 SSL Certificates:"
if [ -d "/etc/letsencrypt/live/$DOMAIN" ]; then
    echo "   ✅ SSL certificates are configured and working"
else
    echo "   ⚠️  SSL certificates need manual setup:"
    echo "   Run: docker-compose -f docker-compose.production.yml run --rm certbot"
fi
echo
echo "3. 🧪 Test the service:"
echo "   curl https://api.$DOMAIN/health"
echo "   curl -X POST https://api.$DOMAIN/api/webgui/deploy [with form data]"
echo
echo "4. 🔒 Security:"
echo "   - Change the default database password"
echo "   - Set up firewall rules (allow only 80, 443, 22)"
echo "   - Enable fail2ban or similar intrusion prevention"
echo "   - Set up monitoring and log aggregation"
echo
print_status $BLUE "📚 Documentation:"
echo "   - Full documentation: ./DEPLOYMENT_GUIDE.md"
echo "   - API documentation: https://api.$DOMAIN/swagger"
echo "   - Production checklist: ./PRODUCTION_CHECKLIST.md"
echo

print_status $GREEN "✅ Your R3E WebGUI Service is now running!"
print_status $BLUE "📱 Example usage:"
echo "   Deploy a contract WebGUI → Access at https://mycontract.$DOMAIN"
echo

print_status $BLUE "📈 Monitor your service:"
echo "   docker-compose -f docker-compose.production.yml logs -f"
echo "   docker-compose -f docker-compose.production.yml ps"