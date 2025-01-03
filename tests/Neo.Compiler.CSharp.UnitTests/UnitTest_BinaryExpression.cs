using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BinaryExpression : DebugAndTestBase<Contract_BinaryExpression>
    {
        [TestMethod]
        public void Test_BinaryIs()
        {
            Contract.BinaryIs();
            AssertGasConsumed(986850);
        }

        [TestMethod]
        public void Test_BinaryAs()
        {
            Contract.BinaryAs();
            AssertGasConsumed(987720);
        }
    }
}
