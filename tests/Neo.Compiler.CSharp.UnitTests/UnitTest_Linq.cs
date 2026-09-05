// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Linq.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Linq : DebugAndTestBase<Contract_Linq>
    {
        [TestMethod]
        public void Test_AggregateSum()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(new BigInteger(-101), Contract.AggregateSum(array));
            AssertGasConsumed(1225020);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(new BigInteger(5), Contract.AggregateSum(array));
            AssertGasConsumed(1287120);
        }

        [TestMethod]
        public void Test_AllGreaterThanZero()
        {
            var array = new List<object>
            {
                1,
                100
            };
            Assert.IsTrue(Contract.AllGreaterThanZero(array));
            AssertGasConsumed(1203090);
            array.Add(0);
            Assert.IsFalse(Contract.AllGreaterThanZero(array));
            AssertGasConsumed(1223010);
        }

        [TestMethod]
        public void Test_IsEmpty()
        {
            var array = new List<object>();

            Assert.IsTrue(Contract.IsEmpty(array));
            AssertGasConsumed(1083600);

            array.Add(1);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);

            Assert.IsFalse(Contract.IsEmpty(array));
            AssertGasConsumed(1146810);
        }

        [TestMethod]
        public void Test_AnyGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsFalse(Contract.AnyGreaterThanZero(array));
            AssertGasConsumed(1223430);
            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThanZero(array));
            AssertGasConsumed(1243350);
        }

        [TestMethod]
        public void Test_AnyGreaterThan()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsFalse(Contract.AnyGreaterThan(array, 0));
            AssertGasConsumed(1223670);

            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThan(array, 0));
            AssertGasConsumed(1243620);
            Assert.IsFalse(Contract.AnyGreaterThan(array, 100));
            AssertGasConsumed(1244040);
        }

        [TestMethod]
        public void Test_Average()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.Average(array));
            AssertGasConsumed(1099590);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(1, Contract.Average(array));
            AssertGasConsumed(1157100);
            array.Add(3);
            Assert.AreEqual(1, Contract.Average(array));
            AssertGasConsumed(1160970);
        }

        [TestMethod]
        public void Test_AverageTwice()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.AverageTwice(array));
            AssertGasConsumed(1117530);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);
            Assert.AreEqual(2, Contract.AverageTwice(array));
            AssertGasConsumed(1228860);
            array.Add(3);
            Assert.AreEqual(3, Contract.AverageTwice(array));
            AssertGasConsumed(1250670);
        }

        [TestMethod]
        public void Test_Count()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(3, Contract.Count(array));
            AssertGasConsumed(1154130);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(7, Contract.Count(array));
            AssertGasConsumed(1166850);
        }

        [TestMethod]
        public void Test_CountGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(0, Contract.CountGreaterThanZero(array));
            AssertGasConsumed(1223550);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(3, Contract.CountGreaterThanZero(array));
            AssertGasConsumed(1306800);
        }

        [TestMethod]
        public void Test_Contains()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsTrue(Contract.Contains(array, 0));
            AssertGasConsumed(1200690);
            array.Add(1);
            Assert.IsFalse(Contract.Contains(array, 9));
            AssertGasConsumed(1264320);
            Assert.IsTrue(Contract.Contains(array, 1));
            AssertGasConsumed(1263900);
        }

        [TestMethod]
        public void Test_ContainsText()
        {
            var array = new List<object>
            {
                "Hello",
                "AA",
                "bbb"
            };
            Assert.IsTrue(Contract.ContainsText(array, "bbb"));
            AssertGasConsumed(1243650);
            Assert.IsFalse(Contract.ContainsText(array, "c"));
            AssertGasConsumed(1244070);
        }

        [TestMethod]
        public void Test_ContainsPerson()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsFalse(Contract.ContainsPerson(array, 0));
            AssertGasConsumed(8627220);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPerson(array, 1));
            AssertGasConsumed(10556070);
            Assert.IsTrue(Contract.ContainsPersonIndex(array, 0));
            AssertGasConsumed(8835090);
        }

        [TestMethod]
        public void Test_ContainsPersonS()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.IsTrue(Contract.ContainsPersonS(array, 0));
            AssertGasConsumed(8584680);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPersonS(array, 10));
            AssertGasConsumed(10556070);
            Assert.IsTrue(Contract.ContainsPersonS(array, -100));
            AssertGasConsumed(10534590);
        }

        [TestMethod]
        public void Test_FirstGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100,
                1
            };
            Assert.AreEqual(1, Contract.FirstGreaterThanZero(array));
            AssertGasConsumed(1243860);

            array.Clear();
            array.Add(2);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            Assert.AreEqual(2, Contract.FirstGreaterThanZero(array));
            AssertGasConsumed(1182930);
        }

        [TestMethod]
        public void Test_SelectTwice()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = (Array)Contract.SelectTwice(array)!;
            AssertGasConsumed(1962090);
            Assert.AreEqual(3, result.Count);

            array.Add(5);
            result = (Array)Contract.SelectTwice(array)!;
            AssertGasConsumed(2228460);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(-2, result[1]);
            Assert.AreEqual(-200, result[2]);
            Assert.AreEqual(10, result[3]);
        }

        [TestMethod]
        public void Test_SelectPersonS()
        {
            var array = new List<object>
            {
                new BigInteger(0),
                new BigInteger(-1),
                new BigInteger(-100),
                new BigInteger(5)
            };
            var result = (Array)Contract.SelectPersonS(array)!;
            AssertGasConsumed(12087810);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(array[0], ((Struct)result[0])[1].GetInteger());
            Assert.AreEqual(array[1], ((Struct)result[1])[1].GetInteger());
            Assert.AreEqual(array[2], ((Struct)result[2])[1].GetInteger());
            Assert.AreEqual(array[3], ((Struct)result[3])[1].GetInteger());
        }

        [TestMethod]
        public void Test_Skip()
        {
            var array = new List<object>
            {
                new BigInteger(0),
                new BigInteger(-1),
                new BigInteger(-100)
            };
            var result = (Array)Contract.Skip(array, 0)!;
            AssertGasConsumed(1891650);
            Assert.AreEqual(3, result.Count);

            array.Add(new BigInteger(1));
            array.Add(new BigInteger(5));
            array.Add(new BigInteger(100));

            result = (Array)Contract.Skip(array, 2)!;
            AssertGasConsumed(2147730);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(-100, result[0]);
            Assert.AreEqual(100, result[3].GetInteger());
            Assert.AreEqual(100, result[3].GetInteger());
        }

        [TestMethod]
        public void Test_Sum()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            Assert.AreEqual(-101, Contract.Sum(array));
            AssertGasConsumed(1154310);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(5, Contract.Sum(array));
            AssertGasConsumed(1164030);
            Assert.AreEqual(10, Contract.SumTwice(array));
            AssertGasConsumed(1289610);
        }

        [TestMethod]
        public void Test_Take()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = (Array)Contract.Take(array, 0)!;
            AssertGasConsumed(1147830);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)Contract.Take(array, 2)!;
            AssertGasConsumed(1646760);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0]);
        }

        [TestMethod]
        public void Test_ToMap()
        {
            var array = new List<object>
            {
                new BigInteger(0),
                new BigInteger(-1),
                new BigInteger(-100),
                new BigInteger(5)
            };
            var result = (Map)Contract.ToMap(array)!;
            AssertGasConsumed(10077300);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(array[0], ((Struct)result[array[0]!.ToString()!])[1].GetInteger());
            Assert.AreEqual(array[1], ((Struct)result[array[1]!.ToString()!])[1].GetInteger());
            Assert.AreEqual(array[2], ((Struct)result[array[2]!.ToString()!])[1].GetInteger());
            Assert.AreEqual(array[3], ((Struct)result[array[3]!.ToString()!])[1].GetInteger());
        }

        [TestMethod]
        public void Test_WhereGreaterThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = (Array)Contract.WhereGreaterThanZero(array)!;
            AssertGasConsumed(1224000);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)Contract.WhereGreaterThanZero(array)!;
            AssertGasConsumed(2043000);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(100, result[1]);
            Assert.AreEqual(56, result[2]);
        }

        [TestMethod]
        public void Test_GenericLinqRegression()
        {
            var array = new List<object> { -5, 0, 4, 7 };

            Assert.IsTrue(Contract.AnyGreaterThanZero(array));
            Assert.IsFalse(Contract.AnyGreaterThan(array, 10));

            var positives = (Array)Contract.WhereGreaterThanZero(array)!;
            Assert.AreEqual(2, positives.Count);

            var aggregate = Contract.AggregateSum(array);
            Assert.IsNotNull(aggregate);
            Assert.AreEqual(new BigInteger(6), aggregate);
        }
    }
}
