using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_OnDeployment
    {
        [TestMethod]
        public void Test_OnDeployment1()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_OnDeployment1.cs");

            Assert.AreEqual(1, testengine.Manifest.Abi.Methods.Length);
            Assert.AreEqual(testengine.Manifest.Abi.Methods[0].Name, "_deploy");
            Assert.AreEqual(testengine.Manifest.Abi.Methods[0].Offset, 0);
            Assert.AreEqual(testengine.Manifest.Abi.Methods[0].ReturnType, SmartContract.ContractParameterType.Void);

            var args = testengine.Manifest.Abi.Methods[0].Parameters;

            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(args[0].Name, "data");
            Assert.AreEqual(args[0].Type, SmartContract.ContractParameterType.Any);
            Assert.AreEqual(args[1].Name, "update");
            Assert.AreEqual(args[1].Type, SmartContract.ContractParameterType.Boolean);
        }

        [TestMethod]
        public void Test_OnDeployment2()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_OnDeployment2.cs");

            Assert.AreEqual(1, testengine.Manifest.Abi.Methods.Length);
            Assert.AreEqual(testengine.Manifest.Abi.Methods[0].Name, "_deploy");
            Assert.AreEqual(testengine.Manifest.Abi.Methods[0].Offset, 0);
            Assert.AreEqual(testengine.Manifest.Abi.Methods[0].ReturnType, SmartContract.ContractParameterType.Void);

            var args = testengine.Manifest.Abi.Methods[0].Parameters;

            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(args[0].Name, "data");
            Assert.AreEqual(args[0].Type, SmartContract.ContractParameterType.Any);
            Assert.AreEqual(args[1].Name, "update");
            Assert.AreEqual(args[1].Type, SmartContract.ContractParameterType.Boolean);
        }
    }
}
