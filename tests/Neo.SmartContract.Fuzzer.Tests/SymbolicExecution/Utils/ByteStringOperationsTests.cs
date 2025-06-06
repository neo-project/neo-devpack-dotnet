using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Types;
using Neo.SmartContract.Fuzzer.SymbolicExecution.Utils;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Fuzzer.Tests.SymbolicExecution.Utils
{
    /// <summary>
    /// Tests for the ByteStringOperations utility class.
    /// </summary>
    [TestClass]
    public class ByteStringOperationsTests
    {
        /// <summary>
        /// Tests the AreEqual method with equal ByteStrings.
        /// </summary>
        [TestMethod]
        public void AreEqual_WithEqualByteStrings_ReturnsTrue()
        {
            // Arrange
            var bytes1 = new ByteString(new byte[] { 1, 2, 3 });
            var bytes2 = new ByteString(new byte[] { 1, 2, 3 });

            // Act
            bool result = ByteStringOperations.AreEqual(bytes1, bytes2);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests the AreEqual method with different ByteStrings.
        /// </summary>
        [TestMethod]
        public void AreEqual_WithDifferentByteStrings_ReturnsFalse()
        {
            // Arrange
            var bytes1 = new ByteString(new byte[] { 1, 2, 3 });
            var bytes2 = new ByteString(new byte[] { 1, 2, 4 });

            // Act
            bool result = ByteStringOperations.AreEqual(bytes1, bytes2);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests the AreEqual method with null ByteStrings.
        /// </summary>
        [TestMethod]
        public void AreEqual_WithNullByteStrings_HandlesNullsCorrectly()
        {
            // Arrange
            ByteString bytes1 = null;
            ByteString bytes2 = null;
            var bytes3 = new ByteString(new byte[] { 1, 2, 3 });

            // Act & Assert
            Assert.IsTrue(ByteStringOperations.AreEqual(bytes1, bytes2));
            Assert.IsFalse(ByteStringOperations.AreEqual(bytes1, bytes3));
            Assert.IsFalse(ByteStringOperations.AreEqual(bytes3, bytes1));
        }

        /// <summary>
        /// Tests the ToHexString method.
        /// </summary>
        [TestMethod]
        public void ToHexString_WithByteString_ReturnsCorrectHexString()
        {
            // Arrange
            var bytes = new ByteString(new byte[] { 0x1A, 0x2B, 0x3C });

            // Act
            string result = ByteStringOperations.ToHexString(bytes);

            // Assert
            Assert.AreEqual("1a2b3c", result);
        }

        /// <summary>
        /// Tests the ToHexString method with null ByteString.
        /// </summary>
        [TestMethod]
        public void ToHexString_WithNullByteString_ReturnsNullString()
        {
            // Arrange
            ByteString bytes = null;

            // Act
            string result = ByteStringOperations.ToHexString(bytes);

            // Assert
            Assert.AreEqual("null", result);
        }

        /// <summary>
        /// Tests the TryGetByteString method.
        /// </summary>
        [TestMethod]
        public void TryGetByteString_WithConcreteByteStringValue_ReturnsTrue()
        {
            // Arrange
            var bytes = new ByteString(new byte[] { 1, 2, 3 });
            var value = new ConcreteValue<ByteString>(bytes);

            // Act
            bool result = ByteStringOperations.TryGetByteString(value, out var resultBytes);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(bytes, resultBytes);
        }

        /// <summary>
        /// Tests the TryGetByteString method with non-ByteString value.
        /// </summary>
        [TestMethod]
        public void TryGetByteString_WithNonByteStringValue_ReturnsFalse()
        {
            // Arrange
            var value = new ConcreteValue<int>(42);

            // Act
            bool result = ByteStringOperations.TryGetByteString(value, out var resultBytes);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(resultBytes);
        }

        /// <summary>
        /// Tests the TryGetInteger method with valid ByteString.
        /// </summary>
        [TestMethod]
        public void TryGetInteger_WithValidByteString_ReturnsTrue()
        {
            // Arrange
            var bytes = new ByteString(new byte[] { 42, 0, 0, 0 }); // 42 in little-endian

            // Act
            bool result = ByteStringOperations.TryGetInteger(bytes, out var resultInt);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(42, resultInt);
        }

        /// <summary>
        /// Tests the TryGetInteger method with empty ByteString.
        /// </summary>
        [TestMethod]
        public void TryGetInteger_WithEmptyByteString_ReturnsZero()
        {
            // Arrange
            var bytes = ByteString.Empty;

            // Act
            bool result = ByteStringOperations.TryGetInteger(bytes, out var resultInt);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, resultInt);
        }

        /// <summary>
        /// Tests the TryGetBoolean method with true ByteString.
        /// </summary>
        [TestMethod]
        public void TryGetBoolean_WithNonZeroByteString_ReturnsTrue()
        {
            // Arrange
            var bytes = new ByteString(new byte[] { 1 });

            // Act
            bool result = ByteStringOperations.TryGetBoolean(bytes, out var resultBool);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(resultBool);
        }

        /// <summary>
        /// Tests the TryGetBoolean method with false ByteString.
        /// </summary>
        [TestMethod]
        public void TryGetBoolean_WithZeroByteString_ReturnsFalse()
        {
            // Arrange
            var bytes = new ByteString(new byte[] { 0 });

            // Act
            bool result = ByteStringOperations.TryGetBoolean(bytes, out var resultBool);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(resultBool);
        }

        /// <summary>
        /// Tests the FromInteger method.
        /// </summary>
        [TestMethod]
        public void FromInteger_WithPositiveInteger_ReturnsCorrectByteString()
        {
            // Arrange
            int value = 42;

            // Act
            var result = ByteStringOperations.FromInteger(value);

            // Assert
            Assert.AreEqual(1, result.GetSpan().Length);
            Assert.AreEqual(42, result.GetSpan()[0]);
        }

        /// <summary>
        /// Tests the FromInteger method with zero.
        /// </summary>
        [TestMethod]
        public void FromInteger_WithZero_ReturnsEmptyByteString()
        {
            // Arrange
            int value = 0;

            // Act
            var result = ByteStringOperations.FromInteger(value);

            // Assert
            Assert.AreEqual(0, result.GetSpan().Length);
            Assert.AreEqual(ByteString.Empty, result);
        }

        /// <summary>
        /// Tests the FromBoolean method with true.
        /// </summary>
        [TestMethod]
        public void FromBoolean_WithTrue_ReturnsByteString()
        {
            // Arrange
            bool value = true;

            // Act
            var result = ByteStringOperations.FromBoolean(value);

            // Assert
            Assert.AreEqual(1, result.GetSpan().Length);
            Assert.AreEqual(1, result.GetSpan()[0]);
        }

        /// <summary>
        /// Tests the FromBoolean method with false.
        /// </summary>
        [TestMethod]
        public void FromBoolean_WithFalse_ReturnsEmptyByteString()
        {
            // Arrange
            bool value = false;

            // Act
            var result = ByteStringOperations.FromBoolean(value);

            // Assert
            Assert.AreEqual(0, result.GetSpan().Length);
            Assert.AreEqual(ByteString.Empty, result);
        }

        /// <summary>
        /// Tests the CreateByteStringComparison method.
        /// </summary>
        [TestMethod]
        public void CreateByteStringComparison_WithEqualOperator_CreatesSymbolicExpression()
        {
            // Arrange
            var bytes = new ByteString(new byte[] { 1, 2, 3 });
            var symbolicVar = new SymbolicVariable("testVar");

            // Act
            var result = ByteStringOperations.CreateByteStringComparison(bytes, Operator.Equal, symbolicVar);

            // Assert
            Assert.IsInstanceOfType(result, typeof(SymbolicExpression));
            Assert.AreEqual(Operator.Equal, ((SymbolicExpression)result).Op);
            Assert.AreEqual(2, ((SymbolicExpression)result).Operands.Length);
            
            var leftOperand = ((SymbolicExpression)result).Operands[0];
            Assert.IsInstanceOfType(leftOperand, typeof(ConcreteValue<ByteString>));
            Assert.AreEqual(bytes, ((ConcreteValue<ByteString>)leftOperand).Value);
            
            var rightOperand = ((SymbolicExpression)result).Operands[1];
            Assert.IsInstanceOfType(rightOperand, typeof(SymbolicVariable));
            Assert.AreEqual("testVar", ((SymbolicVariable)rightOperand).Name);
        }
    }
}