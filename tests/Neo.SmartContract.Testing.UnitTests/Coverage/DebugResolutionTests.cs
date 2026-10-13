// Copyright (C) 2015-2026 The Neo Project.
//
// DebugResolutionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Coverage;
using System;
using System.IO;
using System.IO.Compression;

namespace Neo.SmartContract.Testing.UnitTests.Coverage;

[TestClass]
public class DebugResolutionTests
{
    [TestMethod]
    [DataRow("Token", false)]
    [DataRow("Token.v2", false)]
    [DataRow("Token.v2.release", false)]
    [DataRow("Token", true)]
    [DataRow("Token.v2", true)]
    [DataRow("Token.v2.release", true)]
    public void ManifestLookupPreservesTheCompleteArtifactName(string name, bool compressed)
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            const string json = """{"hash":"0x0000000000000000000000000000000000000000","documents":["Contract.cs"],"methods":[]}""";
            var basePath = Path.Combine(directory, name);
            if (compressed)
            {
                using var archive = ZipFile.Open(basePath + NeoDebugInfo.NEF_DBG_NFO_EXTENSION, ZipArchiveMode.Create);
                using var writer = new StreamWriter(archive.CreateEntry(name + NeoDebugInfo.DEBUG_JSON_EXTENSION).Open());
                writer.Write(json);
            }
            else File.WriteAllText(basePath + NeoDebugInfo.DEBUG_JSON_EXTENSION, json);

            Assert.IsTrue(NeoDebugInfo.TryLoadManifestDebugInfo(basePath + NeoDebugInfo.MANIFEST_FILE_EXTENSION, out var info));
            Assert.IsNotNull(info);
            Assert.AreEqual("Contract.cs", info.Documents[0]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void UnmappedInstructionsDoNotInventASourceLocation(bool hasLaterPoint)
    {
        NeoDebugInfo.SequencePoint[] points = hasLaterPoint ? [new(5, 0, (10, 1), (10, 8))] : [];
        var info = new NeoDebugInfo(UInt160.Zero, "", ["Contract.cs"],
            [new("id", "Contract", "main", (0, 10), [], points)]);
        Assert.IsNull(info.GetSourceLocation(0));
        Assert.IsNull(info.GetSourceLocation(11));
    }

    [TestMethod]
    public void SourceLookupUsesTheClosestPrecedingPoint()
    {
        var info = new NeoDebugInfo(UInt160.Zero, "", ["Contract.cs"],
            [new("id", "Contract", "main", (0, 10), [],
                [new(8, 0, (20, 2), (20, 8)), new(3, 0, (10, 1), (10, 8))])]);
        Assert.AreEqual(10, info.GetSourceLocation(3)!.Line);
        Assert.AreEqual(10, info.GetSourceLocation(7)!.Line);
        Assert.AreEqual(20, info.GetSourceLocation(8)!.Line);
        Assert.AreEqual(20, info.GetSourceLocation(10)!.Line);
    }
}
