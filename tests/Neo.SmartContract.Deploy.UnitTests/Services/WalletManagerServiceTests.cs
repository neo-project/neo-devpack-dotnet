using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Services;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class WalletManagerServiceTests : TestBase
{
    private readonly WalletManagerService _walletManager;
    private readonly Mock<ILogger<WalletManagerService>> _mockLogger;
    private const string ValidWifKey = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g";

    public WalletManagerServiceTests()
    {
        _mockLogger = new Mock<ILogger<WalletManagerService>>();
        _walletManager = new WalletManagerService(_mockLogger.Object);
    }

    [Fact]
    public void GetAccountFromWif_WithNullKey_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _walletManager.GetAccountFromWif(null!));
    }

    [Fact]
    public void GetAccountFromWif_WithEmptyKey_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _walletManager.GetAccountFromWif(""));
    }

    [Fact]
    public void GetAccountFromWif_WithInvalidKey_ShouldThrowContractDeploymentException()
    {
        // Arrange
        var invalidWif = "invalid-wif-key";

        // Act & Assert
        var ex = Assert.Throws<ContractDeploymentException>(() =>
            _walletManager.GetAccountFromWif(invalidWif));
        
        Assert.Contains("Invalid WIF key", ex.Message);
    }

    [Fact]
    public void GetAccountFromWif_WithValidKey_ShouldReturnAccount()
    {
        // Act
        var account = _walletManager.GetAccountFromWif(ValidWifKey);

        // Assert
        Assert.NotNull(account);
        Assert.NotNull(account.PrivateKey);
        Assert.NotNull(account.PublicKey);
        Assert.NotEqual(UInt160.Zero, account.ScriptHash);
    }

    [Fact]
    public void GetAccountAddress_WithNullAccount_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _walletManager.GetAccountAddress(null!));
    }

    [Fact]
    public void GetAccountAddress_WithValidAccount_ShouldReturnAddress()
    {
        // Arrange
        var account = _walletManager.GetAccountFromWif(ValidWifKey);

        // Act
        var address = _walletManager.GetAccountAddress(account);

        // Assert
        Assert.NotNull(address);
        Assert.StartsWith("N", address); // Neo addresses start with 'N'
        Assert.Equal(34, address.Length); // Neo addresses are 34 characters
    }

    [Fact]
    public void GetAccountScriptHash_WithNullAccount_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _walletManager.GetAccountScriptHash(null!));
    }

    [Fact]
    public void GetAccountScriptHash_WithValidAccount_ShouldReturnScriptHash()
    {
        // Arrange
        var account = _walletManager.GetAccountFromWif(ValidWifKey);

        // Act
        var scriptHash = _walletManager.GetAccountScriptHash(account);

        // Assert
        Assert.NotNull(scriptHash);
        Assert.NotEqual(UInt160.Zero, scriptHash);
        Assert.Equal(account.ScriptHash, scriptHash);
    }

    [Fact]
    public async Task GetGasBalanceAsync_WithNullAccountHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        var rpcUrl = "http://localhost:50012";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _walletManager.GetGasBalanceAsync(null!, rpcUrl));
    }

    [Fact]
    public async Task GetGasBalanceAsync_WithNullRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var accountHash = UInt160.Parse("0x1234567890123456789012345678901234567890");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _walletManager.GetGasBalanceAsync(accountHash, null!));
    }

    [Fact]
    public async Task GetGasBalanceAsync_WithEmptyRpcUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var accountHash = UInt160.Parse("0x1234567890123456789012345678901234567890");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _walletManager.GetGasBalanceAsync(accountHash, ""));
    }

    [Fact]
    public void CreateSignatureContract_WithNullPublicKey_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _walletManager.CreateSignatureContract(null!));
    }

    [Fact]
    public void CreateSignatureContract_WithValidPublicKey_ShouldReturnScriptHash()
    {
        // Arrange
        var account = _walletManager.GetAccountFromWif(ValidWifKey);

        // Act
        var scriptHash = _walletManager.CreateSignatureContract(account.PublicKey);

        // Assert
        Assert.NotNull(scriptHash);
        Assert.NotEqual(UInt160.Zero, scriptHash);
        Assert.Equal(account.ScriptHash, scriptHash);
    }

    [Fact(Skip = "Need valid test WIF keys")]
    public void MultipleAccounts_FromDifferentWifs_ShouldHaveDifferentScriptHashes()
    {
        // Arrange
        var wif1 = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g";
        var wif2 = "L2NrNsScMZJzepMXuMPTiZhyBYrrMG5b5a69RP3nfBH49F7V5whU";

        // Act
        var account1 = _walletManager.GetAccountFromWif(wif1);
        var account2 = _walletManager.GetAccountFromWif(wif2);

        // Assert
        Assert.NotEqual(account1.ScriptHash, account2.ScriptHash);
        Assert.NotEqual(account1.PublicKey, account2.PublicKey);
    }
}