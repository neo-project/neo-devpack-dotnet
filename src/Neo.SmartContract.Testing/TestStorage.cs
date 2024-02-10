using Neo.Json;
using Neo.Persistence;
using System;
using System.Linq;

namespace Neo.SmartContract.Testing
{
    public class TestStorage
    {
        /// <summary>
        /// Store
        /// </summary>
        public IStore Store { get; init; } = new MemoryStore();

        /// <summary>
        /// Snapshot
        /// </summary>
        public ISnapshot Snapshot { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage">Store</param>
        public TestStorage(IStore store)
        {
            Store = store;
            Snapshot = Store.GetSnapshot();
        }

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            Snapshot.Commit();
            Snapshot = Store.GetSnapshot();
        }

        /// <summary>
        /// Rollback
        /// </summary>
        public void Rollback()
        {
            Snapshot.Dispose();
            Snapshot = Store.GetSnapshot();
        }

        /// <summary>
        /// Load data from json
        ///
        /// Expected data (in base64):
        /// 
        /// - "key":"value"
        /// - "prefix": { "key":"value" }
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

                    Snapshot.Put(Convert.FromBase64String(entry.Key), Convert.FromBase64String(str.Value));
                }
                else if (entry.Value is JObject obj)
                {
                    // "prefix": { "key":"value" }  in base64

                    foreach (var subEntry in obj.Properties)
                    {
                        if (subEntry.Value is JString subStr)
                        {
                            Snapshot.Put(Convert.FromBase64String(entry.Key).Concat(Convert.FromBase64String(subEntry.Key)).ToArray(),
                                Convert.FromBase64String(subStr.Value));
                        }
                    }
                }
            }
        }
    }
}
