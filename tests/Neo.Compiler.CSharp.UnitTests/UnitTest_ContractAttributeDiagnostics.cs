// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ContractAttributeDiagnostics.cs file belongs to the neo project and is free
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
public class UnitTest_ContractAttributeDiagnostics
{
    [TestMethod]
    public void EmptyContractPermissionMethodReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission("0x01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4", "a", "")]
public class Contract : SmartContract
{
    public static int Test() => 0;
}
""");

        AssertHasInvalidArgumentDiagnostic(context);
    }

    [TestMethod]
    public void InvalidContractPermissionDescriptorReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission("\uFFFD", "a")]
public class Contract : SmartContract
{
    public static int Test() => 0;
}
""");

        AssertHasInvalidArgumentDiagnostic(context);
    }

    [TestMethod]
    public void EmptyContractPermissionDescriptorReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission("", "a")]
public class Contract : SmartContract
{
    public static int Test() => 0;
}
""");

        AssertHasInvalidArgumentDiagnostic(context);
    }

    [TestMethod]
    public void InvalidContractTrustReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractTrust("\uFFFD")]
public class Contract : SmartContract
{
    public static int Test() => 0;
}
""");

        AssertHasInvalidArgumentDiagnostic(context);
    }

    [TestMethod]
    public void NullContractTrustReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractTrust("\0")]
public class Contract : SmartContract
{
    public static int Test() => 0;
}
""");

        AssertHasInvalidArgumentDiagnostic(context);
    }

    private static void AssertHasInvalidArgumentDiagnostic(CompilationContext context)
    {
        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidArgument), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

}
