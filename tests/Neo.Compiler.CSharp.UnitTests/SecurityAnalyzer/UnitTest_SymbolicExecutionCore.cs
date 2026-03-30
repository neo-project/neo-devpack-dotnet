// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SymbolicExecutionCore.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Compiler.SecurityAnalyzer.SymbolicExecution;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class UnitTest_SymbolicExecutionCore
    {
        [TestMethod]
        public void Test_Conditional_Guarded_Update_Is_Not_Detected()
        {
            const string source = """
                using Neo;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;
                using Neo.SmartContract.Framework.Services;

                public class Contract : SmartContract
                {
                    public static void Main(UInt160 owner, ByteString nefFile, string manifest)
                    {
                        if (Runtime.CheckWitness(owner))
                        {
                            ContractManagement.Update(nefFile, manifest, null);
                        }
                    }
                }
                """;

            var (context, warnings) = AnalyzeSource(source);

            OpCode[] opcodes = ((Script)context.CreateExecutable().Script)
                .EnumerateInstructions()
                .Select(tuple => tuple.instruction.OpCode)
                .ToArray();

            Assert.IsTrue(opcodes.Any(opcode =>
                opcode is OpCode.JMPIF or OpCode.JMPIFNOT or OpCode.JMPIF_L or OpCode.JMPIFNOT_L),
                "Expected a conditional branch to exercise symbolic jump handling.");
            Assert.IsFalse(warnings.HasUnguardedUpdate);
            Assert.IsFalse(warnings.AnalysisIncomplete);
        }

        [TestMethod]
        public void Test_Helper_Call_Unguarded_Destroy_Is_Detected()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;

                public class Contract : SmartContract
                {
                    public static void Main()
                    {
                        DestroyCore();
                    }

                    private static void DestroyCore()
                    {
                        ContractManagement.Destroy();
                    }
                }
                """;

            var (context, warnings) = AnalyzeSource(source);

            OpCode[] opcodes = ((Script)context.CreateExecutable().Script)
                .EnumerateInstructions()
                .Select(tuple => tuple.instruction.OpCode)
                .ToArray();

            Assert.IsTrue(opcodes.Any(opcode => opcode is OpCode.CALL or OpCode.CALL_L),
                "Expected a helper method call to exercise symbolic call/return tracking.");
            Assert.IsTrue(warnings.HasUnguardedDestroy);
        }

        [TestMethod]
        public void Test_AssertMessage_Guarded_Destroy_Is_Not_Detected()
        {
            const string source = """
                using Neo;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;
                using Neo.SmartContract.Framework.Services;

                public class Contract : SmartContract
                {
                    public static void Main(UInt160 owner)
                    {
                        ExecutionEngine.Assert(Runtime.CheckWitness(owner), "owner");
                        ContractManagement.Destroy();
                    }
                }
                """;

            var (context, warnings) = AnalyzeSource(source);

            OpCode[] opcodes = ((Script)context.CreateExecutable().Script)
                .EnumerateInstructions()
                .Select(tuple => tuple.instruction.OpCode)
                .ToArray();

            Assert.IsTrue(opcodes.Any(opcode => opcode is OpCode.JMPIF or OpCode.JMPIF_L));
            Assert.IsTrue(opcodes.Contains(OpCode.ABORTMSG));
            Assert.IsFalse(warnings.HasUnguardedDestroy);
        }

        [TestMethod]
        public void Test_ContractCall_Update_And_Destroy_Are_Detected()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;
                using Neo.SmartContract.Framework.Services;

                public class SymbolicContract : SmartContract
                {
                    public static void UpdateViaCall(ByteString nefFile, string manifest)
                    {
                        Neo.SmartContract.Framework.Services.Contract.Call(
                            ContractManagement.Hash,
                            "update",
                            CallFlags.All,
                            nefFile,
                            manifest,
                            null);
                    }

                    public static void DestroyViaCall()
                    {
                        Neo.SmartContract.Framework.Services.Contract.Call(
                            ContractManagement.Hash,
                            "destroy",
                            CallFlags.All);
                    }
                }
                """;

            var (_, warnings) = AnalyzeSource(source);

            Assert.IsTrue(warnings.HasUnguardedUpdate);
            Assert.IsTrue(warnings.HasUnguardedDestroy);
        }

        [TestMethod]
        public void Test_SymbolicValue_Conversions()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("update");
            SymbolicValue textValue = SymbolicValue.FromByteString(utf8);

            Assert.IsTrue(textValue.TryGetString(out string text));
            Assert.AreEqual("update", text);
            Assert.IsFalse(SymbolicValue.FromBoolean(true).TryGetString(out _));

            byte[] hashBytes = Enumerable.Range(1, UInt160.Length).Select(i => (byte)i).ToArray();
            SymbolicValue hashFromBytes = SymbolicValue.FromByteString(hashBytes);
            Assert.IsTrue(hashFromBytes.TryGetUInt160(out UInt160 byteStringHash));
            CollectionAssert.AreEqual(hashBytes, byteStringHash.GetSpan().ToArray());

            SymbolicValue directHash = SymbolicValue.FromUInt160(UInt160.Zero);
            Assert.IsTrue(directHash.TryGetUInt160(out UInt160 zeroHash));
            Assert.AreEqual(UInt160.Zero, zeroHash);

            SymbolicValue witnessArgument = SymbolicValue.FromInteger(BigInteger.One);
            SymbolicValue witnessCheck = SymbolicValue.WitnessCheck(witnessArgument);
            Assert.AreEqual(SymbolicValueKind.WitnessCheck, witnessCheck.Kind);
            Assert.AreSame(witnessArgument, witnessCheck.WitnessArgument);

            Assert.IsFalse(SymbolicValue.Unknown.TryGetUInt160(out _));
        }

        [TestMethod]
        public void Test_SymbolicState_Clone_Is_Independent()
        {
            SymbolicState state = new("main", 5);

            Assert.AreEqual(SymbolicValueKind.Unknown, state.Peek().Kind);
            Assert.AreEqual(SymbolicValueKind.Unknown, state.Pop().Kind);

            state.Push(SymbolicValue.FromBoolean(true));
            state.PushReturn(9);
            state.HasWitnessGuard = true;
            state.BranchDepth = 2;

            SymbolicState clone = state.Clone();
            clone.Push(SymbolicValue.FromInteger(3));
            clone.PushReturn(10);
            clone.InstructionIndex = 7;
            clone.HasWitnessGuard = false;
            clone.BranchDepth = 4;

            Assert.AreEqual(1, state.Stack.Count);
            Assert.AreEqual(1, state.CallStack.Count);
            Assert.AreEqual(5, state.InstructionIndex);
            Assert.IsTrue(state.HasWitnessGuard);
            Assert.AreEqual(2, state.BranchDepth);

            Assert.IsTrue(state.TryPopReturn(out int returnIndex));
            Assert.AreEqual(9, returnIndex);
            Assert.IsFalse(state.TryPopReturn(out _));
        }

        [TestMethod]
        public void Test_WarningInfo_Formats_Destroy_And_Incomplete_Warnings()
        {
            SymbolicFindings findings = new();
            findings.RecordUnguardedUpdate("destroyNow", 42, isDestroy: true);
            findings.MarkIncomplete();

            var warnings = new SymbolicExecutionAnalyzer.SymbolicExecutionWarnings(findings, new JObject());
            string output = warnings.GetWarningInfo();

            Assert.IsTrue(output.Contains("unguarded contract destroy", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(output.Contains("At instruction address: 42", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(output.Contains("analysis was incomplete", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        public void Test_WarningInfo_Without_Findings_Returns_Empty()
        {
            var warnings = new SymbolicExecutionAnalyzer.SymbolicExecutionWarnings(new SymbolicFindings(), null);
            Assert.AreEqual(string.Empty, warnings.GetWarningInfo());
        }

        private static (CompilationContext Context, SymbolicExecutionAnalyzer.SymbolicExecutionWarnings Warnings) AnalyzeSource(string sourceCode)
        {
            CompilationContext context = CompileSingleContract(sourceCode);
            var warnings = SymbolicExecutionAnalyzer.Analyze(
                context.CreateExecutable(),
                context.CreateManifest(),
                debugInfo: null);
            return (context, warnings);
        }

        private static CompilationContext CompileSingleContract(string sourceCode)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
            File.WriteAllText(tempFile, sourceCode);

            try
            {
                var options = new CompilationOptions
                {
                    Optimize = CompilationOptions.OptimizationType.None,
                    Nullable = NullableContextOptions.Enable,
                    SkipRestoreIfAssetsPresent = true
                };

                var engine = new CompilationEngine(options);
                string repoRoot = Syntax.SyntaxProbeLoader.GetRepositoryRoot();
                string frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

                var contexts = engine.CompileSources(new CompilationSourceReferences
                {
                    Projects = new[] { frameworkProject }
                }, tempFile);

                Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
                Assert.IsTrue(contexts[0].Success, string.Join(Environment.NewLine, contexts[0].Diagnostics.Select(static d => d.ToString())));
                return contexts[0];
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
