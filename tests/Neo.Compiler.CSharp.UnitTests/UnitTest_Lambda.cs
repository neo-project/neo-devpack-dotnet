using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Lambda : TestBase<Contract_Lambda>
    {
        public UnitTest_Lambda() : base(Contract_Lambda.Nef, Contract_Lambda.Manifest) { }

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
            Assert.AreEqual(false, result);

            array.Add(1);
            result = Contract.AnyGreatThanZero(array);
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
            Assert.AreEqual(false, result);

            array.Add(1);
            result = Contract.AnyGreatThan(array, 0);
            Assert.AreEqual(true, result);

            result = Contract.AnyGreatThan(array, 100);
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
            Assert.AreEqual(0, result!.Count);

            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = Contract.WhereGreaterThanZero(array);
            Assert.AreEqual(3, result!.Count);
            Assert.AreEqual(1, (result[0] as Integer)!.GetInteger());
            Assert.AreEqual(100, (result[1] as Integer)!.GetInteger());
            Assert.AreEqual(56, (result[2] as Integer)!.GetInteger());
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
            Assert.AreEqual(array.Count, result!.Count);
            Assert.AreEqual(-100, (result[0] as Integer)!.GetInteger());
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
            Assert.AreEqual(array.Count, result!.Count);
            Assert.AreEqual(-100, (result[0] as Integer)!.GetInteger());
        }

        [TestMethod]
        public void Test_ChangeName()
        {
            var result = Contract.ChangeName("L");
            Assert.AreEqual("L !!!", result);
        }

        [TestMethod]
        public void Test_ChangeName2()
        {
            var result = Contract.ChangeName2("L");
            Assert.AreEqual("L !!!", result);
        }

        [TestMethod]
        public void Test_InvokeSum()
        {
            var result = Contract.InvokeSum(2, 3);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Test_InvokeSum2()
        {
            var result = Contract.InvokeSum2(2, 3);
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void Test_Fibo()
        {
            var result = Contract.Fibo(2);
            Assert.AreEqual(1, result);

            result = Contract.Fibo(3);
            Assert.AreEqual(2, result);

            result = Contract.Fibo(4);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void Test_CheckZero()
        {
            var result = Contract.CheckZero(0);
            Assert.AreEqual(true, result);

            result = Contract.CheckZero(1);
            Assert.AreEqual(false, result);

            result = Contract.CheckZero(-1);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_CheckZero2()
        {
            var result = Contract.CheckZero2(0);
            Assert.AreEqual(true, result);

            result = Contract.CheckZero2(1);
            Assert.AreEqual(false, result);

            result = Contract.CheckZero2(-1);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_CheckZero3()
        {
            var result = Contract.CheckZero3(0);
            Assert.AreEqual(true, result);

            result = Contract.CheckZero3(1);
            Assert.AreEqual(false, result);

            result = Contract.CheckZero3(-1);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void Test_CheckPositiveOdd()
        {
            var result = Contract.CheckPositiveOdd(3);
            Assert.AreEqual(true, result);

            result = Contract.CheckPositiveOdd(0);
            Assert.AreEqual(false, result);

            result = Contract.CheckPositiveOdd(2);
            Assert.AreEqual(false, result);

            result = Contract.CheckPositiveOdd(-1);
            Assert.AreEqual(false, result);
        }
        
        [TestMethod]
        public void Test_LambdaDefault()
        {
            var result = Contract.TestLambdaDefault(3);
            Assert.AreEqual(4, result);

            result = Contract.TestLambdaDefault(5);
            Assert.AreEqual(6, result);

            result = Contract.TestLambdaNotDefault(5, 3);
            Assert.AreEqual(8, result);
        }
    }
}
