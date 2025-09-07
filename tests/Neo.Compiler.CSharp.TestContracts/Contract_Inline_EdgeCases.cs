using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Inline_EdgeCases : SmartContract.Framework.SmartContract
    {
        // Test 1: Parameter shadowing issue - inline method parameter might shadow caller's variable
        public static int TestParameterShadowing()
        {
            int value = 10;
            int result = InlineWithSameParamName(5);
            // If inlining is broken, 'value' might be incorrectly modified
            return value + result; // Should return 10 + 5 = 15
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineWithSameParamName(int value)
        {
            return value; // This 'value' should be the parameter, not the caller's variable
        }

        // Test 2: Multiple calls to same inline method
        public static int TestMultipleCalls()
        {
            int a = InlineAdd(1, 2);  // 3
            int b = InlineAdd(3, 4);  // 7
            int c = InlineAdd(a, b);  // 10
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineAdd(int x, int y)
        {
            return x + y;
        }

        // Test 3: Inline method with local variables
        public static int TestLocalVariables()
        {
            return InlineWithLocals(5, 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineWithLocals(int a, int b)
        {
            int temp1 = a * 2;  // 10
            int temp2 = b * 3;  // 9
            int result = temp1 + temp2; // 19
            return result;
        }

        // Test 4: Nested inline calls
        public static int TestNestedInline()
        {
            return InlineOuter(5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineOuter(int x)
        {
            return InlineInner(x * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineInner(int y)
        {
            return y + 1;
        }

        // Test 5: Inline with conditional logic
        public static int TestConditionalInline(bool flag)
        {
            return InlineConditional(flag, 10, 20);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineConditional(bool condition, int a, int b)
        {
            if (condition)
                return a;
            else
                return b;
        }

        // Test 6: Inline void method with side effects
        private static int counter = 0;

        public static int TestVoidInline()
        {
            InlineVoidMethod();
            InlineVoidMethod();
            return counter; // Should be 2
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InlineVoidMethod()
        {
            counter++;
        }

        // Test 7: Inline with expression body that returns value in void context
        public static void TestExpressionBodyVoid()
        {
            InlineExpressionVoid(5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InlineExpressionVoid(int x) => counter = x + 1;

        // Test 8: Inline with expression body that returns value
        public static int TestExpressionBodyReturn()
        {
            return InlineExpressionReturn(7, 3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineExpressionReturn(int a, int b) => a * b;

        // Test 9: Parameter order with different calling conventions
        public static int TestParameterOrder()
        {
            return InlineParamOrder(1, 2, 3, 4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineParamOrder(int a, int b, int c, int d)
        {
            return a * 1000 + b * 100 + c * 10 + d; // Should return 1234
        }

        // Test 10: Inline with out parameters (should this even work?)
        public static int TestOutParameter()
        {
            InlineWithOut(5, out int result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InlineWithOut(int input, out int output)
        {
            output = input * 2;
        }

        // Test 11: Recursive inline (should probably not inline)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineRecursive(int n)
        {
            if (n <= 1) return 1;
            return n * InlineRecursive(n - 1); // Recursive call
        }

        public static int TestRecursiveInline()
        {
            return InlineRecursive(5); // Should return 120 (5!)
        }

        // Test 12: Inline method calling non-inline method
        public static int TestInlineCallingNonInline()
        {
            return InlineWrapper(10);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineWrapper(int x)
        {
            return NonInlineMethod(x);
        }

        private static int NonInlineMethod(int y)
        {
            return y * 2;
        }

        // Test 13: Check stack handling with complex expressions
        public static int TestComplexExpression()
        {
            int x = 5;
            int y = 10;
            // Complex expression with multiple inline calls
            return InlineAdd(InlineMultiply(x, 2), InlineAdd(y, InlineMultiply(3, 4)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InlineMultiply(int a, int b)
        {
            return a * b;
        }
    }
}