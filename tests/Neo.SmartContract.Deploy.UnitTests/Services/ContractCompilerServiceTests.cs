using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Exceptions;
using System.IO;
using System.Threading.Tasks;
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
    public async Task CompileAsync_WithValidContract_ShouldReturnCompiledContract()
    {
        // Arrange
        var contractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var options = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract",
            GenerateDebugInfo = true,
            Optimize = true
        };

        // Act
        var result = await _compilerService.CompileAsync(options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestContract", result.Name);
        Assert.NotEmpty(result.NefBytes);
        Assert.NotNull(result.Manifest);
        Assert.True(File.Exists(result.NefFilePath));
        Assert.True(File.Exists(result.ManifestFilePath));
        
        // Verify manifest contains expected methods
        Assert.Contains(result.Manifest.Abi.Methods, m => m.Name == "testMethod");
        Assert.Contains(result.Manifest.Abi.Methods, m => m.Name == "getValue");

        // Cleanup
        Directory.Delete(Path.GetDirectoryName(contractPath)!, true);
        Directory.Delete(outputDir, true);
    }

    [Fact]
    public async Task CompileAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var options = new CompilationOptions
        {
            SourcePath = "/non/existent/file.cs",
            OutputDirectory = Path.GetTempPath(),
            ContractName = "TestContract"
        };

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _compilerService.CompileAsync(options));
    }

    [Fact]
    public async Task CompileAsync_WithInvalidContract_ShouldThrowCompilationException()
    {
        // Arrange
        var invalidContractCode = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract : SmartContract
    {
        // Invalid syntax
        public static string TestMethod(
        {
            return ""Hello"";
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "InvalidContract.cs");
        File.WriteAllText(contractPath, invalidContractCode);

        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var options = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "InvalidContract"
        };

        // Act & Assert
        await Assert.ThrowsAsync<CompilationException>(() => _compilerService.CompileAsync(options));

        // Cleanup
        Directory.Delete(tempDir, true);
        Directory.Delete(outputDir, true);
    }

    [Fact]
    public async Task LoadAsync_WithValidArtifacts_ShouldReturnCompiledContract()
    {
        // Arrange - First compile a contract to get artifacts
        var contractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var compileOptions = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract"
        };

        var compiled = await _compilerService.CompileAsync(compileOptions);

        // Act - Load from the artifacts
        var loaded = await _compilerService.LoadAsync(compiled.NefFilePath, compiled.ManifestFilePath);

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal("TestContract", loaded.Name);
        Assert.Equal(compiled.NefBytes.Length, loaded.NefBytes.Length);
        Assert.Equal(compiled.Manifest.Name, loaded.Manifest.Name);

        // Cleanup
        Directory.Delete(Path.GetDirectoryName(contractPath)!, true);
        Directory.Delete(outputDir, true);
    }

    [Fact]
    public async Task LoadAsync_WithNonExistentNefFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var manifestPath = Path.Combine(Path.GetTempPath(), "test.manifest.json");
        File.WriteAllText(manifestPath, @"{""name"": ""TestContract""}");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _compilerService.LoadAsync("/non/existent/file.nef", manifestPath));

        // Cleanup
        File.Delete(manifestPath);
    }

    [Fact]
    public async Task LoadAsync_WithNonExistentManifestFile_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nefPath = Path.Combine(Path.GetTempPath(), "test.nef");
        File.WriteAllBytes(nefPath, new byte[] { 0x4E, 0x45, 0x46, 0x33 }); // NEF magic

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _compilerService.LoadAsync(nefPath, "/non/existent/manifest.json"));

        // Cleanup
        File.Delete(nefPath);
    }

    [Fact]
    public async Task CompileAsync_WithDebugInfo_ShouldGenerateDebugFile()
    {
        // Arrange
        var contractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var options = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract",
            GenerateDebugInfo = true
        };

        // Act
        var result = await _compilerService.CompileAsync(options);

        // Assert
        var debugInfoPath = Path.Combine(outputDir, "TestContract.nefdbgnfo");
        Assert.True(File.Exists(debugInfoPath));

        // Cleanup
        Directory.Delete(Path.GetDirectoryName(contractPath)!, true);
        Directory.Delete(outputDir, true);
    }

    [Fact]
    public async Task CompileAsync_WithoutDebugInfo_ShouldNotGenerateDebugFile()
    {
        // Arrange
        var contractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var options = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract",
            GenerateDebugInfo = false
        };

        // Act
        var result = await _compilerService.CompileAsync(options);

        // Assert
        var debugInfoPath = Path.Combine(outputDir, "TestContract.nefdbgnfo");
        Assert.False(File.Exists(debugInfoPath));

        // Cleanup
        Directory.Delete(Path.GetDirectoryName(contractPath)!, true);
        Directory.Delete(outputDir, true);
    }
}