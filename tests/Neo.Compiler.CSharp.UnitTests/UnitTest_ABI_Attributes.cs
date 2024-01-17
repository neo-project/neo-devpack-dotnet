using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Attributes
    {
        [TestMethod]
        public void TestAbiAttributes()
        {
            var testEngine = new TestEngine();
            testEngine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_ABIAttributes.cs");
            var permissions = new JArray(testEngine.Manifest.Permissions.Select(p => p.ToJson()).ToArray()).ToString(false);
            Assert.AreEqual(@"[{""contract"":""0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4"",""methods"":[""a"",""b""]},{""contract"":""*"",""methods"":[""c""]}]", permissions);
            var trust = testEngine.Manifest.Trusts.ToJson(p => p.ToJson());
            Assert.AreEqual(@"[""0x0a0b00ff00ff00ff00ff00ff00ff00ff00ff00a4""]", trust.ToString(false));
        }
    }
}
