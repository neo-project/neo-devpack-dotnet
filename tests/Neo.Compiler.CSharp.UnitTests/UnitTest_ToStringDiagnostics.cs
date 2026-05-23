// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ToStringDiagnostics.cs file belongs to the neo project and is free
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
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ToStringDiagnostics
{
    [TestMethod]
    public void MapToStringReportsUnsupportedToStringType()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test()
    {
        var map = new Map<string, string>();
        map["name"] = "My NFT";
        return map.ToString();
    }
}
""");

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidToStringType), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
        StringAssert.Contains(diagnostics, "StdLib.Serialize");
    }

    [TestMethod]
    public void StringToStringStillCompiles()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test()
    {
        return "Jimmy".ToString();
    }
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    [TestMethod]
    public void ObjectToStringStillCompiles()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(object value)
    {
        return value.ToString();
    }
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    [TestMethod]
    public void NumericToStringStillCompiles()
    {
        AssertCompiles("""
using Neo.SmartContract.Framework;
using System.Numerics;

public class Contract : SmartContract
{
    public static string Test(int value, long longValue, BigInteger bigInteger)
    {
        return value.ToString() + "|" + longValue.ToString() + "|" + bigInteger.ToString();
    }
}
""");
    }

    [TestMethod]
    public void SupportedToStringConversionsExecuteInVm()
    {
        const string source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;
using System.Numerics;

public class Contract : SmartContract
{
    [DisplayName("numeric")]
    public static string Numeric(int value, long longValue, BigInteger bigInteger)
    {
        return value.ToString() + "|" + longValue.ToString() + "|" + bigInteger.ToString();
    }

    [DisplayName("text")]
    public static string Text(string value)
    {
        return value.ToString();
    }

    [DisplayName("character")]
    public static string Character(char value)
    {
        return value.ToString();
    }
}
""";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ToStringDiagnosticsContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("42|-17|12345678901234567890", contract.Numeric(42, -17, BigInteger.Parse("12345678901234567890")));
        Assert.AreEqual("neo", contract.Text("neo"));
        Assert.AreEqual("N", contract.Character('N'));
    }

    [TestMethod]
    public void FrameworkValueToStringTypesStillCompile()
    {
        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(ByteString value)
    {
        return value.ToString();
    }
}
""");

        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(UInt160 value)
    {
        return value.ToString();
    }
}
""");

        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(UInt256 value)
    {
        return value.ToString();
    }
}
""");

        AssertCompiles("""
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static string Test(ECPoint value)
    {
        return value.ToString();
    }
}
""");
    }

    private static void AssertCompiles(string source)
    {
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    public abstract class ToStringDiagnosticsContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("numeric")]
        public abstract string? Numeric(BigInteger? value, BigInteger? longValue, BigInteger? bigInteger);

        [DisplayName("text")]
        public abstract string? Text(string? value);

        [DisplayName("character")]
        public abstract string? Character(char? value);
    }
}
