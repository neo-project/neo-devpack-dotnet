using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using System;
using System.Linq;
using System.Text;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_SmartContract
    {
        private static void DumpAVM(NeoMethod avmMethod)
        {
            Console.WriteLine("dump:" + avmMethod.displayName + " addr in avm:" + avmMethod.funcaddr);
            foreach (var c in avmMethod.body_Codes)
            {
                Console.WriteLine(c.Key.ToString("X04") + "=>" + c.Value.ToString());
            }
        }

        [TestMethod]
        public void GetAllILFunction()
        {
            var nt = NeonTestTool.BuildScript("./TestClasses/Contract1.cs");
            var names = nt.GetAllILFunction();

            CollectionAssert.AreEqual(new string[] {
                "System.Object Neo.Compiler.MSIL.TestClasses.Contract1::Main(System.String,System.Object[])",
                "System.Byte[] Neo.Compiler.MSIL.TestClasses.Contract1::UnitTest_001()",
                "System.Void Neo.Compiler.MSIL.TestClasses.Contract1::.ctor()"
                }, names.ToArray());
        }

        [TestMethod]
        public void TestSourceMap()
        {
            var testtool = NeonTestTool.BuildScript("./TestClasses/Contract1.cs");

            var conv = new ModuleConverter(null);
            ConvOption option = new ConvOption();
            var neomodule = conv.Convert(testtool.modIL, option);

            // Check
            var sourceMap = SourceMapTool.GenMapFile("test", neomodule);
            Assert.AreEqual(@"{""version"":3,""file"":""test.avm"",""sources"":[],""names"":[]}", sourceMap);
        }

        [TestMethod]
        public void TestDumpAFunc()
        {
            var testtool = NeonTestTool.BuildScript("./TestClasses/Contract1.cs");
            var ilmethod = testtool.FindMethod("Contract1", "UnitTest_001");
            var neomethod = testtool.GetNEOVMMethod(ilmethod);
            DumpAVM(neomethod);

            Assert.AreEqual("UnitTest_001", neomethod.displayName);
            Assert.AreEqual(40, neomethod.funcaddr);

            var bytes = testtool.NeoMethodToBytes(neomethod);
            Assert.AreEqual("52c56b6104010203046a00527ac46a00c36a51527ac46203006a51c3616c7566", bytes.ToHexString());
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
