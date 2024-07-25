using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class NullableTest : TestBase<Contract_Nullable>
{
    public NullableTest() : base(Contract_Nullable.Nef, Contract_Nullable.Manifest)
    {
    }

    [TestMethod]
    public void TestNullableEqual()
    {
        Assert.IsTrue(Contract.BigIntegerNullableEqual());
        Assert.IsTrue(Contract.BigIntegerNullableNotEqual());
        Assert.IsTrue(Contract.BigIntegerNullableEqualNull());
        Assert.IsTrue(Contract.H160NullableNotEqual());
        Assert.IsTrue(Contract.H160NullableEqualNull());
        Assert.IsTrue(Contract.H256NullableNotEqual());
        Assert.IsTrue(Contract.H256NullableEqual());
        Assert.IsTrue(Contract.ByteNullableEqual());
        Assert.IsTrue(Contract.ByteNullableNotEqual());
        Assert.IsTrue(Contract.ByteNullableEqualNull());
        Assert.IsTrue(Contract.SByteNullableEqual());
        Assert.IsTrue(Contract.SByteNullableNotEqual());
        Assert.IsTrue(Contract.SByteNullableEqualNull());
        Assert.IsTrue(Contract.ShortNullableEqual());
        Assert.IsTrue(Contract.ShortNullableNotEqual());
        Assert.IsTrue(Contract.ShortNullableEqualNull());
        Assert.IsTrue(Contract.UShortNullableEqual());
        Assert.IsTrue(Contract.UShortNullableNotEqual());
        Assert.IsTrue(Contract.UShortNullableEqualNull());
        Assert.IsTrue(Contract.IntNullableEqual());
        Assert.IsTrue(Contract.IntNullableNotEqual());
        Assert.IsTrue(Contract.IntNullableEqualNull());
        Assert.IsTrue(Contract.UIntNullableEqual());
        Assert.IsTrue(Contract.UIntNullableNotEqual());
        Assert.IsTrue(Contract.UIntNullableEqualNull());
        Assert.IsTrue(Contract.LongNullableEqual());
        Assert.IsTrue(Contract.LongNullableNotEqual());
        Assert.IsTrue(Contract.LongNullableEqualNull());
        Assert.IsTrue(Contract.ULongNullableEqual());
        Assert.IsTrue(Contract.ULongNullableNotEqual());
        Assert.IsTrue(Contract.ULongNullableEqualNull());
        Assert.IsTrue(Contract.BoolNullableEqual());
        Assert.IsTrue(Contract.BoolNullableNotEqual());
        Assert.IsTrue(Contract.BoolNullableEqualNull());
        Assert.IsTrue(Contract.GetNullableValue());
        Assert.IsTrue(Contract.NullableEqual());
        Assert.IsTrue(Contract.NullableToString());
    }
}
