using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IntegerParse
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(snapshot: new TestDataCache());
            Assert.IsTrue(_engine.AddEntryScript<Contract_IntegerParse>().Success);
        }

        [TestMethod]
        public void SByteParse_Test()
        {
            string methodname = "testSbyteparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "127");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(sbyte.MaxValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-128");

            value = result.Pop().GetInteger();
            Assert.AreEqual(sbyte.MinValue, value);

            //test backspace trip
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "128");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-129");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            string methodname = "testByteparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "0");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(byte.MinValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "255");

            value = result.Pop().GetInteger();
            Assert.AreEqual(byte.MaxValue, value);

            //test backspace trip
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "256");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            string methodname = "testUshortparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "0");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(ushort.MinValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "65535");

            value = result.Pop().GetInteger();
            Assert.AreEqual(ushort.MaxValue, value);

            //test backspace trip
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "65536");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            string methodname = "testShortparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "-32768");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(short.MinValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "32767");

            value = result.Pop().GetInteger();
            Assert.AreEqual(short.MaxValue, value);

            //test backspace trip
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-32769");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "32768");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            string methodname = "testUlongparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "18446744073709551615");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(ulong.MaxValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "0");

            value = result.Pop().GetInteger();
            Assert.AreEqual(ulong.MinValue, value);

            //test backspace trip
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "18446744073709551616");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void LongParse_Test()
        {
            string methodname = "testLongparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "-9223372036854775808");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(long.MinValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "9223372036854775807");

            value = result.Pop().GetInteger();
            Assert.AreEqual(long.MaxValue, value);

            //test backspace trip
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-9223372036854775809");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "9223372036854775808");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            string methodname = "testUintparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "4294967295");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(uint.MaxValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "0");

            value = result.Pop().GetInteger();
            Assert.AreEqual(uint.MinValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "4294967296");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void IntParse_Test()
        {
            string methodname = "testIntparse";
            var result = _engine.ExecuteTestCaseStandard(methodname, "2147483647");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(int.MaxValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-2147483648");

            value = result.Pop().GetInteger();
            Assert.AreEqual(int.MinValue, value);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "2147483648");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "-2147483649");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }
    }
}
