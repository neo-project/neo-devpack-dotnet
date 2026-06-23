// Copyright (C) 2015-2026 The Neo Project.
//
// DebugLauncherEndToEndTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.VM.Types;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Debugging.UnitTests
{
    [TestClass]
    public class DebugLauncherEndToEndTests
    {
        // The breakpoint is requested on this 1-based line of the contract source below
        // (the "int sum = a + b;" statement).
        private const int BreakpointLine = 7;

        private const string Source = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Add(int a, int b)
    {
        int sum = a + b;
        return sum;
    }
}";

        [TestMethod]
        public void DebugsCompiledContract_StopsAtSourceLine_ThenReturnsResult()
        {
            var (nef, manifest, debugInfo, sourceFile) = Compile();

            var engine = new TestEngine(true);
            var hash = DebugLauncher.Deploy(engine, nef, manifest);

            using var session = new DebugSession(engine, debugInfo, hash);
            var breakpoints = session.SetBreakpoints(sourceFile, BreakpointLine);
            Assert.AreEqual(1, breakpoints.Count, "the breakpoint should bind to executable code");
            int expectedLine = breakpoints[0].Line;

            DebugStopEvent? stop = null;
            using var stopped = new SemaphoreSlim(0);
            session.Stopped += e => { stop = e; stopped.Release(); };

            var task = session.RunAsync(DebugLauncher.BuildInvocation(hash, "add", 5, 7));

            Assert.IsTrue(stopped.Wait(TimeSpan.FromSeconds(15)), "execution should stop at the C# breakpoint");
            Assert.AreEqual(expectedLine, stop!.Line);
            Assert.IsTrue(session.IsPaused);

            session.Continue();
            Assert.IsTrue(task.Wait(TimeSpan.FromSeconds(15)), "execution should resume and finish");
            Assert.AreEqual(new BigInteger(12), ((StackItem)task.Result).GetInteger());
        }

        private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo, string sourceFile) Compile()
        {
            var sourceFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
            File.WriteAllText(sourceFile, Source);
            try
            {
                var options = new CompilationOptions
                {
                    // A debug build (Extended, unoptimized) gives clean source maps so every
                    // statement line can bind a breakpoint.
                    Debug = CompilationOptions.DebugType.Extended,
                    Optimize = CompilationOptions.OptimizationType.None,
                    Nullable = NullableContextOptions.Enable,
                    SkipRestoreIfAssetsPresent = true
                };

                var engine = new CompilationEngine(options);
                var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
                var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

                var contexts = engine.CompileSources(new CompilationSourceReferences { Projects = new[] { frameworkProject } }, sourceFile);
                Assert.AreEqual(1, contexts.Count);
                var context = contexts[0];
                Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

                var (nef, manifest, debugInfoJson) = context.CreateResults();
                var debugInfo = NeoDebugInfo.FromDebugInfoJson(debugInfoJson);
                return (nef, manifest, debugInfo, sourceFile);
            }
            finally
            {
                if (File.Exists(sourceFile))
                    File.Delete(sourceFile);
            }
        }
    }
}
