using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ByteArrayAssignment : TestBase<Contract_ByteArrayAssignment>
    {
        public UnitTest_ByteArrayAssignment() : base(Contract_ByteArrayAssignment.Nef, Contract_ByteArrayAssignment.Manifest) { }

        [TestMethod]
        public void Test_ByteArrayAssignment()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x04 }, Contract.TestAssignment());
            Assert.AreEqual(1002784930, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOutOfBounds()
        {
            Assert.ThrowsException<TestException>(Contract.TestAssignmentOutOfBounds);
            Assert.AreEqual(1002784750, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOverflow()
        {
            CollectionAssert.AreEqual(new byte[] { 0xff, 0x02, 0x03 }, Contract.TestAssignmentOverflow());
            Assert.AreEqual(1002539560, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentWrongCasting()
        {
            var exception = Assert.ThrowsException<TestException>(Contract.TestAssignmentWrongCasting);
            Assert.AreEqual(1002539020, Engine.FeeConsumed.Value);
            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(1002539020, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentDynamic()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x0a }, Contract.TestAssignmentDynamic(10));
            Assert.AreEqual(1002607330, Engine.FeeConsumed.Value);
        }
    }
}
