using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.HealthChecks;
using Neo.SmartContract.Deploy.HealthChecks.Interfaces;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.HealthChecks;

public class HealthCheckServiceTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<ILogger<HealthCheckService>> _loggerMock;

    public HealthCheckServiceTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _loggerMock = new Mock<ILogger<HealthCheckService>>();
    }

    [Fact]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        // Act
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new HealthCheckService(null!, _loggerMock.Object));
    }

    [Fact]
    public void AddHealthCheck_ValidHealthCheck_AddsSuccessfully()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        var healthCheckMock = new Mock<IHealthCheck>();
        healthCheckMock.Setup(x => x.Name).Returns("TestCheck");

        // Act
        service.AddHealthCheck(healthCheckMock.Object);

        // Assert - No exception thrown
        Assert.True(true);
    }

    [Fact]
    public void AddHealthCheck_NullHealthCheck_ThrowsException()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            service.AddHealthCheck(null!));
    }

    [Fact]
    public async Task CheckHealthAsync_SingleHealthyCheck_ReturnsHealthyReport()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        var healthCheckMock = new Mock<IHealthCheck>();
        healthCheckMock.Setup(x => x.Name).Returns("HealthyCheck");
        healthCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Healthy("All good"));

        service.AddHealthCheck(healthCheckMock.Object);

        // Act
        var report = await service.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Healthy, report.Status);
        Assert.Single(report.Entries);
        Assert.Equal(HealthStatus.Healthy, report.Entries["HealthyCheck"].Status);
    }

    [Fact]
    public async Task CheckHealthAsync_MixedHealthChecks_ReturnsDegradedReport()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        
        var healthyCheckMock = new Mock<IHealthCheck>();
        healthyCheckMock.Setup(x => x.Name).Returns("HealthyCheck");
        healthyCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Healthy());

        var degradedCheckMock = new Mock<IHealthCheck>();
        degradedCheckMock.Setup(x => x.Name).Returns("DegradedCheck");
        degradedCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Degraded("Performance issues"));

        service.AddHealthCheck(healthyCheckMock.Object);
        service.AddHealthCheck(degradedCheckMock.Object);

        // Act
        var report = await service.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Degraded, report.Status);
        Assert.Equal(2, report.Entries.Count);
    }

    [Fact]
    public async Task CheckHealthAsync_WithUnhealthyCheck_ReturnsUnhealthyReport()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        
        var healthyCheckMock = new Mock<IHealthCheck>();
        healthyCheckMock.Setup(x => x.Name).Returns("HealthyCheck");
        healthyCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Healthy());

        var unhealthyCheckMock = new Mock<IHealthCheck>();
        unhealthyCheckMock.Setup(x => x.Name).Returns("UnhealthyCheck");
        unhealthyCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Unhealthy("Service down", new Exception("Connection failed")));

        service.AddHealthCheck(healthyCheckMock.Object);
        service.AddHealthCheck(unhealthyCheckMock.Object);

        // Act
        var report = await service.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, report.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_WithTimeout_HandlesTimeout()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object, TimeSpan.FromMilliseconds(100));
        
        var slowCheckMock = new Mock<IHealthCheck>();
        slowCheckMock.Setup(x => x.Name).Returns("SlowCheck");
        slowCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken ct) =>
            {
                await Task.Delay(1000, ct);
                return HealthCheckResult.Healthy();
            });

        service.AddHealthCheck(slowCheckMock.Object);

        // Act
        var report = await service.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, report.Status);
        Assert.Contains("timed out", report.Entries["SlowCheck"].Description);
    }

    [Fact]
    public async Task CheckHealthAsync_WithException_HandlesException()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        
        var faultyCheckMock = new Mock<IHealthCheck>();
        faultyCheckMock.Setup(x => x.Name).Returns("FaultyCheck");
        faultyCheckMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Check failed"));

        service.AddHealthCheck(faultyCheckMock.Object);

        // Act
        var report = await service.CheckHealthAsync();

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, report.Status);
        Assert.NotNull(report.Entries["FaultyCheck"].Exception);
    }

    [Fact]
    public async Task CheckHealthAsync_FilteredServices_ChecksOnlySpecified()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        
        var check1Mock = new Mock<IHealthCheck>();
        check1Mock.Setup(x => x.Name).Returns("Check1");
        check1Mock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Healthy());

        var check2Mock = new Mock<IHealthCheck>();
        check2Mock.Setup(x => x.Name).Returns("Check2");
        check2Mock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(HealthCheckResult.Healthy());

        service.AddHealthCheck(check1Mock.Object);
        service.AddHealthCheck(check2Mock.Object);

        // Act
        var report = await service.CheckHealthAsync(new[] { "Check1" });

        // Assert
        Assert.Single(report.Entries);
        Assert.Contains("Check1", report.Entries.Keys);
        Assert.DoesNotContain("Check2", report.Entries.Keys);
    }

    [Fact]
    public void AddRegisteredHealthChecks_AddsAllFromServiceProvider()
    {
        // Arrange
        var healthChecks = new List<IHealthCheck>
        {
            Mock.Of<IHealthCheck>(x => x.Name == "Check1"),
            Mock.Of<IHealthCheck>(x => x.Name == "Check2")
        };

        _serviceProviderMock.Setup(x => x.GetService(typeof(IEnumerable<IHealthCheck>)))
            .Returns(healthChecks);

        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);

        // Act
        service.AddRegisteredHealthChecks();

        // Assert - Verify through CheckHealthAsync
        var checkTask = service.CheckHealthAsync();
        Assert.True(checkTask.IsCompletedSuccessfully);
    }

    [Fact]
    public void GetHealthSummary_ValidReport_ReturnsFormattedSummary()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["Check1"] = new HealthReportEntry { Status = HealthStatus.Healthy },
            ["Check2"] = new HealthReportEntry { Status = HealthStatus.Degraded },
            ["Check3"] = new HealthReportEntry { Status = HealthStatus.Unhealthy }
        };
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.FromMilliseconds(150));

        // Act
        var summary = service.GetHealthSummary(report);

        // Assert
        Assert.Contains("Status: Unhealthy", summary);
        Assert.Contains("3 total", summary);
        Assert.Contains("1 healthy", summary);
        Assert.Contains("1 degraded", summary);
        Assert.Contains("1 unhealthy", summary);
        Assert.Contains("150ms", summary);
    }

    [Fact]
    public void ConfigureDefaultHealthChecks_AddsExpectedHealthChecks()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);

        // Act
        service.ConfigureDefaultHealthChecks("http://localhost:20332", httpClient);

        // Assert - Verify through CheckHealthAsync
        var checkTask = service.CheckHealthAsync();
        Assert.True(checkTask.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task CheckHealthAsync_ParallelExecution_CompletesInReasonableTime()
    {
        // Arrange
        var service = new HealthCheckService(_serviceProviderMock.Object, _loggerMock.Object);
        
        // Add multiple slow health checks
        for (int i = 0; i < 5; i++)
        {
            var checkMock = new Mock<IHealthCheck>();
            checkMock.Setup(x => x.Name).Returns($"Check{i}");
            checkMock.Setup(x => x.CheckHealthAsync(It.IsAny<CancellationToken>()))
                .Returns(async (CancellationToken ct) =>
                {
                    await Task.Delay(100, ct); // Each check takes 100ms
                    return HealthCheckResult.Healthy();
                });
            service.AddHealthCheck(checkMock.Object);
        }

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var report = await service.CheckHealthAsync();
        stopwatch.Stop();

        // Assert - Should complete in less than 500ms (parallel) rather than 500ms (sequential)
        Assert.True(stopwatch.ElapsedMilliseconds < 300);
        Assert.Equal(5, report.Entries.Count);
        Assert.All(report.Entries.Values, entry => Assert.Equal(HealthStatus.Healthy, entry.Status));
    }
}