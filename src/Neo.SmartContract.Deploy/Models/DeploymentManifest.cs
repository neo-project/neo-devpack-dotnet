using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Deployment manifest for multi-contract deployments
/// </summary>
public class DeploymentManifest
{
    /// <summary>
    /// Manifest format version
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";
    
    /// <summary>
    /// Name of the deployment
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the deployment
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// List of contracts to deploy
    /// </summary>
    [JsonPropertyName("contracts")]
    public List<ContractDefinition> Contracts { get; set; } = new();
    
    /// <summary>
    /// Contract interactions to setup after deployment
    /// </summary>
    [JsonPropertyName("interactions")]
    public List<ContractInteraction> Interactions { get; set; } = new();
    
    /// <summary>
    /// Global deployment settings
    /// </summary>
    [JsonPropertyName("settings")]
    public DeploymentSettings Settings { get; set; } = new();
    
    /// <summary>
    /// Continue deployment even if a contract fails
    /// </summary>
    [JsonPropertyName("continueOnError")]
    public bool ContinueOnError { get; set; }
    
    /// <summary>
    /// Enable transaction batching for deployment
    /// </summary>
    [JsonPropertyName("enableBatching")]
    public bool EnableBatching { get; set; }
    
    /// <summary>
    /// Maximum contracts per batch
    /// </summary>
    [JsonPropertyName("batchSize")]
    public int BatchSize { get; set; } = 5;
}

/// <summary>
/// Contract definition within a deployment manifest
/// </summary>
public class ContractDefinition
{
    /// <summary>
    /// Unique identifier for the contract
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Contract name
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Contract description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Path to contract project (.csproj)
    /// </summary>
    [JsonPropertyName("projectPath")]
    public string? ProjectPath { get; set; }
    
    /// <summary>
    /// Path to NEF file (for pre-compiled contracts)
    /// </summary>
    [JsonPropertyName("nefPath")]
    public string? NefPath { get; set; }
    
    /// <summary>
    /// Path to manifest file (for pre-compiled contracts)
    /// </summary>
    [JsonPropertyName("manifestPath")]
    public string? ManifestPath { get; set; }
    
    /// <summary>
    /// Initialization parameters
    /// </summary>
    [JsonPropertyName("initParams")]
    public List<object>? InitParams { get; set; }
    
    /// <summary>
    /// Contract dependencies (other contract IDs that must be deployed first)
    /// </summary>
    [JsonPropertyName("dependencies")]
    public List<string> Dependencies { get; set; } = new();
    
    /// <summary>
    /// Contract-specific deployment options
    /// </summary>
    [JsonPropertyName("options")]
    public ContractDeploymentOptions? Options { get; set; }
    
    /// <summary>
    /// Tags for categorization
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Expected contract hash (for verification)
    /// </summary>
    [JsonPropertyName("expectedHash")]
    public string? ExpectedHash { get; set; }
}

/// <summary>
/// Contract interaction definition
/// </summary>
public class ContractInteraction
{
    /// <summary>
    /// Source contract ID
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;
    
    /// <summary>
    /// Target contract ID
    /// </summary>
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;
    
    /// <summary>
    /// Method to call on source contract
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;
    
    /// <summary>
    /// Parameters for the method call
    /// </summary>
    [JsonPropertyName("params")]
    public List<object> Params { get; set; } = new();
    
    /// <summary>
    /// Description of the interaction
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Order of execution (lower numbers execute first)
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }
    
    /// <summary>
    /// Skip if interaction fails
    /// </summary>
    [JsonPropertyName("optional")]
    public bool Optional { get; set; }
}

/// <summary>
/// Global deployment settings
/// </summary>
public class DeploymentSettings
{
    /// <summary>
    /// Default gas limit for deployments
    /// </summary>
    [JsonPropertyName("defaultGasLimit")]
    public long DefaultGasLimit { get; set; } = 100_000_000;
    
    /// <summary>
    /// Default network fee
    /// </summary>
    [JsonPropertyName("defaultNetworkFee")]
    public long DefaultNetworkFee { get; set; } = 1_000_000;
    
    /// <summary>
    /// Verify contracts after deployment
    /// </summary>
    [JsonPropertyName("verifyAfterDeploy")]
    public bool VerifyAfterDeploy { get; set; } = true;
    
    /// <summary>
    /// Wait for transaction confirmation
    /// </summary>
    [JsonPropertyName("waitForConfirmation")]
    public bool WaitForConfirmation { get; set; } = true;
    
    /// <summary>
    /// Enable dry run mode
    /// </summary>
    [JsonPropertyName("dryRun")]
    public bool DryRun { get; set; }
    
    /// <summary>
    /// Rollback on any failure
    /// </summary>
    [JsonPropertyName("rollbackOnFailure")]
    public bool RollbackOnFailure { get; set; }
}

/// <summary>
/// Contract-specific deployment options
/// </summary>
public class ContractDeploymentOptions
{
    /// <summary>
    /// Override deployer account WIF
    /// </summary>
    [JsonPropertyName("wifKey")]
    public string? WifKey { get; set; }
    
    /// <summary>
    /// Override RPC URL
    /// </summary>
    [JsonPropertyName("rpcUrl")]
    public string? RpcUrl { get; set; }
    
    /// <summary>
    /// Override network magic
    /// </summary>
    [JsonPropertyName("networkMagic")]
    public uint? NetworkMagic { get; set; }
    
    /// <summary>
    /// Override gas limit
    /// </summary>
    [JsonPropertyName("gasLimit")]
    public long? GasLimit { get; set; }
    
    /// <summary>
    /// Override network fee
    /// </summary>
    [JsonPropertyName("networkFee")]
    public long? NetworkFee { get; set; }
    
    /// <summary>
    /// Skip verification for this contract
    /// </summary>
    [JsonPropertyName("skipVerification")]
    public bool? SkipVerification { get; set; }
    
    /// <summary>
    /// Custom valid until block offset
    /// </summary>
    [JsonPropertyName("validUntilBlockOffset")]
    public uint? ValidUntilBlockOffset { get; set; }
}