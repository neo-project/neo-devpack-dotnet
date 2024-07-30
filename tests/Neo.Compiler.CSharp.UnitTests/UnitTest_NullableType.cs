using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NullableType : TestBase<Contract_NullableType>
    {
        public UnitTest_NullableType() : base(Contract_NullableType.Nef, Contract_NullableType.Manifest) { }

        [TestMethod]
        public void TestBigInteger()
        {
            Contract.TestBigInteger(BigInteger.One);
            Contract.TestBigInteger(null);
        }

        [TestMethod]
        public void TestInt()
        {
            Contract.TestInt(1);
            Contract.TestInt(null);
        }

        [TestMethod]
        public void TestUInt()
        {
            Contract.TestUInt(1u);
            Contract.TestUInt(null);
        }

        [TestMethod]
        public void TestLong()
        {
            Contract.TestLong(1L);
            Contract.TestLong(null);
        }

        [TestMethod]
        public void TestULong()
        {
            Contract.TestULong(1UL);
            Contract.TestULong(null);
        }

        [TestMethod]
        public void TestShort()
        {
            Contract.TestShort((short)1);
            Contract.TestShort(null);
        }

        [TestMethod]
        public void TestUShort()
        {
            Contract.TestUShort((ushort)1);
            Contract.TestUShort(null);
        }

        [TestMethod]
        public void TestSByte()
        {
            Contract.TestSByte((sbyte)1);
            Contract.TestSByte(null);
        }

        [TestMethod]
        public void TestByte()
        {
            Contract.TestByte((byte)1);
            Contract.TestByte(null);
        }

        [TestMethod]
        public void TestBool()
        {
            Contract.TestBool(true);
            Contract.TestBool(null);
        }

        [TestMethod]
        public void TestUInt160()
        {
            Contract.TestUInt160(UInt160.Zero);
            Contract.TestUInt160(null);
        }

        [TestMethod]
        public void TestUInt256()
        {
            Contract.TestUInt256(UInt256.Zero);
            Contract.TestUInt256(null);
        }

        [TestMethod]
        public void TestUInt160Array()
        {
            Contract.TestUInt160Array(new UInt160[] { UInt160.Zero });
            Contract.TestUInt160Array(null);
        }

        [TestMethod]
        public void TestUInt256Array()
        {
            Contract.TestUInt256Array(new UInt256[] { UInt256.Zero });
            Contract.TestUInt256Array(null);
        }

        [TestMethod]
        public void TestByteArray()
        {
            Contract.TestByteArray(new byte[] { 1 });
            Contract.TestByteArray(null);
        }

        [TestMethod]
        public void TestString()
        {
            Contract.TestString("test");
            Contract.TestString(null);
        }

        [TestMethod]
        public void TestNullableString()
        {
            Contract.TestNullableString("test");
            Contract.TestNullableString(null);
        }
    }
}
