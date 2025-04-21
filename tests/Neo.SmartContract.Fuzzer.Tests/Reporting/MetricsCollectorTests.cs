using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.SmartContract.Fuzzer.Reporting;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using Xunit;

namespace Neo.SmartContract.Fuzzer.Tests.Reporting
{
    /// <summary>
    /// Tests for the MetricsCollector class
    /// </summary>
    public class MetricsCollectorTests
    {
        private readonly string _testDataPath = Path.Combine("TestData");
        private readonly string _manifestPath;

        /// <summary>
        /// Initialize test data
        /// </summary>
        public MetricsCollectorTests()
        {
            _manifestPath = Path.Combine(_testDataPath, "SampleToken.manifest.json");
        }

        /// <summary>
        /// Test collection of basic execution metrics
        /// </summary>
        [Fact]
        public void BasicMetricsCollectionTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            var results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Parameters = new StackItem[] { new ByteString(new byte[20]), new ByteString(new byte[20]), new Integer(100) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(50),
                    GasConsumed = 1000,
                    ReturnValue = StackItem.True
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Parameters = new StackItem[] { new ByteString(new byte[20]), new ByteString(new byte[20]), new Integer(200) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(60),
                    GasConsumed = 1200,
                    ReturnValue = StackItem.True
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Parameters = new StackItem[] { new ByteString(new byte[20]), new ByteString(new byte[20]), new Integer(-100) },
                    Success = false,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(30),
                    ErrorMessage = "Negative amount"
                },
                new ExecutionResult
                {
                    MethodName = "balanceOf",
                    Parameters = new StackItem[] { new ByteString(new byte[20]) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(20),
                    GasConsumed = 500,
                    ReturnValue = new Integer(1000)
                }
            };
            
            // Act
            var metricsCollector = new MetricsCollector(results, manifest.Abi.Methods);
            var overallMetrics = metricsCollector.GetOverallMetrics();
            var methodMetrics = metricsCollector.GetAllMethodMetrics();
            
            // Assert - Overall metrics
            Assert.Equal(4, overallMetrics.TotalExecutions);
            Assert.Equal(3, overallMetrics.SuccessfulExecutions);
            Assert.Equal(1, overallMetrics.FailedExecutions);
            Assert.Equal(0.75, overallMetrics.SuccessRate);
            Assert.Equal(900, overallMetrics.AverageGasUsage);
            Assert.Equal(500, overallMetrics.MinGasUsage);
            Assert.Equal(1200, overallMetrics.MaxGasUsage);
            Assert.True(overallMetrics.AverageExecutionTime > 0);
            
            // Assert - Method metrics
            Assert.Equal(2, methodMetrics.Count);
            
            // Transfer method
            var transferMetrics = methodMetrics["transfer"];
            Assert.Equal("transfer", transferMetrics.MethodName);
            Assert.Equal(3, transferMetrics.TotalExecutions);
            Assert.Equal(2, transferMetrics.SuccessfulExecutions);
            Assert.Equal(1, transferMetrics.FailedExecutions);
            Assert.Equal(2.0/3.0, transferMetrics.SuccessRate);
            Assert.Equal(1100, transferMetrics.AverageGasUsage);
            Assert.Equal(1000, transferMetrics.MinGasUsage);
            Assert.Equal(1200, transferMetrics.MaxGasUsage);
            
            // BalanceOf method
            var balanceOfMetrics = methodMetrics["balanceOf"];
            Assert.Equal("balanceOf", balanceOfMetrics.MethodName);
            Assert.Equal(1, balanceOfMetrics.TotalExecutions);
            Assert.Equal(1, balanceOfMetrics.SuccessfulExecutions);
            Assert.Equal(0, balanceOfMetrics.FailedExecutions);
            Assert.Equal(1.0, balanceOfMetrics.SuccessRate);
            Assert.Equal(500, balanceOfMetrics.AverageGasUsage);
            Assert.Equal(500, balanceOfMetrics.MinGasUsage);
            Assert.Equal(500, balanceOfMetrics.MaxGasUsage);
        }

