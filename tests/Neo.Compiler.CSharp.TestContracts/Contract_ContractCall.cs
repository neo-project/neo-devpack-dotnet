using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [Contract("0x6c0d791bc227496fc099715890d61d92e7c5d737")]
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
