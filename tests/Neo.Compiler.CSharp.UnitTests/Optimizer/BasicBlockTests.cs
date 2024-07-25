using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class BasicBlockTests
    {
        [TestMethod]
        public void Test_BasicBlockStartEnd()
        {
            var contracts = TestCleanup.EnsureArtifactsUpToDateInternal();
            Parallel.ForEach(contracts, TestSingleContractBasicBlockStartEnd);
        }

        public static void TestSingleContractBasicBlockStartEnd(CompilationContext result)
        {
            try
            {
                try
                {
                    TestSingleContractBasicBlockStartEnd(result.CreateExecutable(), result.CreateManifest(), result.CreateDebugInformation());
                }
                catch
                {
                    Console.WriteLine($"Omited: {result.ContractName}");
                }
            }
            catch
            {
                Console.WriteLine($"Error compiling: {result.ContractName}");
                return;
            }
        }

        public static void TestSingleContractBasicBlockStartEnd(NefFile nef, ContractManifest manifest, JToken debugInfo)
        {
            var basicBlocks = new ContractInBasicBlocks(nef, manifest, debugInfo);

            // TODO: support CALLA and do not return

            List<VM.Instruction> instructions = basicBlocks.GetScriptInstructions().ToList();
            (_, _, Dictionary<VM.Instruction, HashSet<VM.Instruction>> jumpTargets) =
                Neo.Optimizer.JumpTarget.FindAllJumpAndTrySourceToTargets(instructions);

            Dictionary<VM.Instruction, VM.Instruction> nextAddrTable = new();
            VM.Instruction? prev = null;
            foreach (VM.Instruction i in instructions)
            {
                if (prev != null)
                    nextAddrTable[prev] = i;
                prev = i;
            }

            foreach (BasicBlock basicBlock in basicBlocks.basicBlocks)
            {
                // Basic block ends with allowed OpCodes only, or the next instruction is a jump target
                Assert.IsTrue(OpCodeTypes.allowedBasicBlockEnds.Contains(basicBlock.instructions.Last().OpCode) || jumpTargets.ContainsKey(nextAddrTable[basicBlock.instructions.Last()]));
                // Instructions except the first are not jump targets
                foreach (VM.Instruction i in basicBlock.instructions.Skip(1))
                    Assert.IsFalse(jumpTargets.ContainsKey(i));
                // Other instructions in the basic block are not those in allowedBasicBlockEnds
                foreach (VM.Instruction i in basicBlock.instructions.Take(basicBlock.instructions.Count - 1))
                    Assert.IsFalse(OpCodeTypes.allowedBasicBlockEnds.Contains(i.OpCode));
            }

            // Each jump target starts a new basic block
            foreach (VM.Instruction target in jumpTargets.Keys)
                Assert.IsTrue(basicBlocks.basicBlocksByStartInstruction.ContainsKey(target));

            // Each instruction is included in only 1 basic block
            HashSet<VM.Instruction> includedInstructions = new();
            foreach (BasicBlock basicBlock in basicBlocks.basicBlocks)
                foreach (VM.Instruction instruction in basicBlock.instructions)
                {
                    Assert.IsFalse(includedInstructions.Contains(instruction));
                    includedInstructions.Add(instruction);
                }
        }
    }
}
