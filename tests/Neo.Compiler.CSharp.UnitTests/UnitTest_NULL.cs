using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NULL : TestBase<Contract_NULL>
    {
        public UnitTest_NULL() : base(Contract_NULL.Nef, Contract_NULL.Manifest) { }

        [TestMethod]
        public void IsNull()
        {
            // True

            Assert.IsTrue(Contract.IsNull(null));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.IsNull(1));
            Assert.AreEqual(1048200, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void IfNull()
        {
            Assert.IsFalse(Contract.IfNull(null));
            Assert.AreEqual(1047180, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullProperty()
        {
            Assert.IsTrue(Contract.NullProperty(null));
            Assert.AreEqual(1048260, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullProperty(""));
            Assert.AreEqual(1048590, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullProperty("123"));
            Assert.AreEqual(1048590, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyGT()
        {
            Assert.IsFalse(Contract.NullPropertyGT(null));
            Assert.AreEqual(1047540, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyGT(""));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGT("123"));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyLT()
        {
            Assert.IsFalse(Contract.NullPropertyLT(null));
            Assert.AreEqual(1047540, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLT(""));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLT("123"));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyGE()
        {
            Assert.IsFalse(Contract.NullPropertyGE(null));
            Assert.AreEqual(1047540, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGE(""));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGE("123"));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyLE()
        {
            Assert.IsFalse(Contract.NullPropertyLE(null));
            Assert.AreEqual(1047540, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyLE(""));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLE("123"));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            {
                var data = (VM.Types.ByteString)Contract.NullCoalescing("a123b")!;
                Assert.AreEqual(1109100, Engine.FeeConsumed.Value);
                Assert.AreEqual("12", System.Text.Encoding.ASCII.GetString(data.GetSpan()));
            }
            // null->null
            {
                Assert.IsNull(Contract.NullCoalescing(null));
                Assert.AreEqual(1047390, Engine.FeeConsumed.Value);
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
                Assert.AreEqual(1047600, Engine.FeeConsumed.Value);
            }

            // null->linux
            {
                Assert.AreEqual("linux", Contract.NullCollation(null));
                Assert.AreEqual(1047690, Engine.FeeConsumed.Value);
            }
        }

        [TestMethod]
        public void NullCollationAndCollation()
        {
            Assert.AreEqual(new BigInteger(123), ((VM.Types.ByteString)Contract.NullCollationAndCollation("nes")!).GetInteger());
            Assert.AreEqual(2522940, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullCollationAndCollation2()
        {
            Assert.AreEqual("111", ((VM.Types.ByteString)Contract.NullCollationAndCollation2("nes")!).GetString());
            Assert.AreEqual(3614520, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            Assert.IsTrue(Contract.EqualNullA(null));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.EqualNullA(1));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);

            // True

            Assert.IsTrue(Contract.EqualNullB(null));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.EqualNullB(1));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void EqualNotNull()
        {
            // True

            Assert.IsFalse(Contract.EqualNotNullA(null));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);

            // False

            Assert.IsTrue(Contract.EqualNotNullA(1));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);

            // True

            Assert.IsFalse(Contract.EqualNotNullB(null));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);

            // False

            Assert.IsTrue(Contract.EqualNotNullB(1));
            Assert.AreEqual(1048080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullTypeTest()
        {
            Contract.NullType(); // no error
            Assert.AreEqual(986340, Engine.FeeConsumed.Value);
        }
    }
}
