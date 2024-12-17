using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Foreach : SmartContract.Framework.SmartContract
    {
        [ByteArray("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly byte[] rawECpoint = default!;

        struct Pending
        {
            public ByteString user;
            public BigInteger amount;
        }

        public static int IntForeach()
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int sum = 0;
            foreach (var item in a)
            {
                sum += item;
            }
            return sum;
        }

        public static string StringForeach()
        {
            string[] list = new string[] { "abc", "def", "hij" };
            string result = "";
            foreach (var item in list)
            {
                result = result + item;
            }
            return result;
        }

        public static int ByteStringEmpty()
        {
            ByteString bytes = ByteString.Empty;
            return bytes.Length;
        }

        public static ByteString ByteStringForeach()
        {
            ByteString[] list = new ByteString[] { "abc", "def", "hij", ByteString.Empty, ByteString.Empty };
            string result = "";
            foreach (var item in list)
            {
                result = result + item;
            }
            return result;
        }

        public static Map<ByteString, BigInteger> StructForeach()
        {
            var a1 = new Pending();
            a1.user = "test1";
            a1.amount = 1;
            var a2 = new Pending();
            a2.user = "test2";
            a2.amount = 2;
            Pending[] list = new Pending[] { a1, a2 };
            Map<ByteString, BigInteger> result = new Map<ByteString, BigInteger>();
            foreach (var item in list)
            {
                result[item.user] = item.amount;
            }
            return result;
        }

        public static List<byte> ByteArrayForeach()
        {
            byte[] a_bytearray = new byte[] { 0x01, 0x0a, 0x11 };
            List<byte> result = new List<byte>();
            foreach (var item in a_bytearray)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<UInt160> UInt160Foreach()
        {
            UInt160[] test = new UInt160[] { UInt160.Zero, UInt160.Zero };
            List<UInt160> result = new List<UInt160>();
            foreach (var item in test)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<UInt256> UInt256Foreach()
        {
            UInt256[] test = new UInt256[] { UInt256.Zero, UInt256.Zero };
            List<UInt256> result = new List<UInt256>();
            foreach (var item in test)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<ECPoint> ECPointForeach()
        {
            ECPoint[] test = new ECPoint[] { (ECPoint)rawECpoint, (ECPoint)rawECpoint };
            List<ECPoint> result = new List<ECPoint>();
            foreach (var item in test)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<BigInteger> BigIntegerForeach()
        {
            BigInteger[] test = new BigInteger[] { 10_000, 1000_000, 1000_000_000, 1000_000_000_000_000_000 };
            List<BigInteger> result = new List<BigInteger>();
            foreach (var item in test)
            {
                result.Add(item);
            }
            return result;
        }

        public static List<object> ObjectArrayForeach()
        {
            object[] test = new object[] { new byte[] { 0x01, 0x02 }, "test", 123 };
            List<object> result = new List<object>();
            foreach (var item in test)
            {
                result.Add(item);
            }
            return result;
        }


        public static int IntForeachBreak(int breakIndex)
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int sum = 0;
            try
            {
                foreach (var item in a)
                {
                    if (breakIndex-- <= 0)
                        break;
                    sum += item;
                }
            }
            catch { }
            return sum;
        }

        public static int TestContinue()
        {
            int[] a = new int[] { 1, 2, 3, 4, 5 };
            int sum = 0;
            try
            {
                foreach (var item in a)
                {
                    if (item % 2 == 0)
                        continue;
                    sum += item;
                }
            }
            catch { }
            return sum;
        }

        public static int IntForloop()
        {
            int[] a = new int[] { 1, 2, 3, 4 };
            int sum = 0;
            int i;
            for (i = 0; i < a.Length; i++)
            {
                sum += a[i];
            }
            return sum;
        }

        public static void TestIteratorForEach()
        {
            var tokens = new StorageMap(Storage.CurrentContext, 3).Find(FindOptions.KeysOnly | FindOptions.RemovePrefix);
            foreach (var item in tokens)
            {
                Runtime.Log(item.ToString()!);
            }
        }

        static (int, string)[] Function1() => new (int, string)[] { new(1, "hello"), new(2, "world") };

        public static void TestForEachVariable()
        {

            foreach (var (a, b) in Function1())
            {
                Runtime.Log($"{a}: {b}");
            }
        }

        public static void TestDo()
        {
            int n = 0;
            do
            {
                Runtime.Log(n.ToString());
                n++;
            } while (n < 5);
        }

        public static void TestWhile()
        {
            int n = 0;
            while (n < 5)
            {
                Runtime.Log(n.ToString());
                n++;
            }
        }
    }
}
