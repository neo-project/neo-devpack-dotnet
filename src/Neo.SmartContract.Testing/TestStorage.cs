using Neo.Json;
using Neo.Persistence;
using System;
using System.Buffers.Binary;
using System.Linq;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// TestStorage centralizes the storage management of our TestEngine
    /// </summary>
    public class TestStorage
    {
        /// <summary>
        /// Store
        /// </summary>
        public IStore Store { get; init; } = new MemoryStore();

        /// <summary>
        /// Snapshot
        /// </summary>
        public SnapshotCache Snapshot { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage">Store</param>
        public TestStorage(IStore store)
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
        /// Load data from json, expected data (in base64):
        /// - "key"     : "value"
        /// - "prefix"  : { "key":"value" }
        /// - "123"     : { "key":"value" }
        /// </summary>
        /// <param name="snapshot">Snapshot to be used</param>
        /// <param name="json">Json Object</param>
        public void LoadFromJson(JObject json)
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

                    byte[] prefix;

                    try
                    {
                        prefix = Convert.FromBase64String(entry.Key);
                    }
                    catch
                    {
                        // It's a number?

                        prefix = new byte[sizeof(int)];
                        BinaryPrimitives.WriteInt32LittleEndian(prefix, int.Parse(entry.Key));
                    }

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
    }
}
