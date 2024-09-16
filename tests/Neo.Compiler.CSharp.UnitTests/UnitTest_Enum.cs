using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Enum : DebugAndTestBase<Contract_Enum>
    {
        [TestMethod]
        public void TestEnumParse()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParse("Value1"));
            AssertGasConsumed(989700);
            Assert.AreEqual(new Integer(2), Contract.TestEnumParse("Value2"));
            AssertGasConsumed(989700);
            Assert.AreEqual(new Integer(3), Contract.TestEnumParse("Value3"));
            AssertGasConsumed(989700);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParse("InvalidValue"));
            AssertGasConsumed(989700);
        }

        [TestMethod]
        public void TestEnumParseIgnoreCase()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParseIgnoreCase("value1", true));
            AssertGasConsumed(989700);
            Assert.AreEqual(new Integer(2), Contract.TestEnumParseIgnoreCase("VALUE2", true));
            AssertGasConsumed(989700);
            Assert.AreEqual(new Integer(3), Contract.TestEnumParseIgnoreCase("VaLuE3", true));
            AssertGasConsumed(989700);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseIgnoreCase("value1", false));
            AssertGasConsumed(989700);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseIgnoreCase("InvalidValue", true));
            AssertGasConsumed(989700);
        }

        [TestMethod]
        public void TestEnumTryParse()
        {
            Assert.IsTrue(Contract.TestEnumTryParse("Value1"));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumTryParse("Value2"));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumTryParse("Value3"));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumTryParse("InvalidValue"));
            AssertGasConsumed(989700);
        }

        [TestMethod]
        public void TestEnumTryParseIgnoreCase()
        {
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("value1", true));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("VALUE2", true));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("VaLuE3", true));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumTryParseIgnoreCase("value1", false));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumTryParseIgnoreCase("InvalidValue", true));
            AssertGasConsumed(989700);
        }

        // [TestMethod]
        // public void TestEnumGetNames()
        // {
        //     var names = Contract.TestEnumGetNames();
        //     AssertGasConsumed(989700);
        //     CollectionAssert.AreEqual(new[] { "Value1", "Value2", "Value3" }, names);
        // }
        //
        // [TestMethod]
        // public void TestEnumGetValues()
        // {
        //     var values = Contract.TestEnumGetValues();
        //     AssertGasConsumed(989700);
        //     CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
        // }

        [TestMethod]
        public void TestEnumIsDefined()
        {
            Assert.IsTrue(Contract.TestEnumIsDefined(1));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumIsDefined(2));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumIsDefined(3));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumIsDefined(0));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumIsDefined(4));
            AssertGasConsumed(989700);
        }

        [TestMethod]
        public void TestEnumIsDefinedByName()
        {
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value1"));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value2"));
            AssertGasConsumed(989700);
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value3"));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumIsDefinedByName("value1"));
            AssertGasConsumed(989700);
            Assert.IsFalse(Contract.TestEnumIsDefinedByName("InvalidValue"));
            AssertGasConsumed(989700);
        }

        [TestMethod]
        public void TestEnumGetName()
        {
            Assert.AreEqual("Value1", Contract.TestEnumGetName(1));
            AssertGasConsumed(989700);
            Assert.AreEqual("Value2", Contract.TestEnumGetName(2));
            AssertGasConsumed(989700);
            Assert.AreEqual("Value3", Contract.TestEnumGetName(3));
            AssertGasConsumed(989700);
            Assert.IsNull(Contract.TestEnumGetName(0));
            AssertGasConsumed(989700);
            Assert.IsNull(Contract.TestEnumGetName(4));
            AssertGasConsumed(989700);
        }

        [TestMethod]
        public void TestEnumGetNameWithType()
        {
            Assert.AreEqual("Value1", Contract.TestEnumGetNameWithType(1));
            AssertGasConsumed(989700);
            Assert.AreEqual("Value2", Contract.TestEnumGetNameWithType(2));
            AssertGasConsumed(989700);
            Assert.AreEqual("Value3", Contract.TestEnumGetNameWithType(3));
            AssertGasConsumed(989700);
            Assert.IsNull(Contract.TestEnumGetNameWithType(0));
            AssertGasConsumed(989700);
            Assert.IsNull(Contract.TestEnumGetNameWithType(4));
            AssertGasConsumed(989700);
        }
    }
}
