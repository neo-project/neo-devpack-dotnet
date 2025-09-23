namespace Neo.SmartContract.Deploy;

/// <summary>
/// Configuration applied to deployment operations.
/// </summary>
public class DeploymentOptions
{
    public NetworkProfile? Network { get; set; }
        = null;

    public bool WaitForConfirmation { get; set; }
        = false;

    public int ConfirmationRetries { get; set; }
        = 30;

    public int ConfirmationDelaySeconds { get; set; }
        = 5;

    public DeploymentOptions Clone()
        => new()
        {
            Network = Network,
            WaitForConfirmation = WaitForConfirmation,
            ConfirmationRetries = ConfirmationRetries,
            ConfirmationDelaySeconds = ConfirmationDelaySeconds
        };
}
