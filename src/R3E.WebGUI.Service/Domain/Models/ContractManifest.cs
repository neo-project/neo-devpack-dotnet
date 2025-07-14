using System.Text.Json.Serialization;

namespace R3E.WebGUI.Service.Domain.Models;

public class ContractManifestInfo
{
    public Guid Id { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string ManifestJson { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime FetchedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ContractMethod> Methods { get; set; } = new();
    public List<ContractEvent> Events { get; set; } = new();
    public bool IsValid { get; set; } = true;
}

public class ContractMethod
{
    public Guid Id { get; set; }
    public Guid ContractManifestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public bool Safe { get; set; }
    public List<ContractParameter> Parameters { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public ContractManifestInfo ContractManifest { get; set; } = null!;
}

public class ContractParameter
{
    public Guid Id { get; set; }
    public Guid ContractMethodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Optional { get; set; }
    public string? DefaultValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public ContractMethod ContractMethod { get; set; } = null!;
}

public class ContractEvent
{
    public Guid Id { get; set; }
    public Guid ContractManifestId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ContractEventParameter> Parameters { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public ContractManifestInfo ContractManifest { get; set; } = null!;
}

public class ContractEventParameter
{
    public Guid Id { get; set; }
    public Guid ContractEventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public ContractEvent ContractEvent { get; set; } = null!;
}

public class ContractAssetInfo
{
    public Guid Id { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string AssetHash { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Decimals { get; set; }
    public string TotalSupply { get; set; } = "0";
    public DateTime LastUpdated { get; set; }
    public bool IsNative { get; set; }
    public string AssetType { get; set; } = "NEP-17"; // NEP-17, NEP-11, Native
}

public class ContractTransaction
{
    public Guid Id { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string TransactionHash { get; set; } = string.Empty;
    public string BlockHash { get; set; } = string.Empty;
    public uint BlockIndex { get; set; }
    public DateTime Timestamp { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Parameters { get; set; } = "[]"; // JSON array
    public string Result { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string GasConsumed { get; set; } = "0";
    public string Exception { get; set; } = string.Empty;
}

public class ContractBalance
{
    public Guid Id { get; set; }
    public string ContractAddress { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string AssetHash { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
    public DateTime LastUpdated { get; set; }
    public ContractAssetInfo? AssetInfo { get; set; }
}

// DTO classes for API responses
public class ContractInfo
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public List<MethodInfo> Methods { get; set; } = new();
    public List<EventInfo> Events { get; set; } = new();
    public List<AssetInfo> Assets { get; set; } = new();
    public List<BalanceInfo> Balances { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class MethodInfo
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public bool Safe { get; set; }
    public List<ParameterInfo> Parameters { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

public class ParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Optional { get; set; }
    public string? DefaultValue { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class EventInfo
{
    public string Name { get; set; } = string.Empty;
    public List<EventParameterInfo> Parameters { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

public class EventParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class AssetInfo
{
    public string Hash { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Decimals { get; set; }
    public string TotalSupply { get; set; } = "0";
    public string AssetType { get; set; } = string.Empty;
}

public class BalanceInfo
{
    public string AssetHash { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
    public int Decimals { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class TransactionInfo
{
    public string Hash { get; set; } = string.Empty;
    public string BlockHash { get; set; } = string.Empty;
    public uint BlockIndex { get; set; }
    public DateTime Timestamp { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public object[] Parameters { get; set; } = Array.Empty<object>();
    public string Result { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string GasConsumed { get; set; } = "0";
    public string? Exception { get; set; }
}