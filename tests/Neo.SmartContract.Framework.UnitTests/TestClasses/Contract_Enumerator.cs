using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Enumerator : SmartContract.Framework.SmartContract
    {
        public static int TestNext(byte[] a)
        {
            int sum = 0;
            var enumerator = Enumerator<byte>.Create(a);

            while (enumerator.Next())
            {
                sum += enumerator.Value;
            }

            return sum;
        }

        public static int TestConcat(byte[] a, byte[] b)
        {
            int sum = 0;
            var enumeratorA = Enumerator<byte>.Create(a);
            var enumeratorB = Enumerator<byte>.Create(b);
            var enumeratorC = enumeratorA.Concat(enumeratorB);

            while (enumeratorC.Next())
            {
                sum += enumeratorC.Value;
            }

            return sum;
        }
    }
}
