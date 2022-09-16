using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IntegerParse
    {
        [TestMethod]
        public void SByteParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testSbyteparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "127");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(sbyte.MaxValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-128");

            value = result.Pop().GetInteger();
            Assert.AreEqual(sbyte.MinValue, value);

            //test backspace trip
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "128");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-129");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void ByteParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testByteparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "0");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(byte.MinValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "255");

            value = result.Pop().GetInteger();
            Assert.AreEqual(byte.MaxValue, value);

            //test backspace trip
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "256");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void UShortParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testUshortparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "0");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(ushort.MinValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "65535");

            value = result.Pop().GetInteger();
            Assert.AreEqual(ushort.MaxValue, value);

            //test backspace trip
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "65536");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void ShortParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testShortparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "-32768");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(short.MinValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "32767");

            value = result.Pop().GetInteger();
            Assert.AreEqual(short.MaxValue, value);

            //test backspace trip
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-32769");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "32768");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void ULongParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testUlongparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "18446744073709551615");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(ulong.MaxValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "0");

            value = result.Pop().GetInteger();
            Assert.AreEqual(ulong.MinValue, value);

            //test backspace trip
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "18446744073709551616");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void LongParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testLongparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, " -9223372036854775808");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(long.MinValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "9223372036854775807");

            value = result.Pop().GetInteger();
            Assert.AreEqual(long.MaxValue, value);

            //test backspace trip
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-9223372036854775809");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "9223372036854775808");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void UIntParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testUintparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "4294967295");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(uint.MaxValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "0");

            value = result.Pop().GetInteger();
            Assert.AreEqual(uint.MinValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-1");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "4294967296");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void IntParse_Test()
        {
            using var testengine = new TestEngine(snapshot: new TestDataCache());
            testengine.AddEntryScript("./TestClasses/Contract_IntegerParse.cs");
            string methodname = "testIntparse";

            var result = testengine.ExecuteTestCaseStandard(methodname, "2147483647");
            var value = result.Pop().GetInteger();
            Assert.AreEqual(int.MaxValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-2147483648");

            value = result.Pop().GetInteger();
            Assert.AreEqual(int.MinValue, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, " 20 ");
            Assert.AreEqual(VMState.FAULT, testengine.State);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "2147483648");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "-2147483649");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "abc");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard(methodname, "@");

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }
    }
}
