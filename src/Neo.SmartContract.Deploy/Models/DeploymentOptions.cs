using Neo;
using System.Collections.Generic;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Options for contract deployment
/// </summary>
public class DeploymentOptions
{
    /// <summary>
    /// Account to use for deployment
    /// </summary>
    public UInt160? DeployerAccount { get; set; }

    /// <summary>
    /// WIF key for direct signing (alternative to wallet)
    /// </summary>
    public string? WifKey { get; set; }

    /// <summary>
    /// RPC URL to connect to
    /// </summary>
    public string? RpcUrl { get; set; }

    /// <summary>
    /// Network magic number
    /// </summary>
    public uint? NetworkMagic { get; set; }

    /// <summary>
    /// Gas limit for deployment transaction
    /// </summary>
    public long GasLimit { get; set; } = 100_000_000;

    /// <summary>
    /// Whether to wait for transaction confirmation
    /// </summary>
    public bool WaitForConfirmation { get; set; } = true;

    /// <summary>
    /// Whether to verify contract deployment after transaction
    /// </summary>
    public bool VerifyAfterDeploy { get; set; } = false;

    /// <summary>
    /// Delay in milliseconds before verification
    /// </summary>
    public int VerificationDelayMs { get; set; } = 5000;

    /// <summary>
    /// Whether this is a dry run (simulation only)
    /// </summary>
    public bool DryRun { get; set; } = false;

    /// <summary>
    /// Initial parameters for contract deployment
    /// </summary>
    public List<object>? InitialParameters { get; set; }

    /// <summary>
    /// Default network fee in GAS fractions
    /// </summary>
    public long DefaultNetworkFee { get; set; } = 1_000_000;

    /// <summary>
    /// Number of blocks before transaction expires
    /// </summary>
    public uint ValidUntilBlockOffset { get; set; } = 100;

    /// <summary>
    /// Number of retries when waiting for confirmation
    /// </summary>
    public int ConfirmationRetries { get; set; } = 30;

    /// <summary>
    /// Delay between confirmation checks in seconds
    /// </summary>
    public int ConfirmationDelaySeconds { get; set; } = 5;
}
