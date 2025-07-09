# Testing Deployed Contracts

This document describes how to test already deployed contracts on the Neo testnet using the `TestDeployedContracts.cs` file.

## Contract Addresses

The following contracts are deployed on testnet:

- **Token Contract**: `0xe5af8922400736cfac7337955dcd0e8d98f608cd`
- **NFT Contract**: `0xea414034cc25ddcc681ef6841a178ad4a0bc37e2`
- **Governance Contract**: `0x5d773713bcfb164d7f7f8e6877269341f9f6c2b1`

## Account Information

- **WIF Key**: `KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb`
- **Account Address**: `NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX`

## Running the Tests

There are two ways to run the tests:

### Method 1: Using the Shell Script

```bash
./run-deployed-tests.sh
```

### Method 2: Using dotnet run

```bash
dotnet run testdeployed
```

## What the Tests Do

The `TestDeployedContracts` class performs the following tests:

### 1. Token Contract Tests
- Verifies the contract is deployed
- Retrieves token information (symbol, decimals, total supply)
- Checks the contract owner
- Gets the account balance
- Attempts to initialize the contract if not already initialized

### 2. NFT Contract Tests
- Verifies the contract is deployed
- Retrieves NFT information (symbol, decimals, total supply)
- Checks the contract owner
- Gets the mint price
- Attempts to initialize the contract if not already initialized
- Checks owned tokens

### 3. Governance Contract Tests
- Verifies the contract is deployed
- Checks if the account is a council member
- Gets the proposal count
- Attempts to initialize the contract if not already initialized
- Creates a test proposal (if the account is a council member)

### 4. Contract Interaction Tests
- Tests token approval for NFT minting
- Attempts to mint an NFT
- Tests governance voting on proposals

## Expected Output

When running the tests, you should see output similar to:

```
=== Testing Deployed Contracts ===
Token Contract: 0xe5af8922400736cfac7337955dcd0e8d98f608cd
NFT Contract: 0xea414034cc25ddcc681ef6841a178ad4a0bc37e2
Governance Contract: 0x5d773713bcfb164d7f7f8e6877269341f9f6c2b1
Account: NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX

Deployer Address: NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX
GAS Balance: [amount] GAS

=== Testing Token Contract ===
1. Checking if contract is deployed...
   ✓ Contract is deployed
...

✅ All tests completed!
```

## Troubleshooting

If tests fail, check:

1. **Network connectivity**: Ensure you can connect to the Neo testnet
2. **GAS balance**: The account needs GAS to perform transactions
3. **Contract state**: The contracts may need to be initialized first
4. **Permissions**: Some operations require specific permissions (e.g., being a council member)

## Notes

- Some operations may fail if the contracts have already been initialized
- Transaction operations require GAS in the account
- The tests are designed to be idempotent where possible