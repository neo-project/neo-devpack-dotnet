using Neo.Cryptography.ECC;
using Neo.SmartContract;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Types_ECPoint : SmartContract.Framework.SmartContract
    {
        [InitialValue("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", ContractParameterType.PublicKey)]
        private static readonly ECPoint publicKey2Ecpoint = default;

        public static bool isValid(ECPoint point) { return point.IsValid; }

        public static string ecpoint2String()
        {
            return (ByteString)publicKey2Ecpoint;
        }

        public static ECPoint ecpointReturn()
        {
            return publicKey2Ecpoint;
        }

        public static object ecpoint2ByteArray()
        {
            return (byte[])publicKey2Ecpoint;
        }
    }
}
