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
        public void Test_ABI_Event()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Event.cs");
            var abi = testengine.scriptEntry.finialABI;

            string expecteventabi = @"{""hash"":""0x77811b3127dea2df1a18230f91396fbcf8c648f4"",""entryPoint"":""Main"",""methods"":[{""name"":""Main"",""parameters"":[{""name"":""method"",""type"":""String""},{""name"":""args"",""type"":""Array""}],""returnType"":""Void""},{""name"":""readOnlyTrue"",""parameters"":[],""returnType"":""Void""},{""name"":""readOnlyFalse1"",""parameters"":[],""returnType"":""Void""},{""name"":""readOnlyFalse2"",""parameters"":[],""returnType"":""Void""}],""readOnlyMethods"":[""readOnlyTrue""],""events"":[{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}]}";
            Assert.AreEqual(abi.ToString(), expecteventabi);
        }
    }
}
