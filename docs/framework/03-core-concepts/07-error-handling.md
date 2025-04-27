# Error Handling in Neo N3 Contracts

Handling errors correctly is crucial for smart contract security and reliability. Unlike traditional C# applications, standard `try-catch` blocks are generally **not recommended** in Neo N3 smart contracts due to their high and potentially unpredictable GAS costs and the way exceptions interact with the NeoVM's execution model.

Instead, Neo contracts typically use two primary mechanisms:

1.  **`Helper.Assert`:** For validating critical conditions and invariants. Failure causes the *entire transaction* to fail and revert.
2.  **Boolean Return Values (or specific error indicators):** For recoverable errors or conditions where the transaction should succeed but indicate a specific outcome.

## `Helper.Assert(bool condition, string? message = null)`

This is the most common way to enforce essential requirements.

*   **Purpose:** Checks if a `condition` is true. If the `condition` is **false**, the NeoVM throws a `FAULT` state, immediately halting execution.
*   **Effect:** The entire transaction fails. All state changes made during the transaction (storage puts, token transfers, event emissions) are **reverted**. GAS fees paid for the transaction are still consumed.
*   **Use Cases:**
    *   Validating critical input parameters (e.g., non-zero amount, valid addresses).
    *   Checking authorization (`Runtime.CheckWitness`).
    *   Ensuring state consistency (e.g., sufficient balance before transfer).
    *   Verifying return values from critical external calls.
*   **`message` (Optional):** Provides a string message that might be included in the exception details visible in some tools or logs, aiding debugging.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class AssertDemo : SmartContract
{
    public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount)
    {
        // CRITICAL CHECKS using Assert
        Helper.Assert(from.IsValid && !from.IsZero, "Invalid 'from' address");
        Helper.Assert(to.IsValid && !to.IsZero, "Invalid 'to' address");
        Helper.Assert(amount > 0, "Transfer amount must be positive");
        Helper.Assert(Runtime.CheckWitness(from), "CheckWitness failed for 'from' address");

        StorageMap balances = new StorageMap(Storage.CurrentContext, "BAL");
        BigInteger fromBalance = (BigInteger)balances.Get(from);
        Helper.Assert(fromBalance >= amount, "Insufficient balance");

        // If any Assert fails, execution stops here, transaction reverts.

        // Proceed with state changes only if all asserts pass
        if (from != to) 
        { 
             balances.Put(from, fromBalance - amount);
             balances.Put(to, (BigInteger)balances.Get(to) + amount);
        }
        Runtime.Notify("Transfer", from, to, amount); // Event only emitted if successful
        return true; // Indicates success
    }
}
```

## Boolean Return Values & Error Indicators

For situations where a failure doesn't necessarily mean the entire transaction should revert, returning `false` (or another indicator like `null` or `-1`) is common.

*   **Purpose:** Signals to the caller (another contract or an off-chain application) that the requested operation could not be completed successfully under the given conditions, but the transaction itself might still be valid.
*   **Effect:** The transaction completes successfully. State changes made *before* the point of failure are **not** reverted (unless an `Assert` or unhandled exception occurs later).
*   **Use Cases:**
    *   Optional operations failing (e.g., trying to vote when voting has closed, but transaction shouldn't fail).
    *   Conditions that the caller might be able to handle or retry (e.g., checking if a name is available).
    *   Indicating specific non-critical outcomes.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

public class ReturnValueDemo : SmartContract
{
    private static readonly StorageMap Votes = new StorageMap(Storage.CurrentContext, "VOTE");
    private static readonly uint VotingEndBlock = 1000;

    public static bool CastVote(UInt160 voter, int option)
    {
        Helper.Assert(Runtime.CheckWitness(voter), "CheckWitness failed");

        // Use boolean return for recoverable/expected conditions
        if (LedgerContract.CurrentIndex >= VotingEndBlock) 
        { 
            Runtime.Log("Voting period ended");
            return false; // Indicate failure, but transaction can succeed
        }
        if (Votes.Get(voter) != null)
        {
            Runtime.Log("Already voted");
            return false; // Indicate failure (already voted)
        }

        // Proceed if checks pass
        Votes.Put(voter, option);
        Runtime.Notify("Voted", voter, option);
        return true; // Indicate success
    }
}
```

## Why Avoid `try-catch`?

While the Neo C# compiler *can* technically process standard C# `try-catch` blocks, their use in smart contracts is **strongly discouraged** for several reasons:

1.  **High & Unpredictable GAS Cost:** Implementing exception handling mechanisms requires significant NeoVM instruction overhead. This makes contracts much more expensive to execute, even if no exception is actually thrown during runtime. The exact GAS cost can be difficult to predict.
2.  **Complexity:** Translating C# exception handling semantics (stack unwinding, filter blocks, finally blocks) into deterministic NeoVM bytecode is complex and can lead to subtle, hard-to-debug behaviors.
3.  **State Reversion Conflict:** `Helper.Assert` provides clear, atomic transaction failure – the entire transaction's state changes are reverted. This is often the desired behavior for critical errors in smart contracts. Using `try-catch` can potentially allow parts of a transaction to succeed even if another part within the `try` block fails, which might lead to an inconsistent or unexpected final state if not managed with extreme care.
4.  **Framework Design:** The `Neo.SmartContract.Framework` is designed around the `Assert` and boolean return value patterns for error handling, making them the idiomatic and expected approach.

Stick to `Helper.Assert` for conditions that must hold true (invariants, authorization) and boolean/specific return values for recoverable or expected failure conditions.

## Handling `Contract.Call` Failures

Calls to other contracts (`Contract.Call`) can fail for various reasons (invalid method/contract, insufficient GAS, permission denied, assertion failure in the called contract). These calls often return `null` or throw an exception that might halt execution.

Always check the return value of `Contract.Call` before using it, often using `Helper.Assert` if the call is critical or a simple conditional check if it's optional.

```csharp
// Check result of external call
object result = Contract.Call(targetHash, "someMethod", CallFlags.ReadOnly);
Helper.Assert(result != null, "External call failed or returned null");
BigInteger value = (BigInteger)result; // Proceed only if assert passes
```

By prioritizing `Helper.Assert` for invariants and boolean returns for recoverable conditions, you can write robust and predictable error-handling logic suitable for the Neo N3 blockchain environment.

[Previous: Deployment Artifacts (NEF & Manifest)](./06-deployment-files.md) | [Next Section: Framework Features](../../04-framework-features/README.md)