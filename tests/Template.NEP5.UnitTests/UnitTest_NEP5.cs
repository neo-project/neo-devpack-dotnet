using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM;
using Neo.VM.Types;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Template.NEP5.UnitTests
{
    [TestClass]
    public class UnitTest_NEP5
    {
        private TestEngine _engine;
        private static readonly byte[] _prefixBalance = { 0x01, 0x01 };
        private static readonly byte[] _prefixContract = { 0x02, 0x02 };

        [TestInitialize]
        public void Init()
        {
            _engine = CreateEngine();

        }

        TestEngine CreateEngine()
        {
            var engine = new TestEngine();
            engine.AddEntryScript(new string[]
            {
                "../../../../../templates/Template.NEP5.CSharp/NEP5.cs",
                "../../../../../templates/Template.NEP5.CSharp/NEP5.Admin.cs",
                "../../../../../templates/Template.NEP5.CSharp/NEP5.Crowdsale.cs",
                "../../../../../templates/Template.NEP5.CSharp/NEP5.Helpers.cs",
                "../../../../../templates/Template.NEP5.CSharp/NEP5.Methods.cs"
            });

            return engine;
        }

        [TestMethod]
        public void Test_name()
        {
            var result = _engine.ExecuteTestCaseStandard("name");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("Token Name", item.GetString());
        }

        [TestMethod]
        public void Test_symbol()
        {
            var result = _engine.ExecuteTestCaseStandard("symbol");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("TokenSymbol", item.GetString());
        }

        [TestMethod]
        public void Test_decimals()
        {
            var result = _engine.ExecuteTestCaseStandard("decimals");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(8, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_totalSupply()
        {
            var engine = CreateEngine();
            var hash = engine.CurrentScriptHash;
            var snapshot = engine.Snapshot as TestSnapshot;

            snapshot.Contracts.Add(hash, new Neo.Ledger.ContractState()
            {
                Manifest = new Neo.SmartContract.Manifest.ContractManifest()
                {
                    Features = Neo.SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

            snapshot.Storages.Add(new Neo.Ledger.StorageKey()
            {
                Id = 0,
                Key = _prefixContract.Concat(Encoding.ASCII.GetBytes("totalSupply")).ToArray()
            },
            new Neo.Ledger.StorageItem()
            {
                IsConstant = false,
                Value = new BigInteger(123).ToByteArray()
            });

            var result = engine.ExecuteTestCaseStandard("totalSupply");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual(123, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_totalSupply_empty()
        {
            var engine = CreateEngine();
            var hash = engine.CurrentScriptHash;
            var snapshot = engine.Snapshot as TestSnapshot;

            snapshot.Contracts.Add(hash, new Neo.Ledger.ContractState()
            {
                Manifest = new Neo.SmartContract.Manifest.ContractManifest()
                {
                    Features = Neo.SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

            var result = engine.ExecuteTestCaseStandard("totalSupply");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));
        }

        [TestMethod]
        public void Test_balanceOf()
        {
            var engine = CreateEngine();
            var hash = engine.CurrentScriptHash;
            var snapshot = engine.Snapshot as TestSnapshot;
            var address = new byte[] { 0xf6, 0x64, 0x43, 0x49, 0x8d, 0x38, 0x78, 0xd3, 0x2b, 0x99, 0x4e, 0x4e, 0x12, 0x83, 0xc6, 0x93, 0x44, 0x21, 0xda, 0xfe };

            snapshot.Contracts.Add(hash, new Neo.Ledger.ContractState()
            {
                Manifest = new Neo.SmartContract.Manifest.ContractManifest()
                {
                    Features = Neo.SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

            snapshot.Storages.Add(new Neo.Ledger.StorageKey()
            {
                Id = 0,
                Key = _prefixBalance.Concat(address).ToArray()
            },
            new Neo.Ledger.StorageItem()
            {
                IsConstant = false,
                Value = new BigInteger(321).ToByteArray()
            });

            var result = engine.ExecuteTestCaseStandard("balanceOf", address);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual(321, item.GetBigInteger());
        }

        [TestMethod]
        public void Test_balanceOf_empty()
        {
            var engine = CreateEngine();
            var hash = engine.CurrentScriptHash;
            var snapshot = engine.Snapshot as TestSnapshot;
            var address = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x11, 0x12, 0x13 };

            snapshot.Contracts.Add(hash, new Neo.Ledger.ContractState()
            {
                Manifest = new Neo.SmartContract.Manifest.ContractManifest()
                {
                    Features = Neo.SmartContract.Manifest.ContractFeatures.HasStorage
                }
            });

            var result = engine.ExecuteTestCaseStandard("balanceOf", address);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));
        }

        [TestMethod]
        public void Test_supportedStandards()
        {
            var result = _engine.ExecuteTestCaseStandard("supportedStandards");
            Assert.AreEqual(1, result.Count);

            var item = (Array)result.Pop();
            Assert.IsInstanceOfType(item, typeof(Array));
            Assert.AreEqual("NEP-5", item[0].GetString());
            Assert.AreEqual("NEP-10", item[1].GetString());
        }
    }
}
