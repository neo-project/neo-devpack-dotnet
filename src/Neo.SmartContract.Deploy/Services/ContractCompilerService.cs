using Microsoft.Extensions.Logging;
using Neo.Compiler;
using Neo.Extensions;
using Neo.IO;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Manifest;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Contract compilation service implementation
/// </summary>
public class ContractCompilerService : IContractCompiler
{
    private readonly ILogger<ContractCompilerService> _logger;

    public ContractCompilerService(ILogger<ContractCompilerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Compile a smart contract from source code or project
    /// </summary>
    public async Task<CompiledContract> CompileAsync(Models.CompilationOptions options)
    {
        // Determine the compilation path (prefer ProjectPath over SourcePath)
        var compilationPath = !string.IsNullOrEmpty(options.ProjectPath) ? options.ProjectPath : options.SourcePath;
        var isProjectCompilation = compilationPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
        
        _logger.LogInformation("Compiling contract from {Path} (Project: {IsProject})", 
            compilationPath, isProjectCompilation);

        if (!File.Exists(compilationPath))
        {
            throw new FileNotFoundException($"File not found: {compilationPath}");
        }

        // Ensure output directory exists
        Directory.CreateDirectory(options.OutputDirectory);

        try
        {
            string contractName;
            if (isProjectCompilation)
            {
                // For project compilation, use project name
                contractName = options.ContractName ?? Path.GetFileNameWithoutExtension(compilationPath);
            }
            else
            {
                // For source file compilation, use source file name
                contractName = options.ContractName ?? Path.GetFileNameWithoutExtension(compilationPath);
            }

            // Use Neo.Compiler.CSharp to compile
            var compileOptions = new Neo.Compiler.CompilationOptions
            {
                Optimize = options.Optimize ? Neo.Compiler.CompilationOptions.OptimizationType.All : Neo.Compiler.CompilationOptions.OptimizationType.None,
                Debug = options.GenerateDebugInfo ? Neo.Compiler.CompilationOptions.DebugType.Extended : Neo.Compiler.CompilationOptions.DebugType.None,
                Nullable = NullableContextOptions.Enable
            };

            // Compile the contract using the compiler engine
            var compiler = new Neo.Compiler.CompilationEngine(compileOptions);
            List<CompilationContext> results;
            
            if (isProjectCompilation)
            {
                // Compile from project
                results = compiler.CompileProject(compilationPath);
            }
            else
            {
                // Compile from source file
                results = compiler.CompileSources(compilationPath);
            }

            if (results.Count == 0)
            {
                throw new CompilationException(compilationPath, new[] { "No compilation results produced" });
            }

            var firstResult = results[0];

            // Check for compilation errors
            if (!firstResult.Success)
            {
                var errors = firstResult.Diagnostics.Select(d => d.ToString()).ToList();
                throw new CompilationException(compilationPath, errors);
            }

            // Create compilation results
            var (nef, manifest, debugInfo) = firstResult.CreateResults(options.OutputDirectory);

            // Get the output paths
            var nefPath = Path.Combine(options.OutputDirectory, $"{contractName}.nef");
            var manifestPath = Path.Combine(options.OutputDirectory, $"{contractName}.manifest.json");
            var debugInfoPath = Path.Combine(options.OutputDirectory, $"{contractName}.nefdbgnfo");

            // Write the compiled files
            await File.WriteAllBytesAsync(nefPath, nef.ToArray());
            await File.WriteAllTextAsync(manifestPath, manifest.ToJson().ToString());

            if (options.GenerateDebugInfo && debugInfo != null)
            {
                await File.WriteAllTextAsync(debugInfoPath, debugInfo.ToString());
            }

            _logger.LogInformation("Contract {ContractName} compiled successfully", contractName);

            return new CompiledContract
            {
                Name = contractName,
                NefFilePath = nefPath,
                ManifestFilePath = manifestPath,
                NefBytes = nef.ToArray(),
                Manifest = manifest
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to compile contract from {Path}", compilationPath);
            throw;
        }
    }

    /// <summary>
    /// Load compiled contract from artifacts
    /// </summary>
    public async Task<CompiledContract> LoadAsync(string nefFilePath, string manifestFilePath)
    {
        _logger.LogInformation("Loading contract artifacts from {NefPath} and {ManifestPath}",
            nefFilePath, manifestFilePath);

        if (!File.Exists(nefFilePath))
        {
            throw new FileNotFoundException($"NEF file not found: {nefFilePath}");
        }

        if (!File.Exists(manifestFilePath))
        {
            throw new FileNotFoundException($"Manifest file not found: {manifestFilePath}");
        }

        try
        {
            // Load NEF file
            var nefBytes = await File.ReadAllBytesAsync(nefFilePath);

            // Load manifest file
            var manifestJson = await File.ReadAllTextAsync(manifestFilePath);
            var manifest = ContractManifest.Parse(manifestJson);

            if (manifest == null)
            {
                throw new InvalidOperationException($"Failed to parse manifest file: {manifestFilePath}");
            }

            var contractName = Path.GetFileNameWithoutExtension(nefFilePath);

            _logger.LogInformation("Contract artifacts loaded for {ContractName}", contractName);

            return new CompiledContract
            {
                Name = contractName,
                NefFilePath = nefFilePath,
                ManifestFilePath = manifestFilePath,
                NefBytes = nefBytes,
                Manifest = manifest
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load contract artifacts from {NefPath}", nefFilePath);
            throw;
        }
    }
}
