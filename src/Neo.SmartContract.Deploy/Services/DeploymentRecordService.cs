using Microsoft.Extensions.Logging;
using Neo.Json;
using Neo.SmartContract.Deploy.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for managing deployment records
/// </summary>
public class DeploymentRecordService : IDeploymentRecordService
{
    private readonly ILogger<DeploymentRecordService> _logger;
    private readonly string _recordsPath;

    public DeploymentRecordService(ILogger<DeploymentRecordService> logger)
    {
        _logger = logger;
        _recordsPath = Path.Combine(Directory.GetCurrentDirectory(), ".deployments");

        // Ensure directory exists
        if (!Directory.Exists(_recordsPath))
        {
            Directory.CreateDirectory(_recordsPath);
        }
    }

    public async Task SaveDeploymentRecordAsync(string contractName, string network, object record)
    {
        try
        {
            var fileName = $"{contractName}.{network}.json";
            var filePath = Path.Combine(_recordsPath, fileName);

            // For now, use a simple JSON representation
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(record, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
            _logger.LogInformation($"Saved deployment record for {contractName} on {network}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to save deployment record for {contractName}");
            throw;
        }
    }

    public async Task<T?> GetDeploymentRecordAsync<T>(string contractName, string network) where T : class
    {
        try
        {
            var fileName = $"{contractName}.{network}.json";
            var filePath = Path.Combine(_recordsPath, fileName);

            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get deployment record for {contractName}");
            return null;
        }
    }

    public async Task<bool> IsDeployedAsync(string contractName, string network)
    {
        var fileName = $"{contractName}.{network}.json";
        var filePath = Path.Combine(_recordsPath, fileName);
        return await Task.FromResult(File.Exists(filePath));
    }
}
