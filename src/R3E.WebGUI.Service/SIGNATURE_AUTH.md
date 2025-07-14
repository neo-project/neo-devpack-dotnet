# Signature-Based Authentication for R3E WebGUI Service

## Overview

The R3E WebGUI Service uses signature-based authentication to ensure that only the actual contract deployer can manage WebGUIs and upload plugins. No user registration is required - authentication is performed by validating Neo wallet signatures.

## How It Works

1. **No Registration Required**: Anyone can deploy a WebGUI as long as they can prove ownership of the deployer address
2. **Signature Validation**: All deployment and modification operations require a valid signature from the deployer's Neo wallet
3. **Timestamp Protection**: Signatures include timestamps to prevent replay attacks (5-minute validity window)

## Required Signatures

### 1. WebGUI Deployment

When deploying a WebGUI from a contract manifest, you must sign a message with the following format:

```
Deploy WebGUI for contract {contractAddress} by {deployerAddress} at {timestamp}
```

**Request Format:**
```json
POST /api/webgui/deploy-from-manifest
{
    "contractAddress": "0x...",
    "contractName": "MyContract",
    "network": "testnet",
    "deployerAddress": "NPvK...",
    "description": "Contract description",
    "timestamp": 1704067200,
    "signature": "signature_hex",
    "publicKey": "public_key_hex"
}
```

### 2. Plugin Upload

When uploading a plugin, you must sign a message that includes the plugin's SHA256 hash:

```
Upload plugin for contract {contractAddress} with hash {pluginHash} at {timestamp}
```

**Request Format:**
```
POST /api/webgui/{contractAddress}/plugin
Headers:
  X-Timestamp: 1704067200
  X-Signature: signature_hex
  X-Public-Key: public_key_hex
Body:
  pluginFile: (binary)
```

## Signature Generation

### Using Neo Wallet (JavaScript Example)

```javascript
// Example using NeoLine wallet
async function signDeployment(contractAddress, deployerAddress) {
    const timestamp = Math.floor(Date.now() / 1000);
    const message = `Deploy WebGUI for contract ${contractAddress} by ${deployerAddress} at ${timestamp}`;
    
    const result = await neoline.signMessage({
        message: message,
        isBase64: false
    });
    
    return {
        timestamp: timestamp,
        signature: result.signature,
        publicKey: result.publicKey
    };
}
```

### Using neo-cli

```bash
# Generate signature using neo-cli
neo> sign "Deploy WebGUI for contract 0x123... by NPvK... at 1704067200"
```

### Using Python (py-neo)

```python
from neo.Core.Cryptography.Crypto import Crypto
from neo.Wallets import Wallet

def sign_deployment(contract_address, deployer_address, private_key):
    timestamp = int(time.time())
    message = f"Deploy WebGUI for contract {contract_address} by {deployer_address} at {timestamp}"
    
    signature = Crypto.Sign(message.encode('utf-8'), private_key)
    public_key = private_key.PublicKey
    
    return {
        'timestamp': timestamp,
        'signature': signature.hex(),
        'publicKey': public_key.encode_point(True).hex()
    }
```

## Security Features

### Timestamp Validation
- All signatures must include a timestamp
- Timestamps are valid for 5 minutes by default
- Prevents replay attacks

### Message Format
- Messages follow a strict format to prevent signature reuse
- Plugin uploads include file hash to ensure integrity
- Contract address and deployer address are bound to the signature

### Public Key Verification
- Public key must correspond to the deployer address
- Address is derived from public key and verified
- Ensures only the actual key holder can sign

## Configuration

### Timestamp Validity Window

Configure in `appsettings.json`:
```json
{
  "R3EWebGUI": {
    "Security": {
      "TimestampValidityMinutes": 5
    }
  }
}
```

## Common Errors

### Invalid or expired timestamp
- **Cause**: Timestamp is older than 5 minutes or in the future
- **Solution**: Ensure system clocks are synchronized and sign just before sending

### Invalid signature or unauthorized deployer
- **Cause**: Signature doesn't match the message or public key doesn't match deployer address
- **Solution**: Verify message format and ensure correct wallet is being used

### Plugin hash mismatch
- **Cause**: Plugin file was modified after signing
- **Solution**: Calculate hash and sign immediately before upload

## Testing

Use the provided deployment script with signature support:

```bash
./deploy-contract-webgui.sh \
  -p MyContract.csproj \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -g
```

The script will prompt for signatures at the appropriate steps.

## Benefits

1. **No Registration**: No need to create accounts or manage passwords
2. **Decentralized**: Authentication is based on blockchain addresses
3. **Secure**: Cryptographic signatures ensure authenticity
4. **Simple**: Uses existing Neo wallet infrastructure
5. **Auditable**: All actions are tied to specific addresses