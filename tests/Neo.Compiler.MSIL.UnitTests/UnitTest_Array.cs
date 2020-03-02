using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Array
    {
        [TestMethod]
        public void Test_IntArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("testIntArray");

            //test 0,1,2
            Assert.IsTrue(result.TryPop(out Array arr));
            CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, arr.Cast<PrimitiveType>().Select(u => (int)u.ToBigInteger()).ToArray());
        }

        [TestMethod]
        public void Test_IntArrayInit()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("testIntArrayInit");

            //test 1,4,5
            Assert.IsTrue(result.TryPop(out Array arr));
            CollectionAssert.AreEqual(new int[] { 1, 4, 5 }, arr.Cast<Integer>().Select(u => (int)u.ToBigInteger()).ToArray());
        }

        /* TODO: We should uncomment this when NEWARRAY_T was done
        [TestMethod]
        public void Test_DefaultArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("TestDefaultArray");

            //test true
            Assert.IsTrue(result.TryPop(out Boolean b) && b.ToBoolean());
        }
        */

        [TestMethod]
        public void Test_StructArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("testStructArray");

            //test (1+5)*7 == 42
            var neostruct = result.Pop() as Struct;

            var bequal = neostruct != null;
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_StructArrayInit()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("testStructArrayInit");

            //test (1+5)*7 == 42
            var neostruct = result.Pop() as Struct;

            var bequal = neostruct != null;
            Assert.IsTrue(bequal);
        }
    }
}
