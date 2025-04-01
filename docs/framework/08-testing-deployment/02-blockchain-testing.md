# Testing on Neo Networks (Neo Express & TestNet)

Since pure unit testing has limitations, testing your smart contract's behavior on a running Neo blockchain environment is crucial. Neo provides two primary environments for this before deploying to MainNet: Neo Express (local) and the public TestNet.

## 1. Neo Express (Local Private Network)

Neo Express is part of the [Neo Blockchain Toolkit (NBT)](https://github.com/neo-project/neo-blockchain-toolkit) and is the recommended tool for local development and testing.

*   **Private Instance:** Runs a private, single-node or multi-node Neo N3 blockchain entirely on your local machine.
*   **Fast & Free:** Transactions are processed instantly, and you can mint yourself unlimited NEO and GAS for testing.
*   **Debugging Support:** Integrates with the VS Code NBT extension for step-through debugging of your C# contract code (requires compiling with debug info).
*   **State Control:** Easily reset the chain, create checkpoints, and manage accounts.

**Typical Workflow:**

1.  **Install:** `dotnet tool install Neo.Express -g`
2.  **Create Instance:** `neox create mychain.neo-express` (Creates a configuration file)
3.  **Run Instance:** `neox run mychain.neo-express` (Starts the local node)
4.  **Create Wallet:** `neox wallet create mywallet`
5.  **Mint Assets:** `neox transfer 10000 NEO genesis mywallet` (Transfer initial assets from the instance's genesis wallet)
6.  **Compile Contract:** `dotnet build -c Debug` (Ensure debug info is generated)
7.  **Deploy Contract:** `neox contract deploy ./bin/Debug/netX.Y/MyContract.nef mywallet --force` (`--force` allows overwriting during development)
8.  **Invoke Methods:** `neox contract invoke MyContract.manifest.json someMethod arg1 arg2 mywallet`
9.  **Inspect Storage/Events:** Use `neox contract storage ...`, `neox contract get ...`, or check Neo Express logs/output.
10. **Debug (VS Code):** Configure `launch.json` to attach to Neo Express, set breakpoints in C#, and invoke methods to trigger debugging sessions.

Neo Express is ideal for rapid iteration, functional testing, GAS usage estimation, and debugging during the development cycle.

## 2. Neo TestNet

The Neo TestNet (currently N3 TestNet T5) is a public blockchain that mirrors the MainNet's protocol and features but uses valueless test tokens (NEO/GAS).

*   **Realistic Environment:** Operates with multiple consensus nodes run by the community, providing a more realistic test of consensus behavior, network latency, and interaction with other deployed contracts.
*   **Public Access:** Anyone can connect and deploy contracts.
*   **Free Test Assets:** Obtain free TestNet NEO/GAS from a faucet (search for "Neo N3 TestNet Faucet").
*   **Final Pre-MainNet Check:** It's the last stage of testing before deploying to MainNet.

**Workflow:**

1.  **Get TestNet Assets:** Use a faucet to fund a TestNet wallet.
2.  **Configure Wallet/Tool:** Point your wallet (e.g., Neon Wallet, NeoLine) or SDK to the N3 TestNet RPC endpoints.
3.  **Compile for Release:** `dotnet build -c Release` (Do not include debug info for TestNet/MainNet).
4.  **Deploy Contract:** Use a wallet or SDK connected to TestNet to perform the deployment transaction, paying fees with TestNet GAS.
5.  **Interact:** Use the wallet/SDK to invoke methods, check balances, and monitor events on a TestNet explorer (like Dora, NeoTube).

Testing on TestNet helps ensure your contract behaves correctly in a multi-node environment and interacts properly with the live network conditions and potentially other contracts before risking real assets on MainNet.

## Testing Strategy

1.  **Unit Tests:** Verify core logic and algorithms in isolation.
2.  **Neo Express:** Perform functional testing, integration testing (contract-to-contract calls), debugging, and initial GAS analysis.
3.  **TestNet:** Conduct final testing in a realistic public environment, focusing on deployment, interaction flows, and behavior under real network conditions.

This multi-layered approach provides the highest confidence in your smart contract's correctness and security.

[Previous: Unit Testing](./01-unit-testing.md) | [Next: Deployment Process](./03-deployment.md)