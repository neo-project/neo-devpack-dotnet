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

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(new BigInteger(5), Contract.AggregateSum(array));
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
            array.Add(0);
            Assert.IsFalse(Contract.AllGreaterThanZero(array));
        }

        [TestMethod]
        public void Test_IsEmpty()
        {
            var array = new List<object>();

            Assert.IsTrue(Contract.IsEmpty(array));

            array.Add(1);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);

            Assert.IsFalse(Contract.IsEmpty(array));
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
            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThanZero(array));
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

            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThan(array, 0));
            Assert.IsFalse(Contract.AnyGreaterThan(array, 100));
        }

        [TestMethod]
        public void Test_Average()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsExactly<TestException>(() => Contract.Average(array));
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(1, Contract.Average(array));
            array.Add(3);
            Assert.AreEqual(1, Contract.Average(array));

            array = new List<object>() { int.MaxValue, int.MaxValue };
            Assert.AreEqual(int.MaxValue, Contract.Average(array));

            array = new List<object>() { (long)int.MaxValue + 2, int.MaxValue };
            Assert.ThrowsExactly<TestException>(() => Contract.Average(array));
        }

        [TestMethod]
        public void Test_AverageLong()
        {
            var array = new List<object>();
            var exception = Assert.ThrowsExactly<TestException>(() => Contract.AverageLong(array));
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(1, Contract.AverageLong(array));

            array.Add(3);
            Assert.AreEqual(1, Contract.AverageLong(array));

            array = new List<object>() { long.MaxValue, long.MaxValue };
            Assert.AreEqual(long.MaxValue, Contract.AverageLong(array));

            array = new List<object>() { (BigInteger)long.MaxValue + 2, long.MaxValue };
            Assert.ThrowsExactly<TestException>(() => Contract.AverageLong(array));
        }

        [TestMethod]
        public void Test_AverageTwice()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsExactly<TestException>(() => Contract.AverageTwice(array));
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);
            Assert.AreEqual(2, Contract.AverageTwice(array));

            array.Add(3);
            Assert.AreEqual(3, Contract.AverageTwice(array));
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

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(7, Contract.Count(array));
        }

        [TestMethod]
        public void Test_CountArrayOverload()
        {
            var expected = new Dictionary<int, long>
            {
                [0] = 1082940,
                [1] = 1143960,
                [2] = 1143990,
                [5] = 1144080,
                [10] = 1144230,
                [50] = 1145430,
                [100] = 1146930
            };
            foreach (var (n, gas) in expected)
            {
                var list = new List<object>();
                for (int i = 0; i < n; i++) list.Add(i);
                Assert.AreEqual(n, Contract.Count(list));
                AssertGasConsumed(gas);
            }
        }

        [TestMethod]
        public void Test_CountEnumerableStillIterates()
        {
            var expected = new Dictionary<int, long>
            {
                [0] = 1083990,
                [1] = 1147890,
                [5] = 1159530,
                [100] = 1435980
            };
            foreach (var (n, gas) in expected)
            {
                var list = new List<object>();
                for (int i = 0; i < n; i++) list.Add(i);
                Assert.AreEqual(n, Contract.CountEnumerable(list));
                AssertGasConsumed(gas);
            }
        }

        [TestMethod]
        public void Test_CountArrayElementTypes()
        {
            Assert.AreEqual(3, Contract.CountStrings(new List<object> { "a", "b", "c" }));
            Assert.AreEqual(3, Contract.CountBigIntegers(new List<object> { 1, 2, 3 }));
            Assert.AreEqual(2, Contract.CountStructs(new List<object> { new List<object> { "a", 1 }, new List<object> { "b", 2 } }));
            Assert.AreEqual(3, Contract.CountNullable(new List<object> { "a", null!, "c" }));
            Assert.AreEqual(4, Contract.CountGeneric(new List<object> { 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void Test_CountNullArray()
        {
            Assert.Throws<TestException>(() => Contract.CountNull());
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

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(3, Contract.CountGreaterThanZero(array));
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
            array.Add(1);
            Assert.IsFalse(Contract.Contains(array, 9));
            Assert.IsTrue(Contract.Contains(array, 1));
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
            Assert.IsFalse(Contract.ContainsText(array, "c"));
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
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPerson(array, 1));
            Assert.IsTrue(Contract.ContainsPersonIndex(array, 0));
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
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPersonS(array, 10));
            Assert.IsTrue(Contract.ContainsPersonS(array, -100));
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

            array.Clear();
            array.Add(2);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            Assert.AreEqual(2, Contract.FirstGreaterThanZero(array));
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
            Assert.AreEqual(3, result.Count);

            array.Add(5);
            result = (Array)Contract.SelectTwice(array)!;
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
            Assert.AreEqual(3, result.Count);

            array.Add(new BigInteger(1));
            array.Add(new BigInteger(5));
            array.Add(new BigInteger(100));

            result = (Array)Contract.Skip(array, 2)!;
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

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(5, Contract.Sum(array));

            Assert.AreEqual(10, Contract.SumTwice(array));

            array = new List<object> { int.MaxValue / 2, int.MaxValue / 2 + 1 };
            Assert.AreEqual(int.MaxValue, Contract.Sum(array));

            array = new List<object> { int.MinValue / 2, int.MinValue / 2 };
            Assert.AreEqual(int.MinValue, Contract.Sum(array));

            array = new List<object> { int.MaxValue, int.MaxValue };
            Assert.ThrowsExactly<TestException>(() => Contract.Sum(array));

            array = new List<object> { int.MinValue, int.MinValue };
            Assert.ThrowsExactly<TestException>(() => Contract.Sum(array));
        }

        [TestMethod]
        public void Test_SumLong()
        {
            var array = new List<object> { 0, -1, -100 };
            Assert.AreEqual(-101, Contract.SumLong(array));

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(5, Contract.SumLong(array));

            array = new List<object> { long.MaxValue / 2, long.MaxValue / 2 + 1 };
            Assert.AreEqual(long.MaxValue, Contract.SumLong(array));

            array = new List<object> { long.MinValue / 2, long.MinValue / 2 };
            Assert.AreEqual(long.MinValue, Contract.SumLong(array));

            array = new List<object> { long.MaxValue, long.MaxValue };
            Assert.ThrowsExactly<TestException>(() => Contract.SumLong(array));

            array = new List<object> { long.MinValue, long.MinValue };
            Assert.ThrowsExactly<TestException>(() => Contract.SumLong(array));
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
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)Contract.Take(array, 2)!;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(-1, result[1]);
        }

        [TestMethod]
        public void Test_Take_Boundaries()
        {
            var empty = new List<object>();
            foreach (int count in new[] { int.MinValue, -1, 0, 1, 3, int.MaxValue })
                Assert.AreEqual(0, ((Array)Contract.Take(empty, count)!).Count);

            var array = new List<object> { 7, -1, 7, 0, -100, 5 };
            foreach (int count in new[] { int.MinValue, -1, 0, 1, 2, 5, 6, 10, int.MaxValue })
            {
                var result = (Array)Contract.Take(array, count)!;
                int expectedCount = count <= 0 ? 0 : count < array.Count ? count : array.Count;
                Assert.AreEqual(expectedCount, result.Count);
                for (int i = 0; i < expectedCount; i++)
                    Assert.AreEqual(new BigInteger((int)array[i]), result[i].GetInteger());
            }
        }

        [TestMethod]
        public void Test_Take_NullSource()
        {
            foreach (int count in new[] { -1, 0, 1 })
            {
                var exception = Assert.ThrowsExactly<TestException>(() => Contract.Take(null, count));
                Assert.AreEqual("An unhandled exception was thrown. source is null", exception.InnerException?.Message);
            }
        }

        [TestMethod]
        public void Test_Take_BigIntegerCounts()
        {
            var array = new List<object> { 7, -1, 7, 0, -100, 5 };
            BigInteger[] counts = [-(BigInteger.One << 200), (BigInteger)int.MinValue - 1,
                -1, 0, 1, 2, 6, 10, (BigInteger)int.MaxValue + 1, BigInteger.One << 200];
            foreach (var count in counts)
            {
                Assert.AreEqual(0, ((Array)Contract.TakeBigInteger(new List<object>(), count)!).Count);
                var result = (Array)Contract.TakeBigInteger(array, count)!;
                int expectedCount = count <= 0 ? 0 : count < array.Count ? (int)count : array.Count;
                Assert.AreEqual(expectedCount, result.Count);
                for (int i = 0; i < expectedCount; i++)
                    Assert.AreEqual(new BigInteger((int)array[i]), result[i].GetInteger());

                var exception = Assert.ThrowsExactly<TestException>(() => Contract.TakeBigInteger(null, count));
                Assert.AreEqual("An unhandled exception was thrown. source is null", exception.InnerException?.Message);
            }
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
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)Contract.WhereGreaterThanZero(array)!;
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
