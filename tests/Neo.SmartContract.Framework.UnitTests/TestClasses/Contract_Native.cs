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
            return (int)Native.NEO("decimals", new object[0]);
        }

        [DisplayName("NEO_Name")]
        public static string NEO_Name()
        {
            return (string)Native.NEO("name", new object[0]);
        }

        [DisplayName("GAS_Decimals")]
        public static int GAS_Decimals()
        {
            return (int)Native.GAS("decimals", new object[0]);
        }

        [DisplayName("GAS_Name")]
        public static string GAS_Name()
        {
            return (string)Native.GAS("name", new object[0]);
        }

        public static BigInteger Policy_GetFeePerByte()
        {
            return (BigInteger)Native.Policy("getFeePerByte", new object[0]);
        }
    }
}
