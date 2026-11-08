// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract.Testing.Coverage;
using System;

namespace Neo.SmartContract.Testing.UnitTests.Coverage;

[TestClass]
public class DebugCollectionParsingTests
{
    private const string Hash = "0x0000000000000000000000000000000000000000";

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void OptionalRootCollectionsDefaultToEmpty(bool documents, bool methods)
    {
        var json = new JObject { ["hash"] = Hash };
        if (documents) json["documents"] = new JArray();
        if (methods) json["methods"] = new JArray();
        var info = NeoDebugInfo.FromDebugInfoJson(json);
        Assert.AreEqual(0, info.Documents.Count);
        Assert.AreEqual(0, info.Methods.Count);
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void OptionalMethodCollectionsDefaultToEmpty(bool parameters, bool points)
    {
        var method = new JObject { ["id"] = "0", ["name"] = "Contract,run", ["range"] = "0-1" };
        if (parameters) method["params"] = new JArray();
        if (points) method["sequence-points"] = new JArray();
        var json = new JObject { ["hash"] = Hash, ["documents"] = new JArray(), ["methods"] = new JArray(method) };
        var info = NeoDebugInfo.FromDebugInfoJson(json);
        Assert.AreEqual(0, info.Methods[0].Parameters.Count);
        Assert.AreEqual(0, info.Methods[0].SequencePoints.Count);
    }

    [TestMethod]
    [DataRow("documents")]
    [DataRow("methods")]
    public void ExplicitInvalidRootCollectionsAreRejected(string property)
    {
        var json = new JObject { ["hash"] = Hash, ["documents"] = new JArray(), ["methods"] = new JArray() };
        json[property] = "invalid";
        Assert.ThrowsExactly<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
        json[property] = null;
        Assert.ThrowsExactly<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
    }

    [TestMethod]
    [DataRow("params")]
    [DataRow("sequence-points")]
    public void ExplicitInvalidMethodCollectionsAreRejected(string property)
    {
        var method = new JObject { ["id"] = "0", ["name"] = "Contract,run", ["range"] = "0-1" };
        var json = new JObject { ["hash"] = Hash, ["methods"] = new JArray(method) };
        method[property] = "invalid";
        Assert.ThrowsExactly<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
        method[property] = null;
        Assert.ThrowsExactly<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
    }

    [TestMethod]
    [DataRow("null")]
    [DataRow("42")]
    [DataRow("{}")]
    public void InvalidDocumentEntriesCannotShiftSourceIndexes(string invalid)
    {
        var json = new JObject
        {
            ["hash"] = Hash,
            ["documents"] = new JArray(JToken.Parse(invalid), "Contract.cs"),
            ["methods"] = new JArray()
        };
        Assert.ThrowsExactly<FormatException>(() => NeoDebugInfo.FromDebugInfoJson(json));
    }

    [TestMethod]
    public void ValidDocumentIndexesArePreserved()
    {
        var info = NeoDebugInfo.FromDebugInfoJson("""
            {"hash":"0x0000000000000000000000000000000000000000","documents":["First.cs","Second.cs"],
            "methods":[{"id":"0","name":"Contract,run","range":"0-1","sequence-points":["0[1]10:1-10:5"]}]}
            """);
        Assert.AreEqual("Second.cs", info.GetSourceLocation(0)!.FileName);
    }
}
