using Neo;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Options for contract deployment with enhanced security and configuration
/// </summary>
public class DeploymentOptions
{
    /// <summary>
    /// Account to use for deployment
    /// </summary>
    public UInt160? DeployerAccount { get; set; }

    /// <summary>
    /// WIF key for direct signing (alternative to wallet)
    /// </summary>
    public string? WifKey { get; set; }

    /// <summary>
    /// Credential key for retrieving WIF from credential provider
    /// </summary>
    public string? CredentialKey { get; set; }

    /// <summary>
    /// RPC URL to connect to
    /// </summary>
    public string? RpcUrl { get; set; }

    /// <summary>
    /// Network magic number
    /// </summary>
    public uint? NetworkMagic { get; set; }

    /// <summary>
    /// Gas limit for deployment transaction
    /// </summary>
    public long GasLimit { get; set; } = 100_000_000;

    /// <summary>
    /// Whether to wait for transaction confirmation
    /// </summary>
    public bool WaitForConfirmation { get; set; } = true;

    /// <summary>
    /// Whether to verify contract deployment after transaction
    /// </summary>
    public bool VerifyAfterDeploy { get; set; } = false;

    /// <summary>
    /// Delay in milliseconds before verification
    /// </summary>
    public int VerificationDelayMs { get; set; } = 5000;

    /// <summary>
    /// Whether this is a dry run (simulation only)
    /// </summary>
    public bool DryRun { get; set; } = false;

    /// <summary>
    /// Initial parameters for contract deployment
    /// </summary>
    public List<object>? InitialParameters { get; set; }

    /// <summary>
    /// Default network fee in GAS fractions
    /// </summary>
    public long DefaultNetworkFee { get; set; } = 1_000_000;

    /// <summary>
    /// Number of blocks before transaction expires
    /// </summary>
    public uint ValidUntilBlockOffset { get; set; } = 100;

    /// <summary>
    /// Number of retries when waiting for confirmation
    /// </summary>
    public int ConfirmationRetries { get; set; } = 30;

    /// <summary>
    /// Delay between confirmation checks in seconds
    /// </summary>
    public int ConfirmationDelaySeconds { get; set; } = 5;

    // Security Options

    /// <summary>
    /// Whether to require secure credential storage (no plain text WIF)
    /// </summary>
    public bool RequireSecureCredentials { get; set; } = false;

    /// <summary>
    /// Whether to enable deployment signing with multiple signatures
    /// </summary>
    public bool EnableMultiSig { get; set; } = false;

    /// <summary>
    /// Additional signers required for multi-sig deployments
    /// </summary>
    public List<SignerConfiguration>? AdditionalSigners { get; set; }

    /// <summary>
    /// Whether to validate RPC endpoint certificate (for HTTPS)
    /// </summary>
    public bool ValidateRpcCertificate { get; set; } = true;

    /// <summary>
    /// Custom RPC certificate validation callback
    /// </summary>
    public Func<object, System.Security.Cryptography.X509Certificates.X509Certificate?, System.Security.Cryptography.X509Certificates.X509Chain?, System.Net.Security.SslPolicyErrors, bool>? RpcCertificateValidationCallback { get; set; }

    // Performance Options

    /// <summary>
    /// Whether to enable performance monitoring
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;

    /// <summary>
    /// Whether to enable deployment metrics collection
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Whether to record deployment history
    /// </summary>
    public bool RecordDeploymentHistory { get; set; } = true;

    /// <summary>
    /// Custom deployment record path
    /// </summary>
    public string? DeploymentRecordPath { get; set; }

    // Advanced Options

    /// <summary>
    /// Whether to enable automatic retry on transient failures
    /// </summary>
    public bool EnableAutoRetry { get; set; } = true;

    /// <summary>
    /// Maximum number of automatic retries
    /// </summary>
    public int MaxAutoRetries { get; set; } = 3;

    /// <summary>
    /// Delay between automatic retries in milliseconds
    /// </summary>
    public int AutoRetryDelayMs { get; set; } = 5000;

    /// <summary>
    /// Custom headers for RPC requests
    /// </summary>
    public Dictionary<string, string>? CustomRpcHeaders { get; set; }

    /// <summary>
    /// Timeout for RPC requests in seconds
    /// </summary>
    public int RpcTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to enable detailed logging
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Custom deployment tags for tracking
    /// </summary>
    public Dictionary<string, string>? DeploymentTags { get; set; }

    /// <summary>
    /// Deployment environment (e.g., development, staging, production)
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    /// Whether to perform pre-deployment health checks
    /// </summary>
    public bool PerformHealthChecks { get; set; } = true;

    /// <summary>
    /// Health check timeout in seconds
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to abort deployment on health check failure
    /// </summary>
    public bool AbortOnHealthCheckFailure { get; set; } = false;

    /// <summary>
    /// Custom deployment hooks
    /// </summary>
    public DeploymentHooks? Hooks { get; set; }

    /// <summary>
    /// Validate options
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when options are invalid</exception>
    public void Validate()
    {
        if (RequireSecureCredentials && !string.IsNullOrEmpty(WifKey))
        {
            throw new InvalidOperationException("Plain text WIF keys are not allowed when RequireSecureCredentials is enabled. Use CredentialKey instead.");
        }

        if (string.IsNullOrEmpty(RpcUrl))
        {
            throw new InvalidOperationException("RPC URL is required.");
        }

        if (GasLimit <= 0)
        {
            throw new InvalidOperationException("Gas limit must be greater than zero.");
        }

        if (ConfirmationRetries < 0)
        {
            throw new InvalidOperationException("Confirmation retries cannot be negative.");
        }

        if (ConfirmationDelaySeconds <= 0)
        {
            throw new InvalidOperationException("Confirmation delay must be greater than zero.");
        }

        if (EnableMultiSig && (AdditionalSigners == null || AdditionalSigners.Count == 0))
        {
            throw new InvalidOperationException("Additional signers are required when multi-sig is enabled.");
        }
    }
}

/// <summary>
/// Configuration for additional signers in multi-sig deployments
/// </summary>
public class SignerConfiguration
{
    /// <summary>
    /// Signer account
    /// </summary>
    public UInt160? Account { get; set; }

    /// <summary>
    /// WIF key for the signer
    /// </summary>
    public string? WifKey { get; set; }

    /// <summary>
    /// Credential key for retrieving WIF from credential provider
    /// </summary>
    public string? CredentialKey { get; set; }

    /// <summary>
    /// Scopes for the signer
    /// </summary>
    public Neo.Network.P2P.Payloads.WitnessScope Scopes { get; set; } = Neo.Network.P2P.Payloads.WitnessScope.CalledByEntry;
}

/// <summary>
/// Deployment hooks for custom logic
/// </summary>
public class DeploymentHooks
{
    /// <summary>
    /// Hook to execute before deployment
    /// </summary>
    public Func<DeploymentOptions, Task>? PreDeployment { get; set; }

    /// <summary>
    /// Hook to execute after successful deployment
    /// </summary>
    public Func<ContractDeploymentInfo, Task>? PostDeployment { get; set; }

    /// <summary>
    /// Hook to execute on deployment failure
    /// </summary>
    public Func<Exception, Task>? OnFailure { get; set; }

    /// <summary>
    /// Hook to execute before transaction submission
    /// </summary>
    public Func<Neo.Network.P2P.Payloads.Transaction, Task<bool>>? PreTransaction { get; set; }
}