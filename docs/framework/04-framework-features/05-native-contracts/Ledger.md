# Native Contract: LedgerContract

Namespace: `Neo.SmartContract.Framework.Native`

Provides access to historical ledger information, such as blocks and transactions.

## Key Methods

*   **`CurrentHash` (`UInt256`)**: Gets the hash of the latest block.
*   **`CurrentIndex` (`uint`)**: Gets the height/index of the latest block.
*   **`GetBlock(uint index)` (`Block`)**: Retrieves block information by block height/index.
    *   Returns a `Block` object or `null` if the index is invalid.
*   **`GetBlock(UInt256 hash)` (`Block`)**: Retrieves block information by block hash.
    *   Returns a `Block` object or `null` if the hash doesn't correspond to a block.
*   **`GetTransaction(UInt256 hash)` (`Transaction`)**: Retrieves transaction information by transaction hash.
    *   Returns a `Transaction` object or `null` if the hash doesn't correspond to a transaction.
*   **`GetTransactionFromBlock(UInt256 blockHash, int txIndex)` (`Transaction`)**: Retrieves a transaction by its index within a specific block (identified by hash).
*   **`GetTransactionFromBlock(uint blockIndex, int txIndex)` (`Transaction`)**: Retrieves a transaction by its index within a specific block (identified by height).
*   **`GetTransactionHeight(UInt256 hash)` (`uint`)**: Gets the block height in which a specific transaction was included. Returns `-1` (as a `uint`, so max value) if not found.

## `Block` Object Properties

The `Block` object returned by `GetBlock` has properties like:

*   `Hash` (`UInt256`)
*   `Version` (`uint`)
*   `PrevHash` (`UInt256`)
*   `MerkleRoot` (`UInt256`)
*   `Timestamp` (`ulong`) (milliseconds since Unix epoch)
*   `Nonce` (`ulong`)
*   `Index` (`uint`) (Block height)
*   `NextConsensus` (`UInt160`)
*   `Witness` (`Witness`)
*   `TransactionsCount` (`int`)

## `Transaction` Object Properties

The `Transaction` object returned by `GetTransaction*` (and `Runtime.Transaction`) has properties like:

*   `Hash` (`UInt256`)
*   `Version` (`byte`)
*   `Nonce` (`uint`)
*   `Sender` (`UInt160`)
*   `SystemFee` (`long`)
*   `NetworkFee` (`long`)
*   `ValidUntilBlock` (`uint`)
*   `Signers` (`Signer[]`)
*   `Attributes` (`TransactionAttribute[]`)
*   `Script` (`ByteString`)
*   `Witnesses` (`Witness[]`)

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

// Permission to call LedgerContract methods
[ContractPermission(nameof(LedgerContract), "currentIndex", "getBlock", "getTransactionHeight")]
public class LedgerDemo : SmartContract
{
    public static uint GetCurrentBlockHeight()
    {
        return LedgerContract.CurrentIndex;
    }

    // Get timestamp of a specific block by height
    public static ulong GetBlockTimestamp(uint index)
    {
        Block block = LedgerContract.GetBlock(index);
        // Accessing block properties costs GAS
        return block?.Timestamp ?? 0; // Return 0 if block not found
    }

    // Get the block height a transaction was included in
    public static uint GetTxHeight(UInt256 txHash)
    {
        return LedgerContract.GetTransactionHeight(txHash);
    }
}
```

**GAS Costs:** Accessing properties of `Block` and `Transaction` objects retrieved from the `LedgerContract` incurs GAS costs.

[Previous: GasToken (GAS)](./GasToken.md) | [Next: NameService (NNS)](./NameService.md)