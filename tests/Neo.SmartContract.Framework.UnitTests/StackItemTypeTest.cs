extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using scfxStackItemType = scfx.Neo.SmartContract.Framework.StackItemType;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StackItemTypeTest
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(((byte)VM.Types.StackItemType.Buffer).ToString("x2"), scfxStackItemType.Buffer.Substring(2));
            Assert.AreEqual(((byte)VM.Types.StackItemType.Integer).ToString("x2"), scfxStackItemType.Integer.Substring(2));
        }
    }
}
