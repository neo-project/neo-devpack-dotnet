using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo;

namespace DeploymentExample.Deploy;

/// <summary>
/// Tests updating a single contract with update functionality
/// </summary>
public class TestSingleUpdate
{
    private readonly ILogger<TestSingleUpdate> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    
    // New updatable contract deployed in demo
    private const string TokenContractHash = "0xaef772277517be7405e42da00177b87c5293f413";

    public TestSingleUpdate(IConfiguration configuration, ILogger<TestSingleUpdate> logger)
    {
        _logger = logger;
        _configuration = configuration;
        
        // Set up service provider
        _serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddNeoContractDeployment()
            .AddSingleton(_configuration)
            .BuildServiceProvider();
    }

    public async Task RunAsync()
    {
        try
        {
            _logger.LogInformation("Testing update of updatable TokenContract...");
            
            await UpdateTokenContract();
            
            _logger.LogInformation("✅ Contract update test completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Contract update test failed");
            throw;
        }
    }

    private async Task UpdateTokenContract()
    {
        await UpdateContract("TokenContract", TokenContractHash, 
            Path.Combine("..", "..", "src", "TokenContract", "TokenContract.cs"));
    }
    
    private async Task UpdateContract(string contractName, string contractHash, string sourcePath)
    {
        _logger.LogInformation($"Updating {contractName}...");
        
        // Read and modify the contract
        var content = await File.ReadAllTextAsync(sourcePath);
        
        // Add a version comment
        var className = $"public class {contractName} : SmartContract";
        var versionComment = $"// Updated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
        
        if (!content.Contains(versionComment))
        {
            content = content.Replace(className, $"{versionComment}\n    {className}");
            await File.WriteAllTextAsync(sourcePath, content);
        }
        
        // Get WIF key
        var wifKey = Environment.GetEnvironmentVariable("NEO_WIF_KEY") ?? 
                     throw new InvalidOperationException("NEO_WIF_KEY environment variable not set");
        
        // Compile the contract
        var compiler = _serviceProvider.GetRequiredService<IContractCompiler>();
        var compilationOptions = new CompilationOptions
        {
            SourcePath = sourcePath,
            ContractName = contractName
        };
        var compiledContract = await compiler.CompileAsync(compilationOptions);
        
        // Create deployment options
        var deploymentOptions = new DeploymentOptions
        {
            WifKey = wifKey,
            RpcUrl = _configuration["Network:RpcUrl"] ?? "https://testnet1.neo.coz.io:443",
            NetworkMagic = 894710606, // TestNet
            GasLimit = 100_000_000,
            WaitForConfirmation = true
        };
        
        // Update the contract directly
        var deployer = _serviceProvider.GetRequiredService<IContractDeployer>();
        var result = await deployer.UpdateAsync(compiledContract, UInt160.Parse(contractHash), deploymentOptions);
        
        if (result.Success)
        {
            _logger.LogInformation($"✅ {contractName} updated: TX {result.TransactionHash}");
        }
        else
        {
            throw new Exception($"Failed to update {contractName}: {result.ErrorMessage}");
        }
    }
}