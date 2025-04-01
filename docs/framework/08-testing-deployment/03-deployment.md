# Deployment Process

Deploying a smart contract makes its code and manifest available on the Neo blockchain, allowing users and other contracts to interact with it. Deployment is achieved by sending a specific transaction that invokes the `deploy` method of the native `ContractManagement` contract.

## Prerequisites

1.  **Compiled Artifacts:** You need the compiled `.nef` file (bytecode) and `.manifest.json` file (metadata) for your contract. Compile using `dotnet build` (use `-c Release` for TestNet/MainNet).
2.  **Neo Wallet:** A Neo N3 compatible wallet (e.g., Neon Wallet, NeoLine, O3 Wallet) holding the address you want to use for deployment.
3.  **GAS:** Sufficient GAS in the deploying wallet to cover:
    *   The base deployment fee (`ContractManagement.GetMinimumDeploymentFee()`).
    *   Transaction network fees (based on transaction size, `PolicyContract.GetFeePerByte()`).
    *   GAS required by your contract's `_deploy` method, if any.

## Deployment Steps (Conceptual)

The exact steps vary depending on the tool (Wallet UI, SDK, Neo Express), but the underlying process involves constructing and sending a deployment transaction:

1.  **Read Files:** Read the content of the `.nef` file (as raw bytes) and the `.manifest.json` file (as a UTF8 string).
2.  **Construct Script:** Create a NeoVM script that calls the `ContractManagement.deploy` method, passing the NEF bytes and manifest string as arguments.
    ```csharp
    // Pseudo-code for script generation
    scriptBuilder.EmitDynamicCall(ContractManagement.Hash, "deploy", nefFileBytes, manifestJsonString);
    script = scriptBuilder.ToArray();
    ```
3.  **Create Transaction:** Build a transaction that includes:
    *   The generated script.
    *   **Signer:** The deploying account (your wallet address) with appropriate scopes (usually `CalledByEntry`).
    *   **Fees:** Sufficient System Fee and Network Fee (calculated based on script size, execution cost, and network policy).
    *   Nonce, ValidUntilBlock, etc.
4.  **Sign Transaction:** Sign the transaction using the private key corresponding to the deploying account.
5.  **Broadcast Transaction:** Send the signed transaction to a Neo node (connected to the target network - Neo Express, TestNet, or MainNet).
6.  **Confirmation:** Wait for the transaction to be included in a block and confirmed by the network.
7.  **`_deploy` Execution:** Upon successful deployment confirmation, the `_deploy(object data, bool update)` method within your newly deployed contract code is automatically executed by the NeoVM with `update` set to `false`.

## Deployment Using Tools

*   **Neo Express (Local):**
    ```bash
    # Ensure Neo Express instance is running
    # Deploy using your neox wallet
    neox contract deploy ./path/to/MyContract.nef YourWalletName --force 
    # --force allows overwriting if already deployed locally
    ```
    Neo Express handles reading the manifest (assumes it's alongside the NEF), calculating fees (using local settings), creating, signing, and broadcasting the transaction to the local instance.

*   **Wallets (Neon, NeoLine, etc.):**
    *   Most graphical wallets provide a UI for contract deployment.
    *   You typically need to select/upload the `.nef` and `.manifest.json` files.
    *   The wallet calculates estimated fees.
    *   You approve the transaction, and the wallet signs and broadcasts it to the selected network (TestNet/MainNet).

*   **SDKs (Neon.js, neow3j, NGD SDK, etc.):**
    *   SDKs provide APIs to programmatically perform the deployment steps: reading files, building the script and transaction, calculating fees, signing, and sending.
    *   This offers more flexibility for automated deployment pipelines.

## Post-Deployment

*   **Record Script Hash:** Once deployed, your contract is identified by its unique **Script Hash**. Note this address down – it's needed to interact with the contract.
*   **Verification (Optional):** Some block explorers allow developers to verify their contract source code, linking the deployed script hash to the original C# code for transparency.
*   **Interaction:** Use wallets, explorers, or SDKs to call the methods defined in your contract's manifest, using its script hash.

Deploying to MainNet is irreversible and involves real costs (GAS). **Always test extensively on Neo Express and TestNet before deploying to MainNet.**

[Previous: Testing on Neo Networks](./02-blockchain-testing.md) | [Next: Interacting with Deployed Contracts](./04-interaction.md)