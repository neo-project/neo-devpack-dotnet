// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using System.Collections.Generic;
using System.Linq;

namespace Neo.TestingEngine
{
    public class TestDataCache : DataCache
    {
        private readonly Dictionary<StorageKey, StorageItem> dict = new();

        public TestDataCache(Block persistingBlock = null)
        {
            this.DeployNativeContracts(persistingBlock);
        }

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
            if (!dict.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
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

        /// <summary>
        /// Include a new value to the storage for unit test
        /// </summary>
        public void AddForTest(StorageKey key, StorageItem value)
        {
            if (Contains(key))
            {
                Delete(key);
            }
            Add(key, value);
        }
    }
}
