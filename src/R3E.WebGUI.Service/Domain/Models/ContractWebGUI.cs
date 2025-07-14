namespace R3E.WebGUI.Service.Domain.Models;

public class ContractWebGUI
{
    public Guid Id { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string ContractName { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string Subdomain { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DeployedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string DeployerAddress { get; set; } = string.Empty;
    public string WebGUIVersion { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public string StoragePath { get; set; } = string.Empty;
    public long ViewCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    // Enhanced contract information
    public Guid? ContractManifestId { get; set; }
    public ContractManifestInfo? ContractManifest { get; set; }
    public string WebGUIType { get; set; } = "custom"; // "auto-generated", "custom", "template"
    public string TemplateId { get; set; } = string.Empty;
    public bool AutoUpdate { get; set; } = true; // Auto-update from contract changes
    public DateTime? LastContractSync { get; set; }
    public string ContractHash { get; set; } = string.Empty; // NEF hash for validation
    public List<string> SupportedWallets { get; set; } = new() { "neoline", "o3", "walletconnect" };
    public bool EnableRealTimeData { get; set; } = true;
    public string CustomDomain { get; set; } = string.Empty;
    public Dictionary<string, string> ThemeSettings { get; set; } = new();
}