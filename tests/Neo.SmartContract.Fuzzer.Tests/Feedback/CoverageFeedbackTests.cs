using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.SmartContract.Fuzzer.Coverage;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Feedback
{
    [TestClass]
    public class CoverageFeedbackTests
    {
        private Mock<CoverageTracker> _mockCoverageTracker;
        private CoverageFeedback _coverageFeedback;
        private readonly int _seed = 12345;

        [TestInitialize]
        public void Setup()
        {
            _mockCoverageTracker = new Mock<CoverageTracker>(null, null, null);
            _coverageFeedback = new CoverageFeedback(_mockCoverageTracker.Object, _seed);
        }

        [TestMethod]
        public void ProcessExecutionResult_NullTestCase_ThrowsArgumentNullException()
        {
            // Arrange
            TestCase testCase = null;
            var result = new ExecutionResult();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _coverageFeedback.ProcessExecutionResult(testCase, result));
        }

        [TestMethod]
        public void ProcessExecutionResult_NullResult_ThrowsArgumentNullException()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "TestMethod" };
            ExecutionResult result = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _coverageFeedback.ProcessExecutionResult(testCase, result));
        }

        [TestMethod]
        public void ProcessExecutionResult_NewMethod_ReturnsFeedbackItem()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "NewMethod" };
            var result = new ExecutionResult { Method = "NewMethod" };

            // Act
            var feedback = _coverageFeedback.ProcessExecutionResult(testCase, result);

            // Assert
            feedback.Should().NotBeNull();
            feedback.Type.Should().Be(FeedbackType.NewCoverage);
            feedback.Description.Should().Contain("New method coverage");
            feedback.RelatedTestCase.Should().NotBeNull();
            feedback.RelatedTestCase.MethodName.Should().Be("NewMethod");
        }

        [TestMethod]
        public void ProcessExecutionResult_ExistingMethod_NoNewCoverage_ReturnsNull()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "ExistingMethod" };
            var result = new ExecutionResult { Method = "ExistingMethod" };

            // First call to add the method to covered methods
            _coverageFeedback.ProcessExecutionResult(testCase, result);

            // Create a mock engine with no new instructions
            var mockEngine = new Mock<ApplicationEngine>();
            result.Engine = mockEngine.Object;

            // Act
            var feedback = _coverageFeedback.ProcessExecutionResult(testCase, result);

            // Assert
            feedback.Should().BeNull();
        }

        [TestMethod]
        public void GetCoverageStatistics_ReturnsExpectedKeys()
        {
            // Act
            var stats = _coverageFeedback.GetCoverageStatistics();

            // Assert
            stats.Should().ContainKeys(
                "MethodsCovered",
                "InstructionsCovered",
                "TotalInstructions",
                "InstructionCoveragePercent",
                "BranchesCovered",
                "TotalBranches",
                "BranchCoveragePercent",
                "UniquePaths"
            );
        }

        [TestMethod]
        public void GetLowCoverageMethods_ReturnsExpectedCount()
        {
            // Arrange
            // Add some methods with different coverage
            var testCase1 = new TestCase { MethodName = "Method1" };
            var testCase2 = new TestCase { MethodName = "Method2" };
            var testCase3 = new TestCase { MethodName = "Method3" };
            
            var result1 = new ExecutionResult { Method = "Method1" };
            var result2 = new ExecutionResult { Method = "Method2" };
            var result3 = new ExecutionResult { Method = "Method3" };
            
            _coverageFeedback.ProcessExecutionResult(testCase1, result1);
            _coverageFeedback.ProcessExecutionResult(testCase2, result2);
            _coverageFeedback.ProcessExecutionResult(testCase3, result3);

            // Act
            var lowCoverageMethods = _coverageFeedback.GetLowCoverageMethods(2);

            // Assert
            lowCoverageMethods.Should().HaveCount(2);
        }

        [TestMethod]
        public void GetUncoveredBranches_ReturnsExpectedCount()
        {
            // Act
            var uncoveredBranches = _coverageFeedback.GetUncoveredBranches(5);

            // Assert
            // Since we haven't added any branches, this should be empty
            uncoveredBranches.Should().BeEmpty();
        }

        [TestMethod]
        public void ProcessExecutionResult_HandlesExceptions_ReturnsNull()
        {
            // Arrange
            var testCase = new TestCase { MethodName = "TestMethod" };
            var result = new ExecutionResult { Method = "TestMethod" };

            // Create a mock engine that throws an exception when accessed
            var mockEngine = new Mock<ApplicationEngine>();
            mockEngine.Setup(e => e.InvocationStack).Throws<InvalidOperationException>();
            result.Engine = mockEngine.Object;

            // Act
            var feedback = _coverageFeedback.ProcessExecutionResult(testCase, result);

            // Assert
            // Should not throw and return null since there's no new coverage
            feedback.Should().BeNull();
        }
    }
}
