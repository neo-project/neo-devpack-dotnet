using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.Optimizer;
using System.Collections.Generic;
using Neo.SmartContract;
using Neo.Json;
using Neo.SmartContract.Manifest;
using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class UnitTest_BasicBlock
    {
        private TestEngine testengine;

        public void Test_SingleContractBasicBlockStartEnd(string fileName)
        {
            testengine = new TestEngine();
            try
            {
                testengine.AddEntryScript(fileName);
            }
            catch (Exception e) { return; }
            (NefFile nef, ContractManifest manifest, JToken debugInfo) = (testengine.Nef, testengine.Manifest, testengine.DebugInfo);
            if (nef == null) { return; }
            ContractInBasicBlocks basicBlocks;
            try
            {
                basicBlocks = new(nef, manifest, debugInfo);
            }
            catch (Exception e) { return; }
            // TODO: support CALLA and do not return

            List<VM.Instruction> instructions = basicBlocks.GetScriptInstructions().ToList();
            (_, _, Dictionary<VM.Instruction, HashSet<VM.Instruction>> jumpTargets) = Neo.Optimizer.JumpTarget.FindAllJumpAndTrySourceToTargets(instructions);

            Dictionary<VM.Instruction, VM.Instruction> nextAddrTable = new();
            VM.Instruction prev = null;
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
                Assert.IsTrue(basicBlocks.basicBlocksByStartingInstruction.ContainsKey(target));
            // Each instruction is included in only 1 basic block
            HashSet<VM.Instruction> includedInstructions = new();
            foreach (BasicBlock basicBlock in basicBlocks.basicBlocks)
                foreach (VM.Instruction instruction in basicBlock.instructions)
                {
                    Assert.IsFalse(includedInstructions.Contains(instruction));
                    includedInstructions.Add(instruction);
                }
        }

        [TestMethod]
        public void Test_BasicBlockStartEnd()
        {
            string[] files = Directory.GetFiles(Utils.Extensions.TestContractRoot, "Contract*.cs");
            Parallel.ForEach(files, Test_SingleContractBasicBlockStartEnd);
            //foreach (string file in files)
            //    Test_SingleContractBasicBlockStartEnd(file);
        }
    }
}
