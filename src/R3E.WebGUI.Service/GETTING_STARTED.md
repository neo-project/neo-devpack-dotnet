# Getting Started with R3E WebGUI Service

Welcome to the R3E WebGUI Service! This guide will help you deploy your first professional web interface for a Neo smart contract in under 5 minutes.

## 🚀 Quick Start (5 Minutes)

### Prerequisites
- ✅ Neo wallet (NeoLine, O3, or compatible)
- ✅ Deployed Neo smart contract
- ✅ Contract address and deployer address
- ✅ .NET 9.0 SDK (for local development)

### Step 1: Start the Service

#### Option A: Docker (Recommended)
```bash
# Clone and start
git clone https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet/src/R3E.WebGUI.Service
docker-compose up -d

# Service available at http://localhost:8888
```

#### Option B: Local Development
```bash
# Clone and run
git clone https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet/src/R3E.WebGUI.Service
dotnet run

# Service available at http://localhost:8888
```

### Step 2: Deploy Your WebGUI

#### Using the Deployment Script
```bash
# Make the script executable
chmod +x deploy-contract-webgui.sh

# Deploy your contract's WebGUI
./deploy-contract-webgui.sh \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -n "MyToken" \
  -e "My awesome NEP-17 token"
```

#### The script will:
1. 📝 Generate a message for you to sign
2. 🔐 Prompt for your wallet signature
3. 🚀 Deploy the WebGUI automatically
4. 🌐 Provide your unique WebGUI URL

### Step 3: Access Your WebGUI
```
🎉 Your WebGUI is ready at: http://mytoken.localhost:8888
```

## 📖 Detailed Walkthrough

### Understanding the Deployment Process

#### 1. Message Signing
The R3E WebGUI Service uses signature-based authentication:
```
Message Format: "Deploy WebGUI for contract {address} by {deployer} at {timestamp}"
Example: "Deploy WebGUI for contract 0x1234... by NPvK... at 1704067200"
```

#### 2. Signature with Your Wallet
- **NeoLine**: `neoline.signMessage({ message, isBase64: false })`
- **O3**: `o3dapi.neo.signMessage({ message })`
- **Manual**: Use any Neo-compatible signing tool

#### 3. WebGUI Generation
The service automatically:
- Fetches your contract manifest from the blockchain
- Parses all methods and events
- Generates a professional JSON configuration
- Creates a modern, responsive web interface
- Deploys to a unique subdomain

### What Gets Generated

#### Contract Information Panel
```
📊 Contract Overview
├── Name: MyToken
├── Address: 0x1234...
├── Network: TestNet
├── Deployer: NPvK...
└── Description: My awesome NEP-17 token
```

#### Method Interface
```
🔧 Contract Methods
├── 📖 Read Methods (No gas required)
│   ├── symbol() → string
│   ├── decimals() → integer
│   └── balanceOf(account) → integer
└── ✍️ Write Methods (Requires signature)
    ├── transfer(from, to, amount, data) → boolean
    └── mint(to, amount) → boolean
```

#### Real-time Features
- 💰 **Wallet Connection**: Connect with one click
- 📡 **Live Updates**: Real-time blockchain data
- 📜 **Event Monitor**: Watch contract events
- 📈 **Transaction History**: Complete interaction log

## 🛠️ Advanced Configuration

### Custom JSON Configuration

After deployment, you can customize your WebGUI by editing the generated JSON configuration:

```bash
# Get your configuration
curl "http://localhost:8888/api/webgui/0x1234.../config"
```

#### Customization Options

##### 1. Theme Customization
```json
{
  "theme": {
    "primaryColor": "#667eea",
    "secondaryColor": "#00d4aa",
    "accentColor": "#764ba2",
    "backgroundColor": "#f8fafc"
  }
}
```

##### 2. Method Categorization
```json
{
  "methods": [
    {
      "name": "transfer",
      "category": "token-operations",
      "displayName": "Transfer Tokens",
      "description": "Send tokens to another address"
    }
  ]
}
```

##### 3. Parameter Validation
```json
{
  "parameters": [
    {
      "name": "amount",
      "type": "integer",
      "validation": {
        "min": "1",
        "max": "1000000000"
      }
    }
  ]
}
```

### Plugin Upload

If you have a Neo plugin for your contract:

```bash
# Calculate plugin hash
PLUGIN_HASH=$(sha256sum MyContractPlugin.zip | awk '{print $1}')
TIMESTAMP=$(date +%s)

# Sign the plugin upload message
MESSAGE="Upload plugin for contract 0x1234... with hash $PLUGIN_HASH at $TIMESTAMP"

# Upload with signature
curl -X POST "http://localhost:8888/api/webgui/0x1234.../plugin" \
  -H "X-Timestamp: $TIMESTAMP" \
  -H "X-Signature: $YOUR_SIGNATURE" \
  -H "X-Public-Key: $YOUR_PUBLIC_KEY" \
  -F "pluginFile=@MyContractPlugin.zip"
```

## 🌐 Production Deployment

### Environment Setup

