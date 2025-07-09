using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Fluent builder for creating deployment manifests
/// </summary>
public class DeploymentManifestBuilder
{
    private readonly DeploymentManifest _manifest;

    /// <summary>
    /// Initialize a new deployment manifest builder
    /// </summary>
    public DeploymentManifestBuilder()
    {
        _manifest = new DeploymentManifest();
    }

    /// <summary>
    /// Set the deployment name
    /// </summary>
    /// <param name="name">Deployment name</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder WithName(string name)
    {
        _manifest.Name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>
    /// Set the deployment description
    /// </summary>
    /// <param name="description">Deployment description</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder WithDescription(string description)
    {
        _manifest.Description = description;
        return this;
    }

    /// <summary>
    /// Enable or disable continuation on error
    /// </summary>
    /// <param name="continueOnError">Whether to continue deployment on error</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder ContinueOnError(bool continueOnError = true)
    {
        _manifest.ContinueOnError = continueOnError;
        return this;
    }

    /// <summary>
    /// Enable batch deployment
    /// </summary>
    /// <param name="batchSize">Maximum contracts per batch</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder EnableBatching(int batchSize = 5)
    {
        _manifest.EnableBatching = true;
        _manifest.BatchSize = batchSize;
        return this;
    }

    /// <summary>
    /// Configure deployment settings
    /// </summary>
    /// <param name="configure">Configuration action</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder WithSettings(Action<DeploymentSettings> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        configure(_manifest.Settings);
        return this;
    }

    /// <summary>
    /// Add a contract from source
    /// </summary>
    /// <param name="id">Unique contract ID</param>
    /// <param name="name">Contract name</param>
    /// <param name="projectPath">Path to contract project</param>
    /// <param name="configure">Optional configuration</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder AddContract(
        string id, 
        string name, 
        string projectPath, 
        Action<ContractDefinitionBuilder>? configure = null)
    {
        var builder = new ContractDefinitionBuilder(id, name)
            .FromProject(projectPath);
        
        configure?.Invoke(builder);
        _manifest.Contracts.Add(builder.Build());
        
        return this;
    }

    /// <summary>
    /// Add a pre-compiled contract
    /// </summary>
    /// <param name="id">Unique contract ID</param>
    /// <param name="name">Contract name</param>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="configure">Optional configuration</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder AddCompiledContract(
        string id,
        string name,
        string nefPath,
        string manifestPath,
        Action<ContractDefinitionBuilder>? configure = null)
    {
        var builder = new ContractDefinitionBuilder(id, name)
            .FromArtifacts(nefPath, manifestPath);
        
        configure?.Invoke(builder);
        _manifest.Contracts.Add(builder.Build());
        
        return this;
    }

    /// <summary>
    /// Add a contract interaction
    /// </summary>
    /// <param name="source">Source contract ID</param>
    /// <param name="target">Target contract ID or hash</param>
    /// <param name="method">Method to call</param>
    /// <param name="configure">Optional configuration</param>
    /// <returns>Builder instance</returns>
    public DeploymentManifestBuilder AddInteraction(
        string source,
        string target,
        string method,
        Action<ContractInteractionBuilder>? configure = null)
    {
        var builder = new ContractInteractionBuilder(source, target, method);
        configure?.Invoke(builder);
        _manifest.Interactions.Add(builder.Build());
        
        return this;
    }

    /// <summary>
    /// Build the deployment manifest
    /// </summary>
    /// <returns>Deployment manifest</returns>
    public DeploymentManifest Build()
    {
        // Validate manifest
        if (string.IsNullOrWhiteSpace(_manifest.Name))
            throw new InvalidOperationException("Deployment manifest must have a name");

        if (_manifest.Contracts.Count == 0)
            throw new InvalidOperationException("Deployment manifest must contain at least one contract");

        return _manifest;
    }

    /// <summary>
    /// Save the manifest to a file
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Task</returns>
    public async Task SaveAsync(string path)
    {
        var manifest = Build();
        var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await File.WriteAllTextAsync(path, json);
    }

    /// <summary>
    /// Load a manifest from file and create a builder
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Builder instance</returns>
    public static async Task<DeploymentManifestBuilder> LoadAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var manifest = JsonSerializer.Deserialize<DeploymentManifest>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize manifest");

        var builder = new DeploymentManifestBuilder();
        builder._manifest.Version = manifest.Version;
        builder._manifest.Name = manifest.Name;
        builder._manifest.Description = manifest.Description;
        builder._manifest.Contracts = manifest.Contracts;
        builder._manifest.Interactions = manifest.Interactions;
        builder._manifest.Settings = manifest.Settings;
        builder._manifest.ContinueOnError = manifest.ContinueOnError;
        builder._manifest.EnableBatching = manifest.EnableBatching;
        builder._manifest.BatchSize = manifest.BatchSize;

        return builder;
    }
}

