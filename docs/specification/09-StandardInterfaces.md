# 10. Standard Interfaces (NEP Standards)

Neo Enhancement Proposals (NEPs) define standard interfaces for smart contract interoperability.

## 10.1 NEP-17 (Fungible Tokens)

Standard interface for fungible tokens (currencies, utility tokens).

### 10.1.1 Required Interface

| Method | Parameters | Return | Description |
|:-------|:-----------|:-------|:------------|
| `symbol` | None | `string` | Token symbol |
| `decimals` | None | `byte` | Decimal places |
| `totalSupply` | None | `BigInteger` | Total supply |
| `balanceOf` | `account` | `BigInteger` | Account balance |
| `transfer` | `from`, `to`, `amount`, `data?` | `bool` | Transfer tokens |

**Required Event:**
```csharp
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger> OnTransfer;
```

### 10.1.2 Simple Implementation

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

[DisplayName("SimpleToken")]
[SupportedStandards(NepStandard.Nep17)]
[ContractPermission(Permission.Any, Method.Any)]
public class SimpleToken : SmartContract
{
    [DisplayName("Transfer")]
    public static event Action<UInt160, UInt160, BigInteger> OnTransfer;

    private static readonly byte[] PrefixBalance = new byte[] { 0x01 };

    [Safe]
    [DisplayName("symbol")]
    public static string Symbol() => "STK";

    [Safe]
    [DisplayName("decimals")]
    public static byte Decimals() => 8;

    [Safe]
    [DisplayName("totalSupply")]
    public static BigInteger TotalSupply() => 1000000;

    [Safe]
    [DisplayName("balanceOf")]
    public static BigInteger BalanceOf(UInt160 account)
    {
        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        return (BigInteger)balances.Get(account);
    }

    [DisplayName("transfer")]
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null)
    {
        if (!Runtime.CheckWitness(from)) return false;
        if (amount <= 0) return false;

        StorageMap balances = new StorageMap(Storage.CurrentContext, PrefixBalance);
        BigInteger fromBalance = (BigInteger)balances.Get(from);
        if (fromBalance < amount) return false;

        balances.Put(from, fromBalance - amount);
        balances.Put(to, (BigInteger)balances.Get(to) + amount);

        OnTransfer(from, to, amount);

        // Call recipient if it's a contract
        if (ContractManagement.GetContract(to) != null)
            Contract.Call(to, "onNEP17Payment", CallFlags.All, new object[] { from, amount, data });

        return true;
    }

    // Payment callback for receiving tokens
    public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        // Handle incoming token payment
    }
}
```

## 10.2 NEP-11 (Non-Fungible Tokens)

Standard interface for unique, non-interchangeable tokens (NFTs).

### 10.2.1 Required Interface

| Method | Parameters | Return | Description |
|:-------|:-----------|:-------|:------------|
| `symbol` | None | `string` | Token symbol |
| `decimals` | None | `byte` | Always 0 for NFTs |
| `totalSupply` | None | `BigInteger` | Total NFT count |
| `balanceOf` | `account` | `BigInteger` | NFTs owned by account |
| `ownerOf` | `tokenId` | `UInt160` | Owner of specific NFT |
| `transfer` | `to`, `tokenId`, `data?` | `bool` | Transfer NFT |

**Required Event:**
```csharp
[DisplayName("Transfer")]
public static event Action<UInt160, UInt160, BigInteger, ByteString> OnTransfer;
```

### 10.2.2 Simple Implementation

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.Numerics;

[DisplayName("SimpleNFT")]
[SupportedStandards(NepStandard.Nep11)]
[ContractPermission(Permission.Any, Method.Any)]
public class SimpleNFT : SmartContract
{
    [DisplayName("Transfer")]
    public static event Action<UInt160, UInt160, BigInteger, ByteString> OnTransfer;

    private static readonly byte[] PrefixOwner = new byte[] { 0x01 };

    [Safe]
    [DisplayName("symbol")]
    public static string Symbol() => "SNFT";

    [Safe]
    [DisplayName("decimals")]
    public static byte Decimals() => 0;

    [Safe]
    [DisplayName("ownerOf")]
    public static UInt160 OwnerOf(ByteString tokenId)
    {
        StorageMap owners = new StorageMap(Storage.CurrentContext, PrefixOwner);
        return (UInt160)owners.Get(tokenId);
    }

    [DisplayName("transfer")]
    public static bool Transfer(UInt160 to, ByteString tokenId, object data = null)
    {
        UInt160 from = OwnerOf(tokenId);
        if (from == null) return false;
        if (!Runtime.CheckWitness(from)) return false;

        StorageMap owners = new StorageMap(Storage.CurrentContext, PrefixOwner);
        owners.Put(tokenId, to);

        OnTransfer(from, to, 1, tokenId);

        // Call recipient if it's a contract
        if (ContractManagement.GetContract(to) != null)
            Contract.Call(to, "onNEP11Payment", CallFlags.All, new object[] { from, 1, tokenId, data });

        return true;
    }

    public static ByteString Mint(UInt160 to, string name)
    {
        ByteString tokenId = CryptoLib.Sha256(name);

        StorageMap owners = new StorageMap(Storage.CurrentContext, PrefixOwner);
        owners.Put(tokenId, to);

        OnTransfer(null, to, 1, tokenId);
        return tokenId;
    }

    // Payment callback for receiving NFTs
    public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
    {
        // Handle incoming NFT
    }
}
```

