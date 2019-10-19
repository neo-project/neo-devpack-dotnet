using Neo.IO.Caching;
using Neo.IO.Wrappers;
using Neo.Ledger;
using Neo.Persistence;

namespace Neo.Compiler.MSIL.Utils
{
    public class TestSnapshot : Snapshot
    {
        private DataCache<UInt256, TrimmedBlock> _Blocks = new TestDataCache<UInt256, TrimmedBlock>();
        private DataCache<UInt256, TransactionState> _Transactions = new TestDataCache<UInt256, TransactionState>();
        private DataCache<UInt160, ContractState> _Contracts = new TestDataCache<UInt160, ContractState>();
        private DataCache<StorageKey, StorageItem> _Storages = new TestDataCache<StorageKey, StorageItem>();
        private DataCache<UInt32Wrapper, HeaderHashList> _HeaderHashList = new TestDataCache<UInt32Wrapper, HeaderHashList>();
        private MetaDataCache<HashIndexState> _BlockHashIndex = new TestMetaDataCache<HashIndexState>();
        private MetaDataCache<HashIndexState> _HeaderHashIndex = new TestMetaDataCache<HashIndexState>();

        public override DataCache<UInt256, TrimmedBlock> Blocks => _Blocks;

        public override DataCache<UInt256, TransactionState> Transactions => _Transactions;

        public override DataCache<UInt160, ContractState> Contracts => _Contracts;

        public override DataCache<StorageKey, StorageItem> Storages => _Storages;

        public override DataCache<UInt32Wrapper, HeaderHashList> HeaderHashList => _HeaderHashList;

        public override MetaDataCache<HashIndexState> BlockHashIndex => _BlockHashIndex;

        public override MetaDataCache<HashIndexState> HeaderHashIndex => _HeaderHashIndex;
    }
}
