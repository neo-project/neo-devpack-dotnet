// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT software license; see LICENSE in the repository root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DelegateByRef
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        public delegate bool TryValue(byte[] input, out int result);
        public delegate void DoubleValue(int input, out int result);
        public delegate void ChangeValue(ref int value);

        public class Contract : SmartContract
        {
            private static bool TryGet(byte[] input, out int result)
            {
                result = 7;
                return true;
            }

            private static bool TryGetOther(byte[] input, out int result)
            {
                result = 9;
                return true;
            }

            private static void Double(int input, out int result) => result = input * 2;
            private static void Change(ref int value) => value += 3;

            public static int InvokeOut()
            {
                TryValue d = TryGet;
                int result;
                bool ok = d.Invoke(new byte[] { 1 }, out result);
                return ok ? result : -1;
            }

            public static int DirectOut()
            {
                TryValue d = TryGetOther;
                int result;
                bool ok = d(new byte[] { 1 }, out result);
                return ok ? result : -1;
            }

            public static int VoidOut()
            {
                DoubleValue d = Double;
                d.Invoke(5, out int result);
                return result;
            }

            public static int BareStatement()
            {
                TryValue d = TryGet;
                d(new byte[] { 1 }, out var result);
                return result;
            }

            public static int InvokeRef()
            {
                ChangeValue d = Change;
                int result = 4;
                d.Invoke(ref result);
                return result;
            }

            public static int LambdaOut()
            {
                TryValue d = (byte[] input, out int result) =>
                {
                    result = 11;
                    return true;
                };
                d(new byte[] { 1 }, out int result);
                return result;
            }

            public static int NewDelegateOut()
            {
                var d = new TryValue(TryGet);
                d(new byte[] { 1 }, out int result);
                return result;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void DelegateByRefWritesReachCaller(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var contract = new TestEngine(true).Deploy<DelegateByRefContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(new BigInteger(7), contract.InvokeOut());
        Assert.AreEqual(new BigInteger(9), contract.DirectOut());
        Assert.AreEqual(new BigInteger(10), contract.VoidOut());
        Assert.AreEqual(new BigInteger(7), contract.BareStatement());
        Assert.AreEqual(new BigInteger(7), contract.InvokeRef());
        Assert.AreEqual(new BigInteger(11), contract.LambdaOut());
        Assert.AreEqual(new BigInteger(7), contract.NewDelegateOut());
    }

    public abstract class DelegateByRefContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("invokeOut")] public abstract BigInteger? InvokeOut();
        [DisplayName("directOut")] public abstract BigInteger? DirectOut();
        [DisplayName("voidOut")] public abstract BigInteger? VoidOut();
        [DisplayName("bareStatement")] public abstract BigInteger? BareStatement();
        [DisplayName("invokeRef")] public abstract BigInteger? InvokeRef();
        [DisplayName("lambdaOut")] public abstract BigInteger? LambdaOut();
        [DisplayName("newDelegateOut")] public abstract BigInteger? NewDelegateOut();
    }
}
