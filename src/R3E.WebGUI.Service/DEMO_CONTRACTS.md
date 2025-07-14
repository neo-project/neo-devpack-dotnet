# Demo Contracts for R3E WebGUI Service

This document provides example contracts that showcase the capabilities of the R3E WebGUI Service. These contracts are designed to demonstrate different features and use cases of the automatic WebGUI generation system.

## 🪙 1. Simple NEP-17 Token Contract

### Contract: BasicToken.cs

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("BasicToken")]
[ManifestExtra("Author", "R3E Community")]
[ManifestExtra("Email", "info@r3e.network")]
[ManifestExtra("Description", "A simple NEP-17 token demonstrating R3E WebGUI capabilities")]
[ManifestExtra("Version", "1.0.0")]
public class BasicToken : SmartContract
{
    // Token settings
    private static readonly string TokenName = "Basic Token";
    private static readonly string TokenSymbol = "BASIC";
    private static readonly byte TokenDecimals = 8;
    private static readonly BigInteger InitialSupply = 1_000_000_00000000; // 1M tokens

    // Storage keys
    private static readonly string TotalSupplyKey = "totalSupply";
    private static readonly string OwnerKey = "owner";

    [InitialValue("NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5", ContractParameterType.Hash160)]
    private static readonly UInt160 InitialOwner = default;

    public static void _deploy(object data, bool update)
    {
        if (update) return;
        
        Storage.Put(Storage.CurrentContext, TotalSupplyKey, InitialSupply);
        Storage.Put(Storage.CurrentContext, OwnerKey, InitialOwner);
        Storage.Put(Storage.CurrentContext, InitialOwner, InitialSupply);
        
        OnTransfer(null, InitialOwner, InitialSupply);
    }

    [DisplayName("Transfer")]
    public static event Action<UInt160?, UInt160?, BigInteger> OnTransfer;

    [Safe]
    public static string Symbol() => TokenSymbol;

    [Safe]
    public static string Name() => TokenName;

    [Safe]
    public static byte Decimals() => TokenDecimals;

    [Safe]
    public static BigInteger TotalSupply()
    {
        return (BigInteger)Storage.Get(Storage.CurrentContext, TotalSupplyKey);
    }

    [Safe]
    public static BigInteger BalanceOf(UInt160 account)
    {
        if (!account.IsValid) throw new Exception("Invalid account");
        return (BigInteger)Storage.Get(Storage.CurrentContext, account);
    }

    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
    {
        if (!from.IsValid || !to.IsValid) throw new Exception("Invalid addresses");
        if (amount < 0) throw new Exception("Invalid amount");
        if (!Runtime.CheckWitness(from)) throw new Exception("Unauthorized");

        var fromBalance = BalanceOf(from);
        if (fromBalance < amount) throw new Exception("Insufficient balance");

        if (amount > 0)
        {
            var toBalance = BalanceOf(to);
            Storage.Put(Storage.CurrentContext, from, fromBalance - amount);
            Storage.Put(Storage.CurrentContext, to, toBalance + amount);
        }

        OnTransfer(from, to, amount);
        return true;
    }

    [Safe]
    public static UInt160 GetOwner()
    {
        return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
    }

    public static bool Mint(UInt160 to, BigInteger amount)
    {
        if (!Runtime.CheckWitness(GetOwner())) throw new Exception("Unauthorized");
        if (!to.IsValid || amount < 0) throw new Exception("Invalid parameters");

        var totalSupply = TotalSupply();
        var toBalance = BalanceOf(to);

        Storage.Put(Storage.CurrentContext, TotalSupplyKey, totalSupply + amount);
        Storage.Put(Storage.CurrentContext, to, toBalance + amount);

        OnTransfer(null, to, amount);
        return true;
    }

