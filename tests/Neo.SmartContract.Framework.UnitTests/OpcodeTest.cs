// Copyright (C) 2015-2024 The Neo Project.
//
// OpcodeTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FrameworkOpCode = scfx.Neo.SmartContract.Framework.OpCode;
using VMOpCode = Neo.VM.OpCode;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class OpcodeTest
    {
        [TestMethod]
        public void TestAllOpcodes()
        {
            // Names
            int i = Enum.GetNames(typeof(VMOpCode)).Length;
            int j = Enum.GetNames(typeof(FrameworkOpCode)).Length;
            Assert.AreEqual(i, j);

            CollectionAssert.AreEqual
                (
                Enum.GetNames(typeof(VMOpCode)),
                Enum.GetNames(typeof(FrameworkOpCode))
                );

            // Values

            CollectionAssert.AreEqual
                (
                Enum.GetValues(typeof(VMOpCode)).Cast<VMOpCode>().Select(u => (byte)u).ToArray(),
                Enum.GetValues(typeof(FrameworkOpCode)).Cast<FrameworkOpCode>().Select(u => (byte)u).ToArray()
                );
        }
    }
}
