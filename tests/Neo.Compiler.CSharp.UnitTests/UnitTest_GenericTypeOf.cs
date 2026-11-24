// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_GenericTypeOf.cs file belongs to the neo project and is free
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
public class UnitTest_GenericTypeOf
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void TypeOfUsesConstructedMethodTypeArgument(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System;
            using System.Collections.Generic;
            using System.Runtime.CompilerServices;

            public class Helper<T>
            {
                public static bool IsInt() => typeof(T) == typeof(int);
            }

            public class Contract : SmartContract
            {
                public static bool IntMatches() => MatchesInt(5);
                public static bool StringMatches() => MatchesInt("five");
                public static bool StringMatchesString() => MatchesString("five");
                public static bool InlineIntMatches() => InlineMatchesInt(5);
                public static bool ConcreteMatches() => typeof(int) == typeof(int);
                public static bool ClassIntMatches() => Helper<int>.IsInt();
                public static bool ClassStringMatches() => Helper<string>.IsInt();
                public static bool GenericTypesDiffer() => typeof(System.Collections.Generic.List<int>) != typeof(System.Collections.Generic.List<string>);
                public static bool ArraysDiffer() => typeof(int[]) != typeof(string[]);
                public static bool LambdaIntMatches() => LambdaMatches(5);
                public static bool LambdaStringMatches() => LambdaMatches("five");

                private static bool MatchesInt<T>(T value) => typeof(T) == typeof(int);
                private static bool MatchesString<T>(T value) => typeof(T) == typeof(string);
                private static bool LambdaMatches<T>(T value)
                {
                    Func<bool> check = () => typeof(T) == typeof(int);
                    return check();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static bool InlineMatchesInt<T>(T value) => typeof(T) == typeof(int);
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<GenericTypeOfContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(true, contract.IntMatches());
        Assert.AreEqual(false, contract.StringMatches());
        Assert.AreEqual(true, contract.StringMatchesString());
        Assert.AreEqual(true, contract.InlineIntMatches());
        Assert.AreEqual(true, contract.ConcreteMatches());
        Assert.AreEqual(true, contract.ClassIntMatches());
        Assert.AreEqual(false, contract.ClassStringMatches());
        Assert.AreEqual(true, contract.GenericTypesDiffer());
        Assert.AreEqual(true, contract.ArraysDiffer());
        Assert.AreEqual(true, contract.LambdaIntMatches());
        Assert.AreEqual(false, contract.LambdaStringMatches());
    }

    public abstract class GenericTypeOfContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("intMatches")] public abstract bool? IntMatches();
        [DisplayName("stringMatches")] public abstract bool? StringMatches();
        [DisplayName("stringMatchesString")] public abstract bool? StringMatchesString();
        [DisplayName("inlineIntMatches")] public abstract bool? InlineIntMatches();
        [DisplayName("concreteMatches")] public abstract bool? ConcreteMatches();
        [DisplayName("classIntMatches")] public abstract bool? ClassIntMatches();
        [DisplayName("classStringMatches")] public abstract bool? ClassStringMatches();
        [DisplayName("genericTypesDiffer")] public abstract bool? GenericTypesDiffer();
        [DisplayName("arraysDiffer")] public abstract bool? ArraysDiffer();
        [DisplayName("lambdaIntMatches")] public abstract bool? LambdaIntMatches();
        [DisplayName("lambdaStringMatches")] public abstract bool? LambdaStringMatches();
    }
}
