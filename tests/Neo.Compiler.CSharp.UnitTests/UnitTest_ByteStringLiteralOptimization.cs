// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ByteStringLiteralOptimization.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ByteStringLiteralOptimization
{
    [TestMethod]
    public void ExplicitByteStringCastFromConstantByteArraySkipsBufferConvert()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static ByteString Key()
    {
        return (ByteString)new byte[] { 0x01, 0x02 };
    }
}
""";

        var instructions = GetMethodInstructions(source, "Contract.Key()");

        CollectionAssert.DoesNotContain(
            instructions.Select(p => p.OpCode).ToArray(),
            OpCode.CONVERT,
            "Constant byte-array literals cast to ByteString should compile as direct PUSHDATA without Buffer conversion.");
    }

    [TestMethod]
    public void ExplicitByteStringStorageKeySkipsBufferConvert()
    {
        const string source = """
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static ByteString? Read()
    {
        return Storage.Get(Storage.CurrentContext, (ByteString)new byte[] { 0x01 });
    }
}
""";

        var instructions = GetMethodInstructions(source, "Contract.Read()");

        Assert.IsFalse(
            HasConvertTo(instructions, StackItemType.Buffer),
            "Explicit ByteString storage keys should avoid the expensive byte[] Buffer conversion.");
    }

    [TestMethod]
    public void ExplicitByteStringCastFromParenthesizedImplicitArraySkipsBufferConvert()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static ByteString Key()
    {
        return (ByteString)(new[] { (byte)0x01, (byte)0x02 });
    }
}
""";

        var instructions = GetMethodInstructions(source, "Contract.Key()");

        Assert.IsFalse(
            HasConvertTo(instructions, StackItemType.Buffer),
            "Parenthesized implicit byte-array literals cast to ByteString should still compile as direct PUSHDATA.");
    }

    [TestMethod]
    public void ExplicitByteStringCastFromVariableByteArrayUsesStandardConversion()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static ByteString Key(byte[] value)
    {
        return (ByteString)value;
    }
}
""";

        var instructions = GetMethodInstructionsByIdPrefix(source, "Contract.Key(");

        Assert.IsTrue(
            instructions.Any(p => p.OpCode == OpCode.LDARG0),
            "Variable byte-array casts should read the runtime argument instead of compiling as constant PUSHDATA.");
    }

    [TestMethod]
    public void ExplicitByteStringCastFromNonConstantByteArrayUsesStandardConversion()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static ByteString Key(byte value)
    {
        return (ByteString)new byte[] { value };
    }
}
""";

        var instructions = GetMethodInstructionsByIdPrefix(source, "Contract.Key(");

        Assert.IsTrue(
            instructions.Any(p => p.OpCode == OpCode.LDARG0),
            "Non-constant byte-array literals should read runtime values instead of compiling as constant PUSHDATA.");
    }

    [TestMethod]
    public void ByteArrayLiteralStillCreatesBuffer()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static byte[] Key()
    {
        return new byte[] { 0x01, 0x02 };
    }
}
""";

        var instructions = GetMethodInstructions(source, "Contract.Key()");
        Assert.IsTrue(instructions.Any(instruction => instruction.OpCode == OpCode.LEFT),
            "Plain byte[] literals should still compile as Buffer values.");
    }

    [TestMethod]
    public void OptimizedByteStringLiteralExecutesInVm()
    {
        const string source = """
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("literal")]
    public static ByteString Literal()
    {
        return (ByteString)new byte[] { 0x01, 0x02 };
    }

    [DisplayName("parenthesizedImplicit")]
    public static ByteString ParenthesizedImplicit()
    {
        return (ByteString)(new[] { (byte)0x03, (byte)0x04 });
    }

    [DisplayName("storageRoundTrip")]
    public static ByteString? StorageRoundTrip()
    {
        Storage.Put(
            Storage.CurrentContext,
            (ByteString)new byte[] { 0x05 },
            (ByteString)new byte[] { 0x06, 0x07 });
        return Storage.Get(Storage.CurrentContext, (ByteString)new byte[] { 0x05 });
    }
}
""";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ByteStringLiteralContract>(context.CreateExecutable(), context.CreateManifest());

        CollectionAssert.AreEqual(new byte[] { 0x01, 0x02 }, contract.Literal());
        CollectionAssert.AreEqual(new byte[] { 0x03, 0x04 }, contract.ParenthesizedImplicit());
        CollectionAssert.AreEqual(new byte[] { 0x06, 0x07 }, contract.StorageRoundTrip());
    }

    private static Neo.VM.Instruction[] GetMethodInstructions(string source, string methodId)
    {
        return GetMethodInstructions(source, methodId, exactMatch: true);
    }

    private static Neo.VM.Instruction[] GetMethodInstructionsByIdPrefix(string source, string methodIdPrefix)
    {
        return GetMethodInstructions(source, methodIdPrefix, exactMatch: false);
    }

    private static Neo.VM.Instruction[] GetMethodInstructions(string source, string methodId, bool exactMatch)
    {
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var nef = context.CreateExecutable();
        var debugInfo = context.CreateDebugInformation();
        var (start, end) = GetMethodRange(debugInfo, methodId, exactMatch);

        return ((Script)nef.Script)
            .EnumerateInstructions()
            .Where(i => i.address >= start && i.address <= end)
            .Select(i => i.instruction)
            .ToArray();
    }

    private static (int start, int end) GetMethodRange(JObject debugInfo, string methodId, bool exactMatch)
    {
        var methods = (JArray)debugInfo["methods"]!;
        var method = methods
            .OfType<JObject>()
            .FirstOrDefault(m =>
            {
                var id = m["id"]?.GetString();
                return exactMatch
                    ? string.Equals(id, methodId, StringComparison.Ordinal)
                    : id?.StartsWith(methodId, StringComparison.Ordinal) == true;
            });

        Assert.IsNotNull(method, $"Unable to find method '{methodId}' in debug info.");

        var range = method["range"]!.GetString();
        var dashIndex = range.IndexOf('-', StringComparison.Ordinal);
        Assert.IsTrue(dashIndex > 0, "Method range should include a dash-delimited offset span.");

        var start = int.Parse(range[..dashIndex]);
        var end = int.Parse(range[(dashIndex + 1)..]);
        return (start, end);
    }

    private static bool HasConvertTo(Neo.VM.Instruction[] instructions, StackItemType type)
    {
        return instructions.Any(instruction =>
            instruction.OpCode == OpCode.CONVERT &&
            instruction.Operand.Span.Length == 1 &&
            instruction.Operand.Span[0] == (byte)type);
    }

    public abstract class ByteStringLiteralContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("literal")]
        public abstract byte[]? Literal();

        [DisplayName("parenthesizedImplicit")]
        public abstract byte[]? ParenthesizedImplicit();

        [DisplayName("storageRoundTrip")]
        public abstract byte[]? StorageRoundTrip();
    }
}
