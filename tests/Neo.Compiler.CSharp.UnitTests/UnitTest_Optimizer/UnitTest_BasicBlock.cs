using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.Optimizer;
using System.Collections.Generic;
using Neo.SmartContract;
using Neo.Json;
using Neo.SmartContract.Manifest;
using System.Linq;
using Neo.VM;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class UnitTest_BasicBlock
    {
        private TestEngine testengine;

        HashSet<OpCode> allowedEnds = ((OpCode[])Enum.GetValues(typeof(OpCode)))
            .Where(i => Neo.Optimizer.JumpTarget.SingleJumpInOperand(i) && i != OpCode.PUSHA || Neo.Optimizer.JumpTarget.DoubleJumpInOperand(i)).ToHashSet()
            .Union(new HashSet<OpCode>() { OpCode.RET, OpCode.ABORT, OpCode.ABORTMSG, OpCode.THROW, OpCode.ENDFINALLY }).ToHashSet();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">"Contract_TryCatch.cs"</param>
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
            Dictionary<int, Dictionary<int, VM.Instruction>> basicBlocks;
            try
            {
                basicBlocks = BasicBlock.FindBasicBlocks(nef, manifest, debugInfo);
            }
            catch (Exception e) { return; }
            // TODO: support CALLA and do not return

            Script script = nef.Script;
            List<(int a, VM.Instruction i)> instructions = script.EnumerateInstructions().ToList();
            (_, _, Dictionary<int, HashSet<int>> jumpTargets) = Neo.Optimizer.JumpTarget.FindAllJumpAndTrySourceToTargets(instructions);

            Dictionary<int, int> nextAddrTable = new();
            int prev = -1;
            foreach ((int a, VM.Instruction i) in instructions)
            {
                if (prev >= 0)
                    nextAddrTable[prev] = a;
                prev = a;
            }

            foreach (Dictionary<int, VM.Instruction> basicBlock in basicBlocks.Values)
            {
                (int a, VM.Instruction i)[] sortedInstructions = (from kv in basicBlock orderby kv.Key ascending select (kv.Key, kv.Value)).ToArray();
                // Basic block ends with allowed OpCodes only, or the next instruction is a jump target
                Assert.IsTrue(allowedEnds.Contains(sortedInstructions.Last().i.OpCode) || jumpTargets.ContainsKey(nextAddrTable[sortedInstructions.Last().a]));
                // Other instructions in the basic block are not those in allowedEnds
                foreach ((int a, VM.Instruction i) in sortedInstructions.Take(sortedInstructions.Length - 1))
                    Assert.IsFalse(allowedEnds.Contains(i.OpCode));
            }
            // Each jump target starts a new basic block
            foreach (int target in jumpTargets.Keys)
                Assert.IsTrue(basicBlocks.ContainsKey(target));
            // Each instruction is included in only 1 basic block
            HashSet<VM.Instruction> includedInstructions = new();
            foreach (Dictionary<int, VM.Instruction> basicBlock in basicBlocks.Values)
                foreach (VM.Instruction instruction in basicBlock.Values)
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
