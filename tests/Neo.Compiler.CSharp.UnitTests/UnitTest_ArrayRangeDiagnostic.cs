// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ArrayRangeDiagnostic.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ArrayRangeDiagnostic
{
    [TestMethod]
    public void GeneralArrayRange_ShouldReportTheProfileDiagnostic()
    {
        const string source = """
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static int[] Slice(int[] values) => values[1..^1];
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var diagnostic = context.Diagnostics.Single(item => item.Id == "NC2010");

        Assert.IsFalse(context.Success);
        Assert.AreEqual(
            "Range access is not supported for 'int[]'. Use ranges only with byte[] or string receivers.",
            diagnostic.GetMessage());
    }
}
