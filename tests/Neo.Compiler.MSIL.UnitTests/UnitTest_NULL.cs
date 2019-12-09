using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_NULL
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_NULL.cs");
        }

        [TestMethod]
        public void IsNull()
        {
            // True

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("IsNull", StackItem.Null);
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("IsNull", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());
        }

        [TestMethod]
        public void EqualNull()
        {
            // True

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("EqualNullA", StackItem.Null);
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("EqualNullA", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());

            // True

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("EqualNullB", StackItem.Null);
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("EqualNullB", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());
        }
    }
}
