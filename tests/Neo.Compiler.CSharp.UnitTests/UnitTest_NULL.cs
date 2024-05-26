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

            // False

            Assert.IsFalse(Contract.IsNull(1));
        }

        [TestMethod]
        public void IfNull()
        {
            Assert.IsFalse(Contract.IfNull(null));
        }

        [TestMethod]
        public void NullProperty()
        {
            Assert.IsTrue(Contract.NullProperty(null));
            Assert.IsFalse(Contract.NullProperty(""));
            Assert.IsTrue(Contract.NullProperty("123"));
        }

        [TestMethod]
        public void NullPropertyGT()
        {
            Assert.IsFalse(Contract.NullPropertyGT(null));
            Assert.IsFalse(Contract.NullPropertyGT(""));
            Assert.IsTrue(Contract.NullPropertyGT("123"));
        }

        [TestMethod]
        public void NullPropertyLT()
        {
            Assert.IsFalse(Contract.NullPropertyLT(null));
            Assert.IsFalse(Contract.NullPropertyLT(""));
            Assert.IsFalse(Contract.NullPropertyLT("123"));
        }

        [TestMethod]
        public void NullPropertyGE()
        {
            Assert.IsFalse(Contract.NullPropertyGE(null));
            Assert.IsTrue(Contract.NullPropertyGE(""));
            Assert.IsTrue(Contract.NullPropertyGE("123"));
        }

        [TestMethod]
        public void NullPropertyLE()
        {
            Assert.IsFalse(Contract.NullPropertyLE(null));
            Assert.IsTrue(Contract.NullPropertyLE(""));
            Assert.IsFalse(Contract.NullPropertyLE("123"));
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            {
                var data = (VM.Types.Buffer)Contract.NullCoalescing("a123b")!;
                Assert.AreEqual("12", System.Text.Encoding.ASCII.GetString(data.GetSpan()));
            }
            // null->null
            {
                Assert.IsNull(Contract.NullCoalescing(null));
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
            }

            // null->linux
            {
                Assert.AreEqual("linux", Contract.NullCollation(null));
            }
        }

        [TestMethod]
        public void NullCollationAndCollation()
        {
            Assert.AreEqual(new BigInteger(123), ((VM.Types.ByteString)Contract.NullCollationAndCollation("nes")!).GetInteger());
        }

        [TestMethod]
        public void NullCollationAndCollation2()
        {
            Assert.AreEqual("111", ((VM.Types.ByteString)Contract.NullCollationAndCollation2("nes")!).GetString());
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            Assert.IsTrue(Contract.EqualNullA(null));

            // False

            Assert.IsFalse(Contract.EqualNullA(1));

            // True

            Assert.IsTrue(Contract.EqualNullB(null));

            // False

            Assert.IsFalse(Contract.EqualNullB(1));
        }

        [TestMethod]
        public void EqualNotNull()
        {
            // True

            Assert.IsFalse(Contract.EqualNotNullA(null));

            // False

            Assert.IsTrue(Contract.EqualNotNullA(1));

            // True

            Assert.IsFalse(Contract.EqualNotNullB(null));

            // False

            Assert.IsTrue(Contract.EqualNotNullB(1));
        }

        [TestMethod]
        public void NullTypeTest()
        {
            Contract.NullType(); // no error
        }
    }
}
