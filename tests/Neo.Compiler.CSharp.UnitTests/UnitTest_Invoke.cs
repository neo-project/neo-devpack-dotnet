using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Invoke : TestBase<Contract_InvokeCsNef>
    {
        [TestMethod]
        public void Test_Return_Integer()
        {
            Assert.AreEqual(new BigInteger(42), Contract.ReturnInteger());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Return_String()
        {
            Assert.AreEqual("hello world", Contract.ReturnString());
            Assert.AreEqual(984270, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Main()
        {
            Assert.AreEqual(new BigInteger(22), Contract.TestMain());
            Assert.AreEqual(984060, Engine.FeeConsumed.Value);
        }
    }
}
