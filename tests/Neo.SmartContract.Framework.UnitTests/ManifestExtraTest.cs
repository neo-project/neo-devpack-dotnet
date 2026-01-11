// Copyright (C) 2015-2026 The Neo Project.
//
// ManifestExtraTest.cs file belongs to the neo project and is free
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
    public class ManifestExtraTest
    {
        public ManifestExtraTest()
        {
            // Ensure also Contract_ExtraAttribute

            TestCleanup.TestInitialize(typeof(Contract_ExtraAttribute));
        }

        [TestMethod]
        public void TestExtraAttribute()
        {
            var extra = Contract_ExtraAttribute.Manifest.Extra;

            Assert.AreEqual("Neo", extra?["Author"]?.GetString());
            Assert.AreEqual("dev@neo.org", extra?["E-mail"]?.GetString());
        }
    }
}
