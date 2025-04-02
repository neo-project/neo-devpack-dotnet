# Neo N3 NEP-17 Token Smart Contract Example

## Overview
This example demonstrates how to implement a compliant NEP-17 token on the Neo N3 blockchain. NEP-17 is the fungible token standard for Neo N3, similar to ERC-20 on Ethereum. This sample provides a complete implementation with essential token functionality and administrative features.

## Key Features
- Complete NEP-17 standard implementation
- Owner and minter role management
- Token minting and burning capabilities
- Contract update functionality
- Event notifications for state changes

## Technical Implementation
The `SampleNep17Token` contract demonstrates several key aspects of NEP-17 token design:

### NEP-17 Standard Compliance
- Implements all required NEP-17 methods and properties
- Inherits from the base `Nep17Token` class
- Includes proper event notifications
- Declares standard compliance with `[SupportedStandards]` attribute

### Role-Based Access Control
The contract implements two distinct administrative roles:
1. **Owner Role**:
   - Controls contract ownership
   - Can update the contract
   - Can transfer ownership

2. **Minter Role**:
   - Controls token creation rights
   - Can mint new tokens
   - Can be assigned by the owner

### Token Properties
- **Symbol**: "SampleNep17Token"
- **Decimals**: 8 (allowing for fractional tokens)
- Storage-based tracking of token balances and supply

### Administrative Functions
- `SetOwner`: Transfers contract ownership to a new address
- `SetMinter`: Assigns minting rights to a new address
- `Mint`: Creates new tokens and assigns them to a specified address
- `Burn`: Destroys tokens from a specified address
- `Update`: Upgrades the contract to a new implementation

## Events
The contract emits the following events:
- `Transfer`: When tokens are transferred between addresses
- `SetOwner`: When contract ownership changes
- `SetMinter`: When minting rights are reassigned

## Security Considerations
- Permission checks using `Runtime.CheckWitness`
- Role-based authorization for sensitive operations
- Protected mint and burn operations
- Secured contract update mechanism

## Usage Scenarios
This NEP-17 token implementation can be customized for:
- Fungible tokens and cryptocurrencies
- Governance tokens with voting rights
- Staking and reward systems
- Tokenized assets and securities
- In-application currencies

## Customization Guide
To adapt this example for your own token:
1. Modify the token properties (Symbol, Decimals)
2. Update the initial owner and minter addresses
3. Adjust minting and burning logic as needed
4. Add any additional token-specific functionality
5. Configure permissions and access controls

## Deployment and Interaction
Once deployed, the token can be:
- Minted by the assigned minter
- Transferred between users
- Burned when no longer needed
- Managed by the contract owner
- Updated to new implementations

This example serves as a solid foundation for creating compliant, secure, and functional fungible tokens on the Neo N3 blockchain.