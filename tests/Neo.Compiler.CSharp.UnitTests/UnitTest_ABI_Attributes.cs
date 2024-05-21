using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Testing;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Attributes
    {
        [TestMethod]
        public void TestAbiAttributes()
        {
            var permissions = new JArray(Contract_ABIAttributes.Manifest.Permissions.Select(p => p.ToJson()).ToArray()).ToString(false);
            Assert.AreEqual(@"[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4"",""methods"":[""a"",""b""]},{""contract"":""*"",""methods"":[""c""]}]", permissions);
            var trust = Contract_ABIAttributes.Manifest.Trusts.ToJson(p => p.ToJson());
            Assert.AreEqual(@"[""0x0a0b00ff00ff00ff00ff00ff00ff00ff00ff00a4""]", trust.ToString(false));
        }
    }
}
