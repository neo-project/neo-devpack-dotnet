using System;
using System.Linq;
using System.Threading;
using Neo.SmartContract.Deploy.Monitoring;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Monitoring;

public class DeploymentMetricsTests : IDisposable
{
    private readonly DeploymentMetrics _metrics;

    public DeploymentMetricsTests()
    {
        _metrics = new DeploymentMetrics(TimeSpan.FromHours(1)); // Short retention for tests
    }

    [Fact]
    public void RecordDeployment_Success_RecordsCorrectly()
    {
        // Arrange
        const string contractName = "TestContract";
        const string contractHash = "0x123456";
        var duration = TimeSpan.FromSeconds(5);

        // Act
        var deploymentId = _metrics.RecordDeployment(contractName, contractHash, true, duration);

        // Assert
        Assert.NotNull(deploymentId);
        Assert.NotEmpty(deploymentId);
        
        var stats = _metrics.GetStatistics();
        Assert.Equal(1, stats.TotalDeployments);
        Assert.Equal(1, stats.SuccessfulDeployments);
        Assert.Equal(0, stats.FailedDeployments);
    }

    [Fact]
    public void RecordDeployment_Failure_RecordsCorrectly()
    {
        // Arrange
        const string contractName = "TestContract";
        const string contractHash = "0x123456";
        var duration = TimeSpan.FromSeconds(2);
        const string error = "Deployment failed";

        // Act
        var deploymentId = _metrics.RecordDeployment(contractName, contractHash, false, duration, error);

        // Assert
        var stats = _metrics.GetStatistics();
        Assert.Equal(1, stats.TotalDeployments);
        Assert.Equal(0, stats.SuccessfulDeployments);
        Assert.Equal(1, stats.FailedDeployments);
    }

    [Fact]
    public void RecordInvocation_Success_UpdatesMetrics()
    {
        // Arrange
        const string contractHash = "0x123456";
        const string method = "transfer";
        var duration = TimeSpan.FromMilliseconds(100);

        // Act
        _metrics.RecordInvocation(contractHash, method, true, duration);

        // Assert
        var stats = _metrics.GetStatistics();
        Assert.Equal(1, stats.TotalInvocations);
        Assert.Equal(1, stats.SuccessfulInvocations);
    }

    [Fact]
    public void RecordMetric_CustomMetric_StoresCorrectly()
    {
        // Arrange
        const string category = "custom";
        const string name = "metric1";
        const double value = 42.5;

        // Act
        _metrics.RecordMetric(category, name, value);

        // Assert
        var metrics = _metrics.GetMetricsByCategory(category);
        Assert.Single(metrics);
        
        var metric = metrics.Values.First();
        Assert.Equal(1, metric.Count);
        Assert.Equal(value, metric.Value);
        Assert.Equal(value, metric.TotalValue);
    }

    [Fact]
    public void StartTimer_AutomaticRecording_RecordsElapsedTime()
    {
        // Arrange
        const string operationName = "test_operation";

        // Act
        using (var timer = _metrics.StartTimer(operationName))
        {
            Thread.Sleep(50); // Simulate work
        }

        // Assert
        var metrics = _metrics.GetMetricsByCategory("operation");
        Assert.Single(metrics);
        
        var metric = metrics.Values.First();
        Assert.True(metric.Value >= 50); // Should be at least 50ms
    }

    [Fact]
    public void GetStatistics_MultipleDeployments_CalculatesCorrectly()
    {
        // Arrange & Act
        _metrics.RecordDeployment("Contract1", "0x111", true, TimeSpan.FromSeconds(5));
        _metrics.RecordDeployment("Contract2", "0x222", true, TimeSpan.FromSeconds(7));
        _metrics.RecordDeployment("Contract3", "0x333", false, TimeSpan.FromSeconds(3), "Error");

        // Assert
        var stats = _metrics.GetStatistics();
        Assert.Equal(3, stats.TotalDeployments);
        Assert.Equal(2, stats.SuccessfulDeployments);
        Assert.Equal(1, stats.FailedDeployments);
        Assert.Equal(TimeSpan.FromSeconds(5), stats.AverageDeploymentTime); // (5+7+3)/3 = 5
        Assert.Equal(0.67, Math.Round(stats.DeploymentSuccessRate, 2));
    }

