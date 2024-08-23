using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke : DebugAndTestBase<Contract_InvokeCsNef>
    {
        [TestMethod]
        public void Test_Return_Integer()
        {
            Assert.AreEqual(new BigInteger(42), Contract.ReturnInteger());
            AssertGasConsumed(984060);
        }

        [TestMethod]
        public void Test_Return_String()
        {
            Assert.AreEqual("hello world", Contract.ReturnString());
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_Main()
        {
            Assert.AreEqual(new BigInteger(22), Contract.TestMain());
            AssertGasConsumed(984060);
        }
    }
}
