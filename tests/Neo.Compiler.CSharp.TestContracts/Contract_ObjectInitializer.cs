// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_ObjectInitializer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_ObjectInitializer : SmartContract.Framework.SmartContract
    {
        public class Table
        {
            private int[] _data = new int[8];

            public int this[int i]
            {
                get { return _data[i]; }
                set { _data[i] = value; }
            }
        }

        public class Holder
        {
            public int P { get; set; }
            public int F;
        }

        public static int IndexInitializer(int a)
        {
            var t = new Table { [1] = a, [2] = a + 1 };
            return t[1] * 10 + t[2];
        }

        public static int PropertyInitializer(int a)
        {
            var h = new Holder { P = a };
            return h.P;
        }

        public static int FieldInitializer(int a)
        {
            var h = new Holder { F = a };
            return h.F;
        }
    }
}
