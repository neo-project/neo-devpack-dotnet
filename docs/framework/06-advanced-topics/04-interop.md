# Interop Layer

Neo smart contracts written in high-level languages like C# do not run directly on the bare metal or OS of the nodes. They execute within the Neo Virtual Machine (NeoVM), a sandboxed environment.

To interact with the blockchain state (storage, blocks, transactions) or perform actions beyond simple computation (logging, notifications, calling other contracts), the NeoVM relies on an **Interop Service Layer**.

The `Neo.SmartContract.Framework` acts as a C# bridge to this Interop Layer.

## NeoVM and Syscalls

*   **NeoVM:** Executes basic stack operations, control flow, arithmetic, etc., defined by standard NeoVM opcodes.
*   **Interop Layer:** Provides access to blockchain-specific functionality.
*   **Syscalls:** The NeoVM uses special `SYSCALL` opcodes to invoke functions within the Interop Layer. Each syscall corresponds to a specific service (e.g., `System.Storage.Put`, `System.Runtime.CheckWitness`, `System.Contract.Call`).
*   **Syscall Hashes:** Each syscall function is identified by a unique hash (a 4-byte integer).

## Framework Role

When you write C# code using the framework, for example:

```csharp
Storage.Put(Storage.CurrentContext, "mykey", "myvalue");
bool isOwner = Runtime.CheckWitness(ownerAddress);
Contract.Call(targetContract, "someMethod", CallFlags.All);
```

The `Neo.Compiler.CSharp` (`nccs`) translates these high-level C# method calls into a sequence of NeoVM opcodes that culminate in a `SYSCALL` instruction with the appropriate syscall hash corresponding to the underlying interop service (`System.Storage.Put`, `System.Runtime.CheckWitness`, `System.Contract.Call`, respectively).

**Essentially, the framework provides type-safe C# wrappers around the NeoVM syscalls.**

## Key Interop Service Categories (Conceptual)

The syscalls, and thus the framework methods, generally fall into these categories:

*   **Runtime Services (`System.Runtime.*`)**: Getting execution context (`ExecutingScriptHash`, `CallingScriptHash`), time (`GetTime`), trigger (`GetTrigger`), checking witnesses (`CheckWitness`), logging (`Log`), notifications (`Notify`), platform info.
*   **Storage Services (`System.Storage.*`)**: Putting, getting, deleting, and finding data in the contract's storage context (`Put`, `Get`, `Delete`, `Find`). Also includes context management (`GetContext`, `GetReadOnlyContext`).
*   **Contract Services (`System.Contract.*`)**: Calling other contracts (`Call`), managing contracts (`Create`, `Update`, `Destroy` - these map to `ContractManagement` native contract calls), checking methods (`GetCallFlags`).
*   **Iterator Services (`System.Iterator.*`)**: Creating and traversing iterators (used by `Storage.Find`).
*   **Callback Services (`System.Callback.*`)**: Creating callbacks (used internally).
*   **Native Contract Wrappers**: Classes like `GasToken`, `NeoToken`, `LedgerContract`, etc., wrap `System.Contract.Call` syscalls directed at the specific native contract hashes.
*   **Crypto Services (`Neo.Crypto.*`)**: Hashing (`Sha256`, `Ripemd160`) and signature verification (`CheckSig`, `CheckMultiSig` - these map to `CryptoLib` native contract calls).
*   **Serialization Services (`System.Binary.*`)**: Serialization/deserialization (`Serialize`, `Deserialize` - mapping to `StdLib` native contract calls).

## Implications

*   **Performance:** Syscalls generally cost more GAS than basic NeoVM opcodes due to the overhead of interacting with the underlying node state.
*   **Limitations:** You can only perform actions explicitly exposed via syscalls. You cannot directly access the node's filesystem, network (except via Oracles), or arbitrary memory.
*   **Framework Dependency:** Your C# contract code is tightly coupled to the `Neo.SmartContract.Framework` because it relies on these wrappers to generate the necessary syscalls.
*   **Abstraction:** The framework hides the complexity of managing the NeoVM stack, arguments, and syscall hashes directly, allowing developers to focus on application logic.

Understanding the Interop Layer helps clarify *why* certain operations are possible and others are not, and why framework methods translate into specific, costed blockchain interactions.

[Previous: GAS Optimization](./03-optimization.md) | [Next Section: Tutorials](../07-tutorials/README.md)