// Copyright (C) 2015-2025 The Neo Project.
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
using System.Globalization;
using System.Linq;

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
    }
}
