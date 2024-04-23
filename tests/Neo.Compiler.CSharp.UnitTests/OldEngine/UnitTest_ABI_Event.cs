using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests.OldEngine
{
    [TestClass]
    public class UnitTest_ABI_Event
    {
        [TestMethod]
        public void Test_Good()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Event.cs");
            var abi = testengine.Manifest.Abi;
            var events = abi.Events[0].ToJson().ToString(false);

            string expecteventabi = @"{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}";
            Assert.AreEqual(expecteventabi, events);
        }
    }
}