#### 1. Domain Configuration
```bash
# Update your domain settings
export WEBGUI_BASE_DOMAIN="your-domain.com"
export WEBGUI_SERVICE_URL="https://api.your-domain.com"
```

#### 2. SSL/TLS Setup
```bash
# Generate SSL certificates
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout ssl/your-domain.key \
  -out ssl/your-domain.crt
```

#### 3. Production Docker Compose
```bash
# Use production configuration
docker-compose -f docker-compose.production.yml up -d
```

### Security Considerations

#### 1. Database Security
- Use strong connection strings
- Enable encryption in transit
- Regular security updates
- Backup and recovery procedures

#### 2. API Security
- Rate limiting enabled by default
- HTTPS enforcement in production
- Input validation and sanitization
- Signature verification for all operations

#### 3. Infrastructure Security
- Regular security scans
- Monitoring and alerting
- Access logging and audit trails
- Firewall and network security

## 🔧 Development Integration

### Continuous Integration

#### GitHub Actions Workflow
```yaml
name: Deploy WebGUI
on:
  push:
    branches: [main]

jobs:
  deploy-webgui:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Deploy Contract WebGUI
        run: |
          ./deploy-contract-webgui.sh \
            -a ${{ secrets.CONTRACT_ADDRESS }} \
            -d ${{ secrets.DEPLOYER_ADDRESS }} \
            -n "${{ github.event.repository.name }}" \
            -e "Auto-deployed from ${{ github.sha }}"
        env:
          WEBGUI_SERVICE_URL: ${{ secrets.WEBGUI_SERVICE_URL }}
```

### Local Development Workflow

#### 1. Contract Development
```bash
# Develop your contract
nano MyContract.cs

# Compile with Neo compiler
neo-express compile MyContract.csproj
```

#### 2. Local Testing
```bash
# Deploy to local Neo network
neo-express contract deploy MyContract.nef

# Deploy WebGUI for testing
./deploy-contract-webgui.sh -a 0x... -d NPvK... -n "MyContract"
```

#### 3. Production Deployment
```bash
# Deploy to TestNet/MainNet
neo-express contract deploy MyContract.nef --network testnet

# Deploy production WebGUI
./deploy-contract-webgui.sh \
  -a 0x... \
  -d NPvK... \
  -n "MyContract" \
  -w testnet \
  -e "Production deployment"
```

## 📊 Monitoring and Analytics

### Health Monitoring
```bash
# Check service health
curl "http://localhost:8888/health"

# Get WebGUI status
curl "http://localhost:8888/api/webgui/0x.../config"
```

### Usage Analytics
The WebGUI service provides built-in analytics:
- 📈 Page views and user engagement
- 🔄 Method invocation statistics
- 💰 Transaction success rates
- 🕒 Response time monitoring

### Performance Optimization
- **CDN Integration**: Global content delivery
- **Caching**: Intelligent response caching
- **Compression**: Gzip compression enabled
- **Monitoring**: Real-time performance metrics

## 🆘 Troubleshooting

### Common Issues

#### WebGUI Not Loading
```bash
# Check if subdomain routing is working
curl -I "http://mycontract.localhost:8888"

# Verify hosts file (local development)
echo "127.0.0.1 mycontract.localhost" >> /etc/hosts
```

#### Signature Validation Errors
```bash
# Verify timestamp is within 5 minutes
date +%s

# Check message format exactly matches
echo "Deploy WebGUI for contract 0x... by NPvK... at $(date +%s)"
```

#### Contract Not Found
```bash
# Verify contract is deployed
curl "https://test1.neo.coz.io:443" \
  -X POST \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"getcontractstate","params":["0x..."],"id":1}'
```

### Getting Help

#### Community Support
- 💬 **Discord**: Join the Neo developer community
- 🐛 **GitHub Issues**: Report bugs and feature requests
- 📚 **Documentation**: Comprehensive guides and references
- 🎥 **Video Tutorials**: Step-by-step visual guides

#### Professional Support
- 🏢 **Enterprise Support**: Available for production deployments
- 🛠️ **Custom Development**: Tailored solutions and integrations
- 📈 **Performance Optimization**: Scaling and optimization services
- 🔐 **Security Audits**: Professional security assessments

## 🎯 Next Steps

### Explore Advanced Features
1. **Custom Themes**: Create branded interfaces
2. **Plugin Development**: Enhance contract functionality
3. **Analytics Integration**: Deep usage insights
4. **Multi-Network Deployment**: Deploy across networks

### Join the Community
1. **Contribute**: Help improve the R3E WebGUI Service
2. **Share**: Show off your WebGUI deployments
3. **Learn**: Participate in workshops and tutorials
4. **Network**: Connect with other Neo developers

### Stay Updated
- ⭐ **Star the Repository**: Get notified of updates
- 📧 **Subscribe**: Join our newsletter for announcements
- 📱 **Follow**: Social media for tips and updates
- 📖 **Read**: Blog posts and technical articles

---

🎉 **Congratulations!** You're now ready to deploy professional web interfaces for your Neo smart contracts. The R3E WebGUI Service transforms your contracts into accessible, beautiful web applications that users will love to interact with.

Start building the future of decentralized applications with R3E! 🚀