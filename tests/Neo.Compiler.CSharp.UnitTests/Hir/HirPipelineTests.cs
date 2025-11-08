// Copyright (C) 2015-2025 The Neo Project.
//
// HirPipelineTests.cs is part of the neo-devpack-dotnet test suite and is
// distributed under the MIT license; see LICENSE for details.

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.VM;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;

namespace Neo.Compiler.CSharp.UnitTests.Hir;

[TestClass]
public sealed class HirPipelineTests
{
    [TestMethod]
    public void HirPipeline_ProducesExecutableAndManifest()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class SimpleContract : SmartContract
{
    public static int Main()
    {
        return 42;
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.Optimize = CompilationOptions.OptimizationType.All;
                options.Debug = CompilationOptions.DebugType.None;
            });

            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");

            var context = contexts[0];
            if (!context.Success)
            {
                var errors = string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));
                Assert.Fail($"HIR compilation failed: {errors}");
            }

            Assert.IsNotNull(context.HirModule, "HIR module should be populated when EnableHir is true.");
            Assert.IsNotNull(context.MirModule, "MIR module should be produced after HIR lowering.");

            var nef = context.CreateExecutable();
            Assert.IsNotNull(nef, "NEF emission should succeed when HIR pipeline completes.");
            Assert.IsTrue(nef.Script.Length > 0, "Generated script should not be empty.");
            Assert.AreEqual(OpCode.RET, (OpCode)nef.Script.Span[^1], "Script should terminate with RET.");

            var manifest = context.CreateManifest();
            Assert.IsNotNull(manifest, "Manifest generation should succeed.");
            Assert.AreEqual("SimpleContract", manifest.Name, "Manifest should carry contract name derived from type.");
            Assert.IsTrue(manifest.Abi.Methods.Any(m => string.Equals(m.Name, "main", StringComparison.OrdinalIgnoreCase)),
                "ABI should expose the Main entry point.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }


    [TestMethod]
    public void HirPipeline_HandlesRecursiveStruct()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class RecursiveContract : SmartContract
{
    private struct Node
    {
        public Node Next;
        public int Value;
    }

    private static Node _head;

    public static int Sum()
    {
        return 0;
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.Optimize = CompilationOptions.OptimizationType.All;
                options.Debug = CompilationOptions.DebugType.None;
            });

            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");

            var context = contexts[0];
            if (!context.Success)
            {
                var errors = string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));
                Assert.Fail($"HIR compilation failed: {errors}");
            }

            Assert.IsNotNull(context.HirModule, "HIR module should be populated when EnableHir is true.");
            Assert.IsTrue(context.HirModule.Functions.ContainsKey("global::RecursiveContract.Sum"),
                "Recursive struct should not prevent HIR function creation.");

            var nef = context.CreateExecutable();
            Assert.IsNotNull(nef, "NEF emission should succeed.");
            Assert.IsTrue(nef.Script.Length > 0, "Generated script should not be empty.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void HirPipeline_StaticFieldStorageMap()
    {
        const string source = """
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class StaticMapContract : SmartContract
{
    private static readonly StorageMap Map = new(Storage.CurrentContext, "prefix");

    public static void Set(ByteString key, ByteString value)
    {
        Map.Put(key, value);
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.Optimize = CompilationOptions.OptimizationType.All;
                options.Debug = CompilationOptions.DebugType.None;
            });

            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");

            var context = contexts[0];
            if (!context.Success)
            {
                var errors = string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString()));
                Assert.Fail($"HIR compilation failed: {errors}");
            }

            var nef = context.CreateExecutable();
            Assert.IsNotNull(nef, "NEF emission should succeed.");
            Assert.IsTrue(nef.Script.Length > 0, "Generated script should not be empty.");

            static bool ContainsAny(byte[] script, params OpCode[] opcodes)
            {
                foreach (var op in opcodes)
                {
                    if (Array.IndexOf(script, (byte)op) >= 0)
                        return true;
                }
                return false;
            }

            var script = nef.Script.ToArray();
            Assert.IsTrue(
                ContainsAny(script, OpCode.STSFLD0, OpCode.STSFLD1, OpCode.STSFLD2, OpCode.STSFLD3, OpCode.STSFLD4, OpCode.STSFLD5, OpCode.STSFLD6, OpCode.STSFLD),
                "Static field store opcode should appear in script.");
            Assert.IsTrue(
                ContainsAny(script, OpCode.LDSFLD0, OpCode.LDSFLD1, OpCode.LDSFLD2, OpCode.LDSFLD3, OpCode.LDSFLD4, OpCode.LDSFLD5, OpCode.LDSFLD6, OpCode.LDSFLD),
                "Static field load opcode should appear in script.");

            var manifest = context.CreateManifest();
            Assert.IsNotNull(manifest, "Manifest generation should succeed.");
            Assert.IsTrue(manifest.Abi.Methods.Any(m => string.Equals(m.Name, "set", StringComparison.OrdinalIgnoreCase)),
                "ABI should expose the Set method.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
