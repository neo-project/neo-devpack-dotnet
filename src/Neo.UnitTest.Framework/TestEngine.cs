// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Neo.Compiler;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.UnitTest.Framework;

public class TestEngine : ApplicationEngine
{
    public const long TestGas = 2000_00000000;

    private static readonly List<MetadataReference> references = new();

    static TestEngine()
    {
        var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        references.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")));
        references.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")));
        references.Add(MetadataReference.CreateFromFile(typeof(string).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location));
        var folder = Path.GetFullPath("../../../../../src/Neo.SmartContract.Framework/");
        var obj = Path.Combine(folder, "obj");
        var st = Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.StartsWith(obj))
            .OrderBy(p => p)
            .Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p));
        CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary);
        var cr = CSharpCompilation.Create(null, st, references, options);
        references.Add(cr.ToMetadataReference());
    }

    public TestEngine(TriggerType trigger = TriggerType.Application, IVerifiable verificable = null,
        DataCache snapshot = null, Block persistingBlock = null, IDiagnostic diagnostic = null)
        : base(trigger, verificable, snapshot, persistingBlock, ProtocolSettings.Default, TestGas, diagnostic)
    {
    }

    public NefFile Nef { get; private set; }
    public JObject Manifest { get; private set; }
    public JObject DebugInfo { get; private set; }

    public CompilationContext AddEntryScript(params string[] files)
    {
        return AddEntryScript(true, files);
    }

    public CompilationContext AddEntryScript(bool debug = true, params string[] files)
    {
        var context = CompilationContext.Compile(files, references, new Options
        {
            AddressVersion = ProtocolSettings.Default.AddressVersion,
            Debug = debug
        });
        if (context.Success)
        {
            Nef = context.CreateExecutable();
            Manifest = context.CreateManifest();
            DebugInfo = context.CreateDebugInformation();
            Reset();
        }

        return context;
    }

    public void Reset()
    {
        State = VMState.BREAK; // Required for allow to reuse the same TestEngine
        InvocationStack.Clear();
        while (ResultStack.Count > 0) ResultStack.Pop();
        if (Nef != null)
        {
            LoadScript(Nef.Script);
            // Mock contract
            var contextState = CurrentContext.GetState<ExecutionContextState>();
            contextState.Contract ??= new ContractState { Nef = Nef };
        }
    }

    private int GetMethodEntryOffset(string methodname)
    {
        if (Manifest is null) return -1;
        var methods = Manifest["abi"]["methods"] as JArray;
        foreach (var item in methods)
        {
            var method = item as JObject;
            if (method["name"].AsString() == methodname)
                return int.Parse(method["offset"].AsString());
        }

        return -1;
    }

    private int GetMethodReturnCount(string methodname)
    {
        if (Manifest is null) return -1;
        var methods = Manifest["abi"]["methods"] as JArray;
        foreach (var item in methods)
        {
            var method = item as JObject;
            if (method["name"].AsString() == methodname)
            {
                var returntype = method["returntype"].AsString();
                if (returntype == "Null" || returntype == "Void")
                    return 0;
                return 1;
            }
        }

        return -1;
    }

    public EvaluationStack ExecuteTestCaseStandard(string methodname, params StackItem[] args)
    {
        return ExecuteTestCaseStandard(methodname, Nef, args);
    }

    public EvaluationStack ExecuteTestCaseStandard(string methodname, NefFile contract, params StackItem[] args)
    {
        var offset = GetMethodEntryOffset(methodname);
        if (offset == -1) throw new Exception("Can't find method : " + methodname);
        var rvcount = GetMethodReturnCount(methodname);
        if (rvcount == -1) throw new Exception("Can't find method return count : " + methodname);
        return ExecuteTestCaseStandard(offset, (ushort)rvcount, contract, args);
    }

    public EvaluationStack ExecuteTestCaseStandard(int offset, ushort rvcount, params StackItem[] args)
    {
        return ExecuteTestCaseStandard(offset, rvcount, Nef, args);
    }

    public EvaluationStack ExecuteTestCaseStandard(int offset, ushort rvcount, NefFile contract,
        params StackItem[] args)
    {
        var context = InvocationStack.Pop();
        context = CreateContext(context.Script, rvcount, offset);
        LoadContext(context);
        // Mock contract
        var contextState = CurrentContext.GetState<ExecutionContextState>();
        contextState.Contract ??= new ContractState { Nef = contract };
        for (var i = args.Length - 1; i >= 0; i--)
            Push(args[i]);
        var initializeOffset = GetMethodEntryOffset("_initialize");
        if (initializeOffset != -1) LoadContext(CurrentContext.Clone(initializeOffset));
        while (true)
        {
            var bfault = (State & VMState.FAULT) > 0;
            var bhalt = (State & VMState.HALT) > 0;
            if (bfault || bhalt) break;
            Console.WriteLine("op:[" +
                              CurrentContext.InstructionPointer.ToString("X04") +
                              "]" +
                              CurrentContext.CurrentInstruction?.OpCode);
            ExecuteNext();
        }

        return ResultStack;
    }
}
