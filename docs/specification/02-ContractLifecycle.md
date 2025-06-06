# 3. Contract Lifecycle

## 3.1 Deployment

Smart contracts are deployed to the Neo blockchain through a transaction that invokes the `deploy` method of the `ContractManagement` native contract. The deployment process requires:

1. A compiled NEF file containing the contract bytecode
2. A manifest file describing the contract's properties and interfaces
3. Optional initialization data

### 3.1.1 NEF File

The NEF (Neo Executable Format) file is a binary format that contains the compiled bytecode of a smart contract. It includes:

- Magic header: Identifies the file as a NEF file
- Compiler information: Name and version of the compiler
- Source URL: Link to the source code
- Tokens: Method tokens for static calls
- Script: The compiled bytecode
- Checksum: Ensures the integrity of the file

The NEF file is created by the Neo compiler (nccs) when compiling a smart contract.

### 3.1.2 Manifest

The manifest is a JSON document that describes the contract's properties and interfaces. It includes:

- Name: The name of the contract
- Groups: Contract groups for permission management
- Features: Contract features
- Supported standards: NEP standards implemented by the contract
- ABI: Application Binary Interface describing the contract's methods and events
- Permissions: Permissions granted to the contract
- Trusts: Contracts trusted by the contract
- Extra: Additional metadata

Example manifest:

```json
{
  "name": "MyContract",
  "groups": [],
  "features": {},
  "supportedstandards": ["NEP-17"],
  "abi": {
    "methods": [
      {
        "name": "symbol",
        "parameters": [],
        "returntype": "String",
        "offset": 0,
        "safe": true
      },
      {
        "name": "decimals",
        "parameters": [],
        "returntype": "Integer",
        "offset": 7,
        "safe": true
      },
      {
        "name": "totalSupply",
        "parameters": [],
        "returntype": "Integer",
        "offset": 14,
        "safe": true
      },
      {
        "name": "balanceOf",
        "parameters": [
          {
            "name": "account",
            "type": "Hash160"
          }
        ],
        "returntype": "Integer",
        "offset": 21,
        "safe": true
      },
      {
        "name": "transfer",
        "parameters": [
          {
            "name": "from",
            "type": "Hash160"
          },
          {
            "name": "to",
            "type": "Hash160"
          },
          {
            "name": "amount",
            "type": "Integer"
          },
          {
            "name": "data",
            "type": "Any"
          }
        ],
        "returntype": "Boolean",
        "offset": 28,
        "safe": false
      }
    ],
    "events": [
      {
        "name": "Transfer",
        "parameters": [
          {
            "name": "from",
            "type": "Hash160"
          },
          {
            "name": "to",
            "type": "Hash160"
          },
          {
            "name": "amount",
            "type": "Integer"
          }
        ]
      }
    ]
  },
  "permissions": [
    {
      "contract": "*",
      "methods": "*"
    }
  ],
  "trusts": [],
  "extra": {
    "Author": "Example Developer",
    "Email": "dev@example.com",
    "Description": "A simple token contract"
  }
}
```

### 3.1.3 Deployment Transaction

To deploy a contract, a transaction is created that invokes the `deploy` method of the `ContractManagement` native contract with the following parameters:

1. **NEF file**: The compiled contract bytecode
2. **Manifest**: The contract manifest
3. **Data**: Optional initialization data

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;

// Deploy a contract
public static Contract DeployContract(ByteString nefFile, string manifest, object data)
{
    // Only authorized accounts can deploy contracts
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized deployment");

    // Deploy the contract
    Contract contract = ContractManagement.Deploy(nefFile, manifest, data);

    // Emit deployment event
    OnContractDeployed(contract.Hash);

    return contract;
}
```

#### Deployment Requirements

The deployment transaction must meet the following requirements:

- **Authorization**: Must be signed by the account that will pay the deployment fee
- **Fee Calculation**: Based on the size of the NEF file and manifest, with a minimum fee set by the `ContractManagement` native contract
- **Validation**: Both NEF file and manifest must pass validation checks
- **Uniqueness**: Contract name must be unique on the blockchain

### 3.1.4 Contract Hash

When a contract is deployed, it is assigned a unique hash that identifies it on the blockchain. The contract hash is calculated as:

```
ContractHash = RIPEMD160(SHA256(Sender + NEF.CheckSum + Name))
```

Where:
- `Sender` is the script hash of the account that deployed the contract
- `NEF.CheckSum` is the checksum of the NEF file
- `Name` is the name of the contract as specified in the manifest

The contract hash is used to reference the contract in subsequent transactions and contract calls.

### 3.1.5 Deployment Constraints

The deployment process is subject to the following constraints:

1. The NEF file and manifest must be valid
2. The contract name must be unique
3. The deployment fee must be paid
4. The transaction must be signed by the deployer

## 3.2 Initialization

When a contract is deployed, the `_deploy` method is automatically called if it exists in the contract. This method can be used to initialize the contract's state.

### 3.2.1 Deploy Method Signature

The `_deploy` method must have the following signature:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System;

public static void _deploy(object data, bool update)
{
    // Initialization code
}
```

