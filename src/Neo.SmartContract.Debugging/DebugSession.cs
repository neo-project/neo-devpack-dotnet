// Copyright (C) 2015-2026 The Neo Project.
//
// DebugSession.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Debugging
{
    /// <summary>
    /// Drives a contract execution through the testing engine and pauses it at source-level
    /// breakpoints. It joins the two debugger primitives — the breakpoint resolver
    /// (<see cref="BreakpointResolver"/>) and the testing engine's per-instruction hook
    /// (<see cref="TestEngine.OnPreExecuteInstruction"/>) — into a single controllable session.
    /// </summary>
    /// <remarks>
    /// Execution runs on a worker thread (<see cref="RunAsync"/>). When the instruction pointer of
    /// the debugged contract reaches a breakpoint address, the session raises <see cref="Stopped"/>
    /// and blocks the execution thread until <see cref="Continue"/> is called. This is a test-mode
    /// debugger: it observes invocations made through the <see cref="TestEngine"/>.
    /// </remarks>
    public sealed class DebugSession : IDisposable
    {
        private readonly TestEngine _engine;
        private readonly NeoDebugInfo _debugInfo;
        private readonly UInt160 _contractHash;
        private readonly HashSet<int> _breakpointAddresses = new();
        private readonly AutoResetEvent _resume = new(false);
        private volatile bool _paused;
        private bool _attached;
        private bool _disposed;

        /// <param name="engine">The engine the debugged invocation runs in.</param>
        /// <param name="debugInfo">The debugged contract's source map.</param>
        /// <param name="contractHash">
        /// The script hash the debugged contract executes under — its deploy hash when the contract
        /// has been deployed, or the script's own hash when running a raw script directly. The
        /// session only breaks while execution is in a context with this hash.
        /// </param>
        public DebugSession(TestEngine engine, NeoDebugInfo debugInfo, UInt160 contractHash)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
            _debugInfo = debugInfo ?? throw new ArgumentNullException(nameof(debugInfo));
            _contractHash = contractHash ?? throw new ArgumentNullException(nameof(contractHash));
        }

        /// <summary>
        /// Raised, on the execution thread, when execution pauses at a breakpoint. Execution stays
        /// paused until <see cref="Continue"/> is called.
        /// </summary>
        public event Action<DebugStopEvent>? Stopped;

        /// <summary>Whether execution is currently paused at a breakpoint.</summary>
        public bool IsPaused => _paused;

        /// <summary>
        /// Resolves and installs breakpoints for the given source <paramref name="file"/> and
        /// <paramref name="lines"/>, returning the breakpoints that bound to executable code. Lines
        /// with no executable code at or after them are skipped.
        /// </summary>
        public IReadOnlyList<ResolvedBreakpoint> SetBreakpoints(string file, params int[] lines)
        {
            List<ResolvedBreakpoint> resolved = new();
            foreach (int line in lines)
            {
                if (_debugInfo.ResolveBreakpoint(file, line) is { } breakpoint)
                {
                    resolved.Add(breakpoint);
                    _breakpointAddresses.Add(breakpoint.Address);
                }
            }
            return resolved;
        }

        /// <summary>
        /// Runs <paramref name="script"/> (an invocation of the debugged contract) on a worker
        /// thread, pausing at any installed breakpoint. The returned task completes when execution
        /// finishes.
        /// </summary>
        public Task<StackItem> RunAsync(Script script, int initialPosition = 0)
        {
            Attach();
            return Task.Run(() => _engine.Execute(script, initialPosition));
        }

        /// <summary>Resumes execution after it has paused at a breakpoint. A no-op if not paused.</summary>
        public void Continue()
        {
            if (_paused)
                _resume.Set();
        }

        private void Attach()
        {
            if (_attached) return;
            _engine.OnPreExecuteInstruction += OnPreExecuteInstruction;
            _attached = true;
        }

        private void OnPreExecuteInstruction(ApplicationEngine engine, Instruction instruction)
        {
            // Only break inside the contract this session is debugging.
            if (engine.CurrentScriptHash != _contractHash)
                return;

            Neo.VM.ExecutionContext? context = engine.CurrentContext;
            if (context is null)
                return;

            int address = context.InstructionPointer;
            if (!_breakpointAddresses.Contains(address))
                return;

            NeoDebugInfo.SourceLocation? location = _debugInfo.GetSourceLocation(address);
            _paused = true;
            Stopped?.Invoke(new DebugStopEvent(
                location?.FileName ?? string.Empty,
                location?.Line ?? 0,
                location?.Column ?? 0,
                address));

            // Block the execution thread until the controller resumes us.
            _resume.WaitOne();
            _paused = false;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_attached)
                _engine.OnPreExecuteInstruction -= OnPreExecuteInstruction;
            _resume.Set();
            _resume.Dispose();
        }
    }
}
