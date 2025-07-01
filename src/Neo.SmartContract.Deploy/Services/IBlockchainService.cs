using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Service for interacting with the Neo blockchain
    /// </summary>
    public interface IBlockchainService
    {
        /// <summary>
        /// Get the RPC client instance
        /// </summary>
        RpcClient GetRpcClient();

        /// <summary>
        /// Test invoke a script without sending a transaction
        /// </summary>
        Task<RpcInvokeResult> TestInvokeAsync(byte[] script, params Signer[] signers);

        /// <summary>
        /// Send a transaction to the blockchain
        /// </summary>
        Task<UInt256> SendTransactionAsync(Transaction transaction);

        /// <summary>
        /// Get the current block count
        /// </summary>
        Task<uint> GetBlockCountAsync();

        /// <summary>
        /// Get NEP-17 token balances for an address
        /// </summary>
        Task<RpcNep17Balance[]> GetNep17BalancesAsync(string address);

        /// <summary>
        /// Get GAS balance for an account
        /// </summary>
        Task<BigDecimal> GetGasBalanceAsync(UInt160 account);
    }
}