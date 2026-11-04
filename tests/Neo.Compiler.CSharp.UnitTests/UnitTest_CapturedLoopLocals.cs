// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CapturedLoopLocals.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CapturedLoopLocals
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;
        public class Contract : SmartContract
        {
            public static int V1(int n)
            {
                int i = 0; int t = 0;
                while (i < n) { Func<int> f = () => i; t += f(); i++; }
                return t;
            }
            public static int V2(int n)
            {
                int i = 0; int acc = 0;
                while (i < n) { Func<int> f = () => acc; acc += f() + 1; i++; }
                return acc;
            }
            public static int V3(int n)
            {
                int i = 0; int t = 0;
                while (i < n) { t += i; i++; }
                return t;
            }
            public static int V5(int n)
            {
                int t = 0;
                for (int i = 0; i < n; i++) { Func<int> f = () => i; t += f(); }
                return t;
            }
            public static int V6(int n)
            {
                int i = 0; int acc = 0;
                do { Func<int> f = () => acc; acc += f() + 1; i++; } while (i < n);
                return acc;
            }
            public static int V7()
            {
                int acc = 0;
                foreach (var x in new int[] { 1, 2, 3 }) { Func<int> f = () => acc; acc += f() + x; }
                return acc;
            }
            public static int V8()
            {
                int i = 0; int acc = 5;
                while (i < 3) { Func<int> f = () => acc; acc += f() + 1; i++; }
                return acc;
            }
            public static int V9(int n)
            {
                int i = 0;
                while (i < 3) { Func<int> f = () => n; n += f(); i++; }
                return n;
            }
            public static int V10()
            {
                int acc = 5;
                foreach (var x in new int[] { 1, 2, 3 }) { Func<int> f = () => acc; acc += f() + x; }
                return acc;
            }
            public static int V11()
            {
                int acc = 0;
                foreach (var x in new int[] { 1, 2, 3 }) { Func<int> f = () => x; acc += f(); }
                return acc;
            }
            public static int V12(bool enter)
            {
                int acc = 5;
                if (enter)
                {
                    int i = 0;
                    while (i < 1) { Func<int> f = () => acc; acc += f(); i++; }
                }
                return acc;
            }
            public static int V13()
            {
                int acc = 0;
                for (int i = 0; i < 3; i++) { Func<int> f = () => acc; acc += f() + 1; }
                return acc;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CapturedLoopLocalsMatchCSharp(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        Assert.AreEqual(0, context.Diagnostics.Count);
        var contract = new TestEngine(true).Deploy<LoopContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(new BigInteger(3), contract.V1(3));
        // acc += f() + 1 doubles the previous accumulator and adds one.
        Assert.AreEqual(new BigInteger(7), contract.V2(3));
        Assert.AreEqual(new BigInteger(3), contract.V3(3));
        Assert.AreEqual(new BigInteger(3), contract.V5(3));
        Assert.AreEqual(new BigInteger(7), contract.V6(3));
        Assert.AreEqual(new BigInteger(11), contract.V7());
        Assert.AreEqual(new BigInteger(47), contract.V8());
        Assert.AreEqual(new BigInteger(16), contract.V9(2));
        Assert.AreEqual(new BigInteger(51), contract.V10());
        Assert.AreEqual(new BigInteger(6), contract.V11());
        Assert.AreEqual(new BigInteger(5), contract.V12(false));
        Assert.AreEqual(new BigInteger(10), contract.V12(true));
        Assert.AreEqual(new BigInteger(7), contract.V13());
    }

    public abstract class LoopContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("v1")] public abstract BigInteger? V1(BigInteger n);
        [DisplayName("v2")] public abstract BigInteger? V2(BigInteger n);
        [DisplayName("v3")] public abstract BigInteger? V3(BigInteger n);
        [DisplayName("v5")] public abstract BigInteger? V5(BigInteger n);
        [DisplayName("v6")] public abstract BigInteger? V6(BigInteger n);
        [DisplayName("v7")] public abstract BigInteger? V7();
        [DisplayName("v8")] public abstract BigInteger? V8();
        [DisplayName("v9")] public abstract BigInteger? V9(BigInteger n);
        [DisplayName("v10")] public abstract BigInteger? V10();
        [DisplayName("v11")] public abstract BigInteger? V11();
        [DisplayName("v12")] public abstract BigInteger? V12(bool enter);
        [DisplayName("v13")] public abstract BigInteger? V13();
    }
}
