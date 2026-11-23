// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_HelperStaticConstructors.cs file belongs to the neo project and is free
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
public class UnitTest_HelperStaticConstructors
{
    [TestMethod]
    [DataRow("helper")]
    [DataRow("base")]
    [DataRow("nested")]
    public void ExplicitStaticConstructorOutsideContractReportsDiagnostic(string kind)
    {
        string source = kind switch
        {
            "helper" => """
                using Neo.SmartContract.Framework;
                public static class Helper
                {
                    public static readonly int Value;
                    static Helper() => Value = 1;
                }
                public class Contract : SmartContract
                {
                    public static int Get() => Helper.Value;
                }
                """,
            "base" => """
                using Neo.SmartContract.Framework;
                public class Base : SmartContract
                {
                    public static readonly int Value;
                    static Base() => Value = 1;
                }
                public class Contract : Base
                {
                    public static int Get() => Value;
                }
                """,
            _ => """
                using Neo.SmartContract.Framework;
                public class Contract : SmartContract
                {
                    public static int Get() => Nested.Value;
                    private static class Nested
                    {
                        public static readonly int Value;
                        static Nested() => Value = 1;
                    }
                }
                """
        };

        var context = TestHelper.CompileSingleContract(source);
        string diagnostics = string.Join(Environment.NewLine, context.Diagnostics);
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.SyntaxNotSupported &&
            d.GetMessage().Contains("Static constructor", StringComparison.Ordinal)), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }
}

