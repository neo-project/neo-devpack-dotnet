extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using scfxHelper = scfx.Neo.SmartContract.Framework.Helper;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StackItemTypeTest
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(((byte)StackItemType.Buffer).ToString("x2"), scfxHelper.StackItemType_Buffer.Substring(2));
            Assert.AreEqual(((byte)StackItemType.Integer).ToString("x2"), scfxHelper.StackItemType_Integer.Substring(2));
        }
    }
}
