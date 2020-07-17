using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Native : SmartContract.Framework.SmartContract
    {
        [DisplayName("NEO_Decimals")]
        public static int NEO_Decimals()
        {
            return (int)NEO.decimals();
        }

        [DisplayName("NEO_Name")]
        public static string NEO_Name()
        {
            return NEO.name();
        }

        [DisplayName("NEO_BalanceOf")]
        public static BigInteger NEO_BalanceOf(byte[] account)
        {
            return NEO.balanceOf(account);
        }       

        [DisplayName("GAS_Decimals")]
        public static int GAS_Decimals()
        {
            return (int)GAS.decimals();
        }

        [DisplayName("GAS_Name")]
        public static string GAS_Name()
        {
            return GAS.name();
        }

        public static BigInteger Policy_GetFeePerByte()
        {
            return Policy.getFeePerByte();
        }

        public static bool Policy_SetMaxTransactionsPerBlock(uint value)
        {
            return Policy.setMaxTransactionsPerBlock(value);
        }

        public static uint Policy_GetMaxTransactionsPerBlock()
        {
            return Policy.getMaxTransactionsPerBlock();
        }
    }
}
