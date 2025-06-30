# NEO Smart Contract - Inscription Example

This example demonstrates on-chain data inscription patterns for storing and retrieving content on the NEO blockchain.

## Overview

The Inscription smart contract showcases how to efficiently store data on-chain with proper indexing and retrieval mechanisms. This pattern is useful for creating decentralized registries, proof of existence systems, and metadata storage.

## Features

- **Data Inscription**: Store arbitrary data on-chain with unique identifiers
- **Content Addressing**: Retrieve inscriptions by hash or ID
- **Metadata Management**: Associate metadata with inscribed content
- **Gas Optimization**: Efficient storage patterns to minimize costs
- **Access Control**: Owner-only inscription capabilities

## Key Concepts Demonstrated

- Efficient on-chain storage patterns
- Content addressing and indexing
- Gas optimization techniques
- Data integrity verification
- Event-driven architecture

## Contract Methods

### Inscription Operations

- `Inscribe(data, metadata)` - Store data with associated metadata
- `GetInscription(id)` - Retrieve inscription by ID
- `GetInscriptionByHash(hash)` - Retrieve inscription by content hash
- `GetInscriptionCount()` - Get total number of inscriptions

### Query Operations

- `ListInscriptions(offset, limit)` - Paginated inscription listing
- `SearchByOwner(owner)` - Find inscriptions by owner
- `VerifyInscription(id, data)` - Verify inscription integrity

## Usage Examples

### Inscribing Data

```csharp
// Inscribe a document with metadata
var documentData = "Important document content";
var metadata = new Map<string, object>
{
    ["title"] = "Contract Agreement",
    ["timestamp"] = Runtime.Time,
    ["hash"] = CryptoLib.Sha256(documentData)
};

var inscriptionId = Inscribe(documentData, metadata);
```

### Retrieving Inscriptions

```csharp
// Get inscription details
var inscription = GetInscription(inscriptionId);
var title = (string)inscription["metadata"]["title"];
var content = (string)inscription["data"];
```

## Gas Optimization Patterns

This example demonstrates several gas optimization techniques:

- **Efficient storage keys**: Minimized key lengths for reduced storage costs
- **Lazy initialization**: Initialize storage only when needed
- **Batch operations**: Group related operations to reduce transaction overhead
- **Compression patterns**: Store data efficiently to minimize storage usage

## Security Considerations

- **Access Control**: Only authorized users can create inscriptions
- **Data Validation**: Input validation prevents invalid or malicious data
- **Integrity Checks**: Content hashing ensures data integrity
- **Rate Limiting**: Prevents spam and abuse

## Use Cases

### Proof of Existence
```csharp
// Prove a document existed at a specific time
var proofId = Inscribe(documentHash, new Map<string, object>
{
    ["type"] = "proof",
    ["timestamp"] = Runtime.Time
});
```

### Decentralized Registry
```csharp
// Register domain names or identifiers
var registryEntry = Inscribe(domainName, new Map<string, object>
{
    ["owner"] = Runtime.CallingScriptHash,
    ["expires"] = Runtime.Time + (365 * 24 * 60 * 60 * 1000) // 1 year
});
```

### Metadata Storage
```csharp
// Store NFT metadata on-chain
var metadata = Inscribe(jsonMetadata, new Map<string, object>
{
    ["type"] = "nft-metadata",
    ["tokenId"] = tokenId,
    ["collection"] = collectionAddress
});
```

## Testing

The example includes comprehensive tests covering:

- Basic inscription and retrieval
- Content addressing functionality
- Gas consumption analysis
- Error handling scenarios
- Edge cases and boundary conditions

Run tests with:
```bash
cd Example.SmartContract.Inscription.UnitTests
dotnet test
```

## Deployment Considerations

- **Storage Costs**: Consider the cost of on-chain storage for large data
- **Gas Limits**: Ensure inscriptions fit within transaction gas limits
- **Indexing Strategy**: Plan your indexing approach for efficient queries
- **Upgrade Path**: Design contracts with future extensibility in mind

## Related Examples

- [Storage Example](Example.SmartContract.Storage/) - Basic storage patterns
- [Events Example](Example.SmartContract.Event/) - Event-driven architecture
- [NEP-11 NFT](Example.SmartContract.NFT/) - NFT metadata patterns

This example provides a foundation for building more complex on-chain data storage systems and demonstrates best practices for efficient blockchain data management.