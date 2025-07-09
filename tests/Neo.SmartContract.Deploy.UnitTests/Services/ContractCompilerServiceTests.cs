using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Services;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class ContractCompilerServiceTests : TestBase
{
    private readonly ContractCompilerService _compilerService;
    private readonly Mock<ILogger<ContractCompilerService>> _mockLogger;

    public ContractCompilerServiceTests()
    {
        _mockLogger = new Mock<ILogger<ContractCompilerService>>();
        _compilerService = new ContractCompilerService(_mockLogger.Object);
    }

    [Fact]
    public async Task CompileAsync_WithNullPath_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _compilerService.CompileAsync(null!));
    }

    [Fact]
    public async Task CompileAsync_WithEmptyPath_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _compilerService.CompileAsync(""));
    }

    [Fact]
    public async Task CompileAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentPath = "/non/existent/file.csproj";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _compilerService.CompileAsync(nonExistentPath));
    }

    [Fact]
    public async Task LoadContractAsync_WithNullNefPath_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _compilerService.LoadContractAsync(null!, "manifest.json"));
    }

    [Fact]
    public async Task LoadContractAsync_WithNullManifestPath_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _compilerService.LoadContractAsync("contract.nef", null!));
    }

    [Fact]
    public async Task LoadContractAsync_WithNonExistentNef_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentNef = "/non/existent/contract.nef";
        var manifestPath = "manifest.json";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _compilerService.LoadContractAsync(nonExistentNef, manifestPath));
    }

    [Fact]
    public async Task LoadContractAsync_WithNonExistentManifest_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var tempNef = Path.GetTempFileName();
        var nonExistentManifest = "/non/existent/manifest.json";

        try
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _compilerService.LoadContractAsync(tempNef, nonExistentManifest));
        }
        finally
        {
            File.Delete(tempNef);
        }
    }

    [Fact]
    public async Task LoadContractAsync_WithValidFiles_ShouldLoadContract()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var nefPath = Path.Combine(tempDir, "test.nef");
        var manifestPath = Path.Combine(tempDir, "test.manifest.json");

        try
        {
            // Create minimal NEF file
            var nefContent = new byte[] 
            { 
                0x4E, 0x45, 0x46, 0x33, // Magic
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Compiler
                0x00, 0x00, 0x00, 0x00, // Source
                0x00, // Reserved
                0x00, 0x00, // Method count
                0x01, 0x00, // Script length
                0x40, // RET opcode
                0x00, 0x00, 0x00, 0x00 // Checksum
            };
            await File.WriteAllBytesAsync(nefPath, nefContent);

            // Create minimal manifest
            var manifestContent = @"{
                ""name"": ""TestContract"",
                ""groups"": [],
                ""features"": {},
                ""supportedstandards"": [],
                ""abi"": {
                    ""methods"": [{
                        ""name"": ""test"",
                        ""parameters"": [],
                        ""returntype"": ""Void"",
                        ""offset"": 0,
                        ""safe"": true
                    }],
                    ""events"": []
                },
                ""permissions"": [],
                ""trusts"": [],
                ""extra"": null
            }";
            await File.WriteAllTextAsync(manifestPath, manifestContent);

            // Act
            var contract = await _compilerService.LoadContractAsync(nefPath, manifestPath);

            // Assert
            Assert.NotNull(contract);
            Assert.Equal("TestContract", contract.Name);
            Assert.Equal(nefPath, contract.NefFilePath);
            Assert.Equal(manifestPath, contract.ManifestFilePath);
            Assert.NotEmpty(contract.NefBytes);
            Assert.NotNull(contract.Manifest);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task LoadContractAsync_WithInvalidManifestJson_ShouldThrowContractDeploymentException()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var nefPath = Path.Combine(tempDir, "test.nef");
        var manifestPath = Path.Combine(tempDir, "test.manifest.json");

        try
        {
            // Create minimal NEF file
            var nefContent = new byte[] { 0x4E, 0x45, 0x46, 0x33 };
            await File.WriteAllBytesAsync(nefPath, nefContent);

            // Create invalid manifest
            await File.WriteAllTextAsync(manifestPath, "{ invalid json }");

            // Act & Assert
            await Assert.ThrowsAsync<ContractDeploymentException>(() =>
                _compilerService.LoadContractAsync(nefPath, manifestPath));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }
}