using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.SmartContract.Deploy.Configuration;
using Neo.Wallets;
using System.Numerics;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Implementation of blockchain interaction service
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        private readonly ILogger<BlockchainService> _logger;
        private readonly NetworkOptions _options;
        private readonly RpcClient _rpcClient;

        public BlockchainService(ILogger<BlockchainService> logger, IOptions<NetworkOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _rpcClient = new RpcClient(new Uri(_options.RpcUrl));
        }

        public RpcClient GetRpcClient() => _rpcClient;

        public async Task<RpcInvokeResult> TestInvokeAsync(byte[] script, params Signer[] signers)
        {
            try
            {
                return await _rpcClient.InvokeScriptAsync(script, signers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test invoke script");
                throw;
            }
        }

        public async Task<UInt256> SendTransactionAsync(Transaction transaction)
        {
            try
            {
                var result = await _rpcClient.SendRawTransactionAsync(transaction);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send transaction");
                throw;
            }
        }

        public async Task<uint> GetBlockCountAsync()
        {
            return await _rpcClient.GetBlockCountAsync();
        }

        public async Task<RpcNep17Balance[]> GetNep17BalancesAsync(string address)
        {
            var result = await _rpcClient.GetNep17BalancesAsync(address);
            return result.Balances.ToArray();
        }

        public async Task<BigDecimal> GetGasBalanceAsync(UInt160 account)
        {
            var balances = await GetNep17BalancesAsync(account.ToAddress(ProtocolSettings.Default.AddressVersion));
            var gasBalance = balances.FirstOrDefault(b => b.AssetHash.Equals(Neo.SmartContract.Native.NativeContract.GAS.Hash));
            return gasBalance?.Amount ?? new BigDecimal(BigInteger.Zero, 0);
        }
    }
}