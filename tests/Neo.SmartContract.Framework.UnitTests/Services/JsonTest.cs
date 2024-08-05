using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM.Types;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class JsonTest : DebugAndTestBase<Contract_Json>
    {
        [TestMethod]
        public void Test_SerializeDeserialize()
        {
            // Empty Serialize

            Assert.AreEqual("null", Contract.Serialize());

            // Empty Serialize

            var exception = Assert.ThrowsException<TestException>(() => Contract.Deserialize(null));
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);

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
