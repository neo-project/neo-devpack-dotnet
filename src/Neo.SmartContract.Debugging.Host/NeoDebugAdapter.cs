// Copyright (C) 2015-2026 The Neo Project.
//
// NeoDebugAdapter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using Neo.Extensions;
using Newtonsoft.Json.Linq;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Debugging.Host
{
    /// <summary>
    /// A Debug Adapter Protocol (DAP) server for Neo smart contracts. It exposes the in-repo
    /// <see cref="DebugSession"/> over DAP so an editor (for example VS Code) can launch a compiled
    /// contract, set source breakpoints, and step through it.
    /// </summary>
    /// <remarks>
    /// This is a test-mode debugger: it deploys the contract into a <see cref="TestEngine"/> and
    /// debugs that invocation. The launch request carries the paths to the compiled artifacts
    /// (<c>nef</c>, <c>manifest</c>, <c>debugInfo</c>), the entry <c>method</c>, and optional
    /// <c>args</c>.
    /// </remarks>
    public class NeoDebugAdapter : DebugAdapterBase
    {
        private const int MainThreadId = 1;

        private NeoDebugInfo? _debugInfo;
        private DebugSession? _session;
        private Neo.VM.Script? _invocation;
        private DebugStopEvent? _lastStop;

        public NeoDebugAdapter(Stream input, Stream output)
        {
            InitializeProtocolClient(input, output);
        }

        /// <summary>Runs the protocol loop until the client disconnects.</summary>
        public void Run()
        {
            Protocol.Run();
            Protocol.WaitForReader();
        }

        protected override InitializeResponse HandleInitializeRequest(InitializeArguments arguments)
        {
            Protocol.SendEvent(new InitializedEvent());
            return new InitializeResponse { SupportsConfigurationDoneRequest = true };
        }

        protected override LaunchResponse HandleLaunchRequest(LaunchArguments arguments)
        {
            IDictionary<string, JToken> cfg = arguments.ConfigurationProperties;
            string nefPath = Require(cfg, "nef");
            string manifestPath = Require(cfg, "manifest");
            string debugInfoPath = Require(cfg, "debugInfo");
            string method = Require(cfg, "method");
            object[] args = ParseArgs(cfg);

            NefFile nef = File.ReadAllBytes(nefPath).AsSerializable<NefFile>();
            ContractManifest manifest = ContractManifest.Parse(File.ReadAllText(manifestPath));
            if (!NeoDebugInfo.TryLoad(debugInfoPath, out _debugInfo))
                throw new ProtocolException($"Could not load debug info from '{debugInfoPath}'.");

            var engine = new TestEngine(true);
            UInt160 hash = DebugLauncher.Deploy(engine, nef, manifest);
            _session = new DebugSession(engine, _debugInfo, hash);
            _session.Stopped += OnStopped;
            _invocation = DebugLauncher.BuildInvocation(hash, method, args);
            return new LaunchResponse();
        }

        protected override SetBreakpointsResponse HandleSetBreakpointsRequest(SetBreakpointsArguments arguments)
        {
            string path = arguments.Source.Path;
            int[] requested = arguments.Breakpoints?.Select(b => b.Line).ToArray() ?? Array.Empty<int>();
            _session?.SetBreakpoints(path, requested);

            var breakpoints = new List<Breakpoint>(requested.Length);
            foreach (int line in requested)
            {
                ResolvedBreakpoint? bound = _debugInfo?.ResolveBreakpoint(path, line);
                breakpoints.Add(bound is { } b
                    ? new Breakpoint(verified: true) { Line = b.Line, Source = arguments.Source }
                    : new Breakpoint(verified: false) { Line = line, Source = arguments.Source });
            }
            return new SetBreakpointsResponse(breakpoints);
        }

        protected override ConfigurationDoneResponse HandleConfigurationDoneRequest(ConfigurationDoneArguments arguments)
        {
            _session!.RunAsync(_invocation!).ContinueWith(_ => Protocol.SendEvent(new TerminatedEvent()));
            return new ConfigurationDoneResponse();
        }

        protected override ContinueResponse HandleContinueRequest(ContinueArguments arguments)
        {
            _session?.Continue();
            return new ContinueResponse { AllThreadsContinued = true };
        }

        protected override ThreadsResponse HandleThreadsRequest(ThreadsArguments arguments)
            => new ThreadsResponse(new List<Thread> { new Thread(MainThreadId, "main") });

        protected override StackTraceResponse HandleStackTraceRequest(StackTraceArguments arguments)
        {
            var frames = new List<StackFrame>();
            if (_lastStop is { } stop)
            {
                var source = new Source { Name = Path.GetFileName(stop.File), Path = stop.File };
                frames.Add(new StackFrame(0, "frame", stop.Line, stop.Column) { Source = source });
            }
            return new StackTraceResponse(frames);
        }

        protected override DisconnectResponse HandleDisconnectRequest(DisconnectArguments arguments)
        {
            _session?.Continue();
            _session?.Dispose();
            return new DisconnectResponse();
        }

        private void OnStopped(DebugStopEvent stop)
        {
            _lastStop = stop;
            Protocol.SendEvent(new StoppedEvent(StoppedEvent.ReasonValue.Breakpoint) { ThreadId = MainThreadId });
        }

        private static string Require(IDictionary<string, JToken> cfg, string key)
        {
            if (!cfg.TryGetValue(key, out JToken? value) || value is null || value.Type == JTokenType.Null)
                throw new ProtocolException($"The launch configuration is missing required field '{key}'.");
            return value.ToString();
        }

        private static object[] ParseArgs(IDictionary<string, JToken> cfg)
        {
            if (!cfg.TryGetValue("args", out JToken? raw) || raw is null || raw.Type == JTokenType.Null)
                return Array.Empty<object>();
            if (raw is JArray array)
                return array.Select(t => t.ToObject<object>()!).ToArray();
            return new object[] { raw.ToObject<object>()! };
        }
    }
}
