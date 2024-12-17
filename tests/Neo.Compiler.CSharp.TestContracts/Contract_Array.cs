using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    internal struct State
    {
        public byte[]? from = default;
        public byte[] to = null!;
        public BigInteger amount = default;
        public State()
        {
        }
    }

    public class Contract_Array : SmartContract.Framework.SmartContract
    {
        // Does NOT work:
        private static readonly byte[] TreeByteLengthPrefix = [0x01, 0x03];

        // Works:
        private static readonly byte[] TreeByteLengthPrefix2 = new byte[] { 0x01, 0x03 };

        public static byte[] GetTreeByteLengthPrefix()
        {
            return TreeByteLengthPrefix;
        }

        public static byte[] GetTreeByteLengthPrefix2()
        {
            return TreeByteLengthPrefix2;
        }

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

        public static int[] TestIntArray()
        {
            var arrobj = new int[3];
            arrobj[0] = 0;
            arrobj[1] = 1;
            arrobj[2] = 2;
            return arrobj;
        }

        public static bool TestDefaultArray()
        {
            var arrobj = new int[3];
            if (arrobj[0] == 0) return true;
            return false;
        }

        public static int[] TestIntArrayInit()
        {
            var arrobj = new int[] { 1, 2, 3 };
            arrobj[1] = 4;
            arrobj[2] = 5;
            return arrobj;
        }

        public static int[] testIntArrayInit2()
        {
            int[] arrobj = { 1, 2, 3 };
            arrobj[1] = 4;
            arrobj[2] = 5;
            return arrobj;
        }

        public static int[] testIntArrayInit3()
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

        public static object TestDefaultState()
        {
            var s = new State();
            return s;
        }

        public static object[] TestEmptyArray()
        {
            var sarray = Array.Empty<object>();
            return sarray;
        }

        public static object TestStructArrayInit()
        {
            var s = new State();
            State[] states = [s];
            var state = states[0];
            return state;
        }

        static readonly byte[] OwnerVar = new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };
        private static byte[] Owner()
        {
            var bs = new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };
            return bs;
        }
        public static byte[] TestByteArrayOwner()
        {
            return OwnerVar;
        }
        public static byte[] TestByteArrayOwnerCall()
        {
            return Owner();
        }

        static readonly string[] SupportedStandards = new string[] { "NEP-5", "NEP-10" };

        public static string[] TestSupportedStandards()
        {
            return SupportedStandards;
        }

        public static void TestElementBinding()
        {
            var a = Ledger.GetBlock(0);
            var array = new[] { a };
            var firstItem = array?[0];
            Runtime.Log(firstItem?.Timestamp.ToString()!);
        }

        // This is new language feature introduced in C# 12
        // ref. https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12#inline-arrays
        public static (int[], List<string>, int[][], int[][]) TestCollectionexpressions()
        {
            // Create an array:
            int[] a = [1, 2, 3, 4, 5, 6, 7, 8];

            // Create a list:
            List<string> b = ["one", "two", "three"];

            // Create a jagged 2D array:
            int[][] twoD = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];

            // Create a jagged 2D array from variables:
            int[] row0 = [1, 2, 3];
            int[] row1 = [4, 5, 6];
            int[] row2 = [7, 8, 9];
            int[][] twoDFromVariables = [row0, row1, row2];

            return (a, b, twoD, twoDFromVariables);
        }
    }
}
