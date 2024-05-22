using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Foreach : TestBase<Contract_Foreach>
    {
        public UnitTest_Foreach() : base(Contract_Foreach.Nef, Contract_Foreach.Manifest) { }

        [TestMethod]
        public void IntForeachTest()
        {
            Assert.AreEqual(10, Contract.IntForeach());
            Assert.AreEqual(6, Contract.IntForeachBreak(3));
        }

        [TestMethod]
        public void IntForloopTest()
        {
            Assert.AreEqual(10, Contract.IntForloop());
            Assert.AreEqual(6, Contract.IntForeachBreak(3));
        }

        [TestMethod]
        public void StringForeachTest()
        {
            Assert.AreEqual("abcdefhij", Contract.StringForeach());
        }

        [TestMethod]
        public void ByteStringEmptyTest()
        {
            Assert.AreEqual(0, Contract.ByteStringEmpty());
        }

        [TestMethod]
        public void BytestringForeachTest()
        {
            Assert.AreEqual("abcdefhij", Encoding.ASCII.GetString(Contract.ByteStringForeach()!));
        }

        [TestMethod]
        public void StructForeachTest()
        {
            var map = Contract.StructForeach()!;

            Assert.AreEqual(map[(ByteString)"test1"], new BigInteger(1));
            Assert.AreEqual(map[(ByteString)"test2"], new BigInteger(2));
        }

        [TestMethod]
        public void ByteArrayForeachTest()
        {
            var array = Contract.ByteArrayForeach()!;

            Assert.AreEqual(array[0], new BigInteger(1));
            Assert.AreEqual(array[1], new BigInteger(10));
            Assert.AreEqual(array[2], new BigInteger(17));
        }

        [TestMethod]
        public void Uint160ForeachTest()
        {
            var array = Contract.UInt160Foreach()!;

            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual((array[0] as ByteString)!.GetSpan().ToHexString(), "0000000000000000000000000000000000000000");
            Assert.AreEqual((array[1] as ByteString)!.GetSpan().ToHexString(), "0000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void Uint256ForeachTest()
        {
            var array = Contract.UInt256Foreach()!;

            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual((array[0] as ByteString)!.GetSpan().ToHexString(), "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.AreEqual((array[1] as ByteString)!.GetSpan().ToHexString(), "0000000000000000000000000000000000000000000000000000000000000000");
        }

        [TestMethod]
        public void EcpointForeachTest()
        {
            var array = Contract.ECPointForeach()!;

            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual((array[0] as ByteString)!.GetSpan().ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
            Assert.AreEqual((array[1] as ByteString)!.GetSpan().ToHexString(), "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
        }

        [TestMethod]
        public void BigintegerForeachTest()
        {
            var array = Contract.BigIntegerForeach()!;
            BigInteger[] expected = [10_000, 1000_000, 1000_000_000, 1000_000_000_000_000_000];

            Assert.AreEqual(array.Count, 4);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(array[i], expected[i]);
            }
        }

        [TestMethod]
        public void ObjectarrayForeachTest()
        {
            var array = Contract.ObjectArrayForeach()!;

            Assert.AreEqual(array.Count, 3);
            CollectionAssert.AreEqual(array[0] as byte[], new byte[] { 0x01, 0x02 });
            Assert.AreEqual((array[1] as ByteString)!.GetString(), "test");
            Assert.AreEqual(array[2], new BigInteger(123));
        }
    }
}
