using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using System;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest1
    {
        private static void DumpNVM(NeoMethod nvmMethod)
        {
            Console.WriteLine("dump:" + nvmMethod.displayName + " addr in nvm:" + nvmMethod.funcaddr);
            foreach (var c in nvmMethod.body_Codes)
            {
                Console.WriteLine(c.Key.ToString("X04") + "=>" + c.Value.ToString());
            }
        }

        private static void DumpBytes(byte[] data)
        {
            Console.WriteLine("NVM=");
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
            DumpNVM(neomethod);
            var bytes = testtool.NeoMethodToBytes(neomethod);
            DumpBytes(bytes);
        }

        [TestMethod]
        public void TestRunAFunc()
        {

            var testtool = NeonTestTool.Build("./TestClasses/Contract2.cs");
            //run this below

            //public static byte UnitTest_001()
            //{
            //    var nb = new byte[] { 1, 2, 3, 4 };
            //    return nb[2];
            //}
            VM.StackItem[] items = new VM.StackItem[]
            {
                "hello",
                new VM.StackItem[]{}
            };
            var result = testtool.RunScript(0, items);
            var resultnum = result.ResultStack.Peek().GetBigInteger();
            // and check if the result is 3

            Assert.AreEqual(resultnum, 3);
        }
    }
}
