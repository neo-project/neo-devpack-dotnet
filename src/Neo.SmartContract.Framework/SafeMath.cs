// Copyright (C) 2015-2026 The Neo Project.
//
// SafeMath.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// Provides unsigned-style arithmetic helpers for <see cref="BigInteger"/> that fault on
    /// negative inputs, underflow, and division/modulo by zero.
    /// </summary>
    public static class SafeMath
    {
        public static BigInteger Add(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "NEGATIVE");
            return left + right;
        }

        public static BigInteger Sub(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "NEGATIVE");
            ExecutionEngine.Assert(left >= right, "UNDERFLOW");
            return left - right;
        }

        public static BigInteger Mul(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "NEGATIVE");
            return left * right;
        }

        public static BigInteger Div(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "NEGATIVE");
            ExecutionEngine.Assert(right != 0, "DIV_BY_ZERO");
            return left / right;
        }

        public static BigInteger Mod(BigInteger left, BigInteger right)
        {
            ExecutionEngine.Assert(left >= 0 && right >= 0, "NEGATIVE");
            ExecutionEngine.Assert(right != 0, "DIV_BY_ZERO");
            return left % right;
        }
    }
}
