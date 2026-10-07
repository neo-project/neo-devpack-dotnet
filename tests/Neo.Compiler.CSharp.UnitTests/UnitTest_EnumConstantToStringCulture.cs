// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
[DoNotParallelize]
public class UnitTest_EnumConstantToStringCulture
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void UnknownConstantUsesInvariantNumericRepresentation(CompilationOptions.OptimizationType optimization)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private enum Value { Known = 1 }

                [DisplayName("constant")]
                public static string Constant() => ((Value)(-99)).ToString();

                [DisplayName("runtime")]
                public static string Runtime(int value) => ((Value)value).ToString();
            }
            """;

        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "~";
            CultureInfo.CurrentCulture = culture;

            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(source, options);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

            var contract = new TestEngine(true).Deploy<EnumFormattingContract>(
                context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual("-99", contract.Runtime(-99));
            Assert.AreEqual("-99", contract.Constant());
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    public abstract class EnumFormattingContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("constant")]
        public abstract string? Constant();

        [DisplayName("runtime")]
        public abstract string? Runtime(BigInteger value);
    }
}
