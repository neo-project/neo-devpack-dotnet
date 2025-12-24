// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_ABI_Fee.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Fee : DebugAndTestBase<Contract_ABIFee>
    {
        [TestMethod]
        public void TestNoFeeMethod()
        {
            // Method without fee should not have fee field
            var method = Contract_ABIFee.Manifest.Abi.GetMethod("noFeeMethod", 0);
            Assert.IsNotNull(method);

            // Check that the method exists and works
            Assert.AreEqual(1, Contract.NoFeeMethod());
        }

        [TestMethod]
        public void TestFixedFeeMethod()
        {
            // Method with fixed fee should have fee in ABI
            var manifest = Contract_ABIFee.Manifest;
            var abiJson = manifest.ToJson();
            var methods = abiJson["abi"]!["methods"] as JArray;

            JObject? fixedFeeMethod = null;
            foreach (var m in methods!)
            {
                if (m!["name"]!.GetString() == "fixedFeeMethod")
                {
                    fixedFeeMethod = m as JObject;
                    break;
                }
            }

            Assert.IsNotNull(fixedFeeMethod);
            var fee = fixedFeeMethod["fee"];
            Assert.IsNotNull(fee);
            Assert.AreEqual("fixed", fee["mode"]!.GetString());
            Assert.AreEqual(100000000, (long)fee["amount"]!.AsNumber());
            Assert.AreEqual("NM7Aky765FG8NhhwtxjXRx7jEL1cnw7PBP", fee["beneficiary"]!.GetString());

            // Check that the method works
            Assert.AreEqual(2, Contract.FixedFeeMethod());
        }

        [TestMethod]
        public void TestDynamicFeeMethod()
        {
            // Method with dynamic fee should have fee in ABI
            var manifest = Contract_ABIFee.Manifest;
            var abiJson = manifest.ToJson();
            var methods = abiJson["abi"]!["methods"] as JArray;

            JObject? dynamicFeeMethod = null;
            foreach (var m in methods!)
            {
                if (m!["name"]!.GetString() == "dynamicFeeMethod")
                {
                    dynamicFeeMethod = m as JObject;
                    break;
                }
            }

            Assert.IsNotNull(dynamicFeeMethod);
            var fee = dynamicFeeMethod["fee"];
            Assert.IsNotNull(fee);
            Assert.AreEqual("dynamic", fee["mode"]!.GetString());
            Assert.AreEqual("NM7Aky765FG8NhhwtxjXRx7jEL1cnw7PBP", fee["beneficiary"]!.GetString());
            Assert.AreEqual("0xb2a4cff31913016155e38e474a2c06d08be296cf", fee["dynamicScriptHash"]!.GetString());

            // Check that the method works
            Assert.AreEqual(3, Contract.DynamicFeeMethod());
        }
    }
}
