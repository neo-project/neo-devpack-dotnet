using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private static void DumpNEF(NeoMethod nefMethod)
        {
            Console.WriteLine("dump:" + nefMethod.displayName + " addr in nef:" + nefMethod.funcaddr);
            foreach (var c in nefMethod.body_Codes)
            {
                Console.WriteLine(c.Key.ToString("X04") + "=>" + c.Value.ToString());
            }
        }

        private static void DumpBytes(byte[] data)
        {
            Console.WriteLine("NEF=");
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
            var ilmethod = testtool.FindMethod("Contract1", "unitTest_001");
            var neomethod = testtool.GetNEOVMMethod(ilmethod);
            DumpNEF(neomethod);
            var bytes = testtool.NeoMethodToBytes(neomethod);
            DumpBytes(bytes);
        }

        [TestMethod]
        public void Test_ByteArray_New()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract1.cs");


            var result = testengine.GetMethod("unitTest_001").Run().ConvertTo(StackItemType.ByteString);
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_ByteArrayPick()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract2.cs");

            var result = testengine.GetMethod("unitTest_002").Run("hello", 1);

            Assert.AreEqual(3, result.GetBigInteger());
        }
    }
}
