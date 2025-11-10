// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_UIntLiteralReturns.cs file belongs to the neo project and is free
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
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.Extensions;
using Neo.SmartContract;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_UIntLiteralReturns
{
    private const string ContractSource = @"
using Neo.SmartContract.Framework;

public class LiteralHashContract : SmartContract
{
    public static UInt160 LiteralHash()
    {
        return ""0xd2a4cff31913016155e38e474a2c06d08be276cf"";
    }
}
";

    private static readonly Lazy<CompilationEngine> Engine = new(() => new CompilationEngine(new CompilationOptions
    {
        Debug = CompilationOptions.DebugType.Extended,
        CompilerVersion = "LiteralHashTests",
        Optimize = CompilationOptions.OptimizationType.All,
        Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
        SkipRestoreIfAssetsPresent = true
    }));

    private static readonly Lazy<CompilationSourceReferences> References = new(() =>
    {
        var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        return new CompilationSourceReferences
        {
            Projects = new[] { frameworkProject }
        };
    });

    [TestMethod]
    public void UInt160_literal_return_executes_as_hash()
    {
        var context = CompileContract(ContractSource);
        Assert.IsTrue(context.Success, $"Compilation failed:{Environment.NewLine}{string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()))}");

        var nef = context.CreateExecutable();
        var manifest = context.CreateManifest();
        var engine = new TestEngine(true);
        var state = engine.Native.ContractManagement.Deploy(
            nef.ToArray(),
            Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)),
            data: null);

        var literalMethod = manifest.Abi.Methods.Single(m => string.Equals(m.Name, "LiteralHash", StringComparison.OrdinalIgnoreCase));
        var expected = UInt160.Parse("0xd2a4cff31913016155e38e474a2c06d08be276cf");
        var actual = InvokeLiteralHash(engine, state.Hash, literalMethod.Name);

        Assert.AreEqual(expected, actual);
    }

    private static UInt160 InvokeLiteralHash(TestEngine engine, UInt160 contractHash, string methodName)
    {
        using ScriptBuilder builder = new();
        builder.Emit(OpCode.NEWARRAY0);
        builder.EmitPush((int)engine.CallFlags);
        builder.EmitPush(methodName);
        builder.EmitPush(contractHash.ToArray());
        builder.EmitSysCall(ApplicationEngine.System_Contract_Call);

        var result = engine.Execute(builder.ToArray());
        return new UInt160(result.GetSpan());
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
}