#### Parameters

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `data` | `object` | Optional initialization data provided during deployment |
| `update` | `bool` | `false` for initial deployment, `true` for updates |

#### Method Requirements

- **Visibility**: Must be `public static`
- **Return Type**: Must be `void`
- **Exception Handling**: Should not throw exceptions during normal operation
- **Conditional Logic**: Should check the `update` parameter to handle deployment vs. update scenarios

### 3.2.2 Initialization Tasks

The `_deploy` method typically performs the following tasks:

1. **Deployment Check**: Verify if this is an initial deployment or an update
2. **State Initialization**: Set up initial contract state (owner, token supply, etc.)
3. **Event Emission**: Notify external systems of the deployment
4. **Validation**: Ensure initialization data is valid

#### Complete Initialization Example

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.Numerics;

public static void _deploy(object data, bool update)
{
    // Handle contract updates
    if (update)
    {
        // Perform any necessary state migrations
        OnContractUpdated("Contract updated successfully");
        return;
    }

    // Validate initialization data
    if (data is null)
        throw new Exception("Initialization data required");

    // Parse initialization data
    UInt160 owner = (UInt160)data;
    if (owner is null || !owner.IsValid)
        throw new Exception("Invalid owner address");

    // Initialize contract state
    Storage.Put(Storage.CurrentContext, "owner", owner);
    Storage.Put(Storage.CurrentContext, "totalSupply", 100_000_000);
    Storage.Put(Storage.CurrentContext, "decimals", 8);
    Storage.Put(Storage.CurrentContext, "symbol", "TOKEN");
    Storage.Put(Storage.CurrentContext, "name", "Example Token");

    // Initialize owner balance
    StorageMap balances = new StorageMap(Storage.CurrentContext, "balances");
    balances.Put(owner, 100_000_000);

    // Emit events
    OnTransfer(null, owner, 100_000_000);
    OnContractDeployed(owner);
}

// Events
public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount);
public delegate void OnContractDeployedDelegate(UInt160 owner);
public delegate void OnContractUpdatedDelegate(string message);

[DisplayName("Transfer")]
public static event OnTransferDelegate OnTransfer;

[DisplayName("ContractDeployed")]
public static event OnContractDeployedDelegate OnContractDeployed;

[DisplayName("ContractUpdated")]
public static event OnContractUpdatedDelegate OnContractUpdated;
```

### 3.2.3 Initialization Constraints

The initialization process is subject to the following constraints:

1. The `_deploy` method must be public and static
2. The `_deploy` method must have the correct signature
3. The `_deploy` method must not throw exceptions

## 3.3 Update

Contracts can be updated by calling the `update` method of the `ContractManagement` native contract. The update process requires:

1. A new NEF file containing the updated contract bytecode
2. A new manifest file describing the updated contract's properties and interfaces

### 3.3.1 Update Method

The `update` method of the `ContractManagement` native contract has the following signature:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;

// Update a contract (basic)
public static bool UpdateContract(ByteString nefFile, string manifest)
{
    // Verify authorization
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized update");

    // Perform the update
    ContractManagement.Update(nefFile, manifest);
    return true;
}

// Update a contract with initialization data
public static bool UpdateContractWithData(ByteString nefFile, string manifest, object data)
{
    // Verify authorization
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized update");

    // Perform the update with data
    ContractManagement.Update(nefFile, manifest, data);
    return true;
}
```

#### Parameters

