using System.Collections.Generic;
using Neo.IO;
using Neo.IO.Caching;
using Neo.Ledger;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class TestSnapshot : TestDataCache
    {
        //private DataCache<UInt256, TrimmedBlock> _Blocks = new TestDataCache<UInt256, TrimmedBlock>();
        //private DataCache<UInt256, TransactionState> _Transactions = new TestDataCache<UInt256, TransactionState>();
        //private DataCache<StorageKey, StorageItem> _Storages = new TestDataCache<StorageKey, StorageItem>();
        //private DataCache<SerializableWrapper<uint>, HeaderHashList> _HeaderHashList = new TestDataCache<SerializableWrapper<uint>, HeaderHashList>();
        //private MetaDataCache<HashIndexState> _BlockHashIndex = new TestMetaDataCache<HashIndexState>();
        //private MetaDataCache<HashIndexState> _HeaderHashIndex = new TestMetaDataCache<HashIndexState>();

        //public override DataCache<UInt256, TrimmedBlock> Blocks => _Blocks;

        //public override DataCache<UInt256, TransactionState> Transactions => _Transactions;

        //public override DataCache<StorageKey, StorageItem> Storages => _Storages;

        //public override DataCache<SerializableWrapper<uint>, HeaderHashList> HeaderHashList => _HeaderHashList;

        //public override MetaDataCache<HashIndexState> BlockHashIndex => _BlockHashIndex;

        //public override MetaDataCache<HashIndexState> HeaderHashIndex => _HeaderHashIndex;

        //protected override void AddInternal(StorageKey key, StorageItem value)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override void DeleteInternal(StorageKey key)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override bool ContainsInternal(StorageKey key)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override StorageItem GetInternal(StorageKey key)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override IEnumerable<(StorageKey Key, StorageItem Value)> SeekInternal(byte[] keyOrPrefix, SeekDirection direction)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override StorageItem TryGetInternal(StorageKey key)
        //{
        //    throw new System.NotImplementedException();
        //}

        //protected override void UpdateInternal(StorageKey key, StorageItem value)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
