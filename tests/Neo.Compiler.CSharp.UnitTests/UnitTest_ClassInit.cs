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

        public class IntInitClass
        {
            public int A;
            public BigInteger B;
        }

        [TestMethod]
        public void Test_InitInt()
        {
            var cs = new IntInit();
            var csClass = new IntInitClass();

            using var fee = Engine.CreateGasWatcher();
            var result = Contract.TestInitInt();
            AssertGasConsumed(1045560);
            Assert.IsNotNull(result);

            Assert.AreEqual(cs.A, (BigInteger)result[0]);
            Assert.AreEqual(cs.B, 0);
            Assert.AreEqual(csClass.B, 0);

            // Assert.AreEqual(cs.B, (BigInteger)result[1]);
        }

        [TestMethod]
        public void Test_InitializationExpression()
        {
            Contract.TestInitializationExpression();
            AssertGasConsumed(1275690);
        }
    }
}
