using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Integration;

/// <summary>
/// Integration tests for multi-contract deployment scenarios
/// </summary>
public class MultiContractDeploymentTests : TestBase
{
    [Fact]
    public async Task DeployMultipleIndependentContracts_ShouldSucceed()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        var contractPaths = new List<string>();
        var deploymentResults = new List<ContractDeploymentInfo>();

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "123456");

            // Create multiple test contract projects
            contractPaths.Add(CreateTestContractProject("TokenContract", GenerateTokenContractCode("MyToken", 1000000)));
            contractPaths.Add(CreateTestContractProject("NFTContract", GenerateTokenContractCode("MyNFT", 10000)));
            contractPaths.Add(CreateTestContractProject("DexContract", GenerateTokenContractCode("MyDex", 500000)));

            // Deploy all contracts
            foreach (var (contractPath, index) in contractPaths.Select((path, i) => (path, i)))
            {
                var contractName = Path.GetFileNameWithoutExtension(contractPath);
                var compilationOptions = new CompilationOptions
                {
                    ProjectPath = contractPath,
                    OutputDirectory = Path.Combine(outputDir, contractName),
                    ContractName = contractName,
                    GenerateDebugInfo = true
                };

                var deploymentOptions = new DeploymentOptions
                {
                    DeployerAccount = toolkit.GetDeployerAccount(),
                    GasLimit = 50_000_000,
                    WaitForConfirmation = false
                };

                var result = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
                Assert.True(result.Success, $"Failed to deploy {contractName}");
                Assert.NotEqual(UInt160.Zero, result.ContractHash);
                
                deploymentResults.Add(result);
            }

            // Verify all contracts are deployed
            Assert.Equal(3, deploymentResults.Count);
            Assert.All(deploymentResults, result => Assert.True(result.Success));
            
            // Verify each contract has unique hash
            var uniqueHashes = deploymentResults.Select(r => r.ContractHash).Distinct().Count();
            Assert.Equal(3, uniqueHashes);

            // Test invoking each contract
            foreach (var (result, index) in deploymentResults.Select((r, i) => (r, i)))
            {
                var name = await toolkit.CallContractAsync<string>(result.ContractHash, "getName");
                Assert.NotNull(name);
                Assert.Contains("My", name); // All contracts have "My" prefix
            }
        }
        finally
        {
            // Cleanup
            foreach (var path in contractPaths)
            {
                var projectDir = new DirectoryInfo(Path.GetDirectoryName(path)!).Parent!.FullName;
                if (Directory.Exists(projectDir))
                    Directory.Delete(projectDir, true);
            }
            Directory.Delete(outputDir, true);
        }
    }

    [Fact]
    public async Task DeployContractsWithDependencies_ShouldDeployInOrder()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        var contractPaths = new Dictionary<string, string>();

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "123456");

            // Step 1: Deploy Registry contract first (no dependencies)
            var registryPath = CreateRegistryContractProject();
            contractPaths["Registry"] = registryPath;

            var registryCompilationOptions = new CompilationOptions
            {
                ProjectPath = registryPath,
                OutputDirectory = Path.Combine(outputDir, "Registry"),
                ContractName = "Registry"
            };

            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = toolkit.GetDeployerAccount(),
                GasLimit = 50_000_000,
                WaitForConfirmation = false
            };

            var registryResult = await toolkit.CompileAndDeployAsync(registryCompilationOptions, deploymentOptions);
            Assert.True(registryResult.Success, "Failed to deploy Registry contract");

            // Step 2: Deploy Token contract with Registry dependency
            var tokenPath = CreateTokenWithRegistryContractProject(registryResult.ContractHash);
            contractPaths["Token"] = tokenPath;

            var tokenCompilationOptions = new CompilationOptions
            {
                ProjectPath = tokenPath,
                OutputDirectory = Path.Combine(outputDir, "Token"),
                ContractName = "Token"
            };

            var tokenResult = await toolkit.CompileAndDeployAsync(tokenCompilationOptions, deploymentOptions);
            Assert.True(tokenResult.Success, "Failed to deploy Token contract");

            // Step 3: Deploy Exchange contract with both Registry and Token dependencies
            var exchangePath = CreateExchangeContractProject(registryResult.ContractHash, tokenResult.ContractHash);
            contractPaths["Exchange"] = exchangePath;

            var exchangeCompilationOptions = new CompilationOptions
            {
                ProjectPath = exchangePath,
                OutputDirectory = Path.Combine(outputDir, "Exchange"),
                ContractName = "Exchange"
            };

            var exchangeResult = await toolkit.CompileAndDeployAsync(exchangeCompilationOptions, deploymentOptions);
            Assert.True(exchangeResult.Success, "Failed to deploy Exchange contract");

            // Verify contracts can interact
            // Register token in registry
            var registerTxHash = await toolkit.InvokeContractAsync(
                registryResult.ContractHash,
                "registerContract",
                "MyToken",
                tokenResult.ContractHash);
            Assert.NotEqual(UInt256.Zero, registerTxHash);

            // Check if token is registered
            var isRegistered = await toolkit.CallContractAsync<bool>(
                registryResult.ContractHash,
                "isRegistered",
                tokenResult.ContractHash);
            Assert.True(isRegistered);

            // Initialize exchange with token
            var initTxHash = await toolkit.InvokeContractAsync(
                exchangeResult.ContractHash,
                "initialize",
                tokenResult.ContractHash);
            Assert.NotEqual(UInt256.Zero, initTxHash);
        }
        finally
        {
            // Cleanup
            foreach (var path in contractPaths.Values)
            {
                var projectDir = new DirectoryInfo(Path.GetDirectoryName(path)!).Parent!.FullName;
                if (Directory.Exists(projectDir))
                    Directory.Delete(projectDir, true);
            }
            Directory.Delete(outputDir, true);
        }
    }

    [Fact]
    public async Task DeployContractsFromManifest_ShouldSucceed()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        var manifestPath = CreateDeploymentManifest(outputDir);

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "123456");

            // Load and parse deployment manifest
            var manifest = await LoadDeploymentManifest(manifestPath);

            var deployedContracts = new Dictionary<string, ContractDeploymentInfo>();

            // Deploy contracts in order specified in manifest
            foreach (var contractDef in manifest.Contracts)
            {
                var contractPath = CreateContractProjectFromDefinition(contractDef, deployedContracts);

                var compilationOptions = new CompilationOptions
                {
                    ProjectPath = contractPath,
                    OutputDirectory = Path.Combine(outputDir, contractDef.Name),
                    ContractName = contractDef.Name,
                    GenerateDebugInfo = contractDef.EnableDebug
                };

                var deploymentOptions = new DeploymentOptions
                {
                    DeployerAccount = toolkit.GetDeployerAccount(),
                    GasLimit = contractDef.GasLimit,
                    WaitForConfirmation = false,
                    InitialParameters = contractDef.InitialParameters
                };

                var result = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
                Assert.True(result.Success, $"Failed to deploy {contractDef.Name}");

                deployedContracts[contractDef.Name] = result;

                // Cleanup temp contract project
                var projectDir = new DirectoryInfo(Path.GetDirectoryName(contractPath)!).Parent!.FullName;
                Directory.Delete(projectDir, true);
            }

            // Verify all contracts from manifest are deployed
            Assert.Equal(manifest.Contracts.Count, deployedContracts.Count);

            // Run post-deployment actions
            foreach (var action in manifest.PostDeploymentActions)
            {
                var contractHash = deployedContracts[action.Contract].ContractHash;
                var txHash = await toolkit.InvokeContractAsync(
                    contractHash,
                    action.Method,
                    action.Parameters.ToArray());
                Assert.NotEqual(UInt256.Zero, txHash);
            }
        }
        finally
        {
            // Cleanup
            Directory.Delete(outputDir, true);
        }
    }

    [Fact]
    public async Task BatchDeployWithRollback_ShouldHandleFailures()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        var successfulDeployments = new List<ContractDeploymentInfo>();
        var contractPaths = new List<string>();

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "123456");

            // Create contract projects, one with intentional compilation error
            contractPaths.Add(CreateTestContractProject("Contract1", GenerateTokenContractCode("First", 1000)));
            contractPaths.Add(CreateTestContractProject("Contract2", GenerateTokenContractCode("Second", 2000)));
            contractPaths.Add(CreateInvalidContractProject()); // This will fail compilation
            contractPaths.Add(CreateTestContractProject("Contract4", GenerateTokenContractCode("Fourth", 4000)));

            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = toolkit.GetDeployerAccount(),
                GasLimit = 50_000_000,
                WaitForConfirmation = false
            };

            // Attempt to deploy all contracts
            var deploymentErrors = new List<string>();
            
            foreach (var contractPath in contractPaths)
            {
                try
                {
                    var contractName = Path.GetFileNameWithoutExtension(contractPath);
                    var compilationOptions = new CompilationOptions
                    {
                        ProjectPath = contractPath,
                        OutputDirectory = Path.Combine(outputDir, contractName),
                        ContractName = contractName
                    };

                    var result = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
                    
                    if (result.Success)
                    {
                        successfulDeployments.Add(result);
                    }
                    else
                    {
                        deploymentErrors.Add($"{contractName}: {result.ErrorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    deploymentErrors.Add($"Contract deployment failed: {ex.Message}");
                }
            }

            // Verify partial deployment
            Assert.Equal(2, successfulDeployments.Count); // Only first 2 should succeed
            Assert.NotEmpty(deploymentErrors);
            Assert.Contains("compilation", deploymentErrors[0], StringComparison.OrdinalIgnoreCase);

            // Verify deployed contracts still work
            foreach (var deployment in successfulDeployments)
            {
                var exists = await toolkit.ContractExistsAsync(deployment.ContractHash);
                Assert.True(exists);
            }
        }
        finally
        {
            // Cleanup
            foreach (var path in contractPaths)
            {
                var projectDir = new DirectoryInfo(Path.GetDirectoryName(path)!).Parent!.FullName;
                if (Directory.Exists(projectDir))
                    Directory.Delete(projectDir, true);
            }
            Directory.Delete(outputDir, true);
        }
    }

    #region Helper Methods

    private string GenerateTokenContractCode(string name, int initialSupply)
    {
        return $@"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace TestContracts
{{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""{name} Contract"")]
    public class TokenContract : SmartContract
    {{
        private const string NameKey = ""name"";
        private const string SupplyKey = ""supply"";

        [DisplayName(""getName"")]
        public static string GetName()
        {{
            return Storage.Get(Storage.CurrentContext, NameKey);
        }}

        [DisplayName(""getSupply"")]
        public static BigInteger GetSupply()
        {{
            var supply = Storage.Get(Storage.CurrentContext, SupplyKey);
            return supply != null ? (BigInteger)supply : 0;
        }}

        public static void _deploy(object data, bool update)
        {{
            if (!update)
            {{
                Storage.Put(Storage.CurrentContext, NameKey, ""{name}"");
                Storage.Put(Storage.CurrentContext, SupplyKey, {initialSupply});
            }}
        }}
    }}
}}";
    }

    private string CreateRegistryContractProject()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo;
