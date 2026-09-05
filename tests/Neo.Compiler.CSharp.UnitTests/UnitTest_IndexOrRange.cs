// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_IndexOrRange.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_IndexOrRange : DebugAndTestBase<Contract_IndexOrRange>
    {
        [TestMethod]
        public void Test_Main()
        {
            var logs = new Queue<string>();
            Contract.OnRuntimeLog += (sender, log) => logs.Enqueue(log);
            var standardEvaluationOrder = GetStandardEvaluationOrder();

            Contract.TestMain();

            // Check logs
            Assert.AreEqual(22, logs.Count);
            foreach (var expected in standardEvaluationOrder)
                Assert.AreEqual(expected, logs.Dequeue());
            Assert.AreEqual("3", logs.Dequeue());
            Assert.AreEqual("10", logs.Dequeue());
            Assert.AreEqual("3", logs.Dequeue());
            Assert.AreEqual("8", logs.Dequeue());
            Assert.AreEqual("2", logs.Dequeue());
            Assert.AreEqual("2", logs.Dequeue());
            Assert.AreEqual("7", logs.Dequeue());
            Assert.AreEqual("3", logs.Dequeue());
            Assert.AreEqual("2", logs.Dequeue());
            Assert.AreEqual("1", logs.Dequeue());
            Assert.AreEqual("123456789", logs.Dequeue());
            Assert.AreEqual("123", logs.Dequeue());
            Assert.AreEqual("3456789", logs.Dequeue());
            Assert.AreEqual("45", logs.Dequeue());
            Assert.AreEqual("89", logs.Dequeue());
            Assert.AreEqual("123456", logs.Dequeue());
            Assert.AreEqual("45", logs.Dequeue());
            Assert.AreEqual("67", logs.Dequeue());
            Assert.AreEqual("1", logs.Dequeue());

            AssertGasConsumed(39100290);
        }

        private static List<string> GetStandardEvaluationOrder()
        {
            var evaluationOrder = new List<string>();

            byte[] GetReceiver()
            {
                evaluationOrder.Add("receiver");
                return new byte[] { 1, 2, 3, 4, 5 };
            }

            int GetStart()
            {
                evaluationOrder.Add("start");
                return 1;
            }

            int GetEnd()
            {
                evaluationOrder.Add("end");
                return 4;
            }

            _ = GetReceiver()[GetStart()..GetEnd()];
            return evaluationOrder;
        }

        [TestMethod]
        public void Test_FromEndRangeEvaluationOrder()
        {
            Assert.AreEqual(0, Contract.TestFromEndRangeEvaluationOrder());
            AssertLogs("receiver", "start", "end");
        }

        [TestMethod]
        public void Test_StringRangeEvaluationOrder()
        {
            Assert.AreEqual("234", Contract.TestStringRangeEvaluationOrder());
            AssertLogs("string receiver", "start", "end");
        }

        [TestMethod]
        public void Test_NullLeftFromEndRangeEvaluatesBothEndpoints()
        {
            Assert.ThrowsException<TestException>(Contract.TestNullLeftFromEndRangeEvaluationOrder);
            AssertLogs("start", "end");
        }

        [TestMethod]
        public void Test_NullRightFromEndRangeEvaluatesBothEndpoints()
        {
            Assert.ThrowsException<TestException>(Contract.TestNullRightFromEndRangeEvaluationOrder);
            AssertLogs("start", "end");
        }

        [TestMethod]
        public void Test_ConditionalNullRangeSkipsEndpoints()
        {
            Assert.IsTrue(Contract.TestConditionalNullRangeSkipsEndpoints());
            AssertNoLogs();
        }

        [TestMethod]
        public void Test_NegativeStartSkipsEndEvaluation()
        {
            Assert.ThrowsException<TestException>(Contract.TestNegativeStartSkipsEndEvaluation);
            AssertLogs("receiver", "negative");
        }

        [TestMethod]
        public void Test_NegativeFromEndStopsAfterEndEvaluation()
        {
            Assert.ThrowsException<TestException>(Contract.TestNegativeFromEndStopsAfterEndEvaluation);
            AssertLogs("receiver", "start", "negative");
        }

        [TestMethod]
        public void Test_RangeEndpointsAcrossOptimizationLevels()
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Services;
                using System;
                using System.ComponentModel;

                public class Contract : SmartContract
                {
                    [DisplayName("nestedRight")]
                    public static int NestedRight(bool flag)
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[1..(flag ? ^1 : ^2)].Length;
                    }

                    [DisplayName("nestedLeftNull")]
                    public static void NestedLeftNull(bool flag)
                    {
                        byte[]? bytes = null;
                        _ = bytes![(flag ? ^3 : ^2)..NextEndpoint()];
                    }

                    [DisplayName("nestedMixed")]
                    public static int NestedMixed(bool fromEnd)
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[1..(fromEnd ? ^1 : 3)].Length;
                    }

                    [DisplayName("nestedOutOfRange")]
                    public static void NestedOutOfRange(bool flag)
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        _ = bytes[(flag ? ^6 : ^7)..NextEndpoint()];
                    }

                    [DisplayName("switchEndpoint")]
                    public static int SwitchEndpoint(int mode)
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[(mode switch { 0 => ^3, _ => 1 })..4].Length;
                    }

                    [DisplayName("checkedEndpoint")]
                    public static int CheckedEndpoint()
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[checked(^2)..].Length;
                    }

                    [DisplayName("castEndpoint")]
                    public static int CastEndpoint()
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[1..(Index)(^1)].Length;
                    }

                    [DisplayName("suppressedEndpoint")]
                    public static int SuppressedEndpoint()
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[1..((^2)!)].Length;
                    }

                    [DisplayName("conditionalThrowEndpoint")]
                    public static int ConditionalThrowEndpoint(bool useEndpoint)
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[1..(useEndpoint ? ^2 : throw new Exception())].Length;
                    }

                    [DisplayName("switchThrowEndpoint")]
                    public static int SwitchThrowEndpoint(int mode)
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        return bytes[1..(mode switch { 0 => ^2, _ => throw new Exception() })].Length;
                    }

                    private static int NextEndpoint()
                    {
                        Runtime.Log("end");
                        return 4;
                    }
                }
                """;

            foreach (var optimization in new[]
            {
                CompilationOptions.OptimizationType.None,
                CompilationOptions.OptimizationType.Basic,
                CompilationOptions.OptimizationType.All
            })
            {
                var options = TestHelper.CreateDefaultOptions();
                options.Optimize = optimization;
                var context = TestHelper.CompileSingleContract(source, options);
                Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

                var engine = new TestEngine(true);
                var contract = engine.Deploy<RangePatternContract>(context.CreateExecutable(), context.CreateManifest());
                Assert.AreEqual(new BigInteger(3), contract.NestedRight(true), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.NestedRight(false), optimization.ToString());
                Assert.AreEqual(new BigInteger(3), contract.NestedMixed(true), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.NestedMixed(false), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.SwitchEndpoint(0), optimization.ToString());
                Assert.AreEqual(new BigInteger(3), contract.SwitchEndpoint(1), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.CheckedEndpoint(), optimization.ToString());
                Assert.AreEqual(new BigInteger(3), contract.CastEndpoint(), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.SuppressedEndpoint(), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.ConditionalThrowEndpoint(true), optimization.ToString());
                Assert.ThrowsException<TestException>(() => contract.ConditionalThrowEndpoint(false), optimization.ToString());
                Assert.AreEqual(new BigInteger(2), contract.SwitchThrowEndpoint(0), optimization.ToString());
                Assert.ThrowsException<TestException>(() => contract.SwitchThrowEndpoint(1), optimization.ToString());

                var logs = new Queue<string>();
                contract.OnRuntimeLog += (_, log) => logs.Enqueue(log);
                Assert.ThrowsException<TestException>(() => contract.NestedLeftNull(true), optimization.ToString());
                AssertSingleEndLog(logs, optimization);
                Assert.ThrowsException<TestException>(() => contract.NestedLeftNull(false), optimization.ToString());
                AssertSingleEndLog(logs, optimization);
                Assert.ThrowsException<TestException>(() => contract.NestedOutOfRange(true), optimization.ToString());
                AssertSingleEndLog(logs, optimization);
                Assert.ThrowsException<TestException>(() => contract.NestedOutOfRange(false), optimization.ToString());
                AssertSingleEndLog(logs, optimization);
            }
        }

        [TestMethod]
        public void Test_UnsupportedSystemIndexEndpointReportsDiagnostic()
        {
            var context = TestHelper.CompileSingleContract("""
                using Neo.SmartContract.Framework;
                using System;

                public class Contract : SmartContract
                {
                    public static int Test()
                    {
                        byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                        Index end;
                        return bytes[1..(end = ^2)].Length;
                    }
                }
                """);

            var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
            Assert.IsFalse(context.Success, diagnostics);
            Assert.IsTrue(context.Diagnostics.Any(p => p.Id == DiagnosticId.SyntaxNotSupported), diagnostics);
            Assert.IsFalse(context.Diagnostics.Any(p => p.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
        }

        [TestMethod]
        public void Test_UserDefinedIndexConversionsReportDiagnostic()
        {
            foreach (string endpoint in new[]
            {
                "default(Endpoint)",
                "(flag ? default(Endpoint) : default(Endpoint))"
            })
            {
                var context = TestHelper.CompileSingleContract($$"""
                    using Neo.SmartContract.Framework;
                    using System;

                    public readonly struct Endpoint
                    {
                        public static implicit operator Index(Endpoint value) => 1;
                    }

                    public class Contract : SmartContract
                    {
                        public static int Test(bool flag)
                        {
                            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
                            return bytes[1..{{endpoint}}].Length;
                        }
                    }
                    """);

                var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
                Assert.IsFalse(context.Success, diagnostics);
                Assert.IsTrue(context.Diagnostics.Any(p => p.Id == DiagnosticId.SyntaxNotSupported), diagnostics);
                Assert.IsFalse(context.Diagnostics.Any(p => p.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
            }
        }

        private static void AssertSingleEndLog(Queue<string> logs, CompilationOptions.OptimizationType optimization)
        {
            Assert.AreEqual(1, logs.Count, optimization.ToString());
            Assert.AreEqual("end", logs.Dequeue(), optimization.ToString());
            Assert.AreEqual(0, logs.Count, optimization.ToString());
        }

        public abstract class RangePatternContract(SmartContractInitialize initialize)
            : Neo.SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("nestedRight")]
            public abstract BigInteger? NestedRight(bool flag);

            [DisplayName("nestedLeftNull")]
            public abstract void NestedLeftNull(bool flag);

            [DisplayName("nestedMixed")]
            public abstract BigInteger? NestedMixed(bool fromEnd);

            [DisplayName("nestedOutOfRange")]
            public abstract void NestedOutOfRange(bool flag);

            [DisplayName("switchEndpoint")]
            public abstract BigInteger? SwitchEndpoint(BigInteger mode);

            [DisplayName("checkedEndpoint")]
            public abstract BigInteger? CheckedEndpoint();

            [DisplayName("castEndpoint")]
            public abstract BigInteger? CastEndpoint();

            [DisplayName("suppressedEndpoint")]
            public abstract BigInteger? SuppressedEndpoint();

            [DisplayName("conditionalThrowEndpoint")]
            public abstract BigInteger? ConditionalThrowEndpoint(bool useEndpoint);

            [DisplayName("switchThrowEndpoint")]
            public abstract BigInteger? SwitchThrowEndpoint(BigInteger mode);
        }
    }
}
