using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Neo.SmartContract.Deploy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeploymentExample.Deploy
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Neo Smart Contract Deployment Example");
            Console.WriteLine("=====================================\n");

            return await Parser.Default.ParseArguments<DeployOptions, DeployManifestOptions, CallOptions, VerifyOptions>(args)
                .MapResult(
                    async (DeployOptions opts) => await RunDeploy(opts),
                    async (DeployManifestOptions opts) => await RunDeployManifest(opts),
                    async (CallOptions opts) => await RunCall(opts),
                    async (VerifyOptions opts) => await RunVerify(opts),
                    errs => Task.FromResult(1));
        }

        static async Task<int> RunDeploy(DeployOptions options)
        {
            try
            {
                using var toolkit = CreateToolkit(options.ConfigFile, options.Network, options.WifKey);

                Console.WriteLine($"🚀 Deploying contract from: {options.ContractPath}");
                Console.WriteLine($"   Network: {options.Network}");

                var result = await toolkit.DeployAsync(options.ContractPath, ParseInitParams(options.InitParams));

                Console.WriteLine("\n✅ Contract deployed successfully!");
                Console.WriteLine($"   Contract Hash: {result.ContractHash}");
                Console.WriteLine($"   Contract Address: {result.ContractAddress}");
                Console.WriteLine($"   Transaction: {result.TransactionHash}");
                Console.WriteLine($"   Gas Consumed: {result.GasConsumed / 100000000m:F8} GAS");

                if (!string.IsNullOrEmpty(options.OutputFile))
                {
                    var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                    await File.WriteAllTextAsync(options.OutputFile, json);
                    Console.WriteLine($"\n💾 Deployment info saved to: {options.OutputFile}");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Deployment failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
                }
                return 1;
            }
        }

        static async Task<int> RunDeployManifest(DeployManifestOptions options)
        {
            try
            {
                using var toolkit = CreateToolkit(options.ConfigFile, options.Network, options.WifKey);

                Console.WriteLine($"📋 Deploying from manifest: {options.ManifestFile}");
                Console.WriteLine($"   Network: {options.Network}");

                var result = await toolkit.DeployFromManifestAsync(options.ManifestFile);

                Console.WriteLine("\n✅ All contracts deployed successfully!");
                Console.WriteLine($"   Total contracts: {result.DeployedContracts.Count}");
                Console.WriteLine($"   Total gas consumed: {result.TotalGasConsumed / 100000000m:F8} GAS");
                Console.WriteLine($"   Duration: {result.DeploymentDuration.TotalSeconds:F2} seconds");

                foreach (var contract in result.DeployedContracts)
                {
                    Console.WriteLine($"\n   📄 {contract.Key}:");
                    Console.WriteLine($"      Hash: {contract.Value.ContractHash}");
                    Console.WriteLine($"      Address: {contract.Value.ContractAddress}");
                }

                if (!string.IsNullOrEmpty(options.OutputFile))
                {
                    var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                    await File.WriteAllTextAsync(options.OutputFile, json);
                    Console.WriteLine($"\n💾 Deployment results saved to: {options.OutputFile}");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Deployment failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
                }
                return 1;
            }
        }

        static async Task<int> RunCall(CallOptions options)
        {
            try
            {
                using var toolkit = CreateToolkit(options.ConfigFile, options.Network, null);

                Console.WriteLine($"📞 Calling {options.Method} on contract {options.ContractHash}");
                Console.WriteLine($"   Network: {options.Network}");
                
                if (options.Arguments?.Any() == true)
                {
                    Console.WriteLine($"   Arguments: {string.Join(", ", options.Arguments)}");
                }

                var args = ParseArguments(options.Arguments);
                var result = await toolkit.CallAsync<object>(options.ContractHash, options.Method, args);

                Console.WriteLine("\n✅ Call successful!");
                Console.WriteLine($"   Result: {FormatResult(result)}");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Call failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
                }
                return 1;
            }
        }

        static async Task<int> RunVerify(VerifyOptions options)
        {
            try
            {
                using var toolkit = CreateToolkit(options.ConfigFile, options.Network, null);

                Console.WriteLine($"🔍 Verifying contracts on {options.Network}...");

                var manifestPath = options.ManifestFile ?? "../../deployment-manifest.json";
                var manifestJson = await File.ReadAllTextAsync(manifestPath);
                var manifest = JsonConvert.DeserializeObject<JObject>(manifestJson);

                var contracts = manifest["contracts"] as JArray;
                if (contracts == null)
                {
                    throw new Exception("No contracts found in manifest");
                }

                var allVerified = true;

                foreach (var contract in contracts)
                {
                    var name = contract["name"]?.ToString();
                    var hash = options.ContractHash ?? contract["deployedHash"]?.ToString();

                    if (string.IsNullOrEmpty(hash))
                    {
                        Console.WriteLine($"\n⚠️  {name}: No deployed hash found");
                        continue;
                    }

                    var exists = await toolkit.ContractExistsAsync(hash);
                    
                    if (exists)
                    {
                        Console.WriteLine($"\n✅ {name}: Verified at {hash}");

                        // Perform additional verification if specified
                        var postActions = contract["postDeploymentActions"] as JArray;
                        if (postActions != null && options.RunTests)
                        {
                            foreach (var action in postActions)
                            {
                                var method = action["method"]?.ToString();
                                var paramsArray = action["params"] as JArray;
                                var expectedResult = action["expectedResult"];

                                if (!string.IsNullOrEmpty(method))
                                {
                                    try
                                    {
                                        var args = paramsArray?.Select(p => p.ToObject<object>()).ToArray() ?? Array.Empty<object>();
                                        var result = await toolkit.CallAsync<object>(hash, method, args);

                                        if (expectedResult != null && !JToken.DeepEquals(JToken.FromObject(result), expectedResult))
                                        {
                                            Console.WriteLine($"   ⚠️  {method}: Result mismatch");
                                            allVerified = false;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"   ✓ {method}: OK");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"   ❌ {method}: {ex.Message}");
                                        allVerified = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\n❌ {name}: Not found at {hash}");
                        allVerified = false;
                    }
                }

                if (allVerified)
                {
                    Console.WriteLine("\n✅ All contracts verified successfully!");
                    return 0;
                }
                else
                {
                    Console.WriteLine("\n⚠️  Some contracts could not be verified");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Verification failed: {ex.Message}");
                if (options.Verbose)
                {
                    Console.WriteLine($"\nStack trace:\n{ex.StackTrace}");
                }
                return 1;
            }
        }

        static DeploymentToolkit CreateToolkit(string? configFile, string network, string? wifKey)
        {
            var toolkit = new DeploymentToolkit(configFile).SetNetwork(network);
            
            if (!string.IsNullOrEmpty(wifKey))
            {
                toolkit.SetWifKey(wifKey);
            }
            else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NEO_WIF_KEY")))
            {
                toolkit.SetWifKey(Environment.GetEnvironmentVariable("NEO_WIF_KEY")!);
            }

            return toolkit;
        }

        static object[] ParseInitParams(string[]? initParams)
        {
            if (initParams == null || initParams.Length == 0)
                return Array.Empty<object>();

            return ParseArguments(initParams);
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
                else if (decimal.TryParse(arg, out var decimalValue))
                    result[i] = decimalValue;
                else if (arg.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    result[i] = arg; // Keep hex strings as-is
                else if (arg.StartsWith("[") && arg.EndsWith("]"))
                {
                    // Parse as JSON array
                    try
                    {
                        result[i] = JsonConvert.DeserializeObject<object[]>(arg) ?? arg;
                    }
                    catch
                    {
                        result[i] = arg;
                    }
                }
                else if (arg.StartsWith("{") && arg.EndsWith("}"))
                {
                    // Parse as JSON object
                    try
                    {
                        result[i] = JsonConvert.DeserializeObject<object>(arg) ?? arg;
                    }
                    catch
                    {
                        result[i] = arg;
                    }
                }
                else
                    result[i] = arg; // String value
            }

            return result;
        }

        static string FormatResult(object result)
        {
            if (result == null)
                return "null";

            if (result is byte[] bytes)
                return $"0x{BitConverter.ToString(bytes).Replace("-", "")}";

            if (result is Array array)
                return $"[{string.Join(", ", array.Cast<object>().Select(FormatResult))}]";

            return JsonConvert.SerializeObject(result, Formatting.None);
        }
    }

    // Command line options
    [Verb("deploy", HelpText = "Deploy a single contract")]
    class DeployOptions : BaseOptions
    {
        [Option('c', "contract", Required = true, HelpText = "Path to contract project or source file")]
        public string ContractPath { get; set; } = "";

        [Option('i', "init-params", Required = false, HelpText = "Initialization parameters")]
        public string[]? InitParams { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file for deployment info")]
        public string? OutputFile { get; set; }
    }

    [Verb("deploy-manifest", HelpText = "Deploy multiple contracts from manifest")]
    class DeployManifestOptions : BaseOptions
    {
        [Option('m', "manifest", Required = true, HelpText = "Path to deployment manifest file")]
        public string ManifestFile { get; set; } = "";

        [Option('o', "output", Required = false, HelpText = "Output file for deployment results")]
        public string? OutputFile { get; set; }
    }

    [Verb("call", HelpText = "Call a contract method")]
    class CallOptions : BaseOptions
    {
        [Option('h', "hash", Required = true, HelpText = "Contract hash or address")]
        public string ContractHash { get; set; } = "";

        [Option('m', "method", Required = true, HelpText = "Method name")]
        public string Method { get; set; } = "";

        [Option('a', "args", Required = false, HelpText = "Method arguments")]
        public string[]? Arguments { get; set; }
    }

    [Verb("verify", HelpText = "Verify deployed contracts")]
    class VerifyOptions : BaseOptions
    {
        [Option('m', "manifest", Required = false, HelpText = "Path to deployment manifest file")]
        public string? ManifestFile { get; set; }

        [Option('h', "hash", Required = false, HelpText = "Specific contract hash to verify")]
        public string? ContractHash { get; set; }

        [Option('t', "run-tests", Required = false, Default = false, HelpText = "Run post-deployment tests")]
        public bool RunTests { get; set; }
    }

    abstract class BaseOptions
    {
        [Option('n', "network", Required = true, HelpText = "Network (mainnet, testnet, local, or RPC URL)")]
        public string Network { get; set; } = "";

        [Option('w', "wif", Required = false, HelpText = "WIF private key (can also use NEO_WIF_KEY env var)")]
        public string? WifKey { get; set; }

        [Option("config", Required = false, HelpText = "Configuration file path")]
        public string? ConfigFile { get; set; }

        [Option('v', "verbose", Required = false, Default = false, HelpText = "Enable verbose output")]
        public bool Verbose { get; set; }
    }
}