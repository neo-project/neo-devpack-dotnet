// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_OptimizedDebugMethods.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral;

[TestClass]
public class UnitTest_OptimizedDebugMethods
{
    [TestMethod]
    public void OptimizedDebugInfoRetainsAllAbiMethods()
    {
        var testContractPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract_Types.cs").FullName;
        var results = new CompilationEngine(new CompilationOptions
        {
            Debug = CompilationOptions.DebugType.Extended,
            Optimize = CompilationOptions.OptimizationType.Experimental,
            Nullable = NullableContextOptions.Enable
        }).CompileSources(testContractPath);

        Assert.AreEqual(1, results.Count);
        Assert.IsTrue(results[0].Success, string.Join(System.Environment.NewLine, results[0].Diagnostics));

        var (_, manifest, debugInfo) = results[0].CreateResults();
        HashSet<int> debugAbiOffsets = ((JArray)debugInfo["methods"]!)
            .Select(method => method?["abi"] as JObject)
            .Where(abi => abi is not null)
            .Select(abi => int.Parse(abi!["offset"]!.ToString()))
            .ToHashSet();

        Assert.AreEqual(manifest.Abi.Methods.Length, debugAbiOffsets.Count);
        foreach (var method in manifest.Abi.Methods)
            Assert.IsTrue(debugAbiOffsets.Contains(method.Offset), $"Missing debug information for ABI method '{method.Name}'.");
    }
}
