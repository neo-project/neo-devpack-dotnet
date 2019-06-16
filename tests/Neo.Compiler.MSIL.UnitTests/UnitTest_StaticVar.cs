using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_StaticVar
    {
        [TestMethod]
        public void Test_StaticVar()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StaticVar.cs");
            StackItem[] _params = new StackItem[] { "testfunc", new StackItem[0] };
            var result = testengine.ExecuteTestCase(_params);

            //test (1+5)*7 == 42
            StackItem wantresult = 42;
            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

    }
}
