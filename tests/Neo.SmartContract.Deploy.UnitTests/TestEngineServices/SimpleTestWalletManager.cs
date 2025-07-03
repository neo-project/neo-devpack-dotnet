using Microsoft.Extensions.Logging;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Deploy.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.UnitTests.TestEngineServices;

/// <summary>
/// Simple mock implementation of IWalletManager for integration tests
/// </summary>
public class SimpleTestWalletManager : IWalletManager
{
    private readonly ILogger<SimpleTestWalletManager> _logger;
    private bool _isLoaded = false;
    private readonly UInt160 _defaultAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688");
    private readonly UInt160 _secondAccount = UInt160.Parse("0xa2983fa2021e0c36e5e37c2771b8bb7b5c525688");

    public SimpleTestWalletManager(ILogger<SimpleTestWalletManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task LoadWalletAsync(string walletPath, string password)
    {
        _logger.LogInformation("Mock loading wallet from {WalletPath}", walletPath);
        _isLoaded = true;
        await Task.CompletedTask;
    }

    public UInt160 GetAccount(string? accountAddress = null)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Wallet not loaded");

        if (string.IsNullOrEmpty(accountAddress))
        {
            return _defaultAccount;
        }

        if (UInt160.TryParse(accountAddress, out var address))
        {
            return address;
        }

        throw new InvalidOperationException($"Invalid account address: {accountAddress}");
    }

    public async Task SignTransactionAsync(Transaction transaction, UInt160? account = null)
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Wallet not loaded");

        _logger.LogDebug("Mock signing transaction");
        await Task.CompletedTask;
    }

    public IEnumerable<UInt160> GetAccounts()
    {
        if (!_isLoaded)
            throw new InvalidOperationException("Wallet not loaded");

        yield return _defaultAccount;
        yield return _secondAccount;
    }

    public bool IsWalletLoaded => _isLoaded;
}
