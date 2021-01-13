using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Iterator : SmartContract.Framework.SmartContract
    {
        public static int TestNextByteArray(byte[] a)
        {
            int sum = 0;
            var iterator = Iterator.Create<byte>(a);

            while (iterator.Next())
            {
                sum += iterator.Value;
            }

            return sum;
        }

        public static int TestNextIntArray(int[] a)
        {
            int sum = 0;
            var iterator = Iterator.Create<int>(a);

            while (iterator.Next())
            {
                sum += iterator.Value;
            }

            return sum;
        }
    }
}
