// Copyright (C) 2015-2026 The Neo Project.
//
// StandardAssertionDiagnosticsTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards;

[TestClass]
public class StandardAssertionDiagnosticsTests
{
    [TestMethod]
    public void AssertLogsReportsExpectedAndActualLogs()
    {
        var test = CreateUninitialized<TestBase<DiagnosticContract>>();
        SetPrivateField(typeof(TestBase<DiagnosticContract>), test, "_contractLogs", new List<string> { "actual" });

        var exception = Assert.ThrowsExactly<AssertFailedException>(() => test.AssertLogs("expected"));

        StringAssert.Contains(exception.Message, "Expected runtime logs: [\"expected\"]");
        StringAssert.Contains(exception.Message, "actual: [\"actual\"]");
    }

    [TestMethod]
    public void AssertNoLogsReportsCapturedLogs()
    {
        var test = CreateUninitialized<TestBase<DiagnosticContract>>();
        SetPrivateField(typeof(TestBase<DiagnosticContract>), test, "_contractLogs", new List<string> { "unexpected" });

        var exception = Assert.ThrowsExactly<AssertFailedException>(test.AssertNoLogs);

        StringAssert.Contains(exception.Message, "Expected no runtime logs");
        StringAssert.Contains(exception.Message, "\"unexpected\"");
    }

    [TestMethod]
    public void AssertTransferEventReportsExpectedAndActualEvents()
    {
        var test = CreateUninitialized<Nep17Tests<DiagnosticNep17Contract>>();
        var actual = UInt160.Parse("0x0102030405060708090a0102030405060708090a");
        var expected = UInt160.Parse("0x0a0908070605040302010a090807060504030201");
        SetPrivateField(typeof(Nep17Tests<DiagnosticNep17Contract>), test, "raisedTransfer",
            new List<(UInt160? from, UInt160? to, BigInteger? amount)> { (actual, null, 1) });

        var exception = Assert.ThrowsExactly<AssertFailedException>(() => test.AssertTransferEvent(expected, null, 2));

        StringAssert.Contains(exception.Message, "Expected transfer events");
        StringAssert.Contains(exception.Message, expected.ToString());
        StringAssert.Contains(exception.Message, actual.ToString());
    }

    [TestMethod]
    public void AssertNoTransferEventReportsCapturedEvents()
    {
        var test = CreateUninitialized<Nep17Tests<DiagnosticNep17Contract>>();
        var account = UInt160.Parse("0x0102030405060708090a0102030405060708090a");
        SetPrivateField(typeof(Nep17Tests<DiagnosticNep17Contract>), test, "raisedTransfer",
            new List<(UInt160? from, UInt160? to, BigInteger? amount)> { (account, null, 1) });

        var exception = Assert.ThrowsExactly<AssertFailedException>(test.AssertNoTransferEvent);

        StringAssert.Contains(exception.Message, "Expected no transfer events");
        StringAssert.Contains(exception.Message, account.ToString());
    }

    [TestMethod]
    public void AssertOnChangeOwnerEventReportsExpectedAndActualEvents()
    {
        var test = CreateUninitialized<OwnableTests<DiagnosticOwnableContract>>();
        var actual = UInt160.Parse("0x0102030405060708090a0102030405060708090a");
        var expected = UInt160.Parse("0x0a0908070605040302010a090807060504030201");
        SetPrivateField(typeof(OwnableTests<DiagnosticOwnableContract>), test, "raisedOnChangeOwner",
            new List<(UInt160? from, UInt160? to)> { (actual, null) });

        var exception = Assert.ThrowsExactly<AssertFailedException>(() => test.AssertOnChangeOwnerEvent(expected, null));

        StringAssert.Contains(exception.Message, "Expected owner change events");
        StringAssert.Contains(exception.Message, expected.ToString());
        StringAssert.Contains(exception.Message, actual.ToString());
    }

    [TestMethod]
    public void AssertNoOnChangeOwnerEventReportsCapturedEvents()
    {
        var test = CreateUninitialized<OwnableTests<DiagnosticOwnableContract>>();
        var account = UInt160.Parse("0x0102030405060708090a0102030405060708090a");
        SetPrivateField(typeof(OwnableTests<DiagnosticOwnableContract>), test, "raisedOnChangeOwner",
            new List<(UInt160? from, UInt160? to)> { (account, null) });

        var exception = Assert.ThrowsExactly<AssertFailedException>(test.AssertNoOnChangeOwnerEvent);

        StringAssert.Contains(exception.Message, "Expected no owner change events");
        StringAssert.Contains(exception.Message, account.ToString());
    }

    private static T CreateUninitialized<T>() where T : class
    {
        return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    }

    private static void SetPrivateField<T>(Type declaringType, T instance, string fieldName, object value)
    {
        var field = declaringType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Field {fieldName} was not found.");

        field.SetValue(instance, value);
    }

    private abstract class DiagnosticContract(SmartContractInitialize initialize) : SmartContract(initialize), IContractInfo
    {
        public static NefFile Nef => throw new NotSupportedException();
        public static ContractManifest Manifest => throw new NotSupportedException();
    }

    private abstract class DiagnosticNep17Contract(SmartContractInitialize initialize) : SmartContract(initialize), INep17Standard, IContractInfo
    {
        public static NefFile Nef => throw new NotSupportedException();
        public static ContractManifest Manifest => throw new NotSupportedException();

#pragma warning disable CS0067
        public event INep17Standard.delTransfer? OnTransfer;
#pragma warning restore CS0067

        public abstract string? Symbol { get; }
        public abstract BigInteger? Decimals { get; }
        public abstract BigInteger? TotalSupply { get; }
        public abstract BigInteger? BalanceOf(UInt160? owner);
        public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);
    }

    private abstract class DiagnosticOwnableContract(SmartContractInitialize initialize) : SmartContract(initialize), IOwnable, IContractInfo
    {
        public static NefFile Nef => throw new NotSupportedException();
        public static ContractManifest Manifest => throw new NotSupportedException();

#pragma warning disable CS0067
        public event IOwnable.delSetOwner? OnSetOwner;
#pragma warning restore CS0067

        public abstract UInt160? Owner { get; set; }
    }
}
