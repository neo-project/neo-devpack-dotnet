using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Fuzzer.Controller;
using System;
using FluentAssertions;

namespace Neo.SmartContract.Fuzzer.Tests.Controller
{
    [TestClass]
    public class FuzzingStatusTests
    {
        [TestMethod]
        public void SuccessRate_CalculatesCorrectly()
        {
            // Arrange
            var status = new FuzzingStatus
            {
                TotalExecutions = 100,
                SuccessfulExecutions = 75
            };

            // Act
            double successRate = status.SuccessRate;

            // Assert
            successRate.Should().Be(0.75);
        }

        [TestMethod]
        public void SuccessRate_ZeroTotalExecutions_ReturnsZero()
        {
            // Arrange
            var status = new FuzzingStatus
            {
                TotalExecutions = 0,
                SuccessfulExecutions = 0
            };

            // Act
            double successRate = status.SuccessRate;

            // Assert
            successRate.Should().Be(0);
        }

        [TestMethod]
        public void ExecutionRate_CalculatesCorrectly()
        {
            // Arrange
            var status = new FuzzingStatus
            {
                ElapsedTime = TimeSpan.FromSeconds(10),
                TotalExecutions = 100
            };

            // Act
            double executionRate = status.ExecutionRate;

            // Assert
            executionRate.Should().Be(10);
        }

        [TestMethod]
        public void ExecutionRate_ZeroElapsedTime_ReturnsZero()
        {
            // Arrange
            var status = new FuzzingStatus
            {
                ElapsedTime = TimeSpan.Zero,
                TotalExecutions = 100
            };

            // Act
            double executionRate = status.ExecutionRate;

            // Assert
            executionRate.Should().Be(0);
        }

        [TestMethod]
        public void Properties_SetAndGetCorrectly()
        {
            // Arrange
            var status = new FuzzingStatus();

            // Act
            status.ElapsedTime = TimeSpan.FromSeconds(10);
            status.TotalMethods = 5;
            status.TotalExecutions = 100;
            status.SuccessfulExecutions = 75;
            status.FailedExecutions = 25;
            status.IssuesFound = 10;
            status.CodeCoverage = 0.8;
            status.Errors = 2;

            // Assert
            status.ElapsedTime.Should().Be(TimeSpan.FromSeconds(10));
            status.TotalMethods.Should().Be(5);
            status.TotalExecutions.Should().Be(100);
            status.SuccessfulExecutions.Should().Be(75);
            status.FailedExecutions.Should().Be(25);
            status.IssuesFound.Should().Be(10);
            status.CodeCoverage.Should().Be(0.8);
            status.Errors.Should().Be(2);
        }
    }
}
