# 12. Appendices

## 12.1 Supported C# Features

### 12.1.1 Supported Types and Operations

| Category | Supported Features |
|:---------|:-------------------|
| **Basic Types** | `bool`, `byte`, `int`, `long`, `string`, `BigInteger`, `UInt160`, `UInt256`, `ECPoint`, `ByteString` |
| **Operators** | Arithmetic (`+`, `-`, `*`, `/`), Comparison (`==`, `!=`, `<`, `>`), Logical (`&&`, `||`, `!`) |
| **Control Flow** | `if/else`, `switch`, `for`, `foreach`, `while`, `try/catch`, `return`, `throw` |
| **Collections** | Arrays, Lists, Dictionaries |
| **Classes** | Classes, structs, methods, properties, events, inheritance |
| **Attributes** | Custom attributes, contract attributes |

### 12.1.2 Key Unsupported Features

| Category | Unsupported Features |
|:---------|:--------------------|
| **Language** | Interfaces, LINQ, async/await, reflection, generics (limited) |
| **System** | File I/O, networking, threading, database access |
| **Advanced** | Pointers, unsafe code, operator overloading, indexers |

### 12.1.3 Neo-Specific Types

```csharp
// Neo blockchain types
UInt160 scriptHash;     // 20-byte script hash (addresses)
UInt256 transactionHash; // 32-byte transaction hash
ECPoint publicKey;      // Elliptic curve point (public keys)
ByteString data;        // Immutable byte array
BigInteger amount;      // Arbitrary precision integer
```

## 12.2 Native Contract Reference

### 12.2.1 Core Contracts

| Contract | Purpose | Key Methods |
|:---------|:--------|:------------|
| **ContractManagement** | Deploy/update contracts | `Deploy`, `Update`, `Destroy`, `GetContract` |
| **NEO** | NEO token operations | `BalanceOf`, `Transfer`, `Vote`, `UnclaimedGas` |
| **GAS** | GAS token operations | `BalanceOf`, `Transfer` |
| **StdLib** | Standard utilities | `Serialize`, `Deserialize`, `Base64Encode` |
| **CryptoLib** | Cryptographic functions | `Sha256`, `Ripemd160`, `VerifyWithECDsa` |

### 12.2.2 System Contracts

| Contract | Purpose | Key Methods |
|:---------|:--------|:------------|
| **Oracle** | External data access | `Request`, `GetPrice`, `SetPrice` |
| **Policy** | Blockchain policies | `GetFeePerByte`, `GetStoragePrice` |
| **RoleManagement** | Node role management | `GetDesignatedByRole`, `DesignateAsRole` |
| **Ledger** | Blockchain data access | `GetBlock`, `GetTransaction`, `CurrentIndex` |

### 12.2.3 Common Usage Examples

```csharp
// Token operations
BigInteger balance = NEO.BalanceOf(account);
bool success = GAS.Transfer(from, to, amount);

// Contract management
Contract contract = ContractManagement.GetContract(scriptHash);
ContractManagement.Deploy(nefFile, manifest, data);

// Utilities
ByteString serialized = StdLib.Serialize(obj);
byte[] hash = CryptoLib.Sha256(data);

// Oracle data request
Oracle.Request(url, filter, "callback", userData, gasForResponse);
```

## 12.3 Attribute Reference

### 12.3.1 Contract Attributes

| Attribute | Purpose | Example |
|:----------|:--------|:--------|
| `DisplayName` | Contract name | `[DisplayName("MyToken")]` |
| `ContractPermission` | Call permissions | `[ContractPermission(Permission.Any, Method.Any)]` |
| `SupportedStandards` | NEP standards | `[SupportedStandards(NepStandard.Nep17)]` |
| `NoReentrant` | Reentrancy protection | `[NoReentrant]` |

### 12.3.2 Method Attributes

| Attribute | Purpose | Example |
|:----------|:--------|:--------|
| `Safe` | Read-only method | `[Safe] public static string Symbol()` |
| `DisplayName` | Custom method name | `[DisplayName("balance_of")]` |
| `NoReentrantMethod` | Method reentrancy protection | `[NoReentrantMethod]` |

### 12.3.3 Field Attributes

| Attribute | Purpose | Example |
|:----------|:--------|:--------|
| `Hash160` | Initialize with address | `[Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]` |
| `ByteArray` | Initialize with hex data | `[ByteArray("0a0b0c0d")]` |
| `Integer` | Initialize with number | `[Integer(42)]` |

## 12.4 Compiler Features

### 12.4.1 Basic Commands

```bash
# Compile contract
nccs MyContract.cs

# Compile with debug info
nccs -d MyContract.cs

# Compile solution
nccs MySolution.sln

# Generate interface
nccs --generate-interface MyContract.cs
```

### 12.4.2 Optimization Levels

| Level | Description |
|:------|:------------|
| `None` | No optimization |
| `Basic` | Standard optimizations |
| `Experimental` | Advanced optimizations |

### 12.4.3 Security Analysis

The compiler automatically checks for:
- Reentrancy vulnerabilities
- Missing authorization checks
- Unsafe storage operations
- Update mechanism issues

## 12.5 Quick Reference

### 12.5.1 Common Gas Costs

| Operation | Cost (datoshi) |
|:----------|:--------------:|
| Basic instructions | 1 |
| Storage operations | 1,000+ |
| Contract calls | 1,000+ |
| Signature verification | 32,768 |
| Multi-signature (n sigs) | 32,768 × n |

### 12.5.2 Key Syscalls

| Category | Syscalls |
|:---------|:---------|
| **Runtime** | `CheckWitness`, `Log`, `Notify`, `GetTime` |
| **Storage** | `Get`, `Put`, `Delete`, `Find` |
| **Contract** | `Call`, `GetCallFlags` |
| **Crypto** | `CheckSig`, `CheckMultisig` |

### 12.5.3 Best Practices Summary

| Category | Key Points |
|:---------|:-----------|
| **Security** | Use `CheckWitness`, validate inputs, prevent reentrancy |
| **Performance** | Minimize storage ops, use `Safe` methods, optimize loops |
| **Testing** | Unit tests, coverage analysis, event verification |
| **Code Quality** | Clear naming, constants for keys, proper documentation |

### 12.5.4 Common Exceptions

| Exception | When It Occurs |
|:----------|:---------------|
| `ArgumentException` | Invalid parameters |
| `InvalidOperation` | Invalid context |
| `ExecutionEngineException` | VM execution error |
| `OutOfGasException` | Insufficient GAS |
