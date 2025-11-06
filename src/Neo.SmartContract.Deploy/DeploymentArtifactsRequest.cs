using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy;

public sealed record DeploymentArtifactsRequest
{
    public DeploymentArtifactsRequest(
        string nefPath,
        string manifestPath,
        object?[]? initializationParameters = null)
    {
        NefPath = nefPath ?? throw new ArgumentNullException(nameof(nefPath));
        ManifestPath = manifestPath ?? throw new ArgumentNullException(nameof(manifestPath));
        InitParams = initializationParameters ?? Array.Empty<object?>();
    }

    public string NefPath { get; init; }

    public string ManifestPath { get; init; }

    public object?[] InitParams { get; init; }

    public bool? WaitForConfirmation { get; init; }

    public int? ConfirmationRetries { get; init; }

    public int? ConfirmationDelaySeconds { get; init; }

    public IReadOnlyList<Signer>? Signers { get; init; }

    public Func<TransactionManager, CancellationToken, Task<Transaction>>? TransactionSignerAsync { get; init; }

    public DeploymentArtifactsRequest WithInitParams(params object?[] parameters)
        => this with { InitParams = parameters ?? Array.Empty<object?>() };

    public DeploymentArtifactsRequest WithConfirmationPolicy(bool? waitForConfirmation, int? retries = null, int? delaySeconds = null)
        => this with
        {
            WaitForConfirmation = waitForConfirmation,
            ConfirmationRetries = retries,
            ConfirmationDelaySeconds = delaySeconds
        };

    public DeploymentArtifactsRequest WithSigners(IReadOnlyList<Signer>? signers)
        => this with { Signers = signers };

    public DeploymentArtifactsRequest WithTransactionSigner(Func<TransactionManager, CancellationToken, Task<Transaction>>? signerAsync)
        => this with { TransactionSignerAsync = signerAsync };
}
