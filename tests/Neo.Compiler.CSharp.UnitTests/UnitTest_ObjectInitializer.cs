// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ObjectInitializer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ObjectInitializer : DebugAndTestBase<Contract_ObjectInitializer>
    {
        [TestMethod]
        public void Test_IndexInitializer()
        {
            Assert.AreEqual(new BigInteger(78), Contract.IndexInitializer(7));
        }

        [TestMethod]
        public void Test_FromEndIndexInitializer()
        {
            Assert.AreEqual(new BigInteger(71234), Contract.FromEndIndexInitializer(7));
        }

        [TestMethod]
        public void Test_PropertyInitializer()
        {
            Assert.AreEqual(new BigInteger(7), Contract.PropertyInitializer(7));
        }

        [TestMethod]
        public void Test_FieldInitializer()
        {
            Assert.AreEqual(new BigInteger(7), Contract.FieldInitializer(7));
        }

        [TestMethod]
        [DataRow(CompilationOptions.OptimizationType.None)]
        [DataRow(CompilationOptions.OptimizationType.Basic)]
        [DataRow(CompilationOptions.OptimizationType.All)]
        public void Test_PartialObjectInitializer_PreservesDeclaredInitializers(CompilationOptions.OptimizationType optimization)
        {
            const string source = """
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Attributes;
                using System.ComponentModel;
                using System.Numerics;

                public class Contract : SmartContract
                {
                    public class Position
                    {
                        public BigInteger Limit = 1000;
                        public bool Active { get; set; } = true;
                        [InitialValue("500", ContractParameterType.Integer)]
                        public BigInteger Fee;
                        public BigInteger Deposit;
                    }

                    public record RecordPosition
                    {
                        public BigInteger LockBlocks = 720;
                        public BigInteger Deposit;
                    }

                    private static BigInteger Counter;

                    public class DynamicPosition
                    {
                        public BigInteger Limit = NextLimit();
                    }

                    public class AttributePosition
                    {
                        [InitialValue("500", ContractParameterType.Integer)]
                        public BigInteger Fee;
                        public BigInteger Deposit;
                    }

                    private static BigInteger NextLimit() => ++Counter;

                    [DisplayName("positionDefault")]
                    public static BigInteger PositionDefault() => Value(new Position());

                    [DisplayName("positionPartial")]
                    public static BigInteger PositionPartial(BigInteger deposit)
                        => Value(new Position { Deposit = deposit });

                    [DisplayName("positionOverride")]
                    public static BigInteger PositionOverride(BigInteger deposit)
                        => Value(new Position { Limit = 2000, Deposit = deposit });

                    [DisplayName("positionAllOverride")]
                    public static BigInteger PositionAllOverride(BigInteger deposit)
                        => Value(new Position { Limit = 2000, Active = false, Fee = 8, Deposit = deposit });

                    [DisplayName("dynamicOverride")]
                    public static BigInteger DynamicOverride(BigInteger limit)
                    {
                        var position = new DynamicPosition { Limit = limit };
                        return Counter * 1000 + position.Limit;
                    }

                    [DisplayName("attributePartial")]
                    public static BigInteger AttributePartial(BigInteger deposit)
                    {
                        var position = new AttributePosition { Deposit = deposit };
                        return position.Fee + position.Deposit;
                    }

                    [DisplayName("attributeOverride")]
                    public static BigInteger AttributeOverride(BigInteger deposit)
                    {
                        var position = new AttributePosition { Fee = 8, Deposit = deposit };
                        return position.Fee + position.Deposit;
                    }

                    [DisplayName("recordDefault")]
                    public static BigInteger RecordDefault() => new RecordPosition().LockBlocks * 1000;

                    [DisplayName("recordPartial")]
                    public static BigInteger RecordPartial(BigInteger deposit)
                        => new RecordPosition { Deposit = deposit }.LockBlocks * 1000 + deposit;

                    private static BigInteger Value(Position position)
                        => position.Limit * 100000 + (position.Active ? 10000 : 0)
                           + position.Fee * 10 + position.Deposit;
                }
                """;

            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(source, options);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString())));

            var contract = new TestEngine(true).Deploy<PartialObjectInitializerContract>(
                context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(100015000), contract.PositionDefault());
            Assert.AreEqual(new BigInteger(100015250), contract.PositionPartial(250));
            Assert.AreEqual(new BigInteger(200015250), contract.PositionOverride(250));
            Assert.AreEqual(new BigInteger(200000330), contract.PositionAllOverride(250));
            Assert.AreEqual(new BigInteger(1250), contract.DynamicOverride(250));
            Assert.AreEqual(new BigInteger(750), contract.AttributePartial(250));
            Assert.AreEqual(new BigInteger(258), contract.AttributeOverride(250));
            Assert.AreEqual(new BigInteger(720000), contract.RecordDefault());
            Assert.AreEqual(new BigInteger(720250), contract.RecordPartial(250));
        }

        [TestMethod]
        public void Test_MultiDimIndexInitializer_FailsWithDiagnostic()
        {
            // A multi-dimensional indexer in an initializer has no lowering; it must be
            // rejected with the multidimensional diagnostic instead of miscompiling.
            Syntax.Helper.AssertClassCompilationFails(@"
public class Grid
{
    private int[] _data = new int[8];
    public int this[int i, int j]
    {
        get { return _data[i * 2 + j]; }
        set { _data[i * 2 + j] = value; }
    }
}

public static int Probe(int a)
{
    var g = new Grid { [0, 1] = a };
    return g[0, 1];
}", "Multi-dimensional indexer initializers should be rejected with a diagnostic.");
        }

        [TestMethod]
        public void Test_EmbeddedNestedInitializer_FailsWithDiagnostic()
        {
            // An embedded initializer has no type of its own; it must be rejected with a
            // diagnostic instead of the NullReferenceException the converter used to throw.
            Syntax.Helper.AssertClassCompilationFails(@"
public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class Wrapper
{
    public Point Inner { get; set; } = new Point();
}

public static int Probe(int a)
{
    var w = new Wrapper { Inner = { X = a, Y = a + 1 } };
    return w.Inner.X * 100 + w.Inner.Y;
}", "Embedded nested object initializers should be rejected with a diagnostic instead of crashing.");
        }
    }

    public abstract class PartialObjectInitializerContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("positionDefault")]
        public abstract BigInteger? PositionDefault();

        [DisplayName("positionPartial")]
        public abstract BigInteger? PositionPartial(BigInteger? deposit);

        [DisplayName("positionOverride")]
        public abstract BigInteger? PositionOverride(BigInteger? deposit);

        [DisplayName("positionAllOverride")]
        public abstract BigInteger? PositionAllOverride(BigInteger? deposit);

        [DisplayName("dynamicOverride")]
        public abstract BigInteger? DynamicOverride(BigInteger? limit);

        [DisplayName("attributePartial")]
        public abstract BigInteger? AttributePartial(BigInteger? deposit);

        [DisplayName("attributeOverride")]
        public abstract BigInteger? AttributeOverride(BigInteger? deposit);

        [DisplayName("recordDefault")]
        public abstract BigInteger? RecordDefault();

        [DisplayName("recordPartial")]
        public abstract BigInteger? RecordPartial(BigInteger? deposit);
    }
}
