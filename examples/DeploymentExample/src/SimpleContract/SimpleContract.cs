using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace DeploymentExample.Contracts
{
    [DisplayName("SimpleContract")]
    [ManifestExtra("Author", "Neo Deployment Example")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "A simple smart contract demonstrating basic functionality")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    public class SimpleContract : SmartContract
    {
        // Contract owner for administrative functions
        private static readonly UInt160 Owner = "NiHURp5SrPnxKHVQNvpDcPVHZnCUXn3w7G".ToScriptHash();

        // Storage prefixes
        private const byte Prefix_Data = 0x01;
        private const byte Prefix_Counter = 0x02;
        private const byte Prefix_Config = 0x03;

        // Access control
        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }

        // Deploy event
        [DisplayName("Deployed")]
        public static event Action<UInt160> OnDeployed;

        // Data storage events
        [DisplayName("DataStored")]
        public static event Action<string, string> OnDataStored;

        [DisplayName("DataDeleted")]
        public static event Action<string> OnDataDeleted;

        // Counter event
        [DisplayName("CounterIncremented")]
        public static event Action<BigInteger> OnCounterIncremented;

        // Configuration event
        [DisplayName("ConfigurationUpdated")]
        public static event Action<string, object> OnConfigurationUpdated;

        // Contract deployment
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;

            // Initialize contract
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Counter }, 0);
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Config } + "version".ToByteArray(), "1.0.0");
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Config } + "initialized".ToByteArray(), Runtime.Time);

            OnDeployed(Owner);
        }

        // Store data with key-value pairs
        [DisplayName("storeData")]
        public static void StoreData(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty");

            if (key.Length > 64)
                throw new Exception("Key too long (max 64 characters)");

            var storageKey = new byte[] { Prefix_Data } + key.ToByteArray();
            Storage.Put(Storage.CurrentContext, storageKey, value);

            OnDataStored(key, value);
        }

        // Retrieve stored data
        [DisplayName("getData")]
        [Safe]
        public static string GetData(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            var storageKey = new byte[] { Prefix_Data } + key.ToByteArray();
            return Storage.Get(Storage.CurrentContext, storageKey) ?? "";
        }

        // Delete stored data
        [DisplayName("deleteData")]
        public static void DeleteData(string key)
        {
            RequireOwner();

            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty");

            var storageKey = new byte[] { Prefix_Data } + key.ToByteArray();
            Storage.Delete(Storage.CurrentContext, storageKey);

            OnDataDeleted(key);
        }

        // Check if data exists
        [DisplayName("hasData")]
        [Safe]
        public static bool HasData(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            var storageKey = new byte[] { Prefix_Data } + key.ToByteArray();
            return Storage.Get(Storage.CurrentContext, storageKey) != null;
        }

        // Increment counter
        [DisplayName("incrementCounter")]
        public static BigInteger IncrementCounter()
        {
            var counterKey = new byte[] { Prefix_Counter };
            var currentValue = (BigInteger)Storage.Get(Storage.CurrentContext, counterKey);
            var newValue = currentValue + 1;

            Storage.Put(Storage.CurrentContext, counterKey, newValue);
            OnCounterIncremented(newValue);

            return newValue;
        }

        // Get current counter value
        [DisplayName("getCounter")]
        [Safe]
        public static BigInteger GetCounter()
        {
            var counterKey = new byte[] { Prefix_Counter };
            return (BigInteger)Storage.Get(Storage.CurrentContext, counterKey);
        }

        // Set configuration value (owner only)
        [DisplayName("setConfig")]
        public static void SetConfig(string key, object value)
        {
            RequireOwner();

            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty");

            var configKey = new byte[] { Prefix_Config } + key.ToByteArray();
            Storage.Put(Storage.CurrentContext, configKey, StdLib.Serialize(value));

            OnConfigurationUpdated(key, value);
        }

        // Get configuration value
        [DisplayName("getConfig")]
        [Safe]
        public static object GetConfig(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            var configKey = new byte[] { Prefix_Config } + key.ToByteArray();
            var data = Storage.Get(Storage.CurrentContext, configKey);

            if (data == null || data.Length == 0)
                return "";

            return StdLib.Deserialize(data);
        }

        // Get all stored keys (pagination support)
        [DisplayName("listKeys")]
        [Safe]
        public static string[] ListKeys(int offset = 0, int limit = 10)
        {
            if (offset < 0) offset = 0;
            if (limit <= 0 || limit > 100) limit = 10;

            var keys = new System.Collections.Generic.List<string>();
            var iterator = Storage.Find(Storage.CurrentContext, new byte[] { Prefix_Data }, FindOptions.KeysOnly | FindOptions.RemovePrefix);

            int currentIndex = 0;
            while (iterator.Next() && keys.Count < limit)
            {
                if (currentIndex >= offset)
                {
                    keys.Add(iterator.Value);
                }
                currentIndex++;
            }

            return keys.ToArray();
        }

        // Calculate storage usage
        [DisplayName("getStorageUsage")]
        [Safe]
        public static Map<string, int> GetStorageUsage()
        {
            var usage = new Map<string, int>();

            // Count data entries
            var dataCount = 0;
            var dataIterator = Storage.Find(Storage.CurrentContext, new byte[] { Prefix_Data }, FindOptions.KeysOnly);
            while (dataIterator.Next())
            {
                dataCount++;
            }

            // Count config entries
            var configCount = 0;
            var configIterator = Storage.Find(Storage.CurrentContext, new byte[] { Prefix_Config }, FindOptions.KeysOnly);
            while (configIterator.Next())
            {
                configCount++;
            }

            usage["data"] = dataCount;
            usage["config"] = configCount;
            usage["total"] = dataCount + configCount + 1; // +1 for counter

            return usage;
        }

        // Batch operations
        [DisplayName("batchStore")]
        public static void BatchStore(Map<string, string> items)
        {
            if (items.Count > 10)
                throw new Exception("Too many items in batch (max 10)");

            foreach (var item in items)
            {
                StoreData(item.Key, item.Value);
            }
        }

        // Get contract info
        [DisplayName("getInfo")]
        [Safe]
        public static Map<string, object> GetInfo()
        {
            var info = new Map<string, object>();
            info["name"] = "SimpleContract";
            info["version"] = GetConfig("version");
            info["owner"] = Owner;
            info["deployTime"] = GetConfig("initialized");
            info["counter"] = GetCounter();

            return info;
        }

        // Update contract
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            RequireOwner();
            ContractManagement.Update(nefFile, manifest, data);
        }

        // Destroy contract
        [DisplayName("destroy")]
        public static void Destroy()
        {
            RequireOwner();
            ContractManagement.Destroy();
        }

        // Verification method
        public static bool Verify() => Runtime.CheckWitness(Owner);
    }
}