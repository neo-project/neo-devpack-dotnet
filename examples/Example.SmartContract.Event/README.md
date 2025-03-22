# Neo N3 Event Smart Contract Example

## Overview
This example demonstrates how to define and emit events in Neo N3 smart contracts. Events are an essential mechanism for communicating state changes and important occurrences from smart contracts to external applications and services, enabling efficient monitoring and reactive behavior.

## Key Features
- Event declaration syntax
- Event emission with parameters
- Custom event naming
- Multiple event types
- Various parameter data types

## Technical Implementation
The `SampleEvent` contract demonstrates several key aspects of Neo N3 events:

### Event Declaration
The example showcases two different event declarations:
1. An event with custom display name and multiple parameters of different types
2. A standard event with byte array and numeric parameters

### Event Types and Parameters
The example demonstrates events with various parameter types:
- Byte arrays (binary data)
- String values
- BigInteger numbers

### Event Emission
The contract shows proper event emission syntax, where:
- Events are triggered with appropriate parameters
- Parameters are passed in the correct order
- Different event types can be emitted in the same transaction

## How Neo N3 Events Work
Events in Neo N3 are implemented as notifications through the Runtime.Notify mechanism:
1. **Declaration**: Events are declared using the `event` keyword with an Action delegate
2. **Display Name**: Optional custom names can be assigned using the `[DisplayName]` attribute
3. **Emission**: Events are triggered by invoking them like methods with appropriate parameters
4. **Subscription**: External applications can subscribe to contract events using the Neo RPC API

## Applications
Events are used for:
- Notifying external systems about state changes
- Creating audit trails and transaction history
- Enabling event-driven architectures
- Supporting off-chain indexing and analytics
- Building reactive user interfaces

## Event Best Practices
The example demonstrates recommended practices:
- Meaningful event names
- Properly typed parameters
- Efficient parameter ordering (smaller data first)
- Clear separation of different event types

## Client-Side Handling
Events emitted by this contract can be captured by:
- RPC clients subscribing to notifications
- Block explorers monitoring contract activity
- Application backends implementing event listeners
- Frontend applications using websocket connections

## Educational Value
This example teaches:
- How to properly declare events in Neo N3 contracts
- The syntax for emitting events with multiple parameters
- How to customize event names for better readability
- Best practices for organizing different event types
- Parameter type handling for events

Events provide a critical communication channel between on-chain smart contracts and off-chain applications, enabling developers to build responsive and data-rich blockchain applications.