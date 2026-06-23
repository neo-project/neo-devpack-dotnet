// Copyright (C) 2015-2026 The Neo Project.
//
// DebugSessionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Neo.SmartContract.Debugging.UnitTests
{
    [TestClass]
    public class DebugSessionTests
    {
        private const string Doc = "/src/Test.cs";

        // PUSH5 PUSH7 ADD -> 12. Address 0 maps to line 5, address 2 (the ADD) maps to line 6.
        private static (byte[] script, NeoDebugInfo debugInfo) BuildFixture()
        {
            using ScriptBuilder sb = new();
            sb.EmitPush(5);
            sb.EmitPush(7);
            sb.Emit(OpCode.ADD);
            byte[] script = sb.ToArray();

            var method = new NeoDebugInfo.Method("0", "T", "main", (0, 3),
                new List<NeoDebugInfo.Parameter>(),
                new List<NeoDebugInfo.SequencePoint>
                {
                    new(0, 0, (5, 1), (5, 10)),
                    new(2, 0, (6, 1), (6, 10)),
                });

            var debugInfo = new NeoDebugInfo(script.ToScriptHash(), "/src",
                new List<string> { Doc }, new List<NeoDebugInfo.Method> { method });

            return (script, debugInfo);
        }

        [TestMethod]
        public void PausesAtBreakpoint_ThenContinuesToCompletion()
        {
            var (script, debugInfo) = BuildFixture();
            var engine = new TestEngine(true);
            using var session = new DebugSession(engine, debugInfo, script.ToScriptHash());

            var breakpoints = session.SetBreakpoints(Doc, 6);
            Assert.AreEqual(1, breakpoints.Count);
            Assert.AreEqual(2, breakpoints[0].Address);

            DebugStopEvent? stop = null;
            using var stopped = new SemaphoreSlim(0);
            session.Stopped += e => { stop = e; stopped.Release(); };

            var task = session.RunAsync(script);

            Assert.IsTrue(stopped.Wait(TimeSpan.FromSeconds(5)), "execution should pause at the breakpoint");
            Assert.IsNotNull(stop);
            Assert.AreEqual(6, stop!.Line);
            Assert.AreEqual(2, stop.Address);
            Assert.IsTrue(session.IsPaused);
            Assert.IsFalse(task.Wait(TimeSpan.FromMilliseconds(250)), "execution should stay paused until Continue");

            session.Continue();
            Assert.IsTrue(task.Wait(TimeSpan.FromSeconds(5)), "execution should resume and finish");
            Assert.AreEqual(new BigInteger(12), task.Result.GetInteger());
            Assert.IsFalse(session.IsPaused);
        }

        [TestMethod]
        public void NoBreakpoints_RunsToCompletionWithoutPausing()
        {
            var (script, debugInfo) = BuildFixture();
            var engine = new TestEngine(true);
            using var session = new DebugSession(engine, debugInfo, script.ToScriptHash());

            var stoppedFired = false;
            session.Stopped += _ => stoppedFired = true;

            var result = session.RunAsync(script).GetAwaiter().GetResult();

            Assert.AreEqual(new BigInteger(12), result.GetInteger());
            Assert.IsFalse(stoppedFired);
            Assert.IsFalse(session.IsPaused);
        }

        [TestMethod]
        public void BreakpointOnUnknownFile_IsNotInstalled()
        {
            var (script, debugInfo) = BuildFixture();
            var engine = new TestEngine(true);
            using var session = new DebugSession(engine, debugInfo, script.ToScriptHash());

            var breakpoints = session.SetBreakpoints("/src/Other.cs", 6);
            Assert.AreEqual(0, breakpoints.Count);

            var stoppedFired = false;
            session.Stopped += _ => stoppedFired = true;

            var result = session.RunAsync(script).GetAwaiter().GetResult();
            Assert.AreEqual(new BigInteger(12), result.GetInteger());
            Assert.IsFalse(stoppedFired);
        }
    }
}
