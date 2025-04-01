# Runtime Services

The static `Runtime` class in `Neo.SmartContract.Framework.Services` provides access to essential information and functionalities related to the execution environment of the smart contract.

## Execution Context

*   **`Runtime.ExecutingScriptHash` (`UInt160`)**: Gets the script hash (address) of the contract currently being executed.
*   **`Runtime.CallingScriptHash` (`UInt160`)**: Gets the script hash of the contract that called the current contract. Returns `null` if called directly by a transaction (the "entry point").
*   **`Runtime.EntryScriptHash` (`UInt160`)**: Gets the script hash of the contract that initiated the current execution chain (the first contract called in a transaction).
*   **`Runtime.InvocationCounter` (`uint`)**: Gets how many contracts have been invoked in the current execution chain. Useful for preventing deep call stacks or certain reentrancy patterns.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class RuntimeContextDemo : SmartContract
{
    public static UInt160 GetSelfHash()
    {
        return Runtime.ExecutingScriptHash;
    }

    public static UInt160 GetCallerHash()
    {
        // This will be null if called directly from a transaction
        return Runtime.CallingScriptHash;
    }
}
```

## Blockchain Information

*   **`Runtime.Time` (`ulong`)**: Gets the timestamp (Unix epoch time in milliseconds) of the current block being processed.
*   **`Runtime.Trigger` (`TriggerType`)**: Gets the trigger that caused the current execution (e.g., `Application`, `Verification`).
*   **`Runtime.Network` (`uint`)**: Gets the magic number of the Neo network the contract is running on (e.g., MainNet, TestNet).
*   **`Runtime.GetNetwork()` (`uint`)**: (Deprecated in newer versions, use `Runtime.Network`).
*   **`Runtime.GasLeft` (`long`)**: Gets the amount of GAS remaining for the current execution context. Useful for controlling execution within GAS limits.
*   **`Runtime.Platform` (`string`)**: Returns "Neo".
*   **`Runtime.GetTrigger()` (`TriggerType`)**: (Deprecated, use `Runtime.Trigger`).
*   **`Runtime.GetScriptContainer()` (`Transaction`)**: (Deprecated, use `Runtime.Transaction`).
*   **`Runtime.Transaction` (`Transaction`)**: Gets the current transaction being executed. Note: Accessing transaction properties costs GAS.
*   **`Runtime.CurrentSigners` (`Signer[]`)**: Gets the signers of the current transaction.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class BlockchainInfoDemo : SmartContract
{
    public static ulong GetCurrentBlockTime()
    {
        return Runtime.Time;
    }

    public static TriggerType GetInvocationTrigger()
    {
        return Runtime.Trigger;
    }

    public static BigInteger GetTransactionSystemFee()
    {
        Transaction tx = Runtime.Transaction;
        // Accessing tx properties costs GAS
        return tx?.SystemFee ?? -1; // Return -1 if tx is somehow null
    }
}
```

## Logging & Notifications

*   **`Runtime.Log(string message)`**: Emits a log message. Logs are recorded on the blockchain (if the transaction succeeds) and can be viewed by explorers or node plugins. Useful for debugging and recording information.
*   **`Runtime.Notify(string eventName, params object[] state)`**: Fires a notification event. Notifications include the executing contract hash and an array of `StackItem`s representing the `state`. Off-chain applications (dApps, explorers) typically subscribe to `Notify` events to react to contract state changes. This is preferred over `Log` for signalling important state transitions.
*   **`Runtime.Notify(params object[] state)`**: (Deprecated, use version with eventName). Fires a notify event with a default event name.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class LogNotifyDemo : SmartContract
{
    // Using Notify with explicit event name (Recommended)
    public static event System.Action<string, BigInteger> OnValueUpdated;

    public static void UpdateValue(string key, BigInteger value)
    {
        Runtime.Log($"Attempting to update {key} to {value}");
        // ... store value ...
        // Use Notify to signal the change to the outside world
        Runtime.Notify("ValueUpdated", key, value); // Event name + parameters

        // Or use the C# event syntax, which compiles to Runtime.Notify
        OnValueUpdated(key, value); 
    }
}
```

## Witness Verification (Authorization)

One of the most critical runtime services is verifying signatures.

*   **`Runtime.CheckWitness(UInt160 hash)`**: Checks if the transaction was signed by the specified script hash (address). Returns `true` if the corresponding account is one of the transaction's signers and its signature covers the current execution context (based on Signer Scopes), `false` otherwise.
*   **`Runtime.CheckWitness(ECPoint pubkey)`**: Checks if the transaction was signed by the account corresponding to the given public key.

This is the standard way to implement access control in smart contracts – ensuring that only authorized accounts can perform sensitive actions.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Neo.SmartContract.Examples
{
    public class CheckWitnessDemo : SmartContract
    {
        private static readonly StorageMap NameMap = new StorageMap(Storage.CurrentContext, "NAME");
        private static readonly byte[] OwnerKey = { 0x01 };

        // Store owner on deploy
        public static void _deploy(object data, bool update)
        { 
            if (!update) 
            { 
                // Assume deployer's hash is passed in data or is the tx sender
                // Example: Get owner from Transaction Signers (simplistic)
                Signer[] signers = Runtime.Transaction.Signers;
                if (signers.Length > 0) {
                  Storage.Put(Storage.CurrentContext, OwnerKey, signers[0].Account);
                  Runtime.Log("Owner set");
                } else {
                   Runtime.Log("Owner NOT set - no signer");
                }
            } 
        }

        // Method only the owner can call
        public static bool SetNameForOwner(string name)
        {
            ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
            if (owner is null || owner.Length != 20) 
            { 
                Runtime.Log("Owner not set or invalid.");
                return false; 
            }

            // Check if the stored owner signed the transaction
            if (!Runtime.CheckWitness((UInt160)owner))
            {
                Runtime.Log("CheckWitness failed for owner.");
                return false;
            }

            NameMap.Put("owner_name", name);
            Runtime.Log("Name set successfully by owner.");
            return true;
        }

        // Method anyone can call if they sign
        public static bool SetNameForSelf(UInt160 user, string name)
        {
            // Check if the 'user' parameter hash signed the transaction
            if (!Runtime.CheckWitness(user))
            { 
                Runtime.Log($"CheckWitness failed for user {user}.");
                return false;
            }
            NameMap.Put(user, name); // Use user hash as key
            Runtime.Log($"Name set successfully for {user}.");
            return true;
        }
    }
}
```

**Important:** `CheckWitness` relies on the `Signers` attached to the transaction and their specified `Scopes`. Ensure users sign transactions with appropriate scopes (`CalledByEntry` is common for simple calls) for `CheckWitness` to function as expected.

[Previous: Storage](./01-storage.md) | [Next: Events](./03-events.md)