// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_EnumParseCatch.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_EnumParseCatch
{
    [TestMethod]
    public void FailedParse_DoesNotLeaveTheEnumTypeOnTheEvaluationStack()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using System;
using System.ComponentModel;

public class Contract : SmartContract
{
    private enum TestEnum
    {
        Value1 = 1
    }

    [DisplayName("parseInvalid")]
    public static int ParseInvalid()
    {
        try
        {
            _ = Enum.Parse(typeof(TestEnum), "Invalid");
        }
        catch (Exception)
        {
            return 42;
        }

        return 0;
    }
}
""");
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(item => item.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<EnumParseCatchContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(42), contract.ParseInvalid());
    }

    public abstract class EnumParseCatchContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("parseInvalid")]
        public abstract BigInteger? ParseInvalid();
    }
}
