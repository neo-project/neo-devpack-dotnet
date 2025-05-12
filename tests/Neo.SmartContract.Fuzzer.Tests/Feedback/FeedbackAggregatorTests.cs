using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Feedback;
using Neo.SmartContract.Fuzzer.StaticAnalysis;
using System;
using System.Linq;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Feedback
{
    [TestClass]
    public class FeedbackAggregatorTests
    {
        private FeedbackAggregator _feedbackAggregator;
        private readonly int _seed = 42;

        [TestInitialize]
        public void Setup()
        {
            _feedbackAggregator = new FeedbackAggregator(_seed);
        }

        [TestMethod]
        public void AddExecutionFeedback_CrashDetected_ReturnsTrue()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[] { new VM.Types.Integer(100) },
                Energy = 1.0,
                Iteration = 1
            };

            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception")
            };

            // Act
            bool isInteresting = _feedbackAggregator.AddExecutionFeedback(testCase, result);

            // Assert
            isInteresting.Should().BeTrue();
        }

        [TestMethod]
        public void AddExecutionFeedback_NewMethodCoverage_ReturnsTrue()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[] { new VM.Types.Integer(100) },
                Energy = 1.0,
                Iteration = 1
            };

            var result = new ExecutionResult
            {
                Success = true
            };

            // Act
            bool isInteresting = _feedbackAggregator.AddExecutionFeedback(testCase, result);

            // Assert
            isInteresting.Should().BeTrue();
        }

        [TestMethod]
        public void AddExecutionFeedback_DuplicateMethodCoverage_ReturnsFalse()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[] { new VM.Types.Integer(100) },
                Energy = 1.0,
                Iteration = 1
            };

            var result = new ExecutionResult
            {
                Success = true
            };

            // First call should be interesting
            _feedbackAggregator.AddExecutionFeedback(testCase, result);

            // Act - Second call with same method
            bool isInteresting = _feedbackAggregator.AddExecutionFeedback(testCase, result);

            // Assert
            isInteresting.Should().BeFalse();
        }

        [TestMethod]
        public void AddStaticAnalysisHint_AddsHintToFeedback()
        {
            // Arrange
            var hint = new StaticAnalysisHint
            {
                FilePath = "test.cs",
                LineNumber = 10,
                RiskType = "TestRisk",
                Description = "Test description",
                Priority = 50,
                MethodName = "TestMethod"
            };

            // Act
            _feedbackAggregator.AddStaticAnalysisHint(hint);
            var feedback = _feedbackAggregator.GetNextFeedback();

            // Assert
            feedback.Should().NotBeNull();
            feedback.Type.Should().Be(FeedbackType.StaticHint);
            feedback.StaticHint.Should().NotBeNull();
            feedback.StaticHint.RiskType.Should().Be("TestRisk");
            feedback.Priority.Should().Be(50);
        }

        [TestMethod]
        public void GetCoverageStatistics_ReturnsCorrectStatistics()
        {
            // Arrange
            var testCase1 = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[] { new VM.Types.Integer(100) },
                Energy = 1.0,
                Iteration = 1
            };

            var testCase2 = new TestCase
            {
                MethodName = "BalanceOf",
                Parameters = new VM.Types.StackItem[] { new VM.Types.ByteString(new byte[] { 1, 2, 3 }) },
                Energy = 1.0,
                Iteration = 2
            };

            var result = new ExecutionResult
            {
                Success = true
            };

            // Act
            _feedbackAggregator.AddExecutionFeedback(testCase1, result);
            _feedbackAggregator.AddExecutionFeedback(testCase2, result);
            var stats = _feedbackAggregator.GetCoverageStatistics();

            // Assert
            stats.Should().ContainKey("MethodsCovered");
            stats["MethodsCovered"].Should().Be(2);
            stats.Should().ContainKey("TotalMethodCalls");
            stats["TotalMethodCalls"].Should().Be(2);
        }

        [TestMethod]
        public void GetAllFeedback_ReturnsAllFeedbackItems()
        {
            // Arrange
            var testCase = new TestCase
            {
                MethodName = "Transfer",
                Parameters = new VM.Types.StackItem[] { new VM.Types.Integer(100) },
                Energy = 1.0,
                Iteration = 1
            };

            var result = new ExecutionResult
            {
                Success = false,
                Exception = new Exception("Test exception")
            };

            var hint = new StaticAnalysisHint
            {
                FilePath = "test.cs",
                LineNumber = 10,
                RiskType = "TestRisk",
                Description = "Test description",
                Priority = 50,
                MethodName = "TestMethod"
            };

            // Act
            _feedbackAggregator.AddExecutionFeedback(testCase, result);
            _feedbackAggregator.AddStaticAnalysisHint(hint);
            var feedback = _feedbackAggregator.GetAllFeedback();

            // Assert
            feedback.Should().HaveCount(2);
            feedback.Should().Contain(f => f.Type == FeedbackType.Crash);
            feedback.Should().Contain(f => f.Type == FeedbackType.StaticHint);
        }
    }
}
