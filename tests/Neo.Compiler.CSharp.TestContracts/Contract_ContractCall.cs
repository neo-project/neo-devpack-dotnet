using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [Contract("0xec0d3d1d7228546998091a9c729d4e6ac693b5a7")]
    public class Contract_Call
    {
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        public static extern byte[] testArgs1(byte a);
        public static extern void testVoid();
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
    }

    public class Contract_ContractCall : SmartContract.Framework.SmartContract
    {
        public static byte[] testContractCall()
        {
            return Contract_Call.testArgs1(4);
        }

        public static void testContractCallVoid()
        {
            Contract_Call.testVoid();
        }
    }
}
