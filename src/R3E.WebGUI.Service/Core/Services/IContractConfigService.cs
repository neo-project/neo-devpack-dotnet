using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.Core.Services;

public interface IContractConfigService
{
    /// <summary>
    /// Load contract configuration from JSON file
    /// </summary>
    Task<ContractWebGUIConfig?> LoadConfigAsync(string contractAddress);

    /// <summary>
    /// Save contract configuration as JSON file
    /// </summary>
    Task SaveConfigAsync(ContractWebGUIConfig config);

    /// <summary>
    /// Save contract configuration as JSON file with explicit address
    /// </summary>
    Task SaveConfigAsync(string contractAddress, ContractWebGUIConfig config);

    /// <summary>
    /// Generate contract configuration from manifest
    /// </summary>
    Task<ContractWebGUIConfig> GenerateConfigFromManifestAsync(
        string contractAddress,
        string contractName,
        string network,
        string deployerAddress,
        string manifestJson,
        string? description = null);

    /// <summary>
    /// Validate contract configuration
    /// </summary>
    Task<(bool IsValid, List<string> Errors)> ValidateConfigAsync(ContractWebGUIConfig config);

    /// <summary>
    /// Get the config file path for a contract
    /// </summary>
    string GetConfigFilePath(string contractAddress);
}