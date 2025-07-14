# R3E WebGUI Hosting Service

A modern hosting service for Neo smart contract WebGUIs with JSON-based configuration, signature authentication, and automatic interface generation.

## Overview

The R3E WebGUI Service provides a complete solution for Neo smart contract developers to deploy beautiful, interactive web interfaces without writing frontend code. The service automatically generates modern WebGUIs from contract manifests and uses JSON configuration files (`contractaddress.webgui.json`) for dynamic content.

### Key Features

- **🎨 Automatic WebGUI Generation**: Deploy professional interfaces directly from contract manifests
- **🔐 Signature Authentication**: Secure deployment without user registration - just sign with your wallet
- **📦 Plugin Management**: Upload and distribute contract plugins with signature validation
- **💰 Multi-Wallet Support**: NeoLine, O3, and WalletConnect integration built-in
- **🌐 Subdomain Routing**: Each contract gets its own subdomain (e.g., `mycontract.r3e-gui.com`)
- **⚡ Real-time Updates**: Live blockchain data, transaction monitoring, and event tracking
- **🎯 Professional Design**: Modern, responsive interfaces with customizable themes

## Architecture

### JSON-Based Configuration System

The service uses a modern JSON-based architecture where each contract's WebGUI is defined by a `contractaddress.webgui.json` file containing:

- Contract metadata (name, address, network, deployer)
- Method definitions with parameter validation
- Event specifications
- Theme customization
- Wallet settings
- Plugin download URLs

### Security Model

- **No Registration Required**: Authentication via Neo wallet signatures
- **Deployer-Only Updates**: Only the original contract deployer can update WebGUIs
- **Timestamp Validation**: Prevents replay attacks with 5-minute signature expiry
- **Plugin Integrity**: SHA256 hash validation for uploaded plugins

## Quick Start

### 1. Prerequisites

- .NET 9.0 SDK
- Neo wallet with deployed contract
- Docker (optional, for containerized deployment)

### 2. Start the Service

```bash
cd src/R3E.WebGUI.Service
dotnet run
```

The service will be available at `http://localhost:8888`

### 3. Deploy Your Contract's WebGUI

#### Option A: Using the Deployment Script (Recommended)

```bash
./deploy-contract-webgui.sh \
  -p MyContract.csproj \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -e "My awesome NEP-17 token contract"
```

The script will:
1. Compile your contract (if needed)
2. Generate a timestamp and create a message to sign
3. Prompt you to sign with your Neo wallet
4. Deploy the WebGUI with signature authentication
5. Provide the WebGUI URL

#### Option B: Direct API Call with Wallet Integration

```javascript
// 1. Create the deployment message
const timestamp = Math.floor(Date.now() / 1000);
const message = `Deploy WebGUI for contract ${contractAddress} by ${deployerAddress} at ${timestamp}`;

// 2. Sign with your wallet (e.g., NeoLine)
const signResult = await neoline.signMessage({
    message: message,
    isBase64: false
});

// 3. Deploy the WebGUI
const response = await fetch('http://localhost:8888/api/webgui/deploy-from-manifest', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        contractAddress: '0x1234567890abcdef1234567890abcdef12345678',
        contractName: 'MyToken',
        network: 'testnet',
        deployerAddress: 'NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5',
        description: 'My awesome NEP-17 token',
        timestamp: timestamp,
        signature: signResult.signature,
        publicKey: signResult.publicKey
    })
});
```

### 4. Access Your WebGUI

Your WebGUI will be instantly available at:
```
http://mytoken.localhost:8888
```

The WebGUI includes:
- ✅ All contract methods with input validation
- ✅ Real-time wallet connection
- ✅ Transaction signing and execution
- ✅ Event monitoring
- ✅ Contract statistics
- ✅ Plugin download (if uploaded)

### 5. Upload Contract Plugin (Optional)

If you have a Neo plugin for your contract:

```bash
# Calculate plugin hash
PLUGIN_HASH=$(sha256sum MyTokenPlugin.zip | awk '{print $1}')
TIMESTAMP=$(date +%s)

# Create and sign message
MESSAGE="Upload plugin for contract 0x1234... with hash $PLUGIN_HASH at $TIMESTAMP"
# Sign with your wallet...

# Upload plugin
curl -X POST "http://localhost:8888/api/webgui/0x1234.../plugin" \
  -H "X-Timestamp: $TIMESTAMP" \
  -H "X-Signature: $YOUR_SIGNATURE" \
  -H "X-Public-Key: $YOUR_PUBLIC_KEY" \
  -F "pluginFile=@MyTokenPlugin.zip"
```

## JSON Configuration Structure

Each contract's WebGUI is powered by a JSON configuration file that's automatically generated from the contract manifest:

