// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NumericTryParseInvalidInput.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NumericTryParseInvalidInput
{
    [TestMethod]
    public void NumericTryParse_ReturnsFalse_ForInvalidInput()
    {
        var contract = CompileAndDeploy();

        AssertParseFailure(contract.IntTryParse(null!));
        AssertParseFailure(contract.IntTryParse("abc"));
        AssertParseFailure(contract.IntTryParse(""));
        AssertParseFailure(contract.UIntTryParse("1.5"));
        AssertParseFailure(contract.ByteTryParse("0x10"));
    }

    [TestMethod]
    public void NumericTryParse_StillParsesValidInput()
    {
        var contract = CompileAndDeploy();

        AssertParseSuccess(contract.IntTryParse("-42"), -42);
        AssertParseSuccess(contract.UIntTryParse("+42"), 42);
        AssertParseSuccess(contract.UIntTryParse("42"), 42);
        AssertParseSuccess(contract.ByteTryParse("255"), 255);
    }

    private static NumericTryParseContract CompileAndDeploy()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""intTryParse"")]
    public static (bool, int) IntTryParse(string s)
    {
        bool success = int.TryParse(s, out int result);
        return (success, result);
    }

    [DisplayName(""uintTryParse"")]
    public static (bool, uint) UIntTryParse(string s)
    {
        bool success = uint.TryParse(s, out uint result);
        return (success, result);
    }

    [DisplayName(""byteTryParse"")]
    public static (bool, byte) ByteTryParse(string s)
    {
        bool success = byte.TryParse(s, out byte result);
        return (success, result);
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<NumericTryParseContract>(context.CreateExecutable(), context.CreateManifest());
    }

    private static void AssertParseFailure(object?[]? result)
    {
        var (success, value) = ReadParseResult(result);
        Assert.IsFalse(success);
        Assert.AreEqual(BigInteger.Zero, value);
    }

    private static void AssertParseSuccess(object?[]? result, BigInteger expected)
    {
        var (success, value) = ReadParseResult(result);
        Assert.IsTrue(success);
        Assert.AreEqual(expected, value);
    }

    private static (bool Success, BigInteger Value) ReadParseResult(object?[]? result)
    {
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result!.Length);

        bool success = result[0] switch
        {
            StackItem stackItem => stackItem.GetBoolean(),
            bool boolean => boolean,
            _ => throw new AssertFailedException($"Unexpected success result type: {result[0]?.GetType().Name ?? "null"}")
        };

        BigInteger value = result[1] switch
        {
            BigInteger integer => integer,
            StackItem stackItem => stackItem.GetInteger(),
            _ => throw new AssertFailedException($"Unexpected parsed result type: {result[1]?.GetType().Name ?? "null"}")
        };

        return (success, value);
    }

    public abstract class NumericTryParseContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("intTryParse")]
        public abstract object?[]? IntTryParse(string s);

        [DisplayName("uintTryParse")]
        public abstract object?[]? UIntTryParse(string s);

        [DisplayName("byteTryParse")]
        public abstract object?[]? ByteTryParse(string s);
    }
}
