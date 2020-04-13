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
            Assert.AreEqual(((byte)StackItemType.Buffer).ToString("x2"), Helper.StackItemType_Buffer.Substring(2));
            Assert.AreEqual(((byte)StackItemType.Integer).ToString("x2"), Helper.StackItemType_Integer.Substring(2));
        }
    }
}
