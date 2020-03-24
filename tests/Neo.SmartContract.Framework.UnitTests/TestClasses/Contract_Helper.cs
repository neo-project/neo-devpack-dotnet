using Neo.SmartContract.Framework;

namespace Compiler.MSIL.TestClasses
{
    public class Contract_Helper : SmartContract
    {
        static readonly byte[] data = "0a0b0c0d0E0F".HexToBytes();

        public static byte[] TestHexToBytes()
        {
            return data;
        }

        public static int AssertCall(bool value)
        {
            Assert(value == true);
            return 5;
        }

        public static void VoidAssertCall(bool value)
        {
            Assert(value == true);
        }
    }
}
