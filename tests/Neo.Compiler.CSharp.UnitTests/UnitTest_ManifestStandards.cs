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

            string output = CheckStandards(json);
            StringAssert.Contains(output, "tokensOf, it's parameters type is not a Hash160");
            StringAssert.Contains(output, "ownerOf, it's parameters type is not a ByteArray");
        }

        [TestMethod]
        public void Nep11_InvalidTokensOfAndOwnerOfShapeVariantsStillProduceExpectedDiagnostics()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            // Remove tokensOf entirely to hit the helper's null branch.
            JToken tokensOf = methods.First(m => m!["name"]!.GetString() == "tokensOf")!;
            methods.Remove(tokensOf);

            // Change ownerOf to hit unsafe, return-type, and parameter-type branches while preserving lookup shape.
            JObject ownerOf = (JObject)methods.First(m => m!["name"]!.GetString() == "ownerOf")!;
            ownerOf["safe"] = false;
            ownerOf["returntype"] = "ByteArray";
            ownerOf["parameters"] = new JArray(
                new JObject { ["name"] = "tokenId", ["type"] = "Hash160" });

            string output = CheckStandards(json);
            StringAssert.Contains(output, "tokensOf, it is not found in the ABI");
            StringAssert.Contains(output, "ownerOf, it is not safe");
            StringAssert.Contains(output, "ownerOf, it's return type is not a Hash160 or an InteropInterface");
            StringAssert.Contains(output, "ownerOf, it's parameters type is not a ByteArray");
        }

        [TestMethod]
        public void Nep11_DivisibleMethodShape_IsAccepted()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            JObject ownerOf = (JObject)methods.First(m => m!["name"]!.GetString() == "ownerOf")!;
            ownerOf["returntype"] = "InteropInterface";

            methods.Add(new JObject
            {
                ["name"] = "balanceOf",
                ["parameters"] = new JArray(
                    new JObject { ["name"] = "owner", ["type"] = "Hash160" },
                    new JObject { ["name"] = "tokenId", ["type"] = "ByteArray" }),
                ["returntype"] = "Integer",
                ["offset"] = 0,
                ["safe"] = true
            });

            JObject transfer = (JObject)methods.First(m => m!["name"]!.GetString() == "transfer")!;
            transfer["parameters"] = new JArray(
                new JObject { ["name"] = "from", ["type"] = "Hash160" },
                new JObject { ["name"] = "to", ["type"] = "Hash160" },
                new JObject { ["name"] = "amount", ["type"] = "Integer" },
                new JObject { ["name"] = "tokenId", ["type"] = "ByteArray" },
                new JObject { ["name"] = "data", ["type"] = "Any" });

            Assert.AreEqual(string.Empty, CheckStandards(json));
        }

        [TestMethod]
        public void Nep11_DivisibleSpecificBalanceOf_DoesNotReplaceCommonBalanceOf()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            JObject balanceOf = (JObject)methods.First(m => m!["name"]!.GetString() == "balanceOf")!;
            balanceOf["parameters"] = new JArray(
                new JObject { ["name"] = "owner", ["type"] = "Hash160" },
                new JObject { ["name"] = "tokenId", ["type"] = "ByteArray" });

            string output = CheckStandards(json);
            StringAssert.Contains(output, "balanceOf, it is not found in the ABI");
        }

        [TestMethod]
        public void Nep11_MissingOwnerOf_ProducesExpectedDiagnostic()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            JToken ownerOf = methods.First(m => m!["name"]!.GetString() == "ownerOf")!;
            methods.Remove(ownerOf);

            string output = CheckStandards(json);
            StringAssert.Contains(output, "ownerOf, it is not found in the ABI");
        }

        [TestMethod]
        public void Nep11_TokensOfShapeVariants_ProduceExpectedDiagnostics()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray methods = (JArray)json["abi"]!["methods"]!;

            JObject tokensOf = (JObject)methods.First(m => m!["name"]!.GetString() == "tokensOf")!;
            tokensOf["safe"] = false;
            tokensOf["returntype"] = "Hash160";

            string output = CheckStandards(json);
            StringAssert.Contains(output, "tokensOf, it is not safe");
            StringAssert.Contains(output, "tokensOf, it's return type is not an InteropInterface");
        }

        [TestMethod]
        public void Nep11_MalformedTransferEvent_DoesNotCrashAndReportsLength()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray events = (JArray)json["abi"]!["events"]!;

            // A Transfer event with fewer than 4 parameters previously crashed the
            // compiler with IndexOutOfRangeException because the event is matched by
            // name only and the per-parameter checks indexed it unconditionally.
            JObject transferEvent = (JObject)events.First(e => e!["name"]!.GetString() == "Transfer")!;
            transferEvent["parameters"] = new JArray(
                new JObject { ["name"] = "from", ["type"] = "Hash160" });

            string output = CheckStandards(json);
            StringAssert.Contains(output, "transfer, it's parameters length is not 4");
        }

        [TestMethod]
        public void Nep11_TransferEventWithExpectedLength_ReportsParameterTypeErrors()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP11.Manifest.ToJson().ToString(false))!;
            JArray events = (JArray)json["abi"]!["events"]!;

            JObject transferEvent = (JObject)events.First(e => e!["name"]!.GetString() == "Transfer")!;
            transferEvent["parameters"] = new JArray(
                new JObject { ["name"] = "from", ["type"] = "ByteArray" },
                new JObject { ["name"] = "to", ["type"] = "ByteArray" },
                new JObject { ["name"] = "amount", ["type"] = "Hash160" },
                new JObject { ["name"] = "tokenId", ["type"] = "Integer" });

            string output = CheckStandards(json);
            StringAssert.Contains(output, "transfer, it's first parameters type is not a Hash160");
            StringAssert.Contains(output, "transfer, it's second parameters type is not a Hash160");
            StringAssert.Contains(output, "transfer, it's third parameters type is not an Integer");
            StringAssert.Contains(output, "transfer, it's fourth parameters type is not a ByteArray");
        }

        [TestMethod]
        public void Nep17_MalformedTransferEvent_DoesNotCrashAndReportsLength()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP17.Manifest.ToJson().ToString(false))!;
            JArray events = (JArray)json["abi"]!["events"]!;

            JObject transferEvent = (JObject)events.First(e => e!["name"]!.GetString() == "Transfer")!;
            transferEvent["parameters"] = new JArray(
                new JObject { ["name"] = "from", ["type"] = "Hash160" });

            string output = CheckStandards(json);
            StringAssert.Contains(output, "Transfer event parameters length is not 3");
        }

        [TestMethod]
        public void Nep17_TransferEventWithExpectedLength_ReportsParameterTypeErrors()
        {
            JObject json = (JObject)JToken.Parse(Contract_NEP17.Manifest.ToJson().ToString(false))!;
            JArray events = (JArray)json["abi"]!["events"]!;

            JObject transferEvent = (JObject)events.First(e => e!["name"]!.GetString() == "Transfer")!;
            transferEvent["parameters"] = new JArray(
                new JObject { ["name"] = "from", ["type"] = "ByteArray" },
                new JObject { ["name"] = "to", ["type"] = "Integer" },
                new JObject { ["name"] = "amount", ["type"] = "Hash160" });

            string output = CheckStandards(json);
            StringAssert.Contains(output, "Transfer event first parameter (from) type is not Hash160");
            StringAssert.Contains(output, "Transfer event second parameter (to) type is not Hash160");
            StringAssert.Contains(output, "Transfer event third parameter (amount) type is not Integer");
        }

        private static string CheckStandards(JObject json)
        {
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

            return stdout.ToString();
        }
    }
}
