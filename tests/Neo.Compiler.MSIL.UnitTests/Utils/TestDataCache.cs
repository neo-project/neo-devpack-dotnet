using Neo.IO;
using Neo.IO.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class TestDataCache : DataCache
    {
        private readonly Dictionary<StorageKey, StorageItem> dict = new Dictionary<StorageKey, StorageItem>();

        protected override void AddInternal(StorageKey key, StorageItem value)
        {
            dict.Add(key, value);
        }

        protected override void DeleteInternal(StorageKey key)
        {
            dict.Remove(key);
        }

        protected override bool ContainsInternal(StorageKey key)
        {
            return dict.ContainsKey(key);
        }

        protected override StorageItem GetInternal(StorageKey key)
        {
            return dict[key];
        }


        protected override IEnumerable<(StorageKey Key, StorageItem Value)> SeekInternal(byte[] keyOrPrefix, SeekDirection direction)
        {
            return dict.Select(u => (u.Key, u.Value));
        }

        protected override StorageItem TryGetInternal(StorageKey key)
        {
            return dict.TryGetValue(key, out var value) ? value : null;
        }

        protected override void UpdateInternal(StorageKey key, StorageItem value)
        {
            dict[key] = value;
        }
    }
}
