// Copyright (C) 2015-2026 The Neo Project.
//
// MalformedSyntaxTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

[TestClass]
public class MalformedSyntaxTests
{
    public static IEnumerable<object[]> IncompleteContractSources()
    {
        yield return ["public class Contract : Neo.SmartContract.Framework.SmartContract {"];
        yield return ["public class Contract : Neo.SmartContract.Framework.SmartContract { public static void Test("];
        yield return ["public class Contract : Neo.SmartContract.Framework.SmartContract { public static int Test() => ; }"];
        yield return ["public class Contract : Neo.SmartContract.Framework.SmartContract { public static void Test() { if (true) { } "];
        yield return ["public class Contract : Neo.SmartContract.Framework.SmartContract { public static void Test() { using var value = new ; } }"];
    }

    [DataTestMethod]
    [DynamicData(nameof(IncompleteContractSources), DynamicDataSourceType.Method)]
    public void IncompleteSource_ProducesDiagnosticsWithoutCompilerException(string source)
    {
        Helper.AssertRawCompilationFails(source, "Incomplete contract source must fail with diagnostics.");
    }
}
