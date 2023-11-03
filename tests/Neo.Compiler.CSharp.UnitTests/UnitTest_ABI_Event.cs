using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Event
    {
        [TestMethod]
        public void Test_Good()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Event.cs");
            var abi = testengine.Manifest.Abi;
            var events = abi.Events[0].ToJson().ToString(false);

            string expecteventabi = @"{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}";
            Assert.AreEqual(expecteventabi, events);
        }

        [TestMethod]
        public void Test_Wrong()
        {
            var testengine = new TestEngine();
            Assert.IsFalse(testengine.AddEntryScript("./TestClasses/Contract_WrongEvent.cs").Success);
        }
    }
}
