# Interacting with Deployed Contracts

Once your smart contract is deployed on a Neo network (Neo Express, TestNet, or MainNet), users and applications need ways to interact with it by calling its public methods.

Interaction typically involves constructing and sending a transaction that invokes a specific method on your contract.

## Required Information

To interact with a deployed contract, you need:

1.  **Contract Script Hash:** The unique address (`UInt160`) of the deployed contract.
2.  **Method Name:** The name of the public static method you want to call (as defined in the manifest, respecting `[DisplayName]`).
3.  **Arguments:** Any arguments required by the method, in the correct order and type.
4.  **Signer Account:** A wallet/account to sign the transaction and potentially pay GAS fees.
5.  **Network Connection:** Access to a Neo node (RPC endpoint) for the target network.

## Interaction Methods

*   **Wallets (Neon, NeoLine, O3, etc.):**
    *   Many wallets provide a user interface for invoking contract methods.
    *   You typically enter the script hash, select the method from a list (often populated by fetching the manifest), input arguments, and choose the signing account.
    *   The wallet handles constructing the script (`Contract.Call`), building the transaction, calculating fees, signing, and broadcasting.
    *   This is the most common way for end-users to interact.

*   **Block Explorers (Dora, NeoTube, etc.):**
    *   Some explorers offer interfaces to read data from contracts (calling `[Safe]` methods) and sometimes even invoke methods (which would usually redirect to a connected wallet for signing).
    *   Primarily used for viewing contract state, transactions, and events.

*   **Neo Express (Local):**
    *   Provides the `neox contract invoke` command for command-line interaction with contracts deployed on a local Neo Express instance.
    *   You provide the manifest file (or contract name if known), method name, arguments, and the signing wallet name.
    *   ```bash
      neox contract invoke ./path/to/MyContract.manifest.json myMethod arg1 "arg 2" MyWalletName -- optional-args
      ```
    *   Useful for testing and scripting interactions locally.

*   **SDKs (Neon.js, neow3j, NGD SDK - C#, Python, Go):**
    *   Software Development Kits provide libraries to interact with Neo nodes and contracts programmatically.
    *   This is how backend services, dApps, and automated scripts typically interact with contracts.
    *   **Steps (Conceptual using an SDK):**
        1.  Connect to a Neo RPC node for the target network.
        2.  Define contract script hash, method name, and arguments.
        3.  Use the SDK's "contract invocation" functions to build the NeoVM script (often abstracted away).
        4.  Create a transaction including the script, signer(s), and calculated fees.
        5.  Sign the transaction using the private key of the signer account (managed securely).
        6.  Send the transaction to the node.
        7.  Optionally, wait for confirmation and retrieve the execution result (return value, events, logs).

## Reading vs. Writing State

*   **Reading (`[Safe]` Methods):** Calling methods marked as `[Safe]` in the manifest only reads data. This can often be done via RPC calls (`invokefunction` or `invokescript`) without sending an actual transaction, meaning no GAS fee is required (though the RPC node might have its own limits).
*   **Writing (Non-`[Safe]` Methods):** Calling methods that modify state (write to storage, transfer tokens, emit events) **always** requires sending a transaction, which must be signed and include GAS fees.

## Monitoring Events

Off-chain applications often need to react to events emitted by smart contracts (`Runtime.Notify`).

*   **SDKs:** Provide mechanisms to subscribe to event notifications from the blockchain or query past transaction logs for specific events emitted by your contract.
*   **Explorers:** Display events associated with transactions involving your contract.

Effective interaction involves choosing the right tool for the job – wallets for users, Neo Express for local testing, SDKs for dApps and automation, and explorers for monitoring.

[Previous: Deployment Process](./03-deployment.md) | [Next Section: Reference](../09-reference/README.md)