    public static bool Burn(UInt160 from, BigInteger amount)
    {
        if (!Runtime.CheckWitness(from)) throw new Exception("Unauthorized");
        if (!from.IsValid || amount < 0) throw new Exception("Invalid parameters");

        var fromBalance = BalanceOf(from);
        if (fromBalance < amount) throw new Exception("Insufficient balance");

        var totalSupply = TotalSupply();
        Storage.Put(Storage.CurrentContext, TotalSupplyKey, totalSupply - amount);
        Storage.Put(Storage.CurrentContext, from, fromBalance - amount);

        OnTransfer(from, null, amount);
        return true;
    }
}
```

### WebGUI Features Demonstrated
- **Token Information Display**: Name, symbol, decimals, total supply
- **Balance Checking**: Real-time balance queries
- **Transfer Functionality**: Wallet-based token transfers
- **Minting/Burning**: Owner-only operations
- **Event Monitoring**: Real-time transfer events

### Deployment Command
```bash
./deploy-contract-webgui.sh \
  -p BasicToken.csproj \
  -a 0x1234567890abcdef1234567890abcdef12345678 \
  -d NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5 \
  -n "BasicToken" \
  -e "Simple NEP-17 token showcasing R3E WebGUI capabilities"
```

## 🏪 2. Marketplace Contract

### Contract: SimpleMarketplace.cs

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("SimpleMarketplace")]
[ManifestExtra("Author", "R3E Community")]
[ManifestExtra("Description", "A simple marketplace demonstrating advanced WebGUI features")]
public class SimpleMarketplace : SmartContract
{
    // Events
    [DisplayName("ItemListed")]
    public static event Action<ByteString, UInt160, BigInteger, string> OnItemListed;

    [DisplayName("ItemSold")]
    public static event Action<ByteString, UInt160, UInt160, BigInteger> OnItemSold;

    [DisplayName("ItemRemoved")]
    public static event Action<ByteString, UInt160> OnItemRemoved;

    // Storage prefixes
    private static readonly byte[] ItemPrefix = "item".ToByteArray();
    private static readonly byte[] OwnerPrefix = "owner".ToByteArray();

    public static bool ListItem(ByteString itemId, BigInteger price, string description)
    {
        if (price <= 0) throw new Exception("Price must be positive");
        if (description.Length > 1000) throw new Exception("Description too long");

        var seller = (UInt160)Runtime.ExecutingScriptHash;
        if (!Runtime.CheckWitness(seller)) throw new Exception("Unauthorized");

        var itemKey = ItemPrefix.Concat(itemId);
        var existingItem = Storage.Get(Storage.CurrentContext, itemKey);
        if (existingItem != null) throw new Exception("Item already exists");

        // Store item data
        var itemData = StdLib.Serialize(new object[] { seller, price, description, Runtime.Time });
        Storage.Put(Storage.CurrentContext, itemKey, itemData);

        // Store owner mapping
        var ownerKey = OwnerPrefix.Concat(seller).Concat(itemId);
        Storage.Put(Storage.CurrentContext, ownerKey, 1);

        OnItemListed(itemId, seller, price, description);
        return true;
    }

    public static bool BuyItem(ByteString itemId, UInt160 buyer)
    {
        if (!buyer.IsValid) throw new Exception("Invalid buyer");
        if (!Runtime.CheckWitness(buyer)) throw new Exception("Unauthorized");

        var itemKey = ItemPrefix.Concat(itemId);
        var itemData = Storage.Get(Storage.CurrentContext, itemKey);
        if (itemData == null) throw new Exception("Item not found");

        var item = (object[])StdLib.Deserialize(itemData);
        var seller = (UInt160)item[0];
        var price = (BigInteger)item[1];

        if (buyer == seller) throw new Exception("Cannot buy your own item");

        // Remove item from storage
        Storage.Delete(Storage.CurrentContext, itemKey);
        var ownerKey = OwnerPrefix.Concat(seller).Concat(itemId);
        Storage.Delete(Storage.CurrentContext, ownerKey);

        OnItemSold(itemId, seller, buyer, price);
        return true;
    }

    public static bool RemoveItem(ByteString itemId)
    {
        var itemKey = ItemPrefix.Concat(itemId);
        var itemData = Storage.Get(Storage.CurrentContext, itemKey);
        if (itemData == null) throw new Exception("Item not found");

        var item = (object[])StdLib.Deserialize(itemData);
        var seller = (UInt160)item[0];

        if (!Runtime.CheckWitness(seller)) throw new Exception("Unauthorized");

        Storage.Delete(Storage.CurrentContext, itemKey);
        var ownerKey = OwnerPrefix.Concat(seller).Concat(itemId);
        Storage.Delete(Storage.CurrentContext, ownerKey);

        OnItemRemoved(itemId, seller);
        return true;
    }

    [Safe]
    public static object[] GetItem(ByteString itemId)
    {
        var itemKey = ItemPrefix.Concat(itemId);
        var itemData = Storage.Get(Storage.CurrentContext, itemKey);
        if (itemData == null) return new object[0];

        return (object[])StdLib.Deserialize(itemData);
    }

    [Safe]
    public static object[] GetUserItems(UInt160 user, int offset = 0, int limit = 10)
    {
        if (!user.IsValid) throw new Exception("Invalid user");
        
        var results = new List<object>();
        var prefix = OwnerPrefix.Concat(user);
        
        var iterator = Storage.Find(Storage.CurrentContext, prefix, FindOptions.RemovePrefix);
        var count = 0;
        
        while (iterator.Next() && count < limit)
        {
            if (count >= offset)
            {
                var itemId = iterator.Key;
                var item = GetItem(itemId);
                if (item.Length > 0)
                {
                    results.Add(new object[] { itemId, item });
                }
            }
            count++;
        }
        
        return results.ToArray();
    }

    [Safe]
    public static object[] GetAllItems(int offset = 0, int limit = 20)
    {
        var results = new List<object>();
        var iterator = Storage.Find(Storage.CurrentContext, ItemPrefix, FindOptions.RemovePrefix);
        var count = 0;
        
        while (iterator.Next() && count < limit)
        {
            if (count >= offset)
            {
                var itemId = iterator.Key;
                var itemData = (object[])StdLib.Deserialize(iterator.Value);
                results.Add(new object[] { itemId, itemData });
            }
            count++;
        }
        
        return results.ToArray();
    }
}
```

