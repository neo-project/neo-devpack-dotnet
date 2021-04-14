using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    [Contract("0102030405060708090A0102030405060708090A")]
    public class Contract1
    {
        public static extern byte[] testArgs1(byte a);
        public static extern void testVoid();
    }

    public class Contract_ContractCall : SmartContract.Framework.SmartContract
    {
        public static byte[] testContractCall()
        {
            return Contract1.testArgs1((byte)4);
        }

        public static void testContractCallVoid()
        {
            Contract1.testVoid();
        }
    }
}
