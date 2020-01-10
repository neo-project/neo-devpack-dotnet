using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.IO;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class ContractTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Contract.cs");
        }

        [TestMethod]
        public void Test_CreateCallDestroy()
        {
            // Create

            byte[] script;
            using (var scriptBuilder = new ScriptBuilder())
            {
                // Drop arguments

                scriptBuilder.Emit(VM.OpCode.DROP);
                scriptBuilder.Emit(VM.OpCode.DROP);

                // Return 123

                scriptBuilder.EmitPush(123);
                script = scriptBuilder.ToArray();
            }

            var manifest = ContractManifest.CreateDefault(script.ToScriptHash());

            // Check first

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray());
            Assert.AreEqual(VMState.FAULT, _engine.State);

            // Create

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("create", script, manifest.ToJson().ToString());
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);

                var item = result.Pop() as InteropInterface;
                Assert.IsTrue(item.Type == StackItemType.InteropInterface);
                var ledger = item.GetInterface<Ledger.ContractState>();
                Assert.AreEqual(manifest.Hash, ledger.ScriptHash);
            }
            // Call

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), Null.Null, Null.Null);
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);

                var item = result.Pop();
                Assert.IsInstanceOfType(item, typeof(ByteArray));
                Assert.AreEqual(123, item.GetBigInteger());
            }
            // Destroy

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("destroy");
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);

                var item = result.Pop();
                Assert.IsInstanceOfType(item, typeof(ByteArray));
                Assert.AreEqual(0, item.GetByteLength());
            }
            // Check again for failures

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray());
                Assert.AreEqual(VMState.FAULT, _engine.State);
            }
        }

        [TestMethod]
        public void Test_Update()
        {
            // Create

            byte[] scriptUpdate;
            using (var scriptBuilder = new ScriptBuilder())
            {
                // Drop arguments

                scriptBuilder.Emit(VM.OpCode.DROP);
                scriptBuilder.Emit(VM.OpCode.DROP);

                // Return 124

                scriptBuilder.EmitPush(123);
                scriptBuilder.Emit(VM.OpCode.INC);
                scriptUpdate = scriptBuilder.ToArray();
            }

            var manifestUpdate = ContractManifest.CreateDefault(scriptUpdate.ToScriptHash());

            byte[] script;
            using (var scriptBuilder = new ScriptBuilder())
            {
                // Drop arguments

                scriptBuilder.Emit(VM.OpCode.DROP);
                scriptBuilder.Emit(VM.OpCode.DROP);

                // Return 123

                scriptBuilder.EmitPush(123);

                // Update

                scriptBuilder.EmitSysCall(InteropService.Contract.Update, scriptUpdate, manifestUpdate.ToJson().ToString());
                script = scriptBuilder.ToArray();
            }

            var manifest = ContractManifest.CreateDefault(script.ToScriptHash());

            // Check first

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray());
            Assert.AreEqual(VMState.FAULT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("call", manifestUpdate.Hash.ToArray());
            Assert.AreEqual(VMState.FAULT, _engine.State);

            // Create

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("create", script, manifest.ToJson().ToString());
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);

                var item = result.Pop() as InteropInterface;
                Assert.IsTrue(item.Type == StackItemType.InteropInterface);
                var ledger = item.GetInterface<Ledger.ContractState>();
                Assert.AreEqual(manifest.Hash, ledger.ScriptHash);
            }
            // Call & Update

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray(), Null.Null, Null.Null);
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);

                var item = result.Pop();
                Assert.IsInstanceOfType(item, typeof(ByteArray));
                Assert.AreEqual(123, item.GetBigInteger());
            }
            // Call Again

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("call", manifestUpdate.Hash.ToArray(), Null.Null, Null.Null);
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);

                var item = result.Pop();
                Assert.IsInstanceOfType(item, typeof(Integer));
                Assert.AreEqual(124, item.GetBigInteger());
            }
            // Check again for failures

            _engine.Reset();
            {
                result = _engine.ExecuteTestCaseStandard("call", manifest.Hash.ToArray());
                Assert.AreEqual(VMState.FAULT, _engine.State);
            }
        }
    }
}
