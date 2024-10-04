using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Pattern : DebugAndTestBase<Contract_Pattern>
    {
        [TestMethod]
        public void Between_Test()
        {
            Assert.AreEqual(true, Contract.Between(50));
            AssertGasConsumed(1047810);
            Assert.AreEqual(false, Contract.Between(1));
            AssertGasConsumed(1047510);
            Assert.AreEqual(false, Contract.Between(100));
            AssertGasConsumed(1047810);
            Assert.AreEqual(false, Contract.Between(200));
            AssertGasConsumed(1047810);
        }

        [TestMethod]
        public void Between2_Test()
        {
            Assert.AreEqual(true, Contract.Between2(50));
            Assert.AreEqual(false, Contract.Between2(1));
            Assert.AreEqual(false, Contract.Between2(100));
            Assert.AreEqual(false, Contract.Between2(200));
        }

        [TestMethod]
        public void Between3_Test()
        {
            Assert.AreEqual(true, Contract.Between3(50));
            Assert.AreEqual(false, Contract.Between3(1));
            Assert.AreEqual(false, Contract.Between3(100));
            Assert.AreEqual(false, Contract.Between3(200));
        }

        [TestMethod]
        public void RecursivePattern_Test()
        {
            Assert.AreEqual(true, Contract.TestRecursivePattern());
        }

        [TestMethod]
        public void TestTypePattern_Test()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TestTypePattern2(1));
            Assert.AreEqual(BigInteger.Zero, Contract.TestTypePattern2("1"));
            Assert.AreEqual(BigInteger.Zero, Contract.TestTypePattern2(new ByteString(new byte[] { 1 })));
            Assert.AreEqual(BigInteger.Zero, Contract.TestTypePattern2(new byte[] { 1 }));
            Assert.AreEqual(BigInteger.One, Contract.TestTypePattern2(true));

            // no errors
            Contract.TestTypePattern("1");
            Contract.TestTypePattern(1);
            Contract.TestTypePattern(true);
        }
    }
}
