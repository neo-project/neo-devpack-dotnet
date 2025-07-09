using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for compiling Neo smart contracts
/// </summary>
public class ContractCompilerService : IContractCompiler
{
    private readonly ILogger<ContractCompilerService>? _logger;

    /// <summary>
    /// Initialize a new instance of ContractCompilerService
    /// </summary>
    /// <param name="logger">Optional logger</param>
    public ContractCompilerService(ILogger<ContractCompilerService>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Compile a smart contract from source code
    /// </summary>
    /// <param name="projectPath">Path to the contract project (.csproj) or source file</param>
    /// <returns>Compiled contract</returns>
    public async Task<CompiledContract> CompileAsync(string projectPath)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
            throw new ArgumentException("Project path cannot be null or empty", nameof(projectPath));

        if (!File.Exists(projectPath))
            throw new FileNotFoundException($"Project file not found: {projectPath}");

        _logger?.LogInformation("Compiling contract from: {ProjectPath}", projectPath);

        var projectDir = Path.GetDirectoryName(projectPath) ?? ".";
        var contractName = Path.GetFileNameWithoutExtension(projectPath);

        try
        {
            // First, restore the project dependencies
            await RunDotNetCommandAsync("restore", projectPath, projectDir);

            // Build the project with Neo.SmartContract.Framework
            await RunDotNetCommandAsync($"build \"{projectPath}\" -c Release", null, projectDir);

            // Find the generated NEF and manifest files
            var outputDir = Path.Combine(projectDir, "bin", "Release");
            if (!Directory.Exists(outputDir))
            {
                outputDir = Path.Combine(projectDir, "bin", "Debug");
            }

            var nefPath = FindFileRecursive(outputDir, "*.nef");
            var manifestPath = FindFileRecursive(outputDir, "*.manifest.json");

            if (string.IsNullOrEmpty(nefPath) || string.IsNullOrEmpty(manifestPath))
            {
                throw new ContractDeploymentException(
                    $"Compilation failed: Could not find NEF or manifest files in {outputDir}");
            }

            _logger?.LogInformation("Contract compiled successfully. NEF: {NefPath}, Manifest: {ManifestPath}", 
                nefPath, manifestPath);

            return await LoadContractAsync(nefPath, manifestPath);
        }
        catch (Exception ex) when (!(ex is ContractDeploymentException))
        {
            throw new ContractDeploymentException($"Failed to compile contract: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Load a pre-compiled contract from NEF and manifest files
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <returns>Compiled contract</returns>
    public async Task<CompiledContract> LoadContractAsync(string nefPath, string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(nefPath))
            throw new ArgumentException("NEF path cannot be null or empty", nameof(nefPath));

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path cannot be null or empty", nameof(manifestPath));

        if (!File.Exists(nefPath))
            throw new FileNotFoundException($"NEF file not found: {nefPath}");

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException($"Manifest file not found: {manifestPath}");

        _logger?.LogInformation("Loading contract from NEF: {NefPath}, Manifest: {ManifestPath}", 
            nefPath, manifestPath);

        try
        {
            var nefBytes = await File.ReadAllBytesAsync(nefPath);
            var manifestJson = await File.ReadAllTextAsync(manifestPath);
            
            // Parse and validate the manifest
            var manifest = ContractManifest.Parse(manifestJson);
            var contractName = manifest.Name;

            return new CompiledContract
            {
                Name = contractName,
                NefFilePath = nefPath,
                ManifestFilePath = manifestPath,
                NefBytes = nefBytes,
                Manifest = manifest
            };
        }
        catch (Exception ex)
        {
            throw new ContractDeploymentException($"Failed to load contract files: {ex.Message}", ex);
        }
    }

    private async Task RunDotNetCommandAsync(string arguments, string? projectPath, string workingDirectory)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };
        
        var outputData = new System.Text.StringBuilder();
        var errorData = new System.Text.StringBuilder();

        process.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                outputData.AppendLine(args.Data);
                _logger?.LogDebug("dotnet output: {Output}", args.Data);
            }
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                errorData.AppendLine(args.Data);
                _logger?.LogWarning("dotnet error: {Error}", args.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = errorData.Length > 0 ? errorData.ToString() : outputData.ToString();
            throw new ContractDeploymentException(
                $"dotnet command failed with exit code {process.ExitCode}: {error}");
        }
    }

    private string? FindFileRecursive(string directory, string searchPattern)
    {
        try
        {
            var files = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
            return files.Length > 0 ? files[0] : null;
        }
        catch
        {
            return null;
        }
    }
}