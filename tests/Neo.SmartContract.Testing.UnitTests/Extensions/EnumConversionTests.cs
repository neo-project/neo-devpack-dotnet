// Copyright (C) 2015-2026 The Neo Project.
// This file belongs to the neo project and is distributed under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests.Extensions;

[TestClass]
public class EnumConversionTests
{
    public enum SignedByte : sbyte { Minimum = sbyte.MinValue, Maximum = sbyte.MaxValue }
    public enum UnsignedByte : byte { Minimum = 0, Maximum = byte.MaxValue }
    public enum SignedShort : short { Minimum = short.MinValue, Maximum = short.MaxValue }
    public enum UnsignedShort : ushort { Minimum = 0, Maximum = ushort.MaxValue }
    public enum SignedInt : int { Minimum = int.MinValue, Maximum = int.MaxValue }
    public enum UnsignedInt : uint { Minimum = 0, Maximum = uint.MaxValue }
    public enum SignedLong : long { Minimum = long.MinValue, Maximum = long.MaxValue }
    public enum UnsignedLong : ulong { Minimum = 0, Maximum = ulong.MaxValue }

    public static IEnumerable<object[]> Boundaries()
    {
        foreach (var type in new[] { typeof(SignedByte), typeof(UnsignedByte), typeof(SignedShort), typeof(UnsignedShort),
            typeof(SignedInt), typeof(UnsignedInt), typeof(SignedLong), typeof(UnsignedLong) })
        {
            foreach (var value in Enum.GetValues(type)) yield return [type, value];
        }
    }

    [TestMethod]
    [DynamicData(nameof(Boundaries))]
    public void ConvertsDeclaredWidthAndNullableValues(Type type, object expected)
    {
        var value = BigInteger.Parse(Convert.ChangeType(expected, Enum.GetUnderlyingType(type))!.ToString()!, System.Globalization.CultureInfo.InvariantCulture);
        StackItem item = new Integer(value);
        Assert.AreEqual(expected, item.ConvertTo(type));
        var nullableType = typeof(Nullable<>).MakeGenericType(type);
        Assert.AreEqual(expected, item.ConvertTo(nullableType));
        Assert.IsNull(StackItem.Null.ConvertTo(nullableType));
        var array = (System.Array)new Neo.VM.Types.Array([item, StackItem.Null]).ConvertTo(nullableType.MakeArrayType())!;
        Assert.AreEqual(expected, array.GetValue(0));
        Assert.IsNull(array.GetValue(1));
    }

    [TestMethod]
    [DataRow(typeof(SignedByte), "128")]
    [DataRow(typeof(UnsignedByte), "-1")]
    [DataRow(typeof(UnsignedInt), "4294967296")]
    [DataRow(typeof(SignedLong), "9223372036854775808")]
    [DataRow(typeof(UnsignedLong), "18446744073709551616")]
    public void RejectsValuesOutsideUnderlyingRange(Type type, string value)
    {
        StackItem item = new Integer(BigInteger.Parse(value));
        Assert.ThrowsExactly<OverflowException>(() => item.ConvertTo(type));
    }

    [TestMethod]
    public void PreservesUnnamedValues()
        => Assert.AreEqual((UnsignedLong)42, new Integer(42).ConvertTo(typeof(UnsignedLong)));
}
