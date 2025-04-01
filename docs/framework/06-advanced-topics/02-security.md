# Security Best Practices

Writing secure smart contracts is paramount, as vulnerabilities can lead to significant financial loss or unexpected behavior. This section outlines common pitfalls and best practices for Neo C# smart contract development.

## 1. Access Control (`Runtime.CheckWitness`)

*   **Problem:** Methods that modify critical state or transfer assets must be protected.
*   **Solution:** **Always** use `Runtime.CheckWitness(authorizedAccount)` to verify that the intended user/admin/contract has signed the transaction before performing sensitive operations.
*   **Pitfall:** Forgetting `CheckWitness` or checking against the wrong account.
*   **Pitfall:** Relying solely on `Runtime.Transaction.Sender`. While often the sender *is* the signer you want, `CheckWitness` is the explicit way to check transaction signatures.

```csharp
// Bad: Anyone can call this and drain funds
public static bool UnsafeWithdraw(UInt160 to, BigInteger amount)
{
    return GasToken.Transfer(Runtime.ExecutingScriptHash, to, amount, null);
}

// Good: Only the owner can withdraw
private static readonly byte[] OwnerKey = { 0x01 }; 
public static bool SafeWithdraw(UInt160 to, BigInteger amount)
{
    ByteString owner = Storage.Get(Storage.CurrentContext, OwnerKey);
    Helper.Assert(owner != null, "Owner not set");
    if (!Runtime.CheckWitness((UInt160)owner)) return false; // Check owner signature
    return GasToken.Transfer(Runtime.ExecutingScriptHash, to, amount, null);
}
```

## 2. Reentrancy Attacks

*   **Problem:** When a contract calls an external contract (`Contract.Call`), the external contract might call back into the original contract *before* the original call finishes. If state updates haven't completed, the reentrant call might exploit an inconsistent state.
*   **Mitigation:**
    *   **Checks-Effects-Interactions Pattern:** Perform checks first, then update internal state (effects), and *only then* interact with external contracts.
    *   **Reentrancy Guards:** Use a storage flag (e.g., `isLocked`) to prevent reentrant calls to sensitive functions while one is already in progress.
    *   **Limit External Calls:** Minimize interactions with untrusted external contracts, especially within functions that modify critical state.
    *   **Use `CallFlags.ReadOnly`:** When calling external contracts just to read data, use restrictive flags like `CallFlags.ReadStates` or `CallFlags.ReadOnly` to prevent the external contract from writing state or calling back unexpectedly (if it respects the flags).

```csharp
// Vulnerable to Reentrancy
private static readonly StorageMap Balances = new StorageMap(Storage.CurrentContext, "BAL");
public static bool VulnerableWithdraw(UInt160 recipient, BigInteger amount)
{
    if (!Runtime.CheckWitness(Runtime.ExecutingScriptHash)) return false; // Assume caller is contract owner
    BigInteger balance = (BigInteger)Balances.Get(Runtime.ExecutingScriptHash);
    Helper.Assert(balance >= amount, "Insufficient balance");

    // Interaction BEFORE effect
    bool success = (bool)Contract.Call(recipient, "receivePayment", CallFlags.All, amount); 
    Helper.Assert(success, "External call failed");

    // Effect: Update balance AFTER external call
    Balances.Put(Runtime.ExecutingScriptHash, balance - amount); 
    return true;
}

// Safer: Checks-Effects-Interactions
private static bool locked = false; // Simple reentrancy guard (in-memory, better to use storage)
public static bool SaferWithdraw(UInt160 recipient, BigInteger amount)
{
    // Check
    if (!Runtime.CheckWitness(Runtime.ExecutingScriptHash)) return false;
    Helper.Assert(!locked, "Reentrancy detected"); // Check guard
    BigInteger balance = (BigInteger)Balances.Get(Runtime.ExecutingScriptHash);
    Helper.Assert(balance >= amount, "Insufficient balance");

    // Effects
    Balances.Put(Runtime.ExecutingScriptHash, balance - amount); // Update balance first
    locked = true; // Set guard

    // Interaction
    bool success = false;
    try {
        success = (bool)Contract.Call(recipient, "receivePayment", CallFlags.All, amount);
    } finally {
        locked = false; // Unset guard even if call fails
    }

    Helper.Assert(success, "External call failed"); // Check result AFTER state change
    return true;
}
```
*(Note: Simple boolean guard shown; persistent storage guard is more robust)*

