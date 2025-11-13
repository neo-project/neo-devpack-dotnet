using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Inline_Invalid : SmartContract.Framework.SmartContract
    {
        // Test 1: Recursive inline method - should fail compilation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RecursiveInline(int n)
        {
            if (n <= 1) return 1;
            return n * RecursiveInline(n - 1); // Recursive call should trigger error
        }

        // Test 2: Inline method with out parameter - should fail compilation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineWithOut(int input, out int output)
        {
            output = input * 2;
        }

        // Test 3: Inline method with ref parameter - should fail compilation
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InlineWithRef(ref int value)
        {
            value *= 2;
        }

        // Test 4: Large inline method - should generate warning but compile
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LargeInlineMethod(int x)
        {
            // Artificially large method with many operations
            int result = x;
            result = result * 2 + 1;
            result = result * 3 + 2;
            result = result * 4 + 3;
            result = result * 5 + 4;
            result = result * 6 + 5;
            result = result * 7 + 6;
            result = result * 8 + 7;
            result = result * 9 + 8;
            result = result * 10 + 9;
            result = result * 11 + 10;
            result = result * 12 + 11;
            result = result * 13 + 12;
            result = result * 14 + 13;
            result = result * 15 + 14;
            result = result * 16 + 15;
            result = result * 17 + 16;
            result = result * 18 + 17;
            result = result * 19 + 18;
            result = result * 20 + 19;
            return result;
        }

        // Entry points for testing
        public static int TestRecursive()
        {
            return RecursiveInline(5);
        }

        public static int TestOut()
        {
            InlineWithOut(5, out int result);
            return result;
        }

        public static int TestRef()
        {
            int value = 5;
            InlineWithRef(ref value);
            return value;
        }

        public static int TestLarge()
        {
            return LargeInlineMethod(1);
        }
    }
}