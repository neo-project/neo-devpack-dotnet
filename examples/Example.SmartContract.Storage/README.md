# Neo N3 Storage Smart Contract Example

## Overview
This example demonstrates how to use the storage capabilities of Neo N3 smart contracts. It provides comprehensive examples of various storage operations, data types, and patterns for persistent data management on the Neo blockchain.

## Key Features
- Storage operations (Put, Get, Delete)
- Multiple data type handling (bytes, strings, integers, booleans)
- Storage map implementations
- Serialization and deserialization of complex objects
- Storage iteration and search functionality

## Technical Implementation
The `SampleStorage` contract demonstrates several Neo N3 storage features:

### Storage Organization
- `StorageMap` usage for organized data storage
- Prefix-based storage segregation using byte, string, and byte array prefixes
- Index-based property access for storage

### Data Type Support
The example includes methods for storing and retrieving various data types:
- Byte arrays (basic and arrays larger than 16 bytes)
- Strings
- Integers
- Booleans
- UInt160 values
- UInt256 values
- ECPoint values
- Custom serialized objects

### Advanced Storage Features
- `Storage.Find` for prefix-based searching
- Iterator patterns for processing multiple storage entries
- Storage context management
- Read-only operations

## Storage Patterns
The example demonstrates several important storage patterns:
1. **Direct Storage**: Using the Storage class directly
2. **StorageMap**: Using maps for organized, prefix-based storage
3. **Index Properties**: Accessing storage through property-like syntax
4. **Serialization**: Storing and retrieving complex objects

## Usage
This contract serves as a reference implementation for:
- Building data-intensive Neo smart contracts
- Implementing persistent state in decentralized applications
- Optimizing storage operations for gas efficiency
- Handling complex data structures on-chain

## Performance Considerations
The example illustrates several key performance aspects:
- Efficient storage key design
- Proper handling of large byte arrays
- Serialization patterns for complex data
- Prefix-based data organization

## Educational Value
This example teaches:
- Fundamental Neo N3 storage concepts
- Best practices for on-chain data management
- Various techniques for working with different data types
- Iteration and search patterns for blockchain storage

## Use Cases
The storage patterns demonstrated in this example are applicable to:
- Token contracts requiring balance tracking
- Voting and governance systems
- Game state management
- Record-keeping applications
- Multi-signature wallets
- Any application requiring persistent state