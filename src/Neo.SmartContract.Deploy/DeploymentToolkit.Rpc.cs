using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Extensions;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy;

public partial class DeploymentToolkit
{
    /// <summary>
    /// Call a contract method (read-only).
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Method return value</returns>
    public Task<T> CallAsync<T>(string contractHashOrAddress, string method, params object[] args)
        => CallAsync<T>(contractHashOrAddress, method, cancellationToken: default, args);

    /// <summary>
    /// Call a contract method (read-only) with cancellation support.
    /// </summary>
    public async Task<T> CallAsync<T>(string contractHashOrAddress, string method, CancellationToken cancellationToken, params object[] args)
    {
        EnsureNotDisposed();

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, protocolSettings);
            var script = BuildContractCallScript(hash, method, CallFlags.ReadOnly, args);
            var result = await rpc.InvokeScriptAsync(script).ConfigureAwait(false);

            if (result.State.HasFlag(VMState.FAULT))
                throw new InvalidOperationException($"Call fault: {result.Exception}");

            if (result.Stack == null || result.Stack.Length == 0)
                return default!;

            var item = result.Stack[0];
            object? value = ConvertStackItem<T>(item);
            return (T)value!;
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Invoke a contract method (state-changing transaction).
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <returns>Transaction hash</returns>
    public Task<UInt256> InvokeAsync(string contractHashOrAddress, string method, params object[] args)
        => InvokeAsync(contractHashOrAddress, method, cancellationToken: default, args);

    /// <summary>
    /// Invoke a contract method (state-changing) with cancellation support.
    /// </summary>
    public async Task<UInt256> InvokeAsync(string contractHashOrAddress, string method, CancellationToken cancellationToken, params object[] args)
    {
        EnsureNotDisposed();
        var wif = EnsureWif();
        var sender = await GetDeployerAccountAsync().ConfigureAwait(false);

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, protocolSettings);
            var script = BuildContractCallScript(hash, method, CallFlags.All, args);

            var signer = new Signer { Account = sender, Scopes = WitnessScope.CalledByEntry };
            var tm = await TransactionManager.MakeTransactionAsync(rpc, script, [signer]).ConfigureAwait(false);

            var key = Neo.Network.RPC.Utility.GetKeyPair(wif);
            tm.AddSignature(key);
            var tx = await tm.SignAsync().ConfigureAwait(false);
            return await rpc.SendRawTransactionAsync(tx).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Get the default deployer account
    /// </summary>
    /// <returns>Deployer account script hash</returns>
    /// <exception cref="InvalidOperationException">Thrown when no deployer account is configured</exception>
    public Task<UInt160> GetDeployerAccountAsync()
    {
        EnsureNotDisposed();
        var wif = EnsureWif();

        var privateKey = Wallet.GetPrivateKeyFromWIF(wif);
        var keyPair = new KeyPair(privateKey);
        var account = Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;
        return Task.FromResult(account);
    }

    /// <summary>
    /// Get the current balance of an account.
    /// </summary>
    /// <param name="address">Account address (null for default deployer)</param>
    /// <returns>GAS balance</returns>
    public Task<decimal> GetGasBalanceAsync(string? address = null)
        => GetGasBalanceAsync(address, CancellationToken.None);

    /// <summary>
    /// Get the current balance of an account with cancellation support.
    /// </summary>
    public async Task<decimal> GetGasBalanceAsync(string? address, CancellationToken cancellationToken)
    {
        EnsureNotDisposed();

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            UInt160 account = !string.IsNullOrEmpty(address)
                ? Neo.Network.RPC.Utility.GetScriptHash(address, protocolSettings)
                : await GetDeployerAccountAsync().ConfigureAwait(false);

            var nep17 = new Nep17API(rpc);
            var balance = await nep17.BalanceOfAsync(NativeContract.GAS.Hash, account).ConfigureAwait(false);
            var decimals = await nep17.DecimalsAsync(NativeContract.GAS.Hash).ConfigureAwait(false);
            var factor = BigInteger.Pow(10, (int)decimals);
            return (decimal)balance / (decimal)factor;
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Check if a contract exists at the given address.
    /// </summary>
    /// <param name="contractHashOrAddress">Contract hash or address</param>
    /// <returns>True if contract exists, false otherwise</returns>
    public Task<bool> ContractExistsAsync(string contractHashOrAddress)
        => ContractExistsAsync(contractHashOrAddress, CancellationToken.None);

    /// <summary>
    /// Check if a contract exists with cancellation support.
    /// </summary>
    public async Task<bool> ContractExistsAsync(string contractHashOrAddress, CancellationToken cancellationToken)
    {
        EnsureNotDisposed();

        return await WithRpcClientAsync(async (rpc, protocolSettings, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var hash = Neo.Network.RPC.Utility.GetScriptHash(contractHashOrAddress, protocolSettings).ToString();
                _ = await rpc.GetContractStateAsync(hash).ConfigureAwait(false);
                return true;
            }
            catch (RpcException ex) when (ex.HResult == RpcUnknownContractCode)
            {
                return false;
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResult> WithRpcClientAsync<TResult>(
        Func<RpcClient, ProtocolSettings, CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();

        var protocolSettings = await GetProtocolSettingsAsync().ConfigureAwait(false);
        using var rpc = _rpcClientFactory.Create(_networkProfile.RpcUri, protocolSettings);
        return await action(rpc, protocolSettings, cancellationToken).ConfigureAwait(false);
    }

    private static byte[] BuildContractCallScript(UInt160 scriptHash, string method, CallFlags flags, params object[] args)
    {
        using var sb = new ScriptBuilder();
        if (args is { Length: > 0 })
        {
            for (int i = args.Length - 1; i >= 0; i--) sb.EmitPush(args[i]!);
            sb.EmitPush(args.Length);
            sb.Emit(OpCode.PACK);
        }
        else
        {
            sb.Emit(OpCode.NEWARRAY0);
        }
        sb.EmitPush((byte)flags);
        sb.EmitPush(method);
        sb.EmitPush(scriptHash);
        sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
        return sb.ToArray();
    }

    private static object? ConvertStackItem<T>(Neo.VM.Types.StackItem item)
    {
        var target = typeof(T);
        if (target == typeof(string)) return item.GetString();
        if (target == typeof(bool)) return item.GetBoolean();
        if (target == typeof(int)) return (int)item.GetInteger();
        if (target == typeof(long)) return (long)item.GetInteger();
        if (target == typeof(BigInteger)) return item.GetInteger();
        if (target == typeof(byte[])) return item.GetSpan().ToArray();
        if (target == typeof(UInt160)) return new UInt160(item.GetSpan());
        if (target == typeof(UInt256)) return new UInt256(item.GetSpan());
        return item.GetString();
    }
}
