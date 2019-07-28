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

            testengine.scriptEntry.DumpNEF();

            var result = testengine.GetMethod("call01").Run();//new test method01
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_AutoEntry_private()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_autoentrypoint.cs");

            testengine.scriptEntry.DumpNEF();
            StackItem[] _params = new StackItem[] { "privateMethod", new StackItem[0] };
            var result = testengine.ExecuteTestCase(_params);//new test method02

            bool hadFault = (testengine.State & VMState.FAULT) > 0;
            Assert.AreEqual(0, result.Count);//because no methodname had found, it do not return anything.
            Assert.IsTrue(hadFault);///because no methodname had found,vm state=fault.
        }

        [TestMethod]
        public void Test_AutoEntry_call02()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_autoentrypoint.cs");

            testengine.scriptEntry.DumpNEF();

            var result = testengine.ExecuteTestCaseStandard("call02", "hello", 33);//old test method
            StackItem wantresult = new byte[0];
            var bequal = wantresult.Equals(result.Pop());

            //if your function return is void,when auto generate a entry point,it always return new byte[0];
            // object Main(string,object[]) must be return sth.
            Assert.IsTrue(bequal);
        }
    }
}