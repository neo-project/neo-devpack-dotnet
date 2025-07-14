#!/bin/bash

# Database migration script for R3E WebGUI Service

echo "🚀 Starting database migration for R3E WebGUI Service..."

# Wait for SQL Server to be ready
echo "⏳ Waiting for SQL Server to be ready..."
until docker exec r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P R3E_Strong_Pass_2024! -Q "SELECT 1" &>/dev/null
do
    echo "  SQL Server is not ready yet. Waiting..."
    sleep 5
done

echo "✅ SQL Server is ready!"

# Run migrations
echo "🔄 Running database migrations..."
docker exec r3e-webgui-service dotnet ef database update --no-build

if [ $? -eq 0 ]; then
    echo "✅ Database migrations completed successfully!"
else
    echo "❌ Database migration failed!"
    exit 1
fi