using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_DirectInit : DebugAndTestBase<Contract_DirectInit>
    {
        [TestMethod]
        public void Test_GetUInt160()
        {
            Assert.AreEqual(Contract.TestGetUInt160()?.ToString(), "0x71a87191aef3fcf5e4441d791ded67ebab1aee7e");
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_GetECPoint()
        {
            Assert.AreEqual(Contract.TestGetECPoint()?.ToString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_GetUInt256()
        {
            Assert.AreEqual(Contract.TestGetUInt256()?.ToString(), "0x25898c9489b9c7f07adab10f995b3e492a23dbd79ae24f1a91c24e107986cfed");
            AssertGasConsumed(984270);
        }

        [TestMethod]
        public void Test_GetString()
        {
            Assert.AreEqual(Contract.TestGetString(), "hello world");
            AssertGasConsumed(984270);
        }
    }
}