using System.ComponentModel;

namespace RegistryContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Contract Registry"")]
    public class Registry : SmartContract
    {
        private const byte PrefixContract = 0x01;
        private const byte PrefixName = 0x02;

        [DisplayName(""registerContract"")]
        public static bool RegisterContract(string name, UInt160 contractHash)
        {
            if (contractHash == UInt160.Zero || string.IsNullOrEmpty(name))
                return false;

            var key = new byte[] { PrefixContract }.Concat(contractHash);
            var nameKey = new byte[] { PrefixName }.Concat(name.ToByteArray());
            
            Storage.Put(Storage.CurrentContext, key, name);
            Storage.Put(Storage.CurrentContext, nameKey, contractHash);
            
            return true;
        }

        [DisplayName(""isRegistered"")]
        public static bool IsRegistered(UInt160 contractHash)
        {
            var key = new byte[] { PrefixContract }.Concat(contractHash);
            return Storage.Get(Storage.CurrentContext, key) != null;
        }

        [DisplayName(""getContractByName"")]
        public static UInt160 GetContractByName(string name)
        {
            var key = new byte[] { PrefixName }.Concat(name.ToByteArray());
            var result = Storage.Get(Storage.CurrentContext, key);
            return result != null ? (UInt160)result : UInt160.Zero;
        }
    }
}";

        return CreateTestContractProject("Registry", contractCode);
    }

    private string CreateTokenWithRegistryContractProject(UInt160 registryHash)
    {
        var contractCode = $@"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using Neo;
using System.ComponentModel;
using System.Numerics;

namespace TokenContract
{{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Token with Registry"")]
    public class Token : SmartContract
    {{
        private static readonly UInt160 RegistryContract = UInt160.Parse(""{registryHash}"");
        
        [DisplayName(""symbol"")]
        public static string Symbol() => ""TKN"";

        [DisplayName(""decimals"")]
        public static byte Decimals() => 8;

        [DisplayName(""totalSupply"")]
        public static BigInteger TotalSupply() => 1000000_00000000;

        public static void _deploy(object data, bool update)
        {{
            if (!update)
            {{
                // Register this token in the registry
                Contract.Call(RegistryContract, ""registerContract"", CallFlags.All, ""MyToken"", Runtime.ExecutingScriptHash);
            }}
        }}
    }}
}}";

        return CreateTestContractProject("Token", contractCode);
    }

    private string CreateExchangeContractProject(UInt160 registryHash, UInt160 tokenHash)
    {
        var contractCode = $@"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo;
using System.ComponentModel;

namespace ExchangeContract
{{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Exchange Contract"")]
    public class Exchange : SmartContract
    {{
        private static readonly UInt160 RegistryContract = UInt160.Parse(""{registryHash}"");
        private const string TokenKey = ""token"";
        private const string InitializedKey = ""initialized"";

        [DisplayName(""initialize"")]
        public static bool Initialize(UInt160 tokenContract)
        {{
            if (Storage.Get(Storage.CurrentContext, InitializedKey) != null)
                return false;

            // Verify token is registered
            var isRegistered = (bool)Contract.Call(RegistryContract, ""isRegistered"", CallFlags.ReadOnly, tokenContract);
            if (!isRegistered)
                return false;

            Storage.Put(Storage.CurrentContext, TokenKey, tokenContract);
            Storage.Put(Storage.CurrentContext, InitializedKey, 1);
            
            return true;
        }}

        [DisplayName(""getToken"")]
        public static UInt160 GetToken()
        {{
            var token = Storage.Get(Storage.CurrentContext, TokenKey);
            return token != null ? (UInt160)token : UInt160.Zero;
        }}
    }}
}}";

        return CreateTestContractProject("Exchange", contractCode);
    }

    private string CreateInvalidContractProject()
    {
        // Intentionally invalid contract for testing error handling
        var contractCode = @"
using Neo.SmartContract.Framework;

namespace InvalidContract
{
    public class Invalid : SmartContract
    {
        public static void InvalidMethod()
        {
            // Syntax error: missing semicolon
            var x = 10
            var y = 20;
            
            // Unknown type
            UnknownType z = new UnknownType();
        }
    }
}";

        return CreateTestContractProject("Invalid", contractCode);
    }

    private string CreateDeploymentManifest(string outputDir)
    {
        var manifest = @"{
  ""version"": ""1.0"",
  ""description"": ""Multi-contract deployment manifest"",
  ""contracts"": [
    {
      ""name"": ""Storage"",
      ""type"": ""storage"",
      ""gasLimit"": 30000000,
      ""enableDebug"": true,
      ""initialParameters"": []
    },
    {
      ""name"": ""Token"",
      ""type"": ""token"",
      ""gasLimit"": 50000000,
      ""enableDebug"": true,
      ""initialParameters"": [""MyToken"", ""TKN"", 8, 1000000],
      ""dependencies"": [""Storage""]
    },
    {
      ""name"": ""Governance"",
      ""type"": ""governance"",
      ""gasLimit"": 40000000,
      ""enableDebug"": false,
      ""initialParameters"": [],
      ""dependencies"": [""Token""]
    }
  ],
  ""postDeploymentActions"": [
    {
      ""contract"": ""Token"",
      ""method"": ""mint"",
      ""parameters"": [""NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB"", 100000]
    },
    {
      ""contract"": ""Governance"",
      ""method"": ""setVotingPeriod"",
      ""parameters"": [86400]
    }
  ]
}";

        var manifestPath = Path.Combine(outputDir, "deployment-manifest.json");
        File.WriteAllText(manifestPath, manifest);
        return manifestPath;
    }

    private async Task<DeploymentManifest> LoadDeploymentManifest(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        return System.Text.Json.JsonSerializer.Deserialize<DeploymentManifest>(json)!;
    }

    private string CreateContractProjectFromDefinition(ContractDefinition definition, Dictionary<string, ContractDeploymentInfo> deployedContracts)
    {
        // Generate contract code based on type
        var contractCode = definition.Type switch
        {
            "storage" => GenerateStorageContract(definition.Name),
            "token" => GenerateTokenContract(definition.Name, definition.InitialParameters),
            "governance" => GenerateGovernanceContract(definition.Name, deployedContracts),
            _ => throw new NotSupportedException($"Contract type {definition.Type} not supported")
        };

        return CreateTestContractProject(definition.Name, contractCode);
    }

    private string GenerateStorageContract(string name)
    {
        return $@"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Contracts
{{
    [ManifestExtra(""Author"", ""Neo"")]
    public class {name} : SmartContract
    {{
        [DisplayName(""put"")]
        public static bool Put(string key, ByteString value)
        {{
            Storage.Put(Storage.CurrentContext, key, value);
            return true;
        }}

        [DisplayName(""get"")]
        public static ByteString Get(string key)
        {{
            return Storage.Get(Storage.CurrentContext, key);
        }}
    }}
}}";
    }

    private string GenerateTokenContract(string name, List<object> parameters)
    {
        var tokenName = parameters.Count > 0 ? parameters[0].ToString() : "Token";
        var symbol = parameters.Count > 1 ? parameters[1].ToString() : "TKN";
        var decimals = parameters.Count > 2 ? parameters[2] : 8;
        var totalSupply = parameters.Count > 3 ? parameters[3] : 1000000;

        return $@"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace Contracts
{{
    [ManifestExtra(""Author"", ""Neo"")]
    [SupportedStandards(""NEP-17"")]
    public class {name} : SmartContract
    {{
        [DisplayName(""symbol"")]
        public static string Symbol() => ""{symbol}"";

        [DisplayName(""decimals"")]
        public static byte Decimals() => {decimals};

        [DisplayName(""totalSupply"")]
        public static BigInteger TotalSupply() => {totalSupply} * BigInteger.Pow(10, {decimals});

        [DisplayName(""balanceOf"")]
        public static BigInteger BalanceOf(UInt160 account)
        {{
            var balance = Storage.Get(Storage.CurrentContext, account);
            return balance != null ? (BigInteger)balance : 0;
        }}

        [DisplayName(""transfer"")]
        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {{
            if (amount <= 0) return false;
            if (!Runtime.CheckWitness(from)) return false;
            
            var fromBalance = BalanceOf(from);
            if (fromBalance < amount) return false;
            
            Storage.Put(Storage.CurrentContext, from, fromBalance - amount);
            Storage.Put(Storage.CurrentContext, to, BalanceOf(to) + amount);
            
            return true;
        }}

        [DisplayName(""mint"")]
        public static bool Mint(UInt160 to, BigInteger amount)
        {{
            if (amount <= 0) return false;
            Storage.Put(Storage.CurrentContext, to, BalanceOf(to) + amount);
            return true;
        }}

        public static void _deploy(object data, bool update)
        {{
            if (!update)
            {{
                // Initial supply to deployer
                var deployer = (UInt160)data;
                Storage.Put(Storage.CurrentContext, deployer, TotalSupply());
            }}
        }}
    }}
}}";
    }

    private string GenerateGovernanceContract(string name, Dictionary<string, ContractDeploymentInfo> deployedContracts)
    {
        var tokenHash = deployedContracts.ContainsKey("Token") 
            ? deployedContracts["Token"].ContractHash.ToString() 
            : "UInt160.Zero";

        return $@"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo;
using System.ComponentModel;

namespace Contracts
{{
    [ManifestExtra(""Author"", ""Neo"")]
    public class {name} : SmartContract
    {{
        private static readonly UInt160 TokenContract = UInt160.Parse(""{tokenHash}"");
        private const string VotingPeriodKey = ""votingPeriod"";

        [DisplayName(""setVotingPeriod"")]
        public static bool SetVotingPeriod(int seconds)
        {{
            if (seconds <= 0) return false;
            Storage.Put(Storage.CurrentContext, VotingPeriodKey, seconds);
            return true;
        }}

        [DisplayName(""getVotingPeriod"")]
        public static int GetVotingPeriod()
        {{
            var period = Storage.Get(Storage.CurrentContext, VotingPeriodKey);
            return period != null ? (int)period : 86400; // Default 24 hours
        }}

        [DisplayName(""createProposal"")]
        public static bool CreateProposal(string description)
        {{
            if (string.IsNullOrEmpty(description)) return false;
            
            // Check proposer has tokens
            var proposer = Runtime.CallingScriptHash;
            var balance = (BigInteger)Contract.Call(TokenContract, ""balanceOf"", CallFlags.ReadOnly, proposer);
            if (balance <= 0) return false;
            
            // Store proposal
            var proposalId = Runtime.Time;
            Storage.Put(Storage.CurrentContext, proposalId, description);
            
            return true;
        }}
    }}
}}";
    }


    #endregion

    #region Nested Classes

    private class DeploymentManifest
    {
        public string Version { get; set; } = "1.0";
        public string Description { get; set; } = "";
        public List<ContractDefinition> Contracts { get; set; } = new();
        public List<PostDeploymentAction> PostDeploymentActions { get; set; } = new();
    }

    private class ContractDefinition
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public long GasLimit { get; set; }
        public bool EnableDebug { get; set; }
        public List<object> InitialParameters { get; set; } = new();
        public List<string> Dependencies { get; set; } = new();
    }

    private class PostDeploymentAction
    {
        public string Contract { get; set; } = "";
        public string Method { get; set; } = "";
        public List<object> Parameters { get; set; } = new();
    }

    #endregion
}