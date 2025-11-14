using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Configuration applied to deployment operations.
/// </summary>
public class DeploymentOptions
{
    public NetworkProfile? Network { get; set; } = null;

    public bool WaitForConfirmation { get; set; } = false;

    public int ConfirmationRetries { get; set; } = 30;

    public int ConfirmationDelaySeconds { get; set; } = 5;

    public Func<ProtocolSettings, IReadOnlyList<Signer>>? SignerProvider { get; set; } = null;

    public Func<TransactionManager, CancellationToken, Task<Transaction>>? TransactionSignerAsync { get; set; } = null;

    public DeploymentOptions Clone() => new()
    {
        Network = Network,
        WaitForConfirmation = WaitForConfirmation,
        ConfirmationRetries = ConfirmationRetries,
        ConfirmationDelaySeconds = ConfirmationDelaySeconds,
        SignerProvider = SignerProvider,
        TransactionSignerAsync = TransactionSignerAsync
    };
}
