# Tutorial: Using Oracles

This tutorial shows how to use the native `OracleContract` to fetch external data (e.g., a cryptocurrency price) from a URL and store it within your smart contract.

## 1. Project Setup

1.  Create a new C# class library project (`dotnet new classlib -n OraclePriceFeed`).
2.  Add NuGet packages: `Neo.SmartContract.Framework`, `Neo.Compiler.CSharp`.
3.  Configure the `.csproj` file, including necessary permissions.

## 2. Contract Code (`OraclePriceFeed.cs`)

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Examples.Oracle
{
    [DisplayName("OraclePriceFeed")]
    [ManifestExtra("Author", "Your Name")]
    // Permissions needed:
    [ContractPermission(nameof(OracleContract), "request", "getPrice")] // To make requests
    [ContractPermission(nameof(GasToken), "transfer")] // To pay Oracle fees
    [ContractPermission(nameof(StdLib), "atoi")] // To parse the result
    [ContractPermission(nameof(Runtime), "log")] // For logging
    public class OraclePriceFeed : SmartContract
    {
        // Storage for the latest price
        private static readonly byte[] LatestPriceKey = { 0x01 };
        private static readonly byte[] RequestorKey = { 0x02 }; // Who can request updates

        // Event for price updates
        public delegate void PriceUpdatedDelegate(BigInteger newPrice, ulong timestamp);
        [DisplayName("PriceUpdated")]
        public static event PriceUpdatedDelegate OnPriceUpdated;

        // Deploy: Set who can request updates
        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                // Set initial requestor (e.g., deployer)
                Storage.Put(Storage.CurrentContext, RequestorKey, Runtime.Transaction.Sender);
            }
        }

        // --- Price Request --- 

        // Only the authorized requestor can trigger an update
        public static void RequestNeoUsdPriceUpdate()
        {
            ByteString requestor = Storage.Get(Storage.CurrentContext, RequestorKey);
            Helper.Assert(requestor != null && Runtime.CheckWitness((UInt160)requestor), "Unauthorized: Only designated requestor can update price");

            // Define request parameters
            // Note: Use a reliable, HTTPS supporting API endpoint
            string url = "https://api.coingecko.com/api/v3/simple/price?ids=neo&vs_currencies=usd"; // Example API
            string filter = "$.neo.usd"; // JSONPath filter for USD price
            string callbackMethod = "__oracleCallback"; // Must match method below
            object userData = "NEO_USD_PRICE"; // Custom data to identify this request type
            long gasForResponse = Oracle.MinimumResponseFee; // Minimum GAS for callback execution

            // Calculate and pay fee
            long requestFee = OracleContract.GetPrice();
            long totalGasFee = requestFee + gasForResponse;
            if (!GasToken.Transfer((UInt160)requestor, OracleContract.Hash, totalGasFee, null))
            {
                throw new System.Exception("Failed to pay Oracle GAS fee");
            }

            // Send the request
            OracleContract.Request(url, filter, callbackMethod, userData, gasForResponse);
            Runtime.Log("Oracle price request sent.");
        }

        // --- Oracle Callback --- 

        // This method MUST be public static and match the name in Request()
        public static void __oracleCallback(string url, object userData, int code, ByteString result)
        { 
            // CRITICAL: Verify the caller is the Oracle Contract
            if (Runtime.CallingScriptHash != OracleContract.Hash) 
            { 
                Runtime.Log("Callback verification failed! Caller: " + Runtime.CallingScriptHash);
                throw new System.Exception("Unauthorized callback caller.");
            }

            // Check if the request identifier matches what we expect
            if ((string)userData != "NEO_USD_PRICE") 
            { 
                Runtime.Log("Callback received for unexpected userData: " + (string)userData);
                return; // Ignore if it's not the request we care about
            }

            // Check the response code
            if (code != OracleResponseCode.Success)
            {
                Runtime.Log($"Oracle request failed for URL {url} with code: {code}");
                // Optionally store error state
                return;
            }

            // Process the successful result
            Runtime.Log("Oracle price request successful.");
            try
            {
                // Assuming the API returns a number (potentially with decimals)
                // CoinGecko example returns a simple number like 9.87
                // We need to handle potential decimals - let's store price * 100
                string priceString = Helper.AsString(result); // e.g., "9.87"
                BigInteger price = ParseDecimalString(priceString, 2); // Helper to parse "9.87" to 987

                // Store the latest price
                Storage.Put(Storage.CurrentContext, LatestPriceKey, price);

                // Emit event
                OnPriceUpdated(price, Runtime.Time); 
                Runtime.Log("Stored price: " + price);
            }
            catch (System.Exception e)
            {
                Runtime.Log("Failed to parse Oracle result: " + e.Message);
                // Optionally store error state
            }
        }

        // --- Price Retrieval --- 

        [Safe]
        public static BigInteger GetLatestPrice()
        {
            // Returns price * 100 (or chosen precision)
            return (BigInteger)Storage.Get(Storage.CurrentContext, LatestPriceKey);
        }

        // --- Helper Function --- 

        // Basic helper to parse a decimal string like "12.34" into an integer
        // representing the value multiplied by 10^decimals
        private static BigInteger ParseDecimalString(string value, int decimals)
        { 
             string integerPart = value;
             string fractionalPart = "";
             int decimalPoint = value.IndexOf('.');

             if (decimalPoint != -1) {
                 integerPart = value.Substring(0, decimalPoint);
                 fractionalPart = value.Substring(decimalPoint + 1);
             }

            BigInteger intValue = integerPart.Length > 0 ? StdLib.Atoi(integerPart, 10) : 0;
            BigInteger fracValue = 0;

            if (fractionalPart.Length > 0) {
                // Trim or pad fractional part to match desired decimals
                if (fractionalPart.Length > decimals) {
                    fractionalPart = fractionalPart.Substring(0, decimals);
                } else if (fractionalPart.Length < decimals) {
                    fractionalPart = fractionalPart.PadRight(decimals, '0');
                }
                fracValue = StdLib.Atoi(fractionalPart, 10);
            }

            BigInteger multiplier = BigInteger.Pow(10, decimals);
            return intValue * multiplier + fracValue;
        }
    }
}
```

## 3. Key Elements Explained

*   **Permissions:** Crucially requires permissions for `OracleContract`, `GasToken`, and potentially `StdLib`.
*   **Request Trigger (`RequestNeoUsdPriceUpdate`):**
    *   Protected by `CheckWitness` to ensure only authorized accounts can initiate requests.
    *   Defines the URL, filter, callback function name (`__oracleCallback`), and user data.
    *   Calculates the total GAS fee (request fee + callback execution fee).
    *   Transfers the fee to the `OracleContract.Hash`.
    *   Calls `OracleContract.Request`.
*   **Callback Method (`__oracleCallback`):**
    *   Must be `public static` and match the name provided in the request.
    *   **MUST** verify `Runtime.CallingScriptHash == OracleContract.Hash` to prevent fake callbacks.
    *   Checks the `userData` to identify which request this callback corresponds to.
    *   Checks the `code` for success or errors.
    *   Parses the `result` (ByteString). This often involves converting to a string (`Helper.AsString`) and potentially parsing JSON (`StdLib.JsonDeserialize`) or numbers (`StdLib.Atoi`).
    *   Handles potential decimal values appropriately (e.g., multiplying by a power of 10 to store as `BigInteger`).
    *   Stores the processed result.
    *   Emits an event (`OnPriceUpdated`).
*   **Storage:** Stores the latest processed price.
*   **Retrieval (`GetLatestPrice`):** A simple `[Safe]` method to read the stored price.
*   **Decimal Parsing:** Includes a helper function (`ParseDecimalString`) to demonstrate handling potential decimal numbers returned by APIs, storing them as scaled integers.

## 4. Compile and Deploy

1.  Compile: `dotnet build`
2.  Deploy the `.nef` and `.manifest.json` files.

## Important Considerations

*   **API Reliability & Trust:** The security and accuracy of your contract depend heavily on the reliability and trustworthiness of the external API and the Neo Oracle nodes.
*   **HTTPS:** Most Oracles require HTTPS URLs.
*   **GAS:** Ensure enough GAS is paid for both the request and the callback execution.
*   **Callback Security:** The callback verification step is non-negotiable.
*   **Data Parsing:** Robustly handle potential errors during data parsing in the callback.

[Previous: Creating a Voting Contract](./02-voting-contract.md) | [Next Section: Testing & Deployment](../08-testing-deployment/README.md)