/// <summary>
/// Builder for contract definitions
/// </summary>
public class ContractDefinitionBuilder
{
    private readonly ContractDefinition _contract;

    internal ContractDefinitionBuilder(string id, string name)
    {
        _contract = new ContractDefinition
        {
            Id = id ?? throw new ArgumentNullException(nameof(id)),
            Name = name ?? throw new ArgumentNullException(nameof(name))
        };
    }

    /// <summary>
    /// Set contract description
    /// </summary>
    public ContractDefinitionBuilder WithDescription(string description)
    {
        _contract.Description = description;
        return this;
    }

    /// <summary>
    /// Set contract source project
    /// </summary>
    public ContractDefinitionBuilder FromProject(string projectPath)
    {
        _contract.ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
        return this;
    }

    /// <summary>
    /// Set contract artifacts
    /// </summary>
    public ContractDefinitionBuilder FromArtifacts(string nefPath, string manifestPath)
    {
        _contract.NefPath = nefPath ?? throw new ArgumentNullException(nameof(nefPath));
        _contract.ManifestPath = manifestPath ?? throw new ArgumentNullException(nameof(manifestPath));
        return this;
    }

    /// <summary>
    /// Add initialization parameters
    /// </summary>
    public ContractDefinitionBuilder WithInitParams(params object[] initParams)
    {
        _contract.InitParams = new List<object>(initParams);
        return this;
    }

    /// <summary>
    /// Add a dependency
    /// </summary>
    public ContractDefinitionBuilder DependsOn(string contractId)
    {
        if (!string.IsNullOrWhiteSpace(contractId))
            _contract.Dependencies.Add(contractId);
        return this;
    }

    /// <summary>
    /// Add multiple dependencies
    /// </summary>
    public ContractDefinitionBuilder DependsOn(params string[] contractIds)
    {
        foreach (var id in contractIds)
        {
            if (!string.IsNullOrWhiteSpace(id))
                _contract.Dependencies.Add(id);
        }
        return this;
    }

    /// <summary>
    /// Add a tag
    /// </summary>
    public ContractDefinitionBuilder WithTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag))
            _contract.Tags.Add(tag);
        return this;
    }

    /// <summary>
    /// Set expected contract hash
    /// </summary>
    public ContractDefinitionBuilder WithExpectedHash(string hash)
    {
        _contract.ExpectedHash = hash;
        return this;
    }

    /// <summary>
    /// Configure contract-specific options
    /// </summary>
    public ContractDefinitionBuilder WithOptions(Action<ContractDeploymentOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        
        _contract.Options ??= new ContractDeploymentOptions();
        configure(_contract.Options);
        return this;
    }

    /// <summary>
    /// Build the contract definition
    /// </summary>
    internal ContractDefinition Build() => _contract;
}

/// <summary>
/// Builder for contract interactions
/// </summary>
public class ContractInteractionBuilder
{
    private readonly ContractInteraction _interaction;

    internal ContractInteractionBuilder(string source, string target, string method)
    {
        _interaction = new ContractInteraction
        {
            Source = source ?? throw new ArgumentNullException(nameof(source)),
            Target = target ?? throw new ArgumentNullException(nameof(target)),
            Method = method ?? throw new ArgumentNullException(nameof(method))
        };
    }

    /// <summary>
    /// Set interaction description
    /// </summary>
    public ContractInteractionBuilder WithDescription(string description)
    {
        _interaction.Description = description;
        return this;
    }

    /// <summary>
    /// Add parameters
    /// </summary>
    public ContractInteractionBuilder WithParams(params object[] parameters)
    {
        _interaction.Params = new List<object>(parameters);
        return this;
    }

    /// <summary>
    /// Set execution order
    /// </summary>
    public ContractInteractionBuilder WithOrder(int order)
    {
        _interaction.Order = order;
        return this;
    }

    /// <summary>
    /// Mark interaction as optional
    /// </summary>
    public ContractInteractionBuilder AsOptional(bool optional = true)
    {
        _interaction.Optional = optional;
        return this;
    }

    /// <summary>
    /// Build the interaction
    /// </summary>
    internal ContractInteraction Build() => _interaction;
}