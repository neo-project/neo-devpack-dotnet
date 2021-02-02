using System.Collections.Generic;
using System.Linq;
using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;

namespace Neo.TestingEngine
{
    public static class TestBlockchain
    {
        public static readonly NeoSystem TheNeoSystem;

        const byte Prefix_Block = 5;
        const byte Prefix_BlockHash = 9;
        const byte Prefix_Transaction = 11;
        const byte Prefix_CurrentBlock = 12;

        const uint MaxStackSize = 2 * 1024;
        const uint MaxItemSize = 1024 * 1024;

        static TestBlockchain()
        {
            TheNeoSystem = new NeoSystem();

            // Ensure that blockchain is loaded

            var _ = Blockchain.Singleton;
        }

        public static StorageKey CreateStorageKey(this NativeContract contract, byte prefix, ISerializable key = null)
        {
            var k = new KeyBuilder(contract.Id, prefix);
            if (key != null) k = k.Add(key);
            return k;
        }

        public static StorageKey CreateStorageKey(this NativeContract contract, byte prefix, uint value)
        {
            return new KeyBuilder(contract.Id, prefix).AddBigEndian(value);
        }

        public static void BlocksDelete(this DataCache snapshot, UInt256 hash)
        {
            snapshot.Delete(NativeContract.Ledger.CreateStorageKey(Prefix_BlockHash, hash));
            snapshot.Delete(NativeContract.Ledger.CreateStorageKey(Prefix_Block, hash));
        }

        public static void TransactionAdd(this DataCache snapshot, params TransactionState[] txs)
        {
            foreach (TransactionState tx in txs)
            {
                snapshot.Add(NativeContract.Ledger.CreateStorageKey(Prefix_Transaction, tx.Transaction.Hash), new StorageItem(tx, true));
            }
        }

        public static bool ContainsTransaction(this DataCache snapshot, Transaction tx)
        {
            var key = NativeContract.Ledger.CreateStorageKey(Prefix_Transaction, tx.Hash);
            return snapshot.Contains(key);
        }

        public static void BlocksAdd(this DataCache snapshot, UInt256 hash, TrimmedBlock block)
        {
            snapshot.Add(NativeContract.Ledger.CreateStorageKey(Prefix_BlockHash, block.Index), new StorageItem(hash.ToArray(), true));
            snapshot.Add(NativeContract.Ledger.CreateStorageKey(Prefix_Block, hash), new StorageItem(block.ToArray(), true));
        }

        public static void UpdateChangedBlocks(this DataCache snapshot, UInt256 oldHash, UInt256 newHash, TrimmedBlock block)
        {
            if (snapshot.Contains(NativeContract.Ledger.CreateStorageKey(Prefix_BlockHash, oldHash)))
            {
                snapshot.BlocksDelete(oldHash);
            }
            snapshot.BlocksAdd(newHash, block);
        }

        public static TrimmedBlock Trim(this Block block)
        {
            return new TrimmedBlock
            {
                Version = block.Version,
                PrevHash = block.PrevHash,
                MerkleRoot = block.MerkleRoot,
                Timestamp = block.Timestamp,
                Index = block.Index,
                NextConsensus = block.NextConsensus,
                Witness = block.Witness,
                Hashes = block.Transactions.Select(p => p.Hash).Prepend(block.ConsensusData.Hash).ToArray(),
                ConsensusData = block.ConsensusData
            };
        }

        public static Block TryGetBlock(this DataCache snapshot, uint index)
        {
            return NativeContract.Ledger.GetBlock(snapshot, index);
        }

        public static List<TrimmedBlock> Blocks(this DataCache snapshot)
        {
            var blockKey = NativeContract.Ledger.CreateStorageKey(Prefix_Block);
            return snapshot.Find(blockKey.ToArray())
                .Select(item => item.Value.Value.AsSerializable<TrimmedBlock>())
                .OrderBy(b => b.Index).ToList();
        }

        public static Block GetLastBlock(this DataCache snapshot)
        {
            var trim = snapshot.Blocks().Last();
            return NativeContract.Ledger.GetBlock(snapshot, trim.Hash);
        }

        public static void SetCurrentBlockHash(this DataCache snapshot, uint index, UInt256 hash)
        {
            var key = NativeContract.Ledger.CreateStorageKey(Prefix_CurrentBlock);
            var item = snapshot.GetAndChange(key, () => new StorageItem());

            var hashIndex = new TestHashIndexState();
            var stack = BinarySerializer.Deserialize(item.Value, MaxStackSize, MaxItemSize);
            hashIndex.FromStackItem(stack);
            hashIndex.Hash = hash;
            hashIndex.Index = index;

            item.Value = BinarySerializer.Serialize(hashIndex.ToStackItem(null), MaxStackSize);
        }

        public static void AddTransactions(this DataCache snapshot, Transaction[] txs, int blockIndex = -1)
        {
            uint index = blockIndex >= 0 ? (uint)blockIndex : NativeContract.Ledger.CurrentIndex(snapshot);
            snapshot.AddTransactions(txs, index);
        }

        public static void AddTransactions(this DataCache snapshot, Transaction[] txs, uint index)
        {
            var states = new List<TransactionState>();
            foreach (var tx in txs)
            {
                var transaction = tx;
                if (snapshot.ContainsTransaction(tx))
                {
                    transaction = NativeContract.Ledger.GetTransaction(snapshot, tx.Hash);
                }

                var state = new TransactionState()
                {
                    BlockIndex = index,
                    Transaction = transaction
                };
                states.Add(state);
            }

            snapshot.TransactionAdd(states.ToArray());
        }
    }
}
