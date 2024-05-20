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
using Neo.SmartContract.Framework.Interfaces;

namespace Oracle
{
    [DisplayName("SampleOracle")]
    [ContractAuthor("code-dev", "dev@neo.org")]
    [ContractDescription("A sample contract to demonstrate how to use Example.SmartContract.Oracle Service")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class SampleOracle : SmartContract, IOracle
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
            Neo.SmartContract.Framework.Native.Oracle.Request(requestUrl, "$.record.propertyName", Method.OnOracleResponse, null, Neo.SmartContract.Framework.Native.Oracle.MinimumResponseFee);
        }

        /// <summary>
        /// This implements the IOracle interface
        /// This method is called after the Oracle receives response from requested URL
        /// </summary>
        /// <param name="requestedUrl">Requested url</param>
        /// <param name="userData">User data provided during the request</param>
        /// <param name="oracleResponse">Oracle response code</param>
        /// <param name="jsonString">Oracle response data</param>
        /// <exception cref="InvalidOperationException">It was not called by the oracle</exception>
        /// <exception cref="Exception">It was not a success</exception>
        public void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode oracleResponse, string jsonString)
        {
            if (Runtime.CallingScriptHash != Neo.SmartContract.Framework.Native.Oracle.Hash)
                throw new InvalidOperationException("No Authorization!");
            if (oracleResponse != OracleResponseCode.Success)
                throw new Exception("Oracle response failure with code " + (byte)oracleResponse);

            var jsonArrayValues = (object[])StdLib.JsonDeserialize(jsonString);
            var jsonFirstValue = (string)jsonArrayValues[0];

            Storage.Put(Storage.CurrentContext, "Response", jsonFirstValue);
        }
    }
}