### WebGUI Features Demonstrated
- **Complex Forms**: Multi-field item listing
- **Data Tables**: Item listings with pagination
- **User-Specific Views**: Personal item management
- **Search/Filter**: Item discovery functionality
- **Real-time Updates**: Live marketplace activity

## 🗳️ 3. Voting Contract

### Contract: SimpleVoting.cs

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("SimpleVoting")]
[ManifestExtra("Author", "R3E Community")]
[ManifestExtra("Description", "A voting contract demonstrating governance WebGUI features")]
public class SimpleVoting : SmartContract
{
    // Events
    [DisplayName("ProposalCreated")]
    public static event Action<BigInteger, UInt160, string, BigInteger> OnProposalCreated;

    [DisplayName("VoteCast")]
    public static event Action<BigInteger, UInt160, bool> OnVoteCast;

    [DisplayName("ProposalFinalized")]
    public static event Action<BigInteger, bool, BigInteger, BigInteger> OnProposalFinalized;

    // Storage keys
    private static readonly string ProposalCountKey = "proposalCount";
    private static readonly byte[] ProposalPrefix = "proposal".ToByteArray();
    private static readonly byte[] VotePrefix = "vote".ToByteArray();

    public static BigInteger CreateProposal(string title, string description, BigInteger duration)
    {
        if (title.Length > 100) throw new Exception("Title too long");
        if (description.Length > 1000) throw new Exception("Description too long");
        if (duration < 3600) throw new Exception("Duration must be at least 1 hour");

        var creator = (UInt160)Runtime.ExecutingScriptHash;
        if (!Runtime.CheckWitness(creator)) throw new Exception("Unauthorized");

        var proposalId = GetProposalCount() + 1;
        Storage.Put(Storage.CurrentContext, ProposalCountKey, proposalId);

        var endTime = Runtime.Time + duration;
        var proposalData = StdLib.Serialize(new object[] 
        { 
            creator, title, description, Runtime.Time, endTime, 0, 0, false 
        });
        
        var proposalKey = ProposalPrefix.Concat(proposalId.ToByteArray());
        Storage.Put(Storage.CurrentContext, proposalKey, proposalData);

        OnProposalCreated(proposalId, creator, title, endTime);
        return proposalId;
    }

