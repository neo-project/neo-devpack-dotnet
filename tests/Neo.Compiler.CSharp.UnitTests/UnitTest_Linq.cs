using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Linq : TestBase<Contract_Linq>
    {
        public UnitTest_Linq() : base(Contract_Linq.Nef, Contract_Linq.Manifest) { }

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
            Assert.AreEqual(1002302740, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(new BigInteger(5), Contract.AggregateSum(array));
            Assert.AreEqual(1003592230, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002281050, Engine.FeeConsumed.Value);
            array.Add(0);
            Assert.IsFalse(Contract.AllGreaterThanZero(array));
            Assert.AreEqual(1003506340, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_IsEmpty()
        {
            var array = new List<object>();

            Assert.IsTrue(Contract.IsEmpty(array));
            Assert.AreEqual(1002160450, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);

            Assert.IsFalse(Contract.IsEmpty(array));
            Assert.AreEqual(1003308310, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002301150, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThanZero(array));
            Assert.AreEqual(1003546420, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002301390, Engine.FeeConsumed.Value);

            array.Add(1);
            Assert.IsTrue(Contract.AnyGreaterThan(array, 0));
            Assert.AreEqual(1003546930, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.AnyGreaterThan(array, 100));
            Assert.AreEqual(1004792890, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_Average()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.Average(array));
            Assert.AreEqual(1002177070, Engine.FeeConsumed.Value);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);

            Assert.AreEqual(1, Contract.Average(array));
            Assert.AreEqual(1003336360, Engine.FeeConsumed.Value);
            array.Add(3);
            Assert.AreEqual(1, Contract.Average(array));
            Assert.AreEqual(1004499700, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_AverageTwice()
        {
            var array = new List<object>();

            var exception = Assert.ThrowsException<TestException>(() => Contract.AverageTwice(array));
            Assert.AreEqual(1002195880, Engine.FeeConsumed.Value);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", exception.InnerException?.Message);

            array.Add(0);
            array.Add(1);
            array.Add(2);
            Assert.AreEqual(2, Contract.AverageTwice(array));
            Assert.AreEqual(1003427890, Engine.FeeConsumed.Value);
            array.Add(3);
            Assert.AreEqual(3, Contract.AverageTwice(array));
            Assert.AreEqual(1004681920, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002231070, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(7, Contract.Count(array));
            Assert.AreEqual(1003399180, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002301270, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            Assert.AreEqual(3, Contract.CountGreaterThanZero(array));
            Assert.AreEqual(1003610080, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002278470, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsFalse(Contract.Contains(array, 9));
            Assert.AreEqual(1003544770, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.Contains(array, 1));
            Assert.AreEqual(1004810650, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002321430, Engine.FeeConsumed.Value);
            Assert.IsFalse(Contract.ContainsText(array, "c"));
            Assert.AreEqual(1003567480, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1010757880, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPerson(array, 1));
            Assert.AreEqual(1022632030, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.ContainsPersonIndex(array, 0));
            Assert.AreEqual(1032521980, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1011454420, Engine.FeeConsumed.Value);
            array.Add(1);
            Assert.IsFalse(Contract.ContainsPersonS(array, 10));
            Assert.AreEqual(1024252420, Engine.FeeConsumed.Value);
            Assert.IsTrue(Contract.ContainsPersonS(array, -100));
            Assert.AreEqual(1037028940, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002321130, Engine.FeeConsumed.Value);

            array.Clear();
            array.Add(2);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            Assert.AreEqual(2, Contract.FirstGreaterThanZero(array));
            Assert.AreEqual(1003505530, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1003039900, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, result.Count);

            array.Add(5);
            result = (Array)Contract.SelectTwice(array)!;
            Assert.AreEqual(1005270400, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1016010370, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002968440, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, result.Count);

            array.Add(new BigInteger(1));
            array.Add(new BigInteger(5));
            array.Add(new BigInteger(100));

            result = (Array)Contract.Skip(array, 2)!;
            Assert.AreEqual(1005117220, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002231610, Engine.FeeConsumed.Value);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            Assert.AreEqual(5, Contract.Sum(array));
            Assert.AreEqual(1003397590, Engine.FeeConsumed.Value);
            Assert.AreEqual(10, Contract.SumTwice(array));
            Assert.AreEqual(1004690200, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002224620, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)Contract.Take(array, 2)!;
            Assert.AreEqual(1003872430, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1012949560, Engine.FeeConsumed.Value);
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
            Assert.AreEqual(1002301720, Engine.FeeConsumed.Value);
            Assert.AreEqual(0, result.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)Contract.WhereGreaterThanZero(array)!;
            Assert.AreEqual(1004346640, Engine.FeeConsumed.Value);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(100, result[1]);
            Assert.AreEqual(56, result[2]);
        }
    }
}
