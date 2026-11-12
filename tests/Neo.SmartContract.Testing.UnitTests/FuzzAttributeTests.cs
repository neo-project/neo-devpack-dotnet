// Copyright (C) 2015-2026 The Neo Project.
//
// FuzzAttributeTests.cs file belongs to the neo project and is free
// software distributed under the terms of the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Attributes;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Neo.SmartContract.Testing.UnitTests;

[TestClass]
public class FuzzAttributeTests
{
    private static readonly MethodInfo FuzzMethod =
        typeof(FuzzAttributeTests).GetMethod(nameof(FuzzParameters), BindingFlags.NonPublic | BindingFlags.Static)!;

    [TestMethod]
    public void FuzzTestGeneratesDeterministicRows()
    {
        var attribute = new FuzzTestAttribute(iterations: 8, seed: 42);
        var rows = attribute.GetData(FuzzMethod).ToArray();
        var replay = attribute.GetData(FuzzMethod).ToArray();
        var differentSeed = new FuzzTestAttribute(iterations: 8, seed: 43).GetData(FuzzMethod).ToArray();

        Assert.AreEqual(8, rows.Length);
        Assert.IsTrue(rows.All(row => row.Length == 4));
        Assert.IsInstanceOfType<int>(rows[0][0]);
        Assert.IsInstanceOfType<string>(rows[0][1]);
        Assert.IsInstanceOfType<UInt160>(rows[0][2]);
        Assert.IsInstanceOfType<BigInteger>(rows[0][3]);
        for (int i = 0; i < rows.Length; i++)
            CollectionAssert.AreEqual(rows[i], replay[i]);
        Assert.IsTrue(rows.Zip(differentSeed, (left, right) => left.SequenceEqual(right)).Any(equal => !equal));
    }

    [TestMethod]
    public void FuzzAttributesAreStandaloneTestMethods()
    {
        Assert.IsTrue(new FuzzTestAttribute() is TestMethodAttribute);
        Assert.IsTrue(new FuzzUInt160Attribute() is TestMethodAttribute);
        Assert.IsTrue(new FuzzAmountAttribute() is TestMethodAttribute);
        Assert.IsTrue(new FuzzStringAttribute() is TestMethodAttribute);
        Assert.IsTrue(new FuzzIntAttribute() is TestMethodAttribute);
    }

    [TestMethod]
    public void FuzzAmountUsesInclusiveBounds()
    {
        var attribute = new FuzzAmountAttribute(count: 64, seed: 42) { Min = "0", Max = "1" };
        var values = attribute.GetData(FuzzMethod).Select(row => (BigInteger)row[0]!).ToArray();

        Assert.AreEqual(64, values.Length);
        Assert.IsTrue(values.All(value => value >= BigInteger.Zero && value <= BigInteger.One));
        Assert.IsTrue(values.Contains(BigInteger.Zero));
        Assert.IsTrue(values.Contains(BigInteger.One));
    }

    [TestMethod]
    public void FuzzIntUsesInclusiveBounds()
    {
        var attribute = new FuzzIntAttribute(count: 64, seed: 42) { Min = 0, Max = 1 };
        var values = attribute.GetData(FuzzMethod).Select(row => (int)row[0]!).ToArray();

        Assert.AreEqual(64, values.Length);
        Assert.IsTrue(values.All(value => value is 0 or 1));
        Assert.IsTrue(values.Contains(0));
        Assert.IsTrue(values.Contains(1));
    }

    [TestMethod]
    public void FuzzStringRespectsMaximumLength()
    {
        var attribute = new FuzzStringAttribute(count: 64, seed: 42) { MaxLength = 8 };
        var values = attribute.GetData(FuzzMethod).Select(row => (string)row[0]!).ToArray();

        Assert.AreEqual(64, values.Length);
        Assert.IsTrue(values.All(value => value.Length <= 8));
        Assert.IsTrue(values.Contains(string.Empty));
    }

    [TestMethod]
    public void FuzzUInt160GeneratesRequestedCount()
    {
        var values = new FuzzUInt160Attribute(count: 12, seed: 42).GetData(FuzzMethod).ToArray();

        Assert.AreEqual(12, values.Length);
        Assert.IsTrue(values.All(row => row.Length == 1 && row[0] is UInt160));
    }

    [TestMethod]
    public void UnsupportedParameterTypeThrows()
    {
        var method = typeof(FuzzAttributeTests).GetMethod(nameof(UnsupportedParameters), BindingFlags.NonPublic | BindingFlags.Static)!;
        var attribute = new FuzzTestAttribute();

        Assert.ThrowsExactly<NotSupportedException>(() => attribute.GetData(method).Single());
    }

    [FuzzTest(iterations: 2, seed: 42)]
    public void StandaloneFuzzTestIsDiscovered(int number, string text)
    {
        Assert.IsNotNull(text);
    }

    [FuzzInt(count: 2, seed: 42, Min = 0, Max = 1)]
    public void StandaloneFuzzIntIsDiscovered(int value)
    {
        Assert.IsTrue(value is 0 or 1);
    }

    [FuzzAmount(count: 2, seed: 42, Min = "0", Max = "1")]
    public void StandaloneFuzzAmountIsDiscovered(BigInteger value)
    {
        Assert.IsTrue(value >= BigInteger.Zero && value <= BigInteger.One);
    }

    private static void FuzzParameters(int number, string text, UInt160 address, BigInteger amount) { }
    private static void UnsupportedParameters(DateTime value) { }
}
