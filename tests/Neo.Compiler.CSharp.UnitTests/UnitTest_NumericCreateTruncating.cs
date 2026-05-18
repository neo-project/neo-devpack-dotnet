using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NumericCreateTruncating
{
    [TestMethod]
    public void NumericCreateTruncating_ReturnsWrappedIntegralValues()
    {
        var contract = DeployContract();

        Assert.AreEqual(new BigInteger(byte.CreateTruncating(-1)), contract.ByteFromInt(-1));
        Assert.AreEqual(new BigInteger(byte.CreateTruncating(256)), contract.ByteFromInt(256));
        Assert.AreEqual(new BigInteger(sbyte.CreateTruncating(255)), contract.SByteFromInt(255));
        Assert.AreEqual(new BigInteger(uint.CreateTruncating(-1)), contract.UIntFromInt(-1));
        Assert.AreEqual(new BigInteger(ulong.CreateTruncating(-1L)), contract.ULongFromLong(-1));
        Assert.AreEqual(new BigInteger(int.CreateTruncating(new BigInteger(4294967295UL))), contract.IntFromBigInteger(new BigInteger(4294967295UL)));
        Assert.AreEqual(BigInteger.CreateTruncating(long.MinValue), contract.BigIntegerFromLong(long.MinValue));
    }

    private static NumericCreateTruncatingContract DeployContract()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System.ComponentModel;
            using System.Numerics;

            public class Contract : SmartContract
            {
                [DisplayName("byteFromInt")]
                public static byte ByteFromInt(int value) => byte.CreateTruncating(value);

                [DisplayName("sByteFromInt")]
                public static sbyte SByteFromInt(int value) => sbyte.CreateTruncating(value);

                [DisplayName("uIntFromInt")]
                public static uint UIntFromInt(int value) => uint.CreateTruncating(value);

                [DisplayName("uLongFromLong")]
                public static ulong ULongFromLong(long value) => ulong.CreateTruncating(value);

                [DisplayName("intFromBigInteger")]
                public static int IntFromBigInteger(BigInteger value) => int.CreateTruncating(value);

                [DisplayName("bigIntegerFromLong")]
                public static BigInteger BigIntegerFromLong(long value) => BigInteger.CreateTruncating(value);
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<NumericCreateTruncatingContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class NumericCreateTruncatingContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("byteFromInt")]
        public abstract BigInteger? ByteFromInt(BigInteger? value);

        [DisplayName("sByteFromInt")]
        public abstract BigInteger? SByteFromInt(BigInteger? value);

        [DisplayName("uIntFromInt")]
        public abstract BigInteger? UIntFromInt(BigInteger? value);

        [DisplayName("uLongFromLong")]
        public abstract BigInteger? ULongFromLong(BigInteger? value);

        [DisplayName("intFromBigInteger")]
        public abstract BigInteger? IntFromBigInteger(BigInteger? value);

        [DisplayName("bigIntegerFromLong")]
        public abstract BigInteger? BigIntegerFromLong(BigInteger? value);
    }
}
