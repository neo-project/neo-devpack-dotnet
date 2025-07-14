using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using R3E.WebGUI.Service.Domain.Models;
using R3E.WebGUI.Service.Infrastructure.Data;
using R3E.WebGUI.Service.Infrastructure.Repositories;
using Xunit;

namespace R3E.WebGUI.Service.UnitTests.Repositories;

public class WebGUIRepositoryTests : IDisposable
{
    private readonly WebGUIDbContext _context;
    private readonly WebGUIRepository _repository;

    public WebGUIRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WebGUIDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new WebGUIDbContext(options);
        _repository = new WebGUIRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ValidWebGUI_ReturnsCreatedWebGUI()
    {
        // Arrange
        var webGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            ContractAddress = "0x1234567890abcdef1234567890abcdef12345678",
            ContractName = "TestContract",
            Network = "testnet",
            Subdomain = "testcontract",
            DeployerAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
            StoragePath = "/storage/test",
            DeployedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateAsync(webGUI);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(webGUI.Id);
        
        var savedWebGUI = await _context.WebGUIs.FindAsync(webGUI.Id);
        savedWebGUI.Should().NotBeNull();
        savedWebGUI!.ContractAddress.Should().Be(webGUI.ContractAddress);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsWebGUI()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();

        // Act
        var result = await _repository.GetByIdAsync(webGUI.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(webGUI.Id);
        result.ContractName.Should().Be(webGUI.ContractName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySubdomainAsync_ExistingActiveSubdomain_ReturnsWebGUI()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();

        // Act
        var result = await _repository.GetBySubdomainAsync(webGUI.Subdomain);

        // Assert
        result.Should().NotBeNull();
        result!.Subdomain.Should().Be(webGUI.Subdomain);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetBySubdomainAsync_InactiveSubdomain_ReturnsNull()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();
        webGUI.IsActive = false;
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySubdomainAsync(webGUI.Subdomain);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchByContractAddressAsync_ExistingAddress_ReturnsResults()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        var network = "testnet";
        
        var webGUI1 = await CreateTestWebGUI(contractAddress, "Contract1", network);
        var webGUI2 = await CreateTestWebGUI(contractAddress, "Contract2", network);
        await CreateTestWebGUI("0xdifferentaddress", "Contract3", network); // Different address

        // Act
        var results = await _repository.SearchByContractAddressAsync(contractAddress, network);

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(w => w.ContractAddress == contractAddress);
        results.Should().OnlyContain(w => w.Network == network);
        results.Should().BeInDescendingOrder(w => w.DeployedAt);
    }

    [Fact]
    public async Task SearchByContractAddressAsync_WithoutNetworkFilter_ReturnsAllNetworks()
    {
        // Arrange
        var contractAddress = "0x1234567890abcdef1234567890abcdef12345678";
        
        await CreateTestWebGUI(contractAddress, "Contract1", "testnet");
        await CreateTestWebGUI(contractAddress, "Contract2", "mainnet");

        // Act
        var results = await _repository.SearchByContractAddressAsync(contractAddress, null);

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(w => w.ContractAddress == contractAddress);
    }

    [Fact]
    public async Task GetPagedAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var network = "testnet";
        for (int i = 0; i < 15; i++)
        {
            await CreateTestWebGUI($"0x{i:D40}", $"Contract{i}", network);
        }

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(2, 5, network);

        // Assert
        items.Should().HaveCount(5);
        totalCount.Should().Be(15);
        items.Should().BeInDescendingOrder(w => w.DeployedAt);
    }

    [Fact]
    public async Task GetPagedAsync_WithNetworkFilter_ReturnsOnlyMatchingNetwork()
    {
        // Arrange
        await CreateTestWebGUI("0x1234", "Contract1", "testnet");
        await CreateTestWebGUI("0x5678", "Contract2", "testnet");
        await CreateTestWebGUI("0x9abc", "Contract3", "mainnet");

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(1, 10, "testnet");

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(w => w.Network == "testnet");
    }

    [Fact]
    public async Task UpdateAsync_ExistingWebGUI_UpdatesSuccessfully()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();
        var newDescription = "Updated description";
        webGUI.Description = newDescription;
        webGUI.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(webGUI);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be(newDescription);
        
        var updatedWebGUI = await _context.WebGUIs.FindAsync(webGUI.Id);
        updatedWebGUI!.Description.Should().Be(newDescription);
        updatedWebGUI.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ExistingWebGUI_DeletesSuccessfully()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();

        // Act
        var result = await _repository.DeleteAsync(webGUI.Id);

        // Assert
        result.Should().BeTrue();
        
        var deletedWebGUI = await _context.WebGUIs.FindAsync(webGUI.Id);
        deletedWebGUI.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentWebGUI_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SubdomainExistsAsync_ExistingActiveSubdomain_ReturnsTrue()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();

        // Act
        var result = await _repository.SubdomainExistsAsync(webGUI.Subdomain);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SubdomainExistsAsync_InactiveSubdomain_ReturnsFalse()
    {
        // Arrange
        var webGUI = await CreateTestWebGUI();
        webGUI.IsActive = false;
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SubdomainExistsAsync(webGUI.Subdomain);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SubdomainExistsAsync_NonExistentSubdomain_ReturnsFalse()
    {
        // Act
        var result = await _repository.SubdomainExistsAsync("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    private async Task<ContractWebGUI> CreateTestWebGUI(
        string? contractAddress = null,
        string? contractName = null,
        string? network = null)
    {
        var webGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            ContractAddress = contractAddress ?? "0x1234567890abcdef1234567890abcdef12345678",
            ContractName = contractName ?? "TestContract",
            Network = network ?? "testnet",
            Subdomain = $"test-{Guid.NewGuid():N}",
            DeployerAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
            StoragePath = "/storage/test",
            DeployedAt = DateTime.UtcNow,
            IsActive = true,
            Description = "Test WebGUI"
        };

        _context.WebGUIs.Add(webGUI);
        await _context.SaveChangesAsync();
        return webGUI;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}