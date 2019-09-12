using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        #region Token Settings
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Name() => "Token Name";
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Symbol() => "TokenSymbol";
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Decimals() => 8;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong MaxSupply() => 1_000_000_000;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong InitialSupply() => 20_000_000;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] SupportedStandards() => new string[] { "NEP-5", "NEP-10" };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] Owner() => "ARx15Bf5VKzmUHYPC48qcrcqiHssbqSvJK".ToScriptHash();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong TokensPerNEO() => 1_000_000_000;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong TokensPerGAS() => 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] NeoToken() => "AHm6aEDPgu7WnmsEFDmCQTJwgZiAcXWNtu".ToScriptHash();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GasToken() => "AKCDN4viZXkCJPTg2jarHA37i1UhtBH2EC".ToScriptHash();
        #endregion

        #region Notifications
        [DisplayName("Transfer")]
        public static event Action<byte[], byte[], BigInteger> OnTransfer;
        #endregion

        #region Storage key prefixes
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] GetStoragePrefixBalance() => new byte[] { 0x01, 0x01 };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] GetStoragePrefixContract() => new byte[] { 0x02, 0x02 };
        #endregion

        public static object Main(string operation, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return Runtime.CheckWitness(Owner());
            }

            else if (Runtime.Trigger == TriggerType.Application)
            {
                #region NEP5 METHODS
                if (operation == "name") return Name();
                if (operation == "symbol") return Symbol();
                if (operation == "decimals") return Decimals();
                if (operation == "totalSupply") return TotalSupply();
                if (operation == "balanceOf") return BalanceOf((byte[])args[0]);
                if (operation == "transfer") return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                #endregion

                #region NEP10 METHODS
                if (operation == "supportedStandards") return SupportedStandards();
                #endregion

                #region CROWDSALE METHODS
                if (operation == "mint") return Mint();
                #endregion

                #region ADMIN METHODS
                if (operation == "deploy") return Deploy();
                if (operation == "migrate") return Migrate((byte[])args[1], (ContractPropertyState)args[2]);
                if (operation == "destroy") return Destroy();
                #endregion
            }
            return false;
        }
    }
}
