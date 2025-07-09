using Neo;
using System;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Information about a contract update
/// </summary>
public class ContractUpdateInfo
{
    /// <summary>
    /// Name of the updated contract
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the updated contract
    /// </summary>
    public UInt160? ContractHash { get; set; }

    /// <summary>
    /// Transaction hash of the update
    /// </summary>
    public UInt256? TransactionHash { get; set; }

    /// <summary>
    /// Block index when the contract was updated
    /// </summary>
    public uint BlockIndex { get; set; }

    /// <summary>
    /// Network magic number
    /// </summary>
    public uint NetworkMagic { get; set; }

    /// <summary>
    /// Timestamp when update was initiated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gas consumed by the update transaction
    /// </summary>
    public long GasConsumed { get; set; }

    /// <summary>
    /// Whether the update was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if update failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether this was a dry run (simulation only)
    /// </summary>
    public bool IsDryRun { get; set; }

    /// <summary>
    /// Whether verification failed after update
    /// </summary>
    public bool VerificationFailed { get; set; }

    /// <summary>
    /// Address of the account that performed the update
    /// </summary>
    public string? UpdaterAddress { get; set; }

    /// <summary>
    /// Network fee paid for the update transaction
    /// </summary>
    public long NetworkFee { get; set; }

    /// <summary>
    /// System fee paid for the update transaction
    /// </summary>
    public long SystemFee { get; set; }

    /// <summary>
    /// Timestamp when the update occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Previous contract version (if available)
    /// </summary>
    public string? PreviousVersion { get; set; }

    /// <summary>
    /// New contract version (if available)
    /// </summary>
    public string? NewVersion { get; set; }
}