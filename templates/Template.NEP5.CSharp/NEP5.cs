using Neo;
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
        static readonly UInt160 Owner = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
        static readonly ulong TokensPerNEO = 1_000_000_000;
        static readonly ulong TokensPerGAS = 1;
        #endregion

        #region Notifications
        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger> OnTransfer;
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
