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

        public static int AssertExtension(bool value)
        {
            (value == true).Assert();
            return 5;
        }

        public static void VoidAssertExtension(bool value)
        {
            (value == true).Assert();
        }

        //public static void AssertCall(bool value)
        //{
        //    Assert((value == true));
        //}
    }
}