    public static bool Vote(BigInteger proposalId, bool support)
    {
        var voter = (UInt160)Runtime.ExecutingScriptHash;
        if (!Runtime.CheckWitness(voter)) throw new Exception("Unauthorized");

        var proposal = GetProposal(proposalId);
        if (proposal.Length == 0) throw new Exception("Proposal not found");

        var endTime = (BigInteger)proposal[4];
        var isFinalized = (bool)proposal[7];

        if (Runtime.Time > endTime) throw new Exception("Voting period ended");
        if (isFinalized) throw new Exception("Proposal already finalized");

        // Check if already voted
        var voteKey = VotePrefix.Concat(proposalId.ToByteArray()).Concat(voter);
        var existingVote = Storage.Get(Storage.CurrentContext, voteKey);
        if (existingVote != null) throw new Exception("Already voted");

        // Record vote
        Storage.Put(Storage.CurrentContext, voteKey, support ? 1 : 0);

        // Update proposal vote counts
        var yesVotes = (BigInteger)proposal[5];
        var noVotes = (BigInteger)proposal[6];

        if (support)
            yesVotes += 1;
        else
            noVotes += 1;

        var updatedProposal = new object[] 
        { 
            proposal[0], proposal[1], proposal[2], proposal[3], 
            proposal[4], yesVotes, noVotes, proposal[7] 
        };
        
        var proposalKey = ProposalPrefix.Concat(proposalId.ToByteArray());
        Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(updatedProposal));

