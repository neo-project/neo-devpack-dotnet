using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

using System;
using System.ComponentModel;

namespace Neo.SmartContract.Template
{
    [DisplayName(nameof(Contract1))]
    [ManifestExtra("Author", "<Your Name Or Company Here>")]
    [ManifestExtra("Description", "<Description Here>")]
    [ManifestExtra("Email", "<Your Public Email Here>")]
    [ManifestExtra("Version", "<Version String Here>")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractoracle/OracleRequest.cs")]
    [ContractPermission("*", "*")]
    public class OracleRequest : Neo.SmartContract.Framework.SmartContract
    {
        public delegate void OnRequestSuccessfulDelegate(string requestedUrl, object jsonValue);

        [DisplayName("RequestSuccessful")]
        public static event OnRequestSuccessfulDelegate OnRequestSuccessful;

        // TODO: Replace it with your own address.
        [InitialValue("<Your Address Here>", ContractParameterType.Hash160)]
        static readonly UInt160 Owner = default;

        private static bool IsOwner() => Runtime.CheckWitness(Owner);

        // When this contract address is included in the transaction signature,
        // this method will be triggered as a VerificationTrigger to verify that the signature is correct.
        // For example, this method needs to be called when withdrawing token from the contract.
        [Safe]
        public static bool Verify() => IsOwner();

        // TODO: Replace it with your methods.
        public static string MyMethod()
        {
            return Storage.Get(Storage.CurrentContext, "Hello");
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // This will be executed during update
                return;
            }

            // This will be executed during deploy
            Storage.Put(Storage.CurrentContext, "Hello", "World");
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void Destroy()
        {
            if (!IsOwner()) throw new Exception("No authorization.");
            ContractManagement.Destroy();
        }

        // TODO: Add your own logic
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
        public static void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode oracleResponse, string jsonString)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new InvalidOperationException("No Authorization!");
            if (oracleResponse != OracleResponseCode.Success)
                throw new Exception("Oracle response failure with code " + (byte)oracleResponse);

            var jsonArrayValues = (object[])StdLib.JsonDeserialize(jsonString);
            var jsonFirstValue = (string)jsonArrayValues[0];

            OnRequestSuccessful(requestedUrl, jsonFirstValue);
        }
    }
}
