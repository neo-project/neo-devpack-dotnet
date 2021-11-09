using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    struct State
    {
        public byte[] from;
        public byte[] to;
        public BigInteger amount;
    }

    public class Contract_Array : SmartContract.Framework.SmartContract
    {
        public static int[][] TestJaggedArray()
        {
            int[] array1 = new int[] { 1, 2, 3, 4 };
            int[] array2 = new int[] { 5, 6, 7, 8 };
            int[] array3 = new int[] { 1, 3, 2, 1 };
            int[] array4 = new int[] { 5, 4, 3, 2 };
            return new int[][] { array1, array2, array3, array4 };
        }

        public static byte[][] TestJaggedByteArray()
        {
            byte[] array1 = new byte[] { 1, 2, 3, 4 };
            byte[] array2 = new byte[] { 5, 6, 7, 8 };
            byte[] array3 = new byte[] { 1, 3, 2, 1 };
            byte[] array4 = new byte[] { 5, 4, 3, 2 };
            return new byte[][] { array1, array2, array3, array4 };
        }

        public static object TestIntArray()
        {
            var arrobj = new int[3];
            arrobj[0] = 0;
            arrobj[1] = 1;
            arrobj[2] = 2;
            return arrobj;
        }

        public static object TestDefaultArray()
        {
            var arrobj = new int[3];
            if (arrobj[0] == 0) return true;
            return false;
        }

        public static object TestIntArrayInit()
        {
            var arrobj = new int[] { 1, 2, 3 };
            arrobj[1] = 4;
            arrobj[2] = 5;
            return arrobj;
        }

        public static object testIntArrayInit2()
        {
            int[] arrobj = { 1, 2, 3 };
            arrobj[1] = 4;
            arrobj[2] = 5;
            return arrobj;
        }

        public static object testIntArrayInit3()
        {
            int[] arrobj = new[] { 1, 2, 3 };
            arrobj[1] = 4;
            arrobj[2] = 5;
            return arrobj;
        }

        public static int[] TestDynamicArrayInit(int length)
        {
            var arrobj = new int[length];
            for (int x = 0; x < length; x++) arrobj[x] = x;
            return arrobj;
        }

        public static byte[] TestDynamicArrayStringInit(string input)
        {
            return new byte[input.Length];
        }

        public static object TestStructArray()
        {
            var s = new State();
            var sarray = new State[3];
            sarray[2] = s;
            return sarray[2];
        }

        public static object TestStructArrayInit()
        {
            var s = new State();
            State[] states = new State[] { s };
            for (var i = 0; i < 1; i++)
            {
                State state = states[i];
                return state;
            }
            return null;
        }

        static readonly byte[] OwnerVar = new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };
        private static byte[] Owner()
        {
            var bs = new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };
            return bs;
        }
        public static object TestByteArrayOwner()
        {
            return OwnerVar;
        }
        public static object TestByteArrayOwnerCall()
        {
            return Owner();
        }

        static readonly string[] SupportedStandards = new string[] { "NEP-5", "NEP-10" };

        public static object TestSupportedStandards()
        {
            return SupportedStandards;
        }
    }
}
