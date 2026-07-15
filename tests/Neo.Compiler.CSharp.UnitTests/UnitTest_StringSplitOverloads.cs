// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringSplitOverloads.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PrimitiveStackItem = Neo.VM.Types.PrimitiveType;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class UnitTest_StringSplitOverloads
{
    [TestMethod]
    public void Split_ModernOverloadSignatures_MatchDotNet()
    {
        const string source = """
using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("splitCharArray")]
    public static string[] SplitCharArray(string value) => value.Split(new[] { ',' });

    [DisplayName("splitString")]
    public static string[] SplitString(string value) => value.Split(",");

    [DisplayName("splitStringRemoveEmpty")]
    public static string[] SplitStringRemoveEmpty(string value) =>
        value.Split(",", StringSplitOptions.RemoveEmptyEntries);

    [DisplayName("splitStringCount")]
    public static string[] SplitStringCount(string value) =>
        value.Split(",", int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
}
""";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<StringSplitContract>(context.CreateExecutable(), context.CreateManifest());
        const string value = "alpha,,beta";

        CollectionAssert.AreEqual(value.Split(new[] { ',' }), ConvertToStrings(contract.SplitCharArray(value)));
        CollectionAssert.AreEqual(value.Split(","), ConvertToStrings(contract.SplitString(value)));
        CollectionAssert.AreEqual(
            value.Split(",", StringSplitOptions.RemoveEmptyEntries),
            ConvertToStrings(contract.SplitStringRemoveEmpty(value)));
        CollectionAssert.AreEqual(
            value.Split(",", int.MaxValue, StringSplitOptions.RemoveEmptyEntries),
            ConvertToStrings(contract.SplitStringCount(value)));
    }

    private static string[] ConvertToStrings(IList<object>? items)
    {
        if (items is null)
            return [];

        return items.Select(item => item switch
        {
            string value => value,
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            PrimitiveStackItem primitive => primitive.GetString() ??
                                            throw new AssertFailedException("Unexpected null string stack item."),
            _ => throw new AssertFailedException($"Unexpected stack item type: {item?.GetType().FullName}")
        }).ToArray();
    }

    public abstract class StringSplitContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("splitCharArray")]
        public abstract IList<object>? SplitCharArray(string value);

        [DisplayName("splitString")]
        public abstract IList<object>? SplitString(string value);

        [DisplayName("splitStringRemoveEmpty")]
        public abstract IList<object>? SplitStringRemoveEmpty(string value);

        [DisplayName("splitStringCount")]
        public abstract IList<object>? SplitStringCount(string value);
    }
}
