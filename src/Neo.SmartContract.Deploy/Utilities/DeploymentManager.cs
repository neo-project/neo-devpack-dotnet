using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Services;
using System.Text;

namespace Neo.SmartContract.Deploy.Utilities
{
    /// <summary>
    /// Utility for managing deployment records
    /// </summary>
    public class DeploymentManager
    {
        private readonly IDeploymentRecordService _recordService;
        private readonly ILogger<DeploymentManager> _logger;

        public DeploymentManager(IDeploymentRecordService recordService, ILogger<DeploymentManager> logger)
        {
            _recordService = recordService;
            _logger = logger;
        }

        /// <summary>
        /// List all deployments for a network
        /// </summary>
        public async Task ListDeploymentsAsync(string network)
        {
            var deployments = await _recordService.GetNetworkDeploymentsAsync(network);
            
            if (deployments.Count == 0)
            {
                _logger.LogInformation("No contracts deployed on {Network}", network);
                return;
            }

            _logger.LogInformation("Deployed contracts on {Network}:", network);
            _logger.LogInformation("{Line}", new string('-', 80));
            _logger.LogInformation("{Contract,-20} {Hash,-50} {Version,-10} {Date}", 
                "Contract", "Hash", "Version", "Deployed");
            _logger.LogInformation("{Line}", new string('-', 80));

            foreach (var (contractName, record) in deployments.OrderBy(d => d.Key))
            {
                _logger.LogInformation("{Contract,-20} {Hash,-50} {Version,-10} {Date:yyyy-MM-dd HH:mm}", 
                    contractName, 
                    record.ContractHash, 
                    record.Version,
                    record.DeployedAt.LocalDateTime);
            }
        }

        /// <summary>
        /// Show detailed deployment information
        /// </summary>
        public async Task ShowDeploymentAsync(string contractName, string? network = null)
        {
            Dictionary<string, DeploymentRecord> deployments;
            
            if (network != null)
            {
                var record = await _recordService.GetDeploymentAsync(contractName, network);
                if (record == null)
                {
                    _logger.LogWarning("Contract {Name} not found on {Network}", contractName, network);
                    return;
                }
                deployments = new Dictionary<string, DeploymentRecord> { { network, record } };
            }
            else
            {
                deployments = await _recordService.GetAllDeploymentsAsync(contractName);
                if (deployments.Count == 0)
                {
                    _logger.LogWarning("Contract {Name} has no deployment records", contractName);
                    return;
                }
            }

            _logger.LogInformation("Deployment information for {Contract}:", contractName);
            
            foreach (var (net, record) in deployments)
            {
                _logger.LogInformation("\n=== {Network} ===", net.ToUpperInvariant());
                _logger.LogInformation("Contract Hash:    {Hash}", record.ContractHash);
                _logger.LogInformation("Transaction:      {Tx}", record.TransactionHash);
                _logger.LogInformation("Version:          {Version}", record.Version);
                _logger.LogInformation("Deployed:         {Date:yyyy-MM-dd HH:mm:ss} UTC", record.DeployedAt);
                _logger.LogInformation("Deployer:         {Address}", record.DeployerAddress);
                _logger.LogInformation("NEF Hash:         {Hash}", record.NefHash);
                _logger.LogInformation("Manifest Hash:    {Hash}", record.ManifestHash);

                if (record.Metadata.Count > 0)
                {
                    _logger.LogInformation("Metadata:");
                    foreach (var (key, value) in record.Metadata)
                    {
                        _logger.LogInformation("  {Key}: {Value}", key, value);
                    }
                }

                if (record.UpdateHistory.Count > 0)
                {
                    _logger.LogInformation("\nUpdate History:");
                    foreach (var update in record.UpdateHistory.OrderByDescending(u => u.UpdatedAt))
                    {
                        _logger.LogInformation("  - {Date:yyyy-MM-dd HH:mm}: v{Old} -> v{New} (Tx: {Tx})",
                            update.UpdatedAt.LocalDateTime,
                            update.PreviousVersion,
                            update.NewVersion,
                            update.TransactionHash.ToString().Substring(0, 10) + "...");
                    }
                }
            }
        }

        /// <summary>
        /// Export deployment information to JSON
        /// </summary>
        public async Task<string> ExportDeploymentsAsync(string? network = null)
        {
            var result = new Dictionary<string, object>();

            if (network != null)
            {
                var deployments = await _recordService.GetNetworkDeploymentsAsync(network);
                result[network] = deployments;
            }
            else
            {
                // Export all networks
                var networks = new[] { "private", "testnet", "mainnet" };
                foreach (var net in networks)
                {
                    var deployments = await _recordService.GetNetworkDeploymentsAsync(net);
                    if (deployments.Count > 0)
                    {
                        result[net] = deployments;
                    }
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// Create deployment summary report
        /// </summary>
        public async Task<string> CreateDeploymentReportAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Deployment Report");
            sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine();

            var networks = new[] { "private", "testnet", "mainnet" };
            
            foreach (var network in networks)
            {
                var deployments = await _recordService.GetNetworkDeploymentsAsync(network);
                if (deployments.Count == 0)
                    continue;

                sb.AppendLine($"## {network.ToUpperInvariant()}");
                sb.AppendLine();
                sb.AppendLine("| Contract | Version | Hash | Deployed | Updates |");
                sb.AppendLine("|----------|---------|------|----------|---------|");

                foreach (var (contractName, record) in deployments.OrderBy(d => d.Key))
                {
                    sb.AppendLine($"| {contractName} | {record.Version} | `{record.ContractHash}` | {record.DeployedAt:yyyy-MM-dd} | {record.UpdateHistory.Count} |");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}