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
        private static byte[] Owner() => new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong TokensPerNEO() => 1_000_000_000;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong TokensPerGAS() => 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] NeoToken() => new byte[] { 0x9b, 0xde, 0x8f, 0x20, 0x9c, 0x88, 0xdd, 0x0e, 0x7c, 0xa3, 0xbf, 0x0a, 0xf0, 0xf4, 0x76, 0xcd, 0xd8, 0x20, 0x77, 0x89 };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] GasToken() => new byte[] { 0x8c, 0x23, 0xf1, 0x96, 0xd8, 0xa1, 0xbf, 0xd1, 0x03, 0xa9, 0xdc, 0xb1, 0xf9, 0xcc, 0xf0, 0xc6, 0x11, 0x37, 0x7d, 0x3b };
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
                if (operation == "migrate") return Migrate((byte[])args[1], (string)args[2]);
                if (operation == "destroy") return Destroy();
                #endregion
            }
            return false;
        }
    }
}
