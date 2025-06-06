# Contract Upgrade and Migration

Smart contracts deployed on the blockchain are typically immutable. However, bugs may be found, or new features may be required. Neo N3 provides a mechanism to update the code of a deployed contract using the `ContractManagement.Update` native method. However, updating requires careful planning, especially regarding storage.

## The `Update` Mechanism

1.  **Compile New Version:** Compile the new version of your contract code (`.nef`, `.manifest.json`).
2.  **Implement `update` Method:** Your *original* contract must have a method (conventionally named `update`) that calls `ContractManagement.Update`. This method should include authorization logic (e.g., `Runtime.CheckWitness` against an owner or admin address) to control who can trigger the update.
3.  **Permissions:** The original contract's manifest must grant permission for the contract to call its own `update` method (`[ContractPermission(Runtime.ExecutingScriptHash, "update")]`).
4.  **Invoke `update`:** An authorized user sends a transaction calling the `update` method, passing the new `.nef` file content and `.manifest.json` content as arguments.
5.  **`ContractManagement.Update` Call:** Inside your `update` method, `ContractManagement.Update(nefFile, manifest, optionalData)` is called.
6.  **Execution:** The NeoVM replaces the contract's code (NEF) and manifest with the new versions.
7.  **`_deploy` Execution:** The `_deploy(object data, bool update)` method of the *new* contract code is executed with the `update` flag set to `true`. The `optionalData` from the `Update` call is passed as the `data` argument.

```csharp
// In YourContract.cs (Version 1)
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

// Grant permission to call self.update
[ContractPermission(nameof(ContractManagement), "update")] 
public class YourContractV1 : SmartContract
{
    private static readonly byte[] OwnerKey = { 0x01 };

    public static void _deploy(object data, bool update)
    { 
        if (!update) 
        { 
            // Set owner on initial deployment
            Storage.Put(Storage.CurrentContext, OwnerKey, Runtime.Transaction.Sender);
        }
        Runtime.Log("V1 Deployed/Updated");
    }

    // Method to trigger the update
    public static void update(ByteString nefFile, string manifest, object data)
    {
        ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
        if (owner is null || !Runtime.CheckWitness((UInt160)owner)) 
            throw new System.Exception("Unauthorized update");

        ContractManagement.Update(nefFile, manifest, data);
        Runtime.Log("Contract update initiated.");
    }
    
    // ... other V1 methods ...
}
```

```csharp
// In YourContract.cs (Version 2)
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

public class YourContractV2 : SmartContract
{
    // Storage keys should ideally remain consistent or be migrated
    private static readonly byte[] OwnerKey = { 0x01 }; 
    private static readonly StorageMap UserData = new StorageMap(Storage.CurrentContext, "UDTA");

    public static void _deploy(object data, bool update)
    { 
        Runtime.Log("V2 Deployed/Updated");
        if (update)
        {
            // Perform migration tasks only needed during an update
            Runtime.Log("Running V2 update logic.");
            // Example: Migrate data passed via 'data' argument
            if (data != null) { /* process migration data */ }
            // Example: Migrate existing storage if format changed
            MigrateStorageFromV1();
        } 
        else 
        { 
            // Set owner only on initial V2 deployment (if V2 is deployed fresh)
             Storage.Put(Storage.CurrentContext, OwnerKey, Runtime.Transaction.Sender);
        }
    }

    // Update method should also exist in V2 for future updates
    public static void update(ByteString nefFile, string manifest, object data)
    {
        ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
        if (owner is null || !Runtime.CheckWitness((UInt160)owner))
            throw new System.Exception("Unauthorized update");
        ContractManagement.Update(nefFile, manifest, data);
    }

    private static void MigrateStorageFromV1()
    {
        // Add logic here if V1 storage needs transformation for V2
        Runtime.Log("Performing storage migration...");
        // e.g., Read old storage format, transform, write to new format
    }

    // ... other V2 methods ...
}
```

## Storage Compatibility & Migration

The biggest challenge with updates is **storage**. When you update a contract, its storage area persists.

*   **Compatible Changes:** If the new contract version uses the same storage keys and data structures (serialization format) as the old version, no migration is needed.
*   **Incompatible Changes:** If you change storage keys, add/remove fields in stored structs, or change serialization methods, the new code might not be able to read the old data correctly.

**Strategies for Handling Storage Changes:**

1.  **Design for Upgradability:**
    *   Use `StorageMap` with consistent prefixes.
    *   Avoid complex nested structures in storage if possible.
    *   Use version numbers within your stored data.
    *   Try to make changes additive rather than breaking.

2.  **Migration in `_deploy`:**
    *   Place migration logic inside the `_deploy` method of the *new* contract version, within the `if (update)` block.
    *   This logic reads data in the old format (using old keys/deserialization), transforms it, and writes it back in the new format.
    *   This can be GAS-intensive if migrating large amounts of data. Consider migrating data lazily (see below).
    *   You might pass necessary migration parameters via the `data` argument of `ContractManagement.Update`.

3.  **Lazy Migration:**
    *   Instead of migrating all data at once in `_deploy`, migrate individual data items only when they are accessed for the first time by the new contract version.
    *   Add version checks when reading data. If old version data is detected, migrate it on the fly before returning/using it.
    *   Requires careful implementation in all methods that read potentially old data.

4.  **Proxy Pattern (Advanced):**
    *   Deploy a minimal, non-updatable Proxy contract that users interact with.
    *   The Proxy contract holds the address of the current *implementation* contract.
    *   All calls to the Proxy are forwarded (using `Contract.Call`) to the implementation contract.
    *   Storage is usually kept in the Proxy contract (or a separate dedicated storage contract) to persist across implementation changes.
    *   To upgrade, deploy a new implementation contract and update the implementation address stored in the Proxy contract.
    *   This pattern separates logic and state, simplifying updates but adding complexity and GAS overhead (due to the extra `Contract.Call`).

5.  **Data Export/Import (Off-Chain):**
    *   For major breaking changes, provide a mechanism in the old contract to export data (e.g., emit events, allow reading state).
    *   Deploy a completely new contract.
    *   Users (or an admin) interact with the new contract to import their data, potentially providing proof of ownership from the old contract.
    *   This is essentially a manual migration, not an in-place update.

**Choosing the right strategy depends on the complexity of the changes, the amount of data, and the desired user experience.** Always test upgrade scenarios thoroughly on a testnet or using local simulation tools like Neo Express.

[Previous: Advanced Topics Overview](./README.md) | [Next: Security Best Practices](./02-security.md)