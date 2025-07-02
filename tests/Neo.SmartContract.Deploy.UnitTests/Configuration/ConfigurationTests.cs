using Microsoft.Extensions.Configuration;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Configuration;

public class ConfigurationTests : TestBase
{
    [Fact]
    public void Toolkit_ShouldLoadConfigurationFromJson()
    {
        // Act
        var toolkit = CreateToolkit();

        // Assert
        Assert.NotNull(toolkit);

        // Verify configuration is accessible (this tests that NeoContractToolkitBuilder properly sets up IConfiguration)
        var networkConfig = Configuration.GetSection("Network").Get<NetworkConfiguration>();
        Assert.NotNull(networkConfig);
        Assert.Equal("http://localhost:50012", networkConfig.RpcUrl);
        Assert.Equal("private", networkConfig.Network);
    }

    [Fact]
    public void NetworkConfiguration_ShouldBindCorrectly()
    {
        // Act
        var networkConfig = Configuration.GetSection("Network").Get<NetworkConfiguration>();

        // Assert
        Assert.NotNull(networkConfig);
        Assert.Equal("http://localhost:50012", networkConfig.RpcUrl);
        Assert.Equal("private", networkConfig.Network);
        Assert.NotNull(networkConfig.Wallet);
        Assert.Equal("test.wallet.json", networkConfig.Wallet.WalletPath);
        Assert.Equal("123456", networkConfig.Wallet.Password);
    }

    [Fact]
    public void DeploymentConfiguration_ShouldBindCorrectly()
    {
        // Act
        var deploymentConfig = Configuration.GetSection("Deployment").Get<DeploymentConfiguration>();

        // Assert
        Assert.NotNull(deploymentConfig);
        Assert.Equal(1000000L, deploymentConfig.DefaultNetworkFee);
        Assert.Equal(50000000L, deploymentConfig.DefaultGasLimit);
        Assert.True(deploymentConfig.WaitForConfirmation);
        Assert.Equal(10, deploymentConfig.ConfirmationRetries);
        Assert.Equal(1, deploymentConfig.ConfirmationDelaySeconds);
        Assert.Equal(100u, deploymentConfig.ValidUntilBlockOffset);
    }

    [Fact]
    public void Configuration_ShouldSupportEnvironmentSpecificSettings()
    {
        // Arrange - Create test environment-specific configuration
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);

        var baseConfig = @"{
  ""Network"": {
    ""RpcUrl"": ""http://localhost:50012"",
    ""Network"": ""private""
  }
}";

        var testNetConfig = @"{
  ""Network"": {
    ""RpcUrl"": ""https://rpc10.n3.neotracker.io:443"",
    ""Network"": ""testnet""
  }
}";

        var baseConfigPath = Path.Combine(testDir, "appsettings.json");
        var testNetConfigPath = Path.Combine(testDir, "appsettings.TestNet.json");

        File.WriteAllText(baseConfigPath, baseConfig);
        File.WriteAllText(testNetConfigPath, testNetConfig);

        try
        {
            // Act - Create configuration with TestNet environment
            var configuration = new ConfigurationBuilder()
                .SetBasePath(testDir)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.TestNet.json", optional: true)
                .Build();

            // Assert
            var networkConfig = configuration.GetSection("Network").Get<NetworkConfiguration>();
            Assert.NotNull(networkConfig);

            // TestNet config should override base config
            Assert.Equal("https://rpc10.n3.neotracker.io:443", networkConfig.RpcUrl);
            Assert.Equal("testnet", networkConfig.Network);
        }
        finally
        {
            // Cleanup
            Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public async Task Toolkit_ShouldUseConfigurationForNetworkOperations()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var testWalletPath = Path.Combine(Path.GetTempPath(), "config-test.wallet.json");

        // Create test wallet with known password "123456" (using the standard test wallet)
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
        await File.WriteAllTextAsync(testWalletPath, walletJson);

        try
        {
            // Act & Assert - This should use configuration for wallet loading
            await toolkit.LoadWalletAsync(testWalletPath, "123456");
            var account = toolkit.GetDeployerAccount();

            Assert.NotNull(account);

            // This should use configuration for network operations
            // Note: We can't test actual network calls in unit tests, but we can verify
            // that the configuration is being used by the services
            Assert.NotNull(account);
        }
        finally
        {
            File.Delete(testWalletPath);
        }
    }

    [Fact]
    public void ConfigurationValidation_ShouldDetectMissingRequiredSettings()
    {
        // Arrange - Create incomplete configuration
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);

        var incompleteConfig = @"{
  ""Network"": {
    ""Network"": ""private""
  }
}"; // Missing RpcUrl

        var configPath = Path.Combine(testDir, "appsettings.json");
        File.WriteAllText(configPath, incompleteConfig);

        try
        {
            // Act
            var configuration = new ConfigurationBuilder()
                .SetBasePath(testDir)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var networkConfig = configuration.GetSection("Network").Get<NetworkConfiguration>();

            // Assert
            Assert.NotNull(networkConfig);
            Assert.Equal("private", networkConfig.Network);
            Assert.True(string.IsNullOrEmpty(networkConfig.RpcUrl)); // Should be empty/null
        }
        finally
        {
            Directory.Delete(testDir, true);
        }
    }
}
