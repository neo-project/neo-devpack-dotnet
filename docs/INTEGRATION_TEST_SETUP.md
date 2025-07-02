# Integration Test Setup Guide

This guide provides detailed instructions for setting up and running integration tests for the Neo Smart Contract Deploy toolkit.

## Overview

Integration tests verify the complete deployment workflow against a real Neo blockchain instance. These tests ensure that:

- Smart contracts compile correctly
- Deployment transactions are properly formed and signed
- Contracts are successfully deployed to the blockchain
- Deployed contracts can be invoked and return expected results

## Prerequisites

### Required Software

1. **.NET 9.0 SDK**
   ```bash
   # Verify installation
   dotnet --version
   ```

2. **Neo Express** (blockchain emulator)
   ```bash
   # Install globally
   dotnet tool install -g Neo.Express
   
   # Verify installation
   neo-express --version
   ```

### Project Structure

The integration tests expect the following structure:
```
contract_plugin/
├── default.neo-express     # Neo Express configuration
├── start-neo-express.sh    # Script to start Neo Express
├── run-tests.sh           # Script to run tests
└── tests/
    └── Neo.SmartContract.Deploy.UnitTests/
        ├── appsettings.json    # Test configuration
        └── Integration/        # Integration test files
```

## Step-by-Step Setup

### 1. Configure Neo Express

The project includes a pre-configured `default.neo-express` file with:
- One consensus node on port 50012 (RPC)
- Three test wallets (consensus node, alice, bob)
- All wallets use password: `123456`

### 2. Start Neo Express

```bash
# From project root directory
./start-neo-express.sh
```

This script will:
- Check if Neo Express is installed
- Stop any existing instances
- Start a new instance on port 50012
- Verify the blockchain is running

Expected output:
```
Starting Neo Express for integration tests...
===========================================
Stopping any existing Neo Express instances...
Starting Neo Express on port 50012...

✅ Neo Express is running successfully!

RPC endpoint: http://localhost:50012

To stop Neo Express, run: neo-express stop
To run tests, use: ./run-tests.sh
```

### 3. Verify Neo Express is Running

```bash
# Check if port is open
nc -zv localhost 50012

# Test RPC endpoint
curl -X POST http://localhost:50012 \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"getblockcount","params":[],"id":1}'
```

Expected response:
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": <current_block_height>
}
```

### 4. Run Integration Tests

```bash
# Run only integration tests
dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
  --filter "FullyQualifiedName~Integration" \
  --verbosity normal

# Or use the test script to run all tests
./run-tests.sh
```

## Test Wallet Configuration

All integration tests use the following wallet configuration:

**Consensus Node Wallet**
- Address: `NVizn8DiExdmnpTQfjiVY3dox8uXg3Vrxv`
- Password: `123456`
- Has initial GAS for deployments

**Test User Wallets**
- Alice: `NNSyinBZAr8HMhjj95MfkKD1PY7YWoDweR`
- Bob: `NQUUbmTNfF5Eg3dqNbbPTnuVJGMPqfHGFA`
- Password: `123456`

## Integration Test Categories

### 1. DeploymentIntegrationTests
Tests the complete deployment workflow:
- `FullDeploymentWorkflow_ShouldDeployAndInvokeContract`
- `ArtifactBasedDeployment_ShouldWork`
- `ContractUpdate_ShouldWork`

### 2. MultiContractDeploymentTests
Tests deploying multiple contracts:
- `DeployMultipleIndependentContracts_ShouldSucceed`
- `DeployContractsWithDependencies_ShouldDeployInOrder`
- `BatchDeployWithRollback_ShouldHandleFailures`

## Troubleshooting Integration Tests

### Common Issues

1. **Connection Refused Errors**
   ```
   System.Net.Http.HttpRequestException: Connection refused (localhost:50012)
   ```
   **Solution**: Start Neo Express using `./start-neo-express.sh`

2. **Wrong Password Errors**
   ```
   System.InvalidOperationException: Wrong password
   ```
   **Solution**: Ensure all test wallets use password `123456`

3. **Port Already in Use**
   ```
   Error: An attempt was made to access a socket in a way forbidden by its access permissions
   ```
   **Solution**:
   ```bash
   # Find process using port
   lsof -i :50012
   
   # Kill the process
   kill -9 <PID>
   ```

4. **Neo Express Crashes**
   **Solution**:
   ```bash
   # Clean up Neo Express data
   rm -rf ~/.neo-express/
   
   # Restart Neo Express
   ./start-neo-express.sh
   ```

### Debug Mode

To run tests with detailed logging:

```bash
# Set logging to debug level
export Logging__LogLevel__Default=Debug

# Run tests with verbose output
dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
  --filter "FullyQualifiedName~Integration" \
  --logger "console;verbosity=detailed"
```

## Continuous Integration Setup

For CI/CD pipelines:

```yaml
# Example GitHub Actions
- name: Install Neo Express
  run: dotnet tool install -g Neo.Express

- name: Start Neo Express
  run: |
    neo-express create -f
    neo-express run -s 1 &
    sleep 10  # Wait for blockchain to start

- name: Run Integration Tests
  run: |
    dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
      --filter "FullyQualifiedName~Integration" \
      --logger "trx" \
      --results-directory ./test-results

- name: Stop Neo Express
  if: always()
  run: neo-express stop
```

## Advanced Configuration

### Custom RPC Port

To use a different port, update:

1. `default.neo-express` - Change `rpc-port`
2. `appsettings.json` - Update `RpcUrl`
3. `start-neo-express.sh` - Update port checks

### Multiple Node Setup

For testing consensus scenarios:

```bash
# Start multi-node network
neo-express run -i default.neo-express

# This starts all configured consensus nodes
```

### Performance Testing

For load testing deployments:

```bash
# Increase block time for stress testing
neo-express set -i default.neo-express \
  --seconds-per-block 1

# Run performance tests
dotnet test --filter "Performance" \
  --configuration Release
```

## Best Practices

1. **Always Clean Up**
   - Stop Neo Express after testing
   - Clean temporary files and directories

2. **Use Consistent Wallets**
   - All test wallets use password `123456`
   - Don't modify wallet files during tests

3. **Handle Async Operations**
   - Use proper async/await patterns
   - Set appropriate timeouts for blockchain operations

4. **Mock When Possible**
   - Use mocks for unit tests
   - Only use real blockchain for integration tests

5. **Log Appropriately**
   - Use ILogger for debugging
   - Don't log sensitive information (private keys)