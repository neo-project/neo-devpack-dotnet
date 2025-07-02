using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Testing;
using System;
using System.IO;

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

        // Initialize test engine (equivalent to Neo Express)
        Engine = new TestEngine();
        
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
            })
            .Build();
    }

    protected string CreateTestContract()
    {
        // Create a simple test contract
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Test Contract"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class TestContract : SmartContract
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
                // Initialization logic for new deployment
                Storage.Put(Storage.CurrentContext, ""initialized"", true);
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
}