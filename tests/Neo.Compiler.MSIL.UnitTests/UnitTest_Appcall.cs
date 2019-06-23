using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_AppCall
    {
        [TestMethod]
        public void Test_Appcall()
        {
            var testengine = new TestEngine();
            testengine.AddAppcallScript("./TestClasses/Contract1.cs", "0102030405060708090A0102030405060708090A");
            //will appcall 0102030405060708090A0102030405060708090A
            testengine.AddEntryScript("./TestClasses/Contract_appcall.cs");

            var result = testengine.GetMethod("testfunc").Run();
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);

        }
    }
}
