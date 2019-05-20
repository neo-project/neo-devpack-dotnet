using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest1
    {
        //private static readonly NeonTestTool testtool = new NeonTestTool("TestContract.dll");

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
            var curpath = System.IO.Directory.GetCurrentDirectory();
            var srcfile = System.IO.Path.GetFullPath("../../../../testdll/TestClass1.dll");
            var nt = new NeonTestTool(srcfile);
            var names = nt.GetAllILFunction();
            foreach (var n in names)
            {
                Console.WriteLine("got name:" + n);
            }
        }

        [TestMethod]
        public void TestDumpAFunc()
        {
            var srcfile = System.IO.Path.GetFullPath("../../../../testdll/TestClass1.dll");

            var testtool = new NeonTestTool(srcfile);

            var ilmethod = testtool.FindMethod("TestClass1", "UnitTest_001");
            var neomethod = testtool.GetNEOVMMethod(ilmethod);
            DumpAVM(neomethod);
            var bytes = testtool.NeoMethodToBytes(neomethod);
            DumpBytes(bytes);
        }

        [TestMethod]
        public void TestRunAFunc()
        {
            var srcfile = System.IO.Path.GetFullPath("../../../../testdll/TestClass1.dll");

            var testtool = new NeonTestTool(srcfile);
            //run this below

            //public static byte UnitTest_001()
            //{
            //    var nb = new byte[] { 1, 2, 3, 4 };
            //    return nb[2];
            //}
            var result = testtool.RunScript(0, null);
            var resultnum = result.ResultStack.Peek().GetBigInteger();
            // and check if the result is 3

            Assert.AreEqual(resultnum, 3);
        }
    }
}
