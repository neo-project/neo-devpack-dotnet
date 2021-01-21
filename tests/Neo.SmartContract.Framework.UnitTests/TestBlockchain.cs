using System.Linq;
using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Native;

namespace Neo.SmartContract.Framework.UnitTests
{
    public static class TestBlockchain
    {
        public static readonly NeoSystem TheNeoSystem;

        const byte Prefix_Block = 5;
        const byte Prefix_BlockHash = 9;
        const byte Prefix_Transaction = 11;
        const byte Prefix_CurrentBlock = 12;

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

        public static void BlocksAdd(this DataCache snapshot, UInt256 hash, TrimmedBlock block)
        {
            snapshot.Add(NativeContract.Ledger.CreateStorageKey(Prefix_BlockHash, block.Index), new StorageItem(hash.ToArray(), true));
            snapshot.Add(NativeContract.Ledger.CreateStorageKey(Prefix_Block, hash), new StorageItem(block.ToArray(), true));
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
    }
}
