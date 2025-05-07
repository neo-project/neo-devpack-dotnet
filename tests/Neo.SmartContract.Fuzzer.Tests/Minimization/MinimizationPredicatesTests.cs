using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Minimization;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Minimization
{
    [TestClass]
    public class MinimizationPredicatesTests
    {
        [TestMethod]
        public void FailsExecution_ExecutionFailed_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult { Success = false };
            var predicate = MinimizationPredicates.FailsExecution();

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void FailsExecution_ExecutionSucceeded_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult { Success = true };
            var predicate = MinimizationPredicates.FailsExecution();

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void FailsWithException_MatchingExceptionType_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new ArgumentException("Test exception")
            };
            var predicate = MinimizationPredicates.FailsWithException(typeof(ArgumentException));

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void FailsWithException_DifferentExceptionType_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new ArgumentException("Test exception")
            };
            var predicate = MinimizationPredicates.FailsWithException(typeof(InvalidOperationException));

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void FailsWithExceptionMessage_MatchingMessage_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception with specific pattern")
            };
            var predicate = MinimizationPredicates.FailsWithExceptionMessage("specific pattern");

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void FailsWithExceptionMessage_NonMatchingMessage_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception with specific pattern")
            };
            var predicate = MinimizationPredicates.FailsWithExceptionMessage("different pattern");

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void ConsumesMoreGasThan_GasAboveThreshold_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                FeeConsumed = 1000
            };
            var predicate = MinimizationPredicates.ConsumesMoreGasThan(500);

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void ConsumesMoreGasThan_GasBelowThreshold_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                FeeConsumed = 1000
            };
            var predicate = MinimizationPredicates.ConsumesMoreGasThan(1500);

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void EmitsNotification_MatchingNotification_ReturnsTrue()
        {
            // Arrange
            var engine = new TestApplicationEngine();
            engine.Notifications.Add(new NotifyEventArgs(UInt160.Zero, "Transfer", new VM.Types.Array()));
            
            var result = new ExecutionResult
            {
                Engine = engine
            };
            var predicate = MinimizationPredicates.EmitsNotification("Transfer");

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void EmitsNotification_NoMatchingNotification_ReturnsFalse()
        {
            // Arrange
            var engine = new TestApplicationEngine();
            engine.Notifications.Add(new NotifyEventArgs(UInt160.Zero, "Transfer", new VM.Types.Array()));
            
            var result = new ExecutionResult
            {
                Engine = engine
            };
            var predicate = MinimizationPredicates.EmitsNotification("Mint");

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void ReturnsValue_MatchingValue_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = true,
                ReturnValue = new VM.Types.Integer(42)
            };
            var predicate = MinimizationPredicates.ReturnsValue(42);

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void ReturnsValue_NonMatchingValue_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = true,
                ReturnValue = new VM.Types.Integer(42)
            };
            var predicate = MinimizationPredicates.ReturnsValue(100);

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void ModifiesStorage_MatchingKey_ReturnsTrue()
        {
            // Arrange
            var key = new byte[] { 1, 2, 3 };
            var result = new ExecutionResult
            {
                StorageChanges = new Dictionary<byte[], byte[]>(new ByteArrayEqualityComparer())
                {
                    { key, new byte[] { 4, 5, 6 } }
                }
            };
            var predicate = MinimizationPredicates.ModifiesStorage(key);

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void ModifiesStorage_NonMatchingKey_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                StorageChanges = new Dictionary<byte[], byte[]>(new ByteArrayEqualityComparer())
                {
                    { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } }
                }
            };
            var predicate = MinimizationPredicates.ModifiesStorage(new byte[] { 7, 8, 9 });

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void And_AllPredicatesTrue_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception")
            };
            var predicate = MinimizationPredicates.And(
                MinimizationPredicates.FailsExecution(),
                MinimizationPredicates.FailsWithExceptionMessage("Test")
            );

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void And_OnePredicateFalse_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception")
            };
            var predicate = MinimizationPredicates.And(
                MinimizationPredicates.FailsExecution(),
                MinimizationPredicates.FailsWithExceptionMessage("Different")
            );

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void Or_OnePredicateTrue_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception")
            };
            var predicate = MinimizationPredicates.Or(
                MinimizationPredicates.FailsExecution(),
                MinimizationPredicates.FailsWithExceptionMessage("Different")
            );

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }

        [TestMethod]
        public void Or_AllPredicatesFalse_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = true
            };
            var predicate = MinimizationPredicates.Or(
                MinimizationPredicates.FailsExecution(),
                MinimizationPredicates.FailsWithExceptionMessage("Test")
            );

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void Not_PredicateTrue_ReturnsFalse()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = false
            };
            var predicate = MinimizationPredicates.Not(MinimizationPredicates.FailsExecution());

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeFalse();
        }

        [TestMethod]
        public void Not_PredicateFalse_ReturnsTrue()
        {
            // Arrange
            var result = new ExecutionResult
            {
                Success = true
            };
            var predicate = MinimizationPredicates.Not(MinimizationPredicates.FailsExecution());

            // Act
            bool matches = predicate(result);

            // Assert
            matches.Should().BeTrue();
        }
    }

    // Helper class for testing
    public class TestApplicationEngine
    {
        public List<NotifyEventArgs> Notifications { get; } = new List<NotifyEventArgs>();
    }

    // Helper class for testing
    public class NotifyEventArgs
    {
        public UInt160 ScriptHash { get; }
        public string EventName { get; }
        public VM.Types.Array State { get; }

        public NotifyEventArgs(UInt160 scriptHash, string eventName, VM.Types.Array state)
        {
            ScriptHash = scriptHash;
            EventName = eventName;
            State = state;
        }
    }

    // Helper class for testing
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            if (x.Length != y.Length)
                return false;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                    return false;
            }
            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            if (obj == null)
                return 0;
            int hash = 17;
            foreach (byte b in obj)
            {
                hash = hash * 31 + b;
            }
            return hash;
        }
    }
}
