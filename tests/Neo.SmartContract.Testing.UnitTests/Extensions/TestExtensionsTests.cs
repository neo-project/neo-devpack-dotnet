using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.TestEngine.UnitTests.Extensions
{
    [TestClass]
    public class TestExtensionsTests
    {
        [TestMethod]
        public void TestConvertEnum()
        {
            StackItem stackItem = new Integer((int)VMState.FAULT);

            Assert.AreEqual(VMState.FAULT, (VMState)stackItem.ConvertTo(typeof(VMState)));
        }
    }
}
