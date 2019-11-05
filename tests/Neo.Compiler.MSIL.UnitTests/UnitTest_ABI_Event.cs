using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_ABI_Event
    {
        [TestMethod]
        public void Test_Good()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Event.cs");
            var abi = testengine.scriptEntry.finialABI;
            Console.WriteLine("abi=" + abi.ToString());
            var events = abi["events"].AsList()[0].ToString();
            Console.WriteLine("event abi info =" + events);

            string expecteventabi = @"{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}";
            Assert.AreEqual(events, expecteventabi);
        }

        [TestMethod]
        public void Test_Wrong()
        {
            var testengine = new TestEngine();
            Assert.ThrowsException<NotSupportedException>(() => testengine.AddEntryScript("./TestClasses/Contract_WrongEvent.cs"));
        }
    }
}
