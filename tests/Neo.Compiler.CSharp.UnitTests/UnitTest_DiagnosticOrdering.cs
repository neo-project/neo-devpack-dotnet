// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DiagnosticOrdering.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DiagnosticOrdering
{
    [TestMethod]
    public void DiagnosticsAreOrderedBySourceAndStableIdentity()
    {
        const string source = """
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static double Unsupported() => 1.0;
    public static int Missing() => missing;
}
""";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsFalse(context.Success);

        var actual = context.Diagnostics.Select(Key).ToArray();
        var expected = actual.OrderBy(item => item, StringComparer.Ordinal).ToArray();
        CollectionAssert.AreEqual(expected, actual);
    }

    private static string Key(Diagnostic diagnostic)
    {
        string path = diagnostic.Location.SourceTree?.FilePath ?? string.Empty;
        int start = diagnostic.Location.IsInSource ? diagnostic.Location.SourceSpan.Start : int.MaxValue;
        int length = diagnostic.Location.IsInSource ? diagnostic.Location.SourceSpan.Length : int.MaxValue;
        return $"{path}\u001f{start:D10}\u001f{length:D10}\u001f{diagnostic.Id}\u001f{diagnostic.Severity}\u001f{diagnostic.GetMessage()}";
    }
}
