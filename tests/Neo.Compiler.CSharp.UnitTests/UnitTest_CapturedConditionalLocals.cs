// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CapturedConditionalLocals.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CapturedConditionalLocals
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;

        public class Contract : SmartContract
        {
            public static bool Guard(bool useCallback)
            {
                bool paused = true;
                Func<bool> callback = null;
                if (useCallback) callback = () => paused;
                if (paused) return false;
                return true;
            }

            public static bool Parameter(bool useCallback, bool paused)
            {
                Func<bool> callback = null;
                if (useCallback) callback = () => paused;
                return paused;
            }

            public static bool ExpressionBody(bool paused) => Invoke(() => paused);

            private static bool Invoke(Func<bool> callback) => callback();

            public static bool Getter
            {
                get
                {
                    bool paused = true;
                    Func<bool> callback = () => paused;
                    return callback();
                }
            }

            public static bool GetterValue() => Getter;

            public static bool SimpleLambda(bool paused)
            {
                Func<bool, bool> outer = value =>
                {
                    bool local = value;
                    Func<bool> inner = () => local;
                    return inner();
                };
                return outer(paused);
            }

            public static bool ParenthesizedLambda(bool paused)
            {
                Func<bool, bool> outer = (bool value) =>
                {
                    bool local = value;
                    Func<bool> inner = () => local;
                    return inner();
                };
                return outer(paused);
            }

            public static bool LoopParameter(bool paused)
            {
                int count = 0;
                while (count < 1)
                {
                    Func<bool> callback = () => paused;
                    if (!callback()) return false;
                    count++;
                }
                return paused;
            }

            public static bool OutCapture(bool paused)
            {
                bool local;
                Assign(out local, paused);
                Func<bool> callback = () => local;
                return callback();
            }

            private static void Assign(out bool target, bool value) => target = value;

            public static bool RefCapture(bool paused)
            {
                bool local = paused;
                Toggle(ref local);
                Func<bool> callback = () => local;
                return callback();
            }

            private static void Toggle(ref bool target) => target = !target;

            public static bool GenericLocal(bool paused) => GenericCapture(paused);

            private static bool GenericCapture<T>(T value)
            {
                T local = value;
                Func<T> callback = () => local;
                return callback().Equals(value);
            }

            public static bool GenericWrite(bool paused) => GenericAssign(!paused, paused);

            private static bool GenericAssign<T>(T initial, T next)
            {
                T local = initial;
                Func<T> callback = () =>
                {
                    local = next;
                    return local;
                };
                return callback().Equals(next) && local.Equals(next);
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ConditionalCaptureKeepsOneStorageOnEveryPath(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<ConditionalContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(false, contract.Guard(false));
        Assert.AreEqual(false, contract.Guard(true));
        Assert.AreEqual(false, contract.Guard(false));
        Assert.AreEqual(true, contract.Parameter(false, true));
        Assert.AreEqual(false, contract.Parameter(true, false));
        Assert.AreEqual(true, contract.Parameter(false, true));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CapturesInOtherBodyShapesKeepTheirValues(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<ConditionalContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(true, contract.ExpressionBody(true));
        Assert.AreEqual(false, contract.ExpressionBody(false));
        Assert.AreEqual(true, contract.GetterValue());
        Assert.AreEqual(true, contract.SimpleLambda(true));
        Assert.AreEqual(false, contract.SimpleLambda(false));
        Assert.AreEqual(true, contract.ParenthesizedLambda(true));
        Assert.AreEqual(false, contract.ParenthesizedLambda(false));
        Assert.AreEqual(true, contract.LoopParameter(true));
        Assert.AreEqual(false, contract.LoopParameter(false));
        Assert.AreEqual(true, contract.OutCapture(true));
        Assert.AreEqual(false, contract.OutCapture(false));
        Assert.AreEqual(false, contract.RefCapture(true));
        Assert.AreEqual(true, contract.RefCapture(false));
        Assert.AreEqual(true, contract.GenericLocal(true));
        Assert.AreEqual(true, contract.GenericLocal(false));
        Assert.AreEqual(true, contract.GenericWrite(true));
        Assert.AreEqual(true, contract.GenericWrite(false));
    }

    public abstract class ConditionalContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("guard")] public abstract bool? Guard(bool useCallback);
        [DisplayName("parameter")] public abstract bool? Parameter(bool useCallback, bool paused);
        [DisplayName("expressionBody")] public abstract bool? ExpressionBody(bool paused);
        [DisplayName("getterValue")] public abstract bool? GetterValue();
        [DisplayName("simpleLambda")] public abstract bool? SimpleLambda(bool paused);
        [DisplayName("parenthesizedLambda")] public abstract bool? ParenthesizedLambda(bool paused);
        [DisplayName("loopParameter")] public abstract bool? LoopParameter(bool paused);
        [DisplayName("outCapture")] public abstract bool? OutCapture(bool paused);
        [DisplayName("refCapture")] public abstract bool? RefCapture(bool paused);
        [DisplayName("genericLocal")] public abstract bool? GenericLocal(bool paused);
        [DisplayName("genericWrite")] public abstract bool? GenericWrite(bool paused);
    }
}
