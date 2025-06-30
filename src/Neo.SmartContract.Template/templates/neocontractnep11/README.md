# NEO Smart Contract - NEP-11 Non-Fungible Token

This template provides a complete implementation of the NEP-11 standard for non-fungible tokens (NFTs) on the NEO blockchain.

## Features

- **Standard Compliance**: Full NEP-11 implementation
- **Token Minting**: Create unique tokens with custom metadata
- **Ownership Management**: Track and transfer token ownership
- **Rich Metadata**: Support for name, description, image, and custom attributes
- **Owner Controls**: Admin functions for minting and contract management
- **Upgrade Support**: Built-in contract upgrade functionality

## Getting Started

### Building the Contract

```bash
dotnet build
```

### Compiling to NEO Bytecode

```bash
dotnet run --project path/to/Neo.Compiler.CSharp -- YourContract.csproj
```

## Contract Methods

### Safe Methods (Read-Only)

- `Symbol()` - Returns the token symbol
- `Decimals()` - Returns 0 (NFTs are indivisible)
- `GetOwner()` - Returns the contract owner
- `BalanceOf(owner)` - Returns token count for an owner
- `OwnerOf(tokenId)` - Returns the owner of a specific token
- `Properties(tokenId)` - Returns token metadata

### State-Changing Methods

- `Mint(to, name, description, image, attributes)` - Create a new NFT
- `Transfer(from, to, tokenId, data)` - Transfer token ownership
- `Burn(tokenId)` - Destroy a token
- `SetOwner(newOwner)` - Change contract ownership
- `Update(nefFile, manifest)` - Upgrade the contract
- `Destroy()` - Destroy the contract

## Usage Examples

### Minting an NFT

```csharp
var attributes = new Map<string, object>
{
    ["rarity"] = "legendary",
    ["power"] = 100,
    ["element"] = "fire"
};

Mint(
    recipientAddress,
    "Dragon Sword",
    "A legendary sword forged by dragons",
    "https://example.com/dragon-sword.png",
    attributes
);
```

### Checking Token Properties

```csharp
var properties = Properties(tokenId);
var name = (string)properties["name"];
var rarity = (string)properties["rarity"];
```

## Security Considerations

- Only the contract owner can mint new tokens
- Token owners can burn their own tokens
- Always validate token IDs before operations
- Be careful with metadata immutability

## Deployment

1. Compile the contract
2. Deploy using NEO CLI or other tools
3. Set appropriate permissions and owner
4. Test on testnet before mainnet deployment

## License

MIT License - see LICENSE file for details