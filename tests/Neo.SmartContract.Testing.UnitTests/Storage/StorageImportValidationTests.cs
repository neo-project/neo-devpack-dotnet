// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Persistence.Providers;
using Neo.SmartContract.Testing.Storage;
using System;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests.Storage;

[TestClass]
public class StorageImportValidationTests
{
    [TestMethod]
    [DataRow("invalid-key")]
    [DataRow("invalid-value")]
    [DataRow("short-key")]
    [DataRow("non-string")]
    [DataRow("duplicate")]
    [DataRow("existing")]
    public void EngineImportRejectsInvalidBatchBeforeWriting(string kind)
    {
        var store = new EngineStorage(new MemoryStore());
        var key = Convert.ToBase64String(new byte[] { 1, 0, 0, 0, 7 });
        var existing = new Neo.SmartContract.StorageKey { Id = 2, Key = new byte[] { 8 } };
        store.Snapshot.Add(existing, new StorageItem(new byte[] { 42 }));
        var before = store.Export().ToString();
        var json = new JObject { [key] = "AQ==" };
        switch (kind)
        {
            case "invalid-key": json["!"] = "AQ=="; break;
            case "invalid-value": json["AgAAAAk="] = "!"; break;
            case "short-key": json["AQ=="] = "AQ=="; break;
            case "non-string": json["AgAAAAk="] = 1; break;
            case "duplicate": json["AQAAAA=="] = new JObject { ["Bw=="] = "Ag==" }; break;
            case "existing": json[Convert.ToBase64String(existing.ToArray())] = "Ag=="; break;
        }
        if (kind == "existing") Assert.ThrowsExactly<ArgumentException>(() => store.Import(json));
        else Assert.ThrowsExactly<FormatException>(() => store.Import(json));
        Assert.AreEqual(before, store.Export().ToString(), "Rejected imports must preserve storage.");
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ContractImportRejectsInvalidEntriesBeforeOverwriting(bool malformedValue)
    {
        var engine = new TestEngine(true);
        var storage = engine.Native.NEO.Storage;
        storage.Put("preserve", BigInteger.One);
        var before = storage.Export().ToString();
        var json = storage.Export();
        var values = (JObject)json.Properties.Single().Value!;
        values[Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("preserve"))] = "Ag==";
        values["YmFk"] = malformedValue ? (JToken)"!" : new JObject();
        Assert.ThrowsExactly<FormatException>(() => storage.Import(json));
        Assert.AreEqual(before, storage.Export().ToString());
    }

    [TestMethod]
    public void EngineImportAcceptsAnEmptyStorageKeyAndValue()
    {
        var store = new EngineStorage(new MemoryStore());
        store.Import(new JObject { ["AQAAAA=="] = "" });
        var values = (JObject)store.Export()["AQAAAA=="]!;
        Assert.AreEqual("", values[""]!.GetString());
    }

    [TestMethod]
    public void ContractImportWithoutMatchingPrefixPreservesStorage()
    {
        var storage = new TestEngine(true).Native.NEO.Storage;
        var before = storage.Export().ToString();
        storage.Import(new JObject { ["unrelated"] = "invalid-base64" });
        Assert.AreEqual(before, storage.Export().ToString());
    }

    [TestMethod]
    public void ContractImportIgnoresOtherContractsAndAcceptsEmptyValues()
    {
        var engine = new TestEngine(true);
        var storage = engine.Native.NEO.Storage;
        var json = storage.Export();
        json["unrelated"] = "not-base64";
        var values = (JObject)json.Properties.First().Value!;
        values["ZW1wdHk="] = "";
        storage.Import(json);
        Assert.IsTrue(storage.Contains("empty"));
        Assert.AreEqual(0, storage.Get("empty").Length);
    }
}