        OnVoteCast(proposalId, voter, support);
        return true;
    }

    public static bool FinalizeProposal(BigInteger proposalId)
    {
        var proposal = GetProposal(proposalId);
        if (proposal.Length == 0) throw new Exception("Proposal not found");

        var endTime = (BigInteger)proposal[4];
        var yesVotes = (BigInteger)proposal[5];
        var noVotes = (BigInteger)proposal[6];
        var isFinalized = (bool)proposal[7];

        if (Runtime.Time <= endTime) throw new Exception("Voting period not ended");
        if (isFinalized) throw new Exception("Already finalized");

        var passed = yesVotes > noVotes;
        
        var finalizedProposal = new object[] 
        { 
            proposal[0], proposal[1], proposal[2], proposal[3], 
            proposal[4], proposal[5], proposal[6], true 
        };
        
        var proposalKey = ProposalPrefix.Concat(proposalId.ToByteArray());
        Storage.Put(Storage.CurrentContext, proposalKey, StdLib.Serialize(finalizedProposal));

        OnProposalFinalized(proposalId, passed, yesVotes, noVotes);
        return true;
    }

    [Safe]
    public static BigInteger GetProposalCount()
    {
        return (BigInteger)Storage.Get(Storage.CurrentContext, ProposalCountKey);
    }

    [Safe]
    public static object[] GetProposal(BigInteger proposalId)
    {
        var proposalKey = ProposalPrefix.Concat(proposalId.ToByteArray());
        var proposalData = Storage.Get(Storage.CurrentContext, proposalKey);
        if (proposalData == null) return new object[0];

        return (object[])StdLib.Deserialize(proposalData);
    }

    [Safe]
    public static bool HasVoted(BigInteger proposalId, UInt160 voter)
    {
        var voteKey = VotePrefix.Concat(proposalId.ToByteArray()).Concat(voter);
        return Storage.Get(Storage.CurrentContext, voteKey) != null;
    }

    [Safe]
    public static object[] GetActiveProposals()
    {
        var results = new List<object>();
        var proposalCount = GetProposalCount();
        
        for (BigInteger i = 1; i <= proposalCount; i++)
        {
            var proposal = GetProposal(i);
            if (proposal.Length > 0)
            {
                var endTime = (BigInteger)proposal[4];
                var isFinalized = (bool)proposal[7];
                
                if (Runtime.Time <= endTime && !isFinalized)
                {
                    results.Add(new object[] { i, proposal });
                }
            }
        }
        
        return results.ToArray();
    }
}
```

### WebGUI Features Demonstrated
- **Governance Interface**: Proposal creation and management
- **Real-time Voting**: Live vote tracking and updates
- **Time-based Logic**: Countdown timers and deadline management
- **User Participation**: Personal voting history
- **Results Display**: Vote tallies and outcome visualization

## 🎮 4. NFT Collection Contract

### Contract: SimpleNFT.cs

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("SimpleNFT")]
[ManifestExtra("Author", "R3E Community")]
[ManifestExtra("Description", "A simple NFT contract showcasing multimedia WebGUI features")]
[SupportedStandards("NEP-11")]
public class SimpleNFT : SmartContract
{
    // Events
    [DisplayName("Transfer")]
    public static event Action<UInt160?, UInt160?, BigInteger, ByteString> OnTransfer;

    [DisplayName("TokenMinted")]
    public static event Action<UInt160, ByteString, string> OnTokenMinted;

    // Storage prefixes
    private static readonly byte[] TokenPrefix = "token".ToByteArray();
    private static readonly byte[] OwnerPrefix = "owner".ToByteArray();
    private static readonly byte[] MetadataPrefix = "metadata".ToByteArray();
    private static readonly string TotalSupplyKey = "totalSupply";

    [Safe]
    public static string Symbol() => "SNFT";

    [Safe]
    public static byte Decimals() => 0;

    [Safe]
    public static BigInteger TotalSupply()
    {
        return (BigInteger)Storage.Get(Storage.CurrentContext, TotalSupplyKey);
    }

    [Safe]
    public static UInt160 OwnerOf(ByteString tokenId)
    {
        var ownerKey = OwnerPrefix.Concat(tokenId);
        return (UInt160)Storage.Get(Storage.CurrentContext, ownerKey);
    }

    [Safe]
    public static object[] Properties(ByteString tokenId)
    {
        var metadataKey = MetadataPrefix.Concat(tokenId);
        var metadata = Storage.Get(Storage.CurrentContext, metadataKey);
        if (metadata == null) return new object[0];

        return (object[])StdLib.Deserialize(metadata);
    }

    [Safe]
    public static Iterator Tokens()
    {
        return Storage.Find(Storage.CurrentContext, TokenPrefix, FindOptions.RemovePrefix);
    }

    [Safe]
    public static Iterator TokensOf(UInt160 owner)
    {
        if (!owner.IsValid) throw new Exception("Invalid owner");
        
        var results = new List<ByteString>();
        var iterator = Storage.Find(Storage.CurrentContext, OwnerPrefix, FindOptions.RemovePrefix);
        
        while (iterator.Next())
        {
            if ((UInt160)iterator.Value == owner)
            {
                results.Add(iterator.Key);
            }
        }
        
        return results.ToArray();
    }

    public static bool Transfer(UInt160 to, ByteString tokenId, object data)
    {
        if (!to.IsValid) throw new Exception("Invalid recipient");

        var owner = OwnerOf(tokenId);
        if (owner == null) throw new Exception("Token not found");
        if (!Runtime.CheckWitness(owner)) throw new Exception("Unauthorized");

        if (owner != to)
        {
            var ownerKey = OwnerPrefix.Concat(tokenId);
            Storage.Put(Storage.CurrentContext, ownerKey, to);
        }

        OnTransfer(owner, to, 1, tokenId);
        return true;
    }

    public static ByteString Mint(UInt160 to, string name, string description, string imageUrl)
    {
        if (!to.IsValid) throw new Exception("Invalid recipient");
        if (name.Length > 100) throw new Exception("Name too long");
        if (description.Length > 1000) throw new Exception("Description too long");
        if (imageUrl.Length > 500) throw new Exception("Image URL too long");

        // Simple authorization - in real contracts, implement proper minting rules
        if (!Runtime.CheckWitness(to)) throw new Exception("Unauthorized");

        var totalSupply = TotalSupply();
        var tokenId = (totalSupply + 1).ToByteArray();

        // Store token existence
        var tokenKey = TokenPrefix.Concat(tokenId);
        Storage.Put(Storage.CurrentContext, tokenKey, 1);

        // Store ownership
        var ownerKey = OwnerPrefix.Concat(tokenId);
        Storage.Put(Storage.CurrentContext, ownerKey, to);

        // Store metadata
        var metadata = new object[] { name, description, imageUrl, Runtime.Time };
        var metadataKey = MetadataPrefix.Concat(tokenId);
        Storage.Put(Storage.CurrentContext, metadataKey, StdLib.Serialize(metadata));

        // Update total supply
        Storage.Put(Storage.CurrentContext, TotalSupplyKey, totalSupply + 1);

        OnTokenMinted(to, tokenId, name);
        OnTransfer(null, to, 1, tokenId);
        
        return tokenId;
    }

    [Safe]
    public static object[] GetTokenDetails(ByteString tokenId)
    {
        var owner = OwnerOf(tokenId);
        if (owner == null) return new object[0];

        var properties = Properties(tokenId);
        return new object[] { tokenId, owner, properties };
    }

    [Safe]
    public static object[] GetCollection(int offset = 0, int limit = 20)
    {
        var results = new List<object>();
        var iterator = Storage.Find(Storage.CurrentContext, TokenPrefix, FindOptions.RemovePrefix);
        var count = 0;
        
        while (iterator.Next() && count < limit)
        {
            if (count >= offset)
            {
                var tokenId = iterator.Key;
                var details = GetTokenDetails(tokenId);
                if (details.Length > 0)
                {
                    results.Add(details);
                }
            }
            count++;
        }
        
        return results.ToArray();
    }
}
```

