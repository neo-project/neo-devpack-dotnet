using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Deploy.UnitTests;

public abstract class TestBase
{
    protected IConfiguration Configuration { get; }

    protected TestBase()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Network:RpcUrl", "http://localhost:50012"},
            {"Network:Network", "private"},
            {"Deployment:GasLimit", "100000000"},
            {"Deployment:WaitForConfirmation", "true"},
            {"Deployment:DefaultNetworkFee", "1000000"},
            {"Deployment:ValidUntilBlockOffset", "100"},
            {"Deployment:ConfirmationRetries", "3"},
            {"Deployment:ConfirmationDelaySeconds", "1"},
            {"Wallet:Path", "test-wallet.json"},
            {"Wallet:Password", "test-password"}
        };

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    protected string CreateTestContract()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Test Contract"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class TestContract : SmartContract
    {
        public static string TestMethod(string input)
        {
            return ""Hello "" + input;
        }

        public static int GetValue()
        {
            return 42;
        }

        public static void _deploy(object data, bool update)
        {
            // Initial deployment
            Storage.Put(Storage.CurrentContext, ""initialized"", 1);
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }
}
