using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy.Steps
{
    /// <summary>
    /// Deployment step for creating checkpoints
    /// </summary>
    public class CheckpointStep : BaseDeploymentStep
    {
        private readonly INeoExpressService _neoExpress;
        private readonly NeoExpressOptions _options;
        private readonly string _checkpointName;

        public CheckpointStep(
            ILogger<CheckpointStep> logger,
            INeoExpressService neoExpress,
            IOptions<NeoExpressOptions> options,
            string checkpointName)
            : base(logger)
        {
            _neoExpress = neoExpress;
            _options = options.Value;
            _checkpointName = checkpointName;
        }

        public override string Name => $"Create Checkpoint: {_checkpointName}";
        public override int Order => 1000; // Run after deployments

        public override async Task<bool> ExecuteAsync(DeploymentContext context)
        {
            try
            {
                if (!_options.EnableCheckpoints)
                {
                    Logger.LogDebug("Checkpoints are disabled");
                    return true;
                }

                Logger.LogInformation("Creating checkpoint: {Name}", _checkpointName);
                
                var checkpoint = await _neoExpress.CreateCheckpointAsync(_checkpointName);
                
                context.Data[$"Checkpoint_{_checkpointName}"] = checkpoint;
                
                Logger.LogInformation("Checkpoint created successfully: {Name}", checkpoint);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to create checkpoint");
                return false;
            }
        }
    }
}