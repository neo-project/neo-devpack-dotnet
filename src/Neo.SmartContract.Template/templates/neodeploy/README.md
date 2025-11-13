# Deployment Project

This project wraps the `Neo.SmartContract.Deploy` toolkit so you can deploy and interact with compiled contracts alongside your solution.

## Getting Started

1. Build your contract project so the `.nef` and `.manifest.json` artifacts are generated (typically under `bin/sc`).
2. Update `deploysettings.json` to point to the artifact paths. The defaults assume your contract project lives next to this deployment project.
3. Declare the networks you plan to target under `customNetworks` inside `deploysettings.json` (mainnet/testnet/devnet are scaffolded for you). Each network can specify its RPC URL, network magic, and optional `wif`. If a network does not have a `wif`, you can still pass one via `--wif` or the `NEO_WIF` environment variable and the CLI will prompt as a fallback.

## Common Commands

```bash
# Deploy artifacts to a Neo-Express instance (default RPC http://localhost:50012)
dotnet run -- deploy --network express --wif <your-wif>

# Deploy to mainnet
dotnet run -- deploy --network mainnet --wif <your-wif>

# Call a read-only method
dotnet run -- call --contract 0xabc... --method symbol

# Invoke a state-changing method
dotnet run -- invoke --contract 0xabc... --method transfer --args "[\"sender\",\"receiver\",100]"
```

### Selecting a Network

- `--network express` (default) targets a local Neo-Express RPC node running on `http://localhost:50012`.
- `--network mainnet` / `testnet` use the public RPC endpoints defined by the toolkit.
- `--network devnet` (or any custom name) resolves to the matching `customNetworks` entry inside `deploysettings.json`. When a `wif` is present in that entry it is loaded automatically so you don't have to pass `--wif` for that network.
- `--rpc http://host:port` overrides the RPC URL directly.

### Network Configuration

`deploysettings.json` keeps network metadata together so you can switch environments with `--network`:

```json
{
  "customNetworks": {
    "mainnet": {
      "rpcUrl": "https://mainnet1.neo.coz.io:443",
      "networkMagic": 860833102,
      "wif": "<mainnet-deployment-key>"
    },
    "testnet": {
      "rpcUrl": "http://seed2t5.neo.org:20332",
      "networkMagic": 894710606,
      "wif": "<testnet-deployment-key>"
    },
    "devnet": {
      "rpcUrl": "http://localhost:50012",
      "networkMagic": 123456789,
      "wif": ""
    }
  }
}
```

Leave `wif` blank to be prompted at runtime or provide the key directly for automated scenarios (CI, scripts, etc.).

### Customising Artifacts

`deploysettings.json` stores the default NEF/manifest paths as relative paths. You can override them per run:

```bash
dotnet run -- deploy --nef ../MyContract/bin/sc/MyContract.nef --manifest ../MyContract/bin/sc/MyContract.manifest.json
```

Provide initialisation parameters with `--init-params`, for example:

```bash
dotnet run -- deploy --init-params "[\"owner\", 1, true]"
```

### Invoking / Calling Contracts

- `call` executes read-only methods. Use `--return` to hint the expected return type (`string`, `bool`, `int`, `long`, `bigint`, `bytes`).
- `invoke` sends a state-changing transaction and prints the transaction hash. Arguments are supplied as JSON via `--args`.

Both commands support the same network overrides as the `deploy` command.
