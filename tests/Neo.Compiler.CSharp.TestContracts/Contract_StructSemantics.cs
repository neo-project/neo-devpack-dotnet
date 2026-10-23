// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_StructSemantics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    /// <summary>
    /// Covers the value semantics of structs. A struct is stored as an array of its fields, so every
    /// place that stores a struct has to store a copy of that array.
    /// </summary>
    public class Contract_StructSemantics : SmartContract.Framework.SmartContract
    {
        public struct Point
        {
            public int X;
            public int Y;
        }

        public struct Segment
        {
            public Point From;
            public Point To;
        }

        public class Holder
        {
            public Point Value;
        }

        private static int Overwrite(Point point)
        {
            point.X = 99;
            return point.X;
        }

        /// <summary>
        /// A struct read into a local variable must not stay attached to the source variable.
        /// </summary>
        public static int CopyOnLocalDeclaration(int a)
        {
            var p = new Point { X = a, Y = a + 1 };
            var q = p;
            q.X = 99;
            return p.X * 1000 + q.X;
        }

        /// <summary>
        /// Same requirement for an assignment expression rather than a declaration.
        /// </summary>
        public static int CopyOnAssignment(int a)
        {
            var p = new Point { X = a, Y = a + 1 };
            var q = new Point();
            q = p;
            q.X = 99;
            return p.X * 1000 + q.X;
        }

        /// <summary>
        /// A nested struct must be copied as well, otherwise the copy shares the inner struct.
        /// </summary>
        public static int CopyIsDeep(int a)
        {
            var s = new Segment { From = new Point { X = a, Y = 1 }, To = new Point { X = 2, Y = 3 } };
            var t = s;
            t.From.X = 99;
            return s.From.X * 1000 + t.From.X;
        }

        /// <summary>
        /// A struct stored in a class field is copied in both directions.
        /// </summary>
        public static int CopyThroughClassField(int a)
        {
            var holder = new Holder { Value = new Point { X = a, Y = a + 1 } };
            var q = holder.Value;
            q.X = 99;
            return holder.Value.X * 1000 + q.X;
        }

        /// <summary>
        /// A struct stored into an array element is copied.
        /// </summary>
        public static int CopyIntoArrayElement(int a)
        {
            var arr = new Point[1];
            var p = new Point { X = a, Y = a + 1 };
            arr[0] = p;
            p.X = 99;
            return arr[0].X * 1000 + p.X;
        }

        /// <summary>
        /// A struct stored by an object initializer is copied.
        /// </summary>
        public static int CopyThroughObjectInitializer(int a)
        {
            var p = new Point { X = a, Y = a + 1 };
            var s = new Segment { From = p, To = new Point { X = 1, Y = 2 } };
            p.X = 99;
            return s.From.X * 1000 + p.X;
        }

        /// <summary>
        /// A struct boxed into an object is a copy, so a later write to the source is not visible.
        /// </summary>
        public static int CopyOnBoxing(int a)
        {
            var p = new Point { X = a, Y = a + 1 };
            object boxed = p;
            p.X = 99;
            return ((Point)boxed).X * 1000 + p.X;
        }

        /// <summary>
        /// A struct stored in an object array is boxed, so it is a copy as well.
        /// </summary>
        public static int CopyIntoObjectArray(int a)
        {
            var p = new Point { X = a, Y = a + 1 };
            var boxed = new object[] { p };
            p.X = 99;
            return ((Point)boxed[0]).X * 1000 + p.X;
        }

        /// <summary>
        /// A foreach iteration variable is a copy of the element it reads.
        /// </summary>
        public static int CopyInForeach(int a)
        {
            var arr = new Point[] { new Point { X = a, Y = a + 1 } };
            int sum = 0;
            foreach (var p in arr)
            {
                arr[0].X = 99;
                sum += p.X;
            }
            return sum;
        }

        /// <summary>
        /// A tuple is a value type, so assigning it copies it.
        /// </summary>
        public static int CopyOnTupleAssignment(int a, int b)
        {
            var t = (a, b);
            var u = t;
            u.Item1 = 99;
            return t.Item1 * 1000 + u.Item1;
        }

        /// <summary>
        /// The default value of a tuple is a tuple of default values.
        /// </summary>
        public static int DefaultTupleFields()
        {
            (int, int) t = default;
            return t.Item1 * 1000 + t.Item2;
        }

        /// <summary>
        /// A struct argument is passed by value, so writes inside the callee stay local.
        /// </summary>
        public static int ArgumentIsCopied(int a)
        {
            var p = new Point { X = a, Y = a + 1 };
            return Overwrite(p) * 1000 + p.X;
        }

        /// <summary>
        /// The default value of a struct is a struct, not null, so its fields can be read.
        /// </summary>
        public static int DefaultStructFields()
        {
            var p = new Point();
            return p.X * 1000 + p.Y;
        }

        /// <summary>
        /// The default value of a nested struct is a struct as well.
        /// </summary>
        public static int DefaultNestedStructFields()
        {
            var s = new Segment();
            return s.From.X * 1000 + s.From.Y * 100 + s.To.X * 10 + s.To.Y;
        }

        /// <summary>
        /// Fields of a nested struct are readable and writable.
        /// </summary>
        public static int WriteNestedStructFields(int a, int b)
        {
            var s = new Segment();
            s.From.X = a;
            s.From.Y = a + b;
            s.To.Y = b;
            return s.From.X * 1000 + s.From.Y * 100 + s.To.Y;
        }
    }
}
