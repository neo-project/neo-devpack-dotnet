using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo;
using Neo.SmartContract.Deploy.Configuration;
using System.Text.Json;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// File-based implementation of deployment record service
    /// </summary>
    public class DeploymentRecordService : IDeploymentRecordService
    {
        private readonly ILogger<DeploymentRecordService> _logger;
        private readonly DeploymentOptions _options;
        private readonly string _recordsPath;
        private readonly JsonSerializerOptions _jsonOptions;

        public DeploymentRecordService(
            ILogger<DeploymentRecordService> logger,
            IOptions<DeploymentOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _recordsPath = Path.Combine(
                Path.GetDirectoryName(_options.ContractsPath) ?? ".", 
                ".deployments"
            );
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Ensure directory exists
            Directory.CreateDirectory(_recordsPath);
        }

        public async Task SaveDeploymentAsync(string contractName, string network, DeploymentRecord record)
        {
            try
            {
                var filePath = GetDeploymentFilePath(contractName);
                var deployments = await LoadDeploymentsAsync(filePath);
                
                // Update or add the deployment record for this network
                deployments[network.ToLowerInvariant()] = record;
                
                // Save back to file
                var json = JsonSerializer.Serialize(deployments, _jsonOptions);
                await File.WriteAllTextAsync(filePath, json);
                
                _logger.LogInformation("Saved deployment record for {Contract} on {Network}", contractName, network);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save deployment record for {Contract}", contractName);
                throw;
            }
        }

        public async Task<DeploymentRecord?> GetDeploymentAsync(string contractName, string network)
        {
            try
            {
                var filePath = GetDeploymentFilePath(contractName);
                if (!File.Exists(filePath))
                {
                    return null;
                }

                var deployments = await LoadDeploymentsAsync(filePath);
                deployments.TryGetValue(network.ToLowerInvariant(), out var record);
                return record;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get deployment record for {Contract}", contractName);
                return null;
            }
        }

        public async Task<Dictionary<string, DeploymentRecord>> GetAllDeploymentsAsync(string contractName)
        {
            try
            {
                var filePath = GetDeploymentFilePath(contractName);
                if (!File.Exists(filePath))
                {
                    return new Dictionary<string, DeploymentRecord>();
                }

                return await LoadDeploymentsAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all deployments for {Contract}", contractName);
                return new Dictionary<string, DeploymentRecord>();
            }
        }

        public async Task<bool> IsDeployedAsync(string contractName, string network)
        {
            var deployment = await GetDeploymentAsync(contractName, network);
            return deployment != null;
        }

        public async Task RemoveDeploymentAsync(string contractName, string network)
        {
            try
            {
                var filePath = GetDeploymentFilePath(contractName);
                if (!File.Exists(filePath))
                {
                    return;
                }

                var deployments = await LoadDeploymentsAsync(filePath);
                if (deployments.Remove(network.ToLowerInvariant()))
                {
                    if (deployments.Count > 0)
                    {
                        var json = JsonSerializer.Serialize(deployments, _jsonOptions);
                        await File.WriteAllTextAsync(filePath, json);
                    }
                    else
                    {
                        // Remove file if no deployments left
                        File.Delete(filePath);
                    }
                    
                    _logger.LogInformation("Removed deployment record for {Contract} on {Network}", contractName, network);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove deployment record for {Contract}", contractName);
                throw;
            }
        }

        public async Task<Dictionary<string, DeploymentRecord>> GetNetworkDeploymentsAsync(string network)
        {
            var result = new Dictionary<string, DeploymentRecord>();
            
            try
            {
                if (!Directory.Exists(_recordsPath))
                {
                    return result;
                }

                var deploymentFiles = Directory.GetFiles(_recordsPath, "*.deployment.json");
                foreach (var file in deploymentFiles)
                {
                    var contractName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file));
                    var deployments = await LoadDeploymentsAsync(file);
                    
                    if (deployments.TryGetValue(network.ToLowerInvariant(), out var record))
                    {
                        result[contractName] = record;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get network deployments for {Network}", network);
            }
            
            return result;
        }

        private string GetDeploymentFilePath(string contractName)
        {
            return Path.Combine(_recordsPath, $"{contractName}.deployment.json");
        }

        private async Task<Dictionary<string, DeploymentRecord>> LoadDeploymentsAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<string, DeploymentRecord>(StringComparer.OrdinalIgnoreCase);
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var deployments = JsonSerializer.Deserialize<Dictionary<string, DeploymentRecord>>(json, _jsonOptions);
                return deployments ?? new Dictionary<string, DeploymentRecord>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load deployment file: {Path}", filePath);
                return new Dictionary<string, DeploymentRecord>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}