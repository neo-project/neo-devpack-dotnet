// Copyright (C) 2015-2026 The Neo Project.
//
// HasCallATests.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class HasCallATests
    {
        [TestMethod]
        public void Test_HasCallA()
        {
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Lambda.Nef));
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Linq.Nef));
            Assert.IsTrue(EntryPoint.HasCallA(Contract_Delegate.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_Polymorphism.Nef));
            Assert.IsFalse(EntryPoint.HasCallA(Contract_TryCatch.Nef));
        }

        [TestMethod]
        public void Test_EntryPointsByPushaIgnoresInvalidTargets()
        {
            var selfTarget = CreateNefFile([(byte)OpCode.PUSHA, 0, 0, 0, 0, (byte)OpCode.RET]);
            var negativeTarget = CreateNefFile([(byte)OpCode.PUSHA, 0xff, 0xff, 0xff, 0xff, (byte)OpCode.RET]);

            Assert.AreEqual(0, EntryPoint.EntryPointsByPusha(selfTarget).Count);
            Assert.AreEqual(0, EntryPoint.EntryPointsByPusha(negativeTarget).Count);
        }

        [TestMethod]
        public void Test_AllEntryPointsPrefersManifestMethodType()
        {
            var nef = CreateNefFile([(byte)OpCode.PUSHA, 5, 0, 0, 0, (byte)OpCode.RET]);
            var entryPoints = EntryPoint.AllEntryPoints(nef, CreateManifest(methodOffset: 5));

            Assert.AreEqual(1, entryPoints.Count);
            Assert.AreEqual(EntryType.PublicMethod, entryPoints[5]);
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

        private static ContractManifest CreateManifest(int methodOffset)
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
                            Offset = methodOffset,
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
