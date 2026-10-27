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
}
