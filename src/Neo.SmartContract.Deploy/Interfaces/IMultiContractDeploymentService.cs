using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for multi-contract deployment service
/// </summary>
public interface IMultiContractDeploymentService
{
    /// <summary>
    /// Deploy multiple contracts from a deployment manifest
    /// </summary>
    /// <param name="manifest">Deployment manifest</param>
    /// <param name="options">Base deployment options</param>
    /// <returns>Multi-contract deployment result</returns>
    Task<MultiContractDeploymentResult> DeployMultipleAsync(DeploymentManifest manifest, DeploymentOptions options);
    
    /// <summary>
    /// Deploy multiple contracts from a manifest file
    /// </summary>
    /// <param name="manifestPath">Path to deployment manifest JSON file</param>
    /// <param name="options">Base deployment options</param>
    /// <returns>Multi-contract deployment result</returns>
    Task<MultiContractDeploymentResult> DeployFromManifestAsync(string manifestPath, DeploymentOptions options);
    
    /// <summary>
    /// Resolve deployment order based on dependencies
    /// </summary>
    /// <param name="contracts">List of contract definitions</param>
    /// <returns>Ordered list of contracts to deploy</returns>
    List<ContractDefinition> ResolveDependencyOrder(IList<ContractDefinition> contracts);
    
    /// <summary>
    /// Setup contract interactions after deployment
    /// </summary>
    /// <param name="deploymentResult">Deployment result</param>
    /// <param name="interactions">List of contract interactions to setup</param>
    /// <param name="options">Invocation options</param>
    /// <returns>Setup results</returns>
    Task<ContractInteractionSetupResult> SetupContractInteractionsAsync(
        MultiContractDeploymentResult deploymentResult, 
        IList<ContractInteraction> interactions,
        InvocationOptions options);
    
    /// <summary>
    /// Rollback deployed contracts in case of failure
    /// </summary>
    /// <param name="deployedContracts">Successfully deployed contracts</param>
    /// <param name="options">Deployment options</param>
    /// <returns>Rollback result</returns>
    Task<RollbackResult> RollbackDeploymentAsync(
        IList<ContractDeploymentInfo> deployedContracts, 
        DeploymentOptions options);
}