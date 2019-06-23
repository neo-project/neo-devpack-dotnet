using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest1
    {
        private static void DumpAVM(NeoMethod avmMethod)
        {
            Console.WriteLine("dump:" + avmMethod.displayName + " addr in avm:" + avmMethod.funcaddr);
            foreach (var c in avmMethod.body_Codes)
            {
                Console.WriteLine(c.Key.ToString("X04") + "=>" + c.Value.ToString());
            }
        }

        private static void DumpBytes(byte[] data)
        {
            Console.WriteLine("AVM=");
            foreach (var b in data)
            {
                Console.Write(b.ToString("X02"));
            }
            Console.WriteLine("");
        }

        [TestMethod]
        public void GetAllILFunction()
        {
            var nt = NeonTestTool.BuildScript("./TestClasses/Contract1.cs");
            var names = nt.GetAllILFunction();
            foreach (var n in names)
            {
                Console.WriteLine("got name:" + n);
            }
        }

        [TestMethod]
        public void TestDumpAFunc()
        {
            var testtool = NeonTestTool.BuildScript("./TestClasses/Contract1.cs");
            var ilmethod = testtool.FindMethod("Contract1", "UnitTest_001");
            var neomethod = testtool.GetNEOVMMethod(ilmethod);
            DumpAVM(neomethod);
            var bytes = testtool.NeoMethodToBytes(neomethod);
            DumpBytes(bytes);
        }

        [TestMethod]
        public void Test_ByteArray_New()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract1.cs");


            var result = testengine.GetMethod("testfunc").Run();
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }
        [TestMethod]
        public void Test_ByteArrayPick()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract2.cs");

            var result = testengine.GetMethod("testfunc").Run("hello", 1, 2, 3, 4);
            StackItem wantresult = 3;

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }
    }
}
