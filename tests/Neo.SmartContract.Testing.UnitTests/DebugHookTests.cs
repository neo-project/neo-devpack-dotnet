// Copyright (C) 2015-2026 The Neo Project.
//
// DebugHookTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class DebugHookTests
    {
        // PUSH5 PUSH7 ADD -> 12. Three single-byte instructions at addresses 0, 1, 2.
        private static byte[] AddScript()
        {
            using ScriptBuilder sb = new();
            sb.EmitPush(5);
            sb.EmitPush(7);
            sb.Emit(OpCode.ADD);
            return sb.ToArray();
        }

        [TestMethod]
        public void OnPreExecuteInstruction_FiresForEachInstruction()
        {
            var engine = new TestEngine(true);
            var opcodes = new List<OpCode>();
            var addresses = new List<int>();
            engine.OnPreExecuteInstruction += (e, instruction) =>
            {
                opcodes.Add(instruction.OpCode);
                addresses.Add(e.CurrentContext!.InstructionPointer);
            };

            var result = engine.Execute(AddScript());

            Assert.AreEqual(new BigInteger(12), result.GetInteger());
            // The hook fires once per executed instruction, in execution order: the three emitted
            // instructions (PUSH 5, PUSH 7, ADD) plus the implicit terminal RET.
            Assert.IsTrue(opcodes.Count >= 3, "the hook should fire for every executed instruction");
            Assert.AreEqual(OpCode.ADD, opcodes[2]);
            Assert.AreEqual(0, addresses[0], "the first instruction is at the script entry");
            for (int i = 1; i < addresses.Count; i++)
                Assert.IsTrue(addresses[i] > addresses[i - 1], "instruction addresses advance monotonically");
        }

        [TestMethod]
        public void NoSubscriber_ExecutesNormally()
        {
            var engine = new TestEngine(true);
            Assert.AreEqual(new BigInteger(12), engine.Execute(AddScript()).GetInteger());
        }

        [TestMethod]
        public void OnPreExecuteInstruction_BlockingHandler_PausesAndResumesExecution()
        {
            var engine = new TestEngine(true);
            using var paused = new SemaphoreSlim(0);
            using var resume = new SemaphoreSlim(0);
            var firstHit = false;

            engine.OnPreExecuteInstruction += (e, instruction) =>
            {
                if (firstHit) return;
                firstHit = true;
                paused.Release(); // tell the test we reached (and are about to block on) the first instruction
                resume.Wait();    // block the VM thread until the test resumes it
            };

            var task = Task.Run(() => engine.Execute(AddScript()).GetInteger());

            Assert.IsTrue(paused.Wait(TimeSpan.FromSeconds(5)), "the debug hook should have been hit");
            // The VM thread is blocked inside the handler, so execution must not have completed.
            Assert.IsFalse(task.Wait(TimeSpan.FromMilliseconds(250)), "execution should be paused inside the handler");

            resume.Release();
            Assert.IsTrue(task.Wait(TimeSpan.FromSeconds(5)), "execution should resume after the handler returns");
            Assert.AreEqual(new BigInteger(12), task.Result);
        }
    }
}
