#!/bin/bash

# Complete startup script for R3E WebGUI Service

echo "🚀 Starting R3E WebGUI Service with Docker Compose..."

# Navigate to the service directory
cd "$(dirname "$0")/.." || exit 1

# Check if we should use dev mode
if [ "$1" == "dev" ]; then
    echo "📦 Starting in development mode..."
    COMPOSE_FILES="-f docker-compose.yml -f docker-compose.dev.yml"
else
    echo "📦 Starting in production mode..."
    COMPOSE_FILES="-f docker-compose.yml"
fi

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker-compose $COMPOSE_FILES down

# Build and start the services
echo "🔨 Building and starting services..."
docker-compose $COMPOSE_FILES up -d --build

# Wait for services to be healthy
echo "⏳ Waiting for services to be healthy..."
sleep 10

# Check SQL Server health
echo "🔍 Checking SQL Server health..."
if docker exec r3e-webgui-sqlserver sh -c "echo 'SELECT 1' | /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024! -C" &>/dev/null; then
    echo "✅ SQL Server is healthy!"
else
    echo "❌ SQL Server health check failed!"
    docker-compose $COMPOSE_FILES logs sqlserver
    exit 1
fi

# Run database migrations
echo "🔄 Applying database migrations..."
docker exec r3e-webgui-service dotnet ef database update --no-build || {
    echo "⚠️  Migrations failed, attempting to create database first..."
    docker exec r3e-webgui-sqlserver sh -c "echo 'CREATE DATABASE R3EWebGUI' | /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024! -C"
    sleep 2
    docker exec r3e-webgui-service dotnet ef database update --no-build
}

# Check service health
echo "🔍 Checking service health..."
sleep 5
if curl -f http://localhost:8888/health &>/dev/null; then
    echo "✅ R3E WebGUI Service is healthy!"
else
    echo "❌ Service health check failed!"
    docker-compose $COMPOSE_FILES logs r3e-webgui-service
    exit 1
fi

echo "🎉 R3E WebGUI Service is running!"
echo ""
echo "📌 Service URLs:"
echo "   - Web Interface: http://localhost:8888"
echo "   - Health Check: http://localhost:8888/health"
echo "   - API Documentation: http://localhost:8888/swagger"
echo ""
echo "📊 Monitoring:"
echo "   - View logs: docker-compose $COMPOSE_FILES logs -f"
echo "   - Stop services: docker-compose $COMPOSE_FILES down"
echo ""

# Follow logs if requested
if [ "$2" == "logs" ]; then
    echo "📜 Following service logs (Ctrl+C to exit)..."
    docker-compose $COMPOSE_FILES logs -f
fi