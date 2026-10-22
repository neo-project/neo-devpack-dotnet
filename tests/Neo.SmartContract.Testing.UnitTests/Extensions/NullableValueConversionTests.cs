// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Testing.UnitTests.Extensions;

[TestClass]
public class NullableValueConversionTests
{
    public struct Result { public int Value; }
    public class Container { public Result? Value { get; set; } }

    [TestMethod]
    [DataRow(0)]
    [DataRow(65)]
    [DataRow(0xD800)]
    [DataRow(0xFFFF)]
    public void CharacterResultsRetainTheirUtf16Value(int value)
    {
        var item = new Integer(value);
        Assert.AreEqual((char)value, item.ConvertTo(typeof(char)));
        Assert.AreEqual((char)value, item.ConvertTo(typeof(char?)));
        var array = (char?[])new Neo.VM.Types.Array([item, StackItem.Null]).ConvertTo(typeof(char?[]))!;
        CollectionAssert.AreEqual(new char?[] { (char)value, null }, array);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0x10000)]
    public void CharacterResultsRejectValuesOutsideTheUtf16Range(int value)
    {
        Assert.ThrowsExactly<OverflowException>(() => new Integer(value).ConvertTo(typeof(char)));
        Assert.ThrowsExactly<OverflowException>(() => new Integer(value).ConvertTo(typeof(char?)));
    }

    [TestMethod]
    public void NullableStructResultsWorkAtEveryNestingLevel()
    {
        var item = new Struct([new Integer(7)]);
        Assert.AreEqual(7, ((Result)item.ConvertTo(typeof(Result))!).Value);
        Assert.AreEqual(7, ((Result?)item.ConvertTo(typeof(Result?)))!.Value.Value);
        var array = (Result?[])new Neo.VM.Types.Array([item, StackItem.Null]).ConvertTo(typeof(Result?[]))!;
        Assert.AreEqual(7, array[0]!.Value.Value);
        Assert.IsNull(array[1]);
        var container = (Container)new Neo.VM.Types.Array([item]).ConvertTo(typeof(Container))!;
        Assert.AreEqual(7, container.Value!.Value.Value);
        Assert.IsNull(StackItem.Null.ConvertTo(typeof(Result?)));
        Assert.IsNull(StackItem.Null.ConvertTo(typeof(char?)));
        Assert.ThrowsExactly<FormatException>(() => new Struct().ConvertTo(typeof(Result?)));
    }
}
