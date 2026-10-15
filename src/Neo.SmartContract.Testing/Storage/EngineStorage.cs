// Copyright (C) 2015-2026 The Neo Project.
//
// EngineStorage.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using Neo.Persistence;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Neo.SmartContract.Testing.Storage
{
    /// <summary>
    /// TestStorage centralizes the storage management of our TestEngine
    /// </summary>
    public class EngineStorage
    {
        // Key to check if native contracts are initialized, by default: ContractManagement.Prefix_NextAvailableId
        private static readonly StorageKey _initKey = new() { Id = Neo.SmartContract.Native.NativeContract.ContractManagement.Id, Key = new byte[] { 15 } };

        /// <summary>
        /// Store
        /// </summary>
        public IStore Store { get; }

        /// <summary>
        /// Snapshot
        /// </summary>
        public DataCache Snapshot { get; private set; }

        /// <summary>
        /// Return true if native contract are initialized
        /// </summary>
        public bool IsInitialized => Snapshot.Contains(_initKey);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">Store</param>
        public EngineStorage(IStore store) : this(store, new StoreCache(store.GetSnapshot())) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">Store</param>
        /// <param name="snapshotCache">Snapshot cache</param>
        internal EngineStorage(IStore store, DataCache snapshotCache)
        {
            Store = store;
            Snapshot = snapshotCache;
        }

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            Snapshot.Commit();
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {
            if (Snapshot is IDisposable sp)
            {
                sp.Dispose();
            }
            Snapshot = new StoreCache(Store.GetSnapshot());
        }

        /// <summary>
        /// Get storage checkpoint
        /// </summary>
        /// <returns>EngineCheckpoint</returns>
        public EngineCheckpoint Checkpoint() => new(Snapshot);

        /// <summary>
        /// Restore
        /// </summary>
        /// <param name="checkpoint">Checkpoint</param>
        public void Restore(EngineCheckpoint checkpoint)
        {
            checkpoint.Restore(Snapshot);
        }

        /// <summary>
        /// Import data from json, expected data (in base64):
        /// - "key"     : "value"
        /// - "prefix"  : { "key":"value" }
        /// </summary>
        /// <param name="json">Json Object</param>
        public void Import(string json)
        {
            if (JToken.Parse(json) is not JObject jo)
            {
                throw new FormatException("The json is not a valid JObject");
            }

            Import(jo);
        }

        /// <summary>
        /// Import data from json, expected data (in base64):
        /// - "key"     : "value"
        /// - "prefix"  : { "key":"value" }
        /// </summary>
        /// <param name="json">Json Object</param>
        public void Import(JObject json)
        {
            var entries = ParseImport(json);
            foreach (var (key, _) in entries)
                if (Snapshot.Contains(key)) throw new ArgumentException("An imported storage key already exists.", nameof(json));
            foreach (var (key, value) in entries)
                Snapshot.Add(key, value);
        }

        internal static IReadOnlyList<(StorageKey Key, StorageItem Value)> ParseImport(JObject json)
        {
            var entries = new List<(StorageKey, StorageItem)>();
            var keys = new HashSet<StorageKey>();
            foreach (var entry in json.Properties)
            {
                var prefix = Convert.FromBase64String(entry.Key);
                if (entry.Value is JString value)
                {
                    Add(prefix, value);
                }
                else if (entry.Value is JObject group)
                {
                    foreach (var item in group.Properties)
                    {
                        if (item.Value is not JString itemValue)
                            throw new FormatException("Storage values must be base64 strings.");
                        Add([.. prefix, .. Convert.FromBase64String(item.Key)], itemValue);
                    }
                }
                else
                {
                    throw new FormatException("Storage entries must be base64 strings or objects.");
                }
            }
            return entries;

            void Add(byte[] bytes, JString value)
            {
                if (bytes.Length < sizeof(int))
                    throw new FormatException("A storage key must include a contract identifier.");
                var key = new StorageKey(bytes);
                if (!keys.Add(key)) throw new FormatException("Duplicate imported storage key.");
                entries.Add((key, new StorageItem(Convert.FromBase64String(value.Value))));
            }
        }

        /// <summary>
        /// Export data to json
        /// </summary>
        public JObject Export()
        {
            var buffer = new byte[sizeof(int)];
            JObject ret = new();

            foreach ((var key, var value) in Snapshot.Seek([], SeekDirection.Forward))
            {
                // "key":"value" in base64

                JObject prefix;
                BinaryPrimitives.WriteInt32LittleEndian(buffer, key.Id);
                var keyId = Convert.ToBase64String(buffer);

                if (ret.ContainsProperty(keyId))
                {
                    prefix = (JObject)ret[keyId]!;
                }
                else
                {
                    prefix = new();
                    ret[keyId] = prefix;
                }

                prefix[Convert.ToBase64String(key.Key.ToArray())] = Convert.ToBase64String(value.Value.ToArray());
            }

            return ret;
        }
    }
}
