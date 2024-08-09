using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;
using Neo.SmartContract.Testing.Exceptions;

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
            Assert.AreEqual(1048140, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.IsNull(1));
            Assert.AreEqual(1048140, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void IfNull()
        {
            Assert.IsFalse(Contract.IfNull(null));
            Assert.AreEqual(1047120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullProperty()
        {
            Assert.ThrowsException<TestException>(() => Contract.NullProperty(null));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullProperty(""));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullProperty("123"));
            Assert.AreEqual(1048350, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyGT()
        {
            Assert.ThrowsException<TestException>(() => Contract.NullPropertyGT(null));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyGT(""));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGT("123"));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyLT()
        {
            Assert.ThrowsException<TestException>(() => Contract.NullPropertyLT(null));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLT(""));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLT("123"));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyGE()
        {
            Assert.ThrowsException<TestException>(() => Contract.NullPropertyGE(null));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGE(""));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGE("123"));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyLE()
        {
            Assert.ThrowsException<TestException>(() => Contract.NullPropertyLE(null));
            Assert.AreEqual(1047150, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyLE(""));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLE("123"));
            Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            {
                var data = (VM.Types.ByteString)Contract.NullCoalescing("a123b")!;
                Assert.AreEqual(1108860, Engine.FeeConsumed.Value);
                Assert.AreEqual("12", System.Text.Encoding.ASCII.GetString(data.GetSpan()));
            }
            // null->null
            {
                Assert.ThrowsException<TestException>(() => Contract.NullCoalescing(null));
                Assert.AreEqual(1108530, Engine.FeeConsumed.Value);
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
                Assert.AreEqual(1047540, Engine.FeeConsumed.Value);
            }

            // null->linux
            {
                Assert.AreEqual("linux", Contract.NullCollation(null));
                Assert.AreEqual(1047630, Engine.FeeConsumed.Value);
            }
        }

        [TestMethod]
        public void NullCollationAndCollation()
        {
            Assert.AreEqual(new BigInteger(123), ((VM.Types.ByteString)Contract.NullCollationAndCollation("nes")!).GetInteger());
            Assert.AreEqual(2522880, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullCollationAndCollation2()
        {
            Assert.AreEqual("111", ((VM.Types.ByteString)Contract.NullCollationAndCollation2("nes")!).GetString());
            Assert.AreEqual(3614460, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            Assert.IsTrue(Contract.EqualNullA(null));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.EqualNullA(1));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);

            // True

            Assert.IsTrue(Contract.EqualNullB(null));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.EqualNullB(1));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void EqualNotNull()
        {
            // True

            Assert.IsFalse(Contract.EqualNotNullA(null));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);

            // False

            Assert.IsTrue(Contract.EqualNotNullA(1));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);

            // True

            Assert.IsFalse(Contract.EqualNotNullB(null));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);

            // False

            Assert.IsTrue(Contract.EqualNotNullB(1));
            Assert.AreEqual(1048020, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullTypeTest()
        {
            Contract.NullType(); // no error
            Assert.AreEqual(1003380, Engine.FeeConsumed.Value);
        }
    }
}
