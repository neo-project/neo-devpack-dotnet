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
            Assert.AreEqual(1493550, fee.Value);
            Assert.IsNotNull(result);

            Assert.AreEqual(cs.A, (BigInteger)result[0]);
            Assert.AreEqual(cs.B, (BigInteger)result[1]);
        }
    }
}
