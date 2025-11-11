// Copyright (C) 2015-2025 The Neo Project.
//
// UInt160ParseTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.Compiler;
using Neo.Extensions;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class UInt160ParseTests
{
    private static readonly byte[] UInt160ExpectedBytes =
    [
        0x01, 0x02, 0x03, 0x04, 0x05,
        0x06, 0x07, 0x08, 0x09, 0x0A,
        0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
        0x10, 0x11, 0x12, 0x13, 0x14
    ];

    private static readonly byte[] UInt256ExpectedBytes =
    [
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
        0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
        0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20
    ];

    private static readonly byte[] ECPointExpectedBytes =
    [
        0x02, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76,
        0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E,
        0x7F, 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86,
        0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E,
        0x8F
    ];

    private const string ContractSource = @"
using Neo.SmartContract.Framework;

public class UInt160ParseContract : SmartContract
{
    public static byte[] ParseRuntimeString()
    {
        byte[] data = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05,
            0x06, 0x07, 0x08, 0x09, 0x0A,
            0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14
        };

        return (byte[])UInt160.Parse((string)(ByteString)data);
    }

    public static void ParseRuntimeStringInvalid()
    {
        byte[] data = new byte[]
        {
            0x01, 0x02, 0x03
        };

        UInt160.Parse((string)(ByteString)data);
    }

    public static byte[] ParseUInt256RuntimeString()
    {
        byte[] data = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
            0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
            0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20
        };

        return (byte[])UInt256.Parse((string)(ByteString)data);
    }

    public static void ParseUInt256RuntimeStringInvalid()
    {
        byte[] data = new byte[]
        {
            0x01, 0x02, 0x03
        };

        UInt256.Parse((string)(ByteString)data);
    }

    public static byte[] ParseECPointRuntimeString()
    {
        byte[] data = new byte[]
        {
            0x02, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76,
            0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E,
            0x7F, 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86,
            0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E,
            0x8F
        };

        return (byte[])ECPoint.Parse((string)(ByteString)data);
    }

    public static void ParseECPointRuntimeStringInvalid()
    {
        byte[] data = new byte[]
        {
            0x02, 0x70, 0x71
        };

        ECPoint.Parse((string)(ByteString)data);
    }
}
";

    private static readonly Lazy<CompilationEngine> Engine = new(() => new CompilationEngine(new CompilationOptions
    {
        Debug = CompilationOptions.DebugType.Extended,
        CompilerVersion = "UInt160ParseTests",
        Optimize = CompilationOptions.OptimizationType.All,
        Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
        SkipRestoreIfAssetsPresent = true
    }));

    private static readonly Lazy<CompilationSourceReferences> References = new(() =>
    {
        var repoRoot = LocateRepositoryRoot();
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        return new CompilationSourceReferences
        {
            Projects = new[] { frameworkProject }
        };
    });

    [TestMethod]
    public void UInt160ParseRuntimeString_ReturnsExpectedBytes()
    {
        var (engine, contractHash, methods) = CompileAndDeploy();
        var stackItem = InvokeContract(engine, contractHash, methods["parseruntimestring"]);
        CollectionAssert.AreEqual(UInt160ExpectedBytes, stackItem.GetSpan().ToArray());
    }

    [TestMethod]
    public void UInt160ParseRuntimeStringInvalid_Throws()
    {
        var (engine, contractHash, methods) = CompileAndDeploy();
        Assert.ThrowsException<TestException>(() => InvokeContract(engine, contractHash, methods["parseruntimestringinvalid"]));
    }

    [TestMethod]
    public void UInt256ParseRuntimeString_ReturnsExpectedBytes()
    {
        var (engine, contractHash, methods) = CompileAndDeploy();
        var stackItem = InvokeContract(engine, contractHash, methods["parseuint256runtimestring"]);
        CollectionAssert.AreEqual(UInt256ExpectedBytes, stackItem.GetSpan().ToArray());
    }

    [TestMethod]
    public void UInt256ParseRuntimeStringInvalid_Throws()
    {
        var (engine, contractHash, methods) = CompileAndDeploy();
        Assert.ThrowsException<TestException>(() => InvokeContract(engine, contractHash, methods["parseuint256runtimestringinvalid"]));
    }

    [TestMethod]
    public void ECPointParseRuntimeString_ReturnsExpectedBytes()
    {
        var (engine, contractHash, methods) = CompileAndDeploy();
        var stackItem = InvokeContract(engine, contractHash, methods["parseecpointruntimestring"]);
        CollectionAssert.AreEqual(ECPointExpectedBytes, stackItem.GetSpan().ToArray());
    }

    [TestMethod]
    public void ECPointParseRuntimeStringInvalid_Throws()
    {
        var (engine, contractHash, methods) = CompileAndDeploy();
        Assert.ThrowsException<TestException>(() => InvokeContract(engine, contractHash, methods["parseecpointruntimestringinvalid"]));
    }

    private static (TestEngine Engine, UInt160 ContractHash, IReadOnlyDictionary<string, string> Methods) CompileAndDeploy()
    {
        var context = CompileContract(ContractSource);
        Assert.IsTrue(context.Success, $"Compilation failed:{Environment.NewLine}{string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()))}");

        var nef = context.CreateExecutable();
        var manifest = context.CreateManifest();
        var methods = manifest.Abi.Methods.ToDictionary(m => m.Name.ToLowerInvariant(), m => m.Name, StringComparer.OrdinalIgnoreCase);
        var engine = new TestEngine(true);
        var state = engine.Native.ContractManagement.Deploy(
            nef.ToArray(),
            Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)),
            data: null);

        return (engine, state.Hash, methods);
    }

    private static StackItem InvokeContract(TestEngine engine, UInt160 contractHash, string methodName)
    {
        using ScriptBuilder builder = new();
        builder.Emit(OpCode.NEWARRAY0);
        builder.EmitPush((int)engine.CallFlags);
        builder.EmitPush(methodName);
        builder.EmitPush(contractHash.ToArray());
        builder.EmitSysCall(ApplicationEngine.System_Contract_Call);

        return engine.Execute(builder.ToArray());
    }

    private static CompilationContext CompileContract(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);
        try
        {
            return Engine.Value.CompileSources(References.Value, tempFile).First();
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private static string LocateRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git")) ||
                File.Exists(Path.Combine(directory.FullName, "global.json")) ||
                File.Exists(Path.Combine(directory.FullName, "neo-devpack-dotnet.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root from current directory.");
    }
}
