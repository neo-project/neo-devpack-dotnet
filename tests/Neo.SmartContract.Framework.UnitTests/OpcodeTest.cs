using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FrameworkOpCode = Neo.SmartContract.Framework.OpCode;
using VMOpCode = Neo.VM.OpCode;

namespace Neo.SmartContract.Framework.UnitTests
{
    // TODO: TRY = 0x3B, TRY_L = 0x3C, ENDT = 0x3D, ENDC = 0x3E, ENDF = 0x3F. These opcodes need other PR to open.
    [TestClass]
    public class OpcodeTest
    {
        //[TestMethod]
        //public void TestAllOpcodes()
        //{
        //    // Names
        //    int i = Enum.GetNames(typeof(VMOpCode)).Length;
        //    int j = Enum.GetNames(typeof(FrameworkOpCode)).Length;
        //    Assert.AreEqual(i, j);

        //    CollectionAssert.AreEqual
        //        (
        //        Enum.GetNames(typeof(VMOpCode)),
        //        Enum.GetNames(typeof(FrameworkOpCode))
        //        );

        //    // Values

        //    CollectionAssert.AreEqual
        //        (
        //        Enum.GetValues(typeof(VMOpCode)).Cast<VMOpCode>().Select(u => (byte)u).ToArray(),
        //        Enum.GetValues(typeof(FrameworkOpCode)).Cast<FrameworkOpCode>().Select(u => (byte)u).ToArray()
        //        );
        //}
    }
}
