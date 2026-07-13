// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_AssignmentEvaluationOrder.cs file belongs to the neo project and is free
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
public class UnitTest_AssignmentEvaluationOrder
{
    [TestMethod]
    public void SimpleAssignmentEvaluatesTargetBeforeRightHandSide()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _order;
                private static int _staticField;

                private class Holder
                {
                    public int Field;
                    public int Value { get; set; }
                }

                private class IndexerBox
                {
                    private readonly int[] _values = new int[1];

                    public int this[int index]
                    {
                        get => _values[index];
                        set => _values[index] = value;
                    }
                }

                [DisplayName("memberReceiver")]
                public static int MemberReceiver()
                {
                    _order = 0;
                    var holder = new Holder();
                    int result = ObserveReceiver(holder).Value = ObserveValue(7);
                    return _order * 100 + holder.Value * 10 + result;
                }

                [DisplayName("staticField")]
                public static int StaticField()
                {
                    _order = 0;
                    int result = Contract._staticField = ObserveValue(6);
                    return _order * 100 + _staticField * 10 + result;
                }

                [DisplayName("instanceField")]
                public static int InstanceField()
                {
                    _order = 0;
                    var holder = new Holder();
                    int result = ObserveReceiver(holder).Field = ObserveValue(5);
                    return _order * 100 + holder.Field * 10 + result;
                }

                [DisplayName("arrayReceiverAndIndex")]
                public static int ArrayReceiverAndIndex()
                {
                    _order = 0;
                    var array = new int[1];
                    int result = ObserveArray(array)[ObserveIndex()] = ObserveValue(9);
                    return _order * 100 + array[0] * 10 + result;
                }

                [DisplayName("indexerReceiverAndIndex")]
                public static int IndexerReceiverAndIndex()
                {
                    _order = 0;
                    var box = new IndexerBox();
                    int result = ObserveIndexer(box)[ObserveIndex()] = ObserveValue(8);
                    return _order * 100 + box[0] * 10 + result;
                }

                [DisplayName("arrayPostIncrement")]
                public static int ArrayPostIncrement()
                {
                    var array = new int[2];
                    int i = 0;
                    array[i++] = i++;
                    return array[0] * 100 + array[1] * 10 + i;
                }

                private static Holder ObserveReceiver(Holder holder)
                {
                    _order = _order * 10 + 1;
                    return holder;
                }

                private static int[] ObserveArray(int[] array)
                {
                    _order = _order * 10 + 1;
                    return array;
                }

                private static IndexerBox ObserveIndexer(IndexerBox box)
                {
                    _order = _order * 10 + 1;
                    return box;
                }

                private static int ObserveIndex()
                {
                    _order = _order * 10 + 2;
                    return 0;
                }

                private static int ObserveValue(int value)
                {
                    _order = _order * 10 + 3;
                    return value;
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<AssignmentOrderContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(1377), contract.MemberReceiver());
        Assert.AreEqual(new BigInteger(366), contract.StaticField());
        Assert.AreEqual(new BigInteger(1355), contract.InstanceField());
        Assert.AreEqual(new BigInteger(12399), contract.ArrayReceiverAndIndex());
        Assert.AreEqual(new BigInteger(12388), contract.IndexerReceiverAndIndex());
        Assert.AreEqual(new BigInteger(102), contract.ArrayPostIncrement());
    }

    [TestMethod]
    public void UnsupportedMemberAssignmentReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System;

            public class Contract : SmartContract
            {
                public static event Action? Changed;

                public static void Main()
                {
                    Contract.Changed = null;
                }
            }
            """);

        Assert.IsFalse(context.Success);
        StringAssert.Contains(
            string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())),
            "Cannot assign to symbol type");
    }

    public abstract class AssignmentOrderContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("memberReceiver")]
        public abstract BigInteger? MemberReceiver();

        [DisplayName("staticField")]
        public abstract BigInteger? StaticField();

        [DisplayName("instanceField")]
        public abstract BigInteger? InstanceField();

        [DisplayName("arrayReceiverAndIndex")]
        public abstract BigInteger? ArrayReceiverAndIndex();

        [DisplayName("indexerReceiverAndIndex")]
        public abstract BigInteger? IndexerReceiverAndIndex();

        [DisplayName("arrayPostIncrement")]
        public abstract BigInteger? ArrayPostIncrement();
    }
}
