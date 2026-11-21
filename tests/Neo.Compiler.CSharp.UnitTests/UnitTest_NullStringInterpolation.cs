// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullStringInterpolation
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NullStringInterpolatesAsEmpty(CompilationOptions.OptimizationType optimization)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static string? GetNull() => null;

                [DisplayName("single")]
                public static string Single(string? value) => $"{value}";

                [DisplayName("middle")]
                public static string Middle(string? value) => $"a{value}b";

                [DisplayName("fromMethod")]
                public static bool FromMethod() => $"{GetNull()}" == "";
            }
            """;

        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<InterpolationContract>(
            context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual("", contract.Single(null));
        Assert.AreEqual("value", contract.Single("value"));
        Assert.AreEqual("ab", contract.Middle(null));
        Assert.AreEqual("avalueb", contract.Middle("value"));
        Assert.IsTrue(contract.FromMethod());
    }

    public abstract class InterpolationContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("single")]
        public abstract string? Single(string? value);

        [DisplayName("middle")]
        public abstract string? Middle(string? value);

        [DisplayName("fromMethod")]
        public abstract bool FromMethod();
    }
}
