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
            Assert.AreEqual(1226940, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(new BigInteger(5), Contract.AggregateSum(array));
            Assert.AreEqual(1289490, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1205250, Engine.FeeConsumed.Value);
            array.Add(0);
            Assert.IsFalse(Contract.AllGreaterThanZero(array));
            Assert.AreEqual(1225290, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_IsEmpty()
        {
            var array = new List<object>();

            Assert.IsTrue(Contract.IsEmpty(array));
            Assert.AreEqual(1084650, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);

            Assert.IsFalse(Contract.IsEmpty(array));
            Assert.AreEqual(1147860, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1225350, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThanZero(array));
            Assert.AreEqual(1245270, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1225590, Engine.FeeConsumed.Value);

            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThan(array, 0));
            Assert.AreEqual(1245540, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.AnyGreaterThan(array, 100));
            Assert.AreEqual(1245960, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Average()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.Average(array));
            Assert.AreEqual(1101270, Engine.FeeConsumed.Value);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(1, Contract.Average(array));
            Assert.AreEqual(1159290, Engine.FeeConsumed.Value);
            array.Add(3);
            Assert.AreEqual(1, Contract.Average(array));
            Assert.AreEqual(1163340, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_AverageTwice()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.AverageTwice(array));
            Assert.AreEqual(1120080, Engine.FeeConsumed.Value);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);
            Assert.AreEqual(2, Contract.AverageTwice(array));
            Assert.AreEqual(1232010, Engine.FeeConsumed.Value);
            array.Add(3);
            Assert.AreEqual(3, Contract.AverageTwice(array));
            Assert.AreEqual(1254030, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1155270, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(7, Contract.Count(array));
            Assert.AreEqual(1168110, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1225470, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(3, Contract.CountGreaterThanZero(array));
            Assert.AreEqual(1308810, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1202670, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsFalse(Contract.Contains(array, 9));
            Assert.AreEqual(1266300, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.Contains(array, 1));
            Assert.AreEqual(1265880, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1245630, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.ContainsText(array, "c"));
            Assert.AreEqual(1246050, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(9682080, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPerson(array, 1));
            Assert.AreEqual(11874150, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.ContainsPersonIndex(array, 0));
            Assert.AreEqual(9889950, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(10378620, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPersonS(array, 10));
            Assert.AreEqual(12798000, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.ContainsPersonS(array, -100));
            Assert.AreEqual(12776520, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1245330, Engine.FeeConsumed.Value);

            array.Clear();
            array.Add(2);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            Assert.AreEqual(2, Contract.FirstGreaterThanZero(array));
            Assert.AreEqual(1184400, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1964100, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, result.Count);

            array.Add(5);
            result = (Array)Contract.SelectTwice(array)!;
            Assert.AreEqual(2230500, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(14934570, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1892640, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, result.Count);

            array.Add(new BigInteger(1));
            array.Add(new BigInteger(5));
            array.Add(new BigInteger(100));

            result = (Array)Contract.Skip(array, 2)!;
            Assert.AreEqual(2148780, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1155810, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(5, Contract.Sum(array));
            Assert.AreEqual(1165980, Engine.FeeConsumed.Value);
            Assert.AreEqual(10, Contract.SumTwice(array));
            Assert.AreEqual(1292610, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1148820, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)Contract.Take(array, 2)!;
            Assert.AreEqual(1647810, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(11873760, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1225920, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)Contract.WhereGreaterThanZero(array)!;
            Assert.AreEqual(2044920, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(100, result[1]);
            Assert.AreEqual(56, result[2]);
        }
    }
}
