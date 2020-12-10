using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;
using System.Numerics;
using System;
using Neo.Cryptography.ECC;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Native : SmartContract.Framework.SmartContract
    {
        [DisplayName("NEO_Decimals")]
        public static int NEO_Decimals()
        {
            return NEO.Decimals;
        }

        [DisplayName("NEO_Transfer")]
        public static bool NEO_Transfer(UInt160 from, UInt160 to, BigInteger amount)
        {
            return NEO.Transfer(from, to, amount, null);
        }

        [DisplayName("NEO_BalanceOf")]
        public static BigInteger NEO_BalanceOf(UInt160 account)
        {
            return NEO.BalanceOf(account);
        }

        [DisplayName("NEO_GetGasPerBlock")]
        public static BigInteger NEO_GetGasPerBlock()
        {
            return NEO.GetGasPerBlock();
        }

        [DisplayName("NEO_UnclaimedGas")]
        public static BigInteger NEO_UnclaimedGas(UInt160 account, uint end)
        {
            return NEO.UnclaimedGas(account, end);
        }

        [DisplayName("NEO_RegisterCandidate")]
        public static bool NEO_RegisterCandidate(ECPoint pubkey)
        {
            return NEO.RegisterCandidate(pubkey);
        }

        [DisplayName("NEO_GetCandidates")]
        public static (ECPoint, BigInteger)[] NEO_GetCandidates()
        {
            return NEO.GetCandidates();
        }

        [DisplayName("GAS_Decimals")]
        public static int GAS_Decimals()
        {
            return GAS.Decimals;
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

        [DisplayName("Policy_IsBlocked")]
        public static object[] Policy_IsBlocked(UInt160 account)
        {
            return Policy.IsBlocked(account);
        }
    }
}
