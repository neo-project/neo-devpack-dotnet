using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_AppCall
    {
        [TestMethod]
        public void Test_Appcall()
        {
            var testengine = new TestEngine();
            testengine.AddAppcallScript("0102030405060708090A0102030405060708090A", "./TestClasses/Contract1.cs");
            //will appcall 0102030405060708090A0102030405060708090A
            testengine.AddEntryScript("./TestClasses/Contract_appcall.cs");

            var result = testengine.GetMethod("testfunc").Run();
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);

        }
    }
}
