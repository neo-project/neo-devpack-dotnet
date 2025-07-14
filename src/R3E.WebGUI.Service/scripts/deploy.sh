#!/bin/bash

# R3E WebGUI Service Deployment Script
set -e

echo "🚀 Starting R3E WebGUI Service deployment..."

# Configuration
ENVIRONMENT=${1:-production}
VERSION=${2:-latest}
SERVICE_NAME="r3e-webgui-service"
COMPOSE_FILE="docker-compose.yml"

if [ "$ENVIRONMENT" = "development" ]; then
    COMPOSE_FILE="docker-compose.dev.yml"
fi

echo "📋 Deployment Configuration:"
echo "   Environment: $ENVIRONMENT"
echo "   Version: $VERSION"
echo "   Compose File: $COMPOSE_FILE"
echo

# Check prerequisites
echo "🔍 Checking prerequisites..."

if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Check if we're in the right directory
if [ ! -f "$COMPOSE_FILE" ]; then
    echo "❌ $COMPOSE_FILE not found. Please run this script from the R3E.WebGUI.Service directory."
    exit 1
fi

echo "✅ Prerequisites check passed"
echo

# Create necessary directories
echo "📁 Creating directories..."
mkdir -p ./data/sqlserver
mkdir -p ./data/webgui-storage
mkdir -p ./logs
mkdir -p ./ssl
echo "✅ Directories created"
echo

# Generate SSL certificates if they don't exist (for development)
if [ "$ENVIRONMENT" = "development" ] && [ ! -f "./ssl/server.crt" ]; then
    echo "🔐 Generating self-signed SSL certificates for development..."
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout ./ssl/server.key \
        -out ./ssl/server.crt \
        -subj "/C=US/ST=State/L=City/O=R3E/CN=*.r3e-gui.local"
    echo "✅ SSL certificates generated"
    echo
fi

# Pull latest images
echo "📥 Pulling latest Docker images..."
docker-compose -f $COMPOSE_FILE pull
echo "✅ Images pulled"
echo

# Stop existing services
echo "🛑 Stopping existing services..."
docker-compose -f $COMPOSE_FILE down --remove-orphans
echo "✅ Services stopped"
echo

# Build and start services
echo "🏗️ Building and starting services..."
docker-compose -f $COMPOSE_FILE up -d --build

# Wait for services to be healthy
echo "⏳ Waiting for services to be healthy..."
sleep 30

# Check service health
echo "🏥 Checking service health..."

for service in r3e-webgui-service sqlserver; do
    echo "   Checking $service..."
    
    max_attempts=30
    attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        if docker-compose -f $COMPOSE_FILE ps $service | grep -q "healthy\|Up"; then
            echo "   ✅ $service is healthy"
            break
        fi
        
        if [ $attempt -eq $max_attempts ]; then
            echo "   ❌ $service failed to become healthy"
            echo "   📋 Service logs:"
            docker-compose -f $COMPOSE_FILE logs --tail=20 $service
            exit 1
        fi
        
        echo "   ⏳ Waiting for $service (attempt $attempt/$max_attempts)..."
        sleep 5
        ((attempt++))
    done
done

# Test API endpoints
echo "🧪 Testing API endpoints..."

API_URL="http://localhost:8080"
if [ "$ENVIRONMENT" = "production" ]; then
    API_URL="https://api.r3e-gui.com"
fi

# Test health endpoint
if curl -f -s "$API_URL/health" > /dev/null; then
    echo "   ✅ Health endpoint is responding"
else
    echo "   ❌ Health endpoint is not responding"
    exit 1
fi

# Test API documentation
if curl -f -s "$API_URL/swagger/index.html" > /dev/null; then
    echo "   ✅ API documentation is accessible"
else
    echo "   ⚠️  API documentation might not be accessible (this is normal in production)"
fi

echo
echo "🎉 Deployment completed successfully!"
echo
echo "📋 Service Information:"
echo "   API Endpoint: $API_URL"
echo "   Health Check: $API_URL/health"
echo "   Documentation: $API_URL/swagger"
echo "   Database: SQL Server on port 1433"
echo
echo "📊 Service Status:"
docker-compose -f $COMPOSE_FILE ps

echo
echo "📝 Useful Commands:"
echo "   View logs: docker-compose -f $COMPOSE_FILE logs -f"
echo "   Stop services: docker-compose -f $COMPOSE_FILE down"
echo "   View service status: docker-compose -f $COMPOSE_FILE ps"
echo "   Access database: docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa"
echo

if [ "$ENVIRONMENT" = "development" ]; then
    echo "🔧 Development Environment Notes:"
    echo "   - Add '127.0.0.1 api.r3e-gui.local *.r3e-gui.local' to your /etc/hosts file"
    echo "   - API available at: http://localhost:8080"
    echo "   - Self-signed SSL certificates generated in ./ssl/"
    echo
fi

echo "✅ R3E WebGUI Service is now running!"