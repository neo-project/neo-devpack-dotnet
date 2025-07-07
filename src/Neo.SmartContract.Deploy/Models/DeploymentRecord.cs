using System;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Deployment record for tracking contract deployments
/// </summary>
public class DeploymentRecord
{
    /// <summary>
    /// Contract name
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Contract hash
    /// </summary>
    public string? ContractHash { get; set; }

    /// <summary>
    /// Deployment transaction hash
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Deployment timestamp
    /// </summary>
    public DateTime DeployedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Contract version
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Update history entries
    /// </summary>
    public UpdateHistoryEntry[]? UpdateHistory { get; set; }
}

/// <summary>
/// Update history entry for tracking contract updates
/// </summary>
public class UpdateHistoryEntry
{
    /// <summary>
    /// Update transaction hash
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Previous version before update
    /// </summary>
    public string? PreviousVersion { get; set; }

    /// <summary>
    /// New version after update
    /// </summary>
    public string? NewVersion { get; set; }
}
