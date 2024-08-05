using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Testing;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ABI_Attributes3() : DebugAndTestBase<Contract_ABIAttributes3>
    {
        [TestMethod]
        public void TestAbiAttributes()
        {
            var permissions = new JArray(Contract_ABIAttributes3.Manifest.Permissions.Select(p => p.ToJson()).ToArray()).ToString(false);
            Assert.AreEqual(@"[{""contract"":""0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4"",""methods"":""*""}]", permissions);
            var trust = Contract_ABIAttributes3.Manifest.Trusts.ToJson(p => p.ToJson());
            Assert.AreEqual(@"[]", trust.ToString(false));
        }

        [TestMethod]
        public void MethodTest()
        {
            Assert.AreEqual(0, Contract.Test());
        }
    }
}
