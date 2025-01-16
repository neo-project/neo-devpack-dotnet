// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_ClassInit.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ClassInit : DebugAndTestBase<Contract_ClassInit>
    {
        public struct IntInit
        {
            public int A;
            public BigInteger B;
        }

        [TestMethod]
        public void Test_InitInt()
        {
            var cs = new IntInit();

            using var fee = Engine.CreateGasWatcher();
            var result = Contract.TestInitInt();
            AssertGasConsumed(1045560);
            Assert.IsNotNull(result);

            Assert.AreEqual(cs.A, (BigInteger)result[0]);
            Assert.AreEqual(cs.B, (BigInteger)result[1]);
        }

        [TestMethod]
        public void Test_InitializationExpression()
        {
            Contract.TestInitializationExpression();
            AssertGasConsumed(1275690);
        }
    }
}
