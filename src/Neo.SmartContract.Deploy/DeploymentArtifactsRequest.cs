using System;

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

    public DeploymentArtifactsRequest WithInitParams(params object?[] parameters)
        => this with { InitParams = parameters ?? Array.Empty<object?>() };

    public DeploymentArtifactsRequest WithConfirmationPolicy(bool? waitForConfirmation, int? retries = null, int? delaySeconds = null)
        => this with
        {
            WaitForConfirmation = waitForConfirmation,
            ConfirmationRetries = retries,
            ConfirmationDelaySeconds = delaySeconds
        };
}

