using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    [Contract("9458d707d90e8e2838e488da0386194a0491bcb9")]
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
