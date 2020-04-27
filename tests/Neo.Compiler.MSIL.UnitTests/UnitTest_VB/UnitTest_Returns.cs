using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.UnitTests.VB
{
    [TestClass]
    public class UnitTest_Returns
    {
        private static void DumpNEF(NeoMethod nefMethod)
        {
            Console.WriteLine("dump:" + nefMethod.displayName + " addr in nef:" + nefMethod.funcaddr);
            foreach (var c in nefMethod.body_Codes)
            {
                Console.WriteLine(c.Key.ToString("X04") + "=>" + c.Value.ToString());
            }
        }

        [TestMethod]
        public void GetAllILFunction()
        {
            var nt = NeonTestTool.BuildScript("./TestClasses_VB/Contract_Return1.vb");
            var names = nt.GetAllILFunction();

            CollectionAssert.AreEqual(new string[] {
                "System.Void SmartContract1::.ctor()",
                "System.Object SmartContract1::Main()",
                "System.Byte[] SmartContract1::UnitTest_001()"
            },
            names);
        }

        [TestMethod]
        public void TestDumpAFunc()
        {
            var testtool = NeonTestTool.BuildScript("./TestClasses_VB/Contract_Return1.vb");
            var ilmethod = testtool.FindMethod("SmartContract1", "Main");
            var neomethod = testtool.GetNEOVMMethod(ilmethod);
            DumpNEF(neomethod);

            var bytes = testtool.NeoMethodToBytes(neomethod);
            Assert.IsTrue(bytes.Length > 0);
        }

        [TestMethod]
        public void Test_ByteArray_New()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses_VB/Contract_Return1.vb");

            var result = testengine.GetMethod("unitTest_001").Run().ConvertTo(StackItemType.ByteString);
            StackItem wantresult = new byte[] { 1, 2, 3, 4 };

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_ByteArrayPick()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses_VB/Contract_Return2.vb");

            var result = testengine.GetMethod("unitTest_002").Run();
            StackItem wantresult = 3;

            var bequal = wantresult.Equals(result);
            Assert.IsTrue(bequal);
        }
    }
}
