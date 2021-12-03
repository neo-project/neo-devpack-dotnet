using System;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract;
using Neo.Cryptography.ECC;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Foreach : SmartContract.Framework.SmartContract
    {
        [InitialValue("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", ContractParameterType.ByteArray)]
        private static readonly byte[] rawECpoint = default;

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

        public static ByteString ByteStringForeach()
        {
            ByteString[] list = new ByteString[] { "abc", "def", "hij" };
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
            int i = 0;
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
                    if (breakIndex-- <= 0) break;
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
    }
}
