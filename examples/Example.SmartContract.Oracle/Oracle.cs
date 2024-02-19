// Copyright (C) 2015-2024 The Neo Project.
//
// Oracle.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace Oracle
{
    [DisplayName("SampleOracle")]
    [ContractAuthor("code-dev", "core@neo.org")]
    [ContractDescription("A sample contract to demonstrate how to use Example.SmartContract.Oracle Service")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/examples/Example.SmartContract.Oracle")]
    [ContractPermission(Permission.WildCard, Method.WildCard)]
    public class SampleOracle : SmartContract
    {

        // (string requestedUrl, object jsonValue)
        [DisplayName("RequestSuccessful")]
        public static event Action<string, string> OnRequestSuccessful;
        public static void DoRequest()
        {
            /*
                JSON DATA
                {
                    "id": "6520ad3c12a5d3765988542a",
                    "record": {
                        "propertyName": "Hello World!"
                    },
                    "metadata": {
                        "name": "Example.SmartContract.HelloWorld",
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
            Neo.SmartContract.Framework.Native.Oracle.Request(requestUrl, "$.record.propertyName", "onOracleResponse", null, Neo.SmartContract.Framework.Native.Oracle.MinimumResponseFee);
        }

        // This method is called after the Example.SmartContract.Oracle receives response from requested URL
        public static void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode oracleResponse, string jsonString)
        {
            if (Runtime.CallingScriptHash != Neo.SmartContract.Framework.Native.Oracle.Hash)
                throw new InvalidOperationException("No Authorization!");
            if (oracleResponse != OracleResponseCode.Success)
                throw new Exception("Example.SmartContract.Oracle response failure with code " + (byte)oracleResponse);

            var jsonArrayValues = (object[])StdLib.JsonDeserialize(jsonString);
            var jsonFirstValue = (string)jsonArrayValues[0];

            OnRequestSuccessful(requestedUrl, jsonFirstValue);
        }

        [DisplayName("_deploy")]
        public static void OnDeployment(object data, bool update)
        {
            if (update)
            {
                // Add logic for fixing contract on update
                return;
            }
            // Add logic here for 1st time deployed
        }

        // TODO: Allow ONLY contract owner to call update
        public static bool Update(ByteString nefFile, string manifest)
        {
            ContractManagement.Update(nefFile, manifest);
            return true;
        }

        // TODO: Allow ONLY contract owner to call destroy
        public static bool Destroy()
        {
            ContractManagement.Destroy();
            return true;
        }
    }
}
