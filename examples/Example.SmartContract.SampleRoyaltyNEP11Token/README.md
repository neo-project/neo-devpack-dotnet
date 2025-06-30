# NEO Smart Contract - Royalty NFT (NEP-11) Example

This example demonstrates an advanced NEP-11 non-fungible token implementation with built-in royalty support for creators and marketplaces.

## Overview

The Royalty NFT contract extends the standard NEP-11 implementation to include automatic royalty distribution on secondary sales. This enables creators to earn ongoing revenue from their digital assets while supporting marketplace ecosystems.

## Features

- **NEP-11 Compliance**: Full NEP-11 standard implementation
- **Creator Royalties**: Automatic royalty payments to original creators
- **Marketplace Support**: Integration with NFT marketplaces and exchanges
- **Flexible Royalty Rates**: Configurable royalty percentages
- **Multi-Recipient Royalties**: Support for multiple royalty recipients
- **Royalty Enforcement**: On-chain royalty calculation and distribution

## Key Concepts Demonstrated

- Advanced NEP-11 token implementation
- Royalty calculation and distribution
- Marketplace integration patterns
- Revenue sharing mechanisms
- Metadata management with royalty information

## Contract Methods

### NFT Operations

- `Mint(to, tokenData, royaltyInfo)` - Create NFT with royalty settings
- `Transfer(from, to, tokenId, data)` - Transfer with royalty processing
- `Burn(tokenId)` - Destroy an NFT
- `Properties(tokenId)` - Get token metadata including royalties

### Royalty Management

- `GetRoyaltyInfo(tokenId, salePrice)` - Calculate royalty for a sale
- `SetDefaultRoyalty(receiver, feeNumerator)` - Set default royalty rate
- `SetTokenRoyalty(tokenId, receiver, feeNumerator)` - Set token-specific royalty
- `DeleteDefaultRoyalty()` - Remove default royalty
- `ResetTokenRoyalty(tokenId)` - Reset token royalty to default

### Marketplace Integration

- `ProcessSale(tokenId, salePrice, buyer)` - Handle marketplace sale with royalties
- `CalculateRoyalty(salePrice, royaltyRate)` - Calculate royalty amount
- `DistributeRoyalties(tokenId, salePrice)` - Distribute royalty payments

## Usage Examples

### Minting with Royalty

```csharp
// Create NFT with 5% royalty to creator
var royaltyInfo = new RoyaltyInfo
{
    Receiver = creatorAddress,
    RoyaltyRate = 500 // 5% (500 basis points out of 10000)
};

var tokenData = new TokenData
{
    Name = "Digital Artwork #1",
    Description = "A unique digital creation",
    Image = "https://example.com/artwork1.png",
    Creator = creatorAddress
};

var tokenId = Mint(collectorAddress, tokenData, royaltyInfo);
```

### Processing a Sale

```csharp
// Marketplace processes a sale with automatic royalty distribution
var salePrice = 1000_00000000; // 1000 GAS
var success = ProcessSale(tokenId, salePrice, buyerAddress);

// Royalties are automatically calculated and distributed
// Creator receives 5% = 50 GAS
// Seller receives 95% = 950 GAS
```

### Custom Royalty Configuration

```csharp
// Set different royalty rates for different tokens
SetTokenRoyalty(artistToken, artistAddress, 1000); // 10% for premium art
SetTokenRoyalty(utilityToken, daoAddress, 250);    // 2.5% for utility NFTs

// Query royalty information
var (receiver, royaltyAmount) = GetRoyaltyInfo(tokenId, salePrice);
```

## Royalty Implementation

### Royalty Calculation
```csharp
public static (UInt160 receiver, BigInteger royaltyAmount) GetRoyaltyInfo(ByteString tokenId, BigInteger salePrice)
{
    // Get token-specific royalty or fall back to default
    var royaltyInfo = GetTokenRoyalty(tokenId) ?? GetDefaultRoyalty();
    
    if (royaltyInfo == null)
        return (UInt160.Zero, 0);
    
    // Calculate royalty amount (rate is in basis points)
    var royaltyAmount = (salePrice * royaltyInfo.RoyaltyRate) / 10000;
    
    return (royaltyInfo.Receiver, royaltyAmount);
}
```