| Parameter | Type | Description |
|:----------|:-----|:------------|
| `nefFile` | `ByteString` | The new NEF file containing the updated contract bytecode |
| `manifest` | `string` | The new manifest describing the updated contract's properties and interfaces |
| `data` | `object` | Optional data to pass to the `_deploy` method during update |

### 3.3.2 Update Constraints

The update process is subject to the following constraints:

1. The update must be initiated by the contract itself
2. The contract name cannot be changed
3. The new NEF file and manifest must be valid
4. The update fee must be paid

### 3.3.3 Update Verification

When a contract is updated, the `verify` method is called if it exists in the contract. This method can be used to verify that the update is authorized.

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public static bool Verify()
{
    // Get the contract owner
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");

    // Verify the owner's signature
    if (!Runtime.CheckWitness(owner))
        return false;

    // Additional verification logic can be added here
    // For example, checking if updates are enabled
    bool updatesEnabled = (bool)Storage.Get(Storage.CurrentContext, "updatesEnabled");
    if (!updatesEnabled)
        return false;

    return true;
}
```

#### Verification Process

1. **Automatic Invocation**: The `verify` method is automatically called during update operations
2. **Authorization Check**: Must return `true` for the update to proceed
3. **Rejection Handling**: If the method returns `false`, the update is rejected
4. **Custom Logic**: Can include additional business logic for update authorization

### 3.3.4 Update Callback

After a contract is updated, the `_deploy` method is called with the `update` parameter set to `true`. This allows the contract to perform any necessary state migrations or updates.

```csharp
public static void _deploy(object data, bool update)
{
    if (!update) return;

    // Perform state migrations or updates
}
```

## 3.4 Destruction

Contracts can be destroyed by calling the `destroy` method of the `ContractManagement` native contract. This removes the contract's code and storage from the blockchain.

### 3.4.1 Destroy Method

The `destroy` method of the `ContractManagement` native contract has the following signature:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;

// Destroy the current contract
public static bool DestroyContract()
{
    // Verify authorization
    if (!Runtime.CheckWitness(GetOwner()))
        throw new Exception("Unauthorized destruction");

    // Optional: Perform cleanup operations
    CleanupBeforeDestruction();

    // Emit destruction event
    OnContractDestroyed(Runtime.ExecutingScriptHash);

    // Destroy the contract
    ContractManagement.Destroy();

    return true;
}

private static void CleanupBeforeDestruction()
{
    // Example: Transfer remaining tokens to owner
    UInt160 owner = GetOwner();
    BigInteger balance = GetContractBalance();

    if (balance > 0)
    {
        // Transfer tokens before destruction
        TransferToOwner(owner, balance);
    }

    // Clear sensitive data
    Storage.Delete(Storage.CurrentContext, "sensitiveData");
}

public delegate void OnContractDestroyedDelegate(UInt160 contractHash);

[DisplayName("ContractDestroyed")]
public static event OnContractDestroyedDelegate OnContractDestroyed;
```

### 3.4.2 Destroy Constraints

The destruction process is subject to the following constraints:

1. The destruction must be initiated by the contract itself
2. The contract must have appropriate permissions

### 3.4.3 Destroy Verification

When a contract is destroyed, the `verify` method is called if it exists in the contract. This method can be used to verify that the destruction is authorized.

```csharp
public static bool Verify()
{
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");
    return Runtime.CheckWitness(owner);
}
```

If the `verify` method returns `false`, the destruction is rejected.

### 3.4.4 Destroy Effects

When a contract is destroyed:

1. The contract's code is removed from the blockchain
2. The contract's storage is cleared
3. The contract can no longer be called
4. Any tokens held by the contract are lost

## 3.5 Contract Lifecycle Management

### 3.5.1 Contract Ownership

Contracts often implement an ownership model to control who can update or destroy the contract. This is typically done by storing the owner's script hash in the contract's storage.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;

// Storage keys
private static readonly byte[] OwnerKey = new byte[] { 0x01 };
private static readonly byte[] PendingOwnerKey = new byte[] { 0x02 };

// Events
public delegate void OwnershipTransferredDelegate(UInt160 previousOwner, UInt160 newOwner);
public delegate void OwnershipTransferInitiatedDelegate(UInt160 currentOwner, UInt160 pendingOwner);

