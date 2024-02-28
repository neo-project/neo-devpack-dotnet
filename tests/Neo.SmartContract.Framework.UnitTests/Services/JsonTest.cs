using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class JsonTest : TestBase<Contract_Json>
    {
        public JsonTest() : base(Contract_Json.Nef, Contract_Json.Manifest) { }

        [TestMethod]
        public void Test_SerializeDeserialize()
        {
            // Empty Serialize

            Assert.AreEqual("null", Contract.Serialize());

            // Empty Serialize

            Assert.ThrowsException<TargetInvocationException>(() => Contract.Deserialize(null));

            // Serialize

            Assert.AreEqual("[null,true,\"asd\"]", Contract.Serialize(new object?[] { null, true, "asd" }));

            // Deserialize

            var item = Contract.Deserialize("[null,true,\"asd\"]");
            Assert.IsInstanceOfType(item, typeof(Array));

            var entry = ((Array)item)[0];
            Assert.IsInstanceOfType(entry, typeof(Null));
            entry = ((Array)item)[1];
            Assert.IsInstanceOfType(entry, typeof(Boolean));
            Assert.AreEqual(true, entry.GetBoolean());
            entry = ((Array)item)[2];
            Assert.IsInstanceOfType(entry, typeof(ByteString));
            Assert.AreEqual("asd", entry.GetString());
        }
    }
}
