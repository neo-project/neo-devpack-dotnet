using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class NullableTest : DebugAndTestBase<Contract_Nullable>
    {
        [TestMethod]
        public void TestBigIntegerNullableEqual()
        {
            Assert.IsTrue(Contract.BigIntegerNullableEqual(1, 1));
            Assert.IsFalse(Contract.BigIntegerNullableEqual(1, null));
        }

        [TestMethod]
        public void TestBigIntegerNullableNotEqual()
        {
            Assert.IsTrue(Contract.BigIntegerNullableNotEqual(1, 2));
            Assert.IsFalse(Contract.BigIntegerNullableNotEqual(1, 1));
        }

        [TestMethod]
        public void TestBigIntegerNullableEqualNull()
        {
            Assert.IsTrue(Contract.BigIntegerNullableEqualNull(null));
            Assert.IsFalse(Contract.BigIntegerNullableEqualNull(1));
        }

        [TestMethod]
        public void TestH160NullableNotEqual()
        {
            Assert.IsTrue(Contract.H160NullableNotEqual(UInt160.Zero, UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4")));
            Assert.IsFalse(Contract.H160NullableNotEqual(UInt160.Zero, UInt160.Zero));
        }

        [TestMethod]
        public void TestH160NullableEqualNull()
        {
            Assert.IsTrue(Contract.H160NullableEqualNull(null));
            Assert.IsFalse(Contract.H160NullableEqualNull(UInt160.Zero));
        }

        [TestMethod]
        public void TestH256NullableNotEqual()
        {
            Assert.IsTrue(Contract.H256NullableNotEqual(UInt256.Zero, UInt256.Parse("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925")));
            Assert.IsFalse(Contract.H256NullableNotEqual(UInt256.Zero, UInt256.Zero));
        }

        [TestMethod]
        public void TestH256NullableEqual()
        {
            Assert.IsTrue(Contract.H256NullableEqual(UInt256.Parse("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925"), UInt256.Parse("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925")));
            Assert.IsTrue(Contract.H256NullableEqual(UInt256.Zero, UInt256.Zero));
        }

        [TestMethod]
        public void TestByteNullableEqual()
        {
            Assert.IsTrue(Contract.ByteNullableEqual(1, 1));
            Assert.IsFalse(Contract.ByteNullableEqual(1, null));
        }

        [TestMethod]
        public void TestByteNullableNotEqual()
        {
            Assert.IsTrue(Contract.ByteNullableNotEqual(1, 2));
            Assert.IsFalse(Contract.ByteNullableNotEqual(1, 1));
        }

        [TestMethod]
        public void TestByteNullableEqualNull()
        {
            Assert.IsTrue(Contract.ByteNullableEqualNull(null));
            Assert.IsFalse(Contract.ByteNullableEqualNull(1));
        }

        [TestMethod]
        public void TestSByteNullableEqual()
        {
            Assert.IsTrue(Contract.SByteNullableEqual(1, 1));
            Assert.IsFalse(Contract.SByteNullableEqual(1, null));
        }

        [TestMethod]
        public void TestSByteNullableNotEqual()
        {
            Assert.IsTrue(Contract.SByteNullableNotEqual(1, 2));
            Assert.IsFalse(Contract.SByteNullableNotEqual(1, 1));
        }

        [TestMethod]
        public void TestSByteNullableEqualNull()
        {
            Assert.IsTrue(Contract.SByteNullableEqualNull(null));
            Assert.IsFalse(Contract.SByteNullableEqualNull(1));
        }

        [TestMethod]
        public void TestShortNullableEqual()
        {
            Assert.IsTrue(Contract.ShortNullableEqual(1, 1));
            Assert.IsFalse(Contract.ShortNullableEqual(1, null));
        }

        [TestMethod]
        public void TestShortNullableNotEqual()
        {
            Assert.IsTrue(Contract.ShortNullableNotEqual(1, 2));
            Assert.IsFalse(Contract.ShortNullableNotEqual(1, 1));
        }

        [TestMethod]
        public void TestShortNullableEqualNull()
        {
            Assert.IsTrue(Contract.ShortNullableEqualNull(null));
            Assert.IsFalse(Contract.ShortNullableEqualNull(1));
        }

        [TestMethod]
        public void TestUShortNullableEqual()
        {
            Assert.IsTrue(Contract.UShortNullableEqual(1, 1));
            Assert.IsFalse(Contract.UShortNullableEqual(1, null));
        }

        [TestMethod]
        public void TestUShortNullableNotEqual()
        {
            Assert.IsTrue(Contract.UShortNullableNotEqual(1, 2));
            Assert.IsFalse(Contract.UShortNullableNotEqual(1, 1));
        }

        [TestMethod]
        public void TestUShortNullableEqualNull()
        {
            Assert.IsTrue(Contract.UShortNullableEqualNull(null));
            Assert.IsFalse(Contract.UShortNullableEqualNull(1));
        }

        [TestMethod]
        public void TestIntNullableEqual()
        {
            Assert.IsTrue(Contract.IntNullableEqual(1, 1));
            Assert.IsFalse(Contract.IntNullableEqual(1, null));
        }

        [TestMethod]
        public void TestIntNullableNotEqual()
        {
            Assert.IsTrue(Contract.IntNullableNotEqual(1, 2));
            Assert.IsFalse(Contract.IntNullableNotEqual(1, 1));
        }

        [TestMethod]
        public void TestIntNullableEqualNull()
        {
            Assert.IsTrue(Contract.IntNullableEqualNull(null));
            Assert.IsFalse(Contract.IntNullableEqualNull(1));
        }

        [TestMethod]
        public void TestUIntNullableEqual()
        {
            Assert.IsTrue(Contract.UIntNullableEqual(1, 1u));
            Assert.IsFalse(Contract.UIntNullableEqual(1, null));
        }

        [TestMethod]
        public void TestUIntNullableNotEqual()
        {
            Assert.IsTrue(Contract.UIntNullableNotEqual(1, 2u));
            Assert.IsFalse(Contract.UIntNullableNotEqual(1, 1u));
        }

        [TestMethod]
        public void TestUIntNullableEqualNull()
        {
            Assert.IsTrue(Contract.UIntNullableEqualNull(null));
            Assert.IsFalse(Contract.UIntNullableEqualNull(1u));
        }

        [TestMethod]
        public void TestLongNullableEqual()
        {
            Assert.IsTrue(Contract.LongNullableEqual(1L, 1));
            Assert.IsFalse(Contract.LongNullableEqual(1L, null));
        }

        [TestMethod]
        public void TestLongNullableNotEqual()
        {
            Assert.IsTrue(Contract.LongNullableNotEqual(1L, 2L));
            Assert.IsFalse(Contract.LongNullableNotEqual(1L, 1L));
        }

        [TestMethod]
        public void TestLongNullableEqualNull()
        {
            Assert.IsTrue(Contract.LongNullableEqualNull(null));
            Assert.IsFalse(Contract.LongNullableEqualNull(1L));
        }

        [TestMethod]
        public void TestULongNullableEqual()
        {
            Assert.IsTrue(Contract.ULongNullableEqual(1UL, 1UL));
            Assert.IsFalse(Contract.ULongNullableEqual(1UL, null));
        }

        [TestMethod]
        public void TestULongNullableNotEqual()
        {
            Assert.IsTrue(Contract.ULongNullableNotEqual(1UL, 2UL));
            Assert.IsFalse(Contract.ULongNullableNotEqual(1UL, 1UL));
        }

        [TestMethod]
        public void TestULongNullableEqualNull()
        {
            Assert.IsTrue(Contract.ULongNullableEqualNull(null));
            Assert.IsFalse(Contract.ULongNullableEqualNull(1UL));
        }

        [TestMethod]
        public void TestBoolNullableEqual()
        {
            Assert.IsTrue(Contract.BoolNullableEqual(true, true));
            Assert.IsFalse(Contract.BoolNullableEqual(true, null));
        }

        [TestMethod]
        public void TestBoolNullableNotEqual()
        {
            Assert.IsTrue(Contract.BoolNullableNotEqual(true, false));
            Assert.IsFalse(Contract.BoolNullableNotEqual(true, true));
        }

        [TestMethod]
        public void TestBoolNullableEqualNull()
        {
            Assert.IsTrue(Contract.BoolNullableEqualNull(null));
            Assert.IsFalse(Contract.BoolNullableEqualNull(true));
        }

        [TestMethod]
        public void TestByteNullableToString()
        {
            Assert.IsTrue(Contract.ByteNullableToString(1));
            Assert.IsFalse(Contract.ByteNullableToString(null));
        }

        [TestMethod]
        public void TestSByteNullableToString()
        {
            Assert.IsTrue(Contract.SByteNullableToString(1));
            Assert.IsFalse(Contract.SByteNullableToString(null));
        }

        [TestMethod]
        public void TestShortNullableToString()
        {
            Assert.IsTrue(Contract.ShortNullableToString(1));
            Assert.IsFalse(Contract.ShortNullableToString(null));
        }

        [TestMethod]
        public void TestUShortNullableToString()
        {
            Assert.IsTrue(Contract.UShortNullableToString(1));
            Assert.IsFalse(Contract.UShortNullableToString(null));
        }

        [TestMethod]
        public void TestIntNullableToString()
        {
            Assert.IsTrue(Contract.IntNullableToString(1));
            Assert.IsFalse(Contract.IntNullableToString(null));
        }

        [TestMethod]
        public void TestUIntNullableToString()
        {
            Assert.IsTrue(Contract.UIntNullableToString(1));
            Assert.IsFalse(Contract.UIntNullableToString(null));
        }

        [TestMethod]
        public void TestLongNullableToString()
        {
            Assert.IsTrue(Contract.LongNullableToString(1));
            Assert.IsFalse(Contract.LongNullableToString(null));
        }

        [TestMethod]
        public void TestULongNullableToString()
        {
            Assert.IsTrue(Contract.ULongNullableToString(1));
            Assert.IsFalse(Contract.ULongNullableToString(null));
        }

        [TestMethod]
        public void TestBoolNullableToString()
        {
            Assert.IsTrue(Contract.BoolNullableToString(true));
            Assert.IsFalse(Contract.BoolNullableToString(null));
        }

        [TestMethod]
        public void TestBigIntegerNullableToString()
        {
            Assert.IsTrue(Contract.BigIntegerNullableToString(1));
            Assert.IsFalse(Contract.BigIntegerNullableToString(null));
        }
    }
}
