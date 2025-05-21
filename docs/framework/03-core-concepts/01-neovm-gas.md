# NeoVM & GAS

Two fundamental concepts in Neo smart contract development are the Neo Virtual Machine (NeoVM) and GAS.

## Neo Virtual Machine (NeoVM)

The NeoVM is the execution engine for Neo smart contracts. It's a lightweight, stack-based virtual machine designed specifically for blockchain environments.

*   **Deterministic Execution:** Given the same contract code and input state, the NeoVM will always produce the same output. This is crucial for blockchain consensus.
*   **Bytecode:** Smart contracts written in high-level languages like C# are compiled into NeoVM bytecode (`.nef` file). This bytecode is what actually runs on the Neo nodes.
*   **Stack-Based:** Operations manipulate data primarily on a stack.
*   **OpCodes:** The VM executes a specific set of instructions called Opcodes (Operation Codes). Each opcode performs a small task (e.g., push data onto the stack, perform arithmetic, access storage, call another contract).
*   **Interop Service Layer:** Provides a bridge between the NeoVM and the underlying blockchain state (e.g., accessing storage, getting current block time, checking witness signatures). The `Neo.SmartContract.Framework` provides C# wrappers for these interop services.
*   **Resource Limits:** Execution is constrained by GAS limits to prevent infinite loops or excessive resource consumption.

Understanding that your C# code ultimately translates into NeoVM opcodes helps in writing efficient and optimized contracts.

## GAS Token

GAS is one of the two native tokens on the Neo blockchain (the other being NEO). It serves as the fuel for the network.

*   **Network Fees:** Users pay GAS to execute transactions and deploy/invoke smart contracts. This compensates the nodes (Consensus Nodes) for processing and storing data.
*   **Execution Cost:** Every NeoVM opcode has an associated GAS cost. More complex operations or those consuming more resources (like storage) cost more GAS.
*   **System Fees & Network Fees:** Transaction fees are divided into System Fees (burned) and Network Fees (distributed to Consensus Nodes).
*   **Preventing Abuse:** The GAS mechanism prevents malicious actors from spamming the network or running computationally expensive code indefinitely, as they would need to pay for the resources consumed.
*   **Generation:** GAS is generated over time and distributed to NEO token holders.

**Implications for Developers:**

*   **Efficiency Matters:** Writing GAS-efficient code is crucial. Unoptimized code can make your contract expensive or even impossible to use if the required GAS exceeds transaction or block limits.
*   **Cost Awareness:** Be mindful of the GAS cost of different framework methods and opcodes, especially storage operations (`Storage.Put`, `Storage.Find`) and complex computations.
*   **Fee Management:** Users invoking your contract must attach enough GAS to cover the system and network fees for the operations performed.

[Previous: Core Concepts](./README.md) | [Next: Transactions](./02-transactions.md)