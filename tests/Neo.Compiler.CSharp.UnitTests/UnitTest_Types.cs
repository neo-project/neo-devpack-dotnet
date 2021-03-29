using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Types
    {
        #region Unsupported Types

        [TestMethod]
        public void float_Test()
        {
            using var testengine = new TestEngine();
            var ex = Assert.ThrowsException<System.Exception>(() => testengine.AddEntryScript("./TestClasses/Contract_Types_Float.cs"));
            Assert.IsTrue(ex.InnerException.Message.Contains("unsupported instruction"));
        }

        [TestMethod]
        public void decimal_Test()
        {
            using var testengine = new TestEngine();
            try
            {
                testengine.AddEntryScript("./TestClasses/Contract_Types_Decimal.cs");
                Assert.Fail("must be fault.");
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("unsupported"));
                return;
            }
        }

        [TestMethod]
        public void double_Test()
        {
            using var testengine = new TestEngine();
            var ex = Assert.ThrowsException<System.Exception>(() => testengine.AddEntryScript("./TestClasses/Contract_Types_Double.cs"));
            Assert.IsTrue(ex.InnerException.Message.Contains("unsupported instruction"));
        }

        #endregion

        [TestMethod]
        public void null_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkNull");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));
        }

        [TestMethod]
        public void bool_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkBoolTrue");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(1, ((Integer)item).GetInteger());

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("checkBoolFalse");

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, ((Integer)item).GetInteger());
        }

        //[TestMethod]
        //public void checkEnumArg_Test()
        //{
        //    using var testengine = new TestEngine();
        //    testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
        //    var methods = (JArray)testengine.ScriptEntry.finalABI["methods"];
        //    var checkEnumArg = methods.Where(u => u["name"].AsString() == "checkEnumArg").FirstOrDefault();
        //    Assert.AreEqual(checkEnumArg["parameters"].ToString(), @"[{""name"":""arg"",""type"":""Integer""}]");
        //}

        [TestMethod]
        public void sbyte_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkSbyte");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void byte_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkByte");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void short_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkShort");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void ushort_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkUshort");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void int_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkInt");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void uint_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkUint");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void long_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkLong");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void ulong_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkUlong");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void bigInteger_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkBigInteger");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void byteArray_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkByteArray");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Buffer));
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, ((VM.Types.Buffer)item).GetSpan().ToArray());
        }

        [TestMethod]
        public void char_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkChar");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual((int)'n', ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void string_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkString");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("neo", ((ByteString)item).GetString());
        }

        [TestMethod]
        public void arrayObj_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkArrayObj");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(1, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void enum_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkEnum");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, ((Integer)item).GetInteger());
        }

        [TestMethod]
        public void class_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkClass");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(1, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void struct_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkStruct");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Struct));
            Assert.AreEqual(1, ((Struct)item).Count);
            Assert.AreEqual("neo", (((Struct)item)[0] as ByteString).GetString());
        }

        [TestMethod]
        public void tuple_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkTuple");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(2, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
            Assert.AreEqual("smart economy", (((VM.Types.Array)item)[1] as ByteString).GetString());
        }

        [TestMethod]
        public void tuple2_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkTuple2");

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Array));
            Assert.AreEqual(2, ((VM.Types.Array)item).Count);
            Assert.AreEqual("neo", (((VM.Types.Array)item)[0] as ByteString).GetString());
            Assert.AreEqual("smart economy", (((VM.Types.Array)item)[1] as ByteString).GetString());
        }

        [TestMethod]
        public void event_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkEvent");
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(1, testengine.Notifications.Count);

            var item = testengine.Notifications.First();

            Assert.AreEqual(1, item.State.Count);
            Assert.AreEqual("dummyEvent", item.EventName);
            Assert.AreEqual("neo", (item.State[0] as ByteString).GetString());
        }

        [TestMethod]
        public void lambda_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkLambda");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }

        [TestMethod]
        public void delegate_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Types.cs");
            var result = testengine.ExecuteTestCaseStandard("checkDelegate");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Pointer));
        }
    }
}
