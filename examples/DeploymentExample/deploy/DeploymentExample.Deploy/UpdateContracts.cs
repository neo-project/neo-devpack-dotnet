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
/// Updates previously deployed contracts with new versions
/// </summary>
public class UpdateContracts
{
    private readonly ILogger<UpdateContracts> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    
    // Deployed contract hashes from previous deployment
    private const string TokenContractHash = "0x2db2dce76b4a7f8116ecfae0d819e7099cb3a256";
    private const string NFTContractHash = "0x8699c5d074fc27cdbd7caec486387c1a29300536";
    private const string GovernanceContractHash = "0xa3db58df3764610e43f3fda0c7b8633636c6c147";

    public UpdateContracts(IConfiguration configuration, ILogger<UpdateContracts> logger)
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
            _logger.LogInformation("Starting contract updates...");
            
            // Add a small change to each contract source file to trigger an update
            await UpdateTokenContract();
            await UpdateNFTContract();
            await UpdateGovernanceContract();
            
            _logger.LogInformation("✅ All contracts updated successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Contract update failed");
            throw;
        }
    }

    private async Task UpdateTokenContract()
    {
        await UpdateContract("TokenContract", TokenContractHash, 
            Path.Combine("..", "..", "src", "TokenContract", "TokenContract.cs"));
    }

    private async Task UpdateNFTContract()
    {
        await UpdateContract("NFTContract", NFTContractHash, 
            Path.Combine("..", "..", "src", "NFTContract", "NFTContract.cs"));
    }

    private async Task UpdateGovernanceContract()
    {
        await UpdateContract("GovernanceContract", GovernanceContractHash, 
            Path.Combine("..", "..", "src", "GovernanceContract", "GovernanceContract.cs"));
    }
    
    private async Task UpdateContract(string contractName, string contractHash, string sourcePath)
    {
        _logger.LogInformation($"Updating {contractName}...");
        
        // Read and modify the contract
        var content = await File.ReadAllTextAsync(sourcePath);
        
        // Add a version comment
        var className = $"public class {contractName} : SmartContract";
        if (!content.Contains("// Version: 1.1"))
        {
            content = content.Replace(className, $"// Version: 1.1\n    {className}");
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