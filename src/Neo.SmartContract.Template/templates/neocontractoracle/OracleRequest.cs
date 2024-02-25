using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Neo.SmartContract.Template
{
    [DisplayName(nameof(OracleRequest))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ManifestExtra("Description", "<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractoracle/OracleRequest.cs")]
    [ContractPermission("*", "*")]
    public class OracleRequest : Neo.SmartContract.Framework.SmartContract
    {
        [Safe]
        public static string GetResponse()
        {
            return Storage.Get(Storage.CurrentContext, "Response");
        }

        public static void DoRequest()
        {
            /*
                JSON DATA EXAMPLE
                {
                    "id": "6520ad3c12a5d3765988542a",
                    "record": {
                        "propertyName": "Hello World!"
                    },
                    "metadata": {
                        "name": "HelloWorld",
                        "readCountRemaining": 98,
                        "timeToExpire": 86379,
                        "createdAt": "2023-10-07T00:58:36.746Z"
                    }
                }
                See JSONPath format at https://github.com/atifaziz/JSONPath
                JSONPath = "$.record.propertyName"
                ReturnValue = ["Hello World!"]
                ReturnValueType = string array
            */
            var requestUrl = "https://api.jsonbin.io/v3/qs/6520ad3c12a5d3765988542a";
            Oracle.Request(requestUrl, "$.record.propertyName", "onOracleResponse", null, Oracle.MinimumResponseFee);
        }

        // This method is called after the Oracle receives response from requested URL
        public static void onOracleResponse(string requestedUrl, object userData, OracleResponseCode oracleResponse, string jsonString)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new InvalidOperationException("No Authorization!");
            if (oracleResponse != OracleResponseCode.Success)
                throw new Exception("Oracle response failure with code " + (byte)oracleResponse);

            var jsonArrayValues = (object[])StdLib.JsonDeserialize(jsonString);
            var jsonFirstValue = (string)jsonArrayValues[0];

            Storage.Put(Storage.CurrentContext, "Response", jsonFirstValue);
        }
    }
}
