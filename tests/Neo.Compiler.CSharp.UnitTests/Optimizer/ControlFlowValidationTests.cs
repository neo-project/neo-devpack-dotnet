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
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using OptimizerJumpTarget = Neo.Optimizer.JumpTarget;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class ControlFlowValidationTests
    {
        [TestMethod]
        public void FindAllJumpAndTrySourceToTargets_ThrowsBadScriptForInvalidJumpTarget()
        {
            var script = new Script(new byte[] { (byte)OpCode.JMP, 0x03, (byte)OpCode.RET });

            var exception = Assert.ThrowsException<BadScriptException>(() =>
                OptimizerJumpTarget.FindAllJumpAndTrySourceToTargets(script));

            StringAssert.Contains(exception.Message, "JMP");
            StringAssert.Contains(exception.Message, "3");
        }

        [TestMethod]
        public void FindAllJumpAndTrySourceToTargets_ThrowsBadScriptForInvalidTryTarget()
        {
            var script = new Script(new byte[] { (byte)OpCode.TRY, 0x02, 0x00, (byte)OpCode.RET });

            var exception = Assert.ThrowsException<BadScriptException>(() =>
                OptimizerJumpTarget.FindAllJumpAndTrySourceToTargets(script));

            StringAssert.Contains(exception.Message, "TRY");
            StringAssert.Contains(exception.Message, "2");
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

            var exception = Assert.ThrowsException<BadScriptException>(() =>
                new InstructionCoverage(CreateNefFile(script.ToArray()), CreateManifest()));

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
