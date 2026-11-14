// Copyright (C) 2015-2026 The Neo Project.
//
// VerboseTracingTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class VerboseTracingTests
    {
        public class StorageContract : SmartContract
        {
            protected StorageContract(SmartContractInitialize initialize) : base(initialize) { }
        }

        [TestMethod]
        public void NoneIsSilentAndBasicTracesResultsLogsAndEvents()
        {
            TestEngine engine = new(true);
            var contract = DeployStorageContract(engine);
            using var output = new StringWriter();
            engine.TraceOutput = output;

            using var script = new ScriptBuilder();
            script.EmitDynamicCall(contract.Hash, "readWrite");
            engine.Execute(script.ToArray());
            Assert.AreEqual(string.Empty, output.ToString());

            engine.Verbose = VerboseLevel.Basic;
            engine.Execute(script.ToArray());
            var trace = output.ToString();

            StringAssert.Contains(trace, "EXECUTION START script=");
            StringAssert.Contains(trace, "EXECUTION END state=HALT gas=");
            StringAssert.Contains(trace, "LOG script=");
            StringAssert.Contains(trace, "message=hello");
            StringAssert.Contains(trace, "EVENT script=");
            StringAssert.Contains(trace, "name=traceEvent state=[]");
            Assert.IsFalse(trace.Contains("CALL depth="));
            Assert.IsFalse(trace.Contains("STORAGE "));
            Assert.IsFalse(trace.Contains("INSTRUCTION "));
        }

        [TestMethod]
        public void DetailedTracesContractCallsReturnsAndStorageOperations()
        {
            TestEngine engine = new(true);
            var contract = DeployStorageContract(engine);
            using var output = new StringWriter();
            engine.TraceOutput = output;
            engine.Verbose = VerboseLevel.Detailed;

            using var script = new ScriptBuilder();
            script.EmitDynamicCall(contract.Hash, "readWrite");
            var result = engine.Execute(script.ToArray());
            var trace = output.ToString();

            Assert.AreEqual("value", result.GetString());
            StringAssert.Contains(trace, "CALL depth=1 script=");
            StringAssert.Contains(trace, "CALL depth=2 script=");
            StringAssert.Contains(trace, "RETURN depth=1 script=");
            StringAssert.Contains(trace, "STORAGE WRITE script=");
            StringAssert.Contains(trace, "key=0x6B6579 value=0x76616C7565");
            StringAssert.Contains(trace, "STORAGE READ script=");
            StringAssert.Contains(trace, "STORAGE DELETE script=");
            Assert.IsFalse(trace.Contains("INSTRUCTION "));
        }

        [TestMethod]
        public void TraceIncludesInstructionGasAndEvaluationStack()
        {
            TestEngine engine = new(true);
            using var output = new StringWriter();
            engine.TraceOutput = output;
            engine.Verbose = VerboseLevel.Trace;

            using var script = new ScriptBuilder();
            script.EmitPush(1);
            script.EmitPush(2);
            script.Emit(OpCode.ADD);
            var result = engine.Execute(script.ToArray());
            var trace = output.ToString();

            Assert.AreEqual(3, result.GetInteger());
            StringAssert.Contains(trace, "opcode=PUSH1");
            StringAssert.Contains(trace, "opcode=ADD");
            StringAssert.Contains(trace, "gas=");
            StringAssert.Contains(trace, "stack=[2, 1]");
            StringAssert.Contains(trace, "CALL depth=1 script=");
            StringAssert.Contains(trace, "EXECUTION END state=HALT");
        }

        [TestMethod]
        public void BasicTracesFaultBeforeThrowing()
        {
            TestEngine engine = new(true);
            using var output = new StringWriter();
            engine.TraceOutput = output;
            engine.Verbose = VerboseLevel.Basic;

            using var script = new ScriptBuilder();
            script.EmitPush(1);
            script.EmitPush(0);
            script.Emit(OpCode.DIV);

            Assert.ThrowsExactly<TestException>(() => engine.Execute(script.ToArray()));
            StringAssert.Contains(output.ToString(), "EXECUTION END state=FAULT gas=");
        }

        private static StorageContract DeployStorageContract(TestEngine engine)
        {
            using var script = new ScriptBuilder();
            script.Emit(OpCode.INITSLOT, new byte[] { 1, 0 });
            script.EmitPush("hello");
            script.EmitSysCall(ApplicationEngine.System_Runtime_Log);
            script.EmitPush(0);
            script.Emit(OpCode.PACK);
            script.EmitPush("traceEvent");
            script.EmitSysCall(ApplicationEngine.System_Runtime_Notify);
            script.EmitPush("value");
            script.EmitPush("key");
            script.EmitSysCall(ApplicationEngine.System_Storage_GetContext);
            script.EmitSysCall(ApplicationEngine.System_Storage_Put);
            script.EmitPush("key");
            script.EmitSysCall(ApplicationEngine.System_Storage_GetContext);
            script.EmitSysCall(ApplicationEngine.System_Storage_Get);
            script.Emit(OpCode.STLOC0);
            script.EmitPush("key");
            script.EmitSysCall(ApplicationEngine.System_Storage_GetContext);
            script.EmitSysCall(ApplicationEngine.System_Storage_Delete);
            script.Emit(OpCode.LDLOC0);
            script.Emit(OpCode.RET);

            var nef = new NefFile
            {
                Compiler = "test",
                Source = string.Empty,
                Script = script.ToArray(),
                Tokens = []
            };
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            var manifest = new ContractManifest
            {
                Name = "StorageContract",
                Groups = [],
                SupportedStandards = [],
                Abi = new ContractAbi
                {
                    Methods =
                    [
                        new ContractMethodDescriptor
                        {
                            Name = "readWrite",
                            Offset = 0,
                            ReturnType = ContractParameterType.ByteArray,
                            Parameters = [],
                            Safe = false
                        }
                    ],
                    Events =
                    [
                        new ContractEventDescriptor
                        {
                            Name = "traceEvent",
                            Parameters = []
                        }
                    ]
                },
                Permissions = [ContractPermission.DefaultPermission],
                Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
                Extra = null
            };

            return engine.Deploy<StorageContract>(nef, manifest);
        }
    }
}
