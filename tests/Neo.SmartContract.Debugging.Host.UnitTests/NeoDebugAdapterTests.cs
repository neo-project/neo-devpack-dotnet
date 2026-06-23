// Copyright (C) 2015-2026 The Neo Project.
//
// NeoDebugAdapterTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Debugging.Host.UnitTests
{
    // Verifies that the DAP request handlers map onto the (separately-tested) DebugSession: the
    // adapter advertises the right capabilities, launches a compiled contract, and binds source
    // breakpoints. The stop/continue execution flow itself is covered by the DebugSession and
    // end-to-end tests in Neo.SmartContract.Debugging.
    [TestClass]
    public class NeoDebugAdapterTests
    {
        private const int BreakpointLine = 6; // "int sum = 5 + 7;"

        private const string ContractSource = @"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Run()
    {
        int sum = 5 + 7;
        return sum;
    }
}";

        // Exposes the protected DAP handlers for direct testing.
        private sealed class TestAdapter : NeoDebugAdapter
        {
            public TestAdapter() : base(Stream.Null, Stream.Null) { }
            public InitializeResponse Initialize() => HandleInitializeRequest(new InitializeArguments());
            public LaunchResponse Launch(IDictionary<string, JToken> cfg)
            {
                var args = new LaunchArguments();
                foreach (var kv in cfg) args.ConfigurationProperties[kv.Key] = kv.Value;
                return HandleLaunchRequest(args);
            }
            public SetBreakpointsResponse SetBreakpoints(string path, params int[] lines)
                => HandleSetBreakpointsRequest(new SetBreakpointsArguments
                {
                    Source = new Source { Name = Path.GetFileName(path), Path = path },
                    Breakpoints = lines.Select(l => new SourceBreakpoint(l)).ToList(),
                });
        }

        [TestMethod]
        public void Initialize_AdvertisesConfigurationDone()
        {
            var adapter = new TestAdapter();
            var response = adapter.Initialize();
            Assert.IsTrue(response.SupportsConfigurationDoneRequest == true);
        }

        [TestMethod]
        public void Launch_ThenSetBreakpoints_BindsSourceLine()
        {
            var (nefPath, manifestPath, debugPath, sourcePath) = CompileToFiles();
            var adapter = new TestAdapter();
            adapter.Initialize();

            adapter.Launch(new Dictionary<string, JToken>
            {
                ["nef"] = nefPath,
                ["manifest"] = manifestPath,
                ["debugInfo"] = debugPath,
                ["method"] = "run",
            });

            var response = adapter.SetBreakpoints(sourcePath, BreakpointLine);
            Assert.AreEqual(1, response.Breakpoints.Count);
            Assert.IsTrue(response.Breakpoints[0].Verified, "the breakpoint should bind to a real source line");
            Assert.AreEqual(BreakpointLine, response.Breakpoints[0].Line);
        }

        [TestMethod]
        public void Launch_MissingRequiredField_Throws()
        {
            var adapter = new TestAdapter();
            adapter.Initialize();
            Assert.ThrowsException<ProtocolException>(() =>
                adapter.Launch(new Dictionary<string, JToken> { ["method"] = "run" }));
        }

        private static (string nef, string manifest, string debug, string source) CompileToFiles()
        {
            var sourcePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
            File.WriteAllText(sourcePath, ContractSource);

            var options = new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended,
                Optimize = CompilationOptions.OptimizationType.None,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };
            var engine = new CompilationEngine(options);
            var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var context = engine.CompileSources(new CompilationSourceReferences { Projects = new[] { frameworkProject } }, sourcePath)[0];
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
            var (nef, manifest, debugInfoJson) = context.CreateResults();

            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var nefPath = basePath + ".nef";
            var manifestPath = basePath + ".manifest.json";
            var debugPath = basePath + ".debug.json";
            File.WriteAllBytes(nefPath, nef.ToArray());
            File.WriteAllText(manifestPath, manifest.ToJson().ToString());
            File.WriteAllText(debugPath, debugInfoJson.ToString());
            return (nefPath, manifestPath, debugPath, sourcePath);
        }
    }
}
