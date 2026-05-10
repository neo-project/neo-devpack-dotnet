// Copyright (C) 2015-2026 The Neo Project.
//
// DisassemblerTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Disassembler.CSharp.Tests;

[TestClass]
public class DisassemblerTests
{
    [TestMethod]
    public void ConvertScriptToInstructions_RETOnlyReturnsSingleRET()
    {
        var script = new byte[] { (byte)OpCode.RET };
        var list = Disassembler.ConvertScriptToInstructions(script);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(OpCode.RET, list[0].OpCode);
    }

    [TestMethod]
    public void ConvertScriptToInstructions_WithoutTerminalRETAppendsRET()
    {
        var script = new byte[] { (byte)OpCode.PUSH0 };
        var list = Disassembler.ConvertScriptToInstructions(script);
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(OpCode.PUSH0, list[0].OpCode);
        Assert.AreEqual(OpCode.RET, list[1].OpCode);
    }

    [TestMethod]
    public void ConvertScriptToInstructions_PUSH0ThenRETHasNoExtraRET()
    {
        var script = new byte[] { (byte)OpCode.PUSH0, (byte)OpCode.RET };
        var list = Disassembler.ConvertScriptToInstructions(script);
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual(OpCode.PUSH0, list[0].OpCode);
        Assert.AreEqual(OpCode.RET, list[1].OpCode);
    }

    [TestMethod]
    public void ConvertMethodToInstructions_FiltersByAddressRangeAndTupleLayout()
    {
        var script = new byte[]
        {
            (byte)OpCode.PUSH0,
            (byte)OpCode.PUSH1,
            (byte)OpCode.RET
        };
        var nef = new NefFile
        {
            Compiler = "test",
            Source = "test.cs",
            Tokens = [],
            Script = script
        };

        var slice = Disassembler.ConvertMethodToInstructions(nef, start: 1, end: 1);
        Assert.AreEqual(1, slice.Count);
        Assert.AreEqual(1, slice[0].offset);
        Assert.AreEqual(0, slice[0].address);
        Assert.AreEqual(OpCode.PUSH1, slice[0].instruction.OpCode);
    }

    [TestMethod]
    public void GetMethod_ReturnsMatchingDebugEntry()
    {
        var abiMethod = new ContractMethodDescriptor
        {
            Name = "main",
            Offset = 0,
            Parameters = [],
            ReturnType = ContractParameterType.Void,
            Safe = false
        };

        var abi = new JObject
        {
            ["name"] = "main",
            ["parameters"] = new JArray(),
            ["returntype"] = "Void",
            ["offset"] = 0,
            ["safe"] = false
        };

        var debugInfo = new JObject
        {
            ["methods"] = new JArray
            {
                new JObject { ["id"] = "Main", ["abi"] = abi }
            }
        };

        var found = Disassembler.GetMethod(abiMethod, debugInfo);
        Assert.IsNotNull(found);
        Assert.AreEqual("Main", found!["id"]!.AsString());
    }

    [TestMethod]
    public void GetMethod_ReturnsNullWhenABIDoesNotMatch()
    {
        var abiMethod = new ContractMethodDescriptor
        {
            Name = "other",
            Offset = 0,
            Parameters = [],
            ReturnType = ContractParameterType.Void,
            Safe = false
        };

        var debugInfo = new JObject
        {
            ["methods"] = new JArray
            {
                new JObject
                {
                    ["id"] = "Main",
                    ["abi"] = new JObject
                    {
                        ["name"] = "main",
                        ["parameters"] = new JArray(),
                        ["returntype"] = "Void",
                        ["offset"] = 0,
                        ["safe"] = false
                    }
                }
            }
        };

        Assert.IsNull(Disassembler.GetMethod(abiMethod, debugInfo));
    }

    [TestMethod]
    public void GetMethodStartEndAddress_ParsesRangeString()
    {
        var method = new JObject
        {
            ["range"] = new JString("10-42")
        };
        var (start, end) = Disassembler.GetMethodStartEndAddress(method);
        Assert.AreEqual(10, start);
        Assert.AreEqual(42, end);
    }

    [TestMethod]
    public void GetMethodStartEndAddress_WithoutRangeReturnsNegative()
    {
        var method = new JObject();
        var (start, end) = Disassembler.GetMethodStartEndAddress(method);
        Assert.AreEqual(-1, start);
        Assert.AreEqual(-1, end);
    }

    [TestMethod]
    public void InstructionToString_PUSH0ContainsOpcodeAndOmitsPriceWhenDisabled()
    {
        var instruction = new Script(new byte[] { (byte)OpCode.PUSH0 }).GetInstruction(0);
        var text = Disassembler.InstructionToString(instruction, addPrice: false);
        Assert.AreEqual("PUSH0", text);
    }

    [TestMethod]
    public void InstructionToString_SYSCALLIncludesDescriptorNameAndDatoshiSuffix()
    {
        using var sb = new ScriptBuilder();
        sb.EmitSysCall(ApplicationEngine.System_Runtime_GetEntryScriptHash);
        var script = sb.ToArray();
        var instruction = new Script(script).GetInstruction(0);
        var text = Disassembler.InstructionToString(instruction, addPrice: true);
        StringAssert.StartsWith(text, "SYSCALL ");
        StringAssert.Contains(text, "System.Runtime.GetEntryScriptHash");
        StringAssert.EndsWith(text, " datoshi]");
    }

    [TestMethod]
    public void InstructionToString_CONVERTShowsStackItemType()
    {
        var script = new byte[]
        {
            (byte)OpCode.CONVERT,
            (byte)StackItemType.Integer
        };
        var instruction = new Script(script).GetInstruction(0);
        var text = Disassembler.InstructionToString(instruction, addPrice: false);
        StringAssert.StartsWith(text, "CONVERT ");
        StringAssert.Contains(text, "Integer");
    }

    [TestMethod]
    public void InstructionToString_PUSHDATA1PrintsAsciiPayloadWhenPrintable()
    {
        using var sb = new ScriptBuilder();
        sb.EmitPush("hello");
        var script = sb.ToArray();
        var instruction = new Script(script).GetInstruction(0);
        var text = Disassembler.InstructionToString(instruction, addPrice: false);
        StringAssert.StartsWith(text, "PUSHDATA1 ");
        StringAssert.Contains(text, "'hello'");
    }
}
