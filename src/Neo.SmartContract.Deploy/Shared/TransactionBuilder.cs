using System;
using System.Linq;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Shared;

/// <summary>
/// Builder for creating Neo transactions with consistent configuration
/// </summary>
public class TransactionBuilder
{
    private readonly Random _random = new();

    /// <summary>
    /// Build a transaction with the specified options
    /// </summary>
    /// <param name="options">Transaction building options</param>
    /// <returns>Built transaction</returns>
    public Transaction Build(TransactionBuildOptions options)
    {
        ValidateOptions(options);

        var signers = options.AdditionalSigners?.ToList() ?? new();

        // Ensure the sender is the first signer
        if (!signers.Any(s => s.Account == options.Sender))
        {
            signers.Insert(0, new Signer
            {
                Account = options.Sender,
                Scopes = options.SenderScope
            });
        }

        var transaction = new Transaction
        {
            Version = 0,
            Nonce = options.Nonce ?? (uint)_random.Next(),
            SystemFee = options.SystemFee,
            NetworkFee = options.NetworkFee,
            ValidUntilBlock = options.ValidUntilBlock,
            Signers = signers.ToArray(),
            Attributes = options.Attributes ?? Array.Empty<TransactionAttribute>(),
            Script = options.Script,
            Witnesses = Array.Empty<Witness>()
        };

        return transaction;
    }

    /// <summary>
    /// Calculate the network fee for a transaction
    /// </summary>
    /// <param name="transaction">Transaction to calculate fee for</param>
    /// <param name="snapshotHeight">Blockchain snapshot height</param>
    /// <returns>Network fee in GAS</returns>
    public static long CalculateNetworkFee(Transaction transaction, uint snapshotHeight)
    {
        // Base fee per byte
        const long FeePerByte = 1000; // 0.00001 GAS per byte

        // Calculate size fee
        var size = transaction.Size;
        var sizeFee = size * FeePerByte;

        // Add verification cost (simplified - actual implementation would verify scripts)
        const long VerificationCost = 1000000; // 0.01 GAS per signature
        var verificationFee = transaction.Signers.Length * VerificationCost;

        return sizeFee + verificationFee;
    }

    private void ValidateOptions(TransactionBuildOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Sender == UInt160.Zero)
            throw new ArgumentException("Sender cannot be zero", nameof(options));

        if (options.Script == null || options.Script.Length == 0)
            throw new ArgumentException("Script cannot be null or empty", nameof(options));

        if (options.ValidUntilBlock == 0)
            throw new ArgumentException("ValidUntilBlock must be greater than 0", nameof(options));

        if (options.SystemFee < 0)
            throw new ArgumentException("SystemFee cannot be negative", nameof(options));

        if (options.NetworkFee < 0)
            throw new ArgumentException("NetworkFee cannot be negative", nameof(options));
    }
}

/// <summary>
/// Options for building a transaction
/// </summary>
public class TransactionBuildOptions
{
    /// <summary>
    /// Transaction sender account
    /// </summary>
    public UInt160 Sender { get; set; } = UInt160.Zero;

    /// <summary>
    /// Witness scope for the sender
    /// </summary>
    public WitnessScope SenderScope { get; set; } = WitnessScope.CalledByEntry;

    /// <summary>
    /// Additional signers for the transaction
    /// </summary>
    public Signer[]? AdditionalSigners { get; set; }

    /// <summary>
    /// Transaction script
    /// </summary>
    public byte[] Script { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// System fee in GAS (for execution)
    /// </summary>
    public long SystemFee { get; set; }

    /// <summary>
    /// Network fee in GAS (for transaction size and verification)
    /// </summary>
    public long NetworkFee { get; set; }

    /// <summary>
    /// Block height until which the transaction is valid
    /// </summary>
    public uint ValidUntilBlock { get; set; }

    /// <summary>
    /// Transaction nonce (random if not specified)
    /// </summary>
    public uint? Nonce { get; set; }

    /// <summary>
    /// Additional transaction attributes
    /// </summary>
    public TransactionAttribute[]? Attributes { get; set; }
}
