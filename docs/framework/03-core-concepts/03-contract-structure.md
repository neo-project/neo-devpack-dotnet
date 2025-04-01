# C# Smart Contract Structure

A Neo smart contract written in C# typically follows a standard structure based on a public class.

```csharp
// 1. Import necessary namespaces
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel; // For DisplayName
using System.Numerics; // For BigInteger

// 2. Define the namespace for your contract
namespace MyNeoContract.Example
{
    // 3. Apply Manifest Attributes to the class
    [DisplayName("MyContractDisplayName")] // User-friendly name in manifest
    [ManifestExtra("Author", "Your Name")]
    [ManifestExtra("Email", "your.email@example.com")]
    [ManifestExtra("Description", "This is my example contract.")]
    [SupportedStandards("NEP-17")] // If applicable
    [ContractPermission("*", "*")] // Define permissions (use carefully!)
    // 4. Define the main contract class (often inherits from SmartContract)
    public class MyContract : SmartContract
    {
        // 5. Constants and Static Readonly Fields (e.g., for storage prefixes)
        private const byte Prefix_Balance = 0x01;
        private static readonly byte[] OwnerKey = { 0x02 }; 
        private static readonly StorageMap Balances = new StorageMap(Storage.CurrentContext, Prefix_Balance);

        // 6. Events
        public delegate void TransferredDelegate(UInt160 from, UInt160 to, BigInteger amount);
        [DisplayName("Transfer")] // Event name in manifest
        public static event TransferredDelegate OnTransfer;

        // 7. Public Static Methods (Contract Entry Points / Callable Methods)
        public static string GetName()
        {
            return "MyContract";
        }

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
        {
            // ... implementation ...
            if (!Runtime.CheckWitness(from)) return false;
            // ... logic ...
            Balances.Put(from, currentFromBalance - amount);
            Balances.Put(to, currentToBalance + amount);
            // Fire event
            OnTransfer(from, to, amount);
            Runtime.Log("Transfer successful");
            return true;
        }

        // 8. Private Helper Methods (Not exposed in manifest)
        private static BigInteger GetBalance(UInt160 account)
        {
           return (BigInteger)Balances.Get(account);
        }

        // 9. Special Methods (Optional)

        // Executed on deployment/update
        public static void _deploy(object data, bool update)
        {
            if (!update) // Only run on initial deployment
            {
                // Initialize storage, set owner, etc.
                Storage.Put(Storage.CurrentContext, OwnerKey, (ByteString)"NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash());
            }
        }

        // Called when contract receives NEP-17 tokens
        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            // Handle incoming payment
        }

        // Called during contract update (requires ContractManagement native contract)
        // public static void update(ByteString nefFile, string manifest) { ... }

        // Called during contract destruction (requires ContractManagement native contract)
        // public static void destroy() { ... }

        // Optional: Verification method, runs on transfers TO the contract address
        // public static bool verify() { ... }
    }
}
```

## Key Components Explained

1.  **Namespaces:** Import framework classes (`Neo.SmartContract.Framework.*`) and standard .NET types (`System.Numerics`, `System.ComponentModel`).
2.  **Namespace:** Organize your contract code within a C# namespace.
3.  **Manifest Attributes:** Class-level attributes (`DisplayName`, `ManifestExtra`, `SupportedStandards`, `ContractPermission`, etc.) populate the `.manifest.json` file, providing metadata about your contract.
4.  **Contract Class:** The main public class containing your contract's logic. Inheriting from `SmartContract` is common practice but not strictly required; it provides convenient access to helper methods.
5.  **Constants/Static Fields:** Define constants or static fields, often used for storage map prefixes or fixed values. `StorageMap` provides a convenient way to segment storage.
6.  **Events:** Define events using C# delegates and the `event` keyword. Use `DisplayName` to control the event name in the manifest. Events are crucial for notifying off-chain applications about state changes.
7.  **Public Static Methods:** These become the callable methods of your smart contract. They are listed in the manifest and can be invoked by users or other contracts.
8.  **Private Helper Methods:** Standard C# private methods for internal logic. They are *not* exposed in the manifest and cannot be called directly from outside the contract.
9.  **Special Methods:** Methods with specific names recognized by the NeoVM or framework:
    *   `_deploy(object data, bool update)`: Executed once upon deployment or update. Ideal for initialization.
    *   `OnNEP17Payment(UInt160 from, BigInteger amount, object data)`: Triggered when the contract address receives NEP-17 tokens.
    *   `verify()`: If present, this method is executed whenever someone tries to use the contract's address as a witness (e.g., sending assets *from* the contract address, or if the contract address is listed as a signer in a transaction). It must return `true` for the verification to pass.
    *   `update(ByteString nef, string manifest)`: Handles contract updates (requires `ContractManagement.Update` permissions).
    *   `destroy()`: Handles contract destruction (requires `ContractManagement.Destroy` permissions).

This structure provides a clear and organized way to write C# smart contracts for the Neo platform.

## Advanced C# Features

Modern versions of the Neo C# compiler (`nccs`) support a growing subset of advanced C# features, including:

*   **LINQ:** Some LINQ query operations on collections (like `Where`, `Select`, `Count`, `Any`) can be compiled, often translating to underlying loops or specific opcodes.
*   **Lambda Expressions:** Anonymous functions can often be used where delegates are expected.
*   **Records and Tuples:** These provide convenient ways to structure data.
*   **Pattern Matching:** C# pattern matching constructs (e.g., in `switch` statements or expressions) are often supported.

While these features enhance expressiveness, be mindful that:

1.  **Not all .NET BCL methods are supported:** Only methods with corresponding NeoVM implementations or translatable logic can be used.
2.  **GAS Costs:** The GAS cost of complex LINQ queries or intricate lambda logic might be less predictable than explicit loops. Always test performance if using these features heavily in GAS-sensitive areas.
3.  **Compiler Limitations:** Check the specific `nccs` version for the exact level of support for the latest C# features.

[Previous: Transactions](./02-transactions.md) | [Next: Entry Points & Methods](./04-entry-points.md)