```json
{
  "contractAddress": "0x1234567890abcdef1234567890abcdef12345678",
  "contractName": "MyToken",
  "network": "testnet",
  "deployerAddress": "NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5",
  "description": "NEP-17 Token with full WebGUI interface",
  "version": "1.0.0",
  "pluginDownloadUrl": "/api/webgui/0x1234.../plugin/download",
  "methods": [
    {
      "name": "transfer",
      "displayName": "Transfer Tokens",
      "description": "Transfer tokens between addresses",
      "safe": false,
      "parameters": [
        {
          "name": "from",
          "type": "hash160",
          "displayName": "From Address",
          "required": true
        },
        {
          "name": "to",
          "type": "hash160",
          "displayName": "To Address",
          "required": true
        },
        {
          "name": "amount",
          "type": "integer",
          "displayName": "Amount",
          "required": true,
          "validation": {
            "min": "1"
          }
        }
      ],
      "returnType": "boolean"
    }
  ],
  "events": [
    {
      "name": "Transfer",
      "displayName": "Transfer Event",
      "parameters": [
        {
          "name": "from",
          "type": "hash160"
        },
        {
          "name": "to",
          "type": "hash160"
        },
        {
          "name": "amount",
          "type": "integer"
        }
      ]
    }
  ],
  "theme": {
    "primaryColor": "#667eea",
    "secondaryColor": "#00d4aa"
  },
  "walletSettings": {
    "supportedWallets": ["neoline", "o3", "walletconnect"],
    "defaultNetwork": "testnet",
    "allowNetworkSwitch": true
  }
}
```

## Complete Workflow Example

Here's a complete example of deploying a NEP-17 token contract with WebGUI:

```bash
# 1. Compile your contract
neo-express compile MyToken.csproj --output ./build

# 2. Deploy contract to Neo network
neo-express contract deploy ./build/MyToken.nef --network testnet
# Note the contract address: 0x1234567890abcdef1234567890abcdef12345678

# 3. Deploy WebGUI with plugin
./deploy-contract-webgui.sh \
  -p MyToken.csproj \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -n "MyToken" \
  -w testnet \
  -g \  # Generate plugin
  -e "NEP-17 Token with full WebGUI"

# 4. Access your WebGUI
open http://mytoken.localhost:8888
```

## WebGUI Features

Each automatically generated WebGUI includes:

### 📊 Contract Overview
- Contract name, address, and network
- Deployer information
- Contract description
- Plugin download link (if available)

### 💰 Wallet Integration
- Connect with NeoLine, O3, or WalletConnect
- Real-time balance display
- Network switching support
- Transaction history

### 🔧 Method Invocation
- All contract methods with proper categorization
- Input validation based on parameter types
- Read-only method calls (no wallet required)
- Write method transactions (wallet signature required)
- Gas estimation before execution

### 📡 Event Monitoring
- Real-time event updates
- Event history with filtering
- Parameter decoding

### 🎨 Professional UI
- Modern, responsive design
- Dark/light theme support
- Mobile-friendly interface
- Smooth animations and transitions

## API Reference

All deployment endpoints require signature authentication. See [SIGNATURE_AUTH.md](SIGNATURE_AUTH.md) for detailed signing instructions.

### Deploy WebGUI from Manifest
```http
POST /api/webgui/deploy-from-manifest
Content-Type: application/json

{
  "contractAddress": "string",     // Contract address (0x + 40 hex chars)
  "contractName": "string",        // Contract name (optional)
  "network": "string",             // "testnet" or "mainnet"
  "deployerAddress": "string",     // Neo address of deployer
  "description": "string",         // Contract description (optional)
  "timestamp": number,             // Unix timestamp
  "signature": "string",           // Hex signature
  "publicKey": "string"            // Hex public key
}
```

**Response:**
```json
{
  "success": true,
  "subdomain": "mytoken",
  "url": "http://mytoken.localhost:8888",
  "contractAddress": "0x1234567890abcdef1234567890abcdef12345678",
  "contractName": "MyToken",
  "configGenerated": true,
  "methodsFound": 5,
  "eventsFound": 3
}
```

### Get Contract Configuration
```http
GET /api/webgui/{contractAddress}/config
```

Returns the complete JSON configuration including methods, events, theme, and wallet settings.

### Upload Plugin
```http
POST /api/webgui/{contractAddress}/plugin
Headers:
    X-Timestamp: {unix_timestamp}
    X-Signature: {signature_hex}
    X-Public-Key: {public_key_hex}
Body:
    pluginFile: (ZIP file, max 10MB)
```

**Signature Message Format:**
```
Upload plugin for contract {contractAddress} with hash {sha256_hash} at {timestamp}
```

### Download Plugin
```http
GET /api/webgui/{contractAddress}/plugin/download
```

Returns the plugin ZIP file for download.

### Search Contracts
```http
GET /api/webgui/search?contractAddress={address}&network={network}
```

Find deployed WebGUIs by contract address.

