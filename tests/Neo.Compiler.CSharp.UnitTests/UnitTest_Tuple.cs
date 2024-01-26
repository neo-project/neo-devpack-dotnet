using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM.Types;
using System.Linq;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Tuple
    {
        [TestMethod]
        public void Test_Assign()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript<Contract_Tuple>();
            var result = testengine.ExecuteTestCaseStandard("t1");

            var tuple = result.Pop<Struct>();
            Assert.AreEqual(5, tuple.Count);
            Assert.AreEqual(1, tuple[2].GetInteger());
            Assert.AreEqual(4, tuple[3].GetInteger());
            Assert.AreEqual(2, ((Struct)tuple[4])[1].GetInteger());
        }
    }
}
