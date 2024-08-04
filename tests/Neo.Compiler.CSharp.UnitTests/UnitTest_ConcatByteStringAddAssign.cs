using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ConcatByteStringAddAssign : TestBase2<Contract_ConcatByteStringAddAssign>
    {
        public UnitTest_ConcatByteStringAddAssign() : base(Contract_ConcatByteStringAddAssign.Nef, Contract_ConcatByteStringAddAssign.Manifest) { }

        [TestMethod]
        public void Test_ByteStringAdd()
        {
            Assert.AreEqual("abc", Encoding.ASCII.GetString(Contract.ByteStringAddAssign(Encoding.ASCII.GetBytes("a"), Encoding.ASCII.GetBytes("b"), "c")!));
            Assert.AreEqual(1970520, Engine.FeeConsumed.Value);
        }
    }
}
