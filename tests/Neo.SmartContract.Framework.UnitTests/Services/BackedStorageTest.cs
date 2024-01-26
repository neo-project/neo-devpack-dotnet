using System;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework.UnitTests.TestClasses;
using Neo.SmartContract.Framework.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class BackedStorageTest
    {
        private TestEngine.TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var system = TestBlockchain.TheNeoSystem;
            var snapshot = system.GetSnapshot().CreateSnapshot();

            _engine = new TestEngine.TestEngine(snapshot: snapshot);
            Assert.IsTrue(_engine.AddEntryScript<Contract_Stored>().Success);
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
            Assert.AreEqual(0, _engine.Snapshot.GetChangeSet().Count(u => u.Key.Id == 0));

            Test_Kind("WithoutConstructor");
            Test_Kind("WithKey");
            Test_Kind("WithString");

            Assert.AreEqual(3, _engine.Snapshot.GetChangeSet().Count(u => u.Key.Id == 0));
        }

        [TestMethod]
        public void Test_Private_Getter_Public_Setter()
        {
            // Read initial value
            Console.WriteLine("GET");
            _engine.Reset();

            // Test private getter

            var result = _engine.ExecuteTestCaseStandard("getPrivateGetterPublicSetter");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().IsNull);


            // Test public setter
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("setPrivateGetterPublicSetter", 123);
            Assert.AreEqual(VMState.HALT, _engine.State);

            // check public setter

            Console.WriteLine("GET");
            _engine.Reset();

            result = _engine.ExecuteTestCaseStandard("getPrivateGetterPublicSetter");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(new BigInteger(123), result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Non_Static_Private_Getter_Public_Setter()
        {
            // Read initial value
            Console.WriteLine("GET");
            _engine.Reset();

            // Test private getter

            var result = _engine.ExecuteTestCaseStandard("getNonStaticPrivateGetterPublicSetter");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().IsNull);


            // Test public setter
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("setNonStaticPrivateGetterPublicSetter", 123);
            Assert.AreEqual(VMState.HALT, _engine.State);

            // check public setter

            Console.WriteLine("GET");
            _engine.Reset();

            result = _engine.ExecuteTestCaseStandard("getNonStaticPrivateGetterPublicSetter");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(new BigInteger(123), result.Pop().GetInteger());
        }

        public void Test_Kind(string kind)
        {
            // Read initial value

            Console.WriteLine("GET");
            _engine.Reset();

            var result = _engine.ExecuteTestCaseStandard("get" + kind);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().IsNull);

            // Test public getter

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(kind[0].ToString().ToLowerInvariant() + kind[1..]);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.IsTrue(result.Pop().IsNull);

            // Put

            Console.WriteLine("PUT");
            _engine.Reset();

            _engine.ExecuteTestCaseStandard("put" + kind, 123);
            Assert.AreEqual(VMState.HALT, _engine.State);

            Console.WriteLine("GET");
            _engine.Reset();

            result = _engine.ExecuteTestCaseStandard("get" + kind);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(new BigInteger(123), result.Pop().GetInteger());

            // Test public getter

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(kind[0].ToString().ToLowerInvariant() + kind[1..]);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(new BigInteger(123), result.Pop().GetInteger());
        }
    }
}
