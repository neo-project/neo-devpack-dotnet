using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ByteArrayAssignment : TestBase2<Contract_ByteArrayAssignment>
    {
        [TestMethod]
        public void Test_ByteArrayAssignment()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x04 }, Contract.TestAssignment());
            Assert.AreEqual(1724190, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOutOfBounds()
        {
            Assert.ThrowsException<TestException>(Contract.TestAssignmentOutOfBounds);
            Assert.AreEqual(1724070, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOverflow()
        {
            CollectionAssert.AreEqual(new byte[] { 0xff, 0x02, 0x03 }, Contract.TestAssignmentOverflow());
            Assert.AreEqual(1478820, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentWrongCasting()
        {
            var exception = Assert.ThrowsException<TestException>(Contract.TestAssignmentWrongCasting);
            Assert.AreEqual(1478340, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(1478340, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentDynamic()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x0a }, Contract.TestAssignmentDynamic(10));
            Assert.AreEqual(1546590, Engine.FeeConsumed.Value);
        }
    }
}
