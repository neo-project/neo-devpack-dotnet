using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_TypeConvert
    {
        [TestMethod]
        public void UnitTest_TestTypeConvert()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TypeConvert.cs");
            var result = testengine.ExecuteTestCaseStandard("testType");

            //test 0,1,2
            Assert.IsTrue(result.TryPop(out Array arr));
            Assert.IsTrue(arr[0].Type == StackItemType.Integer);
            Assert.IsTrue(arr[1].Type == StackItemType.ByteArray);
            Assert.IsTrue((arr[0] as PrimitiveType).ToBigInteger() == (arr[1] as PrimitiveType).ToBigInteger());

            Assert.IsTrue(arr[2].Type == StackItemType.Integer);
            Assert.IsTrue(arr[3].Type == StackItemType.ByteArray);
            Assert.IsTrue((arr[2] as PrimitiveType).ToBigInteger() == (arr[3] as PrimitiveType).ToBigInteger());

            Assert.IsTrue(arr[4].Type == StackItemType.ByteArray);
            Assert.IsTrue(arr[5].Type == StackItemType.Integer);
            Assert.IsTrue((arr[4] as PrimitiveType).ToBigInteger() == (arr[5] as PrimitiveType).ToBigInteger());

            Assert.IsTrue(arr[6].Type == StackItemType.ByteArray);
            Assert.IsTrue(arr[7].Type == StackItemType.Integer);
            Assert.IsTrue((arr[6] as PrimitiveType).ToBigInteger() == (arr[7] as PrimitiveType).ToBigInteger());
        }
    }
}
