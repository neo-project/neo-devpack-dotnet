# R3E WebGUI Service Usage Examples - JSON-Based Design

## Overview

The R3E WebGUI Service uses a modern JSON-based configuration system with signature authentication. WebGUIs are automatically generated from contract manifests, eliminating the need for manual HTML/CSS/JS file creation.

## Example 1: Deploy WebGUI from Contract Manifest

### 1. Prepare Your Neo Contract

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[DisplayName("MyToken")]
[ManifestExtra("Author", "Your Name")]
[ManifestExtra("Email", "your.email@example.com")]
[ManifestExtra("Description", "A sample NEP-17 token")]
public class MyToken : SmartContract
{
    [Safe]
    public static string Symbol() => "MYT";
    
    [Safe]
    public static int Decimals() => 8;
    
    [Safe]
    public static BigInteger BalanceOf(UInt160 account) 
    {
        return (BigInteger)Storage.Get(Storage.CurrentContext, account);
    }
    
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
    {
        // Transfer implementation
        return true;
    }
}
```

### 2. Deploy Contract to Neo Network

```bash
# Deploy using neo-express or your preferred method
neo-express contract deploy ./build/MyToken.nef
# Note the deployed contract address: 0x1234567890abcdef1234567890abcdef12345678
```

### 3. Deploy WebGUI with Signature Authentication

#### Using the deployment script:

```bash
./deploy-contract-webgui.sh \
  -p MyToken.csproj \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -e "NEP-17 Token with full WebGUI interface"
```

The script will:
1. Generate a timestamp
2. Create a message to sign: `Deploy WebGUI for contract 0x123... by NPvK... at 1704067200`
3. Prompt for signature and public key (use your Neo wallet)
4. Deploy the WebGUI

#### Using direct API call:

```javascript
// Step 1: Create and sign message with your Neo wallet
const timestamp = Math.floor(Date.now() / 1000);
const message = `Deploy WebGUI for contract ${contractAddress} by ${deployerAddress} at ${timestamp}`;

// Sign with NeoLine wallet
const signResult = await neoline.signMessage({
    message: message,
    isBase64: false
});

// Step 2: Deploy WebGUI
const response = await fetch('http://localhost:8888/api/webgui/deploy-from-manifest', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        contractAddress: '0x1234567890abcdef1234567890abcdef12345678',
        contractName: 'MyToken',
        network: 'testnet',
        deployerAddress: 'NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5',
        description: 'NEP-17 Token with full WebGUI interface',
        timestamp: timestamp,
        signature: signResult.signature,
        publicKey: signResult.publicKey
    })
});
```

### 4. Access Your WebGUI

Visit: `http://mytoken.localhost:8888`

The WebGUI will automatically:
- Display all contract methods from the manifest
- Provide wallet connection (NeoLine, O3, WalletConnect)
- Allow method invocation with proper parameter validation
- Show real-time blockchain data
- Display contract events

## Example 2: Upload Plugin with Signature

### 1. Generate Plugin (if using Neo compiler)

```bash
# Compile with plugin generation
neo-express compile MyToken.csproj --generate-plugin --output ./output
```

### 2. Upload Plugin

```bash
# Calculate plugin hash
PLUGIN_HASH=$(sha256sum ./output/MyTokenPlugin.zip | awk '{print $1}')
TIMESTAMP=$(date +%s)

# Create message for signing
MESSAGE="Upload plugin for contract 0x1234567890abcdef1234567890abcdef12345678 with hash $PLUGIN_HASH at $TIMESTAMP"

# Sign with your wallet and get signature/public key
# Then upload:
curl -X POST "http://localhost:8888/api/webgui/0x1234567890abcdef1234567890abcdef12345678/plugin" \
  -H "X-Timestamp: $TIMESTAMP" \
  -H "X-Signature: your_signature_here" \
  -H "X-Public-Key: your_public_key_here" \
  -F "pluginFile=@./output/MyTokenPlugin.zip"
```

## Example 3: Contract Configuration Structure

When you deploy a WebGUI, a JSON configuration is automatically generated:

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
      "name": "symbol",
      "displayName": "Token Symbol",
      "description": "Returns the token symbol",
      "returnType": "string",
      "parameters": [],
      "isReadOnly": true,
      "category": "token-info"
    },
    {
      "name": "transfer",
      "displayName": "Transfer Tokens",
      "description": "Transfer tokens between addresses",
      "returnType": "boolean",
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
        },
        {
          "name": "data",
          "type": "any",
          "displayName": "Additional Data",
          "required": false
        }
      ],
      "isReadOnly": false,
      "category": "token-operations"
    }
  ],
  "events": [
    {
      "name": "Transfer",
      "displayName": "Transfer Event",
      "description": "Emitted when tokens are transferred",
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
    "secondaryColor": "#00d4aa",
    "accentColor": "#764ba2"
  },
  "walletSettings": {
    "supportedWallets": ["neoline", "o3", "walletconnect"],
    "defaultNetwork": "testnet",
    "allowNetworkSwitch": true
  }
}
```

## Example 4: Complete Production Workflow

### 1. Compile Contract with All Outputs

```bash
# Compile contract with manifest, NEF, and plugin
neo-express compile MyContract.csproj \
  --generate-plugin \
  --output ./build
```

### 2. Deploy Contract to Network

```bash
# Deploy to testnet
neo-express contract deploy ./build/MyContract.nef \
  --network testnet \
  --account deployer
```

### 3. Deploy WebGUI and Plugin

```bash
# Use the comprehensive deployment script
./deploy-contract-webgui.sh \
  -p MyContract.csproj \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -n "MyContract" \
  -w testnet \
  -g \
  -u ./build/MyContractPlugin.zip \
  -e "Production-ready contract with full features"
```

### 4. Verify Deployment

```bash
# Check configuration
curl "http://localhost:8888/api/webgui/0x1234567890abcdef1234567890abcdef12345678/config"

# Test WebGUI access
curl -I "http://mycontract.localhost:8888/subdomain"

# Download plugin
curl -O "http://localhost:8888/api/webgui/0x1234567890abcdef1234567890abcdef12345678/plugin/download"
```

## Security Best Practices

1. **Always Sign Messages**: Never send deployment requests without proper signatures
2. **Verify Timestamps**: Ensure your system clock is synchronized
3. **Secure Your Private Keys**: Use hardware wallets or secure key management
4. **Validate Plugin Hashes**: Verify plugin integrity before and after upload
5. **Use HTTPS in Production**: Always use encrypted connections

## API Quick Reference

### Deploy WebGUI (Requires Signature)
```
POST /api/webgui/deploy-from-manifest
```

### Get Configuration
```
GET /api/webgui/{contractAddress}/config
```

### Upload Plugin (Requires Signature)
```
POST /api/webgui/{contractAddress}/plugin
```

### Download Plugin
```
GET /api/webgui/{contractAddress}/plugin/download
```

### Search Contracts
```
GET /api/webgui/search?contractAddress={address}
```

### List WebGUIs
```
GET /api/webgui/list?page=1&pageSize=10
```

## Troubleshooting

### Invalid or expired timestamp
- Ensure system clock is synchronized
- Sign and send request within 5 minutes

### Invalid signature or unauthorized deployer
- Verify you're signing with the correct wallet
- Ensure public key corresponds to deployer address
- Check message format exactly matches expected format

### Contract not found on network
- Verify contract is deployed to the specified network
- Check contract address format (0x + 40 hex characters)

## Support

- API Documentation: `/swagger`
- Signature Authentication Guide: `SIGNATURE_AUTH.md`
- GitHub Issues: https://github.com/r3e-network/neo-devpack-dotnet/issues