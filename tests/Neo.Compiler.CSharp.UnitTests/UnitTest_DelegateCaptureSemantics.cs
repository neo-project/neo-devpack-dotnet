// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DelegateCaptureSemantics.cs file belongs to the neo project and is free
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
public class UnitTest_DelegateCaptureSemantics
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("captureMutation")]
            public static int CaptureMutation(int seed)
            {
                int value = seed;
                Func<int> read = () => value;
                value += 5;
                return read();
            }

            [DisplayName("captureBranch")]
            public static int CaptureBranch(bool change)
            {
                int value = 1;
                Func<int> read = () => value;
                if (change)
                    value = 4;
                return read();
            }

            [DisplayName("loopDelegates")]
            public static int LoopDelegates()
            {
                int total = 0;
                for (int i = 1; i <= 3; i++)
                {
                    Func<int> current = () => i;
                    total += current();
                }
                return total;
            }

            [DisplayName("argumentDelegates")]
            public static int ArgumentDelegates()
            {
                int trace = 0;
                Func<int> first = () => { trace = trace * 10 + 1; return 4; };
                Func<int> second = () => { trace = trace * 10 + 2; return 5; };
                Func<int, int, int> sum = (left, right) => trace * 100 + left + right;
                return sum(first(), second());
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CapturedValuesObserveLaterMutations(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(12), contract.CaptureMutation(7));
        Assert.AreEqual(new BigInteger(1), contract.CaptureBranch(false));
        Assert.AreEqual(new BigInteger(4), contract.CaptureBranch(true));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void DelegatesCreatedInsideLoopsReadTheCurrentIteration(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(6), contract.LoopDelegates());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void DelegateArgumentsAreEvaluatedBeforeTheCall(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        Assert.AreEqual(new BigInteger(1209), contract.ArgumentDelegates());
    }

    private static DelegateCaptureContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");
        return new TestEngine(true).Deploy<DelegateCaptureContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class DelegateCaptureContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("captureMutation")] public abstract BigInteger? CaptureMutation(BigInteger seed);
        [DisplayName("captureBranch")] public abstract BigInteger? CaptureBranch(bool change);
        [DisplayName("loopDelegates")] public abstract BigInteger? LoopDelegates();
        [DisplayName("argumentDelegates")] public abstract BigInteger? ArgumentDelegates();
    }
}
