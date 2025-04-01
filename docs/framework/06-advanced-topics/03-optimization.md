# GAS Optimization

GAS is the fuel of the Neo network, and every operation within a smart contract consumes GAS. Optimizing your contract's GAS usage makes it cheaper for users to interact with, prevents hitting block/transaction GAS limits, and reduces overall network load.

## Understanding GAS Costs

*   **OpCode Costs:** Every NeoVM opcode has a base GAS cost.
*   **Interop Service Costs:** Calls to the framework services (`Runtime.*`, `Storage.*`, `Contract.*`, native contracts) translate to specific syscalls (interop opcodes), each with its own cost. These are generally more expensive than basic opcodes.
*   **Storage Costs:** `Storage.Put` costs GAS based on key length + value length. `Storage.Get` costs based on value length. `Storage.Find` costs per item iterated.
*   **Execution Fee Factor:** Costs are multiplied by a factor (`PolicyContract.GetExecFeeFactor()`) which can be adjusted by the Neo Council.

**(Important: Exact GAS costs can change. Always refer to the official Neo N3 documentation for the most current cost tables for opcodes and syscalls relevant to the network you are targeting).**

## Optimization Techniques

1.  **Minimize Storage Operations:** Storage is expensive.
    *   **Read Less:** Avoid reading from storage if the value isn't needed.
    *   **Write Less:** Only write to storage when state actually changes.
    *   **Cache Reads:** If a value from storage is needed multiple times within the *same execution*, read it once into a local variable instead of calling `Storage.Get` repeatedly.
    *   **Combine Writes:** If possible, structure logic to minimize the number of separate `Storage.Put` calls.

2.  **Optimize Storage Structure:**
    *   **Shorter Keys:** Use shorter keys or prefixes for `StorageMap` where possible.
    *   **Pack Data:** Instead of storing multiple boolean flags or small numbers as separate keys, consider packing them into a single `BigInteger` or `ByteString` using bit manipulation or careful serialization. This reduces the number of reads/writes but increases complexity.
    *   **Avoid Large Values:** Storing very large strings or byte arrays costs more.

3.  **Efficient Data Types & Operations:**
    *   **`BigInteger` vs. Primitives:** While `BigInteger` is necessary for balances, use standard C# integer types (`int`, `long`) for counters or values known to fit within their bounds, as operations might be slightly cheaper than `BigInteger` arithmetic.
    *   **`ByteString` vs `byte[]`:** Operations on immutable `ByteString` might sometimes be cheaper than creating/modifying mutable `byte[]` (Buffer).
    *   **Prefer `Helper` Methods:** Use `Helper.Concat`, `Helper.Range` etc., over manual byte manipulation in loops, as the helper methods often map to efficient opcodes.

4.  **Loop Optimization:**
    *   **Avoid Unbounded Loops:** Loops iterating based on user input or unbounded storage reads (`Storage.Find`) are dangerous and costly. Impose limits or redesign.
    *   **Minimize Work Inside Loops:** Perform calculations or storage access outside the loop whenever possible.

5.  **Contract Call Optimization:**
    *   **`Contract.Call` is Expensive:** Each call incurs significant overhead.
    *   **Restrict `CallFlags`:** Use the minimum necessary `CallFlags` (`ReadStates` is cheaper than `WriteStates` or `All`).
    *   **Batch Operations:** If calling another contract multiple times, see if the target contract offers batch methods to reduce call overhead.

6.  **Code Structure & Compiler (`nccs`):**
    *   **Use `O1` Optimization:** Ensure `<NeoBuildOptimization>O1</NeoBuildOptimization>` (usually default) is set in your `.csproj` for compiler optimizations like inlining.
    *   **Static Calls:** The compiler can often optimize direct static method calls within the same contract more effectively than dynamic calls.
    *   **Avoid Unnecessary Abstraction:** While good for readability, excessive layers of method calls *can* sometimes add overhead compared to more direct logic (measure if concerned).

7.  **Algorithmic Efficiency:**
    *   Choose efficient algorithms. A O(n^2) approach will cost significantly more GAS than a O(n log n) or O(n) approach for large inputs.

## Measuring GAS Usage

*   **Neo Express:** Tools like Neo Express often report the GAS consumed by invoked transactions in the local environment.
*   **TestNet/MainNet Explorers:** Block explorers show the GAS consumed by executed transactions.
*   **`Runtime.GasLeft`:** You can programmatically check `Runtime.GasLeft` at different points in your code during testing to understand the cost of specific sections (though this adds a small cost itself).

**Trade-offs:** Optimization often involves trade-offs between GAS cost, code complexity, and readability. Focus on optimizing the most frequently executed or inherently expensive operations (storage, complex loops, external calls). Premature optimization of minor details can sometimes obscure logic without significant GAS savings.

[Previous: Security Best Practices](./02-security.md) | [Next: Interop Layer](./04-interop.md)