[DisplayName("OwnershipTransferred")]
public static event OwnershipTransferredDelegate OwnershipTransferred;

[DisplayName("OwnershipTransferInitiated")]
public static event OwnershipTransferInitiatedDelegate OwnershipTransferInitiated;

public static void _deploy(object data, bool update)
{
    if (update) return;

    UInt160 owner = (UInt160)data;
    if (owner is null || !owner.IsValid)
        throw new Exception("Invalid owner address");

    Storage.Put(Storage.CurrentContext, OwnerKey, owner);
    OwnershipTransferred(null, owner);
}

public static bool Verify()
{
    UInt160 owner = GetOwner();
    return Runtime.CheckWitness(owner);
}

[Safe]
public static UInt160 GetOwner()
{
    return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
}

[Safe]
public static UInt160 GetPendingOwner()
{
    return (UInt160)Storage.Get(Storage.CurrentContext, PendingOwnerKey);
}

// Two-step ownership transfer for security
public static bool InitiateOwnershipTransfer(UInt160 newOwner)
{
    UInt160 currentOwner = GetOwner();
    if (!Runtime.CheckWitness(currentOwner))
        return false;

    if (newOwner is null || !newOwner.IsValid)
        throw new Exception("Invalid new owner address");

    Storage.Put(Storage.CurrentContext, PendingOwnerKey, newOwner);
    OwnershipTransferInitiated(currentOwner, newOwner);
    return true;
}

public static bool AcceptOwnership()
{
    UInt160 pendingOwner = GetPendingOwner();
    if (pendingOwner is null || !Runtime.CheckWitness(pendingOwner))
        return false;

    UInt160 currentOwner = GetOwner();
    Storage.Put(Storage.CurrentContext, OwnerKey, pendingOwner);
    Storage.Delete(Storage.CurrentContext, PendingOwnerKey);

    OwnershipTransferred(currentOwner, pendingOwner);
    return true;
}
```

### 3.5.2 Contract Versioning

Contracts often include version information to track changes and ensure compatibility. This is typically done using the `ContractVersion` attribute.

```csharp
[ContractVersion("1.0.0")]
public class MyContract : SmartContract
{
    // Contract implementation
}
```

The version information is stored in the contract manifest and can be accessed by external systems.

### 3.5.3 Contract Migration

When a contract needs to be replaced with a new implementation, a migration process is often used to transfer state from the old contract to the new one. This typically involves:

1. Deploying the new contract
2. Transferring state from the old contract to the new one
3. Updating references to the old contract
4. Destroying the old contract

```csharp
public static bool Migrate(UInt160 newContractHash)
{
    UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, "owner");
    if (!Runtime.CheckWitness(owner))
        return false;

    // Transfer state to the new contract
    BigInteger totalSupply = (BigInteger)Storage.Get(Storage.CurrentContext, "totalSupply");
    Contract.Call(newContractHash, "initialize", CallFlags.All, new object[] { owner, totalSupply });

    // Destroy the old contract
    ContractManagement.Destroy();

    return true;
}
```

### 3.5.4 Contract Upgradability Patterns

Several patterns can be used to make contracts upgradable:

1. **Proxy Pattern**: A proxy contract delegates calls to an implementation contract that can be replaced.
2. **Data Separation Pattern**: Contract logic and data are separated, allowing the logic contract to be replaced while preserving the data.
3. **Registry Pattern**: A registry contract maintains references to other contracts, allowing them to be replaced.

Example of the proxy pattern:

```csharp
[DisplayName("Proxy")]
public class Proxy : SmartContract
{
    private static readonly byte[] ImplementationKey = new byte[] { 0x01 };
    private static readonly byte[] OwnerKey = new byte[] { 0x02 };

    public static void _deploy(object data, bool update)
    {
        if (update) return;

        UInt160[] parameters = (UInt160[])data;
        UInt160 implementation = parameters[0];
        UInt160 owner = parameters[1];

        Storage.Put(Storage.CurrentContext, ImplementationKey, implementation);
        Storage.Put(Storage.CurrentContext, OwnerKey, owner);
    }

    public static bool UpdateImplementation(UInt160 newImplementation)
    {
        UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
        if (!Runtime.CheckWitness(owner))
            return false;

        Storage.Put(Storage.CurrentContext, ImplementationKey, newImplementation);
        return true;
    }

