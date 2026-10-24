// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Tuple.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    struct StructA
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public byte[] from;
        public byte[] to;
        public BigInteger amount;
        public BigInteger index;
        public StructB data;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    struct StructB
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public byte[] hash;
        public BigInteger height;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    public class Contract_Tuple : SmartContract.Framework.SmartContract
    {
        public static (BigInteger, BigInteger, BigInteger, BigInteger) GetResult()
        {
            return (1, 2, 3, 4);
        }

        public static BigInteger ReadTupleElements(BigInteger a, BigInteger b)
        {
            var t = (a, b, 3, 4, 5, 6, 7);
            return t.Item1 * 1000000 + t.Item3 * 10000 + t.Item4 * 1000 + t.Item7;
        }

        public static BigInteger WriteTupleElement(BigInteger a)
        {
            var t = (a, 2, 3, 4);
            t.Item4 = 99;
            return t.Item4;
        }

        public static BigInteger ReadTupleRest(BigInteger a)
        {
            var t = (a, 2, 3, 4, 5, 6, 7, 8);
            return t.Item8 * 1000 + t.Item5;
        }

        public static object T1()
        {
            var state = new StructA();
            state.data = new StructB();
            BigInteger index = 0;
            (state.amount, state.data.height, _, index) = GetResult();
            _ = GetResult();
            state.index = index;
            return state;
        }

        private sealed class TupleBox
        {
            public int X;
            public int Y;
            public int P { get; set; }
            public static int StaticField;
            public static int StaticProperty { get; set; }
        }

        private static int TupleNext(ref int state)
        {
            return ++state;
        }

        private static TupleBox TuplePick(TupleBox value, ref int state)
        {
            state++;
            return value;
        }

        public static int TupleAssignmentEvaluationOrder(int a)
        {
            var left = new TupleBox();
            var right = new TupleBox();
            int state = a;
            (TuplePick(left, ref state).X, _, TuplePick(right, ref state).Y) =
                (TupleNext(ref state), TupleNext(ref state), TupleNext(ref state));
            return left.X * 100000 + right.Y * 10000 + state;
        }

        public static int TuplePropertyAndStaticAssignment(int a)
        {
            var left = new TupleBox();
            int state = a;
            (TuplePick(left, ref state).X, TuplePick(left, ref state).P,
             TupleBox.StaticField, TupleBox.StaticProperty) =
                (TupleNext(ref state), TupleNext(ref state),
                 TupleNext(ref state), TupleNext(ref state));
            return left.X * 100000 + left.P * 10000
                + TupleBox.StaticField * 1000 + TupleBox.StaticProperty * 100 + state;
        }
    }
}
