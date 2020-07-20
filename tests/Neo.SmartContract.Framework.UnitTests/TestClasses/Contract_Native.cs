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
            return (int)NEO.Decimals();
        }

        [DisplayName("NEO_Name")]
        public static string NEO_Name()
        {
            return NEO.Name();
        }

        [DisplayName("NEO_BalanceOf")]
        public static BigInteger NEO_BalanceOf(byte[] account)
        {
            return NEO.BalanceOf(account);
        }

        [DisplayName("NEO_GetValidators")]
        public static string[] NEO_GetValidators()
        {
            return NEO.GetValidators();
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
            return (int)GAS.Decimals();
        }

        [DisplayName("GAS_Name")]
        public static string GAS_Name()
        {
            return GAS.Name();
        }

        public static BigInteger Policy_GetFeePerByte()
        {
            return Policy.GetFeePerByte();
        }

        public static bool Policy_SetMaxTransactionsPerBlock(uint value)
        {
            return Policy.SetMaxTransactionsPerBlock(value);
        }

        public static uint Policy_GetMaxTransactionsPerBlock()
        {
            return Policy.GetMaxTransactionsPerBlock();
        }

        public static bool Policy_BlockAccount(byte[] account)
        {
            return Policy.BlockAccount(account);
        }

        public static object[] Policy_GetBlockedAccounts()
        {
            return Policy.GetBlockedAccounts();
        }
    }
}
