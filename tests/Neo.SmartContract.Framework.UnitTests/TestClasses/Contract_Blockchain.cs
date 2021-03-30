using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Blockchain : SmartContract
    {
        public static uint GetHeight()
        {
            return Ledger.CurrentIndex;
        }

        public static BigInteger GetTransactionHeight(UInt256 hash)
        {
            return Ledger.GetTransactionHeight(hash);
        }

        public static object GetBlockByHash(UInt256 hash, string whatReturn)
        {
            var block = Ledger.GetBlock(hash);
            return GetBlockInfo(block, whatReturn);
        }

        public static object GetBlockByIndex(uint index, string whatReturn)
        {
            var block = Ledger.GetBlock(index);
            return GetBlockInfo(block, whatReturn);
        }

        private static object GetBlockInfo(Block block, string whatReturn)
        {
            if (block == null)
            {
                Runtime.Log("NULL Block");
                return null;
            }

            if (whatReturn == "Hash") return block.Hash;
            if (whatReturn == "Index") return block.Index;
            if (whatReturn == "MerkleRoot") return block.MerkleRoot;
            if (whatReturn == "NextConsensus") return block.NextConsensus;
            if (whatReturn == "PrevHash") return block.PrevHash;
            if (whatReturn == "Timestamp") return block.Timestamp;
            if (whatReturn == "TransactionsCount") return block.TransactionsCount;
            if (whatReturn == "Version") return block.Version;

            throw new Exception("Uknown property");
        }

        public static object GetTxByHash(UInt256 hash, string whatReturn)
        {
            var tx = Ledger.GetTransaction(hash);
            return GetTxInfo(tx, whatReturn);
        }

        public static object GetTxByBlockHash(UInt256 blockHash, int txIndex, string whatReturn)
        {
            var tx = Ledger.GetTransactionFromBlock(blockHash, txIndex);
            return GetTxInfo(tx, whatReturn);
        }

        public static object GetTxByBlockIndex(uint blockIndex, int txIndex, string whatReturn)
        {
            var tx = Ledger.GetTransactionFromBlock(blockIndex, txIndex);
            return GetTxInfo(tx, whatReturn);
        }

        private static object GetTxInfo(Transaction tx, string whatReturn)
        {
            if (tx == null)
            {
                Runtime.Log("NULL Tx");
                return null;
            }

            if (whatReturn == "Hash") return tx.Hash;
            if (whatReturn == "NetworkFee") return tx.NetworkFee;
            if (whatReturn == "Nonce") return tx.Nonce;
            if (whatReturn == "Script") return tx.Script;
            if (whatReturn == "Sender") return tx.Sender;
            if (whatReturn == "SystemFee") return tx.SystemFee;
            if (whatReturn == "ValidUntilBlock") return tx.ValidUntilBlock;
            if (whatReturn == "Version") return tx.Version;

            throw new Exception("Uknown property");
        }

        public static object GetContract(UInt160 hash, string whatReturn)
        {
            var contract = ContractManagement.GetContract(hash);
            return GetContractInfo(contract, whatReturn);
        }

        private static object GetContractInfo(Contract contract, string whatReturn)
        {
            if (contract == null)
            {
                Runtime.Log("NULL contract");
                return null;
            }

            if (whatReturn == "Id") return contract.Id;
            if (whatReturn == "UpdateCounter") return contract.UpdateCounter;
            if (whatReturn == "Hash") return contract.Hash;
            if (whatReturn == "Manifest") return contract.Manifest;
            if (whatReturn == "Nef") return contract.Nef;

            throw new Exception("Uknown property");
        }
    }
}
