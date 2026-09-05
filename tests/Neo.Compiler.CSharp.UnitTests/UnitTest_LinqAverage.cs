// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LinqAverage.cs file belongs to the neo project and is free
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_LinqAverage
{
    public static IEnumerable<object[]> AverageCases()
    {
        foreach (bool useLong in new[] { false, true })
        {
            long min = useLong ? long.MinValue : int.MinValue;
            long max = useLong ? long.MaxValue : int.MaxValue;
            (long[] Values, long Expected)[] cases =
            [
                ([max, max], max),
                ([min, min], min),
                ([max, max, min], (max - 1) / 3),
                ([min, min, max], (long)(((BigInteger)min - 1) / 3)),
                ([min, max], 0),
                ([1, 2], 1),
                ([-1, -2], -1),
                ([-2, 1], 0),
                ([0], 0),
                ([-10], -10)
            ];
            foreach (bool useSelector in new[] { false, true })
                foreach (var (values, expected) in cases)
                    yield return [useLong, useSelector, values, expected];
        }
    }

    [TestMethod]
    [DynamicData(nameof(AverageCases), DynamicDataSourceType.Method)]
    public void AveragePreservesIntegerRangeAndTruncation(
        bool useLong, bool useSelector, long[] values, long expected)
    {
        string type = useLong ? "long" : "int";
        string selector = useSelector ? "value => value" : "";
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Linq;

            public class Contract : SmartContract
            {
                public static {{type}} Average({{type}}[] values) => values.Average({{selector}});
            }
            """);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        var contract = engine.Deploy<AverageContract>(nef, manifest);
        Assert.AreEqual(new BigInteger(expected), contract.Average(values.Cast<object>().ToArray()));
    }

    public abstract class AverageContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("average")]
        public abstract BigInteger? Average(IList<object> values);
    }
}
