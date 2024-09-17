using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Lambda : DebugAndTestBase<Contract_Lambda>
    {
        [TestMethod]
        public void Test_AnyGreatThanZero()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = Contract.AnyGreatThanZero(array);
            AssertGasConsumed(1188720);
            Assert.AreEqual(false, result);

            array.Add(1);
            result = Contract.AnyGreatThanZero(array);
            AssertGasConsumed(1208640);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Test_AnyGreatThan()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = Contract.AnyGreatThan(array, 0);
            AssertGasConsumed(1188960);
            Assert.AreEqual(false, result);

            array.Add(1);
            result = Contract.AnyGreatThan(array, 0);
            AssertGasConsumed(1208910);
            Assert.AreEqual(true, result);

            result = Contract.AnyGreatThan(array, 100);
            AssertGasConsumed(1209330);
            Assert.AreEqual(false, result);
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
            var result = Contract.WhereGreaterThanZero(array);
            AssertGasConsumed(1189290);
            Assert.AreEqual(0, result!.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = Contract.WhereGreaterThanZero(array);
            AssertGasConsumed(2008290);
            Assert.AreEqual(3, result!.Count);
            Assert.AreEqual(new BigInteger(1), result[0]);
            Assert.AreEqual(new BigInteger(100), result[1]);
            Assert.AreEqual(new BigInteger(56), result[2]);
        }

        [TestMethod]
        public void Test_ForEachVar()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = Contract.ForEachVar(array);
            AssertGasConsumed(2649330);
            Assert.AreEqual(array.Count, result!.Count);
            Assert.AreEqual(new BigInteger(-100), result[0]);
        }

        [TestMethod]
        public void Test_ForVar()
        {
            var array = new List<object>
            {
                0,
                -1,
                -100
            };
            var result = Contract.ForVar(array);
            AssertGasConsumed(2651670);
            Assert.AreEqual(array.Count, result!.Count);
            Assert.AreEqual(new BigInteger(-100), result[0]);
        }

        [TestMethod]
        public void Test_ChangeName()
        {
            var result = Contract.ChangeName("L");
            AssertGasConsumed(1371630);
            Assert.AreEqual("L !!!", result);
        }

        [TestMethod]
        public void Test_ChangeName2()
        {
            var result = Contract.ChangeName2("L");
            AssertGasConsumed(1387350);
            Assert.AreEqual("L !!!", result);
        }

        [TestMethod]
        public void Test_InvokeSum()
        {
            var result = Contract.InvokeSum(2, 3);
            AssertGasConsumed(1066230);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Test_InvokeSum2()
        {
            var result = Contract.InvokeSum2(2, 3);
            AssertGasConsumed(1084560);
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void Test_Fibo()
        {
            var result = Contract.Fibo(2);
            AssertGasConsumed(1103640);
            Assert.AreEqual(1, result);

            result = Contract.Fibo(3);
            AssertGasConsumed(1140960);
            Assert.AreEqual(2, result);

            result = Contract.Fibo(4);
            AssertGasConsumed(1215600);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void Test_CheckZero()
        {
            var result = Contract.CheckZero(0);
            AssertGasConsumed(1066560);
            Assert.AreEqual(true, result);

            result = Contract.CheckZero(1);
            AssertGasConsumed(1066560);
            Assert.AreEqual(false, result);

            result = Contract.CheckZero(-1);
            AssertGasConsumed(1066560);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_CheckZero2()
        {
            var result = Contract.CheckZero2(0);
            AssertGasConsumed(1083960);
            Assert.AreEqual(true, result);

            result = Contract.CheckZero2(1);
            AssertGasConsumed(1083960);
            Assert.AreEqual(false, result);

            result = Contract.CheckZero2(-1);
            AssertGasConsumed(1083960);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_CheckZero3()
        {
            var result = Contract.CheckZero3(0);
            AssertGasConsumed(1084230);
            Assert.AreEqual(true, result);

            result = Contract.CheckZero3(1);
            AssertGasConsumed(1084230);
            Assert.AreEqual(false, result);

            result = Contract.CheckZero3(-1);
            AssertGasConsumed(1084230);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_CheckPositiveOdd()
        {
            var result = Contract.CheckPositiveOdd(3);
            AssertGasConsumed(1067190);
            Assert.AreEqual(true, result);

            result = Contract.CheckPositiveOdd(0);
            AssertGasConsumed(1065900);
            Assert.AreEqual(false, result);

            result = Contract.CheckPositiveOdd(2);
            AssertGasConsumed(1067190);
            Assert.AreEqual(false, result);

            result = Contract.CheckPositiveOdd(-1);
            AssertGasConsumed(1065900);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_LambdaDefault()
        {
            var result = Contract.TestLambdaDefault(3);
            AssertGasConsumed(1066350);
            Assert.AreEqual(4, result);

            result = Contract.TestLambdaDefault(5);
            AssertGasConsumed(1066350);
            Assert.AreEqual(6, result);

            result = Contract.TestLambdaNotDefault(5, 3);
            AssertGasConsumed(1066410);
            Assert.AreEqual(8, result);
        }
    }
}
