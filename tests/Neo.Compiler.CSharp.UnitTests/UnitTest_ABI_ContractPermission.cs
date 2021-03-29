using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_ContractPermission
    {
        [TestMethod]
        public void TestWildcardContractPermission()
        {
            var testEngine = new TestEngine();
            var buildScript = testEngine.Build("./TestClasses/Contract_ABIContractPermission.cs");
            var permissions = buildScript.manifest["permissions"].ToString(false);
            Assert.AreEqual(@"[{""contract"":""0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4"",""methods"":[""a"",""b""]},{""contract"":""*"",""methods"":[""c""]}]", permissions);
        }
    }
}
