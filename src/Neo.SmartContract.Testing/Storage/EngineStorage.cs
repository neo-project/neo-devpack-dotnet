using Neo.Json;
using Neo.Persistence;
using System;
using System.Buffers.Binary;
using System.Linq;

namespace Neo.SmartContract.Testing.Storage
{
    /// <summary>
    /// TestStorage centralizes the storage management of our TestEngine
    /// </summary>
    public class EngineStorage
    {
        // Key to check if native contracts are initialized, by default: Neo.votersCountPrefix
        private static readonly StorageKey _initKey = new() { Id = Native.NativeContract.NEO.Id, Key = new byte[] { 1 } };

        /// <summary>
        /// Store
        /// </summary>
        public IStore Store { get; }

        /// <summary>
        /// Snapshot
        /// </summary>
        public SnapshotCache Snapshot { get; private set; }

        /// <summary>
        /// Return true if native contract are initialized
        /// </summary>
        public bool IsInitialized => Snapshot.Contains(_initKey);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="store">Store</param>
        public EngineStorage(IStore store)
        {
            Store = store;
            Snapshot = new SnapshotCache(Store.GetSnapshot());
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
            Snapshot.Dispose();
            Snapshot = new SnapshotCache(Store.GetSnapshot());
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
            foreach (var entry in json.Properties)
            {
                if (entry.Value is JString str)
                {
                    // "key":"value" in base64

                    Snapshot.Add(new StorageKey(Convert.FromBase64String(entry.Key)), new StorageItem(Convert.FromBase64String(str.Value)));
                }
                else if (entry.Value is JObject obj)
                {
                    // "prefix": { "key":"value" }  in base64

                    byte[] prefix = Convert.FromBase64String(entry.Key);

                    foreach (var subEntry in obj.Properties)
                    {
                        if (subEntry.Value is JString subStr)
                        {
                            Snapshot.Add(
                                new StorageKey(prefix.Concat(Convert.FromBase64String(subEntry.Key)).ToArray()),
                                new StorageItem(Convert.FromBase64String(subStr.Value))
                                );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Export data to json
        /// </summary>
        public JObject Export()
        {
            var buffer = new byte[(sizeof(int))];
            JObject ret = new();

            foreach (var entry in Snapshot.Seek(Array.Empty<byte>(), SeekDirection.Forward))
            {
                // "key":"value" in base64

                JObject prefix;
                BinaryPrimitives.WriteInt32LittleEndian(buffer, entry.Key.Id);
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

                prefix[Convert.ToBase64String(entry.Key.Key.ToArray())] = Convert.ToBase64String(entry.Value.Value.ToArray());
            }

            return ret;
        }
    }
}
