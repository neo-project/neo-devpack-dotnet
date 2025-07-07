using Microsoft.Extensions.Logging;
using Neo.Json;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public async Task<DeploymentRecord?> GetDeploymentRecordAsync(string contractName, string network)
    {
        return await GetDeploymentRecordAsync<DeploymentRecord>(contractName, network);
    }

    public async Task RecordUpdateAsync(string contractName, string network, DeploymentRecord record)
    {
        try
        {
            // Get existing record if it exists
            var existingRecord = await GetDeploymentRecordAsync(contractName, network);

            if (existingRecord != null)
            {
                // Merge update history
                var existingHistory = existingRecord.UpdateHistory?.ToList() ?? new List<UpdateHistoryEntry>();
                var newHistory = record.UpdateHistory?.ToList() ?? new List<UpdateHistoryEntry>();

                existingHistory.AddRange(newHistory);

                record.DeployedAt = existingRecord.DeployedAt; // Preserve original deployment time
                record.UpdateHistory = existingHistory.ToArray();
            }

            // Save the updated record
            await SaveDeploymentRecordAsync(contractName, network, record);

            _logger.LogInformation($"Recorded update for {contractName} on {network}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to record update for {contractName}");
            throw;
        }
    }

    public async Task<bool> IsDeployedAsync(string contractName, string network)
    {
        var fileName = $"{contractName}.{network}.json";
        var filePath = Path.Combine(_recordsPath, fileName);
        return await Task.FromResult(File.Exists(filePath));
    }
}
