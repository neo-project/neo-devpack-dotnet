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
public class UnitTest_EnumParsingCulture
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void AsciiEnumNamesMatchIndependentlyOfBuildCulture(CompilationOptions.OptimizationType optimization)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private enum Value { First = 1 }

                [DisplayName("constant")]
                public static bool Constant(string value) => Enum.TryParse(typeof(Value), value, true, out object result);

                [DisplayName("dynamic")]
                public static bool Dynamic(string value, bool ignoreCase) => Enum.TryParse(typeof(Value), value, ignoreCase, out object result);

                [DisplayName("generic")]
                public static int Generic(string value) => Enum.TryParse<Value>(value, true, out var result) ? (int)result : 0;

                [DisplayName("parse")]
                public static int Parse(string value) => (int)Enum.Parse<Value>(value, true);
            }
            """;

        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(source, options);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

            var contract = new TestEngine(true).Deploy<EnumParsingContract>(
                context.CreateExecutable(), context.CreateManifest());
            foreach (var value in new[] { "First", "first", "FIRST" })
            {
                Assert.IsTrue(contract.Constant(value));
                Assert.IsTrue(contract.Dynamic(value, true));
                Assert.AreEqual(BigInteger.One, contract.Generic(value));
                Assert.AreEqual(BigInteger.One, contract.Parse(value));
            }

            Assert.IsTrue(contract.Dynamic("First", false));
            Assert.IsFalse(contract.Dynamic("first", false));
            Assert.IsFalse(contract.Constant("unknown"));
            Assert.AreEqual(BigInteger.Zero, contract.Generic("unknown"));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    public abstract class EnumParsingContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("constant")] public abstract bool Constant(string value);
        [DisplayName("dynamic")] public abstract bool Dynamic(string value, bool ignoreCase);
        [DisplayName("generic")] public abstract BigInteger? Generic(string value);
        [DisplayName("parse")] public abstract BigInteger? Parse(string value);
    }
}
