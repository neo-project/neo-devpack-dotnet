using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_AppCall
    {
        [TestMethod]
        public void Test_Appcall()
        {
            var hash = UInt160.Parse("0102030405060708090A0102030405060708090A");
            var testengine = new TestEngine();
            testengine.Snapshot.Contracts.Add(hash, new Ledger.ContractState()
            {
                //Manifest = new SmartContract.Manifest.ContractManifest(),
                Script = testengine.Build("./TestClasses/Contract1.cs").finalNEF,
                Manifest = ContractManifest.FromJson(JObject.Parse(testengine.Build("./TestClasses/Contract1.cs").finalManifest)),
            });

            //will appcall 0102030405060708090A0102030405060708090A
            testengine.AddEntryScript("./TestClasses/Contract_appcall.cs");

            var result = testengine.GetMethod("testAppCall").Run().ConvertTo(StackItemType.ByteString);
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }
    }
}
