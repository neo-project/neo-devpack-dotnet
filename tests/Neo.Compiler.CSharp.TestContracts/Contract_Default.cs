using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Default : SmartContract.Framework.SmartContract
    {
        public static bool TestBooleanDefault()
        {
            bool a = default;
            return a;
        }

        public static byte TestByteDefault()
        {
            byte a = default;
            return a;
        }

        public static sbyte TestSByteDefault()
        {
            sbyte a = default;
            return a;
        }

        public static short TestInt16Default()
        {
            short a = default;
            return a;
        }

        public static ushort TestUInt16Default()
        {
            ushort a = default;
            return a;
        }

        public static int TestInt32Default()
        {
            int a = default;
            return a;
        }

        public static uint TestUInt32Default()
        {
            uint a = default;
            return a;
        }

        public static long TestInt64Default()
        {
            long a = default;
            return a;
        }

        public static ulong TestUInt64Default()
        {
            ulong a = default;
            return a;
        }

        public static char TestCharDefault()
        {
            char a = default;
            return a;
        }

#pragma warning disable CS8600,CS8603
        public static string TestStringDefault()
        {
            string a = default;
            return a;
        }

        public static object TestObjectDefault()
        {
            object a = default;
            return a;
        }
#pragma warning restore CS8600,CS8603

        public static BigInteger TestBigIntegerDefault()
        {
            BigInteger a = default;
            return a;
        }

        public static TestStruct TestStructDefault()
        {
            TestStruct a = default;
            return a;
        }

        public struct TestStruct
        {
            public int Value;
        }

#pragma warning disable CS8600,CS8603
        public static TestClass TestClassDefault()
        {
            TestClass a = default;
            return a;
        }
#pragma warning restore CS8600,CS8603

        public class TestClass
        {
            public int Value;
        }
    }
}
