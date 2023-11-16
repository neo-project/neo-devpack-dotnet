using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class BackedStorageTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var system = TestBlockchain.TheNeoSystem;
            var snapshot = system.GetSnapshot().CreateSnapshot();

            _engine = new TestEngine(snapshot: snapshot);
            Assert.IsTrue(_engine.AddEntryScript("./TestClasses/Contract_StorageBacked.cs").Success);
            snapshot.ContractAdd(new ContractState()
            {
                Id = 0,
                Hash = _engine.EntryScriptHash,
                Nef = _engine.Nef,
                Manifest = new Manifest.ContractManifest()
            });
        }

        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(0, _engine.Snapshot.GetChangeSet().Where(u => u.Key.Id == 0).Count());

            Test_Kind("WithoutConstructor");
            Test_Kind("WithKey");
            Test_Kind("WithString");

            Assert.AreEqual(3, _engine.Snapshot.GetChangeSet().Where(u => u.Key.Id == 0).Count());
        }

        public void Test_Kind(string kind)
        {
            Console.WriteLine("GET");
            _engine.Reset();

            var result = _engine.ExecuteTestCaseStandard("get" + kind);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().IsNull);

            Console.WriteLine("PUT");
            _engine.Reset();

            _engine.ExecuteTestCaseStandard("put" + kind, 123);
            Assert.AreEqual(VMState.HALT, _engine.State);

            Console.WriteLine("GET");
            _engine.Reset();

            result = _engine.ExecuteTestCaseStandard("get" + kind);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(new BigInteger(123), result.Pop().GetInteger());
        }
    }
}
