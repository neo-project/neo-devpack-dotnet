using System;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Neo.SmartContract.Deploy;

namespace MyContract.Deploy
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await Parser.Default.ParseArguments<DeployOptions, UpdateOptions, InvokeOptions, InfoOptions>(args)
                .MapResult(
                    async (DeployOptions opts) => await RunDeploy(opts),
                    async (UpdateOptions opts) => await RunUpdate(opts),
                    async (InvokeOptions opts) => await RunInvoke(opts),
                    async (InfoOptions opts) => await RunInfo(opts),
                    errs => Task.FromResult(1));
        }

        static async Task<int> RunDeploy(DeployOptions options)
        {
            try
            {
                Console.WriteLine("🚀 Deploying MyContract to {0}...", options.Network);

                using var toolkit = new DeploymentToolkit(options.ConfigFile)
                    .SetNetwork(options.Network)
                    .SetWifKey(options.WifKey ?? Environment.GetEnvironmentVariable("NEO_WIF_KEY") ?? 
                        throw new Exception("WIF key not provided"));

                // Build contract path
                var contractPath = options.ContractPath ?? 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../src/MyContract/MyContract.csproj");

                // Deploy the contract
                var result = await toolkit.DeployAsync(contractPath);

                Console.WriteLine("✅ Contract deployed successfully!");
                Console.WriteLine($"   Contract Hash: {result.ContractHash}");
                Console.WriteLine($"   Contract Address: {result.ContractAddress}");
                Console.WriteLine($"   Transaction: {result.TransactionHash}");
                Console.WriteLine($"   Gas Consumed: {result.GasConsumed / 100000000m:F8} GAS");

                // Save deployment info
                if (!string.IsNullOrEmpty(options.OutputFile))
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    await File.WriteAllTextAsync(options.OutputFile, json);
                    Console.WriteLine($"💾 Deployment info saved to: {options.OutputFile}");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Deployment failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                return 1;
            }
        }

        static async Task<int> RunUpdate(UpdateOptions options)
        {
            try
            {
                Console.WriteLine("🔄 Updating contract {0} on {1}...", options.ContractHash, options.Network);

                using var toolkit = new DeploymentToolkit(options.ConfigFile)
                    .SetNetwork(options.Network)
                    .SetWifKey(options.WifKey ?? Environment.GetEnvironmentVariable("NEO_WIF_KEY") ?? 
                        throw new Exception("WIF key not provided"));

                // Build contract path
                var contractPath = options.ContractPath ?? 
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../src/MyContract/MyContract.csproj");

                // Update the contract
                var result = await toolkit.UpdateAsync(options.ContractHash, contractPath);

                Console.WriteLine("✅ Contract updated successfully!");
                Console.WriteLine($"   Transaction: {result.TransactionHash}");
                Console.WriteLine($"   Gas Consumed: {result.GasConsumed / 100000000m:F8} GAS");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Update failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                return 1;
            }
        }

        static async Task<int> RunInvoke(InvokeOptions options)
        {
            try
            {
                Console.WriteLine("📞 Invoking {0}.{1} on {2}...", options.ContractHash, options.Method, options.Network);

                using var toolkit = new DeploymentToolkit(options.ConfigFile)
                    .SetNetwork(options.Network);

                // Parse arguments
                var args = ParseArguments(options.Arguments);

                if (options.ReadOnly)
                {
                    // Call method (read-only)
                    var result = await toolkit.CallAsync<object>(options.ContractHash, options.Method, args);
                    Console.WriteLine("✅ Method called successfully!");
                    Console.WriteLine($"   Result: {result}");
                }
                else
                {
                    // Invoke method (transaction)
                    toolkit.SetWifKey(options.WifKey ?? Environment.GetEnvironmentVariable("NEO_WIF_KEY") ?? 
                        throw new Exception("WIF key not provided"));
                    
                    var txHash = await toolkit.InvokeAsync(options.ContractHash, options.Method, args);
                    Console.WriteLine("✅ Method invoked successfully!");
                    Console.WriteLine($"   Transaction: {txHash}");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Invocation failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                return 1;
            }
        }

        static async Task<int> RunInfo(InfoOptions options)
        {
            try
            {
                Console.WriteLine("ℹ️  Getting contract info for {0} on {1}...", options.ContractHash, options.Network);

                using var toolkit = new DeploymentToolkit(options.ConfigFile)
                    .SetNetwork(options.Network);

                // Check if contract exists
                var exists = await toolkit.ContractExistsAsync(options.ContractHash);
                
                if (!exists)
                {
                    Console.WriteLine("❌ Contract not found at address: {0}", options.ContractHash);
                    return 1;
                }

                Console.WriteLine("✅ Contract found!");
                Console.WriteLine($"   Contract Hash: {options.ContractHash}");

                // Get balance if requested
                if (options.ShowBalance)
                {
                    if (!string.IsNullOrEmpty(options.WifKey) || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NEO_WIF_KEY")))
                    {
                        toolkit.SetWifKey(options.WifKey ?? Environment.GetEnvironmentVariable("NEO_WIF_KEY")!);
                        var account = await toolkit.GetDeployerAccountAsync();
                        var balance = await toolkit.GetGasBalanceAsync(account.ToAddress(Neo.ProtocolSettings.Default.AddressVersion));
                        Console.WriteLine($"   Account Balance: {balance:F8} GAS");
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to get info: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                return 1;
            }
        }

        static object[] ParseArguments(string[]? arguments)
        {
            if (arguments == null || arguments.Length == 0)
                return Array.Empty<object>();

            var result = new object[arguments.Length];
            
            for (int i = 0; i < arguments.Length; i++)
            {
                var arg = arguments[i];
                
                // Try to parse as different types
                if (bool.TryParse(arg, out var boolValue))
                    result[i] = boolValue;
                else if (int.TryParse(arg, out var intValue))
                    result[i] = intValue;
                else if (long.TryParse(arg, out var longValue))
                    result[i] = longValue;
                else if (arg.StartsWith("0x"))
                    result[i] = arg; // Keep hex strings as-is
                else
                    result[i] = arg; // String value
            }

            return result;
        }
    }

    // Command line options
    [Verb("deploy", HelpText = "Deploy a new smart contract")]
    class DeployOptions
    {
        [Option('n', "network", Required = true, HelpText = "Network to deploy to (mainnet, testnet, local, or RPC URL)")]
        public string Network { get; set; } = "";

        [Option('w', "wif", Required = false, HelpText = "WIF private key (can also use NEO_WIF_KEY environment variable)")]
        public string? WifKey { get; set; }

        [Option('c', "contract", Required = false, HelpText = "Path to contract project or source file")]
        public string? ContractPath { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file for deployment info")]
        public string? OutputFile { get; set; }

        [Option("config", Required = false, HelpText = "Configuration file path")]
        public string? ConfigFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }
    }

    [Verb("update", HelpText = "Update an existing smart contract")]
    class UpdateOptions
    {
        [Option('h', "hash", Required = true, HelpText = "Contract hash or address to update")]
        public string ContractHash { get; set; } = "";

        [Option('n', "network", Required = true, HelpText = "Network (mainnet, testnet, local, or RPC URL)")]
        public string Network { get; set; } = "";

        [Option('w', "wif", Required = false, HelpText = "WIF private key (can also use NEO_WIF_KEY environment variable)")]
        public string? WifKey { get; set; }

        [Option('c', "contract", Required = false, HelpText = "Path to contract project or source file")]
        public string? ContractPath { get; set; }

        [Option("config", Required = false, HelpText = "Configuration file path")]
        public string? ConfigFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }
    }

    [Verb("invoke", HelpText = "Invoke a smart contract method")]
    class InvokeOptions
    {
        [Option('h', "hash", Required = true, HelpText = "Contract hash or address")]
        public string ContractHash { get; set; } = "";

        [Option('m', "method", Required = true, HelpText = "Method name to invoke")]
        public string Method { get; set; } = "";

        [Option('n', "network", Required = true, HelpText = "Network (mainnet, testnet, local, or RPC URL)")]
        public string Network { get; set; } = "";

        [Option('w', "wif", Required = false, HelpText = "WIF private key (required for state-changing methods)")]
        public string? WifKey { get; set; }

        [Option('a', "args", Required = false, HelpText = "Method arguments")]
        public string[]? Arguments { get; set; }

        [Option('r', "readonly", Required = false, HelpText = "Call method as read-only (no transaction)")]
        public bool ReadOnly { get; set; }

        [Option("config", Required = false, HelpText = "Configuration file path")]
        public string? ConfigFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }
    }

    [Verb("info", HelpText = "Get information about a deployed contract")]
    class InfoOptions
    {
        [Option('h', "hash", Required = true, HelpText = "Contract hash or address")]
        public string ContractHash { get; set; } = "";

        [Option('n', "network", Required = true, HelpText = "Network (mainnet, testnet, local, or RPC URL)")]
        public string Network { get; set; } = "";

        [Option('b', "balance", Required = false, HelpText = "Show account balance")]
        public bool ShowBalance { get; set; }

        [Option('w', "wif", Required = false, HelpText = "WIF private key (required for balance check)")]
        public string? WifKey { get; set; }

        [Option("config", Required = false, HelpText = "Configuration file path")]
        public string? ConfigFile { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }
    }
}