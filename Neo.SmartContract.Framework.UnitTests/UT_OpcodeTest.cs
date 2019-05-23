using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class UT_OpcodeTest
    {
        [TestMethod]
        public void TestAllOpcodes()
        {
            // Names

            CollectionAssert.AreEqual
                (
                Enum.GetNames(typeof(Neo.VM.OpCode)),
                Enum.GetNames(typeof(Neo.SmartContract.Framework.OpCode))
                );

            // Values

            CollectionAssert.AreEqual
                (
                Enum.GetValues(typeof(Neo.VM.OpCode)).Cast<Neo.VM.OpCode>().Select(u => (byte)u).ToArray(),
                Enum.GetValues(typeof(Neo.SmartContract.Framework.OpCode)).Cast<Neo.SmartContract.Framework.OpCode>().Select(u => (byte)u).ToArray()
                );
        }
    }
}