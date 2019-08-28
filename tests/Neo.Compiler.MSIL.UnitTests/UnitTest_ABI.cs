using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.IO.Json;
using System.Linq;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_ABI
    {
        [TestMethod]
        public void Test_ABI()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Abi.cs");
            string expectABI = @"{""hash"":""0x77811b3127dea2df1a18230f91396fbcf8c648f4"",""methods"":[{""name"":""readOnlyTrue"",""parameters"":[],""returnType"":""Void""},{""name"":""readOnlyFalse1"",""parameters"":[],""returnType"":""Void""},{""name"":""readOnlyFalse2"",""parameters"":[],""returnType"":""Void""}],""readOnlyMethods"":[""readOnlyTrue""],""entryPoint"":{""name"":""Main"",""parameters"":[{""name"":""method"",""type"":""String""},{""name"":""args"",""type"":""Array""}],""returnType"":""Void""},""events"":[{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}]}";
            Assert.AreEqual(testengine.ScriptEntry.finialABI.ToString(), expectABI);

            // Check with the real class

            var abi = SmartContract.Manifest.ContractAbi.FromJson(JObject.Parse(expectABI));

            // Hash

            Assert.AreEqual("0x77811b3127dea2df1a18230f91396fbcf8c648f4", abi.Hash.ToString());

            // Entry Point

            Assert.AreEqual("Main", abi.EntryPoint.Name);
            Assert.AreEqual(2, abi.EntryPoint.Parameters.Length);
            Assert.AreEqual("method", abi.EntryPoint.Parameters[0].Name);
            Assert.AreEqual(SmartContract.ContractParameterType.String, abi.EntryPoint.Parameters[0].Type);
            Assert.AreEqual("args", abi.EntryPoint.Parameters[1].Name);
            Assert.AreEqual(SmartContract.ContractParameterType.Array, abi.EntryPoint.Parameters[1].Type);
            Assert.AreEqual(SmartContract.ContractParameterType.Void, abi.EntryPoint.ReturnType);

            // Methods

            Assert.AreEqual(3, abi.Methods.Length);

            Assert.AreEqual("readOnlyTrue", abi.Methods[0].Name);
            Assert.AreEqual(0, abi.Methods[0].Parameters.Length);
            Assert.AreEqual(SmartContract.ContractParameterType.Void, abi.Methods[0].ReturnType);

            Assert.AreEqual("readOnlyFalse1", abi.Methods[1].Name);
            Assert.AreEqual(0, abi.Methods[1].Parameters.Length);
            Assert.AreEqual(SmartContract.ContractParameterType.Void, abi.Methods[1].ReturnType);

            Assert.AreEqual("readOnlyFalse2", abi.Methods[2].Name);
            Assert.AreEqual(0, abi.Methods[2].Parameters.Length);
            Assert.AreEqual(SmartContract.ContractParameterType.Void, abi.Methods[2].ReturnType);

            // Read only methods

            //#TODO Require the last PR
            //CollectionAssert.AreEqual(new string[] { "readOnlyTrue" }, abi.ReadOnlyMethods);

            // Events

            Assert.AreEqual(1, abi.Events.Length);
            Assert.AreEqual("transfer", abi.Events[0].Name);
            Assert.AreEqual(3, abi.Events[0].Parameters.Length);
            Assert.AreEqual("arg1", abi.Events[0].Parameters[0].Name);
            Assert.AreEqual(SmartContract.ContractParameterType.ByteArray, abi.Events[0].Parameters[0].Type);
            Assert.AreEqual("arg2", abi.Events[0].Parameters[1].Name);
            Assert.AreEqual(SmartContract.ContractParameterType.ByteArray, abi.Events[0].Parameters[1].Type);
            Assert.AreEqual("arg3", abi.Events[0].Parameters[2].Name);
            Assert.AreEqual(SmartContract.ContractParameterType.Integer, abi.Events[0].Parameters[2].Type);
        }
    }
}
