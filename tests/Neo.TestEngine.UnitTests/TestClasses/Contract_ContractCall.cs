using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.TestEngine.UnitTests.TestClasses
{
    [Contract("0xfaf5e0e43f3fdeb58cdae3c1630a1db650b05403")]
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
