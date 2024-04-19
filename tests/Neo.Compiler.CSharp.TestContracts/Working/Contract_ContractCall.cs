using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    [Contract("0102030405060708090A0102030405060708090A")]
    public class Contract_Call
    {
        public static extern byte[] testArgs1(byte a);
        public static extern void testVoid();
    }

    public class Contract_ContractCall : SmartContract.Framework.SmartContract
    {
        public static byte[] testContractCall()
        {
            return Contract_Call.testArgs1((byte)4);
        }

        public static void testContractCallVoid()
        {
            Contract_Call.testVoid();
        }
    }
}
