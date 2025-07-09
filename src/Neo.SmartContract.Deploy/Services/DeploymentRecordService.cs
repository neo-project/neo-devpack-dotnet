using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for tracking and persisting deployment records
/// </summary>
public class DeploymentRecordService
{
    private readonly ILogger<DeploymentRecordService>? _logger;
    private readonly string? _persistencePath;
    private readonly ConcurrentDictionary<string, DeploymentRecord> _records;
    private readonly object _persistenceLock = new();

    /// <summary>
    /// Initialize a new instance of DeploymentRecordService
    /// </summary>
    /// <param name="persistencePath">Optional path to persist records</param>
    /// <param name="logger">Optional logger</param>
    public DeploymentRecordService(string? persistencePath = null, ILogger<DeploymentRecordService>? logger = null)
    {
        _persistencePath = persistencePath;
        _logger = logger;
        _records = new ConcurrentDictionary<string, DeploymentRecord>();

        if (!string.IsNullOrEmpty(_persistencePath))
        {
            LoadRecords();
        }
    }

    /// <summary>
    /// Record a deployment
    /// </summary>
    /// <param name="deploymentInfo">Deployment information</param>
    /// <param name="options">Deployment options used</param>
    /// <param name="additionalData">Additional data to store</param>
    /// <returns>Deployment record ID</returns>
    public async Task<string> RecordDeploymentAsync(ContractDeploymentInfo deploymentInfo, DeploymentOptions options, Dictionary<string, object>? additionalData = null)
    {
        var record = new DeploymentRecord
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            ContractName = deploymentInfo.ContractName,
            ContractHash = deploymentInfo.ContractHash.ToString(),
            TransactionHash = deploymentInfo.TransactionHash?.ToString(),
            DeployerAddress = deploymentInfo.DeployerAddress,
            RpcUrl = options.RpcUrl,
            NetworkMagic = options.NetworkMagic,
            GasUsed = deploymentInfo.GasConsumed,
            Success = true,
            AdditionalData = additionalData ?? new Dictionary<string, object>()
        };

        // Add deployment options metadata
        record.Metadata = new Dictionary<string, object>
        {
            ["WaitForConfirmation"] = options.WaitForConfirmation,
            ["VerifyAfterDeploy"] = options.VerifyAfterDeploy,
            ["DryRun"] = options.DryRun,
            ["GasLimit"] = options.GasLimit,
            ["NetworkFee"] = options.DefaultNetworkFee
        };

        _records[record.Id] = record;
        _logger?.LogInformation("Recorded deployment: {ContractName} ({ContractHash}) with ID: {RecordId}", 
            record.ContractName, record.ContractHash, record.Id);

