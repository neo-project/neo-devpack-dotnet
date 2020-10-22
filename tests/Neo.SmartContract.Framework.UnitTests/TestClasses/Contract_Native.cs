using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;
using System.Numerics;
using System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Native : SmartContract.Framework.SmartContract
    {
        [DisplayName("NEO_Decimals")]
        public static int NEO_Decimals()
        {
            return NEO.Decimals;
        }

        [DisplayName("NEO_Name")]
        public static string NEO_Name()
        {
            return NEO.Name;
        }

        [DisplayName("NEO_Transfer")]
        public static bool NEO_Transfer(byte[] from, byte[] to, BigInteger amount)
        {
            return NEO.Transfer(from, to, amount);
        }

        [DisplayName("NEO_BalanceOf")]
        public static BigInteger NEO_BalanceOf(byte[] account)
        {
            return NEO.BalanceOf(account);
        }

        [DisplayName("NEO_GetGasPerBlock")]
        public static BigInteger NEO_GetGasPerBlock()
        {
            return NEO.GetGasPerBlock();
        }

        [DisplayName("NEO_UnclaimedGas")]
        public static BigInteger NEO_UnclaimedGas(byte[] account, uint end)
        {
            return NEO.UnclaimedGas(account, end);
        }

        [DisplayName("NEO_RegisterCandidate")]
        public static bool NEO_RegisterCandidate(byte[] pubkey)
        {
            return NEO.RegisterCandidate(pubkey);
        }

        [DisplayName("NEO_GetCandidates")]
        public static (string, BigInteger)[] NEO_GetCandidates()
        {
            return NEO.GetCandidates();
        }

        [DisplayName("GAS_Decimals")]
        public static int GAS_Decimals()
        {
            return GAS.Decimals;
        }

        [DisplayName("GAS_Name")]
        public static string GAS_Name()
        {
            return GAS.Name;
        }

        [DisplayName("Policy_GetFeePerByte")]
        public static BigInteger Policy_GetFeePerByte()
        {
            return Policy.GetFeePerByte();
        }

        [DisplayName("Policy_GetMaxTransactionsPerBlock")]
        public static uint Policy_GetMaxTransactionsPerBlock()
        {
            return Policy.GetMaxTransactionsPerBlock();
        }

        public static bool Policy_IsBlocked(byte[] account)
        {
            return Policy.IsBlocked(account);
        }
    }
}
