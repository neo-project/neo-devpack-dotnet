using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_OutAlias
{
    [TestMethod]
    public void SequentialOutByteArrayDeclarationsRemainDistinct()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.Numerics;

public class Contract : SmartContract
{
    private static bool TryEncodeBigIntegerScalar(BigInteger value, out byte[] scalar)
    {
        scalar = new byte[] { (byte)value };
        return true;
    }

    public static int Main(BigInteger amountWithdraw, BigInteger fee)
    {
        if (!TryEncodeBigIntegerScalar(amountWithdraw, out byte[] amountWithdrawScalar))
            return -1;
        if (!TryEncodeBigIntegerScalar(fee, out byte[] feeScalar))
            return -2;
        return amountWithdrawScalar[0] * 256 + feeScalar[0];
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var contract = new TestEngine(true).Deploy<OutAliasContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(new BigInteger(1287), contract.Main(5, 7));
    }

    public abstract class OutAliasContract(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("main")]
        public abstract BigInteger? Main(BigInteger amountWithdraw, BigInteger fee);
    }
}
