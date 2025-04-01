# Native Contracts

Neo N3 includes several built-in "native" contracts that provide core blockchain functionalities. These contracts exist from the genesis block and have fixed script hashes. The `Neo.SmartContract.Framework` provides dedicated static classes within the `Neo.SmartContract.Framework.Native` namespace to interact with these contracts easily, acting as wrappers around `Contract.Call`.

Using these native contract wrappers is generally preferred over calling them directly via `Contract.Call` as they provide type safety and convenience.

## Available Native Contract Wrappers

*   **[`ContractManagement`](./ContractManagement.md):** Deploying, updating, and destroying contracts.
*   **[`CryptoLib`](./CryptoLib.md):** Cryptographic functions (hashing, signature verification).
*   **[`GasToken` (GAS)](./GasToken.md): NEP-17 methods for the GAS token.
*   **[`LedgerContract`](./Ledger.md):** Accessing block and transaction data.
*   **[`NameService` (NNS)](./NameService.md): Neo Name Service interactions (domain registration/resolution).
*   **[`NeoToken` (NEO)](./NeoToken.md): NEP-17 methods and governance functions for the NEO token.
*   **[`OracleContract`](./Oracle.md):** Requesting data from off-chain URLs.
*   **[`PolicyContract`](./Policy.md):** Querying and (if authorized) setting network policies (fees, blocked accounts).
*   **[`RoleManagement`](./RoleManagement.md):** Assigning and querying node roles.
*   **[`StdLib`](./StdLib.md):** Standard library functions (serialization, data conversion).

## Permissions

Just like calling any other contract, your contract needs permission declared in its manifest (`[ContractPermission]`) to call methods on native contracts. You can reference native contracts by their well-known names:

```csharp
using Neo.SmartContract.Framework.Attributes;

// Example: Allow calling GasToken's balanceOf and Ledger's currentIndex
[ContractPermission("GasToken", "balanceOf")]
[ContractPermission("LedgerContract", "currentIndex")]

// Allow calling any method on OracleContract
[ContractPermission("OracleContract", "*")] 

public class NativeCallDemo : SmartContract 
{ 
    // ... 
}
```

Refer to the specific pages for details on each native contract wrapper.

[Next: ContractManagement](./ContractManagement.md)