using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Reflection;
using Neo.Extensions;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class HelperTest : TestBase<Contract_Helper>
    {
        public HelperTest() : base(Contract_Helper.Nef, Contract_Helper.Manifest) { }

        [TestMethod]
        public void TestHexToBytes()
        {
            // 0a0b0c0d0E0F
            Assert.AreEqual("0a0b0c0d0e0f", Contract.TestHexToBytes().ToHexString());
        }

        [TestMethod]
        public void TestToBigInteger()
        {
            // 0

            Assert.IsNull(Contract.TestToBigInteger(null));
            Assert.AreEqual(0, Contract.TestToBigInteger(System.Array.Empty<byte>()));

            // Value

            Assert.AreEqual(123, Contract.TestToBigInteger(new byte[] { 123 }));
        }

        [TestMethod]
        public void TestModPow()
        {
            Assert.AreEqual(4, Contract.ModMultiply(4, 7, 6));
            Assert.AreEqual(9, Contract.ModInverse(3, 26));
            Assert.AreEqual(344, Contract.ModPow(23895, 15, 14189));
        }

        [TestMethod]
        public void TestBigIntegerParseandCast()
        {
            Assert.AreEqual(2000000000000000, Contract.TestBigIntegerCast(new byte[] { 0x00, 0x00, 0x8d, 0x49, 0xfd, 0x1a, 0x07 }));
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestBigIntegerParseHexString("00008d49fd1a07"));
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void TestAssert()
        {
            // With extension
            Assert.AreEqual(5, Contract.AssertCall(true));
            AssertNoLogs();

            var ex = Assert.ThrowsException<TestException>(() => Contract.AssertCall(false));
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Test without notification right

            Engine.CallFlags &= ~CallFlags.AllowNotify;
            ex = Assert.ThrowsException<TestException>(() => Contract.AssertCall(false));
            Engine.CallFlags = CallFlags.All;
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Void With extension

            Contract.VoidAssertCall(true);
            AssertNoLogs();

            ex = Assert.ThrowsException<TestException>(() => Contract.VoidAssertCall(false));
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01 }, Contract.TestByteToByteArray());
        }

        [TestMethod]
        public void Test_Reverse()
        {
            CollectionAssert.AreEqual(new byte[] { 0x03, 0x02, 0x01 }, Contract.TestReverse());
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 255 }, Contract.TestSbyteToByteArray());
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 }, Contract.TestStringToByteArray());
        }

        [TestMethod]
        public void Test_Concat()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, Contract.TestConcat());
        }

        [TestMethod]
        public void Test_Range()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02 }, Contract.TestRange());
        }

        [TestMethod]
        public void Test_Take()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02 }, Contract.TestTake());
        }

        [TestMethod]
        public void Test_Last()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x03 }, Contract.TestLast());
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee }, Contract.TestToScriptHash());
        }
    }
}
