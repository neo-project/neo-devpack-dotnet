# Neo N3 HelloWorld Smart Contract Example

## Overview
This example demonstrates the simplest possible smart contract on the Neo N3 blockchain. It provides a basic starting point for developers new to Neo smart contract development and showcases the minimal structure required for a functional contract.

## Key Features
- Minimal contract implementation
- Basic method declaration
- Safe method attribute usage
- Contract metadata annotation

## Technical Implementation
The `HelloWorld` contract demonstrates several fundamental Neo N3 features:
- Contract attribute decorations for metadata
- Safe method annotation for read-only operations
- Basic method implementation
- String return type handling

### Contract Attributes
The example showcases essential contract attributes:
- `DisplayName`: Human-readable name for the contract
- `ContractDescription`: Brief description of the contract's purpose
- `ContractEmail`: Contact information for the contract author
- `ContractVersion`: Version information for the contract
- `ContractSourceCode`: Link to the source code repository
- `ContractPermission`: Permission settings for contract interactions

### Safe Method
The example includes a method marked with the `[Safe]` attribute, indicating that:
- The method does not modify blockchain state
- It can be called without incurring GAS fees
- It performs read-only operations

## Usage
This contract can be used as:
- A starting template for new Neo N3 developers
- A verification that the Neo development environment is set up correctly
- A baseline for more complex contract development

## Deployment
The contract can be compiled and deployed using the Neo SDK tools. Once deployed, the `SayHello` method can be invoked to retrieve the "Hello, World!" message.

## Testing
The contract functionality can be tested by invoking the `SayHello` method and verifying that it returns the expected string.

## Educational Value
This example teaches:
- Basic Neo N3 contract structure
- Proper use of contract attributes
- Implementation of read-only methods
- String handling in Neo smart contracts