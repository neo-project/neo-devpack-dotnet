using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using R3E.WebGUI.Service.Core.Services;
using System.Text;
using Xunit;

namespace R3E.WebGUI.Service.UnitTests.Services;

public class LocalStorageServiceTests : IDisposable
{
    private readonly string _testStoragePath;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<LocalStorageService>> _mockLogger;
    private readonly LocalStorageService _service;

    public LocalStorageServiceTests()
    {
        _testStoragePath = Path.Combine(Path.GetTempPath(), $"webgui-test-{Guid.NewGuid()}");
        
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c.GetValue<string>("Storage:LocalPath"))
            .Returns(_testStoragePath);
        
        _mockLogger = new Mock<ILogger<LocalStorageService>>();
        _service = new LocalStorageService(_mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task UploadWebGUIFilesAsync_ValidFiles_CreatesDirectoryAndUploadsFiles()
    {
        // Arrange
        var subdomain = "testcontract";
        var files = CreateMockFiles();

        // Act
        var result = await _service.UploadWebGUIFilesAsync(subdomain, files);

        // Assert
        result.Should().Be(Path.Combine(_testStoragePath, subdomain));
        
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        Directory.Exists(subdomainPath).Should().BeTrue();
        
        File.Exists(Path.Combine(subdomainPath, "index.html")).Should().BeTrue();
        File.Exists(Path.Combine(subdomainPath, "style.css")).Should().BeTrue();
        
        var htmlContent = await File.ReadAllTextAsync(Path.Combine(subdomainPath, "index.html"));
        htmlContent.Should().Be("<html><body>Test</body></html>");
    }

    [Fact]
    public async Task UploadWebGUIFilesAsync_ExistingDirectory_CleansUpAndRecreates()
    {
        // Arrange
        var subdomain = "testcontract";
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        
        // Create existing directory with a file
        Directory.CreateDirectory(subdomainPath);
        await File.WriteAllTextAsync(Path.Combine(subdomainPath, "old-file.txt"), "old content");
        
        var files = CreateMockFiles();

        // Act
        var result = await _service.UploadWebGUIFilesAsync(subdomain, files);

        // Assert
        result.Should().Be(subdomainPath);
        
        File.Exists(Path.Combine(subdomainPath, "old-file.txt")).Should().BeFalse();
        File.Exists(Path.Combine(subdomainPath, "index.html")).Should().BeTrue();
        File.Exists(Path.Combine(subdomainPath, "style.css")).Should().BeTrue();
    }

    [Fact]
    public async Task UploadWebGUIFilesAsync_EmptyFiles_SkipsEmptyFiles()
    {
        // Arrange
        var subdomain = "testcontract";
        var files = new FormFileCollection();
        
        var emptyFile = new Mock<IFormFile>();
        emptyFile.Setup(f => f.FileName).Returns("empty.txt");
        emptyFile.Setup(f => f.Length).Returns(0);
        
        var validFile = new Mock<IFormFile>();
        validFile.Setup(f => f.FileName).Returns("valid.txt");
        validFile.Setup(f => f.Length).Returns(10);
        validFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        files.Add(emptyFile.Object);
        files.Add(validFile.Object);

        // Act
        var result = await _service.UploadWebGUIFilesAsync(subdomain, files);

        // Assert
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        File.Exists(Path.Combine(subdomainPath, "empty.txt")).Should().BeFalse();
        File.Exists(Path.Combine(subdomainPath, "valid.txt")).Should().BeTrue();
    }

    [Fact]
    public async Task GetFileAsync_ExistingFile_ReturnsFileStream()
    {
        // Arrange
        var subdomain = "testcontract";
        var fileName = "test.html";
        var content = "<html><body>Test Content</body></html>";
        
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        Directory.CreateDirectory(subdomainPath);
        await File.WriteAllTextAsync(Path.Combine(subdomainPath, fileName), content);

        // Act
        using var stream = await _service.GetFileAsync(subdomain, fileName);
        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();

        // Assert
        result.Should().Be(content);
    }

    [Fact]
    public async Task GetFileAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var subdomain = "testcontract";
        var fileName = "nonexistent.html";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _service.GetFileAsync(subdomain, fileName));
    }

    [Fact]
    public async Task DeleteWebGUIFilesAsync_ExistingDirectory_DeletesSuccessfully()
    {
        // Arrange
        var subdomain = "testcontract";
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        
        Directory.CreateDirectory(subdomainPath);
        await File.WriteAllTextAsync(Path.Combine(subdomainPath, "test.html"), "content");

        // Act
        var result = await _service.DeleteWebGUIFilesAsync(subdomain);

        // Assert
        result.Should().BeTrue();
        Directory.Exists(subdomainPath).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteWebGUIFilesAsync_NonExistentDirectory_ReturnsFalse()
    {
        // Arrange
        var subdomain = "nonexistent";

        // Act
        var result = await _service.DeleteWebGUIFilesAsync(subdomain);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ListFilesAsync_ExistingDirectory_ReturnsRelativePaths()
    {
        // Arrange
        var subdomain = "testcontract";
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        
        Directory.CreateDirectory(subdomainPath);
        Directory.CreateDirectory(Path.Combine(subdomainPath, "css"));
        
        await File.WriteAllTextAsync(Path.Combine(subdomainPath, "index.html"), "content");
        await File.WriteAllTextAsync(Path.Combine(subdomainPath, "css", "style.css"), "content");

        // Act
        var files = await _service.ListFilesAsync(subdomain);

        // Assert
        files.Should().HaveCount(2);
        files.Should().Contain("index.html");
        files.Should().Contain(Path.Combine("css", "style.css"));
    }

    [Fact]
    public async Task ListFilesAsync_NonExistentDirectory_ReturnsEmptyCollection()
    {
        // Arrange
        var subdomain = "nonexistent";

        // Act
        var files = await _service.ListFilesAsync(subdomain);

        // Assert
        files.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadWebGUIFilesAsync_FilesWithSubdirectories_CreatesNestedStructure()
    {
        // Arrange
        var subdomain = "testcontract";
        var files = new FormFileCollection();
        
        var cssFile = new Mock<IFormFile>();
        cssFile.Setup(f => f.FileName).Returns("css/style.css");
        cssFile.Setup(f => f.Length).Returns(100);
        cssFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var jsFile = new Mock<IFormFile>();
        jsFile.Setup(f => f.FileName).Returns("js/app.js");
        jsFile.Setup(f => f.Length).Returns(200);
        jsFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        files.Add(cssFile.Object);
        files.Add(jsFile.Object);

        // Act
        await _service.UploadWebGUIFilesAsync(subdomain, files);

        // Assert
        var subdomainPath = Path.Combine(_testStoragePath, subdomain);
        Directory.Exists(Path.Combine(subdomainPath, "css")).Should().BeTrue();
        Directory.Exists(Path.Combine(subdomainPath, "js")).Should().BeTrue();
        File.Exists(Path.Combine(subdomainPath, "css", "style.css")).Should().BeTrue();
        File.Exists(Path.Combine(subdomainPath, "js", "app.js")).Should().BeTrue();
    }

    private IFormFileCollection CreateMockFiles()
    {
        var files = new FormFileCollection();
        
        var htmlFile = new Mock<IFormFile>();
        htmlFile.Setup(f => f.FileName).Returns("index.html");
        htmlFile.Setup(f => f.Length).Returns(100);
        htmlFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) =>
            {
                var content = Encoding.UTF8.GetBytes("<html><body>Test</body></html>");
                stream.Write(content, 0, content.Length);
            })
            .Returns(Task.CompletedTask);
        
        var cssFile = new Mock<IFormFile>();
        cssFile.Setup(f => f.FileName).Returns("style.css");
        cssFile.Setup(f => f.Length).Returns(50);
        cssFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) =>
            {
                var content = Encoding.UTF8.GetBytes("body { margin: 0; }");
                stream.Write(content, 0, content.Length);
            })
            .Returns(Task.CompletedTask);

        files.Add(htmlFile.Object);
        files.Add(cssFile.Object);
        
        return files;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testStoragePath))
        {
            Directory.Delete(_testStoragePath, true);
        }
    }
}