using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using R3E.WebGUI.Service.Core.Services;
using R3E.WebGUI.Service.Domain.Models;
using R3E.WebGUI.Service.Infrastructure.Repositories;
using System.Text;

namespace R3E.WebGUI.Service.UnitTests.Services;

public class WebGUIServiceTests
{
    private readonly Mock<IWebGUIRepository> _mockRepository;
    private readonly Mock<IStorageService> _mockStorageService;
    private readonly Mock<ILogger<WebGUIService>> _mockLogger;
    private readonly WebGUIService _service;

    public WebGUIServiceTests()
    {
        _mockRepository = new Mock<IWebGUIRepository>();
        _mockStorageService = new Mock<IStorageService>();
        _mockLogger = new Mock<ILogger<WebGUIService>>();
        _service = new WebGUIService(_mockRepository.Object, _mockStorageService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task DeployWebGUIAsync_ValidInput_ReturnsContractWebGUI()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var contractName = "TestContract";
        var network = "testnet";
        var deployerAddress = "0xabcdef1234567890abcdef1234567890abcdef12";
        var description = "Test description";
        
        var files = CreateMockFiles();
        var storagePath = "/storage/testcontract";

        _mockRepository.Setup(r => r.SubdomainExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockStorageService.Setup(s => s.UploadWebGUIFilesAsync(It.IsAny<string>(), It.IsAny<IFormFileCollection>()))
            .ReturnsAsync(storagePath);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<ContractWebGUI>()))
            .ReturnsAsync((ContractWebGUI webGUI) => webGUI);

        // Act
        var result = await _service.DeployWebGUIAsync(contractAddress, contractName, network, files, deployerAddress, description);

        // Assert
        result.Should().NotBeNull();
        result.ContractAddress.Should().Be(contractAddress.ToLowerInvariant());
        result.ContractName.Should().Be(contractName);
        result.Network.Should().Be(network);
        result.DeployerAddress.Should().Be(deployerAddress);
        result.Description.Should().Be(description);
        result.IsActive.Should().BeTrue();
        result.StoragePath.Should().Be(storagePath);
        
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<ContractWebGUI>()), Times.Once);
        _mockStorageService.Verify(s => s.UploadWebGUIFilesAsync(It.IsAny<string>(), files), Times.Once);
    }

    [Fact]
    public async Task DeployWebGUIAsync_InvalidContractAddress_ThrowsArgumentException()
    {
        // Arrange
        var invalidAddress = "invalid-address";
        var files = CreateMockFiles();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.DeployWebGUIAsync(invalidAddress, "Test", "testnet", files, "0xvalid", null));
    }

    [Fact]
    public async Task GetBySubdomainAsync_ExistingSubdomain_ReturnsWebGUIAndIncrementsViewCount()
    {
        // Arrange
        var subdomain = "testcontract";
        var webGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            Subdomain = subdomain,
            ViewCount = 5
        };

        _mockRepository.Setup(r => r.GetBySubdomainAsync(subdomain))
            .ReturnsAsync(webGUI);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ContractWebGUI>()))
            .ReturnsAsync((ContractWebGUI w) => w);

        // Act
        var result = await _service.GetBySubdomainAsync(subdomain);

        // Assert
        result.Should().NotBeNull();
        result.Subdomain.Should().Be(subdomain);
        result.ViewCount.Should().Be(6);
        
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<ContractWebGUI>(w => w.ViewCount == 6)), Times.Once);
    }

    [Fact]
    public async Task GetBySubdomainAsync_NonExistentSubdomain_ReturnsNull()
    {
        // Arrange
        var subdomain = "nonexistent";
        _mockRepository.Setup(r => r.GetBySubdomainAsync(subdomain))
            .ReturnsAsync((ContractWebGUI?)null);

        // Act
        var result = await _service.GetBySubdomainAsync(subdomain);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ContractWebGUI>()), Times.Never);
    }

    [Fact]
    public async Task SearchByContractAddressAsync_ValidAddress_ReturnsResults()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var network = "testnet";
        var expectedResults = new List<ContractWebGUI>
        {
            new() { ContractAddress = contractAddress, Network = network }
        };

        _mockRepository.Setup(r => r.SearchByContractAddressAsync(contractAddress, network))
            .ReturnsAsync(expectedResults);

        // Act
        var results = await _service.SearchByContractAddressAsync(contractAddress, network);

        // Assert
        results.Should().NotBeNull();
        results.Should().HaveCount(1);
        results.First().ContractAddress.Should().Be(contractAddress);
    }

    [Fact]
    public async Task GenerateUniqueSubdomainAsync_NameNotTaken_ReturnsOriginalName()
    {
        // Arrange
        var contractName = "TestContract";
        var expectedSubdomain = "testcontract";

        _mockRepository.Setup(r => r.SubdomainExistsAsync(expectedSubdomain))
            .ReturnsAsync(false);

        // Act
        var result = await _service.GenerateUniqueSubdomainAsync(contractName);

        // Assert
        result.Should().Be(expectedSubdomain);
    }

    [Fact]
    public async Task GenerateUniqueSubdomainAsync_NameTaken_ReturnsNameWithSuffix()
    {
        // Arrange
        var contractName = "TestContract";
        var baseSubdomain = "testcontract";
        var expectedSubdomain = "testcontract-1";

        _mockRepository.SetupSequence(r => r.SubdomainExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true)  // base name is taken
            .ReturnsAsync(false); // name with suffix is available

        // Act
        var result = await _service.GenerateUniqueSubdomainAsync(contractName);

        // Assert
        result.Should().Be(expectedSubdomain);
    }

    [Fact]
    public async Task GenerateUniqueSubdomainAsync_ShortName_AddsContractPrefix()
    {
        // Arrange
        var contractName = "AB";
        var expectedSubdomain = "contract-ab";

        _mockRepository.Setup(r => r.SubdomainExistsAsync(expectedSubdomain))
            .ReturnsAsync(false);

        // Act
        var result = await _service.GenerateUniqueSubdomainAsync(contractName);

        // Assert
        result.Should().Be(expectedSubdomain);
    }

    [Fact]
    public async Task GenerateUniqueSubdomainAsync_SpecialCharacters_SanitizesName()
    {
        // Arrange
        var contractName = "Test-Contract_2024!@#";
        var expectedSubdomain = "test-contract-2024";

        _mockRepository.Setup(r => r.SubdomainExistsAsync(expectedSubdomain))
            .ReturnsAsync(false);

        // Act
        var result = await _service.GenerateUniqueSubdomainAsync(contractName);

        // Assert
        result.Should().Be(expectedSubdomain);
    }

    [Fact]
    public async Task UpdateWebGUIAsync_ExistingSubdomain_UpdatesSuccessfully()
    {
        // Arrange
        var subdomain = "testcontract";
        var newDescription = "Updated description";
        var files = CreateMockFiles();
        var storagePath = "/storage/updated";

        var existingWebGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            Subdomain = subdomain,
            Description = "Old description"
        };

        _mockRepository.Setup(r => r.GetBySubdomainAsync(subdomain))
            .ReturnsAsync(existingWebGUI);
        _mockStorageService.Setup(s => s.UploadWebGUIFilesAsync(subdomain, files))
            .ReturnsAsync(storagePath);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ContractWebGUI>()))
            .ReturnsAsync((ContractWebGUI w) => w);

        // Act
        var result = await _service.UpdateWebGUIAsync(subdomain, files, newDescription);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(newDescription);
        result.StoragePath.Should().Be(storagePath);
        result.UpdatedAt.Should().NotBeNull();
        
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ContractWebGUI>()), Times.Once);
    }

    [Fact]
    public async Task UpdateWebGUIAsync_NonExistentSubdomain_ThrowsInvalidOperationException()
    {
        // Arrange
        var subdomain = "nonexistent";
        _mockRepository.Setup(r => r.GetBySubdomainAsync(subdomain))
            .ReturnsAsync((ContractWebGUI?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateWebGUIAsync(subdomain, null, "new description"));
    }

    [Fact]
    public async Task DeleteWebGUIAsync_ExistingSubdomain_MarksAsInactiveAndDeletesFiles()
    {
        // Arrange
        var subdomain = "testcontract";
        var webGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            Subdomain = subdomain,
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetBySubdomainAsync(subdomain))
            .ReturnsAsync(webGUI);
        _mockStorageService.Setup(s => s.DeleteWebGUIFilesAsync(subdomain))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ContractWebGUI>()))
            .ReturnsAsync((ContractWebGUI w) => w);

        // Act
        var result = await _service.DeleteWebGUIAsync(subdomain);

        // Assert
        result.Should().BeTrue();
        webGUI.IsActive.Should().BeFalse();
        
        _mockStorageService.Verify(s => s.DeleteWebGUIFilesAsync(subdomain), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<ContractWebGUI>(w => !w.IsActive)), Times.Once);
    }

    [Fact]
    public async Task DeleteWebGUIAsync_NonExistentSubdomain_ReturnsFalse()
    {
        // Arrange
        var subdomain = "nonexistent";
        _mockRepository.Setup(r => r.GetBySubdomainAsync(subdomain))
            .ReturnsAsync((ContractWebGUI?)null);

        // Act
        var result = await _service.DeleteWebGUIAsync(subdomain);

        // Assert
        result.Should().BeFalse();
        _mockStorageService.Verify(s => s.DeleteWebGUIFilesAsync(It.IsAny<string>()), Times.Never);
    }

    private IFormFileCollection CreateMockFiles()
    {
        var files = new FormFileCollection();
        
        var mockFile1 = new Mock<IFormFile>();
        mockFile1.Setup(f => f.FileName).Returns("index.html");
        mockFile1.Setup(f => f.Length).Returns(1024);
        mockFile1.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("<html></html>")));
        
        var mockFile2 = new Mock<IFormFile>();
        mockFile2.Setup(f => f.FileName).Returns("style.css");
        mockFile2.Setup(f => f.Length).Returns(512);
        mockFile2.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("body { margin: 0; }")));

        files.Add(mockFile1.Object);
        files.Add(mockFile2.Object);

        return files;
    }
}