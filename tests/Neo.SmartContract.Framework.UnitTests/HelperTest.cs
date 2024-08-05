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
        [TestMethod]
        public void TestHexToBytes()
        {
            // 0a0b0c0d0E0F
            Assert.AreEqual("0a0b0c0d0e0f", Contract.TestHexToBytes()!.ToHexString());
            Assert.AreEqual(985170, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestToBigInteger()
        {
            // 0

            Assert.IsNull(Contract.TestToBigInteger(null));
            Assert.AreEqual(1293870, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, Contract.TestToBigInteger(System.Array.Empty<byte>()));
            Assert.AreEqual(1294080, Engine.FeeConsumed.Value);

            // Value

            Assert.AreEqual(123, Contract.TestToBigInteger(new byte[] { 123 }));
            Assert.AreEqual(1294080, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestModPow()
        {
            Assert.AreEqual(4, Contract.ModMultiply(4, 7, 6));
            Assert.AreEqual(1049250, Engine.FeeConsumed.Value);
            Assert.AreEqual(9, Contract.ModInverse(3, 26));
            Assert.AreEqual(1127070, Engine.FeeConsumed.Value);
            Assert.AreEqual(344, Contract.ModPow(23895, 15, 14189));
            Assert.AreEqual(1109730, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void TestBigIntegerParseandCast()
        {
            Assert.AreEqual(2000000000000000, Contract.TestBigIntegerCast(new byte[] { 0x00, 0x00, 0x8d, 0x49, 0xfd, 0x1a, 0x07 }));
            Assert.AreEqual(1540020, Engine.FeeConsumed.Value);
            var exception = Assert.ThrowsException<TestException>(() => Contract.TestBigIntegerParseHexString("00008d49fd1a07"));
            Assert.AreEqual(2033310, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void TestAssert()
        {
            // With extension
            Assert.AreEqual(5, Contract.AssertCall(true));
            Assert.AreEqual(1049400, Engine.FeeConsumed.Value);
            AssertNoLogs();

            var ex = Assert.ThrowsException<TestException>(() => Contract.AssertCall(false));
            Assert.AreEqual(1049370, Engine.FeeConsumed.Value);
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Test without notification right

            Engine.CallFlags &= ~CallFlags.AllowNotify;
            ex = Assert.ThrowsException<TestException>(() => Contract.AssertCall(false));
            Assert.AreEqual(1049370, Engine.FeeConsumed.Value);
            Engine.CallFlags = CallFlags.All;
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Void With extension

            Contract.VoidAssertCall(true);
            Assert.AreEqual(1049130, Engine.FeeConsumed.Value);
            AssertNoLogs();

            ex = Assert.ThrowsException<TestException>(() => Contract.VoidAssertCall(false));
            Assert.AreEqual(1049130, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01 }, Contract.TestByteToByteArray());
            Assert.AreEqual(1048770, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Reverse()
        {
            CollectionAssert.AreEqual(new byte[] { 0x03, 0x02, 0x01 }, Contract.TestReverse());
            Assert.AreEqual(1478970, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 255 }, Contract.TestSbyteToByteArray());
            Assert.AreEqual(1233060, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 }, Contract.TestStringToByteArray());
            Assert.AreEqual(1233270, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Concat()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, Contract.TestConcat());
            Assert.AreEqual(1540830, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Range()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02 }, Contract.TestRange());
            Assert.AreEqual(1294770, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Take()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02 }, Contract.TestTake());
            Assert.AreEqual(1294740, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Last()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x03 }, Contract.TestLast());
            Assert.AreEqual(1294740, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee }, Contract.TestToScriptHash());
            Assert.AreEqual(985170, Engine.FeeConsumed.Value);
        }
    }
}