    public static object Main(string method, object[] args)
    {
        UInt160 implementation = (UInt160)Storage.Get(Storage.CurrentContext, ImplementationKey);
        return Contract.Call(implementation, method, CallFlags.All, args);
    }
}
```

## 3.6 Complete Contract Lifecycle Example

Here's a complete example of a contract that implements the full lifecycle:

```csharp
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

[DisplayName("LifecycleContract")]
[ContractDescription("A contract demonstrating the full lifecycle")]
[ContractEmail("dev@example.com")]
[ContractVersion("1.0.0")]
[ContractPermission(Permission.Any, Method.Any)]
public class LifecycleContract : SmartContract
{
    // Storage keys
    private static readonly byte[] OwnerKey = new byte[] { 0x01 };
    private static readonly byte[] VersionKey = new byte[] { 0x02 };
    private static readonly byte[] DataKey = new byte[] { 0x03 };

    // Events
    public static event Action<UInt160> OwnerChanged;
    public static event Action<string> VersionChanged;
    public static event Action<string> DataChanged;

    // Initialization
    public static void _deploy(object data, bool update)
    {
        if (update)
        {
            // Update logic
            string newVersion = "1.0.1";
            Storage.Put(Storage.CurrentContext, VersionKey, newVersion);
            VersionChanged(newVersion);
            return;
        }

        // Initial deployment logic
        UInt160 owner = (UInt160)data;
        Storage.Put(Storage.CurrentContext, OwnerKey, owner);
        Storage.Put(Storage.CurrentContext, VersionKey, "1.0.0");
        Storage.Put(Storage.CurrentContext, DataKey, "Initial data");
    }

    // Verification
    public static bool Verify()
    {
        UInt160 owner = (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
        return Runtime.CheckWitness(owner);
    }

    // Owner management
    [Safe]
    public static UInt160 GetOwner()
    {
        return (UInt160)Storage.Get(Storage.CurrentContext, OwnerKey);
    }

    public static bool TransferOwnership(UInt160 newOwner)
    {
        UInt160 owner = GetOwner();
        if (!Runtime.CheckWitness(owner))
            return false;

        Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
        OwnerChanged(newOwner);
        return true;
    }

    // Version management
    [Safe]
    public static string GetVersion()
    {
        return (string)Storage.Get(Storage.CurrentContext, VersionKey);
    }

    // Data management
    [Safe]
    public static string GetData()
    {
        return (string)Storage.Get(Storage.CurrentContext, DataKey);
    }

    public static bool SetData(string data)
    {
        UInt160 owner = GetOwner();
        if (!Runtime.CheckWitness(owner))
            return false;

        Storage.Put(Storage.CurrentContext, DataKey, data);
        DataChanged(data);
        return true;
    }

    // Update
    public static bool Update(ByteString nefFile, string manifest)
    {
        UInt160 owner = GetOwner();
        if (!Runtime.CheckWitness(owner))
            return false;

        ContractManagement.Update(nefFile, manifest, null);
        return true;
    }

    // Destruction
    public static bool Destroy()
    {
        UInt160 owner = GetOwner();
        if (!Runtime.CheckWitness(owner))
            return false;

        ContractManagement.Destroy();
        return true;
    }

    // Migration
    public static bool Migrate(UInt160 newContractHash)
    {
        UInt160 owner = GetOwner();
        if (!Runtime.CheckWitness(owner))
            return false;

        // Transfer state to the new contract
        string version = GetVersion();
        string data = GetData();
        Contract.Call(newContractHash, "initialize", CallFlags.All, new object[] { owner, version, data });

        // Destroy the old contract
        ContractManagement.Destroy();

        return true;
    }

    // Migration target method
    public static bool Initialize(UInt160 owner, string version, string data)
    {
        // Ensure this is only called during migration
        if (Storage.Get(Storage.CurrentContext, OwnerKey) != null)
            return false;

        Storage.Put(Storage.CurrentContext, OwnerKey, owner);
        Storage.Put(Storage.CurrentContext, VersionKey, version);
        Storage.Put(Storage.CurrentContext, DataKey, data);

        return true;
    }
}
```

This contract demonstrates:
- Initialization during deployment
- Verification for secure operations
- Owner management
- Version tracking
- Data management
- Contract updates
- Contract destruction
- Contract migration
