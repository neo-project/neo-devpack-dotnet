using Neo.Network.P2P.Payloads;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Options for contract update operations
/// </summary>
public class UpdateOptions
{
    /// <summary>
    /// WIF (Wallet Import Format) private key for signing transactions
    /// </summary>
    public string WifKey { get; set; } = string.Empty;

    /// <summary>
    /// RPC URL to connect to
    /// </summary>
    public string RpcUrl { get; set; } = "http://localhost:10332";

    /// <summary>
    /// Network magic number
    /// </summary>
    public uint NetworkMagic { get; set; } = 894710606; // TestNet by default

    /// <summary>
    /// Wait for transaction confirmation
    /// </summary>
    public bool WaitForConfirmation { get; set; } = true;

    /// <summary>
    /// Verify contract after update
    /// </summary>
    public bool VerifyAfterUpdate { get; set; } = true;

    /// <summary>
    /// Perform a dry run (simulation only)
    /// </summary>
    public bool DryRun { get; set; }

    /// <summary>
    /// Additional signers for the transaction
    /// </summary>
    public Signer[]? AdditionalSigners { get; set; }

    /// <summary>
    /// Maximum gas to consume
    /// </summary>
    public long GasLimit { get; set; } = 100_000_000; // 1 GAS

    /// <summary>
    /// Network fee multiplier
    /// </summary>
    public decimal NetworkFeeMultiplier { get; set; } = 1.5m;

    /// <summary>
    /// Maximum network fee to pay
    /// </summary>
    public long MaxNetworkFee { get; set; } = 10_000_000; // 0.1 GAS

    /// <summary>
    /// Timeout for waiting for confirmation (in seconds)
    /// </summary>
    public int ConfirmationTimeout { get; set; } = 60;

    /// <summary>
    /// Whether to update only the NEF (code) without updating the manifest
    /// </summary>
    public bool UpdateNefOnly { get; set; }

    /// <summary>
    /// Whether to update only the manifest without updating the NEF (code)
    /// </summary>
    public bool UpdateManifestOnly { get; set; }
}