        await PersistRecordsAsync();
        return record.Id;
    }

    /// <summary>
    /// Record a failed deployment
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="error">Error details</param>
    /// <param name="options">Deployment options used</param>
    /// <param name="additionalData">Additional data to store</param>
    /// <returns>Deployment record ID</returns>
    public async Task<string> RecordFailedDeploymentAsync(string contractName, Exception error, DeploymentOptions options, Dictionary<string, object>? additionalData = null)
    {
        var record = new DeploymentRecord
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            ContractName = contractName,
            RpcUrl = options.RpcUrl,
            NetworkMagic = options.NetworkMagic,
            Success = false,
            Error = error.Message,
            ErrorDetails = error.ToString(),
            AdditionalData = additionalData ?? new Dictionary<string, object>()
        };

        _records[record.Id] = record;
        _logger?.LogWarning("Recorded failed deployment: {ContractName} with ID: {RecordId}, Error: {Error}", 
            contractName, record.Id, error.Message);

        await PersistRecordsAsync();
        return record.Id;
    }

    /// <summary>
    /// Get a deployment record by ID
    /// </summary>
    /// <param name="recordId">Record ID</param>
    /// <returns>Deployment record or null</returns>
    public DeploymentRecord? GetRecord(string recordId)
    {
        return _records.TryGetValue(recordId, out var record) ? record : null;
    }

    /// <summary>
    /// Get all deployment records
    /// </summary>
    /// <param name="includeFailures">Whether to include failed deployments</param>
    /// <returns>List of deployment records</returns>
    public List<DeploymentRecord> GetAllRecords(bool includeFailures = true)
    {
        var query = _records.Values.AsEnumerable();
        
        if (!includeFailures)
        {
            query = query.Where(r => r.Success);
        }

        return query.OrderByDescending(r => r.Timestamp).ToList();
    }

    /// <summary>
    /// Get deployment records by contract name
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="includeFailures">Whether to include failed deployments</param>
    /// <returns>List of deployment records</returns>
    public List<DeploymentRecord> GetRecordsByContractName(string contractName, bool includeFailures = true)
    {
        var query = _records.Values.Where(r => r.ContractName.Equals(contractName, StringComparison.OrdinalIgnoreCase));
        
        if (!includeFailures)
        {
            query = query.Where(r => r.Success);
        }

        return query.OrderByDescending(r => r.Timestamp).ToList();
    }

    /// <summary>
    /// Get deployment records by contract hash
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <returns>List of deployment records</returns>
    public List<DeploymentRecord> GetRecordsByContractHash(string contractHash)
    {
        return _records.Values
            .Where(r => r.ContractHash?.Equals(contractHash, StringComparison.OrdinalIgnoreCase) == true)
            .OrderByDescending(r => r.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Get deployment records within a time range
    /// </summary>
    /// <param name="startTime">Start time (UTC)</param>
    /// <param name="endTime">End time (UTC)</param>
    /// <param name="includeFailures">Whether to include failed deployments</param>
    /// <returns>List of deployment records</returns>
    public List<DeploymentRecord> GetRecordsByTimeRange(DateTime startTime, DateTime endTime, bool includeFailures = true)
    {
        var query = _records.Values.Where(r => r.Timestamp >= startTime && r.Timestamp <= endTime);
        
        if (!includeFailures)
        {
            query = query.Where(r => r.Success);
        }

        return query.OrderByDescending(r => r.Timestamp).ToList();
    }

    /// <summary>
    /// Get deployment summary statistics
    /// </summary>
    /// <returns>Deployment summary</returns>
    public DeploymentSummary GetSummary()
    {
        var records = _records.Values.ToList();
        
        return new DeploymentSummary
        {
            TotalDeployments = records.Count,
            SuccessfulDeployments = records.Count(r => r.Success),
            FailedDeployments = records.Count(r => !r.Success),
            UniqueContracts = records.Where(r => !string.IsNullOrEmpty(r.ContractHash))
                                   .Select(r => r.ContractHash)
                                   .Distinct()
                                   .Count(),
            TotalGasUsed = records.Where(r => r.GasUsed.HasValue)
                                 .Sum(r => r.GasUsed!.Value),
            AverageGasUsed = records.Any(r => r.GasUsed.HasValue) 
                           ? records.Where(r => r.GasUsed.HasValue).Average(r => r.GasUsed!.Value)
                           : 0,
            FirstDeployment = records.Any() ? records.Min(r => r.Timestamp) : null,
            LastDeployment = records.Any() ? records.Max(r => r.Timestamp) : null,
            ContractDeploymentCounts = records
                .Where(r => r.Success && !string.IsNullOrEmpty(r.ContractName))
                .GroupBy(r => r.ContractName)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    /// <summary>
    /// Export records to JSON
    /// </summary>
    /// <param name="path">Export file path</param>
    /// <param name="includeFailures">Whether to include failed deployments</param>
    public async Task ExportToJsonAsync(string path, bool includeFailures = true)
    {
        var records = GetAllRecords(includeFailures);
        var json = JsonSerializer.Serialize(records, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await File.WriteAllTextAsync(path, json);
        _logger?.LogInformation("Exported {Count} deployment records to: {Path}", records.Count, path);
    }

    /// <summary>
    /// Clear all records
    /// </summary>
    public async Task ClearRecordsAsync()
    {
        _records.Clear();
        await PersistRecordsAsync();
        _logger?.LogInformation("Cleared all deployment records");
    }

    private async Task PersistRecordsAsync()
    {
        if (string.IsNullOrEmpty(_persistencePath))
            return;

        await Task.Run(() =>
        {
            lock (_persistenceLock)
            {
                try
                {
                    var directory = Path.GetDirectoryName(_persistencePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var json = JsonSerializer.Serialize(_records.Values.ToList(), new JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    File.WriteAllText(_persistencePath, json);
                    _logger?.LogDebug("Persisted {Count} deployment records", _records.Count);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to persist deployment records");
                }
            }
        });
    }

    private void LoadRecords()
    {
        if (string.IsNullOrEmpty(_persistencePath) || !File.Exists(_persistencePath))
            return;

        lock (_persistenceLock)
        {
            try
            {
                var json = File.ReadAllText(_persistencePath);
                var records = JsonSerializer.Deserialize<List<DeploymentRecord>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (records != null)
                {
                    foreach (var record in records)
                    {
                        _records[record.Id] = record;
                    }
                    _logger?.LogInformation("Loaded {Count} deployment records", records.Count);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load deployment records");
            }
        }
    }
}

/// <summary>
/// Deployment record
/// </summary>
public class DeploymentRecord
{
    /// <summary>
    /// Unique record ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of deployment
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Contract name
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Contract hash (if successful)
    /// </summary>
    public string? ContractHash { get; set; }

    /// <summary>
    /// Transaction hash (if successful)
    /// </summary>
    public string? TransactionHash { get; set; }

    /// <summary>
    /// Deployer address
    /// </summary>
    public string? DeployerAddress { get; set; }

    /// <summary>
    /// RPC URL used
    /// </summary>
    public string? RpcUrl { get; set; }

    /// <summary>
    /// Network magic
    /// </summary>
    public uint? NetworkMagic { get; set; }

    /// <summary>
    /// Gas used for deployment
    /// </summary>
    public long? GasUsed { get; set; }

    /// <summary>
    /// Whether deployment was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message (if failed)
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Detailed error information (if failed)
    /// </summary>
    public string? ErrorDetails { get; set; }

    /// <summary>
    /// Deployment metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Additional custom data
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Deployment summary statistics
/// </summary>
public class DeploymentSummary
{
    /// <summary>
    /// Total number of deployments
    /// </summary>
    public int TotalDeployments { get; set; }

    /// <summary>
    /// Number of successful deployments
    /// </summary>
    public int SuccessfulDeployments { get; set; }

    /// <summary>
    /// Number of failed deployments
    /// </summary>
    public int FailedDeployments { get; set; }

    /// <summary>
    /// Number of unique contracts deployed
    /// </summary>
    public int UniqueContracts { get; set; }

    /// <summary>
    /// Total gas used across all deployments
    /// </summary>
    public long TotalGasUsed { get; set; }

    /// <summary>
    /// Average gas used per deployment
    /// </summary>
    public double AverageGasUsed { get; set; }

    /// <summary>
    /// First deployment timestamp
    /// </summary>
    public DateTime? FirstDeployment { get; set; }

    /// <summary>
    /// Last deployment timestamp
    /// </summary>
    public DateTime? LastDeployment { get; set; }

    /// <summary>
    /// Deployment counts by contract name
    /// </summary>
    public Dictionary<string, int> ContractDeploymentCounts { get; set; } = new();

    /// <summary>
    /// Success rate
    /// </summary>
    public double SuccessRate => TotalDeployments > 0 ? (double)SuccessfulDeployments / TotalDeployments : 0;
}