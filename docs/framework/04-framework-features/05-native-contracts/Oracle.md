# Native Contract: OracleContract

Namespace: `Neo.SmartContract.Framework.Native`

Provides the functionality for smart contracts to request data from external resources (URLs) off-chain.

## Oracle Process

1.  **Request:** Your smart contract calls `OracleContract.Request` specifying the URL, filter (optional), callback method, user data (optional), and GAS for the response.
2.  **Fee Payment:** Your contract transfers the required GAS fee to the `OracleContract.Hash`.
3.  **Off-Chain Fetching:** Designated Oracle nodes monitor these requests, fetch data from the URL, optionally apply the filter, and reach consensus on the result.
4.  **Callback:** An Oracle node submits a transaction that calls the specified callback method (`__callback`) in your contract, passing the original user data and the fetched result.

## Key Methods

*   **`Request(string url, string filter, string callback, object userData, long gasForResponse)`**: Initiates an oracle request.
    *   `url`: The HTTP/HTTPS URL to fetch data from (must be HTTPS unless explicitly allowed by Oracle policy).
    *   `filter` (optional): A JSONPath filter string (e.g., `$.data.price`) to extract specific data from the JSON response. Use `null` or empty string for no filter.
    *   `callback`: The name of the *public static* method in your *calling* contract that will receive the response (e.g., `"__callback"`).
    *   `userData` (optional): Any data you want to be passed back to your callback method to help identify the request.
    *   `gasForResponse`: The amount of GAS provided to execute the callback method. This GAS is consumed by the Oracle transaction invoking your callback.
    *   **Requires payment:** Your contract must transfer GAS to `OracleContract.Hash` to cover the request fee (obtained via `GetPrice`).

*   **`GetPrice()` (`long`)**: Returns the minimum GAS fee required per oracle request (excluding `gasForResponse`).

*   **`Verify()` (`bool`)**: Used internally by Oracle nodes to verify their callback transaction. Returns `true` if the calling script hash belongs to an active Oracle node. You typically **do not call this** directly.

## Implementing the Callback Method

Your contract *must* implement the public static callback method specified in the `Request` call.

```csharp
public static void __callback(string url, object userData, int code, ByteString result)
{
    // ... process the oracle response ...
}
```

*   **`url` (`string`)**: The original URL requested.
*   **`userData` (`object`)**: The user data you passed in the `Request` call.
*   **`code` (`int`)**: The Oracle response code (enum `OracleResponseCode`). `Success` (0) indicates a successful fetch.
*   **`result` (`ByteString`)**: The fetched data (after filtering, if applied), as raw bytes. You often need to convert this (`Helper.AsString(result)`) or parse it (e.g., using `StdLib.JsonDeserialize`).

**Important:** The callback method is invoked in a *separate transaction* initiated by an Oracle node. It must contain logic to verify that the caller is a legitimate Oracle node (e.g., by checking `Runtime.CallingScriptHash == OracleContract.Hash` or using `OracleContract.Verify()`, although direct use is less common) before trusting the `result`.

## Example Usage

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

[ContractPermission(nameof(OracleContract), "request", "getPrice")]
[ContractPermission(nameof(GasToken), "transfer")]
[ContractPermission(nameof(StdLib), "jsonDeserialize", "atoi")] // For processing JSON result
public class OracleDemo : SmartContract
{
    // Store results associated with user data
    private static readonly StorageMap PendingRequests = new StorageMap(Storage.CurrentContext, "PENDING");
    private static readonly StorageMap Results = new StorageMap(Storage.CurrentContext, "RESULT");

    // Request the price of NEO from a hypothetical API
    public static void RequestNeoPrice(string requestId)
    {
        string url = "https://api.example.com/prices/neo-usd"; // HTTPS is usually required
        string filter = "$.data.price"; // JSONPath filter to extract the price field
        string callbackMethod = "__callback"; // Must match the method name below
        long gasForResponse = Oracle.MinimumResponseFee; // Use minimum defined by Oracle nodes

        // Check if caller signed
        if (!Runtime.CheckWitness(Runtime.Transaction.Sender)) 
            throw new System.Exception("Unauthorized");

        // Calculate total GAS needed
        long requestFee = OracleContract.GetPrice();
        long totalGas = requestFee + gasForResponse;

        // Pay the fee
        if (!GasToken.Transfer(Runtime.Transaction.Sender, OracleContract.Hash, totalGas, null))
            throw new System.Exception("Failed to pay Oracle fee");

        // Make the request, passing our unique requestId as userData
        OracleContract.Request(url, filter, callbackMethod, requestId, gasForResponse);

        // Mark request as pending (optional, for tracking)
        PendingRequests.Put(requestId, true); 
        Runtime.Log("Oracle request sent.");
    }

    // Callback method MUST be public static
    public static void __callback(string url, object userData, int code, ByteString result)
    { 
        // IMPORTANT: Verify the caller is the OracleContract native hash
        if (Runtime.CallingScriptHash != OracleContract.Hash) 
        { 
             Runtime.Log("Callback verification failed!");
             throw new System.Exception("Unauthorized callback caller.");
        }
        
        Runtime.Log($"Oracle callback received for URL: {url}");
        string requestId = (string)userData; // Retrieve our unique ID

        if (code != OracleResponseCode.Success)
        {
            Runtime.Log($"Oracle request failed with code: {code}");
            Results.Put(requestId, "Error: " + code);
        } 
        else 
        { 
            Runtime.Log("Oracle request successful.");
            // Process the result (example: assume price is a string number)
            string priceString = Helper.AsString(result);
            Results.Put(requestId, priceString); 
            // BigInteger price = StdLib.Atoi(priceString, 10); // Convert if integer
            // Map<string, object> json = (Map<string, object>)StdLib.JsonDeserialize(result); // Parse if JSON
        }

        // Mark request as completed
        PendingRequests.Delete(requestId);
    }

    // Method to retrieve the stored result
    public static string GetResult(string requestId)
    {
        return Results.GetString(requestId);
    }
}
```

**Security:** Always verify the caller in your callback method (`Runtime.CallingScriptHash == OracleContract.Hash`) before trusting the provided data.

[Previous: NeoToken (NEO)](./NeoToken.md) | [Next: PolicyContract](./Policy.md)