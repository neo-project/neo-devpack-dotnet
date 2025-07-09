using System;
using System.Collections.Generic;
using Neo;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Result of a multi-contract deployment operation
/// </summary>
public class MultiContractDeploymentResult
{
    /// <summary>
    /// Deployment ID for tracking
    /// </summary>
    public string DeploymentId { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Timestamp of deployment start
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Timestamp of deployment completion
    /// </summary>
    public DateTime? EndTime { get; set; }
    
    /// <summary>
    /// Overall deployment status
    /// </summary>
    public DeploymentStatus Status { get; set; } = DeploymentStatus.InProgress;
    
    /// <summary>
    /// Successfully deployed contracts
    /// </summary>
    public Dictionary<string, ContractDeploymentInfo> DeployedContracts { get; set; } = new();
    
    /// <summary>
    /// Failed deployments
    /// </summary>
    public Dictionary<string, DeploymentFailure> FailedDeployments { get; set; } = new();
    
    /// <summary>
    /// Contract interaction setup results
    /// </summary>
    public List<InteractionSetupResult> InteractionResults { get; set; } = new();
    
    /// <summary>
    /// Total GAS spent
    /// </summary>
    public decimal TotalGasSpent { get; set; }
    
    /// <summary>
    /// Total network fees
    /// </summary>
    public decimal TotalNetworkFees { get; set; }
    
    /// <summary>
    /// Deployment summary
    /// </summary>
    public DeploymentSummary Summary { get; set; } = new();
    
    /// <summary>
    /// Rollback information if performed
    /// </summary>
    public RollbackResult? RollbackResult { get; set; }
}

/// <summary>
/// Deployment status enum
/// </summary>
public enum DeploymentStatus
{
    /// <summary>
    /// Deployment not started
    /// </summary>
    NotStarted,
    
    /// <summary>
    /// Deployment in progress
    /// </summary>
    InProgress,
    
    /// <summary>
    /// All contracts deployed successfully
    /// </summary>
    Completed,
    
    /// <summary>
    /// Some contracts deployed, some failed
    /// </summary>
    PartiallyCompleted,
    
    /// <summary>
    /// All deployments failed
    /// </summary>
    Failed,
    
    /// <summary>
    /// Deployment rolled back
    /// </summary>
    RolledBack
}

/// <summary>
/// Information about a deployment failure
/// </summary>
public class DeploymentFailure
{
    /// <summary>
    /// Contract ID that failed
    /// </summary>
    public string ContractId { get; set; } = string.Empty;
    
    /// <summary>
    /// Contract name
    /// </summary>
    public string ContractName { get; set; } = string.Empty;
    
    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Full exception details
    /// </summary>
    public Exception? Exception { get; set; }
    
    /// <summary>
    /// Failure timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Stage where failure occurred
    /// </summary>
    public DeploymentStage FailureStage { get; set; }
}

/// <summary>
/// Deployment stage enum
/// </summary>
public enum DeploymentStage
{
    /// <summary>
    /// Compilation stage
    /// </summary>
    Compilation,
    
    /// <summary>
    /// Deployment transaction
    /// </summary>
    Deployment,
    
    /// <summary>
    /// Verification stage
    /// </summary>
    Verification,
    
    /// <summary>
    /// Interaction setup
    /// </summary>
    InteractionSetup
}

/// <summary>
/// Result of setting up a contract interaction
/// </summary>
public class InteractionSetupResult
{
    /// <summary>
    /// Interaction description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Source contract ID
    /// </summary>
    public string SourceContractId { get; set; } = string.Empty;
    
    /// <summary>
    /// Target contract ID
    /// </summary>
    public string TargetContractId { get; set; } = string.Empty;
    
    /// <summary>
    /// Method called
    /// </summary>
    public string Method { get; set; } = string.Empty;
    
    /// <summary>
    /// Transaction hash
    /// </summary>
    public UInt256? TransactionHash { get; set; }
    
    /// <summary>
    /// Success status
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// GAS consumed
    /// </summary>
    public decimal GasConsumed { get; set; }
}

/// <summary>
/// Deployment summary information
/// </summary>
public class DeploymentSummary
{
    /// <summary>
    /// Total contracts in manifest
    /// </summary>
    public int TotalContracts { get; set; }
    
    /// <summary>
    /// Successfully deployed contracts
    /// </summary>
    public int SuccessfulDeployments { get; set; }
    
    /// <summary>
    /// Failed deployments
    /// </summary>
    public int FailedDeployments { get; set; }
    
    /// <summary>
    /// Total interactions configured
    /// </summary>
    public int TotalInteractions { get; set; }
    
    /// <summary>
    /// Successful interactions
    /// </summary>
    public int SuccessfulInteractions { get; set; }
    
    /// <summary>
    /// Total deployment duration
    /// </summary>
    public TimeSpan? Duration { get; set; }
    
    /// <summary>
    /// Deployment messages/warnings
    /// </summary>
    public List<string> Messages { get; set; } = new();
}

/// <summary>
/// Result of contract interaction setup
/// </summary>
public class ContractInteractionSetupResult
{
    /// <summary>
    /// List of interaction results
    /// </summary>
    public List<InteractionSetupResult> Results { get; set; } = new();
    
    /// <summary>
    /// Total interactions attempted
    /// </summary>
    public int TotalAttempted { get; set; }
    
    /// <summary>
    /// Successful interactions
    /// </summary>
    public int Successful { get; set; }
    
    /// <summary>
    /// Failed interactions
    /// </summary>
    public int Failed { get; set; }
    
    /// <summary>
    /// Total GAS consumed
    /// </summary>
    public decimal TotalGasConsumed { get; set; }
}

/// <summary>
/// Result of deployment rollback
/// </summary>
public class RollbackResult
{
    /// <summary>
    /// Rollback status
    /// </summary>
    public RollbackStatus Status { get; set; }
    
    /// <summary>
    /// Contracts successfully rolled back
    /// </summary>
    public List<string> RolledBackContracts { get; set; } = new();
    
    /// <summary>
    /// Contracts that failed to rollback
    /// </summary>
    public Dictionary<string, string> FailedRollbacks { get; set; } = new();
    
    /// <summary>
    /// Rollback timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Rollback messages
    /// </summary>
    public List<string> Messages { get; set; } = new();
}

/// <summary>
/// Rollback status enum
/// </summary>
public enum RollbackStatus
{
    /// <summary>
    /// Not attempted
    /// </summary>
    NotAttempted,
    
    /// <summary>
    /// All contracts rolled back successfully
    /// </summary>
    Success,
    
    /// <summary>
    /// Some contracts rolled back
    /// </summary>
    Partial,
    
    /// <summary>
    /// Rollback failed
    /// </summary>
    Failed
}