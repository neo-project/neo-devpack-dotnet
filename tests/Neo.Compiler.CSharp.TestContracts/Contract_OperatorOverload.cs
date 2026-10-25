// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_OperatorOverload.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

// The operator bodies deliberately compare a subset of the fields; the structural
// Equals/GetHashCode overrides that C# conventionally pairs with them are not what is
// under test here.
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_OperatorOverload : SmartContract.Framework.SmartContract
{
    public struct Vec
    {
        public int X;
        public int Y;

        public Vec(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Vec a, Vec b)
        {
            return a.X == b.X;
        }

        public static bool operator !=(Vec a, Vec b)
        {
            return a.X != b.X;
        }

        public static bool operator <(Vec a, Vec b)
        {
            return a.X < b.X;
        }

        public static bool operator >(Vec a, Vec b)
        {
            return a.X > b.X;
        }

        public static Vec operator +(Vec a, Vec b)
        {
            return new Vec(a.X + b.X, a.Y + b.Y);
        }

        public static Vec operator -(Vec a, Vec b)
        {
            return new Vec(a.X - b.X, a.Y - b.Y);
        }

        public static implicit operator Vec(int x)
        {
            return new Vec(x, x + 1);
        }

        public static explicit operator int(Vec v)
        {
            return v.X * 1000 + v.Y;
        }
    }

    /// <summary>
    /// The equality operators must call the operator body (X only), not compare
    /// the struct representations structurally. Expected: (7,3)==(7,9) is true
    /// and (7,3)!=(7,9) is false, so the result is 1.
    /// </summary>
    public static int EqualityUsesOperatorBody()
    {
        Vec p = new Vec(7, 3);
        Vec q = new Vec(7, 9);
        return (p == q ? 1 : 0) + (p != q ? 2 : 0);
    }

    /// <summary>Expected: 8 != 7 in X, so the result is 1.</summary>
    public static int InequalityUsesOperatorBody()
    {
        Vec p = new Vec(7, 3);
        Vec q = new Vec(8, 3);
        return (p != q ? 1 : 0) + (p == q ? 2 : 0);
    }

    /// <summary>Expected: 7 &lt; 8 in X, so the result is 1.</summary>
    public static int RelationalUsesOperatorBody()
    {
        Vec p = new Vec(7, 9);
        Vec q = new Vec(8, 1);
        return p < q ? 1 : (p > q ? 2 : 3);
    }

    /// <summary>Expected: (8,5) and (6,1) combine to 866.</summary>
    public static int ArithmeticUsesOperatorBody()
    {
        Vec p = new Vec(7, 3);
        Vec q = new Vec(1, 2);
        Vec sum = p + q;
        Vec diff = p - q;
        return sum.X * 100 + sum.Y + diff.X * 10 + diff.Y;
    }

    /// <summary>
    /// The implicit conversion must call the operator body, producing Vec(7,8).
    /// Expected: 708.
    /// </summary>
    public static int ImplicitConversionCallsOperator()
    {
        Vec v = 7;
        return v.X * 100 + v.Y;
    }

    /// <summary>Expected: 7003.</summary>
    public static int ExplicitConversionCallsOperator()
    {
        Vec v = new Vec(7, 3);
        return (int)v;
    }

    /// <summary>Expected: Vec(8,9) converts to 8009 and compares equal in X, so 8010.</summary>
    public static int ChainedOperators()
    {
        Vec v = 7;
        v = v + new Vec(1, 1);
        return (int)v + (v == new Vec(8, 9) ? 1 : 0);
    }
}
