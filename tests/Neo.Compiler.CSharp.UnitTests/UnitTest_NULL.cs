// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_NULL.cs file belongs to the neo project and is free
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
    public class UnitTest_NULL : DebugAndTestBase<Contract_NULL>
    {
        [TestMethod]
        public void IsNull()
        {
            // True

            Assert.IsTrue(Contract.IsNull(null));
            AssertGasConsumed(1047870);

            // False

            Assert.IsFalse(Contract.IsNull(1));
            AssertGasConsumed(1047870);
        }

        [TestMethod]
        public void IfNull()
        {
            Assert.IsFalse(Contract.IfNull(null));
            AssertGasConsumed(1047780);
        }

        [TestMethod]
        public void NullProperty()
        {
            Assert.IsTrue(Contract.NullProperty(null));
            AssertGasConsumed(1048860);
            Assert.IsFalse(Contract.NullProperty(""));
            AssertGasConsumed(1049190);
            Assert.IsTrue(Contract.NullProperty("123"));
            AssertGasConsumed(1049190);
        }

        [TestMethod]
        public void NullPropertyGT()
        {
            Assert.IsFalse(Contract.NullPropertyGT(null));
            AssertGasConsumed(1048140);
            Assert.IsFalse(Contract.NullPropertyGT(""));
            AssertGasConsumed(1048470);
            Assert.IsTrue(Contract.NullPropertyGT("123"));
            AssertGasConsumed(1048470);
        }

        [TestMethod]
        public void NullPropertyLT()
        {
            Assert.IsFalse(Contract.NullPropertyLT(null));
            AssertGasConsumed(1048140);
            Assert.IsFalse(Contract.NullPropertyLT(""));
            AssertGasConsumed(1048470);
            Assert.IsFalse(Contract.NullPropertyLT("123"));
            AssertGasConsumed(1048470);
        }

        [TestMethod]
        public void NullPropertyGE()
        {
            Assert.IsFalse(Contract.NullPropertyGE(null));
            AssertGasConsumed(1048140);
            Assert.IsTrue(Contract.NullPropertyGE(""));
            AssertGasConsumed(1048470);
            Assert.IsTrue(Contract.NullPropertyGE("123"));
            AssertGasConsumed(1048470);
        }

        [TestMethod]
        public void NullPropertyLE()
        {
            Assert.IsFalse(Contract.NullPropertyLE(null));
            AssertGasConsumed(1048140);
            Assert.IsTrue(Contract.NullPropertyLE(""));
            AssertGasConsumed(1048470);
            Assert.IsFalse(Contract.NullPropertyLE("123"));
            AssertGasConsumed(1048470);
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            {
                var data = (VM.Types.ByteString)Contract.NullCoalescing("a123b")!;
                AssertGasConsumed(1109700);
                Assert.AreEqual("12", System.Text.Encoding.ASCII.GetString(data.GetSpan()));
            }
            // null->null
            {
                Assert.IsNull(Contract.NullCoalescing(null));
                AssertGasConsumed(1047990);
            }
        }

        [TestMethod]
        public void NullCollation()
        {
            // call nullCollation(string code)
            // return code ?? "linux"

            // nes->nes
            {
                Assert.AreEqual("nes", Contract.NullCollation("nes"));
                AssertGasConsumed(1048200);
            }

            // null->linux
            {
                Assert.AreEqual("linux", Contract.NullCollation(null));
                AssertGasConsumed(1048290);
            }
        }

        [TestMethod]
        public void NullCollationAndCollation()
        {
            Assert.AreEqual(new BigInteger(123), ((VM.Types.ByteString)Contract.NullCollationAndCollation("nes")!).GetInteger());
            AssertGasConsumed(2523540);
        }

        [TestMethod]
        public void NullCollationAndCollation2()
        {
            Assert.AreEqual("111", ((VM.Types.ByteString)Contract.NullCollationAndCollation2("nes")!).GetString());
            AssertGasConsumed(3615120);
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            Assert.IsTrue(Contract.EqualNullA(null));
            AssertGasConsumed(1048680);

            // False

            Assert.IsFalse(Contract.EqualNullA(1));
            AssertGasConsumed(1048680);

            // True

            Assert.IsTrue(Contract.EqualNullB(null));
            AssertGasConsumed(1047750);

            // False

            Assert.IsFalse(Contract.EqualNullB(1));
            AssertGasConsumed(1047750);
        }

        [TestMethod]
        public void EqualNotNull()
        {
            // True

            Assert.IsFalse(Contract.EqualNotNullA(null));
            AssertGasConsumed(1048680);

            // False

            Assert.IsTrue(Contract.EqualNotNullA(1));
            AssertGasConsumed(1048680);

            // True

            Assert.IsFalse(Contract.EqualNotNullB(null));
            AssertGasConsumed(1047870);

            // False

            Assert.IsTrue(Contract.EqualNotNullB(1));
            AssertGasConsumed(1047870);
        }

        [TestMethod]
        public void NullTypeTest()
        {
            Contract.NullType(); // no error
            AssertGasConsumed(987000);
        }

        [TestMethod]
        public void NullCoalescingAssignment()
        {
            Contract.NullCoalescingAssignment(null);
            AssertGasConsumed(2867310);
        }

        [TestMethod]
        public void StaticNullableCoalesceAssignment()
        {
            Contract.StaticNullableCoalesceAssignment();
            AssertGasConsumed(993870);
        }
    }
}
