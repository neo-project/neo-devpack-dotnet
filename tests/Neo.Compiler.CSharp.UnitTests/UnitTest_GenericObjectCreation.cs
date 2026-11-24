// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_GenericObjectCreation.cs file belongs to the neo project and is free
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
public class UnitTest_GenericObjectCreation
{
    [TestMethod]
    [DataRow("new()")]
    [DataRow("struct")]
    public void TypeParameterConstructor_ReportsDiagnostic(string constraint)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int Test() => Contract.Create<int>();

                private static T Create<T>() where T : {{constraint}} => new T();
            }
            """);

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(static diagnostic => diagnostic.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        var diagnostic = context.Diagnostics.Single(diagnostic => diagnostic.Id == DiagnosticId.SyntaxNotSupported);
        Assert.IsTrue(diagnostic.Location.IsInSource, diagnostics);
        StringAssert.Contains(diagnostic.GetMessage(), "generic type parameter");
        Assert.IsFalse(context.Diagnostics.Any(diagnostic => diagnostic.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }
}
