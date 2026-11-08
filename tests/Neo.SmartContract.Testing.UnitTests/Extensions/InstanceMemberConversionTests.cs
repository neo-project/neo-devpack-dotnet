// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Attributes;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM.Types;
using System;

namespace Neo.SmartContract.Testing.UnitTests.Extensions;

[TestClass]
public class InstanceMemberConversionTests
{
    public class PlainResult
    {
        public static int Shared { get; set; } = 42;
        public int Value { get; set; }
    }

    public class OrderedResult
    {
        [FieldOrder(0)] public static int Shared { get; set; } = 42;
        [FieldOrder(0)] public int Value { get; set; }
    }

    public class IndexedResult
    {
        public int this[int index] { get => index; set => throw new InvalidOperationException("An indexer is not a result field."); }
        public int Value { get; set; }
    }

    public class BaseResult
    {
        [FieldOrder(1)] public int Second { get; set; }
        public int ReadOnly => 99;
    }

    public class InheritedResult : BaseResult
    {
        [FieldOrder(0)] public int First { get; set; }
    }

    public struct StructResult
    {
        public const int Constant = 99;
        public static int Shared;
        public int Value;
    }

    [TestMethod]
    public void ObjectConversionPreservesStaticState()
    {
        try
        {
            var result = (PlainResult)new Neo.VM.Types.Array([new Integer(7)]).ConvertTo(typeof(PlainResult))!;
            Assert.AreEqual(7, result.Value);
            Assert.AreEqual(42, PlainResult.Shared);
        }
        finally { PlainResult.Shared = 42; }
    }

    [TestMethod]
    public void ExplicitOffsetsIgnoreStaticProperties()
    {
        var result = (OrderedResult)new Neo.VM.Types.Array([new Integer(7)]).ConvertTo(typeof(OrderedResult))!;
        Assert.AreEqual(7, result.Value);
        Assert.AreEqual(42, OrderedResult.Shared);
    }

    [TestMethod]
    public void ObjectConversionIgnoresIndexers()
    {
        var result = (IndexedResult)new Neo.VM.Types.Array([new Integer(7)]).ConvertTo(typeof(IndexedResult))!;
        Assert.AreEqual(7, result.Value);
    }

    [TestMethod]
    public void StructConversionIgnoresStaticAndConstantFields()
    {
        StructResult.Shared = 42;
        var result = (StructResult)new Struct([new Integer(7)]).ConvertTo(typeof(StructResult))!;
        Assert.AreEqual(7, result.Value);
        Assert.AreEqual(42, StructResult.Shared);
    }

    [TestMethod]
    public void InheritedPropertiesKeepTheirExplicitOrder()
    {
        var result = (InheritedResult)new Neo.VM.Types.Array([new Integer(7), new Integer(8)]).ConvertTo(typeof(InheritedResult))!;
        Assert.AreEqual(7, result.First);
        Assert.AreEqual(8, result.Second);
        Assert.AreEqual(99, result.ReadOnly);
    }

    [TestMethod]
    public void CachedMembersRetainInstanceValues()
    {
        foreach (int expected in new[] { 7, 8 })
        {
            var result = (PlainResult)new Neo.VM.Types.Array([new Integer(expected)]).ConvertTo(typeof(PlainResult))!;
            Assert.AreEqual(expected, result.Value);
            Assert.AreEqual(42, PlainResult.Shared);
        }
    }

    [TestMethod]
    public void InstanceFieldCountStillMustMatch()
    {
        Assert.ThrowsExactly<FormatException>(() => new Struct().ConvertTo(typeof(StructResult)));
        Assert.ThrowsExactly<FormatException>(() => new Struct([new Integer(1), new Integer(2)]).ConvertTo(typeof(StructResult)));
    }
}
