using Neo;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Options for contract invocation
/// </summary>
public class InvocationOptions
{
    /// <summary>
    /// Account to use for invocation
    /// </summary>
    public UInt160? InvokerAccount { get; set; }

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
    /// Gas limit for transaction
    /// </summary>
    public long GasLimit { get; set; } = 10_000_000;

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
    /// Network fee in GAS fractions
    /// </summary>
    public long NetworkFee { get; set; } = 1_000_000;
}