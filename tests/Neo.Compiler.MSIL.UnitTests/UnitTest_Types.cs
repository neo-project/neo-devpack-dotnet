using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Types
    {
        [TestMethod]
        public void null_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkNull");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Null));
        }

        [TestMethod]
        public void bool_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkBoolTrue");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(true, ((Boolean)item).ToBoolean());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkBoolFalse");

            Assert.IsTrue(result.TryPop(out item));
            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.AreEqual(false, ((Boolean)item).ToBoolean());
        }

        [TestMethod]
        public void sbyte_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkSbyte");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void byte_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkByte");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void short_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkShort");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void ushort_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkUshort");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void int_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkInt");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void uint_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkUint");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void long_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkLong");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void ulong_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkUlong");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void bigInteger_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkBigInteger");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void byteArray_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkByteArray");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Buffer));
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, ((Buffer)item).GetSpan().ToArray());
        }

        [TestMethod]
        public void char_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkChar");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual((int)'n', ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void string_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkString");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("neo", ((ByteString)item).GetString());
        }

        [TestMethod]
        public void arrayObj_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkArrayObj");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(1, ((Array)item).Count);
            Assert.AreEqual("neo", (((Array)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void enum_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkEnum");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).ToBigInteger());
        }

        [TestMethod]
        public void class_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkClass");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(1, ((Array)item).Count);
            Assert.AreEqual("neo", (((Array)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void struct_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkStruct");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Struct));
            Assert.AreEqual(1, ((Struct)item).Count);
            Assert.AreEqual("neo", (((Struct)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void tuple_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkTuple");

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Struct));
            Assert.AreEqual(2, ((Struct)item).Count);
            Assert.AreEqual("neo", (((Struct)item)[0] as ByteString).GetString());
            Assert.AreEqual("smart economy", (((Struct)item)[1] as ByteString).GetString());
        }

        [TestMethod]
        public void event_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkEvent");
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(1, testengine.Notifications.Count);

            var item = testengine.Notifications.First().State;

            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual(2, ((Array)item).Count);
            Assert.AreEqual("dummyEvent", (((Array)item)[0] as ByteString).GetString());
            Assert.AreEqual("neo", (((Array)item)[1] as ByteString).GetString());
        }

        [TestMethod]
        public void lambda_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkLambda");
            Assert.AreEqual(1, result.Count);

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void delegate_Test()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkDelegate");
            Assert.AreEqual(1, result.Count);

            Assert.IsTrue(result.TryPop(out StackItem item));
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }
    }
}
