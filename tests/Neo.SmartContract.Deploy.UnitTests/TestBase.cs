using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Testing;
using System;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.UnitTests;

/// <summary>
/// Base class for deployment toolkit tests
/// </summary>
public abstract class TestBase : IDisposable
{
    protected TestEngine Engine { get; private set; }
    protected IConfiguration Configuration { get; private set; }
    protected ILoggerFactory LoggerFactory { get; private set; }
    protected bool _disposed = false;

    private static readonly object _engineLock = new object();

    protected TestBase()
    {
        // Create test configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Create logger factory
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
        });

        // Initialize test engine (equivalent to Neo Express) with thread safety
        lock (_engineLock)
        {
            Engine = new TestEngine();
        }

        // Create test wallet and accounts
        SetupTestEnvironment();
    }

    private void SetupTestEnvironment()
    {
        // The TestEngine automatically sets up accounts
        // We can get the default account which has initial GAS balance
        var defaultAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"); // Example account
    }

    protected NeoContractToolkit CreateToolkit()
    {
        return NeoContractToolkitBuilder.Create()
            .ConfigureLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<IConfiguration>(Configuration);
                services.AddSingleton(Engine); // Add TestEngine to DI
            })
            .UseDeployer<TestEngineServices.SimpleTestDeployer>() // Use simple mock deployer
            .UseInvoker<TestEngineServices.SimpleTestInvoker>()   // Use simple mock invoker
            .UseWalletManager<TestEngineServices.SimpleTestWalletManager>() // Use simple mock wallet manager
            .Build();
    }

    protected string CreateTestContract()
    {
        // Create a simple test contract
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Test Contract"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""testMethod"")]
        public static string TestMethod(string input)
        {
            return ""Hello "" + input;
        }

        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 42;
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                // Initialization logic for new deployment
                Storage.Put(Storage.CurrentContext, ""initialized"", 1);
            }
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                LoggerFactory?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected string CreateTestContractProject(string contractName = "TestContract", string contractCode = null)
    {
        if (contractCode == null)
        {
            contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace " + contractName + @"
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Test Contract"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class " + contractName + @" : SmartContract
    {
        [DisplayName(""TestMethod"")]
        public static string TestMethod(string input)
        {
            return ""Hello "" + input;
        }

        [DisplayName(""GetValue"")]
        public static int GetValue()
        {
            return 42;
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
        }

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), contractName);
        Directory.CreateDirectory(tempDir);

        // Create the contract source file
        var contractPath = Path.Combine(tempDir, $"{contractName}.cs");
        File.WriteAllText(contractPath, contractCode);

        // Create the project file
        var projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <RootNamespace>{contractName}</RootNamespace>
    <AssemblyName>{contractName}</AssemblyName>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1"" />
  </ItemGroup>

  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Message Text=""Smart contract build completed"" Importance=""high"" />
  </Target>
</Project>";

        var projectPath = Path.Combine(tempDir, $"{contractName}.csproj");
        File.WriteAllText(projectPath, projectContent);

        return projectPath;
    }

    protected async Task CreateTestWalletFile(string walletPath)
    {
        // For now, use the exact same wallet format as the working unit tests
        // We'll create a wallet that uses a well-known format but will transfer
        // GAS from the consensus node during the test
        var walletJson = @"{
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
        await File.WriteAllTextAsync(walletPath, walletJson);
    }
}
