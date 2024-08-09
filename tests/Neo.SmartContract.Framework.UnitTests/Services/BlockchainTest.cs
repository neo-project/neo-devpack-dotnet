using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class BlockchainTest : DebugAndTestBase<Contract_Blockchain>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Block _block;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void Init()
        {
            var tx = new Transaction()
            {
                Attributes = [],
                Signers =
                [
                    new()
                    {
                        Account = UInt160.Zero,
                        AllowedContracts = [],
                        AllowedGroups = [],
                        Rules = [],
                        Scopes = WitnessScope.Global
                    }
                ],
                Witnesses = [],
                Script = System.Array.Empty<byte>()
            };

            _block = Engine.PersistingBlock.Persist(tx, VMState.HALT);
        }

        [TestMethod]
        public void Test_GetTxVMState()
        {
            // None

            Assert.AreEqual((int)VMState.NONE, Contract.GetTxVMState(UInt256.Zero));

            // Hash

            var tx = _block.Transactions[0];
            Assert.AreEqual((int)VMState.HALT, Contract.GetTxVMState(tx.Hash));
        }

        [TestMethod]
        public void Test_GetHeight()
        {
            Assert.AreEqual(_block.Index, Contract.GetHeight());
        }

        [TestMethod]
        public void Test_GetTransactionHeight()
        {
            // Not found

            Assert.AreEqual(BigInteger.MinusOne, Contract.GetTransactionHeight(UInt256.Zero));

            // Found

            Assert.AreEqual(_block.Index, Contract.GetTransactionHeight(_block.Transactions[0].Hash));
        }

        [TestMethod]
        public void Test_GetBlockByHash()
        {
            Test_GetBlock(Contract.GetBlockByHash, _block.Hash, UInt256.Parse("0x0000000000000000000000000000000000000000000000000000000000000001"));
        }

        [TestMethod]
        public void Test_GetBlockByIndex()
        {
            Test_GetBlock((a, b) => Contract.GetBlockByIndex(a, b), new BigInteger(_block.Index), new BigInteger(_block.Index + 100));
        }

        public void Test_GetBlock<T>(Func<T?, string?, object?> method, T foundArg, T notFoundArg)
        {
            // Not found

            Assert.IsNull(method(notFoundArg, ""));

            // Hash

            CollectionAssert.AreEqual(_block.Hash.ToArray(), (method(foundArg, "Hash") as StackItem)!.GetSpan().ToArray());

            // Index

            Assert.AreEqual(_block.Index, (method(foundArg, "Index") as StackItem)!.GetInteger());

            // MerkleRoot

            CollectionAssert.AreEqual(_block.MerkleRoot.ToArray(), (method(foundArg, "MerkleRoot") as StackItem)!.GetSpan().ToArray());

            // NextConsensus

            CollectionAssert.AreEqual(_block.NextConsensus.ToArray(), (method(foundArg, "NextConsensus") as StackItem)!.GetSpan().ToArray());

            // PrevHash

            CollectionAssert.AreEqual(_block.PrevHash.ToArray(), (method(foundArg, "PrevHash") as StackItem)!.GetSpan().ToArray());

            // Timestamp

            Assert.AreEqual(_block.Timestamp, (method(foundArg, "Timestamp") as StackItem)!.GetInteger());

            // TransactionsCount

            Assert.AreEqual(_block.Transactions.Length, (method(foundArg, "TransactionsCount") as StackItem)!.GetInteger());

            // Version

            Assert.AreEqual(_block.Version, (method(foundArg, "Version") as StackItem)!.GetInteger());

            // Uknown property

            Assert.ThrowsException<TestException>(() => method(foundArg, "¿...?"));
        }

        [TestMethod]
        public void Test_GetTxByHash()
        {
            Test_GetTransaction(
                a => Contract.GetTxByHash(_block.Transactions[0].Hash, a),
                a => Contract.GetTxByHash(UInt256.Zero, a),
                true);
        }

        [TestMethod]
        public void Test_GetTxByBlockIndex()
        {
            Test_GetTransaction(
                a => Contract.GetTxByBlockIndex(_block.Index, 0, a),
                a => Contract.GetTxByBlockIndex(_block.Index, _block.Transactions.Length + 1, a),
                false);
        }

        [TestMethod]
        public void Test_GetTxByBlockHash()
        {
            Test_GetTransaction(
               a => Contract.GetTxByBlockHash(_block.Hash, 0, a),
               a => Contract.GetTxByBlockHash(_block.Hash, _block.Transactions.Length + 1, a),
               false);
        }

        public void Test_GetTransaction(Func<string, object?> found, Func<string, object?> notFoundArg, bool expectedNullAsNotFound)
        {
            // Not found

            if (expectedNullAsNotFound)
            {
                Assert.IsNull(notFoundArg(""));
            }
            else
            {
                Assert.ThrowsException<TestException>(() => found(""));
            }

            var tx = _block.Transactions[0];

            // Hash

            CollectionAssert.AreEqual(tx.Hash.ToArray(), (found("Hash") as StackItem)!.GetSpan().ToArray());

            // NetworkFee

            Assert.AreEqual(tx.NetworkFee, (found("NetworkFee") as StackItem)!.GetInteger());

            // Nonce

            Assert.AreEqual(tx.Nonce, (found("Nonce") as StackItem)!.GetInteger());

            // SystemFee

            Assert.AreEqual(tx.SystemFee, (found("SystemFee") as StackItem)!.GetInteger());

            // ValidUntilBlock

            Assert.AreEqual(tx.ValidUntilBlock, (found("ValidUntilBlock") as StackItem)!.GetInteger());

            // Version

            Assert.AreEqual(tx.Version, (found("Version") as StackItem)!.GetInteger());

            // Script

            CollectionAssert.AreEqual(tx.Script.ToArray(), (found("Script") as StackItem)!.GetSpan().ToArray());

            // Sender

            CollectionAssert.AreEqual(tx.Sender.ToArray(), (found("Sender") as StackItem)!.GetSpan().ToArray());

            // Signers

            var item = found("Signers") as VM.Types.Array;
            Assert.IsNotNull(item);

            Assert.AreEqual(1, item.Count);
            Assert.AreEqual(5, (item[0] as VM.Types.Array)!.Count);

            // First scope

            Assert.AreEqual((int)WitnessScope.Global, (found("FirstScope") as StackItem)!.GetInteger());
        }

        [TestMethod]
        public void GetContract()
        {
            // Not found

            Assert.IsNull(Contract.GetContract(UInt160.Zero, ""));

            // Found + Manifest

            var item = Contract.GetContract(Contract.Hash, "Manifest") as Struct;
            Assert.IsNotNull(item);

            var ritem = new ContractManifest();
            ((IInteroperable)ritem).FromStackItem(item);
            Assert.AreEqual(Contract_Blockchain.Manifest.ToJson().ToString(), ritem.ToJson().ToString());

            // Found + UpdateCounter

            Assert.AreEqual(0, ((StackItem)Contract.GetContract(Contract.Hash, "UpdateCounter")!).GetInteger());

            // Found + Id

            Assert.AreEqual(1, ((StackItem)Contract.GetContract(Contract.Hash, "Id")!).GetInteger());

            // Found + Hash

            CollectionAssert.AreEqual(Contract.Hash.ToArray(), (Contract.GetContract(Contract.Hash, "Hash") as StackItem)!.GetSpan().ToArray());

            // Found + Uknown property

            Assert.ThrowsException<TestException>(() => Contract.GetContract(Contract.Hash, "¿..?"));
        }
    }
}