### Automatic Distribution
```csharp
public static bool ProcessSale(ByteString tokenId, BigInteger salePrice, UInt160 buyer)
{
    var owner = OwnerOf(tokenId);
    
    // Calculate and distribute royalties
    var (royaltyReceiver, royaltyAmount) = GetRoyaltyInfo(tokenId, salePrice);
    
    if (royaltyAmount > 0 && royaltyReceiver.IsValid)
    {
        // Transfer royalty to creator
        GAS.Transfer(buyer, royaltyReceiver, royaltyAmount, null);
        
        // Transfer remaining amount to seller
        var sellerAmount = salePrice - royaltyAmount;
        GAS.Transfer(buyer, owner, sellerAmount, null);
    }
    else
    {
        // No royalty, full amount to seller
        GAS.Transfer(buyer, owner, salePrice, null);
    }
    
    // Transfer NFT to buyer
    Transfer(owner, buyer, tokenId, null);
    
    return true;
}
```

## Marketplace Integration

### DEX/Marketplace Contract
```csharp
// Example marketplace integration
public static bool ListForSale(ByteString tokenId, BigInteger price)
{
    var owner = NFTContract.OwnerOf(tokenId);
    if (!Runtime.CheckWitness(owner))
        return false;
    
    // Store listing information
    var listing = new Listing
    {
        TokenId = tokenId,
        Price = price,
        Seller = owner,
        ListedAt = Runtime.Time
    };
    
    Storage.Put(Storage.CurrentContext, GetListingKey(tokenId), StdLib.Serialize(listing));
    return true;
}

public static bool ExecuteSale(ByteString tokenId)
{
    var listing = GetListing(tokenId);
    var buyer = (Transaction)Runtime.ScriptContainer.Sender;
    
    // Process sale with automatic royalty distribution
    return NFTContract.ProcessSale(tokenId, listing.Price, buyer);
}
```

## Royalty Standards

### EIP-2981 Compatibility
This implementation is compatible with the EIP-2981 royalty standard, making it interoperable with Ethereum-based marketplaces and tools.

### Basis Points System
- Royalty rates are expressed in basis points (1/100th of a percent)
- 100 basis points = 1%
- 10000 basis points = 100%
- Maximum recommended royalty: 1000 basis points (10%)

## Security Considerations

### Royalty Validation
- **Rate Limits**: Prevent excessive royalty rates (>50%)
- **Address Validation**: Ensure royalty recipients are valid addresses
- **Overflow Protection**: Prevent arithmetic overflow in calculations
- **Authorization**: Only authorized parties can set royalties

### Marketplace Security
- **Front-running Protection**: Use proper ordering for sale execution
- **Reentrancy Guards**: Prevent reentrancy attacks during transfers
- **Price Validation**: Validate sale prices and prevent manipulation

## Testing

The example includes comprehensive tests for:

- **Basic NFT Operations**: Minting, transferring, burning
- **Royalty Calculations**: Various royalty rates and scenarios
- **Marketplace Integration**: Sale processing and royalty distribution
- **Edge Cases**: Zero royalties, maximum rates, invalid addresses
- **Security Tests**: Unauthorized access, overflow protection

Run tests with:
```bash
cd Example.SmartContract.SampleRoyaltyNEP11Token.UnitTests
dotnet test
```

## Economic Models

### Creator Revenue
```csharp
// Example: Artist mints 100 NFTs with 5% royalty
// Each subsequent sale generates 5% revenue to the artist
// If total secondary sales = 10,000 GAS, artist earns 500 GAS
```

### Platform Fees
```csharp
// Marketplaces can implement additional platform fees
public static BigInteger CalculatePlatformFee(BigInteger salePrice)
{
    return (salePrice * PLATFORM_FEE_RATE) / 10000; // e.g., 2.5%
}
```

## Deployment Considerations

- **Gas Costs**: Royalty processing adds gas overhead to transfers
- **Rate Governance**: Consider governance mechanisms for royalty rates
- **Marketplace Adoption**: Ensure marketplace partners support royalties
- **Creator Onboarding**: Provide tools for creators to set royalties

## Related Standards

- **NEP-11**: Base NFT standard implementation
- **EIP-2981**: Ethereum royalty standard for compatibility
- **NEP-17**: For royalty token payments

## Related Examples

- [NEP-11 Basic](Example.SmartContract.NFT/) - Basic NFT implementation
- [NEP-17 Token](Example.SmartContract.NEP17/) - Fungible token for payments
- [Transfer Example](Example.SmartContract.Transfer/) - Transfer patterns

This example demonstrates how to create a sustainable creator economy on NEO with built-in royalty mechanisms that benefit artists, collectors, and platforms.