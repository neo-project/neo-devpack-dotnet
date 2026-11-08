// Copyright (C) 2015-2026 The Neo Project.
//
// UnsupportedSyntaxDiagnosticOrderTests.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class UnsupportedSyntaxDiagnosticOrderTests
{
    [TestMethod]
    public void SupportedDiagnostics_ShouldHaveStableRuleIdOrder()
    {
        var ids = new UnsupportedSyntaxAnalyzer()
            .SupportedDiagnostics
            .Select(static descriptor => descriptor.Id)
            .ToArray();
        var sortedIds = ids.OrderBy(static id => id, StringComparer.Ordinal).ToArray();

        CollectionAssert.AreEqual(sortedIds, ids);
    }
}
