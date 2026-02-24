// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LocalFunctionSyntax.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class UnitTest_LocalFunctionSyntax
{
    [TestMethod]
    public void LocalFunctionDeclaration_CompilationFails()
    {
        Helper.AssertClassCompilationFails(@"
public static int LocalFunctionIgnoredToday()
{
    int Add(int a, int b)
    {
        return a + b;
    }

    return 1;
}", "Local function declarations must be rejected explicitly.");
    }
}
