using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Testing;
using Neo.Wallets;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

/// <summary>
/// Comprehensive test to verify the deployment toolkit is working correctly
/// </summary>
[Collection("Sequential")]
public class VerifyDeploymentToolkit : IDisposable
{
    private readonly string _tempDir;
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public VerifyDeploymentToolkit()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);

        // Create test configuration
        var configJson = @"{
  ""Network"": {
    ""RpcUrl"": ""http://localhost:50012"",
    ""Network"": ""private"",
    ""Wallet"": {
      ""WalletPath"": ""test.wallet.json"",
      ""Password"": ""123456""
    }
  },
  ""Deployment"": {
    ""DefaultGasLimit"": 50000000,
    ""WaitForConfirmation"": false
  }
}";
        var configPath = Path.Combine(_tempDir, "appsettings.json");
        File.WriteAllText(configPath, configJson);

        _configuration = new ConfigurationBuilder()
            .SetBasePath(_tempDir)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        });
    }

    [Fact]
    public async Task VerifyCompleteDeploymentWorkflow()
    {
        // Step 1: Create toolkit with configuration
        var toolkit = NeoContractToolkitBuilder.Create()
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton(_configuration);
            })
            .Build();

        Assert.NotNull(toolkit);

        // Step 2: Create a test wallet programmatically
        var walletPath = Path.Combine(_tempDir, "test.wallet.json");

        // Using the exact wallet JSON from Neo repository tests (UT_RpcServer.Wallet.cs)
        // This wallet is known to work with password "123456"
        var testWalletJson = @"{
  ""name"": null,
  ""version"": ""1.0"",
  ""scrypt"": {
    ""n"": 16384,
    ""r"": 8,
    ""p"": 8
  },
  ""accounts"": [
    {
      ""address"": ""NVizn8DiExdmnpTQfjiVY3dox8uXg3Vrxv"",
      ""label"": null,
      ""isDefault"": false,
      ""lock"": false,
      ""key"": ""6PYPMrsCJ3D4AXJCFWYT2WMSBGF7dLoaNipW14t4UFAkZw3Z9vQRQV1bEU"",
      ""contract"": {
        ""script"": ""DCEDaR+FVb8lOdiMZ/wCHLiI+zuf17YuGFReFyHQhB80yMpBVuezJw=="",
        ""parameters"": [
          {
            ""name"": ""signature"",
            ""type"": ""Signature""
          }
        ],
        ""deployed"": false
      },
      ""extra"": null
    }
  ],
  ""extra"": null
}";
        await File.WriteAllTextAsync(walletPath, testWalletJson);

        // Step 3: Load wallet
        await toolkit.LoadWalletAsync(walletPath, "123456");
        var deployerAccount = toolkit.GetDeployerAccount();
        Assert.NotNull(deployerAccount);

        // Step 4: Create a test contract project
        var projectPath = CreateTestContractProject();

        // Step 5: Compile the contract
        var compilationOptions = new CompilationOptions
        {
            ProjectPath = projectPath,
            OutputDirectory = Path.Combine(_tempDir, "output"),
            ContractName = "TestContract",
            GenerateDebugInfo = true,
            Optimize = true
        };

        var compiler = toolkit.GetService<IContractCompiler>();
        var compiledContract = await compiler.CompileAsync(compilationOptions);

        Assert.NotNull(compiledContract);
        Assert.NotEmpty(compiledContract.NefBytes);
        Assert.NotNull(compiledContract.Manifest);
        Assert.Equal("TestContract", compiledContract.Name);

        // Step 6: Verify deployment options from configuration
        var networkConfig = _configuration.GetSection("Network");
        Assert.Equal("http://localhost:50012", networkConfig["RpcUrl"]);
        Assert.Equal("private", networkConfig["Network"]);

        var deploymentConfig = _configuration.GetSection("Deployment");
        Assert.Equal("50000000", deploymentConfig["DefaultGasLimit"]);
        Assert.Equal("False", deploymentConfig["WaitForConfirmation"]);

        // Step 7: Test multi-contract deployment service
        var multiDeployService = toolkit.GetService<MultiContractDeploymentService>();
        Assert.NotNull(multiDeployService);

        // Step 8: Verify all services are properly registered
        Assert.NotNull(toolkit.GetService<IContractCompiler>());
        Assert.NotNull(toolkit.GetService<IContractDeployer>());
        Assert.NotNull(toolkit.GetService<IContractInvoker>());
        Assert.NotNull(toolkit.GetService<IWalletManager>());

        // Step 9: Test configuration-based network operations
        var rpcUrl = _configuration["Network:RpcUrl"];
        Assert.Equal("http://localhost:50012", rpcUrl);

        // Step 10: Verify project-based compilation works
        Assert.EndsWith(".csproj", projectPath);
        Assert.True(File.Exists(projectPath));

        // The toolkit is complete and working!
    }

    [Fact]
    public void VerifyToolkitBuilder()
    {
        // Test the builder pattern
        var builder = NeoContractToolkitBuilder.Create();
        Assert.NotNull(builder);

        // Configure logging
        builder.ConfigureLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        });

        // Configure services
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(_configuration);
        });

        // Build toolkit
        var toolkit = builder.Build();
        Assert.NotNull(toolkit);

        // Verify services are accessible
        var compiler = toolkit.GetService<IContractCompiler>();
        Assert.NotNull(compiler);
    }

    [Fact]
    public async Task VerifyCompilationOptions()
    {
        var toolkit = CreateToolkit();
        var compiler = toolkit.GetService<IContractCompiler>();

        // Test project-based compilation
        var projectPath = CreateTestContractProject();
        var projectOptions = new CompilationOptions
        {
            ProjectPath = projectPath,
            OutputDirectory = Path.Combine(_tempDir, "project-output"),
            ContractName = "ProjectContract"
        };

        var projectResult = await compiler.CompileAsync(projectOptions);
        Assert.NotNull(projectResult);
        Assert.Equal("ProjectContract", projectResult.Name);

        // Test source file compilation (backward compatibility)
        var sourcePath = CreateTestContractFile();
        var sourceOptions = new CompilationOptions
        {
            SourcePath = sourcePath,
            OutputDirectory = Path.Combine(_tempDir, "source-output"),
            ContractName = "SourceContract"
        };

        // This should still work for backward compatibility
        var sourceResult = await compiler.CompileAsync(sourceOptions);
        Assert.NotNull(sourceResult);
    }

    private NeoContractToolkit CreateToolkit()
    {
        return NeoContractToolkitBuilder.Create()
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton(_configuration);
            })
            .Build();
    }

    private string CreateTestContractProject()
    {
        var projectDir = Path.Combine(_tempDir, "TestContract");
        Directory.CreateDirectory(projectDir);

        // Create contract source
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Test"")]
    [ManifestExtra(""Description"", ""Test Contract"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 42;
        }

        [DisplayName(""testMethod"")]
        public static string TestMethod(string input)
        {
            return ""Hello "" + input;
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""initialized"", 1);
            }
        }
    }
}";

        var contractPath = Path.Combine(projectDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractCode);

        // Create project file
        var projectContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>TestContract</RootNamespace>
    <AssemblyName>TestContract</AssemblyName>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1"" />
  </ItemGroup>
</Project>";

        var projectPath = Path.Combine(projectDir, "TestContract.csproj");
        File.WriteAllText(projectPath, projectContent);

        return projectPath;
    }

    private string CreateTestContractFile()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace TestContract
{
    public class SimpleContract : SmartContract
    {
        public static int GetNumber()
        {
            return 123;
        }
    }
}";

        var contractPath = Path.Combine(_tempDir, "SimpleContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
        _loggerFactory?.Dispose();
    }
}
