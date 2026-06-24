// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_PackExpressionEvaluationOrder.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_PackExpressionEvaluationOrder
{
    [TestMethod]
    public void PackExpressionsEvaluateElementsLeftToRight()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private static int _trace;
                public static event System.Action<int, int, int> OnValues;

                [DisplayName("arrayInitializer")]
                public static int ArrayInitializer()
                {
                    _trace = 0;
                    int[] values = new int[] { Next(1), Next(2), Next(3) };
                    return _trace;
                }

                [DisplayName("multiDimensionalArrayInitializer")]
                public static int MultiDimensionalArrayInitializer()
                {
                    _trace = 0;
                    int[,] values = new int[,] { { Next(1), Next(2) }, { Next(3), Next(4) } };
                    return _trace;
                }

                [DisplayName("tupleExpression")]
                public static int TupleExpression()
                {
                    _trace = 0;
                    var value = (Next(1), Next(2), Next(3));
                    return _trace;
                }

                [DisplayName("anonymousObject")]
                public static int AnonymousObject()
                {
                    _trace = 0;
                    var value = new { First = Next(1), Second = Next(2), Third = Next(3) };
                    return _trace;
                }

                [DisplayName("eventInvocation")]
                public static int EventInvocation()
                {
                    _trace = 0;
                    OnValues(Next(1), Next(2), Next(3));
                    return _trace;
                }

                [DisplayName("collectionExpression")]
                public static int CollectionExpression()
                {
                    _trace = 0;
                    int[] values = [Next(1), Next(2), Next(3)];
                    return _trace;
                }

                [DisplayName("objectInitializer")]
                public static int ObjectInitializer()
                {
                    _trace = 0;
                    var value = new Box { A = Next(1), B = Next(2), C = Next(3) };
                    return _trace;
                }

                [DisplayName("objectInitializerOutOfFieldOrder")]
                public static int ObjectInitializerOutOfFieldOrder()
                {
                    _trace = 0;
                    var value = new Box { C = Next(1), A = Next(2) };
                    return _trace * 1000 + value.A * 100 + value.B * 10 + value.C;
                }

                [DisplayName("objectInitializerWithDefaultField")]
                public static int ObjectInitializerWithDefaultField()
                {
                    _trace = 0;
                    var value = new Box { A = Next(1), C = Next(2) };
                    return _trace * 1000 + value.A * 100 + value.B * 10 + value.C;
                }

                [DisplayName("virtualObjectInitializer")]
                public static int VirtualObjectInitializer()
                {
                    _trace = 0;
                    var value = new VirtualBox { A = Next(1) };
                    return _trace * 100 + value.GetA();
                }

                [DisplayName("listInitializer")]
                public static int ListInitializer()
                {
                    _trace = 0;
                    var values = new List<int> { Next(1), Next(2), Next(3) };
                    return _trace;
                }

                private static int Next(int value)
                {
                    _trace = _trace * 10 + value;
                    return value;
                }

                public class Box
                {
                    public int A;
                    public int B;
                    public int C;
                }

                public class VirtualBox
                {
                    public int A;

                    public virtual int GetA()
                    {
                        return A;
                    }
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        var engine = new TestEngine(true);
        var contract = engine.Deploy<PackExpressionEvaluationOrderContract>(
            context.CreateExecutable(),
            context.CreateManifest());

        Assert.AreEqual(new BigInteger(123), contract.ArrayInitializer());
        Assert.AreEqual(new BigInteger(1234), contract.MultiDimensionalArrayInitializer());
        Assert.AreEqual(new BigInteger(123), contract.TupleExpression());
        Assert.AreEqual(new BigInteger(123), contract.AnonymousObject());
        Assert.AreEqual(new BigInteger(123), contract.EventInvocation());
        Assert.AreEqual(new BigInteger(123), contract.CollectionExpression());
        Assert.AreEqual(new BigInteger(123), contract.ObjectInitializer());
        Assert.AreEqual(new BigInteger(12201), contract.ObjectInitializerOutOfFieldOrder());
        Assert.AreEqual(new BigInteger(12102), contract.ObjectInitializerWithDefaultField());
        Assert.AreEqual(new BigInteger(101), contract.VirtualObjectInitializer());
        Assert.AreEqual(new BigInteger(123), contract.ListInitializer());
    }

    public abstract class PackExpressionEvaluationOrderContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("arrayInitializer")]
        public abstract BigInteger? ArrayInitializer();

        [DisplayName("multiDimensionalArrayInitializer")]
        public abstract BigInteger? MultiDimensionalArrayInitializer();

        [DisplayName("tupleExpression")]
        public abstract BigInteger? TupleExpression();

        [DisplayName("anonymousObject")]
        public abstract BigInteger? AnonymousObject();

        [DisplayName("eventInvocation")]
        public abstract BigInteger? EventInvocation();

        [DisplayName("collectionExpression")]
        public abstract BigInteger? CollectionExpression();

        [DisplayName("objectInitializer")]
        public abstract BigInteger? ObjectInitializer();

        [DisplayName("objectInitializerOutOfFieldOrder")]
        public abstract BigInteger? ObjectInitializerOutOfFieldOrder();

        [DisplayName("objectInitializerWithDefaultField")]
        public abstract BigInteger? ObjectInitializerWithDefaultField();

        [DisplayName("virtualObjectInitializer")]
        public abstract BigInteger? VirtualObjectInitializer();

        [DisplayName("listInitializer")]
        public abstract BigInteger? ListInitializer();
    }
}
