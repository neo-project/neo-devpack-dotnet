// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_StringBuilder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Text;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_StringBuilder : SmartContract.Framework.SmartContract
{
    public static string AppendPrimitiveValues()
    {
        var builder = new StringBuilder();
        builder.Append((sbyte)-1);
        builder.Append('|');
        builder.Append((byte)2);
        builder.Append('|');
        builder.Append((short)-3);
        builder.Append('|');
        builder.Append((ushort)4);
        builder.Append('|');
        builder.Append(-5);
        builder.Append('|');
        builder.Append(6U);
        builder.Append('|');
        builder.Append(-7L);
        builder.Append('|');
        builder.Append(8UL);
        builder.Append('|');
        builder.Append(true);
        builder.Append('|');
        builder.Append(false);
        return builder.ToString();
    }
}
