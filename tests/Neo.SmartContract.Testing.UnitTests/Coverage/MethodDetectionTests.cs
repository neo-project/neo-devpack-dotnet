// Copyright (C) 2015-2026 The Neo Project.
//
// MethodDetectionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.SmartContract.Testing.UnitTests.Coverage
{
    [TestClass]
    public class MethodDetectionTests
    {
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void NextMethodDetectsLocalCallTargets(bool longCall)
        {
            using var builder = new ScriptBuilder();
            int target = longCall ? 6 : 3;
            builder.Emit(longCall ? OpCode.CALL_L : OpCode.CALL,
                longCall ? BitConverter.GetBytes(target) : [(byte)target]);
            builder.Emit(OpCode.RET);
            builder.Emit(OpCode.PUSH7);
            builder.Emit(OpCode.RET);

            var coverage = CreateCoverage(builder.ToArray());

            CollectionAssert.AreEqual(new[] { 0, target }, coverage.Methods.Select(m => m.Offset).ToArray());
            CollectionAssert.AreEqual(new[] { 0, target - 1 }, coverage.Methods[0].Lines.Select(l => l.Offset).ToArray());
            CollectionAssert.AreEqual(new[] { target, target + 1 }, coverage.Methods[1].Lines.Select(l => l.Offset).ToArray());
        }

        [TestMethod]
        public void NextMethodDetectsBackwardShortCallTarget()
        {
            byte[] script = [(byte)OpCode.PUSH7, (byte)OpCode.RET, (byte)OpCode.CALL, 0xfe, (byte)OpCode.RET];

            var coverage = CreateCoverage(script, entry: 2);

            CollectionAssert.AreEqual(new[] { 0, 2 }, coverage.Methods.Select(m => m.Offset).ToArray());
            Assert.AreEqual("_private0", coverage.Methods[0].Method.Name);
            CollectionAssert.AreEqual(new[] { 0, 1 }, coverage.Methods[0].Lines.Select(l => l.Offset).ToArray());
            CollectionAssert.AreEqual(new[] { 2, 4 }, coverage.Methods[1].Lines.Select(l => l.Offset).ToArray());
        }

        [DataTestMethod]
        [DataRow((byte)0)]
        [DataRow((byte)1)]
        public void NextMethodDoesNotTreatMethodTokensAsLocalCalls(byte tokenIndex)
        {
            byte[] script = [(byte)OpCode.CALLT, tokenIndex, 0, (byte)OpCode.RET];
            var coverage = CreateCoverage(script);

            Assert.AreEqual(1, coverage.Methods.Length);
            Assert.AreEqual("value", coverage.Methods[0].Method.Name);
            CollectionAssert.AreEqual(new[] { 0, 3 }, coverage.Methods[0].Lines.Select(l => l.Offset).ToArray());
        }

        [TestMethod]
        public void TokenCallsDoNotSplitCoverageBeforeLocalMethods()
        {
            byte[] script =
            [
                (byte)OpCode.CALL, 6,
                (byte)OpCode.CALLT, 0, 0,
                (byte)OpCode.RET,
                (byte)OpCode.RET
            ];
            var coverage = CreateCoverage(script);

            CollectionAssert.AreEqual(new[] { 0, 6 }, coverage.Methods.Select(m => m.Offset).ToArray());
            CollectionAssert.AreEqual(new[] { 0, 2, 5 }, coverage.Methods[0].Lines.Select(l => l.Offset).ToArray());
            CollectionAssert.AreEqual(new[] { 6 }, coverage.Methods[1].Lines.Select(l => l.Offset).ToArray());
        }

        private static CoveredContract CreateCoverage(byte[] script, int entry = 0)
        {
            var state = new ContractState
            {
                Hash = UInt160.Zero,
                Nef = new NefFile
                {
                    Compiler = "test",
                    Source = string.Empty,
                    Script = script,
                    Tokens = Enumerable.Range(0, 2).Select(_ => new MethodToken
                    {
                        Hash = Neo.SmartContract.Native.NativeContract.NEO.Hash,
                        Method = "totalSupply",
                        ParametersCount = 0,
                        HasReturnValue = true,
                        CallFlags = CallFlags.ReadOnly
                    }).ToArray()
                },
                Manifest = new ContractManifest
                {
                    Name = "CoverageContract",
                    Groups = [],
                    SupportedStandards = [],
                    Permissions = [ContractPermission.DefaultPermission],
                    Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                    Abi = new ContractAbi
                    {
                        Methods = [new ContractMethodDescriptor
                        {
                            Name = "value",
                            Offset = entry,
                            Parameters = [],
                            ReturnType = ContractParameterType.Integer
                        }],
                        Events = []
                    }
                }
            };
            return new CoveredContract(MethodDetectionMechanism.NextMethod, state.Hash, state);
        }
    }
}
