// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ManifestStandards.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ManifestStandards
    {
        [TestMethod]
        public void Nep11_InvalidTokensOfAndOwnerOfStillProduceExpectedDiagnostics()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            JObject tokensOf = (JObject)methods.First(m => m!["name"]!.GetString() == "tokensOf")!;
            tokensOf["parameters"]![0]!["type"] = "ByteArray";

            JObject ownerOf = (JObject)methods.First(m => m!["name"]!.GetString() == "ownerOf")!;
            ownerOf["parameters"]![0]!["type"] = "Hash160";

            ContractManifest manifest = ContractManifest.FromJson(json);
            var stdout = new StringWriter();
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetOut(stdout);
                manifest.CheckStandards();
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            string output = stdout.ToString();
            StringAssert.Contains(output, "tokensOf, it's parameters type is not a Hash160");
            StringAssert.Contains(output, "ownerOf, it's parameters type is not a ByteArray");
        }
    }
}