    [Fact]
    public void GetDeploymentHistory_ReturnsInDescendingOrder()
    {
        // Arrange & Act
        Thread.Sleep(10);
        _metrics.RecordDeployment("Contract1", "0x111", true, TimeSpan.FromSeconds(1));
        Thread.Sleep(10);
        _metrics.RecordDeployment("Contract2", "0x222", true, TimeSpan.FromSeconds(2));
        Thread.Sleep(10);
        _metrics.RecordDeployment("Contract3", "0x333", true, TimeSpan.FromSeconds(3));

        // Assert
        var history = _metrics.GetDeploymentHistory();
        Assert.Equal(3, history.Count);
        Assert.Equal("Contract3", history[0].ContractName); // Most recent first
        Assert.Equal("Contract2", history[1].ContractName);
        Assert.Equal("Contract1", history[2].ContractName);
    }

    [Fact]
    public void GetDeploymentHistory_WithLimit_RespectsLimit()
    {
        // Arrange & Act
        for (int i = 0; i < 10; i++)
        {
            _metrics.RecordDeployment($"Contract{i}", $"0x{i}", true, TimeSpan.FromSeconds(1));
        }

        // Assert
        var history = _metrics.GetDeploymentHistory(5);
        Assert.Equal(5, history.Count);
    }

    [Fact]
    public void Clear_RemovesAllData()
    {
        // Arrange
        _metrics.RecordDeployment("Contract1", "0x111", true, TimeSpan.FromSeconds(5));
        _metrics.RecordInvocation("0x111", "method", true, TimeSpan.FromMilliseconds(100));
        _metrics.RecordMetric("custom", "metric", 42);

        // Act
        _metrics.Clear();

        // Assert
        var stats = _metrics.GetStatistics();
        Assert.Equal(0, stats.TotalDeployments);
        Assert.Equal(0, stats.TotalInvocations);
        
        var history = _metrics.GetDeploymentHistory();
        Assert.Empty(history);
    }

    [Fact]
    public void MetricData_AverageDuration_CalculatesCorrectly()
    {
        // Arrange
        var metric = new MetricData
        {
            Count = 3,
            TotalDuration = TimeSpan.FromMilliseconds(300)
        };

        // Act
        var average = metric.AverageDuration;

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(100), average);
    }

    [Fact]
    public void MetricData_SuccessRate_CalculatesCorrectly()
    {
        // Arrange
        var metric = new MetricData
        {
            Count = 10,
            SuccessCount = 7
        };

        // Act
        var successRate = metric.SuccessRate;

        // Assert
        Assert.Equal(0.7, successRate);
    }

    [Fact]
    public void MetricData_AverageValue_CalculatesCorrectly()
    {
        // Arrange
        var metric = new MetricData
        {
            Count = 4,
            TotalValue = 100
        };

        // Act
        var average = metric.AverageValue;

        // Assert
        Assert.Equal(25, average);
    }

    [Fact]
    public void RecordDeployment_ConcurrentCalls_ThreadSafe()
    {
        // Arrange
        const int threadCount = 10;
        const int deploymentsPerThread = 100;
        var threads = new Thread[threadCount];

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            var threadIndex = i;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < deploymentsPerThread; j++)
                {
                    _metrics.RecordDeployment(
                        $"Contract_{threadIndex}_{j}",
                        $"0x{threadIndex}{j}",
                        true,
                        TimeSpan.FromSeconds(1));
                }
            });
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Assert
        var stats = _metrics.GetStatistics();
        Assert.Equal(threadCount * deploymentsPerThread, stats.TotalDeployments);
    }

    [Fact]
    public void TimedOperation_Success_RecordsAsOperation()
    {
        // Arrange & Act
        using (var operation = new TimedOperation(_metrics, "test_op"))
        {
            Thread.Sleep(50);
            operation.Success();
        }

        // Assert
        var metrics = _metrics.GetMetricsByCategory("operation");
        Assert.Contains("operation_test_op", metrics.Keys);
    }

    [Fact]
    public void TimedOperation_Failure_RecordsAsFailedOperation()
    {
        // Arrange & Act
        using (var operation = new TimedOperation(_metrics, "test_op"))
        {
            Thread.Sleep(50);
            operation.Failure();
        }

        // Assert
        var metrics = _metrics.GetMetricsByCategory("operation_failed");
        Assert.Contains("operation_failed_test_op", metrics.Keys);
    }

    [Fact]
    public void TimedOperation_AutoDispose_RecordsAsSuccess()
    {
        // Arrange & Act
        using (var operation = new TimedOperation(_metrics, "test_op"))
        {
            Thread.Sleep(50);
            // Auto-dispose without calling Success() or Failure()
        }

        // Assert
        var metrics = _metrics.GetMetricsByCategory("operation");
        Assert.Contains("operation_test_op", metrics.Keys);
    }

    public void Dispose()
    {
        _metrics?.Dispose();
    }
}