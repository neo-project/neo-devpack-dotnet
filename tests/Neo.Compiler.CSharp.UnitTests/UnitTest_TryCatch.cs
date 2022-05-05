using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TryCatch
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine(snapshot: new TestDataCache());
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

        [TestMethod]
        public void Test_TryCatch_Unsafe_Method()
        {
            var methods = new string[] {
                "unsafe_Method",
                "unsafe_Method_Static",
                "unsafe_Block",
                "unsafe_Block_Static",
                "unsafe_Property",
                "unsafe_Property_Static",
                "unsafe_Block_In_Property",
                "unsafe_Block_In_Property_Static"
            };

            var contract = testengine.EntryScriptHash;

            testengine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = contract,
                Nef = testengine.Nef,
                Manifest = ContractManifest.FromJson(testengine.Manifest),
            });

            foreach (var method in methods)
            {
                testengine.Reset();
                var result = testengine.ExecuteTestCaseStandard("tryUnsafeCall", method);
                Console.WriteLine($"[{method}]state={testengine.State}  ; result on stack= {result.Count}");

                Assert.AreEqual(VM.VMState.FAULT, testengine.State, $"[{method}]");
                Assert.AreEqual(0, result.Count);
                Assert.AreEqual("ABORT is executed.", testengine.FaultException?.Message, $"[{method}]");
            }
        }

        [TestMethod]
        public void Test_TryCatch_Unsafe_ContractCall()
        {
            var hash = UInt160.Parse("0102030405060708090A0102030405060708090A");
            var _engine = new TestEngine(snapshot: new TestDataCache());
            _engine.AddEntryScript("./TestClasses/Contract_UnsafeContract.cs");
            testengine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = hash,
                Nef = _engine.Nef,
                Manifest = ContractManifest.FromJson(_engine.Manifest),
            });
            var contract = testengine.EntryScriptHash;
            testengine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = contract,
                Nef = testengine.Nef,
                Manifest = ContractManifest.FromJson(testengine.Manifest),
            });

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("tryUnsafe_ContractCall", hash.ToArray());
            Console.WriteLine($"state={testengine.State}  ; result on stack= {result.Count}");

            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual("ABORT is executed.", testengine.FaultException?.Message);
        }
    }
}
