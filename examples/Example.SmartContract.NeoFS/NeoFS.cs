// Copyright (C) 2015-2026 The Neo Project.
//
// NeoFS.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace NeoFS
{
    /// <summary>
    /// Demonstrates how to request NeoFS object payloads through the native Oracle contract.
    /// </summary>
    [DisplayName("SampleNeoFS")]
    [ContractAuthor("code-dev", "dev@neo.org")]
    [ContractDescription("A sample contract to demonstrate how to request NeoFS objects through the Oracle service")]
    [ContractVersion("0.0.1")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class SampleNeoFS : SmartContract, IOracle
    {
        private const string ContainerId = "C3swfg8MiMJ9bXbeFG6dWJTCoHp9hAEZkHezvbSwK1Cc";
        private const string ObjectId = "3nQH1L8u3eM9jt2mZCs6MyjzdjerdSzBkXCYYj4M4Znk";
        private const string PayloadKey = "NeoFSPayload";

        /// <summary>
        /// Builds a NeoFS URL that returns the full object payload.
        /// </summary>
        [Safe]
        public static string GetObjectUri()
        {
            return "neofs://" + ContainerId + "/" + ObjectId;
        }

        /// <summary>
        /// Builds a NeoFS URL that returns a byte range from the object payload.
        /// </summary>
        [Safe]
        public static string GetRangeUri(uint offset, uint length)
        {
            return GetObjectUri() + "/range/" + offset + "|" + length;
        }

        /// <summary>
        /// Builds a NeoFS URL that returns the object header.
        /// </summary>
        [Safe]
        public static string GetHeaderUri()
        {
            return GetObjectUri() + "/header";
        }

        /// <summary>
        /// Builds a NeoFS URL that returns the object payload hash.
        /// </summary>
        [Safe]
        public static string GetHashUri()
        {
            return GetObjectUri() + "/hash";
        }

        /// <summary>
        /// Returns the last payload accepted from the Oracle callback.
        /// </summary>
        [Safe]
        public static string GetStoredPayload()
        {
            var payload = Storage.Get(Storage.CurrentReadOnlyContext, PayloadKey);
            return payload is null ? "" : (string)payload;
        }

        /// <summary>
        /// Requests the full NeoFS object payload.
        /// </summary>
        public static void RequestObject()
        {
            Oracle.Request(GetObjectUri(), "", Method.OnOracleResponse, "", Oracle.MinimumResponseFee);
        }

        /// <summary>
        /// Requests part of the NeoFS object payload.
        /// </summary>
        public static void RequestRange(uint offset, uint length)
        {
            Oracle.Request(GetRangeUri(offset, length), "", Method.OnOracleResponse, "", Oracle.MinimumResponseFee);
        }

        /// <summary>
        /// Stores a successful NeoFS Oracle response.
        /// </summary>
        public void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode oracleResponse, string payload)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new InvalidOperationException("No Authorization!");
            if (oracleResponse != OracleResponseCode.Success)
                throw new Exception("Oracle response failure with code " + (byte)oracleResponse);

            Storage.Put(PayloadKey, payload);
        }
    }
}
