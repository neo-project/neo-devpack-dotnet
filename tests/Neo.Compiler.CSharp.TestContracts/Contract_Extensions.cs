// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Extensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public static class Ext
    {
        public static int sum(this int a, int b)
        {
            return a + b;
        }

        extension(int value)
        {
            public int Twice() => value * 2;

            public int Triple => value * 3;
        }

        extension(ExtensionBox box)
        {
            public int ExtensionValue
            {
                get => box.Value;
                set => box.Value = value;
            }
        }
    }

    public class ExtensionBox
    {
        public int Value { get; set; }
    }

    public class Contract_Extensions : SmartContract.Framework.SmartContract
    {
        public static int TestSum(int a, int b)
        {
            return a.sum(b);
        }

        public static int TestExtensionMemberMethod(int value)
        {
            return value.Twice();
        }

        public static int TestExtensionMemberProperty(int value)
        {
            return value.Triple;
        }

        public static int TestExtensionMemberCombination(int value)
        {
            return value.Twice() + value.Triple;
        }

        public static int TestExtensionMemberPropertySetter(int value)
        {
            var box = new ExtensionBox();
            box.ExtensionValue = value;
            return box.Value;
        }
    }
}
