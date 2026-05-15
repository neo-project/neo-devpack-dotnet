// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableFrameworkAnnotations.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableFrameworkAnnotations
{
    [TestMethod]
    public void ContractLookupWithoutNullCheck_ReportsNullableDereference()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
    public static string GetName(UInt160 hash)
    {
        return ContractManagement.GetContract(hash).Manifest.Name;
    }

    public static string GetNameById(int id)
    {
        return ContractManagement.GetContractById(id).Manifest.Name;
    }
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        var nullableDereferenceWarnings = context.Diagnostics.Count(d => d.Id == "CS8602");
        Assert.IsTrue(
            nullableDereferenceWarnings >= 2,
            $"Expected a nullable dereference warning for unchecked contract lookup.\n{string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()))}"
        );
    }
}
