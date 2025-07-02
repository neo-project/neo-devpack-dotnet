using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.Wallets;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Services;

public class WalletManagerServiceTests : TestBase
{
    private readonly WalletManagerService _walletService;
    private readonly Mock<ILogger<WalletManagerService>> _mockLogger;
    private readonly string _testWalletPath;

    public WalletManagerServiceTests()
    {
        _mockLogger = new Mock<ILogger<WalletManagerService>>();
        _walletService = new WalletManagerService(_mockLogger.Object);
        _testWalletPath = Path.Combine(Path.GetTempPath(), "test-wallet.json");
        
        CreateTestWallet();
    }

    private void CreateTestWallet()
    {
        // Using the EXACT wallet JSON string from Neo repository tests (UT_RpcServer.Wallet.cs)
        // This wallet is known to work with password "123456"
        var walletJson = "{\"name\":null,\"version\":\"1.0\",\"scrypt\":{\"n\":16384,\"r\":8,\"p\":8},\"accounts\":[{\"address\":\"NVizn8DiExdmnpTQfjiVY3dox8uXg3Vrxv\",\"label\":null,\"isDefault\":false,\"lock\":false,\"key\":\"6PYPMrsCJ3D4AXJCFWYT2WMSBGF7dLoaNipW14t4UFAkZw3Z9vQRQV1bEU\",\"contract\":{\"script\":\"DCEDaR\\u002BFVb8lOdiMZ/wCHLiI\\u002Bzuf17YuGFReFyHQhB80yMpBVuezJw==\",\"parameters\":[{\"name\":\"signature\",\"type\":\"Signature\"}],\"deployed\":false},\"extra\":null}],\"extra\":null}";
        File.WriteAllText(_testWalletPath, walletJson);
    }

    [Fact]
    public async Task LoadWalletAsync_WithValidWallet_ShouldLoadSuccessfully()
    {
        // Act
        await _walletService.LoadWalletAsync(_testWalletPath, "123456");

        // Assert
        var account = _walletService.GetAccount();
        Assert.NotNull(account);
        Assert.Equal("NVizn8DiExdmnpTQfjiVY3dox8uXg3Vrxv", account.ToAddress(ProtocolSettings.Default.AddressVersion));
    }

    [Fact]
    public async Task LoadWalletAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _walletService.LoadWalletAsync("/non/existent/wallet.json", "password"));
    }

    [Fact]
    public async Task LoadWalletAsync_WithInvalidPassword_ShouldThrowWalletException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _walletService.LoadWalletAsync(_testWalletPath, "wrongpassword"));
    }

    [Fact]
    public void GetAccount_WithoutLoadingWallet_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var freshWalletService = new WalletManagerService(_mockLogger.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => freshWalletService.GetAccount());
    }

    [Fact]
    public async Task GetAccount_WithSpecificAddress_ShouldReturnCorrectAccount()
    {
        // Arrange
        await _walletService.LoadWalletAsync(_testWalletPath, "123456");
        var expectedAddress = "NVizn8DiExdmnpTQfjiVY3dox8uXg3Vrxv"; // Address from our test wallet

        // Act
        var account = _walletService.GetAccount(expectedAddress);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(expectedAddress, account.ToAddress(ProtocolSettings.Default.AddressVersion));
    }

    [Fact]
    public async Task GetAccount_WithNonExistentAddress_ShouldThrowWalletException()
    {
        // Arrange
        await _walletService.LoadWalletAsync(_testWalletPath, "123456");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            _walletService.GetAccount("NNonExistentAddressXXXXXXXXXXXXXXXXXX"));
    }

    [Fact]
    public async Task SignTransactionAsync_WithValidTransaction_ShouldSignSuccessfully()
    {
        // Arrange
        await _walletService.LoadWalletAsync(_testWalletPath, "123456");
        var signerAccount = _walletService.GetAccount();

        // Create a simple transaction
        var transaction = new Transaction
        {
            Version = 0,
            Nonce = 1234567890,
            SystemFee = 1000000,
            NetworkFee = 1000000,
            ValidUntilBlock = 2000000,
            Signers = new[] 
            { 
                new Signer 
                { 
                    Account = signerAccount, 
                    Scopes = WitnessScope.CalledByEntry 
                } 
            },
            Attributes = System.Array.Empty<TransactionAttribute>(),
            Script = new byte[] { 0x40, 0x41, 0x9F, 0x00, 0x00, 0x00 }, // Simple script
            Witnesses = System.Array.Empty<Witness>()
        };

        // Act
        await _walletService.SignTransactionAsync(transaction, signerAccount);

        // Assert
        Assert.NotEmpty(transaction.Witnesses);
        Assert.Single(transaction.Witnesses);
        Assert.True(transaction.Witnesses[0].InvocationScript.Length > 0);
    }

    [Fact]
    public async Task SignTransactionAsync_WithInvalidSigner_ShouldThrowWalletException()
    {
        // Arrange
        await _walletService.LoadWalletAsync(_testWalletPath, "123456");
        var invalidSigner = UInt160.Parse("0x1234567890123456789012345678901234567890");

        var transaction = new Transaction
        {
            Version = 0,
            Nonce = 1234567890,
            SystemFee = 1000000,
            NetworkFee = 1000000,
            ValidUntilBlock = 2000000,
            Signers = new[] 
            { 
                new Signer 
                { 
                    Account = invalidSigner, 
                    Scopes = WitnessScope.CalledByEntry 
                } 
            },
            Attributes = System.Array.Empty<TransactionAttribute>(),
            Script = new byte[] { 0x40, 0x41, 0x9F, 0x00, 0x00, 0x00 },
            Witnesses = System.Array.Empty<Witness>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<WalletException>(() => 
            _walletService.SignTransactionAsync(transaction, invalidSigner));
    }

    [Fact]
    public async Task IsWalletLoaded_AfterLoadingWallet_ShouldReturnTrue()
    {
        // Arrange & Act
        await _walletService.LoadWalletAsync(_testWalletPath, "123456");

        // Assert
        Assert.True(_walletService.IsWalletLoaded);
    }

    [Fact]
    public void IsWalletLoaded_WithoutLoadingWallet_ShouldReturnFalse()
    {
        // Arrange
        var freshWalletService = new WalletManagerService(_mockLogger.Object);

        // Assert
        Assert.False(freshWalletService.IsWalletLoaded);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (File.Exists(_testWalletPath))
            {
                File.Delete(_testWalletPath);
            }
        }
        base.Dispose(disposing);
    }
}