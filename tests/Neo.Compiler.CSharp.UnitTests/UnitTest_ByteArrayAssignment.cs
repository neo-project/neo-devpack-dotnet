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
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOutOfBounds()
        {
            Assert.ThrowsException<TestException>(Contract.TestAssignmentOutOfBounds);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOverflow()
        {
            CollectionAssert.AreEqual(new byte[] { 0xff, 0x02, 0x03 }, Contract.TestAssignmentOverflow());
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentWrongCasting()
        {
            var exception = Assert.ThrowsException<TestException>(Contract.TestAssignmentWrongCasting);
            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentDynamic()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x0a }, Contract.TestAssignmentDynamic(10));
        }
    }
}
