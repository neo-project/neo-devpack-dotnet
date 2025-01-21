// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Inline.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Inline : SmartContract.Framework.SmartContract
    {
        public static int TestInline(string method)
        {
            return method switch
            {
                "inline" => inline(),
                "inline_with_one_parameters" => inline_with_one_parameters(3),
                "inline_with_multi_parameters" => inline_with_multi_parameters(2, 3),
                "not_inline" => not_inline(),
                "not_inline_with_one_parameters" => not_inline_with_one_parameters(3),
                "not_inline_with_multi_parameters" => not_inline_with_multi_parameters(2, 3),
                "inline_nested" => inline_A(),
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

        public static int ArrowMethod()
        {
            return ArrowInline(1, 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ArrowInline(int a, int b) => a + b;


        public static void ArrowMethodNoRerurn()
        {
            ArrowInlineNoReturn(1, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ArrowInlineNoReturn(int a, int b) => CallMethodThatReturnsInt(a, b);


        private static int CallMethodThatReturnsInt(int a, int b)
        {
            return a + b;
        }
    }
}
