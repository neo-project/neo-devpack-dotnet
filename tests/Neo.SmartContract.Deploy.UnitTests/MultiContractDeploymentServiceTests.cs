using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Manifest;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class MultiContractDeploymentServiceTests
{
    private readonly Mock<IContractCompiler> _mockCompiler;
    private readonly Mock<IContractDeployer> _mockDeployer;
    private readonly Mock<IContractInvoker> _mockInvoker;
    private readonly Mock<IContractUpdateService> _mockUpdater;
    private readonly Mock<ILogger<MultiContractDeploymentService>> _mockLogger;
    private readonly MultiContractDeploymentService _service;

    public MultiContractDeploymentServiceTests()
    {
        _mockCompiler = new Mock<IContractCompiler>();
        _mockDeployer = new Mock<IContractDeployer>();
        _mockInvoker = new Mock<IContractInvoker>();
        _mockUpdater = new Mock<IContractUpdateService>();
        _mockLogger = new Mock<ILogger<MultiContractDeploymentService>>();

        _service = new MultiContractDeploymentService(
            _mockCompiler.Object,
            _mockDeployer.Object,
            _mockInvoker.Object,
            _mockUpdater.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task DeployMultipleAsync_WithValidManifest_ShouldDeployAllContracts()
    {
        // Arrange
        var manifest = CreateTestManifest();
        var options = CreateTestDeploymentOptions();
        
        SetupMockCompiler();
        SetupMockDeployer();

        // Act
        var result = await _service.DeployMultipleAsync(manifest, options);

        // Assert
        Assert.Equal(DeploymentStatus.Completed, result.Status);
        Assert.Equal(2, result.DeployedContracts.Count);
        Assert.Equal(0, result.FailedDeployments.Count);
        Assert.Equal(2, result.Summary.SuccessfulDeployments);
        Assert.NotNull(result.EndTime);
        
        // Verify deployment order (contract2 depends on contract1)
        _mockDeployer.Verify(d => d.DeployAsync(
            It.Is<CompiledContract>(c => c.Name == "Contract1"),
            It.IsAny<DeploymentOptions>(),
            It.IsAny<object[]>()), Times.Once);
        
        _mockDeployer.Verify(d => d.DeployAsync(
            It.Is<CompiledContract>(c => c.Name == "Contract2"),
            It.IsAny<DeploymentOptions>(),
            It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task DeployMultipleAsync_WithDependencies_ShouldRespectDependencyOrder()
    {
        // Arrange
        var manifest = new DeploymentManifest
        {
            Name = "Test Deployment",
            Contracts = new List<ContractDefinition>
            {
                new() { Id = "contract3", Name = "Contract3", ProjectPath = "contract3.csproj", Dependencies = { "contract2" } },
                new() { Id = "contract1", Name = "Contract1", ProjectPath = "contract1.csproj" },
                new() { Id = "contract2", Name = "Contract2", ProjectPath = "contract2.csproj", Dependencies = { "contract1" } }
            }
        };
        
        var deploymentOrder = new List<string>();
        
        _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>()))
            .ReturnsAsync((string path) => new CompiledContract
            {
                Name = Path.GetFileNameWithoutExtension(path),
                NefBytes = new byte[] { 1, 2, 3 },
                Manifest = CreateTestManifest("Test")
            });

        _mockDeployer.Setup(d => d.DeployAsync(It.IsAny<CompiledContract>(), It.IsAny<DeploymentOptions>(), It.IsAny<object[]>()))
            .ReturnsAsync((CompiledContract contract, DeploymentOptions _, object[] __) =>
            {
                deploymentOrder.Add(contract.Name);
                return new ContractDeploymentInfo
                {
                    ContractHash = UInt160.Zero,
                    ContractName = contract.Name,
                    TransactionHash = UInt256.Zero,
                    GasConsumed = 1000000,
                    NetworkFee = 100000,
                    Manifest = contract.Manifest
                };
            });

        // Act
        var result = await _service.DeployMultipleAsync(manifest, CreateTestDeploymentOptions());

        // Assert
        Assert.Equal(3, deploymentOrder.Count);
        Assert.Equal("contract1", deploymentOrder[0]);
        Assert.Equal("contract2", deploymentOrder[1]);
        Assert.Equal("contract3", deploymentOrder[2]);
    }

    [Fact]
    public async Task DeployMultipleAsync_WithCircularDependency_ShouldThrow()
    {
        // Arrange
        var manifest = new DeploymentManifest
        {
            Name = "Test Deployment",
            Contracts = new List<ContractDefinition>
            {
                new() { Id = "contract1", Name = "Contract1", ProjectPath = "contract1.csproj", Dependencies = { "contract2" } },
                new() { Id = "contract2", Name = "Contract2", ProjectPath = "contract2.csproj", Dependencies = { "contract1" } }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.DeployMultipleAsync(manifest, CreateTestDeploymentOptions()));
    }

    [Fact]
    public async Task DeployMultipleAsync_WithFailureAndContinueOnError_ShouldContinueDeployment()
    {
        // Arrange
        var manifest = CreateTestManifest();
        manifest.ContinueOnError = true;
        
        var options = CreateTestDeploymentOptions();
        
        _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>()))
            .ReturnsAsync((string path) => new CompiledContract
            {
                Name = Path.GetFileNameWithoutExtension(path),
                NefBytes = new byte[] { 1, 2, 3 },
                Manifest = CreateTestManifest("Test")
            });

        // First deployment fails
        _mockDeployer.SetupSequence(d => d.DeployAsync(It.IsAny<CompiledContract>(), It.IsAny<DeploymentOptions>(), It.IsAny<object[]>()))
            .ThrowsAsync(new ContractDeploymentException("Deployment failed"))
            .ReturnsAsync(new ContractDeploymentInfo
            {
                ContractHash = UInt160.Zero,
                ContractName = "Contract2",
                TransactionHash = UInt256.Zero,
                GasConsumed = 1000000,
                NetworkFee = 100000
            });

        // Act
        var result = await _service.DeployMultipleAsync(manifest, options);

        // Assert
        Assert.Equal(DeploymentStatus.PartiallyCompleted, result.Status);
        Assert.Equal(1, result.DeployedContracts.Count);
        Assert.Equal(1, result.FailedDeployments.Count);
        Assert.Contains("contract1", result.FailedDeployments.Keys);
    }

    [Fact]
    public async Task DeployMultipleAsync_WithFailureAndRollback_ShouldRollbackDeployedContracts()
    {
        // Arrange
        var manifest = CreateTestManifest();
        manifest.Settings.RollbackOnFailure = true;
        manifest.ContinueOnError = true;
        
        var options = CreateTestDeploymentOptions();
        
        SetupMockCompiler();
        
        // First deployment succeeds, second fails
        _mockDeployer.SetupSequence(d => d.DeployAsync(It.IsAny<CompiledContract>(), It.IsAny<DeploymentOptions>(), It.IsAny<object[]>()))
            .ReturnsAsync(new ContractDeploymentInfo
            {
                ContractHash = UInt160.Parse("0x1234567890123456789012345678901234567890"),
                ContractName = "Contract1",
                TransactionHash = UInt256.Zero,
                GasConsumed = 1000000,
                NetworkFee = 100000,
                Manifest = CreateTestManifest("Contract1")
            })
            .ThrowsAsync(new ContractDeploymentException("Deployment failed"));

        // Setup rollback
        _mockInvoker.Setup(i => i.InvokeAsync(
            It.IsAny<UInt160>(),
            It.Is<string>(m => m == "destroy"),
            It.IsAny<object[]>(),
            It.IsAny<InvocationOptions>()))
            .ReturnsAsync(UInt256.Zero);

        // Act
        var result = await _service.DeployMultipleAsync(manifest, options);

        // Assert
        Assert.Equal(DeploymentStatus.RolledBack, result.Status);
        Assert.NotNull(result.RollbackResult);
        Assert.Equal(RollbackStatus.Success, result.RollbackResult.Status);
        Assert.Contains("0x1234567890123456789012345678901234567890", result.RollbackResult.RolledBackContracts);
    }

    [Fact]
    public async Task DeployFromManifestAsync_WithValidFile_ShouldDeployContracts()
    {
        // Arrange
        var manifestPath = Path.GetTempFileName();
        var manifest = CreateTestManifest();
        var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await File.WriteAllTextAsync(manifestPath, json);
        
        try
        {
            SetupMockCompiler();
            SetupMockDeployer();

            // Act
            var result = await _service.DeployFromManifestAsync(manifestPath, CreateTestDeploymentOptions());

            // Assert
            Assert.Equal(DeploymentStatus.Completed, result.Status);
            Assert.Equal(2, result.DeployedContracts.Count);
        }
        finally
        {
            File.Delete(manifestPath);
        }
    }

    [Fact]
    public async Task SetupContractInteractionsAsync_WithValidInteractions_ShouldSetupSuccessfully()
    {
        // Arrange
        var deploymentResult = new MultiContractDeploymentResult
        {
            DeployedContracts = new Dictionary<string, ContractDeploymentInfo>
            {
                ["contract1"] = new ContractDeploymentInfo 
                { 
                    ContractHash = UInt160.Parse("0x1234567890123456789012345678901234567890"),
                    ContractName = "Contract1"
                },
                ["contract2"] = new ContractDeploymentInfo 
                { 
                    ContractHash = UInt160.Parse("0x0987654321098765432109876543210987654321"),
                    ContractName = "Contract2"
                }
            }
        };

        var interactions = new List<ContractInteraction>
        {
            new()
            {
                Source = "contract1",
                Target = "contract2",
                Method = "setPartner",
                Params = new List<object> { "@contract:contract2" },
                Order = 1
            }
        };

        _mockInvoker.Setup(i => i.InvokeAsync(
            It.IsAny<UInt160>(),
            It.IsAny<string>(),
            It.IsAny<object[]>(),
            It.IsAny<InvocationOptions>()))
            .ReturnsAsync(UInt256.Zero);

        // Act
        var result = await _service.SetupContractInteractionsAsync(
            deploymentResult, 
            interactions, 
            CreateTestInvocationOptions());

        // Assert
        Assert.Equal(1, result.TotalAttempted);
        Assert.Equal(1, result.Successful);
        Assert.Equal(0, result.Failed);
        Assert.Single(result.Results);
        Assert.True(result.Results[0].Success);
    }

    [Fact]
    public void ResolveDependencyOrder_WithComplexDependencies_ShouldReturnCorrectOrder()
    {
        // Arrange
        var contracts = new List<ContractDefinition>
        {
            new() { Id = "D", Dependencies = { "B", "C" } },
            new() { Id = "B", Dependencies = { "A" } },
            new() { Id = "C", Dependencies = { "A" } },
            new() { Id = "A", Dependencies = new List<string>() },
            new() { Id = "E", Dependencies = { "D" } }
        };

        // Act
        var ordered = _service.ResolveDependencyOrder(contracts);

        // Assert
        Assert.Equal(5, ordered.Count);
        
        var orderMap = ordered.Select((c, i) => new { c.Id, Index = i })
            .ToDictionary(x => x.Id, x => x.Index);
        
        // A should come before B and C
        Assert.True(orderMap["A"] < orderMap["B"]);
        Assert.True(orderMap["A"] < orderMap["C"]);
        
        // B and C should come before D
        Assert.True(orderMap["B"] < orderMap["D"]);
        Assert.True(orderMap["C"] < orderMap["D"]);
        
        // D should come before E
        Assert.True(orderMap["D"] < orderMap["E"]);
    }

    [Fact]
    public async Task DeployMultipleAsync_WithBatchingEnabled_ShouldDeployInBatches()
    {
        // Arrange
        var manifest = new DeploymentManifest
        {
            Name = "Batch Test",
            EnableBatching = true,
            BatchSize = 2,
            Contracts = new List<ContractDefinition>
            {
                new() { Id = "A", Name = "A", ProjectPath = "a.csproj" },
                new() { Id = "B", Name = "B", ProjectPath = "b.csproj" },
                new() { Id = "C", Name = "C", ProjectPath = "c.csproj", Dependencies = { "A" } },
                new() { Id = "D", Name = "D", ProjectPath = "d.csproj", Dependencies = { "B" } }
            }
        };

        SetupMockCompiler();
        SetupMockDeployer();

        // Act
        var result = await _service.DeployMultipleAsync(manifest, CreateTestDeploymentOptions());

        // Assert
        Assert.Equal(DeploymentStatus.Completed, result.Status);
        Assert.Equal(4, result.DeployedContracts.Count);
        
        // Verify all contracts were deployed
        _mockDeployer.Verify(d => d.DeployAsync(
            It.IsAny<CompiledContract>(),
            It.IsAny<DeploymentOptions>(),
            It.IsAny<object[]>()), Times.Exactly(4));
    }

    [Fact]
    public async Task DeployMultipleAsync_WithContractReference_ShouldResolveReferences()
    {
        // Arrange
        var manifest = new DeploymentManifest
        {
            Name = "Reference Test",
            Contracts = new List<ContractDefinition>
            {
                new() { Id = "token", Name = "Token", ProjectPath = "token.csproj" },
                new() 
                { 
                    Id = "dex", 
                    Name = "DEX", 
                    ProjectPath = "dex.csproj",
                    Dependencies = { "token" },
                    InitParams = new List<object> { "@contract:token", 1000000 }
                }
            }
        };

        var capturedInitParams = null as object[];
        
        SetupMockCompiler();
        
        _mockDeployer.Setup(d => d.DeployAsync(
            It.IsAny<CompiledContract>(),
            It.IsAny<DeploymentOptions>(),
            It.IsAny<object[]>()))
            .Callback<CompiledContract, DeploymentOptions, object[]>((c, o, p) =>
            {
                if (c.Name == "dex")
                    capturedInitParams = p;
            })
            .ReturnsAsync((CompiledContract c, DeploymentOptions o, object[] p) => new ContractDeploymentInfo
            {
                ContractHash = c.Name == "token" 
                    ? UInt160.Parse("0x1111111111111111111111111111111111111111")
                    : UInt160.Parse("0x2222222222222222222222222222222222222222"),
                ContractName = c.Name,
                TransactionHash = UInt256.Zero
            });

        // Act
        var result = await _service.DeployMultipleAsync(manifest, CreateTestDeploymentOptions());

        // Assert
        Assert.NotNull(capturedInitParams);
        Assert.Equal(2, capturedInitParams.Length);
        Assert.Equal(UInt160.Parse("0x1111111111111111111111111111111111111111"), capturedInitParams[0]);
        Assert.Equal(1000000, capturedInitParams[1]);
    }

    #region Helper Methods

    private DeploymentManifest CreateTestManifest()
    {
        return new DeploymentManifest
        {
            Name = "Test Deployment",
            Contracts = new List<ContractDefinition>
            {
                new()
                {
                    Id = "contract1",
                    Name = "Contract1",
                    ProjectPath = "contract1.csproj"
                },
                new()
                {
                    Id = "contract2",
                    Name = "Contract2",
                    ProjectPath = "contract2.csproj",
                    Dependencies = { "contract1" }
                }
            }
        };
    }

    private DeploymentOptions CreateTestDeploymentOptions()
    {
        return new DeploymentOptions
        {
            WifKey = "L1234567890123456789012345678901234567890123456789012345",
            RpcUrl = "http://localhost:10332",
            NetworkMagic = 12345678,
            GasLimit = 100_000_000,
            WaitForConfirmation = false
        };
    }

    private InvocationOptions CreateTestInvocationOptions()
    {
        return new InvocationOptions
        {
            WifKey = "L1234567890123456789012345678901234567890123456789012345",
            RpcUrl = "http://localhost:10332",
            NetworkMagic = 12345678,
            GasLimit = 10_000_000,
            WaitForConfirmation = false
        };
    }

    private void SetupMockCompiler()
    {
        _mockCompiler.Setup(c => c.CompileAsync(It.IsAny<string>()))
            .ReturnsAsync((string path) => new CompiledContract
            {
                Name = Path.GetFileNameWithoutExtension(path),
                NefBytes = new byte[] { 1, 2, 3 },
                Manifest = CreateTestManifest("Test")
            });
    }

    private void SetupMockDeployer()
    {
        _mockDeployer.Setup(d => d.DeployAsync(It.IsAny<CompiledContract>(), It.IsAny<DeploymentOptions>(), It.IsAny<object[]>()))
            .ReturnsAsync((CompiledContract contract, DeploymentOptions _, object[] __) => new ContractDeploymentInfo
            {
                ContractHash = UInt160.Zero,
                ContractName = contract.Name,
                TransactionHash = UInt256.Zero,
                GasConsumed = 1000000,
                NetworkFee = 100000,
                Manifest = contract.Manifest
            });
    }

    private ContractManifest CreateTestManifest(string name)
    {
        return new ContractManifest
        {
            Name = name,
            Abi = new ContractAbi
            {
                Methods = new[]
                {
                    new ContractMethodDescriptor
                    {
                        Name = "destroy",
                        Parameters = Array.Empty<ContractParameterDefinition>(),
                        ReturnType = ContractParameterType.Void,
                        Safe = false
                    }
                }
            }
        };
    }

    #endregion
}