using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Iterator : SmartContract.Framework.SmartContract
    {
        public static int TestNextArray(byte[] a)
        {
            int sum = 0;
            var iterator = Iterator<byte, byte>.Create(a);

            while (iterator.Next())
            {
                sum += iterator.Value;
            }

            return sum;
        }

        public static int TestConcatArray(byte[] a, byte[] b)
        {
            int sum = 0;
            var iteratorA = Iterator<byte, byte>.Create(a);
            var iteratorB = Iterator<byte, byte>.Create(b);
            var iteratorC = iteratorA.Concat(iteratorB);

            while (iteratorC.Next())
            {
                sum += iteratorC.Value;
            }

            return sum;
        }

        public static int TestConcatMap(Map<byte, byte> a, Map<byte, byte> b)
        {
            int sum = 0;
            var iteratorA = Iterator<byte, byte>.Create(a);
            var iteratorB = Iterator<byte, byte>.Create(b);
            var iteratorC = iteratorA.Concat(iteratorB);

            while (iteratorC.Next())
            {
                sum += iteratorC.Key;
                sum += iteratorC.Value;
            }

            return sum;
        }

        public static int TestConcatKeys(Map<byte, byte> a, Map<byte, byte> b)
        {
            int sum = 0;
            var iteratorA = Iterator<byte, byte>.Create(a);
            var iteratorB = Iterator<byte, byte>.Create(b);
            var iteratorC = iteratorA.Concat(iteratorB);
            var enumerator = iteratorC.Keys;

            while (enumerator.Next())
            {
                sum += enumerator.Value;
            }

            return sum;
        }

        public static int TestConcatValues(Map<byte, byte> a, Map<byte, byte> b)
        {
            int sum = 0;
            var iteratorA = Iterator<byte, byte>.Create(a);
            var iteratorB = Iterator<byte, byte>.Create(b);
            var iteratorC = iteratorA.Concat(iteratorB);
            var enumerator = iteratorC.Values;

            while (enumerator.Next())
            {
                sum += enumerator.Value;
            }

            return sum;
        }
    }
}
