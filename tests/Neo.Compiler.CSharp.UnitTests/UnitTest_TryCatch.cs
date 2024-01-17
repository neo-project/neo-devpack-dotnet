using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TryCatch
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_TryCatch.cs");
        }

        [TestMethod]
        public void Test_TryCatch_Succ()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("try01");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_UncatchableException()
        {
            testengine.Reset();
            _ = testengine.ExecuteTestCaseStandard("tryUncatchableException");
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
        }

        [TestMethod]
        public void Test_TryCatch_ThrowByCall()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("try03");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryCatch_Throw()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("try02");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryNest()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryNest");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryFinally()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryFinally");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryFinallyAndRethrow");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            Assert.AreEqual(testengine.State, VM.VMState.FAULT);
        }

        [TestMethod]
        public void Test_TryCatch()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryCatch");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryWithTwoFinally");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 9);
        }

        [TestMethod]
        public void Test_TryECPointCast()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryecpointCast");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryValidECPointCast()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryvalidByteString2Ecpoint");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryInvalidUInt160Cast()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryinvalidByteArray2UInt160");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryValidUInt160Cast()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryvalidByteArray2UInt160");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryInvalidUInt256Cast()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryinvalidByteArray2UInt256");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 4);
        }

        [TestMethod]
        public void Test_TryValidUInt256Cast()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryvalidByteArray2UInt256");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var value = result.Pop();
            Console.WriteLine("result:" + value.Type + "  " + value.ToString());
            var num = value as Neo.VM.Types.Integer;
            Console.WriteLine("result = " + num.GetInteger().ToString());
            Assert.AreEqual(num.GetInteger(), 3);
        }

        [TestMethod]
        public void Test_TryNULLECPointCast_1()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryNULL2Ecpoint_1");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var array = result.Pop() as Neo.VM.Types.Struct;
            Assert.AreEqual(4, array[0]);
            Assert.IsTrue(array[1] is Neo.VM.Types.Null);
        }

        [TestMethod]
        public void Test_TryNULLUInt160Cast_1()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryNULL2Uint160_1");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var array = result.Pop() as Neo.VM.Types.Struct;
            Assert.AreEqual(4, array[0]);
            Assert.IsTrue(array[1] is Neo.VM.Types.Null);
        }

        [TestMethod]
        public void Test_TryNULLUInt256Cast_1()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryNULL2Uint256_1");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var array = result.Pop() as Neo.VM.Types.Struct;
            Assert.AreEqual(4, array[0]);
            Assert.IsTrue(array[1] is Neo.VM.Types.Null);
        }

        [TestMethod]
        public void Test_TryNULLBytestringCast_1()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryNULL2Bytestring_1");
            Console.WriteLine("state=" + testengine.State + "  result on stack= " + result.Count);
            var array = result.Pop() as Neo.VM.Types.Struct;
            Assert.AreEqual(4, array[0]);
            Assert.IsTrue(array[1] is Neo.VM.Types.Null);
        }
    }
}
