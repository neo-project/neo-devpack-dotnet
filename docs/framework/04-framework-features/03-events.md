# Events (`Runtime.Notify`)

Smart contract events are the standard way for contracts to broadcast information about state changes or significant occurrences to the outside world. Off-chain applications, explorers, and potentially other contracts can listen for these events to track activity and react accordingly.

In the `Neo.SmartContract.Framework`, events are defined using standard C# delegate and event syntax, which the compiler translates into `Runtime.Notify` syscalls.

## Defining Events

1.  **Declare a Delegate:** Define a `delegate` (typically derived from `System.Action<...>` or a custom delegate) specifying the signature of your event (the number and types of its parameters). Choose parameter types that are valid NeoVM types (e.g., `UInt160`, `BigInteger`, `string`, `ByteString`, `bool`).
2.  **Declare an Event:** Declare a `public static event` using the delegate type.
3.  **`[DisplayName]` Attribute:** **Crucially**, apply the `[DisplayName("MyEventName")]` attribute to the event declaration. This defines the `eventName` string that will be emitted in the `Runtime.Notify` call and recorded in the contract manifest's ABI section. This name is how external applications identify and subscribe to your event. Choose clear, descriptive names (camelCase is a common convention).

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics; // For BigInteger
using System.ComponentModel; // For DisplayName

namespace Neo.SmartContract.Examples
{
    public class EventDemo : SmartContract
    {
        // 1. Delegate Definitions (can reuse System.Action or define custom)
        // public delegate void TransferredDelegate(UInt160 from, UInt160 to, BigInteger amount);
        // public delegate void ApprovalDelegate(UInt160 owner, UInt160 spender, BigInteger amount);

        // 2. Event Declarations with DisplayName (Essential!)
        [DisplayName("Transfer")] // Event name for off-chain listeners
        public static event System.Action<UInt160, UInt160, BigInteger> OnTransfer;
        // Using System.Action<...> is often convenient

        [DisplayName("Approval")]
        public static event System.Action<UInt160, UInt160, BigInteger> OnApproval;

        [DisplayName("ItemAdded")]
        public static event System.Action<BigInteger, string, UInt160> OnItemAdded; // ItemID, Description, Owner

        // ... rest of contract logic ...
    }
}
```

## Firing Events

To fire (emit) an event, simply call it like a static method from within your contract code, passing arguments matching the delegate signature.

```csharp
// ... Inside a Transfer method ...
BigInteger amount = 100;
UInt160 from = GetSender();
UInt160 to = GetRecipient();

// Update balances...
Balances.Put(from, currentFromBalance - amount);
Balances.Put(to, currentToBalance + amount);

// 3. Fire the event - this compiles to Runtime.Notify("Transfer", from, to, amount)
OnTransfer(from, to, amount);
```

When `OnTransfer(...)` is executed:
1.  The C# compiler translates this into `Runtime.Notify("Transfer", from, to, amount)`.
2.  The `eventName` is "Transfer" (from `[DisplayName]`).
3.  The `state` passed to `Runtime.Notify` is an array of `StackItem`s: `[from, to, amount]`.
4.  This notification (containing the event name and state array) is recorded in the Application Log of the successful transaction.

## Events vs. `Runtime.Log`

*   **`Runtime.Log`:**
    *   Intended primarily for **debugging** or simple informational messages during development.
    *   Emits only a single string message.
    *   Less structured, making reliable parsing by off-chain tools difficult.
    *   Consumes slightly less GAS than `Notify`.
*   **Events (`Runtime.Notify`)**: 
    *   The **standard mechanism** for signalling significant state changes or actions.
    *   Emits a structured notification: `(ExecutingContractHash, EventName, StateArray)`.
    *   The structure (`EventName` and parameter types) is defined in the manifest, allowing robust parsing and subscription by external tools.
    *   **Use events for all state changes or actions that external applications might need to track.**

## Event Parameters and Indexing

*   **Parameter Types:** Can be any valid NeoVM data type, including primitives, `ByteString`, `UInt160/256`, `ECPoint`, `Map`, `List`, and serializable structs.
*   **Limits:** There are limits on the size of the `state` array and the total size of the notification. Avoid overly large or deeply nested data in event parameters.
*   **Indexing:** Off-chain applications often need to filter events based on specific parameters (e.g., find all `Transfer` events *to* a specific address). To facilitate this, consider making key identifiers (like addresses, token IDs) **primary parameters** in your event signature. While Neo nodes don't automatically index all parameters like some other chains, well-designed event structures and dedicated indexer services make filtering much easier.
    *   *Bad Example:* `OnDataStored(BigInteger recordId, Map<string, object> allData)` - Hard to filter for events related to a specific user within `allData`.
    *   *Good Example:* `OnDataStored(UInt160 user, BigInteger recordId, string dataType)` - Easier to filter for events related to a specific `user`.

## Listening to Events Off-Chain

External applications (dApps, backend services, indexers) monitor the blockchain for `Runtime.Notify` calls.

*   **SDKs (Neon.js, neow3j, etc.):** Provide methods to:
    *   Fetch the Application Log for a specific transaction (`getapplicationlog` RPC call). This log contains all `Notify` calls (and `Log` messages) from that transaction.
    *   Subscribe to new blocks and process the Application Logs of included transactions.
    *   Filter logs based on the emitting contract hash and potentially the `eventName`.
*   **Indexers/Middleware:** Services often run dedicated indexers that monitor the chain, parse Application Logs, decode event `state` arrays based on contract manifests (ABIs), and store the structured event data in a queryable database for dApps to consume easily.

Designing clear, well-structured events with appropriate parameters and distinct names is crucial for building interactive and observable smart contracts and dApps on Neo.

[Previous: Runtime Services](./02-runtime.md) | [Next: Contract Interaction](./04-contract-interaction.md)