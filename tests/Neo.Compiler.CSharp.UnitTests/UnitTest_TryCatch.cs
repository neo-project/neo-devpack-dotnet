using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TryCatch : TestBase<Contract_TryCatch>
    {
        public UnitTest_TryCatch() : base(Contract_TryCatch.Nef, Contract_TryCatch.Manifest) { }

        [TestMethod]
        public void Test_TryCatch_Succ()
        {
            Assert.AreEqual(new BigInteger(3), Contract.Try01());
            Assert.AreEqual(1002064870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_UncatchableException()
        {
            _ = Assert.ThrowsException<TestException>(() => Contract.TryUncatchableException());
            Assert.AreEqual(1002063850, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryCatch_ThrowByCall()
        {
            Assert.AreEqual(new BigInteger(4), Contract.Try03());
            Assert.AreEqual(1002096100, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryCatch_Throw()
        {
            Assert.AreEqual(new BigInteger(4), Contract.Try02());
            Assert.AreEqual(1002080740, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryNest()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryNest());
            Assert.AreEqual(1002158080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ThrowInCatch()
        {
            var exception = Assert.ThrowsException<TestException>(() => Contract.ThrowInCatch());
            Assert.AreEqual(1002095650, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(3), exception.CurrentContext?.LocalVariables?[0].GetInteger());
            Assert.AreEqual(1002095650, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryFinally()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryFinally());
            Assert.AreEqual(1002064870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryFinallyAndRethrow()
        {
            _ = Assert.ThrowsException<TestException>(() => Contract.TryFinallyAndRethrow());
            Assert.AreEqual(1002095590, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryCatch()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryCatch());
            Assert.AreEqual(1002095770, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryWithTwoFinally()
        {
            Assert.AreEqual(new BigInteger(9), Contract.TryWithTwoFinally());
            Assert.AreEqual(1002066640, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryECPointCast()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryecpointCast());
            Assert.AreEqual(1002081010, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryValidECPointCast()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteString2Ecpoint());
            Assert.AreEqual(1002065440, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryInvalidUInt160Cast()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt160());
            Assert.AreEqual(1002081010, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryValidUInt160Cast()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt160());
            Assert.AreEqual(1002065440, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryInvalidUInt256Cast()
        {
            Assert.AreEqual(new BigInteger(4), Contract.TryinvalidByteArray2UInt256());
            Assert.AreEqual(1002081010, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryValidUInt256Cast()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TryvalidByteArray2UInt256());
            Assert.AreEqual(1002064990, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_TryNULLECPointCast_1()
        {
            var array = Contract.TryNULL2Ecpoint_1();
            Assert.AreEqual(1002813130, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);
        }

        [TestMethod]
        public void Test_TryNULLUInt160Cast_1()
        {
            var array = Contract.TryNULL2Uint160_1();
            Assert.AreEqual(1002813130, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);
        }

        [TestMethod]
        public void Test_TryNULLUInt256Cast_1()
        {
            var array = Contract.TryNULL2Uint256_1();
            Assert.AreEqual(1002813130, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);
        }

        [TestMethod]
        public void Test_TryNULLBytestringCast_1()
        {
            var array = Contract.TryNULL2Bytestring_1();
            Assert.AreEqual(1002559450, Engine.FeeConsumed.Value);
            Assert.AreEqual(new BigInteger(4), array?[0]);
            Assert.IsNull(array?[1]);
        }
    }
}
