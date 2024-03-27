using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Linq
    {
        private TestEngine testengine;
        private TestDataCache snapshot;

        [TestInitialize]
        public void Init()
        {
            snapshot = new TestDataCache();
            testengine = new TestEngine(snapshot: snapshot);
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Linq.cs");
        }

        [TestMethod]
        public void Test_AggregateSum()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("aggregateSum", array).Pop();
            Assert.AreEqual(-101, result.GetInteger());

            testengine.Reset();
            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = testengine.ExecuteTestCaseStandard("aggregateSum", array).Pop();
            Assert.AreEqual(5, result.GetInteger());
        }

        [TestMethod]
        public void Test_AllGreaterThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(1);
            array.Add(100);
            var result = testengine.ExecuteTestCaseStandard("allGreaterThanZero", array).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            array.Add(0);
            result = testengine.ExecuteTestCaseStandard("allGreaterThanZero", array).Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }

        [TestMethod]
        public void Test_IsEmpty()
        {
            testengine.Reset();
            var array = new Array();

            var result = testengine.ExecuteTestCaseStandard("isEmpty", array).Pop();
            Assert.AreEqual(true, result.GetBoolean());
            testengine.Reset();
            array.Add(1);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            result = testengine.ExecuteTestCaseStandard("isEmpty", array).Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }

        [TestMethod]
        public void Test_AnyGreaterThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("anyGreaterThanZero", array).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("anyGreaterThanZero", array).Pop();
            Assert.AreEqual(true, result.GetBoolean());
        }

        [TestMethod]
        public void Test_AnyGreaterThan()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("anyGreaterThan", array, 0).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("anyGreaterThan", array, 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("anyGreaterThan", array, 100).Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }

        [TestMethod]
        public void Test_Average()
        {
            testengine.Reset();
            var array = new Array();

            testengine.ExecuteTestCaseStandard("average", array);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", testengine.FaultException.Message);

            testengine.Reset();
            array.Add(0);
            array.Add(1);
            array.Add(2);

            var result = testengine.ExecuteTestCaseStandard("average", array).Pop();
            Assert.AreEqual(1, result.GetInteger());

            testengine.Reset();
            array.Add(3);
            result = testengine.ExecuteTestCaseStandard("average", array).Pop();
            Assert.AreEqual(1, result.GetInteger());
        }

        [TestMethod]
        public void Test_AverageTwice()
        {
            testengine.Reset();
            var array = new Array();

            testengine.ExecuteTestCaseStandard("averageTwice", array);
            Assert.AreEqual("An unhandled exception was thrown. source is empty", testengine.FaultException.Message);

            testengine.Reset();
            array.Add(0);
            array.Add(1);
            array.Add(2);
            var result = testengine.ExecuteTestCaseStandard("averageTwice", array).Pop();
            Assert.AreEqual(2, result.GetInteger());

            testengine.Reset();
            array.Add(3);

            result = testengine.ExecuteTestCaseStandard("averageTwice", array).Pop();
            Assert.AreEqual(3, result.GetInteger());
        }

        [TestMethod]
        public void Test_Count()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("count", array).Pop();
            Assert.AreEqual(3, result.GetInteger());

            testengine.Reset();
            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = testengine.ExecuteTestCaseStandard("count", array).Pop();
            Assert.AreEqual(7, result.GetInteger());
        }

        [TestMethod]
        public void Test_CountGreaterThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("countGreaterThanZero", array).Pop();
            Assert.AreEqual(0, result.GetInteger());

            testengine.Reset();
            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = testengine.ExecuteTestCaseStandard("countGreaterThanZero", array).Pop();
            Assert.AreEqual(3, result.GetInteger());
        }

        [TestMethod]
        public void Test_Contains()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("contains", array, 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("contains", array, 9).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("contains", array, 1).Pop();
            Assert.AreEqual(true, result.GetBoolean());
        }

        [TestMethod]
        public void Test_ContainsText()
        {
            testengine.Reset();
            var array = new Array();
            array.Add("Hello");
            array.Add("AA");
            array.Add("bbb");
            var result = testengine.ExecuteTestCaseStandard("containsText", array, "bbb").Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("containsText", array, "c").Pop();
            Assert.AreEqual(false, result.GetBoolean());
        }

        [TestMethod]
        public void Test_ContainsPerson()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("containsPerson", array, 0).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("containsPerson", array, 1).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("containsPersonIndex", array, 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());
        }

        [TestMethod]
        public void Test_ContainsPersonS()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("containsPersonS", array, 0).Pop();
            Assert.AreEqual(true, result.GetBoolean());

            testengine.Reset();
            array.Add(1);
            result = testengine.ExecuteTestCaseStandard("containsPersonS", array, 10).Pop();
            Assert.AreEqual(false, result.GetBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("containsPersonS", array, -100).Pop();
            Assert.AreEqual(true, result.GetBoolean());
        }

        [TestMethod]
        public void Test_FirstGreaterThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            array.Add(1);
            var result = testengine.ExecuteTestCaseStandard("firstGreaterThanZero", array).Pop();
            Assert.AreEqual(1, result.GetInteger());

            testengine.Reset();
            array.Clear();
            array.Add(2);
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            result = testengine.ExecuteTestCaseStandard("firstGreaterThanZero", array).Pop();
            Assert.AreEqual(2, result.GetInteger());
        }

        [TestMethod]
        public void Test_SelectTwice()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("selectTwice", array).Pop();
            Assert.AreEqual(3, result.Count);

            testengine.Reset();
            array.Add(5);
            result = (Array)testengine.ExecuteTestCaseStandard("selectTwice", array).Pop();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(-2, result[1]);
            Assert.AreEqual(-200, result[2]);
            Assert.AreEqual(10, result[3]);
        }

        [TestMethod]
        public void Test_SelectPersonS()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            array.Add(5);
            var result = (Array)testengine.ExecuteTestCaseStandard("selectPersonS", array).Pop();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(array[0], ((Struct)result[0])[1]);
            Assert.AreEqual(array[1], ((Struct)result[1])[1]);
            Assert.AreEqual(array[2], ((Struct)result[2])[1]);
            Assert.AreEqual(array[3], ((Struct)result[3])[1]);
        }


        [TestMethod]
        public void Test_Skip()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("skip", array, 0).Pop();
            Assert.AreEqual(3, result.Count);

            testengine.Reset();
            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)testengine.ExecuteTestCaseStandard("skip", array, 2).Pop();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(-100, result[0]);
            Assert.AreEqual(100, result[3]);
            Assert.AreEqual(100, result[3]);
        }

        [TestMethod]
        public void Test_Sum()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = testengine.ExecuteTestCaseStandard("sum", array).Pop();
            Assert.AreEqual(-101, result.GetInteger());

            testengine.Reset();
            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = testengine.ExecuteTestCaseStandard("sum", array).Pop();
            Assert.AreEqual(5, result.GetInteger());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("sumTwice", array).Pop();
            Assert.AreEqual(10, result.GetInteger());
        }

        [TestMethod]
        public void Test_Take()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("take", array, 0).Pop();
            Assert.AreEqual(0, result.Count);

            testengine.Reset();
            array.Add(1);
            array.Add(5);
            array.Add(100);

            result = (Array)testengine.ExecuteTestCaseStandard("take", array, 2).Pop();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0]);
        }

        [TestMethod]
        public void Test_ToMap()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            array.Add(5);
            var result = (Map)testengine.ExecuteTestCaseStandard("toMap", array).Pop();
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(array[0], ((Struct)result[array[0].ToString()])[1]);
            Assert.AreEqual(array[1], ((Struct)result[array[1].ToString()])[1]);
            Assert.AreEqual(array[2], ((Struct)result[array[2].ToString()])[1]);
            Assert.AreEqual(array[3], ((Struct)result[array[3].ToString()])[1]);
        }

        [TestMethod]
        public void Test_WhereGreaterThanZero()
        {
            testengine.Reset();
            var array = new Array();
            array.Add(0);
            array.Add(-1);
            array.Add(-100);
            var result = (Array)testengine.ExecuteTestCaseStandard("whereGreaterThanZero", array).Pop();
            Assert.AreEqual(0, result.Count);

            testengine.Reset();
            array.Add(1);
            array.Add(-8);
            array.Add(100);
            array.Add(56);

            result = (Array)testengine.ExecuteTestCaseStandard("whereGreaterThanZero", array).Pop();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(100, result[1]);
            Assert.AreEqual(56, result[2]);
        }
    }
}