### List WebGUIs
```http
GET /api/webgui/list?page={page}&pageSize={pageSize}&network={network}
```

Get paginated list of all deployed WebGUIs.

### Get WebGUI Info
```http
GET /api/webgui/{subdomain}
```

Get WebGUI details by subdomain.


## Docker Deployment

### Quick Start with Docker Compose

```bash
cd src/R3E.WebGUI.Service
docker-compose up -d
```

This will start:
- R3E WebGUI Service on port 8888
- SQL Server database
- Nginx reverse proxy with subdomain routing

### Production Deployment

For production environments:

```bash
# Build the image
docker build -t r3e-webgui-service .

# Run with environment variables
docker run -d \
  -p 8888:8080 \
  -e ConnectionStrings__DefaultConnection="Your_Connection_String" \
  -e R3EWebGUI__BaseDomain="your-domain.com" \
  -e NEO_RPC_TESTNET="https://test1.neo.coz.io:443" \
  -e NEO_RPC_MAINNET="https://mainnet1.neo.coz.io:443" \
  -v webgui-storage:/app/webgui-storage \
  r3e-webgui-service
```

## Configuration

### Environment Variables

```bash
# Database
ConnectionStrings__DefaultConnection="Server=localhost;Database=R3EWebGUI;..."

# Service Settings
R3EWebGUI__BaseDomain="localhost"              # Domain for subdomains
R3EWebGUI__StorageBasePath="/app/webgui-storage"
R3EWebGUI__MaxFileSizeKB=5120                 # Max file size in KB
R3EWebGUI__AllowedNetworks=["testnet","mainnet"]

# Neo RPC Endpoints
NEO_RPC_TESTNET="https://test1.neo.coz.io:443"
NEO_RPC_MAINNET="https://mainnet1.neo.coz.io:443"

# Rate Limiting
R3EWebGUI__RateLimiting__EnableRateLimiting=true
R3EWebGUI__RateLimiting__PermitLimit=100
R3EWebGUI__RateLimiting__WindowMinutes=1
```

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=R3EWebGUIService;Trusted_Connection=true"
  },
  "R3EWebGUI": {
    "BaseDomain": "localhost",
    "StorageBasePath": "./webgui-storage",
    "MaxFileSizeKB": 5120,
    "AllowedNetworks": ["testnet", "mainnet"],
    "SignatureExpiryMinutes": 5,
    "RpcEndpoints": {
      "Testnet": "https://test1.neo.coz.io:443",
      "Mainnet": "https://mainnet1.neo.coz.io:443"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## Security Best Practices

### 🔐 Signature Authentication
- All deployments require wallet signatures
- Signatures expire after 5 minutes
- Only contract deployers can update WebGUIs
- Plugin uploads require hash validation

### 🛡️ Input Validation
- Contract address format validation
- File type and size restrictions
- Parameter type validation in WebGUI
- XSS prevention in generated content

### 🔒 Infrastructure Security
- Run service as non-root user
- Use HTTPS in production
- Enable rate limiting
- Regular security updates

### 📝 Audit Trail
- All deployments are logged
- Signature validation tracked
- Plugin uploads recorded
- Access logs maintained

## Development

### Prerequisites
- .NET 9.0 SDK
- SQL Server or SQL Server Express LocalDB
- Neo wallet (for testing deployments)

### Local Development Setup

```bash
# Clone the repository
git clone https://github.com/r3e-network/neo-devpack-dotnet.git
cd neo-devpack-dotnet/src/R3E.WebGUI.Service

# Initialize database
dotnet ef database update

# Run the service
dotnet run

# Service will be available at http://localhost:8888
```

### Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## Troubleshooting

### Common Issues

**WebGUI not loading at subdomain:**
- Ensure hosts file includes: `127.0.0.1 mycontract.localhost`
- Check if subdomain routing is working in nginx/IIS

**Signature validation failing:**
- Verify timestamp is within 5 minutes
- Ensure message format exactly matches expected format
- Check public key corresponds to deployer address

**Contract not found on network:**
- Verify contract is deployed to the correct network
- Check RPC endpoint is accessible
- Ensure contract address format is correct (0x + 40 hex chars)

**Plugin upload failing:**
- Verify file is a valid ZIP archive
- Check file size is under 10MB
- Ensure you're signing with the deployer's wallet

## Contributing

We welcome contributions! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/r3e-network/neo-devpack-dotnet/wiki)
- 🐛 [Issue Tracker](https://github.com/r3e-network/neo-devpack-dotnet/issues)
- 💬 [Discussions](https://github.com/r3e-network/neo-devpack-dotnet/discussions)
- 🌐 [Neo Documentation](https://docs.neo.org/)

## Acknowledgments

- Neo Foundation for blockchain infrastructure
- Contributors to the neo-devpack-dotnet project
- Community members for feedback and testing