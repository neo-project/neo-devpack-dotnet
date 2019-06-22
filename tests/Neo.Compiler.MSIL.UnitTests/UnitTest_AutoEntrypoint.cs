using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_AutoEntryPoint
    {
        [TestMethod]
        public void Test_AutoEntry()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_autoentrypoint.cs");

            testengine.scriptEntry.DumpAVM();

            StackItem[] _params = new StackItem[] { "call01", new StackItem[0] };
            var result = testengine.ExecuteTestCase(_params);
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_AutoEntry_private()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_autoentrypoint.cs");

            testengine.scriptEntry.DumpAVM();

            StackItem[] _params = new StackItem[] { "privateMethod", new StackItem[0] };
            var result = testengine.ExecuteTestCase(_params);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_AutoEntry_call02()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_autoentrypoint.cs");

            testengine.scriptEntry.DumpAVM();

            StackItem[] _params = new StackItem[] { "call02", new StackItem[] { "hello", 33 } };
            var result = testengine.ExecuteTestCase(_params);
            StackItem wantresult = new byte[0];
            var bequal = wantresult.Equals(result.Pop());

            //if your function return is void,when auto generate a entry point,it always return new byte[0];
            // object Main(string,object[]) must be return sth.
            Assert.IsTrue(bequal);
        }
    }
}