using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_NULL
    {
        private TestEngine testengine;

        [TestInitialize]
        public void Init()
        {
            testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_NULL.cs");
        }

        [TestMethod]
        public void IsNull()
        {
            // True

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("isNull", StackItem.Null);
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("isNull", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());
        }

        [TestMethod]
        public void NullCoalescing()
        {
            //  call NullCoalescing(string code)
            // return  code ?.Substring(1,2);

            // a123b->12
            testengine.Reset();
            {
                var result = testengine.ExecuteTestCaseStandard("nullCoalescing", "a123b");
                var item = result.Pop() as ByteArray;
                System.ReadOnlySpan<byte> data = item; 
                var str = System.Text.Encoding.ASCII.GetString(data);
                Assert.IsTrue(str == "12");
            }
            // null->null

            testengine.Reset();
            {
                var result = testengine.ExecuteTestCaseStandard("nullCoalescing", StackItem.Null);
                var item = result.Pop();

                Assert.IsTrue(item.IsNull);
            }
        }

        [TestMethod]
        public void NullCollation()
        {
            // call nullCollation(string code)
            // return code ?? "linux"

            // nes->nes
            testengine.Reset();
            {
                var result = testengine.ExecuteTestCaseStandard("nullCollation", "nes");
                var item = result.Pop() as ByteArray;
                System.ReadOnlySpan<byte> data = item;
                var str = System.Text.Encoding.ASCII.GetString(data);
                Assert.IsTrue(str == "nes");
            }

            // null->linux
            testengine.Reset();
            {
                var result = testengine.ExecuteTestCaseStandard("nullCollation", StackItem.Null);
                var item = result.Pop() as ByteArray;
                System.ReadOnlySpan<byte> data = item;
                var str = System.Text.Encoding.ASCII.GetString(data);
                Assert.IsTrue(str == "linux");
            }
        }
        [TestMethod]
        public void NullCollationAndCollation()
        {
            var _testengine = new TestEngine();
            _testengine.AddEntryScript("./TestClasses/Contract_NULL.cs");
            _testengine.Snapshot.Contracts.Add(testengine.EntryScriptHash, new Ledger.ContractState()
            {
                Script = testengine.EntryContext.Script,
                Manifest = new ContractManifest()
                {
                    Features = ContractFeatures.HasStorage
                }
            });
            {
                var result = _testengine.ExecuteTestCaseStandard("nullCollationAndCollation", "nes");
                var item = result.Pop() as ByteArray;
                var num = item.ToBigInteger();
                Assert.IsTrue(num == 123);
            }
        }
        [TestMethod]
        public void NullCollationAndCollation2()
        {
            var _testengine = new TestEngine();
            _testengine.AddEntryScript("./TestClasses/Contract_NULL.cs");
            _testengine.Snapshot.Contracts.Add(testengine.EntryScriptHash, new Ledger.ContractState()
            {
                Script = testengine.EntryContext.Script,
                Manifest = new ContractManifest()
                {
                    Features = ContractFeatures.HasStorage
                }
            });
            {
                var result = _testengine.ExecuteTestCaseStandard("nullCollationAndCollation2", "nes");
                var item = result.Pop() as ByteArray;
                System.ReadOnlySpan<byte> data = item;
                var num = System.Text.Encoding.ASCII.GetString(data);
                Assert.IsTrue(num == "111");
            }
        }
        [TestMethod]
        public void EqualNull()
        {
            // True

            testengine.Reset();
            var result = testengine.ExecuteTestCaseStandard("equalNullA", StackItem.Null);
            var item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("equalNullA", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());

            // True

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("equalNullB", StackItem.Null);
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsTrue(item.ToBoolean());

            // False

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("equalNullB", new Integer(1));
            item = result.Pop();

            Assert.IsInstanceOfType(item, typeof(Boolean));
            Assert.IsFalse(item.ToBoolean());
        }
    }
}
