using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy.Steps
{
    /// <summary>
    /// Deployment step for updating existing contracts
    /// </summary>
    public class UpdateContractStep : BaseDeploymentStep
    {
        private readonly IContractUpdateService _updateService;
        private readonly IContractLoader _contractLoader;
        private readonly NetworkOptions _networkOptions;
        private readonly string _contractName;
        private readonly string? _version;

        public UpdateContractStep(
            ILogger<UpdateContractStep> logger,
            IContractUpdateService updateService,
            IContractLoader contractLoader,
            IOptions<NetworkOptions> networkOptions,
            string contractName,
            string? version = null)
            : base(logger)
        {
            _updateService = updateService;
            _contractLoader = contractLoader;
            _networkOptions = networkOptions.Value;
            _contractName = contractName;
            _version = version;
        }

        public override string Name => $"Update {_contractName}";
        public override int Order => 20; // After compilation

        public override async Task<bool> ExecuteAsync(DeploymentContext context)
        {
            try
            {
                // Check if contract can be updated
                var checkResult = await _updateService.CheckUpdateAsync(_contractName, _networkOptions.Network);
                
                if (!checkResult.CanUpdate)
                {
                    if (!checkResult.IsDeployed)
                    {
                        Logger.LogWarning("Contract {Name} is not deployed on {Network}, skipping update", 
                            _contractName, _networkOptions.Network);
                    }
                    else
                    {
                        Logger.LogError("Cannot update {Name}: {Reason}", 
                            _contractName, checkResult.Reason);
                    }
                    return false;
                }

                Logger.LogInformation("Contract {Name} can be updated. Current version: {Version}", 
                    _contractName, checkResult.CurrentVersion);

                // Load contract files
                var contract = await _contractLoader.LoadContractAsync(_contractName);

                // Update the contract
                var result = await _updateService.UpdateContractAsync(
                    _contractName,
                    _networkOptions.Network,
                    contract.NefBytes,
                    contract.Manifest,
                    null,
                    _version
                );

                if (result.Success)
                {
                    Logger.LogInformation("Contract {Name} updated successfully from v{Old} to v{New}", 
                        _contractName, result.PreviousVersion, result.NewVersion);
                    
                    // Store in context for other steps
                    context.DeployedContracts[_contractName] = result.ContractHash!;
                    context.Data[$"{_contractName}_Updated"] = true;
                    context.Data[$"{_contractName}_Version"] = result.NewVersion!;
                    
                    return true;
                }
                else
                {
                    Logger.LogError("Failed to update {Name}: {Error}", 
                        _contractName, result.ErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating contract {Name}", _contractName);
                return false;
            }
        }
    }
}