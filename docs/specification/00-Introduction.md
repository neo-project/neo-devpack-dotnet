# 1. Introduction

## 1.1 Purpose

This document provides a formal specification for Neo N3 smart contracts, defining the structure, behavior, capabilities, and constraints of smart contracts on the Neo N3 blockchain platform. This specification serves as a comprehensive reference for developers, auditors, and researchers working with Neo N3 smart contracts.

### 1.1.1 Target Audience

This specification is intended for:

- **Smart Contract Developers**: Building decentralized applications on Neo N3
- **Security Auditors**: Reviewing smart contract implementations
- **Blockchain Researchers**: Understanding Neo N3 contract capabilities
- **Tool Developers**: Creating development tools and frameworks
- **Technical Writers**: Documenting Neo N3 applications

### 1.1.2 Neo N3 Smart Contract Platform

The Neo N3 smart contract platform enables developers to build decentralized applications using mainstream programming languages, with C# being the primary supported language. This specification focuses exclusively on the C# implementation of Neo N3 smart contracts.

### 1.1.3 Compiler and Toolchain

The Neo C# Compiler (`nccs`) compiles C# source code into Neo Executable Format (NEF) bytecode that executes on the NeoVM. The compiler provides:

- **Modern C# Support**: Latest C# language features and syntax
- **Interface Generation**: Automatic generation of type-safe contract interfaces
- **Optimization Levels**: Multiple optimization strategies for performance
- **Debug Information**: Comprehensive debugging and analysis capabilities

## 1.2 Scope

This specification covers:

- Smart contract structure and syntax
- Contract lifecycle (deployment, execution, update, destruction)
- Execution model and virtual machine
- Storage and state management
- Events and notifications
- Contract interaction mechanisms
- Security considerations and constraints
- Standard interfaces (NEP standards)
- Testing framework and interface generation
- Compiler features and capabilities
- Supported C# syntax and features
- Native contracts and their APIs

This specification does not cover:
- The Neo N3 blockchain protocol
- The Neo N3 consensus mechanism
- The Neo N3 network protocol
- The Neo N3 wallet and account system
- The Neo N3 token economics

## 1.3 Terminology

- **Smart Contract**: A self-executing program deployed on the Neo blockchain that enforces the terms of an agreement.
- **NeoVM**: The Neo Virtual Machine that executes smart contract bytecode.
- **NEF**: Neo Executable Format, the binary format for compiled Neo smart contracts.
- **Manifest**: A JSON document that describes a smart contract's properties, permissions, and interfaces.
- **Syscall**: A system call that provides access to blockchain functionality from within a smart contract.
- **GAS**: The utility token of the Neo blockchain used to pay for transaction fees and smart contract execution.
- **NEP**: Neo Enhancement Proposal, a design document providing information or describing new features for Neo.
- **Datoshi**: The smallest unit of GAS, where 1 GAS = 10^8 datoshi.
- **Storage Context**: The scope in which storage operations are performed.
- **Call Flags**: Permissions granted to a contract call.
- **Trigger Type**: The context in which a contract is executed.
- **Witness**: An entity that authorizes a transaction or operation.
- **Script Hash**: A 160-bit hash that uniquely identifies a contract or account.
- **UInt160**: A 160-bit unsigned integer, typically used to represent script hashes.
- **UInt256**: A 256-bit unsigned integer, typically used to represent transaction hashes or block hashes.
- **ByteString**: A sequence of bytes, used to represent binary data.
- **Iterator**: An object that allows traversal of a collection of items.
- **Native Contract**: A contract that is built into the Neo N3 platform.
- **Reentrancy**: A vulnerability where a contract can be called recursively before the first call completes.

## 1.4 Document Conventions

This document uses the following conventions:

- Code examples are provided in C# syntax.
- EBNF (Extended Backus-Naur Form) is used for formal syntax definitions.
- Tables are used to present structured information.
- Notes and warnings are highlighted to draw attention to important information.

## 1.5 References

1. [Neo Whitepaper](https://docs.neo.org/docs/en-us/basic/whitepaper.html)
2. [Neo N3 Documentation](https://docs.neo.org/docs/en-us/index.html)
3. [Neo GitHub Repository](https://github.com/neo-project/neo)
4. [Neo DevPack .NET](https://github.com/neo-project/neo-devpack-dotnet)
5. [C# Language Specification](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/introduction)
6. [NEP Standards](https://github.com/neo-project/proposals)