## 10.3 Other NEP Standards

### 10.3.1 NEP-24 (NFT Royalties)

Defines royalty payments for NFT sales.

**Required Method:**
```csharp
[Safe]
[DisplayName("royaltyInfo")]
public static RoyaltyInfo RoyaltyInfo(ByteString tokenId, BigInteger salePrice)
{
    // Calculate royalty (e.g., 10% of sale price)
    BigInteger royaltyAmount = salePrice * 10 / 100;
    UInt160 creator = GetCreator(tokenId);
    return new RoyaltyInfo { Receiver = creator, Amount = royaltyAmount };
}

public class RoyaltyInfo
{
    public UInt160 Receiver;
    public BigInteger Amount;
    public RoyaltyInfo() { }
}
```

### 10.3.2 Payment Callbacks

**NEP-17 Payment Callback:**
```csharp
public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
{
    // Handle fungible token payment
    UInt160 token = Runtime.CallingScriptHash;
    // Process payment...
}
```

**NEP-11 Payment Callback:**
```csharp
public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
{
    // Handle NFT payment (amount is always 1)
    UInt160 nftContract = Runtime.CallingScriptHash;
    // Process NFT...
}
```

## 10.4 Standard Implementation Tips

### 10.4.1 Multiple Standards

Implement multiple standards by combining interfaces:

```csharp
[SupportedStandards(NepStandard.Nep11, NepStandard.Nep24)]
public class RoyaltyNFT : SmartContract
{
    // Implement both NEP-11 and NEP-24 methods
}
```

### 10.4.2 Standard Verification

Check if a contract implements a standard:

```csharp
public static bool IsNEP17(UInt160 contractHash)
{
    try
    {
        string symbol = (string)Contract.Call(contractHash, "symbol", CallFlags.ReadStates);
        byte decimals = (byte)(BigInteger)Contract.Call(contractHash, "decimals", CallFlags.ReadStates);
        return true;
    }
    catch
    {
        return false;
    }
}
```

### 10.4.3 Deployment Callback

Handle contract initialization:

```csharp
public static void _deploy(object data, bool update)
{
    if (update) return; // Skip on updates

    UInt160 owner = (UInt160)data;
    Storage.Put(Storage.CurrentContext, "owner", owner);
}
```

### 10.4.4 Verification Callback

Enable contract as transaction signer:

```csharp
public static bool Verify()
{
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");
    return Runtime.CheckWitness(owner);
}
```