        /// <summary>
        /// Test error categorization
        /// </summary>
        [Fact]
        public void ErrorCategorizationTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            var results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "gas limit exceeded"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "invalid jump destination"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "arithmetic operation overflow"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "index out of range"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "null reference exception"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "invalid cast from Integer to ByteString"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "execution fault: unknown error"
                },
                new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = false,
                    ErrorMessage = "some other error"
                }
            };
            
            // Act
            var metricsCollector = new MetricsCollector(results, manifest.Abi.Methods);
            var overallMetrics = metricsCollector.GetOverallMetrics();
            var transferMetrics = metricsCollector.GetMethodMetrics("transfer");
            
            // Assert
            Assert.Equal(8, overallMetrics.FailedExecutions);
            Assert.Equal(8, overallMetrics.ErrorTypes.Values.Sum());
            
            // Check error categorization
            Assert.Equal(1, overallMetrics.ErrorTypes["OUT_OF_GAS"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["INVALID_JUMP"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["ARITHMETIC_ERROR"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["INDEX_OUT_OF_RANGE"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["NULL_REFERENCE"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["TYPE_ERROR"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["FAULT_EXCEPTION"]);
            Assert.Equal(1, overallMetrics.ErrorTypes["OTHER"]);
            
            // Check method-specific error categorization
            Assert.Equal(8, transferMetrics.ErrorTypes.Values.Sum());
        }

        /// <summary>
        /// Test calculation of derived metrics
        /// </summary>
        [Fact]
        public void DerivedMetricsCalculationTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            var results = new List<ExecutionResult>();
            
            // Add 100 results for the transfer method with varying gas usage
            var random = new Random(12345);
            for (int i = 0; i < 100; i++)
            {
                long gas = 1000 + random.Next(0, 1000);
                results.Add(new ExecutionResult
                {
                    MethodName = "transfer",
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(random.Next(10, 100)),
                    GasConsumed = gas,
                    ReturnValue = StackItem.True
                });
            }
            
            // Act
            var metricsCollector = new MetricsCollector(results, manifest.Abi.Methods);
            var transferMetrics = metricsCollector.GetMethodMetrics("transfer");
            
            // Assert
            Assert.Equal(100, transferMetrics.TotalExecutions);
            Assert.Equal(100, transferMetrics.SuccessfulExecutions);
            Assert.Equal(0, transferMetrics.FailedExecutions);
            Assert.Equal(1.0, transferMetrics.SuccessRate);
            
            // Check gas usage statistics
            Assert.True(transferMetrics.AverageGasUsage >= 1000);
            Assert.True(transferMetrics.AverageGasUsage <= 2000);
            Assert.True(transferMetrics.MinGasUsage >= 1000);
            Assert.True(transferMetrics.MaxGasUsage <= 2000);
            Assert.True(transferMetrics.GasUsageStdDev > 0);
            
            // Check that standard deviation is reasonable
            Assert.True(transferMetrics.GasUsageStdDev < (transferMetrics.MaxGasUsage - transferMetrics.MinGasUsage));
        }

        /// <summary>
        /// Test handling of methods not in the manifest
        /// </summary>
        [Fact]
        public void UnknownMethodHandlingTest()
        {
            // Arrange
            var manifestJson = File.ReadAllText(_manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);
            
            var results = new List<ExecutionResult>
            {
                new ExecutionResult
                {
                    MethodName = "unknownMethod",
                    Parameters = new StackItem[] { new Integer(123) },
                    Success = true,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddMilliseconds(50),
                    GasConsumed = 1000,
                    ReturnValue = StackItem.True
                }
            };
            
            // Act
            var metricsCollector = new MetricsCollector(results, manifest.Abi.Methods);
            var methodMetrics = metricsCollector.GetAllMethodMetrics();
            var unknownMethodMetrics = metricsCollector.GetMethodMetrics("unknownMethod");
            
            // Assert
            Assert.NotNull(unknownMethodMetrics);
            Assert.Equal("unknownMethod", unknownMethodMetrics.MethodName);
            Assert.Equal(1, unknownMethodMetrics.ParameterCount);
            Assert.Equal("Unknown", unknownMethodMetrics.ReturnType);
            Assert.False(unknownMethodMetrics.IsSafe);
            Assert.Equal(1, unknownMethodMetrics.TotalExecutions);
            Assert.Equal(1, unknownMethodMetrics.SuccessfulExecutions);
            Assert.Equal(0, unknownMethodMetrics.FailedExecutions);
            Assert.Equal(1.0, unknownMethodMetrics.SuccessRate);
        }
    }
}