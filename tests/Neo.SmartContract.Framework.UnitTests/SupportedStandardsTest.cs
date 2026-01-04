// Copyright (C) 2015-2026 The Neo Project.
//
// SupportedStandardsTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class SupportedStandardsTest
    {
        public SupportedStandardsTest()
        {
            // Ensure also Contract_ExtraAttribute
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandards));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard11Enum));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard26));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard17Enum));
            TestCleanup.TestInitialize(typeof(Contract_SupportedStandard27));
        }

        [TestMethod]
        public void TestAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandards.Manifest.SupportedStandards, new string[] { "NEP-17", "NEP-9" });
        }

        [TestMethod]
        public void TestStandardNEP11AttributeEnum()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard11Enum.Manifest.SupportedStandards, new string[] { "NEP-11" });
        }

        [TestMethod]
        public void TestStandardNEP17AttributeEnum()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard17Enum.Manifest.SupportedStandards, new string[] { "NEP-17" });
        }

        [TestMethod]
        public void TestStandardNEP11PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard26.Manifest.SupportedStandards, new string[] { "NEP-26" });
        }

        [TestMethod]
        public void TestStandardNEP17PayableAttribute()
        {
            CollectionAssert.AreEqual(Contract_SupportedStandard27.Manifest.SupportedStandards, new string[] { "NEP-27" });
        }
    }
}
