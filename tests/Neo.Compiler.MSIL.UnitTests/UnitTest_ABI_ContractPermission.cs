using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_ContractPermission
    {
        [TestMethod]
        public void TestWildcardContractPermission()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_ABIContractPermission.cs", true, false);
            var manifest = JObject.Parse(buildScript.finalManifest);
            var permissions = manifest["permissions"].ToString(false);
            Assert.AreEqual(@"[{""contract"":""*"",""methods"":[""a"",""b""]},{""contract"":""0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4"",""methods"":[""a"",""b""]}]", permissions);
        }
    }
}
