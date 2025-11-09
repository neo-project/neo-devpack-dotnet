# Deployment Project

This project wraps the `Neo.SmartContract.Deploy` toolkit so you can deploy and interact with compiled contracts alongside your solution.

## Getting Started

1. Build your contract project so the `.nef` and `.manifest.json` artifacts are generated (typically under `bin/sc`).
2. Update `deploysettings.json` to point to the artifact paths. The defaults assume your contract project lives next to this deployment project.
3. Export the private key that should sign deployments and set it through the `NEO_WIF` environment variable, `deploysettings.json`, or the `--wif` option.

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
- `--network devnet` (or any custom name) resolves to the `customNetworks` entry inside `deploysettings.json`.
- `--rpc http://host:port` overrides the RPC URL directly.

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
