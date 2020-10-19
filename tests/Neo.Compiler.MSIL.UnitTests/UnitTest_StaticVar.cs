using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.TestingEngine;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVar
    {
        [TestMethod]
        public void Test_StaticVar()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("main");

            //test (1+5)*7 == 42
            StackItem wantresult = 42;
            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_StaticVarInit()
        {
            ByteString var1;
            ByteString var2;
            {
                var testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_StaticVarInit.cs");
                var result = testengine.ExecuteTestCaseStandard("staticInit");
                // static byte[] callscript = ExecutionEngine.EntryScriptHash;
                // ...
                // return callscript
                var1 = (result.Pop() as ByteString);
            }
            {
                var testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_StaticVarInit.cs");
                var result = testengine.ExecuteTestCaseStandard("directGet");
                // return ExecutionEngine.EntryScriptHash
                var2 = (result.Pop() as ByteString);
            }
            Assert.IsNotNull(var1);
            Assert.AreEqual(var1, var2);
        }
    }
}
