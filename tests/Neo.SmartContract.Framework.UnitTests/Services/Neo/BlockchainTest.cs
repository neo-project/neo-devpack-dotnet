using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO;
using Neo.Ledger;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class BlockchainTest
    {
        private Network.P2P.Payloads.Block _block;
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var _ = TestBlockchain.TheNeoSystem;
            var snapshot = Blockchain.Singleton.GetSnapshot().Clone();

            _block = new Network.P2P.Payloads.Block()
            {
                ConsensusData = new Network.P2P.Payloads.ConsensusData(),
                Index = 1,
                PrevHash = Blockchain.GenesisBlock.Hash,
                Witness = new Network.P2P.Payloads.Witness() { InvocationScript = new byte[0], VerificationScript = new byte[0] },
                NextConsensus = UInt160.Zero,
                MerkleRoot = UInt256.Zero,
                Transactions = new Network.P2P.Payloads.Transaction[]
                {
                     new Network.P2P.Payloads.Transaction()
                     {
                          Attributes = new Network.P2P.Payloads.TransactionAttribute[0],
                          Signers = new Network.P2P.Payloads.Signer[]{ new Network.P2P.Payloads.Signer() { Account = UInt160.Zero } },
                          Witnesses = new Network.P2P.Payloads.Witness[]{ },
                          Script = new byte[0]
                     }
                }
            };

            snapshot.BlockHashIndex.GetAndChange().Index = _block.Index;
            snapshot.BlockHashIndex.GetAndChange().Hash = _block.Hash;
            snapshot.Blocks.Add(_block.Hash, _block.Trim());
            snapshot.Transactions.Add(_block.Transactions[0].Hash, new TransactionState()
            {
                BlockIndex = _block.Index,
                Transaction = _block.Transactions[0],
                VMState = VMState.HALT
            });

            // Fake header_index

            var header_index = (List<UInt256>)Blockchain.Singleton.GetType()
                .GetField("header_index", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(Blockchain.Singleton);
            header_index.Add(_block.Hash);

            _engine = new TestEngine(snapshot: snapshot, persistingBlock: _block);
            _engine.AddEntryScript("./TestClasses/Contract_Blockchain.cs");
        }

        [TestCleanup]
        public void Clean()
        {
            // Revert header_index
            var header_index = (List<UInt256>)Blockchain.Singleton.GetType()
                .GetField("header_index", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(Blockchain.Singleton);
            header_index.RemoveAt(1);
        }

        [TestMethod]
        public void Test_GetHeight()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getHeight");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(_block.Index, item.GetInteger());
        }

        [TestMethod]
        public void Test_GetTransactionHeight()
        {
            // Not found

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getTransactionHeight", new VM.Types.ByteString(UInt256.Zero.ToArray()));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(BigInteger.MinusOne, item.GetInteger());

            // Found

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getTransactionHeight", new VM.Types.ByteString(_block.Transactions[0].Hash.ToArray()));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(_block.Index, item.GetInteger());
        }

        [TestMethod]
        public void Test_GetBlockByHash()
        {
            Test_GetBlock("getBlockByHash", new VM.Types.ByteString(_block.Hash.ToArray()), new VM.Types.ByteString(UInt256.Parse("0x0000000000000000000000000000000000000000000000000000000000000001").ToArray()));
        }

        [TestMethod]
        public void Test_GetTxByHash()
        {
            Test_GetTransaction("getTxByHash", new StackItem[] { new VM.Types.ByteString(_block.Transactions[0].Hash.ToArray()) },
                new StackItem[] { new VM.Types.ByteString(UInt256.Zero.ToArray()) },
                true);
        }

        [TestMethod]
        public void Test_GetTxByBlockIndex()
        {
            Test_GetTransaction("getTxByBlockIndex", new StackItem[] {
                new Integer(_block.Index), new Integer(0) },
                new StackItem[] { new Integer(_block.Index), new Integer(_block.Transactions.Length + 1) },
                false);
        }

        [TestMethod]
        public void Test_GetTxByBlockHash()
        {
            Test_GetTransaction("getTxByBlockHash", new StackItem[] {
                new VM.Types.ByteString(_block.Hash.ToArray()), new Integer(0) },
                new StackItem[] { new VM.Types.ByteString(_block.Hash.ToArray()), new Integer(_block.Transactions.Length + 1) },
                false);
        }

        public void Test_GetTransaction(string method, StackItem[] foundArgs, StackItem[] notFoundArgs, bool expectedNullAsNotFound)
        {
            // Not found

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard(method, Concat(notFoundArgs, new VM.Types.ByteString(new byte[0])));

            if (expectedNullAsNotFound)
            {
                Assert.AreEqual(VMState.HALT, _engine.State);
                Assert.AreEqual(1, result.Count);
                Assert.IsInstanceOfType(result.Pop(), typeof(Null));
            }
            else
            {
                Assert.AreEqual(VMState.FAULT, _engine.State);
                Assert.AreEqual(0, result.Count);
            }

            var tx = _block.Transactions[0];

            // Hash

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Hash"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(tx.Hash.ToArray(), item.GetSpan().ToArray());

            // NetworkFee

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("NetworkFee"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(tx.NetworkFee, item.GetInteger());

            // Nonce

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Nonce"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(tx.Nonce, item.GetInteger());

            // SystemFee

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("SystemFee"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(tx.SystemFee, item.GetInteger());

            // ValidUntilBlock

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("ValidUntilBlock"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(tx.ValidUntilBlock, item.GetInteger());

            // Version

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Version"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(tx.Version, item.GetInteger());

            // Script

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Script"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(tx.Script, item.GetSpan().ToArray());

            // Sender

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, Concat(foundArgs, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Sender"))));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(tx.Sender.ToArray(), item.GetSpan().ToArray());
        }

        private StackItem[] Concat(StackItem[] a, VM.Types.ByteString b)
        {
            return a.Concat(new StackItem[] { b }).ToArray();
        }

        [TestMethod]
        public void Test_GetBlockByIndex()
        {
            Test_GetBlock("getBlockByIndex", new Integer(_block.Index), new Integer(_block.Index + 100));
        }

        public void Test_GetBlock(string method, StackItem foundArg, StackItem notFoundArg)
        {
            // Not found

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard(method, notFoundArg, new VM.Types.ByteString(new byte[0]));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));

            // Hash

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Hash")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(_block.Hash.ToArray(), item.GetSpan().ToArray());

            // Index

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Index")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(_block.Index, item.GetInteger());

            // MerkleRoot

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("MerkleRoot")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(_block.MerkleRoot.ToArray(), item.GetSpan().ToArray());

            // NextConsensus

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("NextConsensus")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(_block.NextConsensus.ToArray(), item.GetSpan().ToArray());

            // PrevHash

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("PrevHash")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(_block.PrevHash.ToArray(), item.GetSpan().ToArray());

            // Timestamp

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Timestamp")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(_block.Timestamp, item.GetInteger());

            // TransactionsCount

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("TransactionsCount")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(_block.Transactions.Length, item.GetInteger());

            // Version

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Version")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(_block.Version, item.GetInteger());

            // Uknown property

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard(method, foundArg, new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("ASD")));
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void GetContract()
        {
            var contract = new ContractState()
            {
                Hash = new byte[] { 0x01, 0x02, 0x03 }.ToScriptHash(),
                Nef = new NefFile()
                {
                    Script = new byte[] { 0x01, 0x02, 0x03 },
                    Compiler = "neon",
                    Version = "test",
                    Tokens = System.Array.Empty<MethodToken>()
                },
                Manifest = new Manifest.ContractManifest()
                {
                    SupportedStandards = new string[0],
                    Groups = new Manifest.ContractGroup[0],
                    Trusts = Manifest.WildcardContainer<UInt160>.Create(),
                    Permissions = new Manifest.ContractPermission[0],
                    Abi = new Manifest.ContractAbi()
                    {
                        Methods = new Manifest.ContractMethodDescriptor[0],
                        Events = new Manifest.ContractEventDescriptor[0],
                    },
                }
            };
            _engine.Snapshot.ContractAdd(contract);

            // Not found

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("getContract", new VM.Types.ByteString(UInt160.Zero.ToArray()), new VM.Types.ByteString(new byte[20]));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Null));

            // Found + Manifest

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getContract", new VM.Types.ByteString(contract.Hash.ToArray()), new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Manifest")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            Assert.AreEqual(contract.Manifest.ToString(), item.GetString());

            // Found + UpdateCounter

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getContract", new VM.Types.ByteString(contract.Hash.ToArray()), new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("UpdateCounter")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Integer));
            Assert.AreEqual(0, item.GetInteger());

            // Found + Id

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getContract", new VM.Types.ByteString(contract.Hash.ToArray()), new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Id")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.Integer));
            Assert.AreEqual(0, item.GetInteger());

            // Found + Hash

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getContract", new VM.Types.ByteString(contract.Hash.ToArray()), new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("Hash")));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            CollectionAssert.AreEqual(contract.Hash.ToArray(), item.GetSpan().ToArray());

            // Found + Uknown property

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getContract", new VM.Types.ByteString(contract.Hash.ToArray()), new VM.Types.ByteString(Utility.StrictUTF8.GetBytes("ASD")));
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }
    }
}
