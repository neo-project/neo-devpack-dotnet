using System.Text.RegularExpressions;
using R3E.WebGUI.Service.Domain.Models;
using R3E.WebGUI.Service.Infrastructure.Repositories;

namespace R3E.WebGUI.Service.Core.Services;

public class WebGUIService : IWebGUIService
{
    private readonly IWebGUIRepository _repository;
    private readonly IStorageService _storageService;
    private readonly ILogger<WebGUIService> _logger;

    public WebGUIService(
        IWebGUIRepository repository,
        IStorageService storageService,
        ILogger<WebGUIService> logger)
    {
        _repository = repository;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<ContractWebGUI> DeployWebGUIAsync(
        string contractAddress,
        string contractName,
        string network,
        IFormFileCollection files,
        string deployerAddress,
        string? description = null)
    {
        // Validate contract address format
        if (!IsValidContractAddress(contractAddress))
        {
            throw new ArgumentException("Invalid contract address format");
        }

        // Generate unique subdomain
        var subdomain = await GenerateUniqueSubdomainAsync(contractName);

        // Upload files to storage
        var storagePath = await _storageService.UploadWebGUIFilesAsync(subdomain, files);

        // Create WebGUI record
        var webGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            ContractAddress = contractAddress.ToLowerInvariant(),
            ContractName = contractName,
            Network = network.ToLowerInvariant(),
            Subdomain = subdomain,
            Description = description ?? $"WebGUI for {contractName}",
            DeployedAt = DateTime.UtcNow,
            DeployerAddress = deployerAddress,
            StoragePath = storagePath,
            IsActive = true,
            Metadata = new Dictionary<string, object>
            {
                ["fileCount"] = files.Count,
                ["totalSize"] = files.Sum(f => f.Length)
            }
        };

        await _repository.CreateAsync(webGUI);
        _logger.LogInformation($"Deployed WebGUI for contract {contractAddress} at subdomain {subdomain}");

        return webGUI;
    }

    public async Task<ContractWebGUI?> GetBySubdomainAsync(string subdomain)
    {
        var webGUI = await _repository.GetBySubdomainAsync(subdomain);
        if (webGUI != null)
        {
            // Increment view count
            webGUI.ViewCount++;
            await _repository.UpdateAsync(webGUI);
        }
        return webGUI;
    }

    public async Task<IEnumerable<ContractWebGUI>> SearchByContractAddressAsync(string contractAddress, string? network = null)
    {
        return await _repository.SearchByContractAddressAsync(contractAddress, network);
    }

    public async Task<PagedResult<ContractWebGUI>> ListWebGUIsAsync(int page, int pageSize, string? network = null)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize, network);
        return new PagedResult<ContractWebGUI>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ContractWebGUI> UpdateWebGUIAsync(string subdomain, IFormFileCollection? files, string? description)
    {
        var webGUI = await _repository.GetBySubdomainAsync(subdomain);
        if (webGUI == null)
        {
            throw new InvalidOperationException($"WebGUI with subdomain '{subdomain}' not found");
        }

        if (files != null && files.Any())
        {
            // Upload new files
            var storagePath = await _storageService.UploadWebGUIFilesAsync(subdomain, files);
            webGUI.StoragePath = storagePath;
            webGUI.Metadata["fileCount"] = files.Count;
            webGUI.Metadata["totalSize"] = files.Sum(f => f.Length);
        }

        if (!string.IsNullOrEmpty(description))
        {
            webGUI.Description = description;
        }

        webGUI.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(webGUI);

        _logger.LogInformation($"Updated WebGUI for subdomain {subdomain}");
        return webGUI;
    }

    public async Task<bool> DeleteWebGUIAsync(string subdomain)
    {
        var webGUI = await _repository.GetBySubdomainAsync(subdomain);
        if (webGUI == null)
        {
            return false;
        }

        // Delete files from storage
        await _storageService.DeleteWebGUIFilesAsync(subdomain);

        // Mark as inactive instead of hard delete
        webGUI.IsActive = false;
        await _repository.UpdateAsync(webGUI);

        _logger.LogInformation($"Deleted WebGUI for subdomain {subdomain}");
        return true;
    }

    public async Task<string> GenerateUniqueSubdomainAsync(string contractName)
    {
        // Sanitize contract name for subdomain
        var baseSubdomain = Regex.Replace(contractName.ToLowerInvariant(), @"[^a-z0-9-]", "-");
        baseSubdomain = Regex.Replace(baseSubdomain, @"-+", "-").Trim('-');

        // Ensure minimum length
        if (baseSubdomain.Length < 3)
        {
            baseSubdomain = $"contract-{baseSubdomain}";
        }

        // Check uniqueness and add suffix if needed
        var subdomain = baseSubdomain;
        var suffix = 1;
        while (await _repository.SubdomainExistsAsync(subdomain))
        {
            subdomain = $"{baseSubdomain}-{suffix}";
            suffix++;
        }

        return subdomain;
    }

    public async Task<ContractWebGUI> DeployWebGUIFromConfigAsync(
        ContractWebGUIConfig config,
        string deployerAddress)
    {
        // Validate contract address format
        if (!IsValidContractAddress(config.ContractAddress))
        {
            throw new ArgumentException("Invalid contract address format");
        }

        // Generate unique subdomain
        var subdomain = await GenerateUniqueSubdomainAsync(config.ContractName);

        // Save JSON configuration
        await _storageService.SaveConfigAsync(config.ContractAddress, config);

        // Create WebGUI record
        var webGUI = new ContractWebGUI
        {
            Id = Guid.NewGuid(),
            ContractAddress = config.ContractAddress.ToLowerInvariant(),
            ContractName = config.ContractName,
            Network = config.Network.ToLowerInvariant(),
            Subdomain = subdomain,
            Description = config.Description ?? $"WebGUI for {config.ContractName}",
            DeployedAt = DateTime.UtcNow,
            DeployerAddress = deployerAddress,
            StoragePath = $"configs/{config.ContractAddress}.webgui.json",
            IsActive = true,
            Metadata = new Dictionary<string, object>
            {
                ["configBased"] = true,
                ["version"] = config.Version,
                ["theme"] = config.Theme.Primary
            }
        };

        await _repository.CreateAsync(webGUI);
        _logger.LogInformation($"Deployed JSON-based WebGUI for contract {config.ContractAddress} at subdomain {subdomain}");

        return webGUI;
    }

    private bool IsValidContractAddress(string address)
    {
        // Neo contract address validation (0x prefix + 40 hex chars)
        return Regex.IsMatch(address, @"^0x[a-fA-F0-9]{40}$");
    }
}