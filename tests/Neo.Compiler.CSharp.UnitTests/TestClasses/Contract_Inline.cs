using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Inline : SmartContract.Framework.SmartContract
    {
        public static object TestInline(string method)
        {
            return method switch
            {
                "inline" => inline(),
                "inline_with_one_parameters" => inline_with_one_parameters(3),
                "inline_with_multi_parameters" => inline_with_multi_parameters(2, 3),
                "not_inline" => not_inline(),
                "not_inline_with_one_parameters" => not_inline_with_one_parameters(3),
                "not_inline_with_multi_parameters" => not_inline_with_multi_parameters(2, 3),
                "inline_nested"=>inline_A(),
                _ => 99
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int inline()
        {
            return 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int inline_with_one_parameters(int a)
        {
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int inline_with_multi_parameters(int a, int b)
        {
            return a + b;
        }


        private static int not_inline()
        {
            return 1;
        }

        private static int not_inline_with_one_parameters(int a)
        {
            return a;
        }

        private static int not_inline_with_multi_parameters(int a, int b)
        {
            return a + b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int inline_A()
        {
            return not_inline_B();
        }

        private static int not_inline_B()
        {
            return inline_C();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int inline_C()
        {
            return 3;
        }

    }
}
