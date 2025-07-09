using Neo;
using System;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Information about a contract deployment
/// </summary>
public class ContractDeploymentInfo
{
    /// <summary>
    /// Name of the deployed contract
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the deployed contract
    /// </summary>
    public UInt160? ContractHash { get; set; }

    /// <summary>
    /// Transaction hash of the deployment
    /// </summary>
    public UInt256? TransactionHash { get; set; }

    /// <summary>
    /// Block index when the contract was deployed
    /// </summary>
    public uint BlockIndex { get; set; }

    /// <summary>
    /// Network magic number
    /// </summary>
    public uint NetworkMagic { get; set; }

    /// <summary>
    /// Timestamp when deployment was initiated
    /// </summary>
    public DateTime DeployedAt { get; set; }

    /// <summary>
    /// Gas consumed by the deployment transaction
    /// </summary>
    public long GasConsumed { get; set; }

    /// <summary>
    /// Whether the deployment was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if deployment failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether this was a dry run (simulation only)
    /// </summary>
    public bool IsDryRun { get; set; }

    /// <summary>
    /// Whether verification failed after deployment
    /// </summary>
    public bool VerificationFailed { get; set; }
}
