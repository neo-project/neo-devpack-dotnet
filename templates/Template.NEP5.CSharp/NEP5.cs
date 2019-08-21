using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
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
        public static ulong MaxSupply() => 1000000000;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] SupportedStandards() => new string[] { "NEP-5", "NEP-10" };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] Owner() => "ARx15Bf5VKzmUHYPC48qcrcqiHssbqSvJK".ToScriptHash();

        #endregion

        #region Storage key prefixes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetStoragePrefixBalance() => "B_";
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetStoragePrefixContract() => "A_";

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

                #region NEP5.1 METHODS
                #endregion

                #region ADMIN METHODS
                if (operation == "deploy") return Deploy();
                #endregion
            }
            return false;
        }
    }
}