## 3. Integer Overflow/Underflow

*   **Problem:** Arithmetic operations on bounded integer types can wrap around (overflow/underflow) leading to incorrect calculations (e.g., large balances becoming small).
*   **Solution:** `BigInteger` (which represents arbitrarily large integers) is the standard for balances and critical arithmetic in Neo contracts and **does not overflow/underflow** in the traditional sense. Always use `BigInteger` for token amounts and sensitive calculations.
*   **Pitfall:** Using standard C# types like `int` or `long` for values that could exceed their bounds.

## 4. Input Validation (`Helper.Assert`)

*   **Problem:** Incorrect or malicious inputs can lead to unexpected behavior or state corruption.
*   **Solution:** Use `Helper.Assert` liberally to validate:
    *   Input arguments (e.g., amounts > 0, addresses are valid and not zero).
    *   State preconditions (e.g., balance >= amount).
    *   Return values from external calls.
*   **Pitfall:** Assuming inputs are always valid.

## 5. Incorrect Calculation / Logic Errors

*   **Problem:** Standard programming bugs in calculations or state transitions.
*   **Solution:** Rigorous testing (unit tests, simulation testing), code reviews, and clear, simple logic.

## 6. Timestamp Dependence

*   **Problem:** Relying on `Runtime.Time` for critical logic can be manipulated by consensus nodes to some extent.
*   **Solution:** Avoid using `Runtime.Time` as the *sole* factor for critical decisions like unlocking funds or determining winners. Use block height (`LedgerContract.CurrentIndex`) or external oracles for more reliable time-based logic if needed.

## 7. GAS Issues & Denial of Service (DoS)

*   **Problem:** Methods consuming excessive GAS can become unusable.
*   **Problem:** Malicious users might exploit operations that loop over user-provided data, causing excessive GAS usage (DoS).
*   **Solution:**
    *   Be mindful of GAS costs ([GAS Optimization](./03-optimization.md)).
    *   Avoid unbounded loops. If looping over data, ensure there are limits (e.g., process only N items per call).
    *   Consider if operations need to be pausable or require specific roles to execute.
    *   Ensure users provide sufficient GAS for operations.

## 8. Unhandled Exceptions

*   **Problem:** An unhandled exception (e.g., from `Helper.Assert`, failed casts, division by zero) will cause the entire transaction to fail, rolling back all state changes made within that transaction.
*   **Solution:** Use assertions (`Helper.Assert`) for conditions that *must* be true. Handle expected error conditions gracefully (e.g., return `false` from a method) rather than throwing exceptions unless failure should revert the entire transaction.

## 9. Oracle Security

*   **Problem:** Relying on data from Oracles requires trusting the Oracle nodes and the data source.
*   **Solution:**
    *   **Verify Callback Caller:** Crucially, always check `Runtime.CallingScriptHash == OracleContract.Hash` in your `__callback` method.
    *   **Use Multiple Oracles/Sources:** For critical data, consider requesting from multiple sources and aggregating/validating results.
    *   **Validate Data:** Sanitize and validate data received from oracles before using it.

## 10. Upgrade Security

*   **Problem:** Flaws in the `update` method logic can allow unauthorized contract updates.
*   **Solution:** Ensure the `update` method robustly checks the authorization (e.g., `Runtime.CheckWitness` against a secure owner/admin address or multi-sig).

**General Principles:**

*   **Keep it Simple:** Complexity breeds bugs.
*   **Least Privilege:** Grant minimal permissions (`[ContractPermission]`, `CallFlags`).
*   **Test Thoroughly:** Cover edge cases, failures, and security scenarios.
*   **Code Reviews:** Have others review your contract logic.
*   **Stay Updated:** Be aware of newly discovered vulnerabilities and best practices in the Neo ecosystem.

[Previous: Contract Upgrade & Migration](./01-contract-upgrade.md) | [Next: GAS Optimization](./03-optimization.md)