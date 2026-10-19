// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_StringConcatEmpty.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_StringConcatEmpty : SmartContract.Framework.SmartContract
    {
        public static string? ConcatNeoRight() => string.Concat("Neo", "");

        public static string? ConcatNeoLeft() => string.Concat("", "Neo");

        public static string? ConcatBoth() => string.Concat("", "");

        public static string? ConcatNullRight() => string.Concat((string?)null, "");

        public static string? ConcatNullLeft() => string.Concat("", (string?)null);

        public static string? ConcatRight(string? value) => string.Concat(value, "");

        public static string? ConcatLeft(string? value) => string.Concat("", value);

        public static string? ConcatNormal() => string.Concat("Neo", "N3");
    }
}
