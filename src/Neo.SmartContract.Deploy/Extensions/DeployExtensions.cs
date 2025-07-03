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
    public static SimpleToolkit MainNet(string? configPath = null)
    {
        return new SimpleToolkit(configPath).SetNetwork("mainnet");
    }

    /// <summary>
    /// Create a deployment toolkit for TestNet
    /// </summary>
    public static SimpleToolkit TestNet(string? configPath = null)
    {
        return new SimpleToolkit(configPath).SetNetwork("testnet");
    }

    /// <summary>
    /// Create a deployment toolkit for local/private network
    /// </summary>
    public static SimpleToolkit Local(string? configPath = null)
    {
        return new SimpleToolkit(configPath).SetNetwork("local");
    }

    /// <summary>
    /// Deploy and wait for confirmation with a simple one-liner
    /// </summary>
    public static async Task<UInt160?> QuickDeploy(this SimpleToolkit toolkit, string path, params object[] initParams)
    {
        var result = await toolkit.Deploy(path, initParams);
        return result.Success ? result.ContractHash : null;
    }

    /// <summary>
    /// Deploy and return the transaction hash without waiting
    /// </summary>
    public static async Task<UInt256?> QuickDeployNoWait(this SimpleToolkit toolkit, string path, params object[] initParams)
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
    public static SimpleToolkit ToMainNet => DeployExtensions.MainNet();

    /// <summary>
    /// Deploy to TestNet
    /// </summary>
    public static SimpleToolkit ToTestNet => DeployExtensions.TestNet();

    /// <summary>
    /// Deploy to local network
    /// </summary>
    public static SimpleToolkit ToLocal => DeployExtensions.Local();
}
