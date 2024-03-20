using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Lambda
    {
        private TestEngine testengine;
        private TestDataCache snapshot;

        [TestInitialize]
        public void Init()
        {
            snapshot = new TestDataCache();
            testengine = new TestEngine(snapshot: snapshot);
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Lambda.cs");
        }


        [TestMethod]
        public void Test_AnyGreatThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("anyGreatThanZero", array).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("anyGreatThanZero", array).Pop();
            Assert.AreEqual(true, result.GetBoolean());
        }

        [TestMethod]
        public void Test_AnyGreatThan()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("anyGreatThan", array, 0).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("anyGreatThan", array, 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("anyGreatThan", array, 100).Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }


        [TestMethod]
        public void Test_WhereGreatThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("whereGreatThanZero", array).Pop();
            Assert.AreEqual(0, result.Count);

            testengine.Reset();
            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)testengine.ExecuteTestCaseStandard("whereGreatThanZero", array).Pop();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(100, result[1]);
            Assert.AreEqual(56, result[2]);

        }


        [TestMethod]
        public void Test_ForEachVar()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("forEachVar", array).Pop();
            Assert.AreEqual(array.Count, result.Count);
            Assert.AreEqual(-100, result[0]);
        }

        [TestMethod]
        public void Test_ForVar()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("forVar", array).Pop();
            Assert.AreEqual(array.Count, result.Count);
            Assert.AreEqual(-100, result[0]);
        }

        [TestMethod]
        public void Test_ChangeName()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("changeName", "L").Pop();
            Assert.AreEqual("L !!!", result.GetString());
        }

        [TestMethod]
        public void Test_ChangeName2()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("changeName2", "L").Pop();
            Assert.AreEqual("L !!!", result.GetString());
        }

        [TestMethod]
        public void Test_InvokeSum()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("invokeSum", 2, 3).Pop();
            Assert.AreEqual(5, result.GetInteger());
        }

        [TestMethod]
        public void Test_InvokeSum2()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("invokeSum2", 2, 3).Pop();
            Assert.AreEqual(6, result.GetInteger());
        }

        [TestMethod]
        public void Test_Fibo()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("fibo", 2).Pop();
            Assert.AreEqual(1, result.GetInteger());


            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("fibo", 3).Pop();
            Assert.AreEqual(2, result.GetInteger());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("fibo", 4).Pop();
            Assert.AreEqual(3, result.GetInteger());
        }


        [TestMethod]
        public void Test_CheckZero()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("checkZero", 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZero", 1).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZero", -1).Pop();
            Assert.AreEqual(false, result.GetBoolean());

        }


        [TestMethod]
        public void Test_CheckZero2()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("checkZero2", 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZero2", 1).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZero2", -1).Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }


        [TestMethod]
        public void Test_CheckZero3()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("checkZero3", 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZero3", 1).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkZero3", -1).Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }

        [TestMethod]
        public void Test_CheckPositiveOdd()
        {
            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("checkPositiveOdd", 3).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkPositiveOdd", 0).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkPositiveOdd", 2).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkPositiveOdd", -1).Pop();
            Assert.AreEqual(false, result.GetBoolean());

        }
    }
}
