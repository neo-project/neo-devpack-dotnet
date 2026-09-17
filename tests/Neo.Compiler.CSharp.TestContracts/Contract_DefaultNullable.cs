// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_DefaultNullable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_DefaultNullable : SmartContract.Framework.SmartContract
    {
        public static bool TestDefaultLiteral()
        {
            int? x = default;
            return x == null;
        }

        public static bool TestDefaultExpression()
        {
            int? x = default(int?);
            return x == null;
        }

        public static int TestNullCoalescing()
        {
            int? x = default;
            return x ?? 42;
        }

        public static int TestNullCoalescingExpression()
        {
            int? x = default(int?);
            return x ?? 42;
        }

        public static bool TestHasValue()
        {
            int? x = default;
            return x.HasValue;
        }

        public static bool TestHasValueExpression()
        {
            int? x = default(int?);
            return x.HasValue;
        }

        public static int? TestReturnDefault() => default;

        public static int? TestReturnDefaultExpression() => default(int?);

        public static bool TestComparison(int? other)
        {
            int? x = default;
            return x == other;
        }

        public static bool TestComparisonExpression(int? other)
        {
            int? x = default(int?);
            return x == other;
        }

        public static bool TestNotEqualNull()
        {
            int? x = default;
            return x != null;
        }

        public static bool TestNotEqualNullExpression()
        {
            int? x = default(int?);
            return x != null;
        }
    }
}
