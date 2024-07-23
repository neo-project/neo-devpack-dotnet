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
            Assert.AreEqual(1002124000, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.IsNull(1));
            Assert.AreEqual(1003172200, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void IfNull()
        {
            Assert.IsFalse(Contract.IfNull(null));
            Assert.AreEqual(1002122980, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullProperty()
        {
            Assert.IsTrue(Contract.NullProperty(null));
            Assert.AreEqual(1002124060, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullProperty(""));
            Assert.AreEqual(1003172650, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullProperty("123"));
            Assert.AreEqual(1004221240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyGT()
        {
            Assert.IsFalse(Contract.NullPropertyGT(null));
            Assert.AreEqual(1002123340, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyGT(""));
            Assert.AreEqual(1003171210, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGT("123"));
            Assert.AreEqual(1004219080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyLT()
        {
            Assert.IsFalse(Contract.NullPropertyLT(null));
            Assert.AreEqual(1002123340, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLT(""));
            Assert.AreEqual(1003171210, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLT("123"));
            Assert.AreEqual(1004219080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyGE()
        {
            Assert.IsFalse(Contract.NullPropertyGE(null));
            Assert.AreEqual(1002123340, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGE(""));
            Assert.AreEqual(1003171210, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyGE("123"));
            Assert.AreEqual(1004219080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullPropertyLE()
        {
            Assert.IsFalse(Contract.NullPropertyLE(null));
            Assert.AreEqual(1002123340, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.NullPropertyLE(""));
            Assert.AreEqual(1003171210, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.NullPropertyLE("123"));
            Assert.AreEqual(1004219080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            {
                var data = (VM.Types.Buffer)Contract.NullCoalescing("a123b")!;
                Assert.AreEqual(1002184900, Engine.FeeConsumed.Value);
                Assert.AreEqual("12", System.Text.Encoding.ASCII.GetString(data.GetSpan()));
            }
            // null->null
            {
                Assert.IsNull(Contract.NullCoalescing(null));
                Assert.AreEqual(1003232290, Engine.FeeConsumed.Value);
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
                Assert.AreEqual(1002123400, Engine.FeeConsumed.Value);
            }

            // null->linux
            {
                Assert.AreEqual("linux", Contract.NullCollation(null));
                Assert.AreEqual(1003171090, Engine.FeeConsumed.Value);
            }
        }

        [TestMethod]
        public void NullCollationAndCollation()
        {
            Assert.AreEqual(new BigInteger(123), ((VM.Types.ByteString)Contract.NullCollationAndCollation("nes")!).GetInteger());
            Assert.AreEqual(1003598740, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullCollationAndCollation2()
        {
            Assert.AreEqual("111", ((VM.Types.ByteString)Contract.NullCollationAndCollation2("nes")!).GetString());
            Assert.AreEqual(1004690320, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            Assert.IsTrue(Contract.EqualNullA(null));
            Assert.AreEqual(1002123880, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.EqualNullA(1));
            Assert.AreEqual(1003171960, Engine.FeeConsumed.Value);

            // True

            Assert.IsTrue(Contract.EqualNullB(null));
            Assert.AreEqual(1004220040, Engine.FeeConsumed.Value);

            // False

            Assert.IsFalse(Contract.EqualNullB(1));
            Assert.AreEqual(1005268120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void EqualNotNull()
        {
            // True

            Assert.IsFalse(Contract.EqualNotNullA(null));
            Assert.AreEqual(1002123880, Engine.FeeConsumed.Value);

            // False

            Assert.IsTrue(Contract.EqualNotNullA(1));
            Assert.AreEqual(1003171960, Engine.FeeConsumed.Value);

            // True

            Assert.IsFalse(Contract.EqualNotNullB(null));
            Assert.AreEqual(1004220040, Engine.FeeConsumed.Value);

            // False

            Assert.IsTrue(Contract.EqualNotNullB(1));
            Assert.AreEqual(1005268120, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void NullTypeTest()
        {
            Contract.NullType(); // no error
            Assert.AreEqual(1002062140, Engine.FeeConsumed.Value);
        }
    }
}
