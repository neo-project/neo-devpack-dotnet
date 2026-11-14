// Copyright (C) 2015-2026 The Neo Project.
//
// EmptyCollectionOpcodesTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.ControlFlow;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StackItem = Neo.VM.Types.StackItem;
using VmArray = Neo.VM.Types.Array;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class EmptyCollectionOpcodesTests
    {
        public TestContext TestContext { get; set; } = null!;

        private static byte[] Bytes(params OpCode[] opCodes) => opCodes.Select(static op => (byte)op).ToArray();

        // PUSH1 JMPIF(->PUSH0) PUSH5 DROP PUSH0 <collection op> RET, preceded by a rewritable pair so that offsets shift.
        private static byte[] JumpToPatternStartScript(OpCode collectionOp) =>
        [
            (byte)OpCode.PUSH0, (byte)collectionOp, (byte)OpCode.DROP,
            (byte)OpCode.PUSH1, (byte)OpCode.JMPIF, 4,
            (byte)OpCode.PUSH5, (byte)OpCode.DROP,
            (byte)OpCode.PUSH0, (byte)collectionOp, (byte)OpCode.RET
        ];

        // PUSH0 PUSH1 JMPIF(->collectionOp) DROP PUSH0 <collection op> RET: the jump lands on the second instruction of the pattern.
        private static byte[] JumpToPatternSecondScript(OpCode collectionOp) =>
        [
            (byte)OpCode.PUSH0, (byte)OpCode.PUSH1, (byte)OpCode.JMPIF, 4,
            (byte)OpCode.DROP, (byte)OpCode.PUSH0, (byte)collectionOp, (byte)OpCode.RET
        ];

        private static (VMState State, string[] Stack) Run(byte[] script)
        {
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            VMState state = engine.Execute();
            return (state, engine.ResultStack.Select(Describe).ToArray());
        }

        private static string Describe(StackItem item)
        {
            if (item is VmArray array)
                return $"{item.Type}[{string.Join(",", Enumerable.Range(0, array.Count).Select(i => Describe(array[i])))}]";
            return $"{item.Type}:{(item.IsNull ? "" : Convert.ToHexString(item.GetSpan().ToArray()))}";
        }

        private static byte[] Apply(byte[] script)
        {
            var (nef, _, _) = Peephole.UseEmptyCollectionOpcodes(CreateNefFile(script), CreateManifest());
            return nef.Script.ToArray();
        }

        private static OpCode[] OpCodesOf(byte[] script) =>
            new Script(script).EnumerateInstructions().Select(static item => item.instruction.OpCode).ToArray();

        private static void AssertSameBehavior(byte[] original, byte[] optimized, string expectedResult)
        {
            var expected = Run(original);
            var actual = Run(optimized);
            Assert.AreEqual(VMState.HALT, expected.State);
            Assert.AreEqual(VMState.HALT, actual.State);
            CollectionAssert.AreEqual(expected.Stack, actual.Stack);
            Assert.AreEqual(expectedResult, actual.Stack.Single());
        }

        [TestMethod]
        [DataRow(OpCode.PACK, OpCode.NEWARRAY0, "Array[]")]
        [DataRow(OpCode.NEWARRAY, OpCode.NEWARRAY0, "Array[]")]
        [DataRow(OpCode.NEWSTRUCT, OpCode.NEWSTRUCT0, "Struct[]")]
        public void Rewrites_Push0AndCollectionOp_ToDedicatedOpcode(OpCode op, OpCode expectedOp, string expectedResult)
        {
            byte[] original = Bytes(OpCode.PUSH0, op, OpCode.RET);
            byte[] optimized = Apply(original);

            CollectionAssert.AreEqual(new[] { expectedOp, OpCode.RET }, OpCodesOf(optimized));
            AssertSameBehavior(original, optimized, expectedResult);
        }

        [TestMethod]
        [DataRow(OpCode.PACK)]
        [DataRow(OpCode.NEWARRAY)]
        [DataRow(OpCode.NEWSTRUCT)]
        public void Rewrite_PreservesIdentitySemantics(OpCode op)
        {
            // DUP EQUAL compares a reference with itself; two fresh collections follow the VM equality rules of their type.
            byte[] sameRef = Bytes(OpCode.PUSH0, op, OpCode.DUP, OpCode.EQUAL, OpCode.RET);
            byte[] differentRefs = Bytes(OpCode.PUSH0, op, OpCode.PUSH0, op, OpCode.EQUAL, OpCode.RET);

            foreach (byte[] script in new[] { sameRef, differentRefs })
            {
                byte[] optimized = Apply(script);
                Assert.IsLessThan(script.Length, optimized.Length);
                var expected = Run(script);
                var actual = Run(optimized);
                Assert.AreEqual(VMState.HALT, actual.State);
                CollectionAssert.AreEqual(expected.Stack, actual.Stack);
            }
        }

        [TestMethod]
        [DataRow(OpCode.PACK, OpCode.NEWARRAY0, "Array[]")]
        [DataRow(OpCode.NEWARRAY, OpCode.NEWARRAY0, "Array[]")]
        [DataRow(OpCode.NEWSTRUCT, OpCode.NEWSTRUCT0, "Struct[]")]
        public void Rewrite_RetargetsJumpToPatternStart(OpCode op, OpCode expectedOp, string expectedResult)
        {
            byte[] original = JumpToPatternStartScript(op);
            byte[] optimized = Apply(original);

            CollectionAssert.AreEqual(
                new[] { expectedOp, OpCode.DROP, OpCode.PUSH1, OpCode.JMPIF, OpCode.PUSH5, OpCode.DROP, expectedOp, OpCode.RET },
                OpCodesOf(optimized));

            Script script = new(optimized);
            var instructions = script.EnumerateInstructions().ToArray();
            var jump = instructions.Single(static item => item.instruction.OpCode == OpCode.JMPIF);
            int target = jump.address + jump.instruction.TokenI8;
            Assert.AreEqual(expectedOp, script.GetInstruction(target).OpCode);
            Assert.AreEqual(instructions[6].address, target);

            AssertSameBehavior(original, optimized, expectedResult);
        }

        [TestMethod]
        [DataRow(OpCode.PACK)]
        [DataRow(OpCode.NEWARRAY)]
        [DataRow(OpCode.NEWSTRUCT)]
        public void Rewrite_IsSkipped_WhenSecondInstructionIsJumpTarget(OpCode op)
        {
            byte[] original = JumpToPatternSecondScript(op);
            byte[] optimized = Apply(original);

            CollectionAssert.AreEqual(original, optimized);
            AssertSameBehavior(original, optimized, op == OpCode.NEWSTRUCT ? "Struct[]" : "Array[]");
        }

        [TestMethod]
        [DataRow(OpCode.PACK)]
        [DataRow(OpCode.NEWARRAY)]
        [DataRow(OpCode.NEWSTRUCT)]
        public void Rewrite_IsSkipped_WhenInstructionsBelongToDifferentBasicBlocks(OpCode op)
        {
            byte[] script = JumpToPatternSecondScript(op);
            ContractInBasicBlocks blocks = new(CreateNefFile(script), CreateManifest(), null);

            foreach (var (_, block) in blocks.sortedListInstructions)
            {
                for (int i = 0; i + 1 < block.Count; i++)
                    Assert.IsFalse(block[i].OpCode == OpCode.PUSH0 && block[i + 1].OpCode == op);
            }

            CollectionAssert.AreEqual(script, Apply(script));
        }

        [TestMethod]
        public void Rewrite_DoesNotChangeOtherPatterns()
        {
            byte[][] scripts =
            [
                Bytes(OpCode.PUSH1, OpCode.PUSH1, OpCode.PACK, OpCode.RET),
                Bytes(OpCode.PUSH1, OpCode.NEWARRAY, OpCode.RET),
                Bytes(OpCode.PUSH1, OpCode.NEWSTRUCT, OpCode.RET),
                [(byte)OpCode.PUSH0, (byte)OpCode.NEWARRAY_T, (byte)Neo.VM.Types.StackItemType.Integer, (byte)OpCode.RET]
            ];

            foreach (byte[] script in scripts)
            {
                byte[] optimized = Apply(script);
                CollectionAssert.AreEqual(script, optimized);
                var expected = Run(script);
                var actual = Run(optimized);
                Assert.AreEqual(VMState.HALT, actual.State);
                CollectionAssert.AreEqual(expected.Stack, actual.Stack);
            }
        }

        [TestMethod]
        [DataRow(OpCode.PACK, OpCode.NEWARRAY0)]
        [DataRow(OpCode.NEWARRAY, OpCode.NEWARRAY0)]
        [DataRow(OpCode.NEWSTRUCT, OpCode.NEWSTRUCT0)]
        public void FullPipeline_AppliesRewrite_AndKeepsManifestOffsetsValid(OpCode op, OpCode expectedOp)
        {
            byte[] original = JumpToPatternStartScript(op);
            ContractManifest manifest = CreateManifest();
            var (nef, optimizedManifest, _) = Neo.Optimizer.Optimizer.Optimize(
                CreateNefFile(original), manifest, null, CompilationOptions.OptimizationType.All);
            byte[] optimized = nef.Script.ToArray();

            Script strict = new(optimized, true);
            var instructions = strict.EnumerateInstructions().ToArray();
            Assert.Contains(expectedOp, instructions.Select(static item => item.instruction.OpCode));
            for (int i = 0; i + 1 < instructions.Length; i++)
                Assert.IsFalse(instructions[i].instruction.OpCode == OpCode.PUSH0 && instructions[i + 1].instruction.OpCode == op);

            HashSet<int> boundaries = instructions.Select(static item => item.address).ToHashSet();
            foreach (var method in optimizedManifest.Abi.Methods)
                Assert.Contains(method.Offset, boundaries);

            var expected = Run(original);
            var actual = Run(optimized);
            Assert.AreEqual(VMState.HALT, actual.State);
            CollectionAssert.AreEqual(expected.Stack, actual.Stack);
        }

        [TestMethod]
        [DataRow(OpCode.PACK)]
        [DataRow(OpCode.NEWARRAY)]
        [DataRow(OpCode.NEWSTRUCT)]
        public void Rewrite_IsIdempotent(OpCode op)
        {
            foreach (byte[] script in new[] { Bytes(OpCode.PUSH0, op, OpCode.RET), JumpToPatternStartScript(op) })
            {
                byte[] once = Apply(script);
                byte[] twice = Apply(once);
                CollectionAssert.AreEqual(once, twice);
            }
        }

        [TestMethod]
        [DataRow(OpCode.PACK, OpCode.NEWARRAY0)]
        [DataRow(OpCode.NEWARRAY, OpCode.NEWARRAY0)]
        [DataRow(OpCode.NEWSTRUCT, OpCode.NEWSTRUCT0)]
        public void Gas_IsReduced_ForEveryHardfork(OpCode op, OpCode expectedOp)
        {
            byte[] original = Bytes(OpCode.PUSH0, op, OpCode.RET);
            byte[] optimized = Bytes(expectedOp, OpCode.RET);

            foreach (Hardfork current in Enum.GetValues<Hardfork>().OrderBy(static h => (int)h))
            {
                long before = MeasureGas(original, current);
                long after = MeasureGas(optimized, current);
                long saving = before - after;
                TestContext.WriteLine(
                    $"PUSH0 {op} -> {expectedOp} | {current}: before={before} after={after} saving={saving} ({(double)saving / before:P2}) datoshi");
                Assert.IsGreaterThan(0L, saving, $"{op} must save GAS under {current}");
            }
        }

        private static long MeasureGas(byte[] script, Hardfork enabledUpTo)
        {
            ProtocolSettings settings = TestEngine.Default with
            {
                Hardforks = TestEngine.Default.Hardforks.ToImmutableDictionary(
                    static p => p.Key,
                    p => p.Key <= enabledUpTo ? 0u : uint.MaxValue)
            };
            TestEngine engine = new(settings, true);
            engine.Execute(new Script(script));
            return engine.FeeConsumed.Value;
        }

        private static NefFile CreateNefFile(byte[] script) => new()
        {
            Compiler = "test",
            Source = "test.cs",
            Tokens = Array.Empty<MethodToken>(),
            Script = script
        };

        private static ContractManifest CreateManifest() => new()
        {
            Name = "TestContract",
            Groups = Array.Empty<ContractGroup>(),
            SupportedStandards = Array.Empty<string>(),
            Abi = new ContractAbi
            {
                Methods =
                [
                    new ContractMethodDescriptor
                    {
                        Name = "main",
                        Offset = 0,
                        Parameters = Array.Empty<ContractParameterDefinition>(),
                        ReturnType = ContractParameterType.Any,
                        Safe = false
                    }
                ],
                Events = Array.Empty<ContractEventDescriptor>()
            },
            Permissions = Array.Empty<ContractPermission>(),
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            Extra = null
        };
    }
}
