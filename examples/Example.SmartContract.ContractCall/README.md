# Neo N3 Contract Call Smart Contract Example

## Overview
This example demonstrates how to implement contract-to-contract calls in Neo N3 smart contracts. Contract calls enable powerful composability, allowing smart contracts to interact with and leverage functionality from other contracts on the blockchain, creating modular and extensible decentralized applications.

## Key Features
- Contract-to-contract method invocation
- Call flag specification
- Parameter passing between contracts
- Static contract reference declaration
- NEP-17 payment handling

## Technical Implementation
The `SampleContractCall` contract demonstrates several key aspects of contract calls:

### Contract References
The example showcases how to:
- Declare static references to other contracts using the `[Hash160]` attribute
- Specify contract addresses in a compile-time safe manner
- Reference the calling contract and executing contract

### Method Invocation
The contract demonstrates:
- Calling methods on other contracts using `Contract.Call`
- Specifying appropriate call flags
- Passing multiple parameters to the target contract
- Handling return values from contract calls

### Call Context
The example shows how to access important context information:
- Obtaining the executing script hash (the contract's own address)
- Identifying the calling script hash (the address of the contract that initiated the call)
- Using appropriate call flags for different operations

## Contract Call Patterns
The example demonstrates a common pattern in Neo N3 development:
1. **Receiving NEP-17 Payments**: Implementing the `onNEP17Payment` method to handle token transfers
2. **Checking the Token Balance**: Calling back to the token contract to verify the balance
3. **Forwarding Information**: Calling another contract with the payment and balance information

## Call Flags
The example uses `CallFlags.All` to specify the permissions granted to the called contract:
- Allows read operations
- Allows write operations
- Allows state changes

For more restricted calls, other flags could be used:
- `CallFlags.ReadOnly`: For operations that only read state
- `CallFlags.ReadStates`: For operations that only read blockchain state
- `CallFlags.WriteStates`: For operations that write blockchain state
- `CallFlags.AllowCall`: For operations that may call other contracts
- `CallFlags.AllowNotify`: For operations that may generate notifications

## Use Cases
Contract calls enable numerous use cases:
- Composable DeFi protocols
- Multi-contract governance systems
- Cross-contract token transfers
- Modular smart contract architectures
- Contract upgradability patterns

## Security Considerations
When implementing contract calls:
- Always validate the calling contract when handling sensitive operations
- Use appropriate call flags to limit the permissions granted
- Verify return values from external contract calls
- Guard against re-entrancy attacks
- Consider gas consumption for complex call patterns

## Educational Value
This example teaches:
- How to make contract-to-contract calls in Neo N3
- Proper handling of contract addresses and references
- Context management in contract interactions
- NEP-17 token reception patterns
- Parameter passing between contracts

Contract calls are a fundamental building block for creating complex and interconnected decentralized applications on Neo N3.