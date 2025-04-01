# System Limits & Costs

Developing Neo smart contracts requires awareness of various system limits and associated GAS costs to ensure contracts are executable and economical.

**Note:** These values can be changed by the Neo Council via the `PolicyContract`. Always verify current values on the target network (MainNet/TestNet) if precision is critical.

## GAS Costs

*   **Execution Fees:** Every NeoVM opcode and Interop Syscall has an associated GAS cost. Complex operations, storage access, and contract calls are generally more expensive.
    *   Costs are calculated as `BaseCost * ExecFeeFactor / DefaultFactor` where `ExecFeeFactor` is set by policy (`PolicyContract.GetExecFeeFactor()`).
    *   Refer to the official [Neo N3 Opcode reference](https://developers.neo.org/docs/n3/reference/neovm/opcodes/stack) and Syscall documentation for base costs.
*   **Storage Fees (`PolicyContract.GetStoragePrice()`):**
    *   A fee (in GAS) is charged per byte of data stored (`key_length + value_length`).
    *   Paid when `Storage.Put` is called.
*   **Network Fees (`PolicyContract.GetFeePerByte()`):**
    *   Fee paid based on the byte size of the transaction itself.
    *   Compensates consensus nodes for including the transaction.
*   **System Fees:**
    *   Collected by the system for certain operations (like contract deployment, execution exceeding free limits) and are burned.

## Execution Limits

*   **Max GAS per Transaction:** Transactions have a GAS limit. If execution exceeds this, the transaction fails and state changes are reverted (though fees are still paid).
    *   There's a base amount of free GAS (~10 GAS) per transaction for basic operations.
*   **Max GAS per Block (`PolicyContract.GetMaxBlockSystemFee()`):** The total system fees collected by transactions within a single block cannot exceed this limit.
*   **Max Invocation Stack Depth:** NeoVM limits how deeply contracts can call each other (typically around 1024 levels) to prevent stack overflows.
*   **Max Item Size:** Limits on the size of individual items pushed onto the stack or stored (e.g., `ByteString` size).
*   **Max Stack Size:** Limits on the number of items on the NeoVM execution stack.

## Transaction & Block Limits

*   **Max Transactions per Block (`PolicyContract.GetMaxTransactionsPerBlock()`):** Limits the number of transactions a block can contain.
*   **Max Block Size (`PolicyContract.GetMaxBlockSize()`):** Limits the total byte size of a block.
*   **Transaction Size:** Individual transactions also have size limits.

## Other Limits

*   **Manifest Size:** The `.manifest.json` file has a size limit.
*   **NEF Size:** The `.nef` file has a size limit.
*   **Parameter Count:** Limits on the number of parameters a method can accept.

## Querying Limits On-Chain

You can use the `PolicyContract` native contract wrapper to query some of these limits directly within a smart contract or via RPC calls:

```csharp
long feePerByte = PolicyContract.GetFeePerByte();
uint storagePrice = PolicyContract.GetStoragePrice();
uint maxTxPerBlock = PolicyContract.GetMaxTransactionsPerBlock();
// etc.
```

**Implications:**

*   Design contracts to be GAS-efficient.
*   Be aware of potential limits when designing complex interactions or storing large amounts of data.
*   Ensure users attach sufficient GAS to cover execution and network fees.
*   Test GAS consumption using Neo Express or TestNet.

[Previous: Reference Overview](./README.md) | [Back to Main README](../README.md)