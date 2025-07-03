using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using System.Numerics;

namespace SampleToken.Deploy
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Check if user wants streamlined mode
            if (args.Length > 0 && args[0] == "--streamlined")
            {
                await StreamlinedDeployment();
                return;
            }

            // Otherwise use the full-featured deployment
            await FullDeployment();
        }

        /// <summary>
        /// Streamlined deployment using DeploymentToolkit
        /// </summary>
        static async Task StreamlinedDeployment()
        {
            Console.WriteLine("=== Streamlined Token Deployment ===\n");

            try
            {
                // Create toolkit - auto-loads config from appsettings.json
                var toolkit = new DeploymentToolkit();
                
                // Set network based on environment or default to testnet
                var network = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() ?? "testnet";
                toolkit.SetNetwork(network);
                
                Console.WriteLine($"Network: {network}");
                
                // Get deployer account
                var owner = await toolkit.GetDeployerAccount();
                Console.WriteLine($"Deployer: {owner.ToAddress(53)}");
                
                // Deploy the contract with owner initialization
                var contractPath = "../../src/SampleToken.Contract/SampleToken.Contract.csproj";
                var result = await toolkit.Deploy(contractPath, new object[] { owner });
                
                if (result.Success)
                {
                    Console.WriteLine($"\n✅ Contract deployed!");
                    Console.WriteLine($"   Hash: {result.ContractHash}");
                    Console.WriteLine($"   Transaction: {result.TransactionHash}");
                    
                    // Verify deployment
                    var name = await toolkit.Call<string>(result.ContractHash.ToString(), "getName");
                    var symbol = await toolkit.Call<string>(result.ContractHash.ToString(), "getSymbol");
                    var totalSupply = await toolkit.Call<BigInteger>(result.ContractHash.ToString(), "totalSupply");
                    
                    Console.WriteLine($"\n   Token: {name} ({symbol})");
                    Console.WriteLine($"   Total Supply: {totalSupply}");
                    
                    // Check GAS balance
                    var gasBalance = await toolkit.GetGasBalance();
                    Console.WriteLine($"   Deployer GAS: {gasBalance}");
                }
                else
                {
                    Console.WriteLine($"\n❌ Deployment failed: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Full-featured deployment with detailed configuration
        /// </summary>
        static async Task FullDeployment()
        {
            Console.WriteLine("=== Sample Token Deployment ===");
            Console.WriteLine();

            try
            {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                // Create toolkit with configuration
                var toolkit = NeoContractToolkitBuilder.Create()
                    .ConfigureLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Information);
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<IConfiguration>(configuration);
                    })
                    .Build();

                // Load wallet
                var walletPath = configuration["Wallet:Path"] ?? "wallet.json";
                var walletPassword = configuration["Wallet:Password"] ?? 
                    throw new InvalidOperationException("Wallet password not configured");

                Console.WriteLine($"Loading wallet from: {walletPath}");
                await toolkit.LoadWalletAsync(walletPath, walletPassword);
                
                var deployerAccount = toolkit.GetDeployerAccount();
                Console.WriteLine($"Deployer account: {deployerAccount.ToAddress(53)}"); // 53 = N3 MainNet

                // Compile the contract
                var contractPath = Path.GetFullPath(Path.Combine(
                    Directory.GetCurrentDirectory(), 
                    "../../src/SampleToken.Contract/SampleToken.Contract.csproj"));

                Console.WriteLine($"\nCompiling contract from: {contractPath}");
                
                var compilationOptions = new CompilationOptions
                {
                    ProjectPath = contractPath,
                    OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "artifacts"),
                    ContractName = "SampleToken",
                    GenerateDebugInfo = true,
                    Optimize = true
                };

                // Set deployment options
                var deploymentOptions = new DeploymentOptions
                {
                    ContractName = "SampleToken",
                    DeployerAccount = deployerAccount,
                    GasLimit = configuration.GetValue<long>("Deployment:GasLimit", 100_000_000),
                    WaitForConfirmation = configuration.GetValue<bool>("Deployment:WaitForConfirmation", true),
                    InitializationParams = new object[] { deployerAccount } // Pass deployer as initial owner
                };

                // Deploy the contract
                Console.WriteLine("\nDeploying contract...");
                var result = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);

                if (result.Success)
                {
                    Console.WriteLine($"\n✅ Contract deployed successfully!");
                    Console.WriteLine($"   Contract Hash: {result.ContractHash}");
                    Console.WriteLine($"   Transaction: {result.TransactionHash}");
                    Console.WriteLine($"   Gas Consumed: {result.GasConsumed} GAS");

                    // Save deployment info
                    var deploymentInfo = new
                    {
                        ContractHash = result.ContractHash.ToString(),
                        TransactionHash = result.TransactionHash.ToString(),
                        DeployedAt = DateTime.UtcNow,
                        Network = configuration["Network:Network"],
                        RpcUrl = configuration["Network:RpcUrl"]
                    };

                    var deploymentInfoPath = Path.Combine(
                        compilationOptions.OutputDirectory,
                        "deployment-info.json");
                    
                    await File.WriteAllTextAsync(deploymentInfoPath, 
                        System.Text.Json.JsonSerializer.Serialize(deploymentInfo, 
                            new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                    
                    Console.WriteLine($"\n   Deployment info saved to: {deploymentInfoPath}");

                    // Perform initial setup if needed
                    await PerformInitialSetup(toolkit, result.ContractHash);
                }
                else
                {
                    Console.WriteLine($"\n❌ Deployment failed: {result.ErrorMessage}");
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }

        static async Task PerformInitialSetup(NeoContractToolkit toolkit, UInt160 contractHash)
        {
            Console.WriteLine("\n=== Performing Initial Setup ===");

            try
            {
                // Verify deployment
                Console.WriteLine("\n1. Verifying contract deployment...");
                
                var name = await toolkit.CallContractAsync<string>(contractHash, "getName");
                var symbol = await toolkit.CallContractAsync<string>(contractHash, "getSymbol");
                var decimals = await toolkit.CallContractAsync<BigInteger>(contractHash, "getDecimals");
                var totalSupply = await toolkit.CallContractAsync<BigInteger>(contractHash, "totalSupply");
                var owner = await toolkit.CallContractAsync<UInt160>(contractHash, "getOwner");

                Console.WriteLine($"   Token Name: {name}");
                Console.WriteLine($"   Symbol: {symbol}");
                Console.WriteLine($"   Decimals: {decimals}");
                Console.WriteLine($"   Total Supply: {FormatTokenAmount(totalSupply, (byte)decimals)}");
                Console.WriteLine($"   Owner: {owner.ToAddress(53)}");

                // Check deployer balance
                var deployerAccount = toolkit.GetDeployerAccount();
                var balance = await toolkit.CallContractAsync<BigInteger>(
                    contractHash, "balanceOf", deployerAccount);
                
                Console.WriteLine($"   Deployer Balance: {FormatTokenAmount(balance, (byte)decimals)}");

                // Optional: Set up initial minters
                Console.WriteLine("\n2. Setting up initial configuration...");
                
                // Example: Add a minter (uncomment to use)
                // var minterAddress = "NXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx";
                // var minter = minterAddress.ToScriptHash(53);
                // var txHash = await toolkit.InvokeContractAsync(
                //     contractHash, "setMinter", minter, true);
                // Console.WriteLine($"   Added minter: {minterAddress} (tx: {txHash})");

                Console.WriteLine("   ✅ Initial setup complete!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Setup warning: {ex.Message}");
            }
        }

        static string FormatTokenAmount(BigInteger amount, byte decimals)
        {
            var divisor = BigInteger.Pow(10, decimals);
            var wholePart = amount / divisor;
            var fractionalPart = amount % divisor;
            
            if (fractionalPart == 0)
                return $"{wholePart:N0}";
            
            var fractionalStr = fractionalPart.ToString().PadLeft(decimals, '0').TrimEnd('0');
            return $"{wholePart:N0}.{fractionalStr}";
        }
    }
}