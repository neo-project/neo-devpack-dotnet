using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BigInteger
    {

        private TestEngine testengine;

        [TestInitialize]
        public void Initialize()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_BigInteger.cs");
        }

        [TestMethod]
        public void Test_Pow()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testPow", 2, 3);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(8, value);
        }

        [TestMethod]
        public void Test_Sqrt()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testSqrt", 4);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(2, value);
        }

        [TestMethod]
        public void Test_Sbyte()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testsbyte", 127);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(127, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testsbyte", -128);

            value = result.Pop().GetInteger();
            Assert.AreEqual(-128, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testsbyte", 128);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testsbyte", -129);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void Test_byte()
        {
            var result = testengine.ExecuteTestCaseStandard("testbyte", 0);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(0, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testbyte", 255);

            value = result.Pop().GetInteger();
            Assert.AreEqual(255, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testbyte", -1);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testbyte", 256);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

        }

        [TestMethod]
        public void Test_short()
        {
            var result = testengine.ExecuteTestCaseStandard("testshort", 32767);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(32767, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testshort", -32768);

            value = result.Pop().GetInteger();
            Assert.AreEqual(-32768, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testshort", 32768);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testshort", -32769);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void Test_ushort()
        {
            var result = testengine.ExecuteTestCaseStandard("testushort", 0);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(0, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testushort", 65535);

            value = result.Pop().GetInteger();
            Assert.AreEqual(65535, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testushort", -1);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testushort", 65536);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void Test_int()
        {
            var result = testengine.ExecuteTestCaseStandard("testint", -2147483648);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(-2147483648, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testint", 2147483647);

            value = result.Pop().GetInteger();
            Assert.AreEqual(2147483647, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testint", -2147483649);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testint", 2147483648);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void Test_uint()
        {
            var result = testengine.ExecuteTestCaseStandard("testuint", 0);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(0, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testuint", 4294967295);

            value = result.Pop().GetInteger();
            Assert.AreEqual(4294967295, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testuint", -1);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testuint", 4294967296);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void Test_long()
        {
            var result = testengine.ExecuteTestCaseStandard("testlong", -9223372036854775808);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(-9223372036854775808, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testlong", 9223372036854775807);

            value = result.Pop().GetInteger();
            Assert.AreEqual(9223372036854775807, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testlong", 9223372036854775808);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }

        [TestMethod]
        public void Test_ulong()
        {
            var result = testengine.ExecuteTestCaseStandard("testulong", 0);

            var value = result.Pop().GetInteger();
            Assert.AreEqual(0, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testulong", 18446744073709551615);

            value = result.Pop().GetInteger();
            Assert.AreEqual(18446744073709551615, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testulong", -1);

            Assert.AreEqual(VMState.FAULT, testengine.State);
            Assert.IsNotNull(testengine.FaultException);
        }
        [TestMethod]
        public void Test_IsEven()
        {
            // Test 0
            var result = testengine.ExecuteTestCaseStandard("testIsEven", 0);
            var value = result.Pop().GetBoolean();
            Assert.AreEqual(new BigInteger(0).IsEven, value);
            testengine.Reset();
            // Test 1
            result = testengine.ExecuteTestCaseStandard("testIsEven", 1);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(new BigInteger(1).IsEven, value);
            testengine.Reset();
            // Test 2
            result = testengine.ExecuteTestCaseStandard("testIsEven", 2);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(new BigInteger(2).IsEven, value);
            testengine.Reset();
            // Test -1
            result = testengine.ExecuteTestCaseStandard("testIsEven", -1);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(new BigInteger(-1).IsEven, value);
            testengine.Reset();
            // Test -2
            result = testengine.ExecuteTestCaseStandard("testIsEven", -2);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(new BigInteger(-2).IsEven, value);
            testengine.Reset();
        }
        [TestMethod]
        public void Test_Add()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testAdd", 123456789, 987654321);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(new BigInteger(1111111110), value);
        }

        [TestMethod]
        public void Test_Subtract()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testSubtract", 123456789, 987654321);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(new BigInteger(-864197532), value);
        }

        [TestMethod]
        public void Test_Multiply()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testMultiply", 123, 321);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(new BigInteger(39483), value);
        }

        [TestMethod]
        public void Test_Divide()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testDivide", 123456, 123);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(BigInteger.Divide(123456, 123), value);
        }

        [TestMethod]
        public void Test_Negate()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testNegate", 123456);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(new BigInteger(-123456), value);
        }

        [TestMethod]
        public void Test_Remainder()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testRemainder", 123456, 123);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(BigInteger.Remainder(123456, 123), value);
        }

        [TestMethod]
        public void Test_Compare()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testCompare", 123, 321);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(BigInteger.Compare(123, 321), value);
        }

        [TestMethod]
        public void Test_GreatestCommonDivisor()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("testCompare", 48, 18);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(48, 18), value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testCompare", -48, -18);
            value = result.Pop().GetInteger();
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(-48, -18), value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testCompare", 24, 12);
            value = result.Pop().GetInteger();
            Assert.AreEqual(BigInteger.GreatestCommonDivisor(24, 12), value);
        }
    }
}
