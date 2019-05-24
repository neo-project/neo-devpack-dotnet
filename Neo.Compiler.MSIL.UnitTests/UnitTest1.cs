using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
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
            var nt = NeonTestTool.Build("./TestClasses/Contract1.cs");
            var names = nt.GetAllILFunction();
            foreach (var n in names)
            {
                Console.WriteLine("got name:" + n);
            }
        }

        [TestMethod]
        public void TestDumpAFunc()
        {
            var testtool = NeonTestTool.Build("./TestClasses/Contract1.cs");
            var ilmethod = testtool.FindMethod("Contract1", "UnitTest_001");
            var neomethod = testtool.GetNEOVMMethod(ilmethod);
            DumpAVM(neomethod);
            var bytes = testtool.NeoMethodToBytes(neomethod);
            DumpBytes(bytes);
        }

        [TestMethod]
        public void Test_ByteArray_New() => JsonTestTool.TestOneCaseInJson("./TestJsons/test_newarray.json","newarray");
        [TestMethod]
        public void Test_ByteArrayPick() => JsonTestTool.TestOneCaseInJson("./TestJsons/test_newarray.json", "pickarray");
    }
}
