// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_StoredDefaults.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StoredDefaults
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using Neo.SmartContract.Framework.Attributes;

        public class StoredDefaults : SmartContract
        {
            public enum State { None, Active }

            [Stored(0x01)] public static int Number { get; set; }
            [Stored(0x02)] public static bool Flag { get; set; }
            [Stored(0x03)] public static char Letter { get; set; }
            [Stored(0x04)] public static byte[] Data { get; set; }
            [Stored(0x05)] public static State Status { get; set; }
            [Stored(0x06)] public static State InitializedStatus { get; set; } = State.Active;

            public static int NumberDefault() => Number;
            public static bool FlagDefault() => Flag;
            public static int LetterDefault() => Letter;
            public static bool DataDefault() => Data == null;
            public static int StatusDefault() => (int)Status;
            public static int InitializedStatusDefault() => (int)InitializedStatus;

            public static bool FlagRoundTrip() { Flag = true; return Flag; }
            public static int LetterRoundTrip() { Letter = 'A'; return Letter; }
            public static bool DataRoundTrip() { Data = new byte[] { 1, 2 }; return Data[1] == 2; }
            public static int StatusRoundTrip() { Status = State.Active; return (int)Status; }
        }
        """;

    [TestMethod]
    public void MissingKeysReturnDefaultsAndWrittenValuesRoundTrip()
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(path, Source);
        try
        {
            var result = new CompilationEngine(new CompilationOptions { SkipRestoreIfAssetsPresent = true })
                .CompileSources(path).Single();
            Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics));

            var engine = new TestEngine(true);
            var contract = engine.Deploy<StoredDefaultsContract>(result.CreateExecutable(), result.CreateManifest());

            Assert.AreEqual(0, contract.NumberDefault());
            Assert.IsFalse(contract.FlagDefault());
            Assert.AreEqual(0, contract.LetterDefault());
            Assert.IsTrue(contract.DataDefault());
            Assert.AreEqual(0, contract.StatusDefault());
            Assert.AreEqual(1, contract.InitializedStatusDefault());

            Assert.IsTrue(contract.FlagRoundTrip());
            Assert.AreEqual('A', contract.LetterRoundTrip());
            Assert.IsTrue(contract.DataRoundTrip());
            Assert.AreEqual(1, contract.StatusRoundTrip());
        }
        finally
        {
            File.Delete(path);
        }
    }

    public abstract class StoredDefaultsContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("numberDefault")] public abstract int NumberDefault();
        [DisplayName("flagDefault")] public abstract bool FlagDefault();
        [DisplayName("letterDefault")] public abstract int LetterDefault();
        [DisplayName("dataDefault")] public abstract bool DataDefault();
        [DisplayName("statusDefault")] public abstract int StatusDefault();
        [DisplayName("initializedStatusDefault")] public abstract int InitializedStatusDefault();
        [DisplayName("flagRoundTrip")] public abstract bool FlagRoundTrip();
        [DisplayName("letterRoundTrip")] public abstract int LetterRoundTrip();
        [DisplayName("dataRoundTrip")] public abstract bool DataRoundTrip();
        [DisplayName("statusRoundTrip")] public abstract int StatusRoundTrip();
    }
}
