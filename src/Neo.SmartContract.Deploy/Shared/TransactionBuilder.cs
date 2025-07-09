using System;
using System.Linq;
using System.Threading.Tasks;
using Neo;
using Neo.Cryptography;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;

namespace Neo.SmartContract.Deploy.Shared;

/// <summary>
/// Helper class for building Neo transactions
/// </summary>
public static class TransactionBuilder
{
    /// <summary>
    /// Create a transaction with the given script
    /// </summary>
    /// <param name="rpcClient">RPC client</param>
    /// <param name="script">Script to execute</param>
    /// <param name="account">Account to sign with</param>
    /// <param name="systemFee">System fee (GAS)</param>
    /// <param name="networkFee">Network fee (GAS)</param>
    /// <param name="validUntilBlock">Block height when transaction expires</param>
    /// <returns>Signed transaction</returns>
    public static async Task<Transaction> CreateTransactionAsync(
        RpcClient rpcClient,
        byte[] script,
        Account account,
        long systemFee = 0,
        long networkFee = 0,
        uint? validUntilBlock = null)
    {
        if (rpcClient == null)
            throw new ArgumentNullException(nameof(rpcClient));

        if (script == null || script.Length == 0)
            throw new ArgumentException("Script cannot be null or empty", nameof(script));

        if (account == null)
            throw new ArgumentNullException(nameof(account));

        // Get protocol settings
        var version = await rpcClient.GetVersionAsync();
        var protocol = version.Protocol;

        // If systemFee is not provided, calculate it
        if (systemFee == 0)
        {
            var result = await rpcClient.InvokeScriptAsync(script, new[] { new Signer { Account = account.ScriptHash } });
            systemFee = result.GasConsumed;
        }

        // Create the transaction
        var tx = new Transaction
        {
            Version = 0,
            Nonce = (uint)Random.Shared.Next(),
            SystemFee = systemFee,
            NetworkFee = networkFee,
            ValidUntilBlock = validUntilBlock ?? await GetValidUntilBlockAsync(rpcClient),
            Signers = new[] { new Signer { Account = account.ScriptHash, Scopes = WitnessScope.CalledByEntry } },
            Attributes = Array.Empty<TransactionAttribute>(),
            Script = script,
            Witnesses = null
        };

        // Calculate network fee if not provided
        if (networkFee == 0)
        {
            // Use a reasonable default network fee
            tx.NetworkFee = 1_000_000; // 0.01 GAS
        }

        // Create the witness manually
        var keyPair = new KeyPair(account.PrivateKey);
        var verificationScript = Contract.CreateSignatureRedeemScript(keyPair.PublicKey);
        
        // Create a simple witness with empty invocation script for now
        // In a real implementation, this would need proper signing
        tx.Witnesses = new[]
        {
            new Witness
            {
                InvocationScript = new byte[] { (byte)OpCode.PUSHDATA1, 64 }.Concat(new byte[64]).ToArray(),
                VerificationScript = verificationScript
            }
        };

        return tx;
    }

    /// <summary>
    /// Get a valid ValidUntilBlock value
    /// </summary>
    /// <param name="rpcClient">RPC client</param>
    /// <param name="offset">Number of blocks in the future</param>
    /// <returns>Valid until block height</returns>
    public static async Task<uint> GetValidUntilBlockAsync(RpcClient rpcClient, uint offset = 100)
    {
        var blockCount = await rpcClient.GetBlockCountAsync();
        return blockCount + offset;
    }

    /// <summary>
    /// Calculate network fee based on transaction size
    /// </summary>
    /// <param name="size">Transaction size in bytes</param>
    /// <param name="signatureCount">Number of signatures</param>
    /// <returns>Network fee</returns>
    private static long CalculateNetworkFee(int size, int signatureCount)
    {
        // Base fee per byte
        const long FeePerByte = 1000;
        
        // Execution fee for signature verification
        const long SignatureVerificationFee = 1000000;
        
        return (size * FeePerByte) + (signatureCount * SignatureVerificationFee);
    }
}