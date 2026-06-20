// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Inline.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.VM;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inline : DebugAndTestBase<Contract_Inline>
    {
        [TestMethod]
        public void Test_Inline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("inline"));
            AssertGasConsumed(1048650);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_with_one_parameters"));
            AssertGasConsumed(1050090);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("inline_with_multi_parameters"));
            AssertGasConsumed(1052070);
        }

        [TestMethod]
        public void Test_NoInline()
        {
            Assert.AreEqual(BigInteger.One, Contract.TestInline("not_inline"));
            AssertGasConsumed(1067970);
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("not_inline_with_one_parameters"));
            AssertGasConsumed(1071270);
            Assert.AreEqual(new BigInteger(5), Contract.TestInline("not_inline_with_multi_parameters"));
            AssertGasConsumed(1073190);
        }

        [TestMethod]
        public void Test_NoInlineOptionDisablesAggressiveInlining()
        {
            const string source = """
                using System.Runtime.CompilerServices;
                using Neo.SmartContract.Framework;

                public class Contract : SmartContract
                {
                    public static int Main(int value)
                    {
                        return AddOne(value);
                    }

                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    private static int AddOne(int value)
                    {
                        return value + 1;
                    }
                }
                """;

            var inlineContext = TestHelper.CompileSingleContract(source);
            var noInlineOptions = TestHelper.CreateDefaultOptions();
            noInlineOptions.NoInline = true;
            var noInlineContext = TestHelper.CompileSingleContract(source, noInlineOptions);

            Assert.IsFalse(MethodContainsCall(inlineContext, "Contract.Main(int)"));
            Assert.IsTrue(MethodContainsCall(noInlineContext, "Contract.Main(int)"));
        }

        [TestMethod]
        public void Test_InlineCombinedMethodImplOptions()
        {
            const string source = """
                using System.Runtime.CompilerServices;
                using Neo.SmartContract.Framework;

                public class Contract : SmartContract
                {
                    public static int Main(int value)
                    {
                        return AddOne(value);
                    }

                    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                    private static int AddOne(int value)
                    {
                        return value + 1;
                    }
                }
                """;

            var context = TestHelper.CompileSingleContract(source);
            Assert.IsFalse(MethodContainsCall(context, "Contract.Main(int)"));
        }

        [TestMethod]
        public void Test_InlineExtensionMethodReceiver()
        {
            const string source = """
                using System.Runtime.CompilerServices;
                using Neo.SmartContract.Framework;

                public class Contract : SmartContract
                {
                    public static int Main(int value)
                    {
                        return value.Add(2);
                    }
                }

                public static class IntExtensions
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    public static int Add(this int value, int amount)
                    {
                        return value + amount;
                    }
                }
                """;

            var context = TestHelper.CompileSingleContract(source);
            Assert.IsFalse(MethodContainsCall(context, "Contract.Main(int)"));

            var engine = new TestEngine(true);
            var contract = engine.Deploy<InlineMainContract>(context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(9), contract.Main(7));
        }

        [TestMethod]
        public void Test_NestedInline()
        {
            Assert.AreEqual(new BigInteger(3), Contract.TestInline("inline_nested"));
            AssertGasConsumed(1071930);
        }

        [TestMethod]
        public void Test_InlineCallerParameterScope()
        {
            Assert.AreEqual(new BigInteger(14), Contract.InlineThenUseCallerParameter(7));
            Assert.AreEqual(new BigInteger(14), Contract.InlineDuplicateParameter(7));
        }

        [TestMethod]
        public void Test_ArrowMethod()
        {
            Assert.AreEqual(new BigInteger(3), Contract.ArrowMethod());
        }

        [TestMethod]
        public void Test_ArrowMethodNoReturn()
        {
            Contract.ArrowMethodNoRerurn();
        }

        private static bool MethodContainsCall(CompilationContext context, string methodId)
        {
            var nef = context.CreateExecutable();
            var (start, end) = GetMethodRange(context.CreateDebugInformation(), methodId);

            return ((Script)nef.Script)
                .EnumerateInstructions()
                .Where(instruction => instruction.address >= start && instruction.address <= end)
                .Any(instruction => instruction.instruction.OpCode is OpCode.CALL or OpCode.CALL_L);
        }

        private static (int start, int end) GetMethodRange(JObject debugInfo, string methodId)
        {
            var methods = (JArray)debugInfo["methods"]!;
            var method = methods
                .OfType<JObject>()
                .FirstOrDefault(m => string.Equals(m["id"]?.GetString(), methodId, StringComparison.Ordinal));

            Assert.IsNotNull(method, $"Unable to find method '{methodId}' in debug info.");

            var range = method["range"]!.GetString();
            var dashIndex = range.IndexOf('-', StringComparison.Ordinal);
            Assert.IsTrue(dashIndex > 0, "Method range should include a dash-delimited offset span.");

            return (int.Parse(range[..dashIndex]), int.Parse(range[(dashIndex + 1)..]));
        }

        public abstract class InlineMainContract(SmartContractInitialize initialize)
            : Neo.SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("main")]
            public abstract BigInteger? Main(BigInteger? value);
        }
    }
}
