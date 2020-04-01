using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Buffer
    {
        [TestMethod]
        public void Test_Buffer()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BufferReverse.cs");
            var result = testengine.GetMethod("testType").Run();

            StackItem reverseArray = new byte[] { 0x03, 0x02, 0x01 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }
    }
}
