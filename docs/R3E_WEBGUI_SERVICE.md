# R3E WebGUI Hosting Service - Complete Documentation

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Getting Started](#getting-started)
4. [API Reference](#api-reference)
5. [Development](#development)
6. [Testing](#testing)
7. [Deployment](#deployment)
8. [Security](#security)
9. [Monitoring](#monitoring)
10. [Troubleshooting](#troubleshooting)

## Overview

The R3E WebGUI Hosting Service is a production-ready microservice that provides hosting for Neo smart contract WebGUIs with automatic subdomain generation, contract search capabilities, and comprehensive management features.

### Key Features

- **WebGUI Hosting**: Upload and host interactive web interfaces for Neo smart contracts
- **Subdomain Management**: Automatic generation of unique subdomains (e.g., `mycontract.r3e-gui.com`)
- **Contract Search**: Find contracts by address across different networks
- **Rate Limiting**: Protect against abuse with configurable rate limits
- **File Validation**: Comprehensive validation of uploaded files and metadata
- **Analytics**: Track usage, view counts, and deployment statistics
- **Health Monitoring**: Built-in health checks and monitoring endpoints
- **Docker Support**: Complete containerization with Docker Compose
- **API Documentation**: Auto-generated Swagger/OpenAPI documentation

## Architecture

### System Components

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│     NGINX       │    │   R3E WebGUI    │    │   SQL Server    │
│  (Load Balancer │───▶│     Service     │───▶│   (Database)    │
│   & Subdomain   │    │      API        │    │                 │
│    Routing)     │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   File Storage  │    │    Logging &    │    │   Health Checks │
│  (WebGUI Files) │    │   Monitoring    │    │   & Metrics     │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Technology Stack

- **Backend**: ASP.NET Core 9.0
- **Database**: SQL Server with Entity Framework Core
- **Storage**: Local file system (configurable for cloud storage)
- **Load Balancer**: NGINX with subdomain routing
- **Containerization**: Docker and Docker Compose
- **Testing**: xUnit, Moq, FluentAssertions
- **Documentation**: Swagger/OpenAPI

### Data Flow

1. **WebGUI Deployment**:
   ```
   Developer → R3E Compiler → WebGUI Service API → File Storage + Database
   ```

2. **Subdomain Access**:
   ```
   User Browser → NGINX → WebGUI Service → File Storage → WebGUI Content
   ```

3. **Contract Search**:
   ```
   Client → API → Database → Search Results
   ```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker and Docker Compose
- SQL Server (or SQL Server Express)

### Quick Start

1. **Clone the repository**:
   ```bash
   git clone https://github.com/r3e-network/neo-devpack-dotnet.git
   cd neo-devpack-dotnet/src/R3E.WebGUI.Service
   ```

2. **Start with Docker Compose**:
   ```bash
   chmod +x scripts/deploy.sh
   ./scripts/deploy.sh development
   ```

3. **Access the service**:
   - API: http://localhost:8080
   - Documentation: http://localhost:8080/swagger
   - Health Check: http://localhost:8080/health

### Manual Setup

1. **Configure the database**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=R3EWebGUIService;Trusted_Connection=true"
     }
   }
   ```

2. **Run the service**:
   ```bash
   dotnet run
   ```

## API Reference

### Authentication

The API supports optional API key authentication. Include the API key in the header:

```
X-API-Key: your-api-key-here
```

### Endpoints

#### Deploy WebGUI

```http
POST /api/webgui/deploy
Content-Type: multipart/form-data

Form Data:
- contractAddress: string (required) - Neo contract address (0x...)
- contractName: string (required) - Name of the contract
- network: string (optional) - "testnet" or "mainnet" (default: "testnet")
- deployerAddress: string (required) - Deployer's Neo address
- description: string (optional) - Description of the WebGUI
- webGUIFiles: file[] (required) - WebGUI files to upload
```

**Response:**
```json
{
  "success": true,
  "subdomain": "mycontract",
  "url": "https://mycontract.r3e-gui.com",
  "contractAddress": "0x1234567890abcdef1234567890abcdef12345678"
}
```

#### Search Contracts

```http
GET /api/webgui/search?contractAddress={address}&network={network}
```

**Response:**
```json
[
  {
    "id": "guid",
    "contractAddress": "0x1234567890abcdef1234567890abcdef12345678",
    "contractName": "MyContract",
    "network": "testnet",
    "subdomain": "mycontract",
    "description": "Contract description",
    "deployedAt": "2024-01-01T00:00:00Z",
    "deployerAddress": "0xabcdef...",
    "viewCount": 42,
    "isActive": true
  }
]
```

#### List WebGUIs

```http
GET /api/webgui/list?page={page}&pageSize={pageSize}&network={network}
```

#### Get WebGUI Info

```http
GET /api/webgui/{subdomain}
```

#### Update WebGUI

```http
PUT /api/webgui/{subdomain}/update
Content-Type: multipart/form-data

Form Data:
- description: string (optional) - Updated description
- webGUIFiles: file[] (optional) - Updated files
```

### Rate Limits

| Endpoint | Limit |
|----------|-------|
| General API | 60 requests/minute, 1000 requests/hour |
| Deploy | 1 request/minute, 50 deployments/day |

Rate limit headers are included in responses:
- `X-RateLimit-Limit-Minute`
- `X-RateLimit-Remaining-Minute`
- `X-RateLimit-Reset-Minute`

### Error Handling

All errors follow a consistent format:

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "timestamp": "2024-01-01T00:00:00Z",
  "path": "/api/webgui/deploy",
  "details": [
    {
      "field": "contractAddress",
      "message": "Contract address must be in format 0x followed by 40 hexadecimal characters"
    }
  ]
}
```

## Development

### Project Structure

```
src/R3E.WebGUI.Service/
├── API/
│   ├── Controllers/         # REST API controllers
│   ├── Middleware/         # Custom middleware
│   └── Validation/         # Input validation
├── Core/
│   └── Services/           # Business logic
├── Domain/
│   └── Models/             # Domain entities
├── Infrastructure/
│   ├── Data/               # Database context
│   └── Repositories/       # Data access
├── Examples/               # Usage examples
├── scripts/                # Deployment scripts
├── Dockerfile              # Container definition
├── docker-compose.yml      # Multi-container setup
└── README.md               # Service documentation
```

### Configuration

Configuration is managed through `appsettings.json` and environment variables:

```json
{
  "R3EWebGUI": {
    "BaseDomain": "r3e-gui.com",
    "MaxFileSize": 10485760,
    "MaxFileCount": 100,
    "AllowedFileTypes": [".html", ".css", ".js", ...]
  },
  "RateLimit": {
    "MaxRequestsPerMinute": 60,
    "MaxDeploymentsPerDay": 50
  },
  "Security": {
    "RequireHttps": true,
    "EnableApiKey": false
  }
}
```

### Adding New Features

1. **Add domain models** in `Domain/Models/`
2. **Implement repository interface** in `Infrastructure/Repositories/`
3. **Add business logic** in `Core/Services/`
4. **Create API controller** in `API/Controllers/`
5. **Add validation** in `API/Validation/`
6. **Write tests** in corresponding test projects

## Testing

### Test Structure

```
tests/
├── R3E.WebGUI.Service.UnitTests/
│   ├── Controllers/        # Controller tests
│   ├── Services/           # Service tests
│   └── Repositories/       # Repository tests
├── R3E.WebGUI.Service.IntegrationTests/
│   └── WebGUIIntegrationTests.cs
└── R3E.WebGUI.Deploy.UnitTests/
    └── Services/           # Deployment tool tests
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/R3E.WebGUI.Service.UnitTests/

# Run integration tests
dotnet test tests/R3E.WebGUI.Service.IntegrationTests/
```

### Test Categories

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test complete API workflows
- **Repository Tests**: Test data access with in-memory database
- **Service Tests**: Test business logic with mocked dependencies

## Deployment

### Docker Deployment

1. **Production deployment**:
   ```bash
   ./scripts/deploy.sh production
   ```

2. **Development deployment**:
   ```bash
   ./scripts/deploy.sh development
   ```

### Manual Deployment

1. **Build the application**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Set up the database**:
   ```bash
   dotnet ef database update
   ```

3. **Configure environment variables**:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Production
   export ConnectionStrings__DefaultConnection="..."
   ```

4. **Run the application**:
   ```bash
   dotnet R3E.WebGUI.Service.dll
   ```

### Environment Configuration

| Environment | Purpose | Database | HTTPS | Logging |
|-------------|---------|----------|-------|---------|
| Development | Local dev | LocalDB | Optional | Verbose |
| Staging | Pre-prod testing | SQL Server | Required | Info |
| Production | Live service | SQL Server | Required | Warning |

### Health Checks

The service includes comprehensive health checks:

- `/health` - Basic health check
- `/health/ready` - Readiness probe for Kubernetes

Health check includes:
- Database connectivity
- File system access
- Memory usage
- Response time

## Security

### Security Features

1. **Input Validation**:
   - File type restrictions
   - File size limits
   - Contract address validation
   - SQL injection prevention

2. **Rate Limiting**:
   - Per-IP request limits
   - Deployment frequency limits
   - Configurable thresholds

3. **HTTPS Enforcement**:
   - Redirect HTTP to HTTPS
   - HSTS headers
   - Secure cookies

4. **Security Headers**:
   - X-Frame-Options
   - X-Content-Type-Options
   - X-XSS-Protection
   - Referrer-Policy

5. **API Key Authentication** (optional):
   - Header-based authentication
   - Configurable requirement

### Security Best Practices

1. **File Upload Security**:
   - Validate file extensions
   - Scan for malicious content
   - Isolate file storage
   - Limit file sizes

2. **Database Security**:
   - Use parameterized queries
   - Encrypt connection strings
   - Regular backups
   - Access logging

3. **Network Security**:
   - Use HTTPS everywhere
   - Configure CORS properly
   - Implement rate limiting
   - Monitor for attacks

## Monitoring

### Logging

The service uses structured logging with different levels:

```json
{
  "timestamp": "2024-01-01T00:00:00Z",
  "level": "Information",
  "messageTemplate": "WebGUI deployed for contract {ContractAddress}",
  "properties": {
    "ContractAddress": "0x1234...",
    "Subdomain": "mycontract",
    "RequestId": "abc123"
  }
}
```

### Metrics

Key metrics to monitor:

- **Request Metrics**:
  - Request count per endpoint
  - Response times
  - Error rates

- **Business Metrics**:
  - WebGUI deployments per day
  - Active contracts
  - Storage usage

- **System Metrics**:
  - CPU usage
  - Memory usage
  - Disk space
  - Database performance

### Alerts

Set up alerts for:

- Error rate > 5%
- Response time > 2 seconds
- Disk usage > 80%
- Database connection failures
- Rate limit violations

### Log Analysis

Common log queries:

```sql
-- Find deployment errors
SELECT * FROM Logs 
WHERE Level = 'Error' AND MessageTemplate LIKE '%deploy%'

-- Monitor response times
SELECT AVG(ResponseTime) FROM Logs 
WHERE MessageTemplate LIKE '%request completed%'
GROUP BY DATE(Timestamp)

-- Track rate limit violations
SELECT COUNT(*) FROM Logs 
WHERE MessageTemplate LIKE '%rate limit%'
GROUP BY Hour(Timestamp)
```

## Troubleshooting

### Common Issues

#### 1. Database Connection Errors

**Symptoms**: Service fails to start, database-related errors

**Solutions**:
- Check connection string format
- Verify SQL Server is running
- Ensure database exists
- Check firewall settings

```bash
# Test database connectivity
sqlcmd -S localhost -E -Q "SELECT 1"
```

#### 2. File Upload Failures

**Symptoms**: 400 errors on deployment, file validation errors

**Solutions**:
- Check file size limits
- Verify file types are allowed
- Ensure storage directory exists
- Check disk space

#### 3. Subdomain Routing Issues

**Symptoms**: 404 errors on subdomain access

**Solutions**:
- Check NGINX configuration
- Verify DNS settings
- Check file permissions
- Validate subdomain generation

#### 4. Rate Limiting Issues

**Symptoms**: 429 Too Many Requests errors

**Solutions**:
- Check rate limit configuration
- Clear rate limit cache
- Adjust limits for legitimate traffic
- Implement proper retry logic

### Debugging

1. **Enable verbose logging**:
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Debug",
         "R3E.WebGUI.Service": "Trace"
       }
     }
   }
   ```

2. **Check health endpoints**:
   ```bash
   curl http://localhost:8080/health
   ```

3. **Monitor logs**:
   ```bash
   docker-compose logs -f r3e-webgui-service
   ```

4. **Database diagnostics**:
   ```bash
   docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa
   ```

### Performance Tuning

1. **Database Optimization**:
   - Add indexes on frequently queried columns
   - Optimize connection pool settings
   - Use read replicas for search operations

2. **File Storage Optimization**:
   - Use CDN for static files
   - Implement file compression
   - Set appropriate cache headers

3. **API Optimization**:
   - Enable response compression
   - Implement response caching
   - Use async/await properly

### Monitoring Commands

```bash
# Check service status
docker-compose ps

# View logs
docker-compose logs -f [service-name]

# Check resource usage
docker stats

# Database status
docker exec -it r3e-webgui-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"

# Test API endpoints
curl -f http://localhost:8080/health
curl -X GET "http://localhost:8080/api/webgui/list?page=1&pageSize=10"

# Check file storage
ls -la /path/to/webgui-storage/
```

---

## Support

For support and questions:

- **GitHub Issues**: [r3e-network/neo-devpack-dotnet](https://github.com/r3e-network/neo-devpack-dotnet/issues)
- **Documentation**: [R3E Community Docs](https://docs.r3e-network.com)
- **Community**: [R3E Discord](https://discord.gg/r3e-community)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.