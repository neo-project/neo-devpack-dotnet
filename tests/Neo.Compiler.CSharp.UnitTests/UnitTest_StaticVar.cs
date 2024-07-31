using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_StaticVar : TestBase<Contract_StaticVar>
    {
        public UnitTest_StaticVar() : base(Contract_StaticVar.Nef, Contract_StaticVar.Manifest) { }

        [TestMethod]
        public void Test_InitialValue()
        {
            Assert.AreEqual("hello world", Contract.Testinitalvalue());
            Assert.AreEqual(2956140, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_StaticVar()
        {
            //test (1+5)*7 == 42
            Assert.AreEqual(new BigInteger(42), Contract.TestMain());
            Assert.AreEqual(2988480, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testBigIntegerParse()
        {
            Assert.AreEqual(new BigInteger(123), Contract.TestBigIntegerParse());
            Assert.AreEqual(2956440, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testBigIntegerParse2()
        {
            Assert.AreEqual(new BigInteger(123), Contract.TestBigIntegerParse2("123"));
            Assert.AreEqual(4004280, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_GetUInt160()
        {
            Assert.AreEqual("0x71a87191aef3fcf5e4441d791ded67ebab1aee7e", Contract.TestGetUInt160()?.ToString());
            Assert.AreEqual(2956140, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_GetECPoint()
        {
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", Contract.TestGetECPoint()?.ToString());
            Assert.AreEqual(2956140, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_GetString()
        {
            Assert.AreEqual("hello world", Contract.TestGetString());
            Assert.AreEqual(2956140, Engine.FeeConsumed.Value);
        }
    }
}
