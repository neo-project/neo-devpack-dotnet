using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_OnDeployment
    {
        [TestMethod]
        public void Test_OnDeployment1()
        {
            Assert.AreEqual(1, Contract_OnDeployment1.Manifest.Abi.Methods.Length);
            Assert.AreEqual(Contract_OnDeployment1.Manifest.Abi.Methods[0].Name, "_deploy");
            Assert.AreEqual(Contract_OnDeployment1.Manifest.Abi.Methods[0].Offset, 0);
            Assert.AreEqual(Contract_OnDeployment1.Manifest.Abi.Methods[0].ReturnType, SmartContract.ContractParameterType.Void);

            var args = Contract_OnDeployment1.Manifest.Abi.Methods[0].Parameters;

            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(args[0].Name, "data");
            Assert.AreEqual(args[0].Type, SmartContract.ContractParameterType.Any);
            Assert.AreEqual(args[1].Name, "update");
            Assert.AreEqual(args[1].Type, SmartContract.ContractParameterType.Boolean);
        }

        [TestMethod]
        public void Test_OnDeployment2()
        {
            Assert.AreEqual(1, Contract_OnDeployment2.Manifest.Abi.Methods.Length);
            Assert.AreEqual(Contract_OnDeployment2.Manifest.Abi.Methods[0].Name, "_deploy");
            Assert.AreEqual(Contract_OnDeployment2.Manifest.Abi.Methods[0].Offset, 0);
            Assert.AreEqual(Contract_OnDeployment2.Manifest.Abi.Methods[0].ReturnType, SmartContract.ContractParameterType.Void);

            var args = Contract_OnDeployment2.Manifest.Abi.Methods[0].Parameters;

            Assert.AreEqual(2, args.Length);
            Assert.AreEqual(args[0].Name, "data");
            Assert.AreEqual(args[0].Type, SmartContract.ContractParameterType.Any);
            Assert.AreEqual(args[1].Name, "update");
            Assert.AreEqual(args[1].Type, SmartContract.ContractParameterType.Boolean);
        }
    }
}
