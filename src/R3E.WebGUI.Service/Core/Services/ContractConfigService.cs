using System.Text.Json;
using R3E.WebGUI.Service.Domain.Models;
using Neo.SmartContract.Manifest;

namespace R3E.WebGUI.Service.Core.Services;

public class ContractConfigService : IContractConfigService
{
    private readonly IStorageService _storageService;
    private readonly ILogger<ContractConfigService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ContractConfigService(
        IStorageService storageService,
        ILogger<ContractConfigService> logger)
    {
        _storageService = storageService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<ContractWebGUIConfig?> LoadConfigAsync(string contractAddress)
    {
        try
        {
            var configPath = GetConfigFileName(contractAddress);
            var configData = await _storageService.GetFileContentAsync("configs", configPath);
            
            if (string.IsNullOrEmpty(configData))
                return null;

            return JsonSerializer.Deserialize<ContractWebGUIConfig>(configData, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading config for contract {ContractAddress}", contractAddress);
            return null;
        }
    }

    public async Task SaveConfigAsync(ContractWebGUIConfig config)
    {
        try
        {
            var configPath = GetConfigFileName(config.ContractAddress);
            var configJson = JsonSerializer.Serialize(config, _jsonOptions);
            
            await _storageService.SaveFileAsync("configs", configPath, configJson);
            _logger.LogInformation("Saved config for contract {ContractAddress}", config.ContractAddress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving config for contract {ContractAddress}", config.ContractAddress);
            throw;
        }
    }

    public async Task SaveConfigAsync(string contractAddress, ContractWebGUIConfig config)
    {
        config.ContractAddress = contractAddress;
        await SaveConfigAsync(config);
    }

    public async Task<ContractWebGUIConfig> GenerateConfigFromManifestAsync(
        string contractAddress,
        string contractName,
        string network,
        string deployerAddress,
        string manifestJson,
        string? description = null)
    {
        try
        {
            var manifest = JsonSerializer.Deserialize<ContractManifest>(manifestJson);
            
            var config = new ContractWebGUIConfig
            {
                ContractAddress = contractAddress,
                ContractName = contractName,
                Description = description ?? $"Smart contract interface for {contractName}",
                DeployerAddress = deployerAddress,
                Network = network,
                DeployedAt = DateTime.UtcNow,
                Version = "1.0",
                Methods = new List<ContractMethodConfig>(),
                Events = new List<ContractEventConfig>(),
                RpcEndpoints = GetDefaultRpcEndpoints(),
                Theme = GetDefaultTheme(),
                Features = new ContractFeatures
                {
                    RealTimeUpdates = true,
                    TransactionHistory = true,
                    EventMonitoring = true,
                    GasEstimation = true,
                    MultiWallet = true,
                    Analytics = false,
                    Notifications = true
                }
            };

            // Convert manifest methods to config methods
            if (manifest?.Abi?.Methods != null)
            {
                foreach (var method in manifest.Abi.Methods)
                {
                    var methodConfig = new ContractMethodConfig
                    {
                        Name = method.Name,
                        DisplayName = FormatDisplayName(method.Name),
                        Description = $"Invoke {method.Name} method",
                        ReturnType = method.ReturnType != null ? method.ReturnType.ToString() : "void",
                        IsReadOnly = method.Safe,
                        Parameters = new List<ContractParameterConfig>(),
                        Category = DetermineMethodCategory(method.Name),
                        GasEstimate = method.Safe ? 0.01m : 0.05m
                    };

                    // Convert parameters
                    if (method.Parameters != null)
                    {
                        foreach (var param in method.Parameters)
                        {
                            methodConfig.Parameters.Add(new ContractParameterConfig
                            {
                                Name = param.Name,
                                Type = param.Type != null ? param.Type.ToString() : "string",
                                DisplayName = FormatDisplayName(param.Name),
                                Description = $"{param.Name} parameter",
                                Required = true
                            });
                        }
                    }

                    config.Methods.Add(methodConfig);
                }
            }

            // Convert manifest events to config events
            if (manifest?.Abi?.Events != null)
            {
                foreach (var evt in manifest.Abi.Events)
                {
                    var eventConfig = new ContractEventConfig
                    {
                        Name = evt.Name,
                        DisplayName = FormatDisplayName(evt.Name),
                        Description = $"{evt.Name} event",
                        Parameters = new List<ContractParameterConfig>()
                    };

                    // Convert event parameters
                    if (evt.Parameters != null)
                    {
                        foreach (var param in evt.Parameters)
                        {
                            eventConfig.Parameters.Add(new ContractParameterConfig
                            {
                                Name = param.Name,
                                Type = param.Type != null ? param.Type.ToString() : "string",
                                DisplayName = FormatDisplayName(param.Name),
                                Description = $"{param.Name} parameter"
                            });
                        }
                    }

                    config.Events.Add(eventConfig);
                }
            }

            await SaveConfigAsync(config);
            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating config from manifest for {ContractAddress}", contractAddress);
            throw;
        }
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateConfigAsync(ContractWebGUIConfig config)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(config.ContractAddress))
            errors.Add("Contract address is required");

        if (string.IsNullOrWhiteSpace(config.ContractName))
            errors.Add("Contract name is required");

        if (string.IsNullOrWhiteSpace(config.Network))
            errors.Add("Network is required");

        if (string.IsNullOrWhiteSpace(config.DeployerAddress))
            errors.Add("Deployer address is required");

        // Validate contract address format
        if (!string.IsNullOrWhiteSpace(config.ContractAddress) && 
            !config.ContractAddress.StartsWith("0x") && 
            config.ContractAddress.Length != 42)
        {
            errors.Add("Contract address must be a valid hex address");
        }

        // Validate methods
        foreach (var method in config.Methods)
        {
            if (string.IsNullOrWhiteSpace(method.Name))
                errors.Add("Method name is required");

            foreach (var param in method.Parameters)
            {
                if (string.IsNullOrWhiteSpace(param.Name))
                    errors.Add($"Parameter name is required for method {method.Name}");
                
                if (string.IsNullOrWhiteSpace(param.Type))
                    errors.Add($"Parameter type is required for {method.Name}.{param.Name}");
            }
        }

        return (errors.Count == 0, errors);
    }

    public string GetConfigFilePath(string contractAddress)
    {
        return Path.Combine("configs", GetConfigFileName(contractAddress));
    }

    private string GetConfigFileName(string contractAddress)
    {
        return $"{contractAddress.ToLowerInvariant()}.webgui.json";
    }

    private Dictionary<string, string> GetDefaultRpcEndpoints()
    {
        return new Dictionary<string, string>
        {
            ["testnet"] = "https://test1.neo.coz.io:443",
            ["mainnet"] = "https://rpc1.neo.coz.io:443",
            ["local"] = "http://localhost:20332"
        };
    }

    private WebGUITheme GetDefaultTheme()
    {
        return new WebGUITheme
        {
            PrimaryColor = "#667eea",
            SecondaryColor = "#00d4aa",
            AccentColor = "#764ba2",
            BackgroundColor = "#f8fafc",
            CardColor = "#ffffff",
            TextColor = "#1a1a1a",
            BorderRadius = "12px",
            FontFamily = "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
            Layout = "modern"
        };
    }

    private string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        
        // Convert camelCase to Title Case
        var result = string.Empty;
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
                result += " ";
            result += i == 0 ? char.ToUpper(name[i]) : name[i];
        }
        return result;
    }

    private string DetermineMethodCategory(string methodName)
    {
        var lowerName = methodName.ToLowerInvariant();
        
        if (lowerName.StartsWith("get") || lowerName.StartsWith("is") || lowerName.StartsWith("has"))
            return "Query";
        
        if (lowerName.StartsWith("set") || lowerName.StartsWith("update") || lowerName.StartsWith("change"))
            return "Modify";
        
        if (lowerName.Contains("transfer") || lowerName.Contains("send") || lowerName.Contains("mint") || lowerName.Contains("burn"))
            return "Token";
        
        if (lowerName.Contains("owner") || lowerName.Contains("admin") || lowerName.Contains("permission"))
            return "Admin";
        
        return "General";
    }
}