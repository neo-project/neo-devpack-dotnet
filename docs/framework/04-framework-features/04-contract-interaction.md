# Contract Interaction (`Contract.Call`)

A core strength of smart contract platforms is **composability** – the ability for contracts to call and interact with each other. Neo N3 enables this primarily through the static `Contract.Call` method, allowing your contract to invoke methods on other deployed contracts, including native contracts.

## `Contract.Call` Method Signature

Located in `Neo.SmartContract.Framework.Services.Contract`.

```csharp
// Standard method signature
public static extern object Call(UInt160 scriptHash, string method, CallFlags flags, params object[] args);
```

*   **`scriptHash` (`UInt160`)**: The address (script hash) of the target contract to be called. This must be known by the calling contract (it can be hardcoded with `[InitialValue]`, stored, or passed as an argument).
*   **`method` (`string`)**: The exact name of the **public static** method to invoke on the target contract. This name must match the one defined in the *target contract's manifest* (respecting any `[DisplayName]` attributes used there). Case sensitivity matters.
*   **`flags` (`CallFlags`)**: A crucial enum controlling the permissions and state access granted during the execution of the called method. See details below.
*   **`args` (`params object[]`)**: A variable number of arguments passed to the target method. These must match the expected parameter types and order defined in the target method's signature. Arguments must be valid NeoVM types.
*   **Return Value (`object`)**: The value returned by the called method. This will be a `StackItem` representing the result, which often needs to be **checked for null** and then **cast** to the expected C# type (e.g., `(BigInteger)result`, `(bool)result`, `(UInt160)result`).

## `CallFlags` Enum (Permissions)

This enum dictates what the *called* contract is allowed to do during its execution initiated by this `Call`. Choosing the correct flags is vital for security and functionality.

*   **`None` (0)**: No permissions granted. The called method can only perform basic computations using its arguments. Very restrictive.
*   **`ReadStates` (1)**: Allows reading state from the blockchain (e.g., its own storage via `Storage.Get`, other contract storage if permitted, `Runtime` info, `Ledger` info). Cannot write state or notify.
*   **`WriteStates` (2)**: Allows modifying state (e.g., `Storage.Put`, `Storage.Delete`). Implicitly includes `ReadStates`. Cannot call other contracts or notify.
*   **`AllowCall` (4)**: Allows the called contract to make further `Contract.Call` invocations to *other* contracts.
*   **`AllowNotify` (8)**: Allows the called contract to emit events using `Runtime.Notify`.
*   **Combined Flags (Bitwise OR):**
    *   **`States` (3)**: `ReadStates | WriteStates`. Allows reading and writing state.
    *   **`ReadOnly` (5)**: `ReadStates | AllowCall`. Allows reading state and making further *read-only* calls (useful for chaining view functions).
    *   **`All` (15)**: `ReadStates | WriteStates | AllowCall | AllowNotify`. Grants full permissions. Convenient but use cautiously – grants maximum capability to the called contract.

**Best Practice:** Always use the **most restrictive `CallFlags` necessary** for the target method's intended operation. If a method only needs to return a value from its storage, use `CallFlags.ReadStates`. If it needs to update its state and call another contract, use `CallFlags.WriteStates | CallFlags.AllowCall` (or `CallFlags.All` if notifications are also needed). Using unnecessarily broad flags increases potential security risks if the target contract is malicious or buggy.

## Dynamic Calls (`Contract.Call` with Variable Hash)

While you often call known contracts (like native contracts or specific partner contracts), you can also call contracts whose script hashes are determined dynamically at runtime (e.g., passed as arguments, retrieved from storage).

```csharp
// Example: Calling a dynamically provided token contract
public static bool ForwardTransfer(UInt160 tokenContractHash, UInt160 from, UInt160 to, BigInteger amount)
{
    Helper.Assert(Runtime.CheckWitness(from), "Sender must sign");
    Helper.Assert(tokenContractHash.IsValid && !tokenContractHash.IsZero, "Invalid token hash");

    // Call the transfer method on the provided token contract hash
    object result = Contract.Call(tokenContractHash, "transfer", CallFlags.All, from, to, amount, null);

    return result is bool && (bool)result;
}
```
**Security Warning:** Be extremely careful when calling dynamically determined contracts. Ensure proper validation of the `tokenContractHash` and understand the potential risks of interacting with arbitrary, potentially malicious code.

