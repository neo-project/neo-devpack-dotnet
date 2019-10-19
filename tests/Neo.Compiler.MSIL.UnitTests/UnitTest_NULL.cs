using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_NULL
    {
        [TestMethod]
        public void IsNull()
        {
            // True

            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_NULL.cs");
            var result = testengine.ExecuteTestCaseStandard("IsNull", StackItem.Null);
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.GetBoolean());

            // False

            testengine.InvocationStack.Clear();
            result = testengine.ExecuteTestCaseStandard("IsNull", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.GetBoolean());
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_NULL.cs");
            var result = testengine.ExecuteTestCaseStandard("EqualNull", StackItem.Null);
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.GetBoolean());

            // False

            testengine.InvocationStack.Clear();
            result = testengine.ExecuteTestCaseStandard("EqualNull", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.GetBoolean());
        }
    }
}
