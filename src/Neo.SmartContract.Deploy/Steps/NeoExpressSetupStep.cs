using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Deploy.Services;

namespace Neo.SmartContract.Deploy.Steps
{
    /// <summary>
    /// Deployment step for setting up Neo Express environment
    /// </summary>
    public class NeoExpressSetupStep : BaseDeploymentStep
    {
        private readonly INeoExpressService _neoExpress;
        private readonly NeoExpressOptions _options;

        public NeoExpressSetupStep(
            ILogger<NeoExpressSetupStep> logger,
            INeoExpressService neoExpress,
            IOptions<NeoExpressOptions> options)
            : base(logger)
        {
            _neoExpress = neoExpress;
            _options = options.Value;
        }

        public override string Name => "Setup Neo Express";
        public override int Order => -100; // Run before everything else

        public override async Task<bool> ExecuteAsync(DeploymentContext context)
        {
            try
            {
                // Check if Neo Express is installed
                if (!await _neoExpress.IsInstalledAsync())
                {
                    Logger.LogError("Neo Express is not installed. Please install it using: dotnet tool install -g Neo.Express");
                    return false;
                }

                // Create instance if needed
                if (_options.AutoCreate && !File.Exists(_options.ConfigFile))
                {
                    Logger.LogInformation("Neo Express configuration not found. Creating new instance...");
                    if (!await _neoExpress.CreateInstanceAsync())
                    {
                        return false;
                    }
                }

                // Start Neo Express if needed
                if (_options.AutoStart && !await _neoExpress.IsRunningAsync())
                {
                    Logger.LogInformation("Starting Neo Express...");
                    if (!await _neoExpress.StartAsync())
                    {
                        return false;
                    }

                    // Wait a bit for Neo Express to fully initialize
                    await Task.Delay(_options.StartupWaitMs);
                }

                // Create deployment wallet if it doesn't exist
                var deployAccount = await _neoExpress.GetAccountAsync("deploy");
                if (deployAccount == null)
                {
                    Logger.LogInformation("Creating deployment wallet...");
                    var address = await _neoExpress.CreateWalletAsync("deploy");
                    if (string.IsNullOrEmpty(address))
                    {
                        return false;
                    }

                    // Transfer some GAS for deployment
                    Logger.LogInformation("Transferring GAS to deployment wallet...");
                    if (!await _neoExpress.TransferGasAsync(address, _options.InitialGasAmount))
                    {
                        Logger.LogWarning("Failed to transfer GAS. You may need to do this manually.");
                    }

                    deployAccount = new NeoExpressAccount { Name = "deploy", Address = address };
                }

                // Store Neo Express account info in context
                context.Data["NeoExpressAccount"] = deployAccount;
                context.Data["NeoExpressRunning"] = true;
                context.Data["NeoExpressService"] = _neoExpress;

                Logger.LogInformation("Neo Express setup completed. Using account: {Address}", deployAccount.Address);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to setup Neo Express");
                return false;
            }
        }
    }
}