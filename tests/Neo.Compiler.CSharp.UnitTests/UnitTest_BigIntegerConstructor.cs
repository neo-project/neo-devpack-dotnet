// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_BigIntegerConstructor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_BigIntegerConstructor
{
    [TestMethod]
    public void ParameterlessConstructor_ReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System.Numerics;

            public class Contract : SmartContract
            {
                public static BigInteger Create() => new BigInteger();
            }
            """);

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(static diagnostic => diagnostic.ToString()));
        Assert.IsFalse(context.Success, diagnostics);

        var diagnostic = context.Diagnostics.Single(diagnostic => diagnostic.Id == DiagnosticId.BigIntegerCreation);
        StringAssert.Contains(diagnostic.GetMessage(), "BigInteger.Zero");
        Assert.IsFalse(context.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [DataTestMethod]
    [DataRow("int")]
    [DataRow("uint")]
    [DataRow("long")]
    [DataRow("ulong")]
    public void UnsupportedIntegralConstructors_ReportDiagnostic(string parameterType)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;
            using System.Numerics;

            public class Contract : SmartContract
            {
                public static BigInteger Create({{parameterType}} value) => new BigInteger(value);
            }
            """);

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(static diagnostic => diagnostic.ToString()));
        Assert.IsFalse(context.Success, diagnostics);

        var diagnostic = context.Diagnostics.Single(diagnostic => diagnostic.Id == DiagnosticId.BigIntegerCreation);
        Assert.IsTrue(diagnostic.Location.IsInSource, diagnostics);
        StringAssert.Contains(diagnostic.GetMessage(), "implicit conversion");
        StringAssert.Contains(diagnostic.GetMessage(), "BigInteger(byte[])");
        Assert.IsFalse(context.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void UnsignedBigEndianConstructor_ReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System.Numerics;

            public class Contract : SmartContract
            {
                public static BigInteger Create(byte[] value) => new BigInteger(value, true, true);
            }
            """);

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(static diagnostic => diagnostic.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.AreEqual(1, context.Diagnostics.Count(diagnostic => diagnostic.Id == DiagnosticId.BigIntegerCreation), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void ByteArrayConstructor_Compiles()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System.Numerics;

            public class Contract : SmartContract
            {
                public static BigInteger Create(byte[] value) => new BigInteger(value);
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
    }

    [TestMethod]
    public void UserDefinedBigIntegerConstructor_Compiles()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;

            public class BigInteger
            {
                public BigInteger(int value)
                {
                }
            }

            public class Contract : SmartContract
            {
                public static int Create()
                {
                    _ = new BigInteger(1);
                    return 1;
                }
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
    }
}
