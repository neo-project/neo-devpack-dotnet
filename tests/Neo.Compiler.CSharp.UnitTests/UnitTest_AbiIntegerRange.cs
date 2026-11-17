// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_AbiIntegerRange.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_AbiIntegerRange
{
    public abstract class RangeContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("isNeg")] public abstract bool? IsNeg(BigInteger value);
        [DisplayName("echo")] public abstract BigInteger? Echo(BigInteger value);
        [DisplayName("isBigByte")] public abstract bool? IsBigByte(BigInteger value);
        [DisplayName("identity")] public abstract BigInteger? Identity(BigInteger value);
    }

    [TestMethod]
    public void ExportedNarrowIntegersRejectOutOfRangeArguments()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;
            using System.Numerics;

            public class Contract : SmartContract
            {
                [DisplayName("isNeg")] public static bool IsNeg(uint value) => value < 0;
                [DisplayName("echo")] public static ushort Echo(ushort value) => value;
                [DisplayName("isBigByte")] public static bool IsBigByte(byte value) => value > 255;
                [DisplayName("identity")] public static BigInteger Identity(BigInteger value) => value;
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var contract = new TestEngine(true).Deploy<RangeContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.IsFalse(contract.IsNeg(5)!.Value);
        Assert.AreEqual(new BigInteger(42), contract.Echo(42));
        Assert.IsFalse(contract.IsBigByte(200)!.Value);
        Assert.ThrowsExactly<TestException>(() => contract.IsNeg(-1));
        Assert.ThrowsExactly<TestException>(() => contract.Echo(-1));
        Assert.ThrowsExactly<TestException>(() => contract.Echo(65536));
        Assert.ThrowsExactly<TestException>(() => contract.IsBigByte(1000));
        Assert.AreEqual(new BigInteger(65536), contract.Identity(65536));
    }
}
