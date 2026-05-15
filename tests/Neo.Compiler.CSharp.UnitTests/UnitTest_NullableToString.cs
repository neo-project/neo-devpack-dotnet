// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableToString.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableToString
{
    [TestMethod]
    public void NullableIntegerToString_LoadsReceiverOnce()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Main(int? value) => value.ToString();
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var methodBlock = ExtractMethodBlock(context.CreateAssembly(), "Contract.Main(int?)");
        var receiverLoads = methodBlock
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Count(line => line.Contains("LDARG 0", StringComparison.Ordinal));

        Assert.AreEqual(1, receiverLoads, $"Nullable<T>.ToString() should leave only the string result on the stack.\n{methodBlock}");
    }

    private static string ExtractMethodBlock(string assembly, string methodSignature)
    {
        var normalized = assembly.Replace("\r\n", "\n", StringComparison.Ordinal);
        var marker = $"// {methodSignature}";
        var start = normalized.IndexOf(marker, StringComparison.Ordinal);
        Assert.IsTrue(start >= 0, $"Method section '{methodSignature}' was not found in generated assembly.\n{assembly}");

        var next = normalized.IndexOf("\n// ", start + marker.Length, StringComparison.Ordinal);
        if (next < 0)
            next = normalized.Length;

        return normalized[start..next];
    }
}
