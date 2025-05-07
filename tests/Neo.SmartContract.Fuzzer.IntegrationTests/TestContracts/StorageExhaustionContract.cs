using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Fuzzer.IntegrationTests.TestContracts
{
    [DisplayName("StorageExhaustionContract")]
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("Email", "dev@neo.org")]
    [ManifestExtra("Description", "A contract with storage exhaustion vulnerabilities for testing")]
    public class StorageExhaustionContract : Framework.SmartContract
    {
        // Storage maps
        private static readonly StorageMap DataMap = new StorageMap(Storage.CurrentContext, "data");
        private static readonly StorageMap LargeDataMap = new StorageMap(Storage.CurrentContext, "largeData");

        // Vulnerable to storage exhaustion - large key
        public static void StoreLargeKey(string key, string value)
        {
            // No check on key size
            DataMap.Put(key, value);
        }

        // Vulnerable to storage exhaustion - large value
        public static void StoreLargeValue(string key, string value)
        {
            // No check on value size
            LargeDataMap.Put(key, value);
        }

        // Vulnerable to storage exhaustion - many operations
        public static void StoreManyItems(string[] keys, string[] values)
        {
            // No check on number of operations
            for (int i = 0; i < keys.Length; i++)
            {
                DataMap.Put(keys[i], values[i]);
            }
        }

        // Safe version with key size check
        public static void SafeStoreLargeKey(string key, string value)
        {
            if (key.Length > 64)
                throw new Exception("Key too large");
                
            DataMap.Put(key, value);
        }

        // Safe version with value size check
        public static void SafeStoreLargeValue(string key, string value)
        {
            if (value.Length > 1024)
                throw new Exception("Value too large");
                
            LargeDataMap.Put(key, value);
        }

        // Safe version with operation count check
        public static void SafeStoreManyItems(string[] keys, string[] values)
        {
            if (keys.Length > 10)
                throw new Exception("Too many operations");
                
            for (int i = 0; i < keys.Length; i++)
            {
                DataMap.Put(keys[i], values[i]);
            }
        }
    }
}
