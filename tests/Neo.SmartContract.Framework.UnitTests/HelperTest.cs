using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class HelperTest : DebugAndTestBase<Contract_Helper>
    {
        [TestMethod]
        public void TestHexToBytes()
        {
            // 0a0b0c0d0E0F
            Assert.AreEqual("0a0b0c0d0e0f", Contract.TestHexToBytes()!.ToHexString());
            AssertGasConsumed(985170);
        }

        [TestMethod]
        public void TestToBigInteger()
        {
            // 0

            Assert.IsNull(Contract.TestToBigInteger(null));
            AssertGasConsumed(1293870);
            Assert.AreEqual(0, Contract.TestToBigInteger([]));
            AssertGasConsumed(1294080);

            // Value

            Assert.AreEqual(123, Contract.TestToBigInteger([123]));
            AssertGasConsumed(1294080);
        }

        [TestMethod]
        public void TestModPow()
        {
            Assert.AreEqual(4, Contract.ModMultiply(4, 7, 6));
            AssertGasConsumed(1049250);
            Assert.AreEqual(9, Contract.ModInverse(3, 26));
            AssertGasConsumed(1127070);
            Assert.AreEqual(344, Contract.ModPow(23895, 15, 14189));
            AssertGasConsumed(1109730);
        }

        [TestMethod]
        public void TestBigIntegerParseandCast()
        {
            Assert.AreEqual(2000000000000000, Contract.TestBigIntegerCast([0x00, 0x00, 0x8d, 0x49, 0xfd, 0x1a, 0x07]));
            AssertGasConsumed(1540020);
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestBigIntegerParseHexString("00008d49fd1a07"));
            AssertGasConsumed(2033310);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void TestAssert()
        {
            // With extension
            Assert.AreEqual(5, Contract.AssertCall(true));
            AssertGasConsumed(1049400);
            AssertNoLogs();

            var ex = Assert.ThrowsException<TestException>(() => Contract.AssertCall(false));
            AssertGasConsumed(1049370);
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Test without notification right

            Engine.CallFlags &= ~CallFlags.AllowNotify;
            ex = Assert.ThrowsException<TestException>(() => Contract.AssertCall(false));
            AssertGasConsumed(1049370);
            Engine.CallFlags = CallFlags.All;
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Void With extension

            Contract.VoidAssertCall(true);
            AssertGasConsumed(1049130);
            AssertNoLogs();

            ex = Assert.ThrowsException<TestException>(() => Contract.VoidAssertCall(false));
            AssertGasConsumed(1049130);
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01 }, Contract.TestByteToByteArray());
            AssertGasConsumed(1048770);
        }

        [TestMethod]
        public void Test_Reverse()
        {
            CollectionAssert.AreEqual(new byte[] { 0x03, 0x02, 0x01 }, Contract.TestReverse());
            AssertGasConsumed(1478970);
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 255 }, Contract.TestSbyteToByteArray());
            AssertGasConsumed(1233060);
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            CollectionAssert.AreEqual("hello world"u8.ToArray(), Contract.TestStringToByteArray());
            AssertGasConsumed(1233270);
        }

        [TestMethod]
        public void Test_Concat()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, Contract.TestConcat());
            AssertGasConsumed(1540830);
        }

        [TestMethod]
        public void Test_Range()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02 }, Contract.TestRange());
            AssertGasConsumed(1294770);
        }

        [TestMethod]
        public void Test_Take()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02 }, Contract.TestTake());
            AssertGasConsumed(1294740);
        }

        [TestMethod]
        public void Test_Last()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x03 }, Contract.TestLast());
            AssertGasConsumed(1294740);
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee }, Contract.TestToScriptHash());
            AssertGasConsumed(985170);
        }
    }
}
