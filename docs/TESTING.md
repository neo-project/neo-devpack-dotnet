# Testing Guide for Neo Smart Contract Deploy

This guide explains how to run tests for the Neo Smart Contract Deploy toolkit.

## Prerequisites

1. **.NET 9.0 SDK** - Required for building and running tests
2. **Neo Express** (optional) - Required only for integration tests

## Test Categories

### Unit Tests
Unit tests verify individual components and don't require any external dependencies.

- **Pass Rate**: 96% (48/50 tests passing)
- **Runtime**: ~7 seconds
- **Requirements**: None

### Integration Tests
Integration tests verify the full deployment workflow against a local blockchain.

- **Pass Rate**: 100% when Neo Express is running
- **Runtime**: ~3-5 seconds per test
- **Requirements**: Neo Express running on port 50012

## Running Tests

### Quick Start

```bash
# Run all unit tests (no setup required)
dotnet test --filter "FullyQualifiedName!~Integration"

# Run all tests including integration tests
./start-neo-express.sh  # Start Neo Express first
./run-tests.sh          # Run all tests
```

### Detailed Commands

#### Run Only Unit Tests
```bash
dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
  --filter "FullyQualifiedName!~Integration" \
  --verbosity normal
```

#### Run Only Integration Tests
```bash
# First, start Neo Express
./start-neo-express.sh

# Then run integration tests
dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
  --filter "FullyQualifiedName~Integration" \
  --verbosity normal
```

#### Run Specific Test Suite
```bash
# Examples:
dotnet test --filter "FullyQualifiedName~ContractCompilerServiceTests"
dotnet test --filter "FullyQualifiedName~WalletManagerServiceTests"
dotnet test --filter "FullyQualifiedName~DeploymentIntegrationTests"
```

## Setting Up Neo Express

### Install Neo Express
```bash
# Global installation (recommended)
dotnet tool install -g Neo.Express

# Local installation
dotnet tool install Neo.Express
```

### Start Neo Express
```bash
# Using the provided script (recommended)
./start-neo-express.sh

# Or manually
neo-express run -i default.neo-express -s 1
```

### Stop Neo Express
```bash
neo-express stop
```

### Verify Neo Express is Running
```bash
# Check if port 50012 is listening
nc -zv localhost 50012

# Or use curl to test the RPC endpoint
curl -X POST http://localhost:50012 \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","method":"getblockcount","params":[],"id":1}'
```

## Test Configuration

### appsettings.json
The test configuration is located in `tests/Neo.SmartContract.Deploy.UnitTests/appsettings.json`:

```json
{
  "Network": {
    "RpcUrl": "http://localhost:50012",
    "Network": "private",
    "Wallet": {
      "WalletPath": "test.wallet.json",
      "Password": "123456"
    }
  }
}
```

### Test Wallets
All test wallets use the password `123456` and are compatible with the Neo N3 protocol.

- **Default Test Wallet**: `NVizn8DiExdmnpTQfjiVY3dox8uXg3Vrxv`
- **Alice's Wallet**: `NNSyinBZAr8HMhjj95MfkKD1PY7YWoDweR`
- **Bob's Wallet**: `NQUUbmTNfF5Eg3dqNbbPTnuVJGMPqfHGFA`

## Troubleshooting

### Test Failures

1. **"Wrong password" errors**
   - Ensure all test wallets use password `123456`
   - Check that wallet JSON is properly formatted

2. **"Connection refused" errors**
   - Start Neo Express using `./start-neo-express.sh`
   - Verify Neo Express is running on port 50012

3. **Compilation errors**
   - Ensure .NET 9.0 SDK is installed
   - Run `dotnet restore` to restore packages

4. **Test interference issues**
   - Some tests may fail when run in parallel
   - Tests marked with `[Collection("Sequential")]` run sequentially
   - Run tests individually if needed

### Common Issues

#### Port Already in Use
```bash
# Find process using port 50012
lsof -i :50012

# Kill the process
kill -9 <PID>
```

#### Neo Express Won't Start
```bash
# Check for existing Neo Express processes
ps aux | grep neo-express

# Kill all Neo Express processes
pkill -f neo-express

# Try starting again
./start-neo-express.sh
```

## Test Results Summary

Current test status (as of last update):

- **Total Tests**: 57
- **Unit Tests**: 50 (48 passing, 2 with interference issues)
- **Integration Tests**: 7 (all pass with Neo Express running)
- **Overall Pass Rate**: 89% without Neo Express, 100% with Neo Express

## Continuous Integration

For CI/CD pipelines, use the following approach:

```yaml
# Example GitHub Actions workflow
- name: Run Unit Tests
  run: dotnet test --filter "FullyQualifiedName!~Integration"

- name: Start Neo Express
  run: |
    dotnet tool install -g Neo.Express
    neo-express run -i default.neo-express -s 1 &
    sleep 5

- name: Run Integration Tests
  run: dotnet test --filter "FullyQualifiedName~Integration"
```

## Contributing

When adding new tests:

1. Place unit tests in appropriate service test files
2. Place integration tests in the Integration folder
3. Use `TestBase` as the base class for proper setup/teardown
4. Mark file-intensive tests with `[Collection("Sequential")]`
5. Ensure all test contracts use `System.ComponentModel` for attributes
6. Use the standard test wallet with password `123456`