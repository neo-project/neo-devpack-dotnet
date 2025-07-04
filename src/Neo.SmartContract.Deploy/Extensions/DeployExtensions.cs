using System;
using System.Threading.Tasks;
using Neo;

namespace Neo.SmartContract.Deploy.Extensions;

/// <summary>
/// Extension methods for simplified deployment
/// </summary>
public static class DeployExtensions
{
    /// <summary>
    /// Create a deployment toolkit for MainNet
    /// </summary>
    public static DeploymentToolkit MainNet(string? configPath = null)
    {
        return new DeploymentToolkit(configPath).SetNetwork("mainnet");
    }

    /// <summary>
    /// Create a deployment toolkit for TestNet
    /// </summary>
    public static DeploymentToolkit TestNet(string? configPath = null)
    {
        return new DeploymentToolkit(configPath).SetNetwork("testnet");
    }

    /// <summary>
    /// Create a deployment toolkit for local/private network
    /// </summary>
    public static DeploymentToolkit Local(string? configPath = null)
    {
        return new DeploymentToolkit(configPath).SetNetwork("local");
    }

    /// <summary>
    /// Deploy and wait for confirmation with a simple one-liner
    /// </summary>
    public static async Task<UInt160?> QuickDeploy(this DeploymentToolkit toolkit, string path, params object[] initParams)
    {
        var result = await toolkit.Deploy(path, initParams);
        return result.Success ? result.ContractHash : null;
    }

    /// <summary>
    /// Deploy and return the transaction hash without waiting
    /// </summary>
    public static async Task<UInt256?> QuickDeployNoWait(this DeploymentToolkit toolkit, string path, params object[] initParams)
    {
        // Temporarily disable wait for confirmation
        Environment.SetEnvironmentVariable("Deployment__WaitForConfirmation", "false");

        var result = await toolkit.Deploy(path, initParams);

        // Re-enable
        Environment.SetEnvironmentVariable("Deployment__WaitForConfirmation", null);

        return result.Success ? result.TransactionHash : null;
    }
}

/// <summary>
/// Static helper class for one-line deployments
/// </summary>
public static class Deploy
{
    /// <summary>
    /// Deploy to MainNet
    /// </summary>
    public static DeploymentToolkit ToMainNet => DeployExtensions.MainNet();

    /// <summary>
    /// Deploy to TestNet
    /// </summary>
    public static DeploymentToolkit ToTestNet => DeployExtensions.TestNet();

    /// <summary>
    /// Deploy to local network
    /// </summary>
    public static DeploymentToolkit ToLocal => DeployExtensions.Local();
}
