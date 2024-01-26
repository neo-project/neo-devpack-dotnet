using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using System;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVar
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(snapshot: new TestDataCache());
            Assert.IsTrue(_engine.AddEntryScript<Contract_StaticVar>().Success);
        }

        [TestMethod]
        public void Test_InitialValue()
        {
            var result = _engine.ExecuteTestCaseStandard("testinitalvalue");
            Assert.AreEqual("hello world", result.Pop().GetString());
        }

        [TestMethod]
        public void Test_StaticVar()
        {
            var result = _engine.ExecuteTestCaseStandard("testMain");

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
            using var _engine = new TestEngine();
            _engine.AddEntryScript<Contract_StaticVarInit>();
            var result = _engine.ExecuteTestCaseStandard("staticInit");
            // static byte[] callscript = ExecutionEngine.EntryScriptHash;
            // ...
            // return callscript
            var1 = result.Pop() as VM.Types.Buffer;

            _engine.Reset();
            _engine.AddEntryScript<Contract_StaticVarInit>();
            result = _engine.ExecuteTestCaseStandard("directGet");
            // return ExecutionEngine.EntryScriptHash
            var2 = result.Pop() as ByteString;

            Assert.IsNotNull(var1);
            Assert.IsTrue(var1.GetSpan().SequenceEqual(var2.GetSpan()));
        }

        [TestMethod]
        public void Test_testBigIntegerParse()
        {
            var result = _engine.ExecuteTestCaseStandard("testBigIntegerParse");
            var var1 = result.Pop();
            Assert.IsInstanceOfType(var1, typeof(Integer));
            Assert.AreEqual(123, var1.GetInteger());
        }

        [TestMethod]
        public void Test_testBigIntegerParse2()
        {
            var result = _engine.ExecuteTestCaseStandard("testBigIntegerParse2", "123");
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
                using var _engine = new TestEngine();
                _engine.AddEntryScript<Contract_StaticConstruct>();
                var result = _engine.ExecuteTestCaseStandard("testStatic");
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
