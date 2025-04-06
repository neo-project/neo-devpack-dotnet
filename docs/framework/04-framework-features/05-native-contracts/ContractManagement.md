# Native Contract: ContractManagement

Namespace: `Neo.SmartContract.Framework.Native`

Provides functionalities for managing smart contracts on the blockchain: deployment, updates, and retrieval of contract information.

## Key Methods

*   **`GetContract(UInt160 hash)` (`Contract`)**: Retrieves information about a deployed contract by its script hash.
    *   Returns a `Contract` object containing `Id`, `UpdateCounter`, `Hash`, `Nef`, and `Manifest`.
    *   Returns `null` if no contract with that hash exists.
    *   Requires read permissions (`CallFlags.ReadStates`).

*   **`HasMethod(UInt160 hash, string method, int pcount)` (`bool`)**: Checks if a deployed contract has a specific method with a given parameter count.
    *   Useful for verifying interface compatibility before calling.
    *   Requires read permissions (`CallFlags.ReadStates`).

*   **`Deploy(ByteString nefFile, string manifest)` (`Contract`)**: Deploys a new contract.
    *   `nefFile`: The compiled NeoVM bytecode (`.nef` file content).
    *   `manifest`: The contract's manifest (`.manifest.json` content).
    *   This method can *only* be called from the `_deploy` method of the contract being deployed. It automatically happens when a user sends a deployment transaction.
    *   You generally **do not call this directly** from your regular contract methods.

*   **`Deploy(ByteString nefFile, string manifest, object data)` (`Contract`)**: Deploys a new contract and passes `data` to its `_deploy` method.
    *   Same constraints as the version without `data`.

*   **`Update(ByteString nefFile, string manifest)`**: Updates the calling contract's code and manifest.
    *   Can only be called from within the contract itself.
    *   Requires the contract to have permission to call itself (`[ContractPermission(Runtime.ExecutingScriptHash, "update")]`).
    *   The logic for *who* can trigger an update must be implemented within the `update` method (e.g., checking `Runtime.CheckWitness` against an owner address).

*   **`Update(ByteString nefFile, string manifest, object data)`**: Updates the calling contract and passes `data` to its `_deploy` method (with `update` flag set to `true`).

*   **`Destroy()`**: Destroys the calling contract.
    *   Can only be called from within the contract itself.
    *   Requires the contract to have permission to call itself (`[ContractPermission(Runtime.ExecutingScriptHash, "destroy")]`).
    *   Logic for *who* can trigger destruction must be implemented (e.g., in a `destroy` method).
    *   Destroyed contracts cannot be called, and their storage becomes inaccessible.

*   **`GetMinimumDeploymentFee()` (`long`)**: Gets the minimum GAS fee required for contract deployment.

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

[ContractPermission("ContractManagement", "getContract", "hasMethod")]
[ContractPermission(nameof(ContractManagement), "update")] // Permission to update self
public class ContractMgmtDemo : SmartContract
{
    private static readonly byte[] OwnerKey = { 0x01 };

    public static void _deploy(object data, bool update)
    {
        if (!update)
        {
            // Store owner on initial deployment (example)
            Storage.Put(Storage.CurrentContext, OwnerKey, Runtime.Transaction.Sender);
        }
    }

    // Check if another contract exists
    public static bool CheckContractExists(UInt160 hash)
    {
        Contract contract = ContractManagement.GetContract(hash);
        return contract != null;
    }

    // Check if another contract supports a specific method (e.g., NEP-17 transfer)
    public static bool SupportsTransfer(UInt160 hash)
    {
        // NEP-17 transfer has 4 parameters: from, to, amount, data
        return ContractManagement.HasMethod(hash, "transfer", 4);
    }

    // Method to trigger an update (must be called by the owner)
    public static void UpdateContract(ByteString nefFile, string manifest)
    {
        ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
        if (owner == null || !Runtime.CheckWitness((UInt160)owner))
        {
            throw new System.Exception("Unauthorized: Only owner can update.");
        }
        ContractManagement.Update(nefFile, manifest);
    }
}
```

[Previous: Native Contracts Overview](./README.md) | [Next: CryptoLib](./CryptoLib.md)