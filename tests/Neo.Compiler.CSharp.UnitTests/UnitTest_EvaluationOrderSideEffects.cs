// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_EvaluationOrderSideEffects.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_EvaluationOrderSideEffects
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            private static int _trace;

            private static int Record(int marker, int value)
            {
                _trace = _trace * 10 + marker;
                return value;
            }

            private static int AddThree(int first, int second, int third) => first + second + third;

            [DisplayName("arguments")]
            public static int Arguments()
            {
                _trace = 0;
                int result = AddThree(Record(1, 10), Record(2, 20), Record(3, 30));
                return _trace * 1000 + result;
            }

            [DisplayName("shortCircuit")]
            public static int ShortCircuit(bool first, bool second)
            {
                _trace = 0;
                bool result = first && Record(1, 1) > 0 && second && Record(2, 1) > 0;
                return _trace * 10 + (result ? 1 : 0);
            }

            [DisplayName("conditional")]
            public static int Conditional(bool chooseFirst)
            {
                _trace = 0;
                int result = chooseFirst ? Record(3, 7) : Record(4, 9);
                return _trace * 10 + result;
            }

            [DisplayName("coalesce")]
            public static int Coalesce(bool useValue)
            {
                _trace = 0;
                int? value = useValue ? Record(5, 8) : null;
                int result = value ?? Record(6, 9);
                return _trace * 10 + result;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CallsAreEvaluatedLeftToRight(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(123060), contract.Arguments(), optimization.ToString());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ShortCircuitOperatorsDoNotEvaluateSkippedOperands(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(0), contract.ShortCircuit(false, false));
        Assert.AreEqual(new BigInteger(10), contract.ShortCircuit(true, false));
        Assert.AreEqual(new BigInteger(121), contract.ShortCircuit(true, true));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ConditionalAndCoalescingEvaluateOnlyTheSelectedExpression(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(37), contract.Conditional(true));
        Assert.AreEqual(new BigInteger(49), contract.Conditional(false));
        Assert.AreEqual(new BigInteger(58), contract.Coalesce(true));
        Assert.AreEqual(new BigInteger(69), contract.Coalesce(false));
    }

    private static EvaluationOrderContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");
        return new TestEngine(true).Deploy<EvaluationOrderContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class EvaluationOrderContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("arguments")] public abstract BigInteger? Arguments();
        [DisplayName("shortCircuit")] public abstract BigInteger? ShortCircuit(bool first, bool second);
        [DisplayName("conditional")] public abstract BigInteger? Conditional(bool chooseFirst);
        [DisplayName("coalesce")] public abstract BigInteger? Coalesce(bool useValue);
    }
}
