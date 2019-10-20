using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Switch
    {
        /// <summary>
        /// switch of more than 6 entries require a ComputeStringHash method
        /// </summary>
        [TestMethod]
        public void Test_SwitchLong()
        {
<<<<<<< HEAD
<<<<<<< HEAD
            RandomAccessStack<StackItem> result;
            TestEngine testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SwitchLong.cs");

            // Test cases

            for (int x = 0; x <= 20; x++)
            {
                testengine.Reset();
=======
            TestEngine testengine;

=======
>>>>>>> 9129587... UT optimization (#117)
            RandomAccessStack<StackItem> result;
            TestEngine testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SwitchLong.cs");

            // Test cases

            for (int x = 0; x <= 20; x++)
            {
<<<<<<< HEAD
                testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_SwitchLong.cs");

>>>>>>> a2dbe13... 1.add support for switchlong
=======
                testengine.Reset();
>>>>>>> 9129587... UT optimization (#117)
                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

<<<<<<< HEAD
<<<<<<< HEAD
            testengine.Reset();
=======
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_SwitchLong.cs");

>>>>>>> a2dbe13... 1.add support for switchlong
=======
            testengine.Reset();
>>>>>>> 9129587... UT optimization (#117)
            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }

        [TestMethod]
        public void Test_Switch6()
        {
            RandomAccessStack<StackItem> result;
            TestEngine testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Switch6.cs");

            // Test cases

            for (int x = 0; x <= 5; x++)
            {
<<<<<<< HEAD
<<<<<<< HEAD
                testengine.Reset();
=======
                testengine = new TestEngine();
                testengine.AddEntryScript("./TestClasses/Contract_Switch6.cs");

>>>>>>> b4c4c78... change test filename
=======
                testengine.Reset();
>>>>>>> 9129587... UT optimization (#117)
                result = testengine.ExecuteTestCaseStandard(x.ToString());
                Assert.AreEqual(result.Pop().GetBigInteger(), x + 1);
            }

            // Test default

<<<<<<< HEAD
<<<<<<< HEAD
            testengine.Reset();
=======
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Switch6.cs");

>>>>>>> b4c4c78... change test filename
=======
            testengine.Reset();
>>>>>>> 9129587... UT optimization (#117)
            result = testengine.ExecuteTestCaseStandard("default");
            Assert.AreEqual(result.Pop().GetBigInteger(), 99);
        }
    }
}
