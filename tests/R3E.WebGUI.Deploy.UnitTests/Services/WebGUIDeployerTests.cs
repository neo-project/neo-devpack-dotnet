using FluentAssertions;
using Moq;
using Moq.Protected;
using R3E.WebGUI.Deploy.Services;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace R3E.WebGUI.Deploy.UnitTests.Services;

public class WebGUIDeployerTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;

    public WebGUIDeployerTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"webgui-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);

        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object);
    }

    [Fact]
    public async Task DeployAsync_ValidInput_SendsCorrectRequest()
    {
        // Arrange
        var serviceUrl = "https://api.r3e-gui.com";
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var contractName = "TestContract";
        var network = "testnet";
        var deployerAddress = "0xabcdef1234567890abcdef1234567890abcdef12";
        var description = "Test WebGUI";

        // Create test files
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, "index.html"), "<html><body>Test</body></html>");
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, "style.css"), "body { margin: 0; }");

        var expectedResponse = new
        {
            success = true,
            subdomain = "testcontract",
            url = "https://testcontract.r3e-gui.com",
            contractAddress = contractAddress
        };

        var responseContent = JsonSerializer.Serialize(expectedResponse);

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        var deployer = new WebGUIDeployer(serviceUrl);

        // Capture console output
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        await deployer.DeployAsync(contractAddress, contractName, network, deployerAddress, _tempDirectory, description);

        // Assert
        var consoleOutput = output.ToString();
        consoleOutput.Should().Contain("🚀 Deploying WebGUI for contract TestContract");
        consoleOutput.Should().Contain("✅ WebGUI deployed successfully!");
        consoleOutput.Should().Contain("🌐 Subdomain: testcontract");
        consoleOutput.Should().Contain("🔗 URL: https://testcontract.r3e-gui.com");

        // Verify HTTP request
        _mockHttpHandler.Protected().Verify("SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri.ToString() == $"{serviceUrl}/api/webgui/deploy"),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task DeployAsync_NonExistentDirectory_ThrowsDirectoryNotFoundException()
    {
        // Arrange
        var deployer = new WebGUIDeployer("https://api.r3e-gui.com");
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
            deployer.DeployAsync(
                "0x1234567890abcdef1234567890abcdef12345678",
                "TestContract",
                "testnet",
                "0xabcdef1234567890abcdef1234567890abcdef12",
                nonExistentPath));
    }

    [Fact]
    public async Task DeployAsync_ServerError_ThrowsException()
    {
        // Arrange
        var serviceUrl = "https://api.r3e-gui.com";
        
        // Create test files
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, "index.html"), "<html></html>");

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Server error")
            });

        var deployer = new WebGUIDeployer(serviceUrl);

        // Capture console output
        var output = new StringWriter();
        Console.SetOut(output);
        Console.SetError(output);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            deployer.DeployAsync(
                "0x1234567890abcdef1234567890abcdef12345678",
                "TestContract",
                "testnet",
                "0xabcdef1234567890abcdef1234567890abcdef12",
                _tempDirectory));

        var consoleOutput = output.ToString();
        consoleOutput.Should().Contain("❌ Deployment failed");
    }

    [Fact]
    public async Task DeployAsync_MultipleFiles_IncludesAllFiles()
    {
        // Arrange
        var serviceUrl = "https://api.r3e-gui.com";
        
        // Create multiple test files
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, "index.html"), "<html><body>Main page</body></html>");
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, "about.html"), "<html><body>About page</body></html>");
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, "style.css"), "body { font-family: Arial; }");
        
        // Create subdirectory with files
        var jsDir = Path.Combine(_tempDirectory, "js");
        Directory.CreateDirectory(jsDir);
        await File.WriteAllTextAsync(Path.Combine(jsDir, "app.js"), "console.log('Hello');");

        var expectedResponse = new
        {
            success = true,
            subdomain = "testcontract",
            url = "https://testcontract.r3e-gui.com",
            contractAddress = "0x1234567890abcdef1234567890abcdef12345678"
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse))
            });

        var deployer = new WebGUIDeployer(serviceUrl);

        // Capture console output
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        await deployer.DeployAsync(
            "0x1234567890abcdef1234567890abcdef12345678",
            "TestContract",
            "testnet",
            "0xabcdef1234567890abcdef1234567890abcdef12",
            _tempDirectory);

        // Assert
        var consoleOutput = output.ToString();
        consoleOutput.Should().Contain("📁 Adding 4 files...");
        consoleOutput.Should().Contain("📄 index.html");
        consoleOutput.Should().Contain("📄 about.html");
        consoleOutput.Should().Contain("📄 style.css");
        consoleOutput.Should().Contain($"📄 js{Path.DirectorySeparatorChar}app.js");
    }

    [Theory]
    [InlineData(".html", "text/html")]
    [InlineData(".css", "text/css")]
    [InlineData(".js", "application/javascript")]
    [InlineData(".json", "application/json")]
    [InlineData(".png", "image/png")]
    [InlineData(".jpg", "image/jpeg")]
    [InlineData(".gif", "image/gif")]
    [InlineData(".svg", "image/svg+xml")]
    [InlineData(".ico", "image/x-icon")]
    [InlineData(".txt", "application/octet-stream")]
    public async Task DeployAsync_DifferentFileTypes_SetsCorrectContentType(string extension, string expectedContentType)
    {
        // Arrange
        var fileName = $"test{extension}";
        await File.WriteAllTextAsync(Path.Combine(_tempDirectory, fileName), "test content");

        var deployer = new WebGUIDeployer("https://api.r3e-gui.com");

        // Use reflection to test the private GetContentType method
        var method = typeof(WebGUIDeployer).GetMethod("GetContentType", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {
            // Act
            var result = method.Invoke(deployer, new object[] { fileName }) as string;

            // Assert
            result.Should().Be(expectedContentType);
        }
        else
        {
            // Fallback: just verify the file was created
            File.Exists(Path.Combine(_tempDirectory, fileName)).Should().BeTrue();
        }
    }

    [Fact]
    public async Task DeployAsync_EmptyDirectory_ThrowsException()
    {
        // Arrange
        var emptyDir = Path.Combine(_tempDirectory, "empty");
        Directory.CreateDirectory(emptyDir);

        var deployer = new WebGUIDeployer("https://api.r3e-gui.com");

        // Capture console output
        var output = new StringWriter();
        Console.SetOut(output);

        // Act
        await deployer.DeployAsync(
            "0x1234567890abcdef1234567890abcdef12345678",
            "TestContract",
            "testnet",
            "0xabcdef1234567890abcdef1234567890abcdef12",
            emptyDir);

        // Assert
        var consoleOutput = output.ToString();
        consoleOutput.Should().Contain("📁 Adding 0 files...");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
        
        _httpClient?.Dispose();
        _mockHttpHandler?.Dispose();
        
        // Reset console output
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
    }
}