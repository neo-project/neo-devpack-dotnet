using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Template.NEP5.CSharp
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "This is a NEP5 example")]
    [SupportedStandards("NEP5", "NEP10")]
    [Features(ContractFeatures.HasStorage | ContractFeatures.Payable)]
    public partial class NEP5 : SmartContract
    {
        #region Token Settings
        static readonly ulong MaxSupply = 10_000_000_000_000_000;
        static readonly ulong InitialSupply = 2_000_000_000_000_000;
        static readonly byte[] Owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
        static readonly ulong TokensPerNEO = 1_000_000_000;
        static readonly ulong TokensPerGAS = 1;
        static readonly byte[] NeoToken = "0xde5f57d430d3dece511cf975a8d37848cb9e0525".HexToBytes();
        static readonly byte[] GasToken = "0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc".HexToBytes();
        #endregion

        #region Notifications
        [DisplayName("Transfer")]
        public static event Action<byte[], byte[], BigInteger> OnTransfer;
        #endregion

        // When this contract address is included in the transaction signature,
        // this method will be triggered as a VerificationTrigger to verify that the signature is correct.
        // For example, this method needs to be called when withdrawing token from the contract.
        public static bool Verify() => IsOwner();

        public static string Name() => "Token Name";

        public static string Symbol() => "TokenSymbol";

        public static ulong Decimals() => 8;
    }
}
