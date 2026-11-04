// Copyright (C) 2015-2026 The Neo Project.
//
// ControlFlowValidationTests.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.Collections.Generic;
using ControlFlowJumpTarget = Neo.Compiler.ControlFlow.JumpTarget;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class ControlFlowValidationTests
    {
        [TestMethod]
        public void FindAllJumpAndTrySourceToTargets_ThrowsBadScriptForInvalidJumpTarget()
        {
            var script = new Script(new byte[] { (byte)OpCode.JMP, 0x03, (byte)OpCode.RET });

            var exception = Assert.ThrowsExactly<BadScriptException>(() =>
                ControlFlowJumpTarget.FindAllJumpAndTrySourceToTargets(script));

            StringAssert.Contains(exception.Message, "JMP");
            StringAssert.Contains(exception.Message, "3");
        }

        [TestMethod]
        public void FindAllJumpAndTrySourceToTargets_ThrowsBadScriptForInvalidTryTarget()
        {
            var script = new Script(new byte[] { (byte)OpCode.TRY, 0x02, 0x00, (byte)OpCode.RET });

            var exception = Assert.ThrowsExactly<BadScriptException>(() =>
                ControlFlowJumpTarget.FindAllJumpAndTrySourceToTargets(script));

            StringAssert.Contains(exception.Message, "TRY");
            StringAssert.Contains(exception.Message, "2");
        }

        [TestMethod]
        public void ComputeJumpTarget_ThrowsBadScriptForDynamicCall()
        {
            var instruction = new Script(new byte[] { (byte)OpCode.CALLA }).GetInstruction(0);

            var exception = Assert.ThrowsExactly<BadScriptException>(() =>
                ControlFlowJumpTarget.ComputeJumpTarget(0, instruction));

            StringAssert.Contains(exception.Message, "CALLA");
        }

        [TestMethod]
        public void ComputeJumpTarget_ThrowsBadScriptForNonJumpInstruction()
        {
            var instruction = new Script(new byte[] { (byte)OpCode.NOP }).GetInstruction(0);

            var exception = Assert.ThrowsExactly<BadScriptException>(() =>
                ControlFlowJumpTarget.ComputeJumpTarget(0, instruction));

            StringAssert.Contains(exception.Message, "NOP");
        }

        [TestMethod]
        public void ComputeTryTarget_ThrowsBadScriptForNonTryInstruction()
        {
            var instruction = new Script(new byte[] { (byte)OpCode.RET }).GetInstruction(0);

            var exception = Assert.ThrowsExactly<BadScriptException>(() =>
                ControlFlowJumpTarget.ComputeTryTarget(0, instruction));

            StringAssert.Contains(exception.Message, "RET");
        }

        [TestMethod]
        public void InstructionCoverage_ThrowsBadScriptForTooDeepCallChain()
        {
            var script = new List<byte>();
            for (int i = 0; i <= InstructionCoverage.MaxControlFlowAnalysisDepth; i++)
            {
                script.Add((byte)OpCode.CALL);
                script.Add(0x02);
            }
            script.Add((byte)OpCode.RET);

            var exception = Assert.ThrowsExactly<BadScriptException>(() => new InstructionCoverage(CreateNefFile(script.ToArray()), CreateManifest()));

            StringAssert.Contains(exception.Message, "Control flow analysis depth");
        }

        [TestMethod]
        public void InstructionCoverage_ThrowsBadScriptInsteadOfOverflowingForVeryDeepCallChain()
        {
            const int chainLength = 50_000;
            var script = new List<byte>();
            for (int i = 0; i < chainLength; i++)
            {
                script.Add((byte)OpCode.CALL);
                script.Add(0x02);
            }
            script.Add((byte)OpCode.RET);

            var exception = Assert.ThrowsExactly<BadScriptException>(() => new InstructionCoverage(CreateNefFile(script.ToArray()), CreateManifest()));

            StringAssert.Contains(exception.Message, "Control flow analysis depth");
        }

        [TestMethod]
        public void InstructionCoverage_ThrowsBadScriptInsteadOfOverflowingForVeryDeepConditionalChain()
        {
            const int chainLength = 50_000;
            int returnAddress = chainLength * 5;
            var script = new List<byte>();
            for (int i = 0; i < chainLength; i++)
            {
                int instructionAddress = script.Count;
                script.Add((byte)OpCode.JMPIF_L);
                script.AddRange(BitConverter.GetBytes(returnAddress - instructionAddress));
            }
            script.Add((byte)OpCode.RET);

            var exception = Assert.ThrowsExactly<BadScriptException>(() => new InstructionCoverage(CreateNefFile(script.ToArray()), CreateManifest()));

            StringAssert.Contains(exception.Message, "Control flow analysis depth");
        }

        [TestMethod]
        public void InstructionCoverage_ThrowsBadScriptInsteadOfOverflowingForDeeplyNestedTry()
        {
            int tryCount = InstructionCoverage.MaxControlFlowAnalysisDepth + 1;
            int finallyAddress = tryCount * 9 + 1;
            var script = new List<byte>();
            for (int i = 0; i < tryCount; i++)
            {
                int instructionAddress = script.Count;
                script.Add((byte)OpCode.TRY_L);
                script.AddRange(BitConverter.GetBytes(0));
                script.AddRange(BitConverter.GetBytes(finallyAddress - instructionAddress));
            }
            script.Add((byte)OpCode.RET);
            script.Add((byte)OpCode.ENDFINALLY);

            var exception = Assert.ThrowsExactly<BadScriptException>(() => new InstructionCoverage(CreateNefFile(script.ToArray()), CreateManifest()));

            StringAssert.Contains(exception.Message, "Control flow analysis depth");
        }

        [TestMethod]
        public void InstructionCoverage_AllowsReasonableCallChain()
        {
            byte[] script =
            [
                (byte)OpCode.CALL, 0x02,
                (byte)OpCode.CALL, 0x02,
                (byte)OpCode.RET
            ];

            var coverage = new InstructionCoverage(CreateNefFile(script), CreateManifest());

            Assert.AreEqual(BranchType.OK, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[2]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[4]);
        }

        [TestMethod]
        public void InstructionCoverage_AbortRemainsTerminalInsideTry()
        {
            AssertAbortRemainsTerminalInsideTry(OpCode.ABORT);
        }

        [TestMethod]
        public void InstructionCoverage_AbortMsgRemainsTerminalInsideTry()
        {
            AssertAbortRemainsTerminalInsideTry(OpCode.ABORTMSG);
        }

        [TestMethod]
        public void InstructionCoverage_ThrowEntersExceptionHandlers()
        {
            var coverage = CreateTryCatchFinallyCoverage(OpCode.THROW);

            Assert.AreEqual(BranchType.OK, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[6]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[9]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[11]);
        }

        [TestMethod]
        [DataRow(OpCode.THROW, OpCode.NOP, BranchType.THROW)]
        [DataRow(OpCode.THROW, OpCode.ABORT, BranchType.ABORT)]
        [DataRow(OpCode.ABORT, OpCode.NOP, BranchType.ABORT)]
        public void CatchTerminationPreservesFaultThroughFinally(OpCode catchExit, OpCode finallyExit, BranchType expected)
        {
            byte[] script =
            [
                (byte)OpCode.TRY, 0x06, 0x09,
                (byte)OpCode.PUSH0, (byte)OpCode.THROW, (byte)OpCode.NOP,
                (byte)OpCode.DROP, (byte)OpCode.PUSH1, (byte)catchExit,
                (byte)finallyExit, (byte)OpCode.ENDFINALLY,
                (byte)OpCode.RET
            ];
            var nef = CreateNefFile(script);
            var manifest = CreateManifest();
            var coverage = new InstructionCoverage(nef, manifest);
            Assert.AreEqual(expected, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.UNCOVERED, coverage.coveredMap[11]);
            var handlers = new TryCatchFinallyCoverage(nef, manifest);
            var handler = System.Linq.Enumerable.Single(handlers.allTry.Values);
            Assert.IsTrue(handler.catchBlocks.Contains(handler.catchBlock!));
            Assert.AreEqual(catchExit != OpCode.ABORT, handler.finallyBlocks.Contains(handler.finallyBlock!));
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            Assert.AreEqual(VMState.FAULT, engine.Execute());
        }

        [TestMethod]
        [DataRow(OpCode.NOP, BranchType.THROW)]
        [DataRow(OpCode.ABORT, BranchType.ABORT)]
        public void UncaughtThrowRunsFinallyWithoutReachingContinuation(OpCode finallyExit, BranchType expected)
        {
            byte[] script =
            [
                (byte)OpCode.TRY, 0x00, 0x05,
                (byte)OpCode.PUSH0, (byte)OpCode.THROW,
                (byte)finallyExit, (byte)OpCode.ENDFINALLY,
                (byte)OpCode.RET
            ];
            var nef = CreateNefFile(script);
            var manifest = CreateManifest();
            var coverage = new InstructionCoverage(nef, manifest);
            Assert.AreEqual(expected, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.UNCOVERED, coverage.coveredMap[7]);
            var handlers = new TryCatchFinallyCoverage(nef, manifest);
            var handler = System.Linq.Enumerable.Single(handlers.allTry.Values);
            Assert.AreEqual(0, handler.catchBlocks.Count);
            Assert.IsTrue(handler.finallyBlocks.Contains(handler.finallyBlock!));
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            Assert.AreEqual(VMState.FAULT, engine.Execute());
        }

        [TestMethod]
        public void NormalTryFinallyKeepsItsContinuationReachable()
        {
            byte[] script =
            [
                (byte)OpCode.TRY, 0x00, 0x06,
                (byte)OpCode.NOP, (byte)OpCode.ENDTRY, 0x04,
                (byte)OpCode.PUSH1, (byte)OpCode.ENDFINALLY,
                (byte)OpCode.RET
            ];
            var nef = CreateNefFile(script);
            var manifest = CreateManifest();
            var coverage = new InstructionCoverage(nef, manifest);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[8]);
            var handlers = new TryCatchFinallyCoverage(nef, manifest);
            var handler = System.Linq.Enumerable.Single(handlers.allTry.Values);
            Assert.IsTrue(handler.finallyBlocks.Contains(handler.finallyBlock!));
            Assert.IsTrue(handler.endingBlocks.Contains(handlers.contractInBasicBlocks.basicBlocksByStartAddr[8]));
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            Assert.AreEqual(VMState.HALT, engine.Execute());
            Assert.AreEqual(1, engine.ResultStack.Count);
            Assert.AreEqual(System.Numerics.BigInteger.One, engine.ResultStack.Pop().GetInteger());
        }

        [TestMethod]
        [DataRow(OpCode.THROW, OpCode.THROW, BranchType.THROW)]
        [DataRow(OpCode.THROW, OpCode.ABORT, BranchType.THROW)]
        [DataRow(OpCode.ABORT, OpCode.ABORT, BranchType.ABORT)]
        public void ConditionalCatchTerminationHasNoSuccessfulPath(OpCode leftExit, OpCode rightExit, BranchType expected)
        {
            byte[] script =
            [
                (byte)OpCode.TRY, 0x06, 0x00,
                (byte)OpCode.PUSH0, (byte)OpCode.THROW, (byte)OpCode.NOP,
                (byte)OpCode.DROP, (byte)OpCode.PUSH0, (byte)OpCode.JMPIF, 0x04,
                (byte)OpCode.PUSH0, (byte)leftExit,
                (byte)OpCode.PUSH1, (byte)rightExit,
                (byte)OpCode.RET
            ];
            var nef = CreateNefFile(script);
            var manifest = CreateManifest();
            var coverage = new InstructionCoverage(nef, manifest);
            Assert.AreEqual(expected, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.UNCOVERED, coverage.coveredMap[14]);
            var handlers = new TryCatchFinallyCoverage(nef, manifest);
            var handler = System.Linq.Enumerable.Single(handlers.allTry.Values);
            Assert.IsTrue(handler.catchBlocks.Contains(handlers.contractInBasicBlocks.basicBlocksByStartAddr[10]));
            Assert.IsTrue(handler.catchBlocks.Contains(handlers.contractInBasicBlocks.basicBlocksByStartAddr[12]));
            using ExecutionEngine engine = new();
            engine.LoadScript(new Script(script));
            Assert.AreEqual(VMState.FAULT, engine.Execute());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ExceptionEntryPointsPreserveHandlerAndAbortSemantics(bool hasHandler)
        {
            var nef = CreateNefFile(CreateTryCatchFinallyScript(OpCode.THROW));
            var manifest = CreateManifest();
            manifest.Abi.Methods[0].Offset = 11;
            var coverage = new InstructionCoverage(nef, manifest);
            var stack = new Stack<TryState>();
            stack.Push(new TryState(-1, -1, TryType.NONE, false));
            if (hasHandler)
                stack.Push(new TryState(6, 9, TryType.TRY, true));

            Assert.AreEqual(hasHandler ? BranchType.OK : BranchType.THROW, coverage.HandleThrow(0, 3, stack));
            Assert.AreEqual(hasHandler ? BranchType.OK : BranchType.UNCOVERED, coverage.coveredMap[6]);
            Assert.AreEqual(hasHandler ? BranchType.OK : BranchType.UNCOVERED, coverage.coveredMap[9]);
            Assert.AreEqual(hasHandler ? 2 : 1, stack.Count, "Analysis must not consume the caller's handler stack.");

            coverage = new InstructionCoverage(nef, manifest);
            Assert.AreEqual(BranchType.ABORT, coverage.HandleAbort(0, 3, stack));
            Assert.AreEqual(hasHandler ? BranchType.OK : BranchType.UNCOVERED, coverage.coveredMap[6]);
            Assert.AreEqual(hasHandler ? 2 : 1, stack.Count);
        }

        private static void AssertAbortRemainsTerminalInsideTry(OpCode abortOpCode)
        {
            var coverage = CreateTryCatchFinallyCoverage(abortOpCode);

            Assert.AreEqual(BranchType.ABORT, coverage.coveredMap[0]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[6]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[9]);
            Assert.AreEqual(BranchType.OK, coverage.coveredMap[11]);

            var (optimizedNef, optimizedManifest, _) = Reachability.RemoveUncoveredInstructions(
                CreateNefFile(CreateTryCatchFinallyScript(abortOpCode)), CreateManifest());
            var optimizedCoverage = new InstructionCoverage(optimizedNef, optimizedManifest);

            Assert.AreEqual(BranchType.ABORT, optimizedCoverage.coveredMap[0]);
        }

        private static InstructionCoverage CreateTryCatchFinallyCoverage(OpCode terminatingOpCode)
            => new(CreateNefFile(CreateTryCatchFinallyScript(terminatingOpCode)), CreateManifest());

        private static byte[] CreateTryCatchFinallyScript(OpCode terminatingOpCode)
        {
            byte[] script =
            [
                (byte)OpCode.TRY, 0x06, 0x09,
                (byte)terminatingOpCode,
                (byte)OpCode.ENDTRY, 0x07,
                (byte)OpCode.NOP,
                (byte)OpCode.ENDTRY, 0x04,
                (byte)OpCode.NOP,
                (byte)OpCode.ENDFINALLY,
                (byte)OpCode.RET
            ];

            return script;
        }

        private static NefFile CreateNefFile(byte[] script)
        {
            return new NefFile
            {
                Compiler = "test",
                Source = "test.cs",
                Tokens = Array.Empty<MethodToken>(),
                Script = script
            };
        }

        private static ContractManifest CreateManifest()
        {
            return new ContractManifest
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
                            ReturnType = ContractParameterType.Void,
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
}
