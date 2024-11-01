using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ByteString : DebugAndTestBase<Contract_ByteString>
    {
        [TestMethod]
        public void Test_Literal00ToFF()
        {
            Assert.AreEqual(Contract.Literal00ToFF()!.Length, 256);
        }

        [TestMethod]
        public void Test_LiteralWithOtherChar()
        {
            Assert.AreEqual(Contract.LiteralWithOtherChar()!.Length, Utility.StrictUTF8.GetByteCount("你好\x00\x80\xff"));
        }
    }
}
