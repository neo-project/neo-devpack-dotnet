using Neo;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Contract deployment information
/// </summary>
public class ContractDeploymentInfo
{
    /// <summary>
    /// Contract name
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Deployed contract hash
    /// </summary>
    public UInt160 ContractHash { get; set; } = UInt160.Zero;

    /// <summary>
    /// Deployment transaction hash
    /// </summary>
    public UInt256 TransactionHash { get; set; } = UInt256.Zero;

    /// <summary>
    /// Block index where contract was deployed
    /// </summary>
    public uint BlockIndex { get; set; }

    /// <summary>
    /// Network magic number
    /// </summary>
    public uint NetworkMagic { get; set; }

    /// <summary>
    /// Deployment timestamp
    /// </summary>
    public DateTime DeployedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gas consumed for deployment
    /// </summary>
    public long GasConsumed { get; set; }

    /// <summary>
    /// Whether the deployment was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Any error message if deployment failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
