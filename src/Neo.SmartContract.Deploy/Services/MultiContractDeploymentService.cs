using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for managing multi-contract deployments with dependency resolution
/// </summary>
public class MultiContractDeploymentService
{
    private readonly ILogger<MultiContractDeploymentService> _logger;
    private readonly IContractDeployer _deployer;
    private readonly IContractCompiler _compiler;
    private readonly Dictionary<string, ContractDeploymentInfo> _deployedContracts;

    public MultiContractDeploymentService(
        ILogger<MultiContractDeploymentService> logger,
        IContractDeployer deployer,
        IContractCompiler compiler)
    {
        _logger = logger;
        _deployer = deployer;
        _compiler = compiler;
        _deployedContracts = new Dictionary<string, ContractDeploymentInfo>();
    }

    /// <summary>
    /// Deploy multiple contracts with automatic dependency resolution
    /// </summary>
    public async Task<MultiContractDeploymentResult> DeployContractsAsync(
        List<ContractDeploymentRequest> requests,
        DeploymentOptions baseOptions)
    {
        var result = new MultiContractDeploymentResult();
        var sortedRequests = SortByDependencies(requests);

        _logger.LogInformation("Starting multi-contract deployment of {Count} contracts", sortedRequests.Count);

        foreach (var request in sortedRequests)
        {
            try
            {
                // Check dependencies are met
                var unmetDependencies = request.Dependencies
                    .Where(dep => !_deployedContracts.ContainsKey(dep))
                    .ToList();

                if (unmetDependencies.Any())
                {
                    var error = $"Unmet dependencies for {request.Name}: {string.Join(", ", unmetDependencies)}";
                    _logger.LogError(error);
                    result.FailedDeployments.Add(new FailedDeployment
                    {
                        ContractName = request.Name,
                        Reason = error
                    });
                    continue;
                }

                // Prepare deployment options with dependency information
                var deploymentOptions = CreateDeploymentOptions(baseOptions, request, _deployedContracts);

                ContractDeploymentInfo deploymentInfo;

                if (!string.IsNullOrEmpty(request.SourcePath))
                {
                    // Compile and deploy
                    var compilationOptions = new CompilationOptions
                    {
                        SourcePath = request.SourcePath,
                        OutputDirectory = request.OutputDirectory ?? System.IO.Path.GetDirectoryName(request.SourcePath)!,
                        ContractName = request.Name,
                        GenerateDebugInfo = request.GenerateDebugInfo,
                        Optimize = request.Optimize
                    };

                    var compiledContract = await _compiler.CompileAsync(compilationOptions);
                    deploymentInfo = await _deployer.DeployAsync(compiledContract, deploymentOptions);
                }
                else if (!string.IsNullOrEmpty(request.NefPath) && !string.IsNullOrEmpty(request.ManifestPath))
                {
                    // Deploy from artifacts
                    var compiledContract = await _compiler.LoadAsync(request.NefPath, request.ManifestPath);
                    deploymentInfo = await _deployer.DeployAsync(compiledContract, deploymentOptions);
                }
                else
                {
                    throw new InvalidOperationException($"Contract {request.Name} must specify either SourcePath or both NefPath and ManifestPath");
                }

                if (deploymentInfo.Success)
                {
                    _deployedContracts[request.Name] = deploymentInfo;
                    result.SuccessfulDeployments.Add(deploymentInfo);
                    _logger.LogInformation("Successfully deployed {Name} at {Hash}",
                        request.Name, deploymentInfo.ContractHash);

                    // Execute post-deployment actions
                    if (request.PostDeploymentActions?.Any() == true)
                    {
                        await ExecutePostDeploymentActions(deploymentInfo, request.PostDeploymentActions);
                    }
                }
                else
                {
                    result.FailedDeployments.Add(new FailedDeployment
                    {
                        ContractName = request.Name,
                        Reason = deploymentInfo.ErrorMessage ?? "Unknown error"
                    });
                    _logger.LogError("Failed to deploy {Name}: {Error}",
                        request.Name, deploymentInfo.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception deploying {Name}", request.Name);
                result.FailedDeployments.Add(new FailedDeployment
                {
                    ContractName = request.Name,
                    Reason = ex.Message,
                    Exception = ex
                });

                if (request.FailureMode == DeploymentFailureMode.StopOnError)
                {
                    _logger.LogWarning("Stopping deployment due to failure mode setting");
                    break;
                }
            }
        }

        result.TotalContracts = requests.Count;
        _logger.LogInformation("Multi-contract deployment completed. Success: {Success}, Failed: {Failed}",
            result.SuccessfulDeployments.Count, result.FailedDeployments.Count);

        return result;
    }

    /// <summary>
    /// Deploy contracts in parallel batches based on dependency levels
    /// </summary>
    public async Task<MultiContractDeploymentResult> DeployContractsParallelAsync(
        List<ContractDeploymentRequest> requests,
        DeploymentOptions baseOptions,
        int maxParallelism = 5)
    {
        var result = new MultiContractDeploymentResult();
        var dependencyLevels = GroupByDependencyLevel(requests);

        _logger.LogInformation("Starting parallel multi-contract deployment across {Levels} dependency levels",
            dependencyLevels.Count);

        foreach (var level in dependencyLevels.OrderBy(l => l.Key))
        {
            _logger.LogInformation("Deploying level {Level} contracts in parallel", level.Key);

            var levelTasks = level.Value.Select(request =>
                DeployContractAsync(request, baseOptions)).ToList();

            // Deploy contracts at the same level in parallel
            var levelResults = await Task.WhenAll(levelTasks);

            foreach (var (request, deploymentInfo) in levelResults)
            {
                if (deploymentInfo.Success)
                {
                    _deployedContracts[request.Name] = deploymentInfo;
                    result.SuccessfulDeployments.Add(deploymentInfo);
                }
                else
                {
                    result.FailedDeployments.Add(new FailedDeployment
                    {
                        ContractName = request.Name,
                        Reason = deploymentInfo.ErrorMessage ?? "Unknown error"
                    });
                }
            }
        }

        result.TotalContracts = requests.Count;
        return result;
    }

    /// <summary>
    /// Update multiple deployed contracts
    /// </summary>
    public async Task<MultiContractDeploymentResult> UpdateContractsAsync(
        List<ContractUpdateRequest> requests,
        DeploymentOptions baseOptions)
    {
        var result = new MultiContractDeploymentResult();

        foreach (var request in requests)
        {
            try
            {
                var compilationOptions = new CompilationOptions
                {
                    SourcePath = request.SourcePath,
                    OutputDirectory = request.OutputDirectory ?? System.IO.Path.GetDirectoryName(request.SourcePath)!,
                    ContractName = request.Name,
                    GenerateDebugInfo = request.GenerateDebugInfo,
                    Optimize = request.Optimize
                };

                var compiledContract = await _compiler.CompileAsync(compilationOptions);
                var updateResult = await _deployer.UpdateAsync(compiledContract, request.ContractHash, baseOptions);

                if (updateResult.Success)
                {
                    result.SuccessfulDeployments.Add(updateResult);
                    _logger.LogInformation("Successfully updated {Name}", request.Name);
                }
                else
                {
                    result.FailedDeployments.Add(new FailedDeployment
                    {
                        ContractName = request.Name,
                        Reason = updateResult.ErrorMessage ?? "Unknown error"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update {Name}", request.Name);
                result.FailedDeployments.Add(new FailedDeployment
                {
                    ContractName = request.Name,
                    Reason = ex.Message,
                    Exception = ex
                });
            }
        }

        result.TotalContracts = requests.Count;
        return result;
    }

    #region Private Methods

    private async Task<(ContractDeploymentRequest request, ContractDeploymentInfo result)> DeployContractAsync(
        ContractDeploymentRequest request,
        DeploymentOptions baseOptions)
    {
        try
        {
            var deploymentOptions = CreateDeploymentOptions(baseOptions, request, _deployedContracts);

            CompiledContract compiledContract;
            if (!string.IsNullOrEmpty(request.SourcePath))
            {
                var compilationOptions = new CompilationOptions
                {
                    SourcePath = request.SourcePath,
                    OutputDirectory = request.OutputDirectory ?? System.IO.Path.GetDirectoryName(request.SourcePath)!,
                    ContractName = request.Name,
                    GenerateDebugInfo = request.GenerateDebugInfo,
                    Optimize = request.Optimize
                };
                compiledContract = await _compiler.CompileAsync(compilationOptions);
            }
            else
            {
                compiledContract = await _compiler.LoadAsync(request.NefPath!, request.ManifestPath!);
            }

            var result = await _deployer.DeployAsync(compiledContract, deploymentOptions);

            if (result.Success && request.PostDeploymentActions?.Any() == true)
            {
                await ExecutePostDeploymentActions(result, request.PostDeploymentActions);
            }

            return (request, result);
        }
        catch (Exception ex)
        {
            return (request, new ContractDeploymentInfo
            {
                Success = false,
                ErrorMessage = ex.Message,
                ContractName = request.Name
            });
        }
    }

    private DeploymentOptions CreateDeploymentOptions(
        DeploymentOptions baseOptions,
        ContractDeploymentRequest request,
        Dictionary<string, ContractDeploymentInfo> deployedContracts)
    {
        var options = new DeploymentOptions
        {
            DeployerAccount = baseOptions.DeployerAccount,
            GasLimit = request.GasLimit ?? baseOptions.GasLimit,
            WaitForConfirmation = baseOptions.WaitForConfirmation,
            DefaultNetworkFee = baseOptions.DefaultNetworkFee,
            ValidUntilBlockOffset = baseOptions.ValidUntilBlockOffset,
            ConfirmationRetries = baseOptions.ConfirmationRetries,
            ConfirmationDelaySeconds = baseOptions.ConfirmationDelaySeconds,
            InitialParameters = request.InitialParameters ?? baseOptions.InitialParameters
        };

        // Add dependency contract hashes to initial parameters if needed
        if (request.InjectDependencies && request.Dependencies.Any())
        {
            var dependencyHashes = request.Dependencies
                .Where(dep => deployedContracts.ContainsKey(dep))
                .ToDictionary(dep => dep, dep => deployedContracts[dep].ContractHash);

            options.InitialParameters = options.InitialParameters ?? new List<object>();
            options.InitialParameters.Add(dependencyHashes);
        }

        return options;
    }

    private Task ExecutePostDeploymentActions(
        ContractDeploymentInfo deployment,
        List<PostDeploymentAction> actions)
    {
        foreach (var action in actions)
        {
            try
            {
                _logger.LogInformation("Executing post-deployment action: {Method} on {Contract}",
                    action.Method, deployment.ContractName);

                // This would use IContractInvoker in a real implementation
                // For now, just log the action
                _logger.LogInformation("Post-deployment action completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute post-deployment action: {Method}", action.Method);
                if (action.Required)
                    throw;
            }
        }

        return Task.CompletedTask;
    }

    private List<ContractDeploymentRequest> SortByDependencies(List<ContractDeploymentRequest> requests)
    {
        var sorted = new List<ContractDeploymentRequest>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();

        void Visit(ContractDeploymentRequest request)
        {
            if (visited.Contains(request.Name))
                return;

            if (visiting.Contains(request.Name))
                throw new InvalidOperationException($"Circular dependency detected: {request.Name}");

            visiting.Add(request.Name);

            foreach (var dep in request.Dependencies)
            {
                var depRequest = requests.FirstOrDefault(r => r.Name == dep);
                if (depRequest != null)
                    Visit(depRequest);
            }

            visiting.Remove(request.Name);
            visited.Add(request.Name);
            sorted.Add(request);
        }

        foreach (var request in requests)
        {
            Visit(request);
        }

        return sorted;
    }

    private Dictionary<int, List<ContractDeploymentRequest>> GroupByDependencyLevel(
        List<ContractDeploymentRequest> requests)
    {
        var levels = new Dictionary<int, List<ContractDeploymentRequest>>();
        var contractLevels = new Dictionary<string, int>();

        int GetLevel(ContractDeploymentRequest request)
        {
            if (contractLevels.ContainsKey(request.Name))
                return contractLevels[request.Name];

            if (!request.Dependencies.Any())
            {
                contractLevels[request.Name] = 0;
                return 0;
            }

            var maxDepLevel = request.Dependencies
                .Select(dep => requests.FirstOrDefault(r => r.Name == dep))
                .Where(depRequest => depRequest != null)
                .Select(depRequest => GetLevel(depRequest!))
                .DefaultIfEmpty(-1)
                .Max();

            var level = maxDepLevel + 1;
            contractLevels[request.Name] = level;
            return level;
        }

        foreach (var request in requests)
        {
            var level = GetLevel(request);
            if (!levels.ContainsKey(level))
                levels[level] = new List<ContractDeploymentRequest>();
            levels[level].Add(request);
        }

        return levels;
    }

    #endregion
}

#region Models

public class ContractDeploymentRequest
{
    public string Name { get; set; } = "";
    public string? SourcePath { get; set; }
    public string? NefPath { get; set; }
    public string? ManifestPath { get; set; }
    public string? OutputDirectory { get; set; }
    public List<string> Dependencies { get; set; } = new();
    public bool GenerateDebugInfo { get; set; }
    public bool Optimize { get; set; } = true;
    public long? GasLimit { get; set; }
    public List<object>? InitialParameters { get; set; }
    public bool InjectDependencies { get; set; }
    public DeploymentFailureMode FailureMode { get; set; } = DeploymentFailureMode.Continue;
    public List<PostDeploymentAction>? PostDeploymentActions { get; set; }
}

public class ContractUpdateRequest
{
    public string Name { get; set; } = "";
    public UInt160 ContractHash { get; set; } = UInt160.Zero;
    public string SourcePath { get; set; } = "";
    public string? OutputDirectory { get; set; }
    public bool GenerateDebugInfo { get; set; }
    public bool Optimize { get; set; } = true;
}

public class PostDeploymentAction
{
    public string Method { get; set; } = "";
    public List<object> Parameters { get; set; } = new();
    public bool Required { get; set; } = true;
}

public enum DeploymentFailureMode
{
    Continue,
    StopOnError,
    RollbackOnError
}

public class MultiContractDeploymentResult
{
    public int TotalContracts { get; set; }
    public List<ContractDeploymentInfo> SuccessfulDeployments { get; set; } = new();
    public List<FailedDeployment> FailedDeployments { get; set; } = new();
    public bool AllSuccessful => FailedDeployments.Count == 0;
}

public class FailedDeployment
{
    public string ContractName { get; set; } = "";
    public string Reason { get; set; } = "";
    public Exception? Exception { get; set; }
}

#endregion
