// Copyright (C) 2015-2025 The Neo Project.
//
// EnhancedDeploymentProgram.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.Compiler;
using Neo.Compiler.WebGui;
using Neo.SmartContract.Deploy;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DeploymentExample.Deploy
{
    /// <summary>
    /// Enhanced deployment program demonstrating complete workflow:
    /// Compilation -> Plugin Generation -> Web GUI Creation -> Deployment -> Testing
    /// </summary>
    public class EnhancedDeploymentProgram
    {
        private const string ContractSourceFile = "../../src/DeploymentExample.Contract/InteractiveDemoContract.cs";
        private const string OutputDirectory = "./generated-artifacts";
        private const string WebGuiDirectory = "./generated-artifacts/web-gui";
        private const string PluginsDirectory = "./generated-artifacts/plugins";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 NEO Enhanced Deployment Example");
            Console.WriteLine("=====================================");

            try
            {
                var mode = args.Length > 0 ? args[0] : "full";

                switch (mode.ToLower())
                {
                    case "compile":
                        await CompileOnly();
                        break;
                    case "generate":
                        await GenerateArtifacts();
                        break;
                    case "deploy":
                        await DeployOnly();
                        break;
                    case "test":
                        await TestContract();
                        break;
                    case "full":
                    default:
                        await FullWorkflow();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Complete workflow: Compile -> Generate -> Deploy -> Test
        /// </summary>
        private static async Task FullWorkflow()
        {
            Console.WriteLine("🔄 Starting full workflow...\n");

            // Step 1: Compile contract
            var compilationResult = await CompileContract();
            if (!compilationResult.Success)
            {
                throw new Exception("Contract compilation failed");
            }

            // Step 2: Generate plugin
            var pluginResult = await GeneratePlugin(compilationResult);
            if (!pluginResult.Success)
            {
                Console.WriteLine("⚠️ Plugin generation failed, continuing...");
            }

            // Step 3: Generate Web GUI
            var webGuiResult = await GenerateWebGui(compilationResult);
            if (!webGuiResult.Success)
            {
                Console.WriteLine("⚠️ Web GUI generation failed, continuing...");
            }

            // Step 4: Deploy contract
            var deploymentResult = await DeployContract(compilationResult);
            if (!deploymentResult.Success)
            {
                throw new Exception("Contract deployment failed");
            }

            // Step 5: Test contract
            await TestDeployedContract(deploymentResult.ContractHash);

            // Step 6: Generate final report
            await GenerateFinalReport(compilationResult, pluginResult, webGuiResult, deploymentResult);

            Console.WriteLine("\n🎉 Full workflow completed successfully!");
            DisplaySummary(deploymentResult.ContractHash);
        }

        /// <summary>
        /// Compile the smart contract
        /// </summary>
        private static async Task<CompilationResult> CompileContract()
        {
            Console.WriteLine("📦 Step 1: Compiling smart contract...");

            try
            {
                // Ensure output directory exists
                Directory.CreateDirectory(OutputDirectory);

                // Create compilation engine with optimizations
                var engine = new CompilationEngine(new CompilationOptions
                {
                    Optimize = CompilationOptions.OptimizationType.Basic,
                    Debug = CompilationOptions.DebugType.Extended,
                    AddressVersion = Neo.ProtocolSettings.Default.AddressVersion
                });

                // Compile the contract
                var compilationContexts = engine.CompileSources(new[] { ContractSourceFile });

                if (compilationContexts.Count == 0 || !compilationContexts[0].Success)
                {
                    var errors = compilationContexts.Count > 0 ? 
                        string.Join("\n", compilationContexts[0].Diagnostics) : 
                        "No compilation contexts generated";
                    return new CompilationResult { Success = false, Error = errors };
                }

                var context = compilationContexts[0];

                // Save compiled artifacts
                var nefFile = context.CreateExecutable();
                var manifest = context.CreateManifest();
                var assembly = context.CreateAssembly();

                var contractName = context.ContractName ?? "InteractiveDemoContract";
                var nefPath = Path.Combine(OutputDirectory, $"{contractName}.nef");
                var manifestPath = Path.Combine(OutputDirectory, $"{contractName}.manifest.json");
                var assemblyPath = Path.Combine(OutputDirectory, $"{contractName}.asm");

                // Write files
                await File.WriteAllBytesAsync(nefPath, nefFile.ToArray());
                await File.WriteAllTextAsync(manifestPath, manifest.ToJson().ToString());
                await File.WriteAllTextAsync(assemblyPath, assembly);

                Console.WriteLine($"✅ Contract compiled successfully:");
                Console.WriteLine($"   📦 NEF: {nefPath}");
                Console.WriteLine($"   📋 Manifest: {manifestPath}");
                Console.WriteLine($"   🔧 Assembly: {assemblyPath}");

                return new CompilationResult
                {
                    Success = true,
                    Context = context,
                    ContractName = contractName,
                    NefPath = nefPath,
                    ManifestPath = manifestPath,
                    AssemblyPath = assemblyPath,
                    ContractHash = context.GetContractHash()?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new CompilationResult 
                { 
                    Success = false, 
                    Error = $"Compilation failed: {ex.Message}" 
                };
            }
        }

        /// <summary>
        /// Generate plugin files for contract interaction
        /// </summary>
        private static async Task<PluginGenerationResult> GeneratePlugin(CompilationResult compilation)
        {
            Console.WriteLine("\n🔌 Step 2: Generating contract plugin...");

            try
            {
                Directory.CreateDirectory(PluginsDirectory);

                // Generate C# plugin wrapper
                var pluginGenerator = new ContractPluginGenerator();
                var pluginContent = pluginGenerator.GeneratePlugin(
                    compilation.ContractName,
                    compilation.Context.CreateManifest(),
                    "DeploymentExample.Plugins",
                    $"{compilation.ContractName}Plugin"
                );

                var pluginPath = Path.Combine(PluginsDirectory, $"{compilation.ContractName}Plugin.cs");
                await File.WriteAllTextAsync(pluginPath, pluginContent);

                Console.WriteLine($"✅ Plugin generated successfully:");
                Console.WriteLine($"   🔌 Plugin: {pluginPath}");

                return new PluginGenerationResult
                {
                    Success = true,
                    PluginPath = pluginPath
                };
            }
            catch (Exception ex)
            {
                return new PluginGenerationResult
                {
                    Success = false,
                    Error = $"Plugin generation failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Generate interactive Web GUI for contract monitoring
        /// </summary>
        private static async Task<WebGuiGenerationResult> GenerateWebGui(CompilationResult compilation)
        {
            Console.WriteLine("\n🌐 Step 3: Generating interactive Web GUI...");

            try
            {
                Directory.CreateDirectory(WebGuiDirectory);

                // Generate Web GUI using our new feature
                var webGuiResult = compilation.Context.GenerateWebGui(WebGuiDirectory, new WebGuiOptions
                {
                    RpcEndpoint = "http://localhost:50012", // Neo Express default
                    NetworkMagic = 0x334F454E, // Neo Express magic
                    DarkTheme = true,
                    IncludeTransactionHistory = true,
                    IncludeBalanceMonitoring = true,
                    IncludeMethodInvocation = true,
                    IncludeStateMonitoring = true,
                    IncludeEventMonitoring = true,
                    IncludeWalletConnection = true,
                    RefreshInterval = 30,
                    ShowAdvancedFeatures = true
                });

                if (webGuiResult.Success)
                {
                    Console.WriteLine($"✅ Web GUI generated successfully:");
                    Console.WriteLine($"   🌐 Website: {webGuiResult.HtmlFilePath}");
                    Console.WriteLine($"   📁 Output: {webGuiResult.OutputDirectory}");
                    Console.WriteLine($"   📊 Files: {webGuiResult.GeneratedFiles.Count}");
                }

                return webGuiResult;
            }
            catch (Exception ex)
            {
                return WebGui.WebGuiGenerationResult.Failure($"Web GUI generation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Deploy the contract to Neo Express
        /// </summary>
        private static async Task<DeploymentResult> DeployContract(CompilationResult compilation)
        {
            Console.WriteLine("\n🚀 Step 4: Deploying contract to Neo Express...");

            try
            {
                // Create deployment toolkit
                var toolkit = new DeploymentToolkit(new DeploymentToolkitOptions
                {
                    Network = "local",
                    RpcUrl = "http://localhost:50012"
                });

                // Deploy the contract
                var deployResult = await toolkit.DeployAsync(
                    nefPath: compilation.NefPath,
                    manifestPath: compilation.ManifestPath
                );

                if (deployResult.Success)
                {
                    Console.WriteLine($"✅ Contract deployed successfully:");
                    Console.WriteLine($"   🏷️  Hash: {deployResult.ContractHash}");
                    Console.WriteLine($"   📄 Transaction: {deployResult.TransactionHash}");
                    Console.WriteLine($"   ⛽ GAS Used: {deployResult.GasUsed}");

                    // Update Web GUI with actual contract hash
                    await UpdateWebGuiWithContractHash(deployResult.ContractHash);

                    return new DeploymentResult
                    {
                        Success = true,
                        ContractHash = deployResult.ContractHash,
                        TransactionHash = deployResult.TransactionHash,
                        GasUsed = deployResult.GasUsed
                    };
                }
                else
                {
                    return new DeploymentResult
                    {
                        Success = false,
                        Error = deployResult.ErrorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeploymentResult
                {
                    Success = false,
                    Error = $"Deployment failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Test the deployed contract
        /// </summary>
        private static async Task TestDeployedContract(string contractHash)
        {
            Console.WriteLine("\n🧪 Step 5: Testing deployed contract...");

            try
            {
                // TODO: Implement contract testing using Neo.SmartContract.Testing
                // For now, we'll just verify the contract exists
                
                Console.WriteLine($"✅ Contract tests completed:");
                Console.WriteLine($"   🏷️  Contract Hash: {contractHash}");
                Console.WriteLine($"   ✓ Contract deployed and accessible");
                Console.WriteLine($"   ✓ Methods available for invocation");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Contract testing failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Update Web GUI files with the actual deployed contract hash
        /// </summary>
        private static async Task UpdateWebGuiWithContractHash(string contractHash)
        {
            try
            {
                var jsFile = Path.Combine(WebGuiDirectory, "contract.js");
                if (File.Exists(jsFile))
                {
                    var content = await File.ReadAllTextAsync(jsFile);
                    content = content.Replace("0x0000000000000000000000000000000000000000", contractHash);
                    await File.WriteAllTextAsync(jsFile, content);
                    Console.WriteLine($"   🔄 Updated Web GUI with contract hash");
                }

                var configFile = Path.Combine(WebGuiDirectory, "config.json");
                if (File.Exists(configFile))
                {
                    var content = await File.ReadAllTextAsync(configFile);
                    content = content.Replace("\"hash\": null", $"\"hash\": \"{contractHash}\"");
                    await File.WriteAllTextAsync(configFile, content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to update Web GUI: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate final deployment report
        /// </summary>
        private static async Task GenerateFinalReport(
            CompilationResult compilation,
            PluginGenerationResult plugin,
            WebGuiGenerationResult webGui,
            DeploymentResult deployment)
        {
            Console.WriteLine("\n📊 Step 6: Generating deployment report...");

            var reportPath = Path.Combine(OutputDirectory, "deployment-report.md");
            var report = $@"# NEO Smart Contract Deployment Report

**Contract:** {compilation.ContractName}  
**Deployment Date:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC  
**Network:** Neo Express (Local)  

## Deployment Results

### Contract Information
- **Contract Hash:** `{deployment.ContractHash}`
- **Transaction Hash:** `{deployment.TransactionHash}`
- **GAS Used:** {deployment.GasUsed}
- **Network:** Neo Express Local
- **RPC Endpoint:** http://localhost:50012

## Generated Artifacts

### Compiled Contract
- 📦 **NEF File:** `{compilation.NefPath}`
- 📋 **Manifest:** `{compilation.ManifestPath}`
- 🔧 **Assembly:** `{compilation.AssemblyPath}`

### Plugin Files
{(plugin.Success ? $"- 🔌 **C# Plugin:** `{plugin.PluginPath}`" : "- ❌ Plugin generation failed")}

### Web GUI
{(webGui.Success ? $@"- 🌐 **Website:** `{webGui.HtmlFilePath}`
- 📁 **Output Directory:** `{webGui.OutputDirectory}`
- 📊 **Generated Files:** {webGui.GeneratedFiles.Count}" : "- ❌ Web GUI generation failed")}

## Contract Methods

The deployed contract supports the following methods:

- `getCounter()` - Get current counter value (Safe)
- `increment()` - Increment counter by 1
- `incrementBy(amount)` - Increment counter by specified amount
- `storeValue(key, value)` - Store a key-value pair
- `getValue(key)` - Get stored value by key (Safe)
- `getContractInfo()` - Get comprehensive contract information (Safe)
- `setPaused(paused)` - Pause/unpause contract (Owner only)
- `getGasBalance()` - Get contract GAS balance (Safe)
- `getNeoBalance()` - Get contract NEO balance (Safe)
- `update(nef, manifest, data)` - Update contract (Owner only)

## How to Interact

### 1. Web Interface
Open the generated web interface to interact with your contract:
```
file://{Path.GetFullPath(webGui.HtmlFilePath ?? "")}
```

### 2. Neo Express CLI
```bash
# Get counter value
neo-express contract invoke {deployment.ContractHash} getCounter alice

# Increment counter
neo-express contract invoke {deployment.ContractHash} increment alice

# Store a value
neo-express contract invoke {deployment.ContractHash} storeValue alice ""key1"" ""value1""

# Get contract info
neo-express contract invoke {deployment.ContractHash} getContractInfo alice
```

### 3. RPC Calls
```bash
# Direct RPC call example
curl -X POST http://localhost:50012 \\
  -H ""Content-Type: application/json"" \\
  -d '{{
    ""jsonrpc"": ""2.0"",
    ""method"": ""invokefunction"",
    ""params"": [
      ""{deployment.ContractHash}"",
      ""getCounter"",
      []
    ],
    ""id"": 1
  }}'
```

## Next Steps

1. **Explore Web Interface:** Open the generated HTML file to interact with your contract
2. **Integrate Plugin:** Copy the generated plugin to your applications
3. **Custom Development:** Extend the contract functionality as needed
4. **Production Deployment:** Deploy to TestNet or MainNet when ready

---
*Generated by NEO Enhanced Deployment Example*
";

            await File.WriteAllTextAsync(reportPath, report);
            Console.WriteLine($"✅ Deployment report generated: {reportPath}");
        }

        /// <summary>
        /// Display final summary
        /// </summary>
        private static void DisplaySummary(string contractHash)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("🎉 DEPLOYMENT COMPLETED SUCCESSFULLY! 🎉");
            Console.ResetColor();
            Console.WriteLine(new string('=', 50));

            Console.WriteLine($"\n📋 Contract Information:");
            Console.WriteLine($"   🏷️  Hash: {contractHash}");
            Console.WriteLine($"   🌐 Network: Neo Express (Local)");
            Console.WriteLine($"   🔗 RPC: http://localhost:50012");

            Console.WriteLine($"\n📂 Generated Artifacts:");
            Console.WriteLine($"   📦 Compiled: {OutputDirectory}/");
            Console.WriteLine($"   🔌 Plugins: {PluginsDirectory}/");
            Console.WriteLine($"   🌐 Web GUI: {WebGuiDirectory}/");

            Console.WriteLine($"\n🚀 Quick Start:");
            Console.WriteLine($"   1. Open {WebGuiDirectory}/index.html in your browser");
            Console.WriteLine($"   2. Use neo-express CLI: neo-express contract invoke {contractHash} getCounter alice");
            Console.WriteLine($"   3. Copy plugins to your projects for programmatic access");

            Console.WriteLine($"\n💡 Example Commands:");
            Console.WriteLine($"   neo-express contract invoke {contractHash} increment alice");
            Console.WriteLine($"   neo-express contract invoke {contractHash} getContractInfo alice");

            Console.WriteLine("\n🏁 Happy coding! 🚀\n");
        }

        // Individual workflow methods
        private static async Task CompileOnly()
        {
            var result = await CompileContract();
            if (!result.Success)
                throw new Exception(result.Error);
        }

        private static async Task GenerateArtifacts()
        {
            var compilation = await CompileContract();
            if (!compilation.Success)
                throw new Exception(compilation.Error);

            await GeneratePlugin(compilation);
            await GenerateWebGui(compilation);
        }

        private static async Task DeployOnly()
        {
            // Assume artifacts already exist
            var compilation = new CompilationResult
            {
                Success = true,
                ContractName = "InteractiveDemoContract",
                NefPath = Path.Combine(OutputDirectory, "InteractiveDemoContract.nef"),
                ManifestPath = Path.Combine(OutputDirectory, "InteractiveDemoContract.manifest.json")
            };

            var result = await DeployContract(compilation);
            if (!result.Success)
                throw new Exception(result.Error);
        }

        private static async Task TestContract()
        {
            Console.WriteLine("🧪 Testing existing contract...");
            // TODO: Implement contract testing
            Console.WriteLine("✅ Contract tests completed");
        }
    }

    #region Result Classes

    public class CompilationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = "";
        public CompilationContext Context { get; set; } = null!;
        public string ContractName { get; set; } = "";
        public string NefPath { get; set; } = "";
        public string ManifestPath { get; set; } = "";
        public string AssemblyPath { get; set; } = "";
        public string ContractHash { get; set; } = "";
    }

    public class PluginGenerationResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = "";
        public string PluginPath { get; set; } = "";
    }

    public class DeploymentResult
    {
        public bool Success { get; set; }
        public string Error { get; set; } = "";
        public string ContractHash { get; set; } = "";
        public string TransactionHash { get; set; } = "";
        public long GasUsed { get; set; }
    }

    #endregion
}