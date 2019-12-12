using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Array
    {
        [TestMethod]
        public void Test_IntArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("intarray");

            //test (1+5)*7 == 42
            StackItem wantresult = 33;
            var bequal = wantresult.Equals(result.Pop());
            Assert.IsTrue(bequal);
        }

        [TestMethod]
        public void Test_IntArrayInit()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("intarrayinit");

            //test 1,4,5
            Assert.IsTrue(result.TryPop(out Array arr));
            var wantarray = new int[] { 1, 4, 5 };
            Assert.AreEqual(wantarray.Length, arr.Count);
            var resultarray = arr.Cast<Integer>().Select(u => u.ToBigInteger()).ToArray();
            for(var i=0;i<wantarray.Length;i++)
            {
                Assert.AreEqual(resultarray[i], wantarray[i]);
            }
        }

        [TestMethod]
        public void Test_StructArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Array.cs");
            var result = testengine.ExecuteTestCaseStandard("structarray");

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
            var result = testengine.ExecuteTestCaseStandard("structarrayinit");

            //test (1+5)*7 == 42
            var neostruct = result.Pop() as Struct;

            var bequal = neostruct != null;
            Assert.IsTrue(bequal);
        }
    }
}
