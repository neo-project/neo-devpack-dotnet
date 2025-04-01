# Native Contract: GasToken (GAS)

Namespace: `Neo.SmartContract.Framework.Native`

Represents the GAS token contract, which follows the NEP-17 fungible token standard. GAS is used to pay for network fees and computation.

## NEP-17 Standard Methods

Provides static methods corresponding to the NEP-17 standard:

*   **`Symbol` (`string`)**: Returns the token symbol ("GAS").
*   **`Decimals` (`byte`)**: Returns the number of decimals (8).
*   **`TotalSupply()` (`System.Numerics.BigInteger`)**: Returns the total supply of GAS.
*   **`BalanceOf(UInt160 account)` (`System.Numerics.BigInteger`)**: Gets the GAS balance of the specified account.
*   **`Transfer(UInt160 from, UInt160 to, System.Numerics.BigInteger amount, object data)` (`bool`)**: Transfers GAS from the `from` account to the `to` account.
    *   Requires `Runtime.CheckWitness(from)` to be true, or the calling contract must have been approved by `from`.
    *   Returns `true` on success, `false` otherwise.
    *   The `data` parameter is passed to the `onNEP17Payment` method if `to` is a deployed contract.

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

// Need permission to call GasToken methods
[ContractPermission(nameof(GasToken), "symbol", "decimals", "balanceOf", "transfer")]
public class GasDemo : SmartContract
{
    public static string GetGasSymbol()
    {
        return GasToken.Symbol;
    }

    public static byte GetGasDecimals()
    {
        return GasToken.Decimals;
    }

    public static BigInteger GetAccountGasBalance(UInt160 account)
    {
        return GasToken.BalanceOf(account);
    }

    // Example: A contract method that requires a GAS payment
    public static bool PayFeeWithGas(UInt160 payer, BigInteger requiredFee)
    {
        // Check if the payer signed the transaction
        if (!Runtime.CheckWitness(payer)) return false;

        // Transfer the required fee from the payer to this contract
        // Note: The contract itself needs GAS to execute this call
        bool success = GasToken.Transfer(payer, Runtime.ExecutingScriptHash, requiredFee, null);

        if (success) 
        { 
            Runtime.Log("Fee paid successfully.");
            // ... perform action that required the fee ...
        } 
        else 
        { 
            Runtime.Log("Fee payment transfer failed.");
        }
        return success;
    }

    // Method called when this contract receives GAS
    public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
    {
        // Check if the payment was GAS
        if (Runtime.CallingScriptHash == GasToken.Hash)
        {
            Runtime.Log($"Received {amount * BigInteger.Pow(10, -GasToken.Decimals)} GAS from {from}");
            // Process the received GAS payment
        }
    }
}
```

**Important Considerations:**

*   **Fees:** Remember that calling any contract method, including `GasToken.Transfer`, requires GAS fees for the transaction itself.
*   **Authorization:** `GasToken.Transfer` strictly enforces authorization. The `from` address must either sign the transaction (`Runtime.CheckWitness`) or have previously approved the calling contract via the standard NEP-17 `approve` mechanism (though `approve` is not directly exposed in the `GasToken` C# wrapper, it can be called via `Contract.Call`).

[Previous: CryptoLib](./CryptoLib.md) | [Next: LedgerContract](./Ledger.md)