using System.Text.Json.Serialization;

namespace R3E.WebGUI.Service.Domain.Models;

public class ContractWebGUIConfig
{
    [JsonPropertyName("contractAddress")]
    public string ContractAddress { get; set; } = string.Empty;

    [JsonPropertyName("contractName")]
    public string ContractName { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("deployer")]
    public string DeployerAddress { get; set; } = string.Empty;

    [JsonPropertyName("network")]
    public string Network { get; set; } = string.Empty;

    [JsonPropertyName("deployedAt")]
    public DateTime DeployedAt { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("pluginDownloadUrl")]
    public string? PluginDownloadUrl { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("author")]
    public ContractAuthor? Author { get; set; }

    [JsonPropertyName("license")]
    public string? License { get; set; }

    [JsonPropertyName("repository")]
    public string? Repository { get; set; }

    [JsonPropertyName("methods")]
    public List<ContractMethodConfig> Methods { get; set; } = new();

    [JsonPropertyName("events")]
    public List<ContractEventConfig> Events { get; set; } = new();

    [JsonPropertyName("supportedWallets")]
    public List<string> SupportedWallets { get; set; } = new() { "neoline", "o3", "walletconnect" };

    [JsonPropertyName("theme")]
    public WebGUITheme Theme { get; set; } = new();

    [JsonPropertyName("rpcEndpoints")]
    public Dictionary<string, string> RpcEndpoints { get; set; } = new();

    [JsonPropertyName("features")]
    public ContractFeatures Features { get; set; } = new();
}

public class ContractAuthor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("github")]
    public string? Github { get; set; }
}

public class ContractMethodConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("returnType")]
    public string ReturnType { get; set; } = string.Empty;

    [JsonPropertyName("parameters")]
    public List<ContractParameterConfig> Parameters { get; set; } = new();

    [JsonPropertyName("isReadOnly")]
    public bool IsReadOnly { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("gasEstimate")]
    public decimal? GasEstimate { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; } = false;
}

public class ContractParameterConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; } = true;

    [JsonPropertyName("defaultValue")]
    public object? DefaultValue { get; set; }

    [JsonPropertyName("validation")]
    public ParameterValidation? Validation { get; set; }
}

public class ParameterValidation
{
    [JsonPropertyName("minLength")]
    public int? MinLength { get; set; }

    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("pattern")]
    public string? Pattern { get; set; }

    [JsonPropertyName("min")]
    public decimal? Min { get; set; }

    [JsonPropertyName("max")]
    public decimal? Max { get; set; }
}

public class ContractEventConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("parameters")]
    public List<ContractParameterConfig> Parameters { get; set; } = new();
}

public class WebGUITheme
{
    [JsonPropertyName("primaryColor")]
    public string PrimaryColor { get; set; } = "#667eea";

    [JsonPropertyName("secondaryColor")]
    public string SecondaryColor { get; set; } = "#00d4aa";

    [JsonPropertyName("accentColor")]
    public string AccentColor { get; set; } = "#764ba2";

    [JsonPropertyName("backgroundColor")]
    public string BackgroundColor { get; set; } = "#f8fafc";

    [JsonPropertyName("cardColor")]
    public string CardColor { get; set; } = "#ffffff";

    [JsonPropertyName("textColor")]
    public string TextColor { get; set; } = "#1a1a1a";

    [JsonPropertyName("borderRadius")]
    public string BorderRadius { get; set; } = "8px";

    [JsonPropertyName("fontFamily")]
    public string FontFamily { get; set; } = "'Inter', -apple-system, BlinkMacSystemFont, sans-serif";

    [JsonPropertyName("layout")]
    public string Layout { get; set; } = "modern"; // modern, classic, minimal
}

public class ContractFeatures
{
    [JsonPropertyName("realTimeUpdates")]
    public bool RealTimeUpdates { get; set; } = true;

    [JsonPropertyName("transactionHistory")]
    public bool TransactionHistory { get; set; } = true;

    [JsonPropertyName("eventMonitoring")]
    public bool EventMonitoring { get; set; } = true;

    [JsonPropertyName("gasEstimation")]
    public bool GasEstimation { get; set; } = true;

    [JsonPropertyName("multiWallet")]
    public bool MultiWallet { get; set; } = true;

    [JsonPropertyName("analytics")]
    public bool Analytics { get; set; } = false;

    [JsonPropertyName("notifications")]
    public bool Notifications { get; set; } = true;
}