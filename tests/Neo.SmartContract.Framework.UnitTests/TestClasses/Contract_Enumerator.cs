using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Enumerator : SmartContract.Framework.SmartContract
    {
        public static int TestNextByteArray(byte[] a)
        {
            int sum = 0;
            var enumerator = Enumerator<byte>.Create(a);

            while (enumerator.Next())
            {
                sum += enumerator.Value;
            }

            return sum;
        }

        public static int TestNextIntArray(int[] a)
        {
            int sum = 0;
            var enumerator = Enumerator<int>.Create(a);

            while (enumerator.Next())
            {
                sum += enumerator.Value;
            }

            return sum;
        }

        public static Enumerator<int> TestIntEnumerator()
        {
            var enumerator = Enumerator<int>.Create(new int[3] { 4, 6, 8 });
            return enumerator;
        }
    }
}
