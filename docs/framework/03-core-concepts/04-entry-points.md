# Entry Points & Methods

Smart contract methods are the functions that allow users and other contracts to interact with your deployed code. In C# contracts for Neo, these are defined as public static methods within your contract class.

## Defining Callable Methods

Any `public static` method within your main contract class will typically be compiled into a callable function in the NeoVM and listed in the contract's manifest file.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics; // For BigInteger

namespace MyNeoContract.Example
{
    public class MyMethodContract : SmartContract
    {
        // This is a callable method
        public static BigInteger Add(BigInteger a, BigInteger b)
        {
            return a + b;
        }

        // This is also callable
        [DisplayName("getUserBalance")] // Control the name in the manifest
        public static BigInteger GetBalance(UInt160 user)
        {
            // ... logic to retrieve balance from storage ...
            StorageMap balances = new StorageMap(Storage.CurrentContext, "BAL");
            return (BigInteger)(balances.Get(user) ?? BigInteger.Zero);
        }

        // This method requires the caller to have signed the transaction
        public static bool UpdateBalance(UInt160 user, BigInteger change)
        {
            if (!Runtime.CheckWitness(user))
            {
                Runtime.Log("CheckWitness failed for balance update.");
                return false; // Authorization failed
            }
            // ... logic to update balance ...
            Runtime.Log("Balance updated successfully.");
            return true;
        }

        // Private methods are NOT callable from outside
        private static void InternalCalculation()
        {
            // ... helper logic ...
        }

        // Instance methods are NOT callable
        public void InstanceMethod()
        {
            // Not allowed in static context of NeoVM
        }
    }
}
```

**Key Points:**

*   **`public static`:** Methods must be both `public` and `static` to be directly callable entry points.
*   **`[DisplayName]` Attribute:** You can use this attribute to specify a different name for the method in the `.manifest.json` file than its C# name. This is useful for adhering to conventions (like camelCase) or providing clearer names.
*   **Parameters & Return Types:** Methods can accept parameters and return values. These must be types compatible with the NeoVM (see [Data Types](./05-data-types.md)).
*   **Private/Instance Methods:** Regular C# `private` or instance (non-static) methods are internal to the contract's implementation and cannot be called directly via a transaction.

## Special Methods (Entry Points)

As mentioned in [Contract Structure](./03-contract-structure.md), certain method names have special meaning and are triggered by specific events or phases:

*   **`_deploy(object data, bool update)`**: Called automatically by the system only once when the contract is deployed or updated. Cannot be called directly by users afterwards.
*   **`OnNEP17Payment(UInt160 from, BigInteger amount, object data)`**: Called automatically by the system when the contract receives NEP-17 tokens via a `transfer` call where this contract is the recipient.
*   **`verify()`**: If present and returns `true`, allows the contract's script hash to be used as a witness in a transaction's `Signers` array. This is crucial if the contract needs to authorize actions itself (e.g., sending assets it holds, approving certain operations). It's executed during the transaction **verification phase**, not the application execution phase. Its success allows the transaction to proceed to execution if other conditions are met.

These special methods act as specific entry points triggered by the Neo protocol itself under defined circumstances.

## Calling Methods

Users or other contracts invoke your public static methods using tools, SDKs, or the `Contract.Call` method from within another smart contract. They need to know:

1.  Your contract's **Script Hash** (address).
2.  The **Method Name** (as defined in the manifest, respecting `DisplayName` if used).
3.  The required **Arguments** in the correct order and type.

[Previous: Contract Structure](./03-contract-structure.md) | [Next: Data Types](./05-data-types.md)