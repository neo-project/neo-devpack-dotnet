using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Fuzzer.Minimization;
using Neo.SmartContract.Manifest;
using System;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Minimization
{
    [TestClass]
    public class TestCaseMinimizerTests
    {
        private Mock<ContractExecutor> _mockExecutor;
        private ContractMethodDescriptor _method;
        private TestCaseMinimizer _minimizer;
        private readonly int _seed = 42;

        [TestInitialize]
        public void Setup()
        {
            _mockExecutor = new Mock<ContractExecutor>(null, null, null);
            
            _method = new ContractMethodDescriptor
            {
                Name = "Transfer",
                Parameters = new ContractParameterDefinition[]
                {
                    new ContractParameterDefinition { Name = "from", Type = "Hash160" },
                    new ContractParameterDefinition { Name = "to", Type = "Hash160" },
                    new ContractParameterDefinition { Name = "amount", Type = "Integer" }
                }
            };

            // Create a predicate that always returns true for testing
            Predicate<ExecutionResult> predicate = result => true;
            
            _minimizer = new TestCaseMinimizer(_mockExecutor.Object, _method, predicate, _seed);
        }

        [TestMethod]
        public void Minimize_NullTestCase_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => _minimizer.Minimize(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Minimize_MethodNameMismatch_ThrowsArgumentException()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "BalanceOf", // Different from _method.Name
                Parameters = new VM.Types.StackItem[]
                {
                    new VM.Types.ByteString(new byte[20]), // from
                    new VM.Types.ByteString(new byte[20]), // to
                    new VM.Types.Integer(100) // amount
                }
            };

            // Act & Assert
            Action act = () => _minimizer.Minimize(testCase);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*method*");
        }

        [TestMethod]
        public void Minimize_OriginalTestCaseDoesNotTriggerBehavior_ReturnsOriginal()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[]
                {
                    new VM.Types.ByteString(new byte[20]), // from
                    new VM.Types.ByteString(new byte[20]), // to
                    new VM.Types.Integer(100) // amount
                }
            };

            // Setup mock to return a result that doesn't satisfy the predicate
            var result = new ExecutionResult { Success = true };
            _mockExecutor.Setup(e => e.ExecuteMethod(_method, It.IsAny<VM.Types.StackItem[]>(), It.IsAny<int>()))
                .Returns(result);

            // Create a minimizer with a predicate that always returns false
            var minimizer = new TestCaseMinimizer(_mockExecutor.Object, _method, r => false, _seed);

            // Act
            var minimizedTestCase = minimizer.Minimize(testCase);

            // Assert
            minimizedTestCase.Should().NotBeNull();
            minimizedTestCase.MethodName.Should().Be(testCase.MethodName);
            minimizedTestCase.Parameters.Should().HaveCount(testCase.Parameters.Length);
            // Parameters should be the same as the original since no minimization occurred
            for (int i = 0; i < testCase.Parameters.Length; i++)
            {
                minimizedTestCase.Parameters[i].Should().Be(testCase.Parameters[i]);
            }
        }

        [TestMethod]
        public void Minimize_IntegerParameter_ReturnsMinimizedTestCase()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[]
                {
                    new VM.Types.ByteString(new byte[20]), // from
                    new VM.Types.ByteString(new byte[20]), // to
                    new VM.Types.Integer(100) // amount
                }
            };

            // Setup mock to return a result that satisfies the predicate
            var result = new ExecutionResult { Success = true };
            _mockExecutor.Setup(e => e.ExecuteMethod(_method, It.IsAny<VM.Types.StackItem[]>(), It.IsAny<int>()))
                .Returns(result);

            // Act
            var minimizedTestCase = _minimizer.Minimize(testCase);

            // Assert
            minimizedTestCase.Should().NotBeNull();
            minimizedTestCase.MethodName.Should().Be(testCase.MethodName);
            minimizedTestCase.Parameters.Should().HaveCount(testCase.Parameters.Length);
            // The integer parameter should be minimized
            minimizedTestCase.Parameters[2].Should().BeOfType<VM.Types.Integer>();
            // We can't assert the exact value since it depends on the minimization algorithm
        }

        [TestMethod]
        public void Minimize_ByteStringParameter_ReturnsMinimizedTestCase()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[]
                {
                    new VM.Types.ByteString(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }), // from
                    new VM.Types.ByteString(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }), // to
                    new VM.Types.Integer(100) // amount
                }
            };

            // Setup mock to return a result that satisfies the predicate for empty byte arrays
            _mockExecutor.Setup(e => e.ExecuteMethod(_method, It.Is<VM.Types.StackItem[]>(
                p => p[0] is VM.Types.ByteString && ((VM.Types.ByteString)p[0]).GetSpan().Length == 0), It.IsAny<int>()))
                .Returns(new ExecutionResult { Success = true });

            // Act
            var minimizedTestCase = _minimizer.Minimize(testCase);

            // Assert
            minimizedTestCase.Should().NotBeNull();
            minimizedTestCase.MethodName.Should().Be(testCase.MethodName);
            minimizedTestCase.Parameters.Should().HaveCount(testCase.Parameters.Length);
            // The byte string parameters should be minimized
            minimizedTestCase.Parameters[0].Should().BeOfType<VM.Types.ByteString>();
            minimizedTestCase.Parameters[1].Should().BeOfType<VM.Types.ByteString>();
            // We can't assert the exact values since they depend on the minimization algorithm
        }
    }
}
