using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVar
    {
        [TestMethod]
        public void Test_StaticVar()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("main");

            //test (1+5)*7 == 42
            StackItem wantresult = 42;
            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_StaticVarInit()
        {
            Neo.VM.Types.Buffer var1;
            ByteString var2;
            {
                var testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_StaticVarInit.cs");
                var result = testengine.ExecuteTestCaseStandard("staticInit");
                // static byte[] callscript = ExecutionEngine.EntryScriptHash;
                // ...
                // return callscript
                var1 = (result.Pop() as Neo.VM.Types.Buffer);
            }
            {
                var testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_StaticVarInit.cs");
                var result = testengine.ExecuteTestCaseStandard("directGet");
                // return ExecutionEngine.EntryScriptHash
                var2 = (result.Pop() as ByteString);
            }
            Assert.IsNotNull(var1);
            Assert.IsTrue(var1.GetSpan().SequenceEqual(var2.GetSpan()));
        }

        [TestMethod]
        public void Test_testBigIntegerParse()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("testBigIntegerParse");
            var var1 = result.Pop();
            Assert.IsInstanceOfType(var1, typeof(Integer));
            Assert.AreEqual(123, var1.GetInteger());
        }

        [TestMethod]
        public void Test_testBigIntegerParse2()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("testBigIntegerParse2", "123");
            var var1 = result.Pop();
            Assert.IsInstanceOfType(var1, typeof(Integer));
            Assert.AreEqual(123, var1.GetInteger());
        }

        [TestMethod]
        public void Test_StaticConsturct()
        {
            StackItem var1;
            try
            {
                var testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_StaticConstruct.cs");
                var result = testengine.ExecuteTestCaseStandard("testStatic");
                // static byte[] callscript = ExecutionEngine.EntryScriptHash;
                // ...
                // return callscript
                var1 = (result.Pop());

                Assert.IsNotNull(var1);
                Assert.IsTrue(var1.GetInteger() == 4);
                Assert.Fail("should throw a error \"not support opcode xxx\" in this case.");
            }
            catch (Exception err)
            {
                Console.WriteLine("error message:" + err.Message);
                //need throw a error.
                Assert.IsTrue(err.Message.Contains("not support opcode"));
            }
        }
    }
}
