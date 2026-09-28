// Copyright (C) 2015-2026 The Neo Project.
//
// StrategyCoverageTests.cs file belongs to the neo project and is free
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
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class StrategyCoverageTests
    {
        [TestMethod]
        public void UseIsNull_RewritesEqualityWithNull()
        {
            var (optimizedNef, _, _) = Peephole.UseIsNull(
                CreateNefFile([(byte)OpCode.PUSHNULL, (byte)OpCode.EQUAL, (byte)OpCode.RET]),
                CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.ISNULL, OpCode.RET },
                GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void FoldNotInEqual_FoldsEqualityNegation()
        {
            var (optimizedNef, _, _) = Peephole.FoldNotInEqual(
                CreateNefFile([(byte)OpCode.EQUAL, (byte)OpCode.NOT, (byte)OpCode.RET]),
                CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.NOTEQUAL, OpCode.RET },
                GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void FoldNotInJmp_FoldsNegatedConditionalJump()
        {
            var (optimizedNef, _, _) = Peephole.FoldNotInJmp(
                CreateNefFile([(byte)OpCode.PUSH1, (byte)OpCode.NOT, (byte)OpCode.JMPIF, 0x02, (byte)OpCode.PUSH0, (byte)OpCode.RET]),
                CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.PUSH1, OpCode.JMPIFNOT, OpCode.PUSH0, OpCode.RET },
                GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void ReplaceJumpWithRet_RewritesJumpToReturn()
        {
            var (optimizedNef, _, _) = JumpCompresser.ReplaceJumpWithRet(
                CreateNefFile([(byte)OpCode.JMP, 0x02, (byte)OpCode.RET]),
                CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.RET, OpCode.RET },
                GetOpCodes(optimizedNef));
        }

        [TestMethod]
        public void RemoveMultiRet_CollapsesAdjacentReturns()
        {
            var (optimizedNef, _, _) = Reachability.RemoveMultiRet(
                CreateNefFile([(byte)OpCode.RET, (byte)OpCode.RET, (byte)OpCode.RET]),
                CreateManifest());

            CollectionAssert.AreEqual(
                new[] { OpCode.RET },
                GetOpCodes(optimizedNef));
        }

        private static OpCode[] GetOpCodes(NefFile nef)
            => new Script(nef.Script.ToArray()).EnumerateInstructions()
                .Select(static item => item.instruction.OpCode).ToArray();

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
}
