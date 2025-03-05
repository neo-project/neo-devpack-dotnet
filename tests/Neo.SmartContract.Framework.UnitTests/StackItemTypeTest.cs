// Copyright (C) 2015-2025 The Neo Project.
//
// StackItemTypeTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using scfxStackItemType = scfx.Neo.SmartContract.Framework.StackItemType;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StackItemTypeTest
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(((byte)VM.Types.StackItemType.Buffer).ToString("x2"), scfxStackItemType.Buffer[2..]);
            Assert.AreEqual(((byte)VM.Types.StackItemType.Integer).ToString("x2"), scfxStackItemType.Integer[2..]);
        }
    }
}
