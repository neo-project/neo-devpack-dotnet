using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_LiftedNullableUnary
{
    [TestMethod]
    public void LiftedUnaryOperatorsPropagateNullAndPreserveValues()
    {
        const string source = """
using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    private static int? _value;

    [DisplayName("not")]
    public static bool? Not(bool? value) => !value;

    [DisplayName("negate")]
    public static int? Negate(int? value) => -value;

    [DisplayName("complement")]
    public static int? Complement(int? value) => ~value;

    [DisplayName("preIncrement")]
    public static int? PreIncrement(int? value) => ++value;

    [DisplayName("preDecrement")]
    public static int? PreDecrement(int? value) => --value;

    [DisplayName("postIncrement")]
    public static int? PostIncrement(int? value) => value++;

    [DisplayName("postIncrementStored")]
    public static int? PostIncrementStored(int? value)
    {
        value++;
        return value;
    }

    [DisplayName("preIncrementField")]
    public static int? PreIncrementField(int? value)
    {
        _value = value;
        return ++_value;
    }

    [DisplayName("postIncrementArrayStored")]
    public static int? PostIncrementArrayStored(int? value)
    {
        int?[] values = [value];
        values[0]++;
        return values[0];
    }
}
""";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<NullableUnaryContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsNull(contract.Not(null));
        Assert.IsNull(contract.Negate(null));
        Assert.IsNull(contract.Complement(null));
        Assert.IsNull(contract.PreIncrement(null));
        Assert.IsNull(contract.PreDecrement(null));
        Assert.IsNull(contract.PostIncrement(null));
        Assert.IsNull(contract.PostIncrementStored(null));
        Assert.IsNull(contract.PreIncrementField(null));
        Assert.IsNull(contract.PostIncrementArrayStored(null));

        Assert.AreEqual(false, contract.Not(true));
        Assert.AreEqual(new BigInteger(-5), contract.Negate(5));
        Assert.AreEqual(new BigInteger(~5), contract.Complement(5));
        Assert.AreEqual(new BigInteger(6), contract.PreIncrement(5));
        Assert.AreEqual(new BigInteger(4), contract.PreDecrement(5));
        Assert.AreEqual(new BigInteger(5), contract.PostIncrement(5));
        Assert.AreEqual(new BigInteger(6), contract.PostIncrementStored(5));
        Assert.AreEqual(new BigInteger(6), contract.PreIncrementField(5));
        Assert.AreEqual(new BigInteger(6), contract.PostIncrementArrayStored(5));
    }

    public abstract class NullableUnaryContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("not")]
        public abstract bool? Not(bool? value);

        [DisplayName("negate")]
        public abstract BigInteger? Negate(BigInteger? value);

        [DisplayName("complement")]
        public abstract BigInteger? Complement(BigInteger? value);

        [DisplayName("preIncrement")]
        public abstract BigInteger? PreIncrement(BigInteger? value);

        [DisplayName("preDecrement")]
        public abstract BigInteger? PreDecrement(BigInteger? value);

        [DisplayName("postIncrement")]
        public abstract BigInteger? PostIncrement(BigInteger? value);

        [DisplayName("postIncrementStored")]
        public abstract BigInteger? PostIncrementStored(BigInteger? value);

        [DisplayName("preIncrementField")]
        public abstract BigInteger? PreIncrementField(BigInteger? value);

        [DisplayName("postIncrementArrayStored")]
        public abstract BigInteger? PostIncrementArrayStored(BigInteger? value);
    }
}