### WebGUI Features Demonstrated
- **NFT Gallery**: Visual display of token collection
- **Image/Media Display**: Multimedia content rendering
- **Transfer Interface**: NFT ownership management
- **Minting Interface**: Create new tokens with metadata
- **Collection Browser**: Paginated NFT exploration

## 📊 Demo Deployment Guide

### 1. Preparation
```bash
# Clone the repository
git clone https://github.com/neo-project/neo-devpack-dotnet.git
cd neo-devpack-dotnet

# Start the WebGUI Service
cd src/R3E.WebGUI.Service
docker-compose up -d
```

### 2. Deploy Demo Contracts
```bash
# Deploy all demo contracts
./scripts/deploy-demos.sh

# Or deploy individually
./deploy-contract-webgui.sh -p demos/BasicToken.csproj -a 0xABC... -d NPvK...
./deploy-contract-webgui.sh -p demos/SimpleMarketplace.csproj -a 0xDEF... -d NPvK...
./deploy-contract-webgui.sh -p demos/SimpleVoting.csproj -a 0x123... -d NPvK...
./deploy-contract-webgui.sh -p demos/SimpleNFT.csproj -a 0x456... -d NPvK...
```

### 3. Access Demo WebGUIs
- **Basic Token**: http://basictoken.localhost:8888
- **Marketplace**: http://simplemarketplace.localhost:8888
- **Voting**: http://simplevoting.localhost:8888
- **NFT Collection**: http://simplenft.localhost:8888

### 4. Testing Scenarios

#### Basic Token Demo
1. Connect wallet
2. Check token balance
3. Transfer tokens to another address
4. View transfer history
5. (Owner) Mint additional tokens

#### Marketplace Demo
1. List an item for sale
2. Browse available items
3. Purchase an item
4. View your listed items
5. Remove an item from sale

#### Voting Demo
1. Create a new proposal
2. Vote on active proposals
3. View voting results
4. Finalize completed proposals
5. Browse proposal history

#### NFT Demo
1. Mint a new NFT with metadata
2. View the NFT collection
3. Transfer NFT ownership
4. Browse NFT details
5. View personal NFT collection

## 🎯 WebGUI Features Showcased

### Automatic Generation
- **Contract Parsing**: Manifest-based interface generation
- **Method Categorization**: Smart grouping of contract functions
- **Parameter Validation**: Type-based input validation
- **Return Value Display**: Formatted result presentation

### User Experience
- **Wallet Integration**: Seamless connection to Neo wallets
- **Real-time Updates**: Live blockchain data synchronization
- **Responsive Design**: Mobile-friendly interfaces
- **Professional Styling**: Modern, clean visual design

### Advanced Features
- **Event Monitoring**: Real-time contract event display
- **Transaction History**: Complete interaction logs
- **Error Handling**: User-friendly error messages
- **Gas Estimation**: Transaction cost prediction

### Developer Benefits
- **Zero Frontend Code**: Automatic interface generation
- **Consistent Design**: Standardized professional appearance
- **Rapid Deployment**: Minutes from contract to WebGUI
- **Easy Customization**: JSON-based configuration system

These demo contracts provide comprehensive examples of the R3E WebGUI Service capabilities, showcasing how different types of smart contracts can benefit from automatic, professional web interface generation.