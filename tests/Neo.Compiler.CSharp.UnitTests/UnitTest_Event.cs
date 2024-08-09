using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Event() : DebugAndTestBase<Contract_Event>
    {
        [TestMethod]
        public void Test_Good()
        {
            var abi = Contract_Event.Manifest.Abi;
            var events = abi.Events[0].ToJson().ToString(false);

            string expecteventabi = @"{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}";
            Assert.AreEqual(expecteventabi, events);
        }

        [TestMethod]
        public void TestEvent()
        {
            var flag = false;

            Contract.OnTransfer += (a, b, c) =>
            {
                CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, a);
                CollectionAssert.AreEqual(new byte[] { 4, 5, 6 }, b);
                Assert.AreEqual(7, c);
                flag = true;
            };

            Contract.Test();
            Assert.IsTrue(flag);
        }
    }
}
