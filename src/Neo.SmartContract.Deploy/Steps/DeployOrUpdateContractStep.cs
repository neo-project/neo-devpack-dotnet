using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy.Steps
{
    /// <summary>
    /// Deployment step that automatically deploys or updates a contract based on its current state
    /// </summary>
    public class DeployOrUpdateContractStep : BaseDeploymentStep
    {
        private readonly IDeploymentService _deploymentService;
        private readonly IContractUpdateService _updateService;
        private readonly IDeploymentRecordService _recordService;
        private readonly IContractLoader _contractLoader;
        private readonly NetworkOptions _networkOptions;
        private readonly string _contractName;
        private readonly string? _version;

        public DeployOrUpdateContractStep(
            ILogger<DeployOrUpdateContractStep> logger,
            IDeploymentService deploymentService,
            IContractUpdateService updateService,
            IDeploymentRecordService recordService,
            IContractLoader contractLoader,
            IOptions<NetworkOptions> networkOptions,
            string contractName,
            string? version = null)
            : base(logger)
        {
            _deploymentService = deploymentService;
            _updateService = updateService;
            _recordService = recordService;
            _contractLoader = contractLoader;
            _networkOptions = networkOptions.Value;
            _contractName = contractName;
            _version = version;
        }

        public override string Name => $"Deploy/Update {_contractName}";
        public override int Order => 20;

        public override async Task<bool> ExecuteAsync(DeploymentContext context)
        {
            try
            {
                // Check if contract is already deployed
                var isDeployed = await _recordService.IsDeployedAsync(_contractName, _networkOptions.Network);
                
                if (isDeployed)
                {
                    Logger.LogInformation("Contract {Name} is already deployed on {Network}, checking for updates...", 
                        _contractName, _networkOptions.Network);
                    
                    // Check if update is possible
                    var checkResult = await _updateService.CheckUpdateAsync(_contractName, _networkOptions.Network);
                    
                    if (checkResult.CanUpdate)
                    {
                        // Load contract and update
                        var contract = await _contractLoader.LoadContractAsync(_contractName);
                        var updateResult = await _updateService.UpdateContractAsync(
                            _contractName,
                            _networkOptions.Network,
                            contract.NefBytes,
                            contract.Manifest,
                            null,
                            _version
                        );

                        if (updateResult.Success)
                        {
                            Logger.LogInformation("Contract {Name} updated from v{Old} to v{New}", 
                                _contractName, updateResult.PreviousVersion, updateResult.NewVersion);
                            
                            context.DeployedContracts[_contractName] = updateResult.ContractHash!;
                            context.Data[$"{_contractName}_Action"] = "Updated";
                            context.Data[$"{_contractName}_Version"] = updateResult.NewVersion!;
                            return true;
                        }
                        else
                        {
                            Logger.LogError("Failed to update {Name}: {Error}", 
                                _contractName, updateResult.ErrorMessage);
                            return false;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Contract {Name} cannot be updated: {Reason}", 
                            _contractName, checkResult.Reason);
                        
                        // Still add to context if it exists
                        if (checkResult.ContractHash != null)
                        {
                            context.DeployedContracts[_contractName] = checkResult.ContractHash;
                            context.Data[$"{_contractName}_Action"] = "Existing";
                            context.Data[$"{_contractName}_Version"] = checkResult.CurrentVersion ?? "Unknown";
                        }
                        
                        return true; // Don't fail if contract exists but can't be updated
                    }
                }
                else
                {
                    Logger.LogInformation("Contract {Name} not found on {Network}, deploying new instance...", 
                        _contractName, _networkOptions.Network);
                    
                    // Deploy new contract
                    var contract = await _contractLoader.LoadContractAsync(_contractName);
                    var deployResult = await _deploymentService.DeployContractAsync(
                        _contractName,
                        contract.NefBytes,
                        contract.Manifest
                    );

                    if (deployResult.Success)
                    {
                        Logger.LogInformation("Contract {Name} deployed successfully at {Hash}", 
                            _contractName, deployResult.Hash);
                        
                        context.DeployedContracts[_contractName] = deployResult.Hash!;
                        context.Data[$"{_contractName}_Action"] = "Deployed";
                        context.Data[$"{_contractName}_Version"] = "1.0.0";
                        return true;
                    }
                    else
                    {
                        Logger.LogError("Failed to deploy {Name}: {Error}", 
                            _contractName, deployResult.ErrorMessage);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in deploy/update step for {Name}", _contractName);
                return false;
            }
        }
    }
}