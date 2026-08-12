// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ControlFlowDslEmitSwitch.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ControlFlowDslEmitSwitch
    {
        // PUSH1/PUSH2/PUSH3 opcodes are contiguous (0x11, 0x12, 0x13).
        private static readonly byte OpPush0 = (byte)OpCode.PUSH0;
        private static readonly byte OpPush1 = (byte)OpCode.PUSH1;
        private static readonly byte OpPush2 = (byte)OpCode.PUSH2;
        private static readonly byte OpPush3 = (byte)OpCode.PUSH3;

        private static TestEngine _engine = null!;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
        {
            _engine = new TestEngine(true);
        }

        /// <summary>
        /// Builds a 3-case integer switch using the OLD dispatch pattern:
        ///   DUP  PUSH(val)  NUMEQUAL  JMPIF
        ///
        /// The discriminant (0–16) is prepended as a PUSH opcode inline so the
        /// script is self-contained with no arguments.
        /// </summary>
        private static byte[] BuildOldScript(int discriminant)
        {
            var b = new List<byte>();
            void W(params byte[] bytes) => b.AddRange(bytes);

            // Inline discriminant
            W((byte)((byte)OpCode.PUSH0 + discriminant));

            var jmpPos = new int[3];

            W((byte)OpCode.DUP, OpPush1, (byte)OpCode.NUMEQUAL, (byte)OpCode.JMPIF);
            jmpPos[0] = b.Count;
            W(0);

            W((byte)OpCode.DUP, OpPush2, (byte)OpCode.NUMEQUAL, (byte)OpCode.JMPIF);
            jmpPos[1] = b.Count;
            W(0);

            W((byte)OpCode.DUP, OpPush3, (byte)OpCode.NUMEQUAL, (byte)OpCode.JMPIF);
            jmpPos[2] = b.Count;
            W(0);

            W((byte)OpCode.JMP);
            var defaultJmpPos = b.Count;
            W(0);

            // Case bodies (DROP + return value + RET) and forward-patch JMPIF offsets.
            // JMPIF offset is relative to the byte AFTER the offset byte itself.
            for (var i = 0; i < 3; i++)
            {
                b[jmpPos[i]] = (byte)(b.Count - (jmpPos[i] + 1));
                W((byte)OpCode.DROP);
                W((byte)((byte)OpCode.PUSH1 + i)); // PUSH1, PUSH2, PUSH3
                W((byte)OpCode.RET);
            }

            b[defaultJmpPos] = (byte)(b.Count - (defaultJmpPos + 1));
            W((byte)OpCode.DROP, OpPush0, (byte)OpCode.RET);

            return [.. b];
        }

        /// <summary>
        /// Builds the same 3-case integer switch using the OPTIMIZED dispatch pattern:
        ///   DUP  PUSH(val)  JMPEQ   (no NUMEQUAL)
        /// </summary>
        private static byte[] BuildNewScript(int discriminant)
        {
            var b = new List<byte>();
            void W(params byte[] bytes) => b.AddRange(bytes);

            W((byte)((byte)OpCode.PUSH0 + discriminant));

            int[] jmpPos = new int[3];

            W((byte)OpCode.DUP, OpPush1, (byte)OpCode.JMPEQ);
            jmpPos[0] = b.Count; W(0);

            W((byte)OpCode.DUP, OpPush2, (byte)OpCode.JMPEQ);
            jmpPos[1] = b.Count; W(0);

            W((byte)OpCode.DUP, OpPush3, (byte)OpCode.JMPEQ);
            jmpPos[2] = b.Count; W(0);

            W((byte)OpCode.JMP);
            int defaultJmpPos = b.Count; W(0);

            for (int i = 0; i < 3; i++)
            {
                b[jmpPos[i]] = (byte)(b.Count - (jmpPos[i] + 1));
                W((byte)OpCode.DROP);
                W((byte)((byte)OpCode.PUSH1 + i));
                W((byte)OpCode.RET);
            }

            b[defaultJmpPos] = (byte)(b.Count - (defaultJmpPos + 1));
            W((byte)OpCode.DROP, OpPush0, (byte)OpCode.RET);

            return [.. b];
        }

        private static (int result, long gasConsumed) Execute(byte[] scriptBytes)
        {
            var result = _engine.Execute(new Script(scriptBytes));
            return ((int)result.GetInteger(), _engine.FeeConsumed.Value);
        }

        /// <summary>
        /// Verifies that the JMPEQ dispatch returns the same value as the
        /// NUMEQUAL+JMPIF dispatch for every branch (cases 1, 2, 3 and default).
        /// </summary>
        [TestMethod]
        public void Test_EmitSwitch_Optimized_CorrectResults_MatchAndDefault()
        {
            foreach (int disc in new[] { 1, 2, 3, 0 })
            {
                var (oldResult, _) = Execute(BuildOldScript(disc));
                var (optResult, _) = Execute(BuildNewScript(disc));
                Assert.AreEqual(oldResult, optResult, $"Discriminant={disc}: JMPEQ result {optResult} must equal NUMEQUAL+JMPIF result {oldResult}");
            }
        }

        /// <summary>
        /// Verifies that the JMPEQ dispatch is strictly cheaper than NUMEQUAL+JMPIF,
        /// and that the worst-case saving (all 3 cases traversed) equals exactly
        /// 3 × 240 datoshi = 720 datoshi.
        /// </summary>
        [TestMethod]
        public void Test_EmitSwitch_JMPEQ_SavesGas_Versus_NumEqualJmpIf()
        {
            const long savingPerCase = 8 * 30; // 8 opcode units × 30 datoshi/unit = 240 datoshi

            // Worst case: discriminant=3
            var (_, oldGas) = Execute(BuildOldScript(3));
            var (_, newGas) = Execute(BuildNewScript(3));

            Assert.IsTrue(newGas < oldGas, $"Optimized script must consume less gas. Old={oldGas} datoshi, New={newGas} datoshi");

            long saving = oldGas - newGas;
            long expected = savingPerCase * 3; // 720 datoshi
            Assert.AreEqual(expected, saving, $"Expected saving of {expected} datoshi (3 cases × {savingPerCase}), but got {saving}");
        }

        /// <summary>
        /// Verifies that the gas saving scales linearly: each additional comparison
        /// before a match saves exactly 240 datoshi (one NUMEQUAL eliminated).
        /// </summary>
        [TestMethod]
        public void Test_EmitSwitch_JMPEQ_GasSaving_ScalesWithCasesTraversed()
        {
            const long savingPerCase = 8 * 30; // 240 datoshi per comparison eliminated

            // 1 comparison traversed
            var (_, oldGas1) = Execute(BuildOldScript(1));
            var (_, newGas1) = Execute(BuildNewScript(1));
            Assert.AreEqual(savingPerCase * 1, oldGas1 - newGas1, "Discriminant=1 (1 comparison): expected saving of 240 datoshi");

            // 2 comparisons traversed (case 2 matches on second try)
            var (_, oldGas2) = Execute(BuildOldScript(2));
            var (_, newGas2) = Execute(BuildNewScript(2));
            Assert.AreEqual(savingPerCase * 2, oldGas2 - newGas2, "Discriminant=2 (2 comparisons): expected saving of 480 datoshi");

            // 3 comparisons traversed (case 3 matches on third try)
            var (_, oldGas3) = Execute(BuildOldScript(3));
            var (_, newGas3) = Execute(BuildNewScript(3));
            Assert.AreEqual(savingPerCase * 3, oldGas3 - newGas3, "Discriminant=3 (3 comparisons): expected saving of 720 datoshi");
        }
    }
}
