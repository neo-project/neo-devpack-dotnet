namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Network configuration settings
/// </summary>
public class NetworkConfiguration
{
    /// <summary>
    /// Network RPC URL
    /// </summary>
    public string RpcUrl { get; set; } = string.Empty;

    /// <summary>
    /// Network magic number for transaction signing
    /// Retrieved from RPC if not explicitly set
    /// </summary>
    public uint? NetworkMagic { get; set; }

    /// <summary>
    /// Wallet configuration
    /// </summary>
    public WalletConfiguration Wallet { get; set; } = new();
}

/// <summary>
/// Wallet configuration settings
/// </summary>
public class WalletConfiguration
{
    /// <summary>
    /// Path to wallet file
    /// </summary>
    public string WalletPath { get; set; } = string.Empty;

    /// <summary>
    /// Wallet password (use environment variables for production)
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Deployment configuration settings
/// </summary>
public class DeploymentConfiguration
{
    /// <summary>
    /// Whether to wait for transaction confirmation
    /// </summary>
    public bool WaitForConfirmation { get; set; } = true;

    /// <summary>
    /// Number of retries when waiting for confirmation
    /// </summary>
    public int ConfirmationRetries { get; set; } = 30;

    /// <summary>
    /// Delay between confirmation checks in seconds
    /// </summary>
    public int ConfirmationDelaySeconds { get; set; } = 5;

    /// <summary>
    /// Number of blocks before transaction expires
    /// </summary>
    public uint ValidUntilBlockOffset { get; set; } = 100;

    /// <summary>
    /// Default network fee in GAS fractions
    /// </summary>
    public long DefaultNetworkFee { get; set; } = 1000000;

    /// <summary>
    /// Default gas limit for transactions
    /// </summary>
    public long DefaultGasLimit { get; set; } = 50000000;
}
