using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using System;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVar
    {
        [TestMethod]
        public void Test_InitialValue()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            Assert.IsTrue(testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVar.cs").Success);
            var result = testengine.ExecuteTestCaseStandard("testinitalvalue");

            Assert.AreEqual("hello world", result.Pop().GetString());
        }

        [TestMethod]
        public void Test_StaticVar()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("testMain");

            //test (1+5)*7 == 42
            StackItem wantresult = 42;
            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_StaticVarInit()
        {
            VM.Types.Buffer var1;
            ByteString var2;

            using var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVarInit.cs");
            var result = testengine.ExecuteTestCaseStandard("staticInit");
            // static byte[] callscript = ExecutionEngine.EntryScriptHash;
            // ...
            // return callscript
            var1 = result.Pop() as VM.Types.Buffer;

            testengine.Reset();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVarInit.cs");
            result = testengine.ExecuteTestCaseStandard("directGet");
            // return ExecutionEngine.EntryScriptHash
            var2 = result.Pop() as ByteString;

            Assert.IsNotNull(var1);
            Assert.IsTrue(var1.GetSpan().SequenceEqual(var2.GetSpan()));
        }

        [TestMethod]
        public void Test_testBigIntegerParse()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("testBigIntegerParse");
            var var1 = result.Pop();
            Assert.IsInstanceOfType(var1, typeof(Integer));
            Assert.AreEqual(123, var1.GetInteger());
        }

        [TestMethod]
        public void Test_testBigIntegerParse2()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVar.cs");
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
                using var testengine = new TestEngine();
                testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticConstruct.cs");
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

        [TestMethod]
        public void Test_GetUInt160()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("testGetUInt160");
            var value = result.Pop().GetSpan();

            Assert.AreEqual(value.ToArray().ToHexString(), "7eee1aabeb67ed1d791d44e4f5fcf3ae9171a871");
        }

        [TestMethod]
        public void Test_GetECPoint()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_StaticVar.cs");
            var result = testengine.ExecuteTestCaseStandard("testGetECPoint");
            var value = result.Pop().GetSpan();

            Assert.AreEqual(value.ToArray().ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
        }
    }
}
