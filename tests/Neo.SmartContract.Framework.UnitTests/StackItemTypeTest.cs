using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FrameworkStackItemType = Neo.SmartContract.Framework.StackItemType;
using VMStackItemType = Neo.VM.Types.StackItemType;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StackItemTypeTest
    {
        [TestMethod]
        public void TestValues()
        {
            Assert.AreEqual(((byte)FrameworkStackItemType.Buffer).ToString("x2"), Helper.StackItemType_Buffer.Substring(2));
            Assert.AreEqual(((byte)FrameworkStackItemType.Integer).ToString("x2"), Helper.StackItemType_Integer.Substring(2));
        }

        [TestMethod]
        public void TestAllTypes()
        {
            // Names

            CollectionAssert.AreEqual
                (
                Enum.GetNames(typeof(VMStackItemType)),
                Enum.GetNames(typeof(FrameworkStackItemType))
                );

            // Values

            CollectionAssert.AreEqual
                (
                Enum.GetValues(typeof(VMStackItemType)).Cast<VMStackItemType>().Select(u => (byte)u).ToArray(),
                Enum.GetValues(typeof(FrameworkStackItemType)).Cast<FrameworkStackItemType>().Select(u => (byte)u).ToArray()
                );
        }
    }
}
