using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_Stack : DebugAndTestBase<Contract_Stack>
{
    [TestMethod]
    public void Test_Push()
    {
        // Original tests
        TestPushValue(sbyte.MinValue);
        TestPushValue(sbyte.MaxValue);
        TestPushValue(byte.MaxValue);
        TestPushValue(byte.MinValue);
        TestPushValue(short.MaxValue);
        TestPushValue(short.MinValue);
        TestPushValue(ushort.MaxValue);
        TestPushValue(uint.MaxValue);
        TestPushValue(int.MaxValue);
        TestPushValue(int.MinValue);
        TestPushValue(ulong.MaxValue);
        TestPushValue(ulong.MinValue);
        TestPushValue(long.MaxValue);
        TestPushValue(long.MinValue);

        // Edge cases
        TestPushValue(-1);
        TestPushValue(0);
        TestPushValue(16);
        TestPushValue(17);
        TestPushValue(-128);
        TestPushValue(127);
        TestPushValue(-129);
        TestPushValue(128);
        TestPushValue(-32768);
        TestPushValue(32767);
        TestPushValue(-32769);
        TestPushValue(32768);
        TestPushValue(-2147483648);
        TestPushValue(2147483647);
        TestPushValue(BigInteger.Parse("-2147483649"));
        TestPushValue(BigInteger.Parse("2147483648"));
        TestPushValue(BigInteger.Parse("9223372036854775807"));
        TestPushValue(BigInteger.Parse("-9223372036854775808"));
        TestPushValue(BigInteger.Parse("9223372036854775808"));
        TestPushValue(BigInteger.Parse("-9223372036854775809"));
        TestPushValue(BigInteger.Pow(2, 127) - 1);
        TestPushValue(-BigInteger.Pow(2, 127));
        TestPushValue(BigInteger.Pow(2, 127));
        TestPushValue(-BigInteger.Pow(2, 127) - 1);
        TestPushValue(BigInteger.Pow(2, 255) - 1);
        TestPushValue(-BigInteger.Pow(2, 255));
        TestPushValue(255);
        TestPushValue(256);
        TestPushValue(-127);
        TestPushValue(-128);
        TestPushValue(-129);
        TestPushValue(251);
        TestPushValue(257);
        TestPushValue(65535);
        TestPushValue(4294967295);
    }

    private void TestPushValue(BigInteger value)
    {
        var result = Contract.Test_Push_Integer(value);
        Assert.AreEqual(value, result, $"Failed for value: {value}");
    }

    [TestMethod]
    public void Test_Push_Internal()
    {
        var result = Contract.Test_Push_Integer_Internal();
        Assert.IsNotNull(result);
        Assert.AreEqual(new BigInteger(byte.MinValue), result[0]);
        Assert.AreEqual(new BigInteger(byte.MaxValue), result[1]);
        Assert.AreEqual(new BigInteger(sbyte.MinValue), result[2]);
        Assert.AreEqual(new BigInteger(sbyte.MaxValue), result[3]);
        Assert.AreEqual(new BigInteger(short.MinValue), result[4]);
        Assert.AreEqual(new BigInteger(short.MaxValue), result[5]);
        Assert.AreEqual(new BigInteger(ushort.MaxValue), result[6]);
        Assert.AreEqual(new BigInteger(uint.MaxValue), result[7]);
        Assert.AreEqual(new BigInteger(int.MinValue), result[8]);
        Assert.AreEqual(new BigInteger(int.MaxValue), result[9]);
        Assert.AreEqual(new BigInteger(ulong.MaxValue), result[10]);
        Assert.AreEqual(new BigInteger(long.MinValue), result[11]);
        Assert.AreEqual(new BigInteger(long.MaxValue), result[12]);
    }

    [TestMethod]
    public void Test_External()
    {
        var result = Contract.Test_External();
        Assert.IsNotNull(result);
        Assert.AreEqual(BigInteger.MinusOne, result[0]);
        Assert.AreEqual(new BigInteger(-1000000), result[1]); // Previous error value 15777216
        Assert.AreEqual(new BigInteger(-1000000000000), result[2]); // previous error value 280474976710656
    }
}
