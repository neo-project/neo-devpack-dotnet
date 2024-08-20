using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Out : DebugAndTestBase<Contract_Out>
    {
        [TestMethod]
        public void Test_BasicOut()
        {
            Assert.AreEqual(42, Contract.TestOutVar());
            Assert.AreEqual(42, Contract.TestExistingVar());
        }

        [TestMethod]
        public void Test_OutInLoop()
        {
            Assert.AreEqual(210, Contract.TestOutInLoop()); // 42 * 5
        }

        [TestMethod]
        public void Test_OutConditional()
        {
            Assert.AreEqual("42", Contract.TestOutConditional(true));
            Assert.AreEqual("Hello", Contract.TestOutConditional(false));
        }

        [TestMethod]
        public void Test_OutSwitch()
        {
            Assert.AreEqual(42, Contract.TestOutSwitch(1));
            Assert.AreEqual(10, Contract.TestOutSwitch(2));
            Assert.AreEqual(-1, Contract.TestOutSwitch(3));
        }

        [TestMethod]
        public void Test_OutDiscard()
        {
            // This test is mainly to ensure that the method compiles and runs without error
            // Since we're discarding values, we can't really assert on them
            Contract.TestOutDiscard();
        }
    }
}
