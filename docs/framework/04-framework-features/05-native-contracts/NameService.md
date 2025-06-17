# Native Contract: NameService (NNS)

Namespace: `Neo.SmartContract.Framework.Native`

Provides interaction with the Neo Name Service (NNS), a decentralized system for mapping human-readable names (like `mywallet.neo`) to Neo addresses or other data.

## Key Methods

*   **`AddRoot(string root)`**: Adds a new root domain (e.g., ".neo", ".gas"). Requires authorization from the NNS admins (usually the Neo Council).

*   **`Register(string name, UInt160 owner)` (`bool`)**: Registers a second-level domain name (e.g., "mywallet" under the ".neo" root).
    *   `name`: The full domain name (e.g., "mywallet.neo").
    *   `owner`: The address that will own the registered name.
    *   Requires payment of a registration fee in GAS.
    *   Requires `Runtime.CheckWitness` of the account paying the fee.
    *   Returns `true` on success.

*   **`Renew(string name)` (`uint`)**: Renews the registration of a domain name, extending its validity period.
    *   Requires payment of a renewal fee in GAS.
    *   Returns the new expiration timestamp.

*   **`SetAdmin(string name, UInt160 admin)`**: Sets an administrative address for a domain. The admin can manage records but doesn't own the name.
    *   Requires `Runtime.CheckWitness` of the domain owner.

*   **`SetRecord(string name, RecordType type, string data)`**: Sets the data associated with a domain name record.
    *   `name`: The domain name (e.g., "mywallet.neo" or subdomains like "sub.mywallet.neo").
    *   `type` (`RecordType` enum): The type of record (e.g., `A` for N3 address, `CNAME` for alias, `TXT` for text data).
    *   `data`: The content of the record.
    *   Requires `Runtime.CheckWitness` of the domain owner or admin.

*   **`GetRecord(string name, RecordType type)` (`string`)**: Retrieves the data for a specific record type associated with a domain name.
    *   Returns `null` if the name or record type doesn't exist.

*   **`DeleteRecord(string name, RecordType type)`**: Deletes a specific record.
    *   Requires `Runtime.CheckWitness` of the domain owner or admin.

*   **`GetAllRecords(string name)` (`Iterator`)**: Returns an iterator over all records (type and data) associated with a name.

*   **`Resolve(string name, RecordType type)` (`string`)**: Resolves a domain name to its corresponding record data, handling CNAME records recursively.
    *   This is the primary method for looking up addresses or other data associated with a name.

*   **`OwnerOf(string name)` (`UInt160`)**: Gets the owner address of a registered domain name.

*   **`Transfer(UInt160 to, string name)` (`bool`)**: Transfers ownership of a domain name.
    *   Requires `Runtime.CheckWitness` of the current owner.
    *   Requires payment of a transfer fee in GAS.
    *   Returns `true` on success.

*   **`IsAvailable(string name)` (`bool`)**: Checks if a domain name is currently available for registration.

*   **`GetPrice()` (`long`)**: Gets the current registration fee in GAS.

## `RecordType` Enum

*   `A`: N3 Address (UInt160)
*   `CNAME`: Canonical Name (alias to another NNS name)
*   `TXT`: Text record
*   `AAAA`: IPv6 Address

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

// Permissions required for the methods used
[ContractPermission(nameof(NameService), "resolve", "isAvailable", "register")]
[ContractPermission(nameof(GasToken), "transfer")] // Needed for registration fee
public class NnsDemo : SmartContract
{
    // Resolve a .neo address
    public static string ResolveAddress(string name)
    { 
        // name should be like "mywallet.neo"
        return NameService.Resolve(name, RecordType.A);
    }

    // Check if a name is available
    public static bool CheckAvailability(string name)
    { 
        // name should be like "newname.neo"
        return NameService.IsAvailable(name);
    }

    // Register a name for the caller (requires caller to pay GAS)
    public static bool RegisterNameForCaller(string name)
    {
        UInt160 caller = Runtime.Transaction.Sender; // Or get from Signers
        if (!Runtime.CheckWitness(caller)) return false; // Ensure caller signed

        if (!NameService.IsAvailable(name)) 
        {
            Runtime.Log("Name not available.");
            return false;
        }

        long price = NameService.GetPrice();

        // Transfer registration fee to NNS contract
        if (!GasToken.Transfer(caller, NameService.Hash, price, null))
        {
            Runtime.Log("Failed to pay registration fee.");
            return false;
        }

        // Attempt registration
        bool success = NameService.Register(name, caller);
        if (success)
        { 
            Runtime.Log("Name registered successfully.");
        } else {
            // Important: If registration fails, the fee is NOT automatically refunded.
            // Need to handle potential refunds if necessary.
            Runtime.Log("Name registration failed after payment.");
        }
        return success;
    }
}
```

**Important:** Interacting with NNS methods that require fees (`Register`, `Renew`, `Transfer`) involves transferring GAS to the `NameService.Hash` address within the same transaction.

[Previous: LedgerContract](./Ledger.md) | [Next: NeoToken (NEO)](./NeoToken.md)