// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_InitSlot.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_InitSlot
    {
        [TestMethod]
        public void EmitInitSlot_UsesActualParameterCountForGenericMethods()
        {
            var context = TestCleanup.TestInitialize(typeof(Contract_Lambda));
            Assert.IsNotNull(context, "Failed to compile Contract_Lambda for inspection.");

            static bool IsAnyGeneric(string id) =>
                id.Contains("Contract_Lambda.Any", StringComparison.Ordinal) &&
                id.Contains("System.Predicate", StringComparison.Ordinal);

            var initSlot = GetInitSlotInstruction(context!, IsAnyGeneric);

            Assert.AreEqual(OpCode.INITSLOT, initSlot.OpCode, "The first instruction must initialize stack slots.");
            var operand = initSlot.Operand.Span;
            Assert.IsTrue(operand.Length >= 2, "INITSLOT must contain local and argument counts.");
            Assert.AreEqual(2, operand[1], "Generic helpers must allocate exactly the declared parameter count.");
        }

        [TestMethod]
        public void EmitInitSlot_ReusesReleasedSystemCallTemps()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using System;
                using System.Numerics;

                namespace Neo.Compiler.CSharp.TestContracts;

                public class TempSlotContract : SmartContract.Framework.SmartContract
                {
                    private enum Flags
                    {
                        None = 0,
                        A = 1,
                        B = 2
                    }

                    public static int Probe(string value)
                    {
                        int total = value.LastIndexOf("a");
                        total += value.LastIndexOf("b");
                        total += value.Trim().Length;
                        total += value.TrimStart().Length;
                        total += value.TrimEnd().Length;
                        total += value.Remove(0, 1).Length;
                        total += value.Insert(0, "z").Length;
                        total += BigInteger.TryParse(value, out BigInteger parsed) ? 1 : 0;
                        Flags parsedFlag = Enum.Parse<Flags>(value);
                        Flags parsedFlagIgnoreCase = Enum.Parse<Flags>(value, true);
                        total += parsedFlag == Flags.A ? 1 : 0;
                        total += parsedFlagIgnoreCase == Flags.B ? 1 : 0;
                        total += Enum.TryParse<Flags>(value, out Flags tryParsedFlag) ? 1 : 0;
                        total += tryParsedFlag == Flags.A ? 1 : 0;
                        total += Enum.TryParse<Flags>(value, true, out Flags tryParsedFlagIgnoreCase) ? 1 : 0;
                        total += tryParsedFlagIgnoreCase == Flags.B ? 1 : 0;
                        total += Flags.A.HasFlag(Flags.A) ? 1 : 0;
                        return total;
                    }
                }
                """;

            var context = CompileSource(source);
            var initSlot = GetInitSlotInstruction(context, static id =>
                id.Contains("TempSlotContract.Probe", StringComparison.Ordinal));

            var operand = initSlot.Operand.Span;
            Assert.IsTrue(operand.Length >= 2, "INITSLOT must contain local and argument counts.");
            Assert.IsTrue(operand[0] <= 12, $"System-call temporary locals should be reusable; actual local count was {operand[0]}.");

            var unoptimizedContext = CompileSource(source, CompilationOptions.OptimizationType.None);
            var unoptimizedInitSlot = GetInitSlotInstruction(unoptimizedContext, static id =>
                id.Contains("TempSlotContract.Probe", StringComparison.Ordinal));

            var unoptimizedOperand = unoptimizedInitSlot.Operand.Span;
            Assert.IsTrue(unoptimizedOperand.Length >= 2, "INITSLOT must contain local and argument counts.");
            Assert.IsTrue(unoptimizedOperand[0] > operand[0], "Disabling basic optimization should preserve unreleased anonymous slots.");
        }

        [TestMethod]
        public void EmitInitSlot_ForStaticFieldInitializerTemps()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using System.ComponentModel;

                public class Contract : SmartContract
                {
                    private static int _counter;
                    private static int Value = Combine(second: Next(), first: Next());

                    [DisplayName("get")]
                    public static int Get()
                    {
                        return Value;
                    }

                    private static int Next()
                    {
                        _counter++;
                        return _counter;
                    }

                    private static int Combine(int first, int second)
                    {
                        return first * 10 + second;
                    }
                }
                """;

            var context = TestHelper.CompileSingleContract(source);
            var nef = context.CreateExecutable();
            var manifest = context.CreateManifest();
            var initialize = manifest.Abi.GetMethod("_initialize", 0);

            Assert.IsNotNull(initialize, "Static field initialization should emit an _initialize method.");

            var script = (Script)nef.Script;
            var initStaticSlot = script.GetInstruction(initialize.Offset);
            var initLocalSlot = script.GetInstruction(initialize.Offset + initStaticSlot.Size);

            Assert.AreEqual(OpCode.INITSSLOT, initStaticSlot.OpCode, "_initialize should initialize static slots first.");
            Assert.AreEqual(OpCode.INITSLOT, initLocalSlot.OpCode, "Static field initializer temporaries should initialize local slots.");

            var operand = initLocalSlot.Operand.Span;
            Assert.IsTrue(operand.Length >= 2, "INITSLOT must contain local and argument counts.");
            Assert.AreEqual(2, operand[0], "The out-of-order named argument initializer requires two temporary local slots.");
            Assert.AreEqual(0, operand[1], "_initialize should not allocate argument slots.");

            var engine = new TestEngine(true);
            var contract = engine.Deploy<StaticInitializerContract>(nef, manifest);
            Assert.AreEqual(new BigInteger(21), contract.Get());
        }

        [TestMethod]
        public void EmitInitSlot_InitializesSlotsDiscoveredFromVTableMethods()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using System.ComponentModel;

                public class Contract : SmartContract
                {
                    [DisplayName("read")]
                    public static int Read()
                    {
                        return new Outer().Read();
                    }
                }

                public class Outer
                {
                    public virtual int Read()
                    {
                        return new Inner().Read();
                    }
                }

                public static class NestedState
                {
                    public static readonly Base Value = new();
                }

                public class Base
                {
                    public virtual int Read()
                    {
                        return 42;
                    }
                }

                public class Inner
                {
                    public virtual int Read()
                    {
                        return NestedState.Value.Read();
                    }
                }
                """;

            var context = TestHelper.CompileSingleContract(source);
            var nef = context.CreateExecutable();
            var manifest = context.CreateManifest();

            var engine = new TestEngine(true);
            var contract = engine.Deploy<LateDiscoveredInitializerContract>(nef, manifest);
            Assert.AreEqual(new BigInteger(42), contract.Read());
        }

        private static CompilationContext CompileSource(
            string source,
            CompilationOptions.OptimizationType optimize = CompilationOptions.OptimizationType.Basic)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
            File.WriteAllText(tempPath, source);

            try
            {
                var engine = new CompilationEngine(new CompilationOptions
                {
                    Debug = CompilationOptions.DebugType.Extended,
                    Optimize = optimize
                });
                var context = engine.CompileSources(tempPath).Single();
                Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
                return context;
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }

        private static Neo.VM.Instruction GetInitSlotInstruction(CompilationContext context, Func<string, bool> matchesMethod)
        {
            var nef = context.CreateExecutable();
            var debugInfo = context.CreateDebugInformation();
            var methods = (JArray)debugInfo["methods"]!;

            JObject? methodEntry = methods
                .OfType<JObject>()
                .FirstOrDefault(m => matchesMethod(m["id"]!.GetString()));

            Assert.IsNotNull(methodEntry, "Unable to find target method in debug info.");

            var range = methodEntry["range"]!.GetString();
            var dashIndex = range.IndexOf('-', StringComparison.Ordinal);
            Assert.IsTrue(dashIndex > 0, "Method range should include a dash-delimited offset span.");

            var startOffset = int.Parse(range[..dashIndex], CultureInfo.InvariantCulture);
            var script = (Script)nef.Script;

            var started = false;
            foreach (var (address, instruction) in script.EnumerateInstructions())
            {
                if (!started)
                {
                    if (address != startOffset)
                        continue;
                    started = true;
                }

                if (instruction.OpCode == OpCode.INITSLOT)
                    return instruction;
            }

            Assert.Fail($"Unable to resolve instruction at offset {startOffset} for the selected method.");
            throw new InvalidOperationException();
        }

        public abstract class StaticInitializerContract(SmartContractInitialize initialize)
            : SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("get")]
            public abstract BigInteger? Get();
        }

        public abstract class LateDiscoveredInitializerContract(SmartContractInitialize initialize)
            : SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("read")]
            public abstract BigInteger? Read();
        }
    }
}