## Contract Permissions (`[ContractPermission]`)

For a `Contract.Call` to succeed, the *calling* contract (your contract) must have permission declared in its *own* manifest to interact with the specified target contract and method. This acts as a firewall defined at deployment time.

```csharp
// In MyCallingContract.cs

// Grant permission to call 'balanceOf' on the GasToken native contract
[ContractPermission(nameof(GasToken), "balanceOf")]

// Grant permission to call 'transfer' and 'approve' on a specific NEP-17 token hash
[ContractPermission("0x123abc...", "transfer", "approve")]

// Grant permission to call *any* method ("*") on a specific Oracle contract
[ContractPermission("0x456def...", "*")]

// Grant permission to call a specific method ("processData") on *any* contract ("*")
// Use wildcards carefully!
[ContractPermission("*", "processData")]

// Grant permission to call any method on any contract (Least secure, generally avoid)
// [ContractPermission("*", "*")]

public class MyCallingContract : SmartContract
{
    // ... methods using Contract.Call ...
}
```
*   If the required permission is missing from the calling contract's manifest, the `Contract.Call` attempt will **fail at runtime**, causing the transaction to FAULT.
*   Specify the **minimum necessary permissions**. Avoid blanket `"*", "*"` permissions unless absolutely required and the implications are fully understood.

## Error Handling & Return Values

*   `Contract.Call` returns an `object` which represents the `StackItem` returned by the target method.
*   **Failure Modes:** The call can fail silently (return `null`) or throw an exception (causing the calling transaction to FAULT) if:
    *   The target `scriptHash` does not exist.
    *   The target `method` does not exist or has mismatching parameters.
    *   The provided `CallFlags` are insufficient for the operations performed by the target method.
    *   The calling contract lacks the necessary `[ContractPermission]` in its manifest.
    *   An `Assert` fails or an unhandled exception occurs within the *called* contract.
    *   Insufficient GAS is available for the call's execution.
*   **Return Value Checking:** **ALWAYS** check the return value of `Contract.Call` before attempting to use or cast it.
    *   Check for `null` if the call might legitimately fail or return nothing.
    *   Use `Helper.Assert(result != null, "...")` if the call is expected to succeed and return a value.
    *   Safely cast the result: `if (result is BigInteger) { value = (BigInteger)result; } else { /* handle error */ }`.

```csharp
// Example: Robustly handling Contract.Call return
[Safe] // Assume GetInfo is read-only
public static string GetTokenNameFromHash(UInt160 tokenHash)
{
    object result = Contract.Call(tokenHash, "symbol", CallFlags.ReadStates); // Read-only call

    // Check 1: Did the call itself fail/return null?
    if (result == null)
    {
        Runtime.Log("Call to get symbol failed or returned null.");
        return "Error: Call Failed";
    }

    // Check 2: Is the result the expected type? (Optional but safer)
    if (result is ByteString || result is string) // Symbol could be ByteString or String
    {
        return Helper.AsString((ByteString)result); // Cast carefully
    }
    else
    {
        Runtime.Log("Call returned unexpected type for symbol.");
        return "Error: Invalid Type";
    }
}
```

## GAS Considerations

*   `Contract.Call` has a significant base GAS cost.
*   The execution of the *called* method also consumes GAS, which is charged to the *original transaction*.
*   Ensure the initial transaction includes enough GAS to cover the costs of both the calling contract and any contracts it calls via `Contract.Call`.

Contract interaction is fundamental to the Neo ecosystem, enabling complex dApps and protocols. However, it requires diligent attention to addresses, method names, arguments, `CallFlags`, permissions, return value checking, and GAS costs.

[Previous: Events](./03-events.md) | [Next: Native Contracts](./05-native-contracts/README.md)