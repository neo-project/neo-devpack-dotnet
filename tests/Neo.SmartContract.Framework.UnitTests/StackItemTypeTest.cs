using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StackItemTypeTest
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(((byte)VM.Types.StackItemType.Buffer).ToString("x2"), StackItemType.Buffer.Substring(2));
            Assert.AreEqual(((byte)VM.Types.StackItemType.Integer).ToString("x2"), StackItemType.Integer.Substring(2));
        }
    }
}
