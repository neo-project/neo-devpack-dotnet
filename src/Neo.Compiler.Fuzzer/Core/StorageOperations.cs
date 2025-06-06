using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Fuzzer
{
    /// <summary>
    /// Extension methods for FragmentGenerator to handle storage operations
    /// </summary>
    public static class StorageOperationsExtensions
    {
        /// <summary>
        /// Generate a random storage operation
        /// </summary>
        public static string GenerateEnhancedStorageOperation(this FragmentGenerator generator)
        {
            Random random = new Random();
            string[] storageOperations = {
                "BasicStorageOperations",
                "StorageMapOperations",
                // "StorageFindOperations", // Commented out due to BigInteger reference issues
                "StorageContextOperations",
                "StorageDeleteOperations"
            };

            string operation = storageOperations[random.Next(storageOperations.Length)];

            switch (operation)
            {
                case "BasicStorageOperations":
                    return GenerateBasicStorageOperations(generator);
                case "StorageMapOperations":
                    return GenerateStorageMapOperations(generator);
                case "StorageFindOperations":
                    return GenerateStorageFindOperations(generator);
                case "StorageContextOperations":
                    return GenerateStorageContextOperations(generator);
                case "StorageDeleteOperations":
                    return GenerateStorageDeleteOperations(generator);
                default:
                    return GenerateBasicStorageOperations(generator);
            }
        }

        /// <summary>
        /// Generate basic storage operations (Put/Get)
        /// </summary>
        private static string GenerateBasicStorageOperations(FragmentGenerator generator)
        {
            string key = generator.GenerateIdentifier("key");
            string value = generator.GenerateIdentifier("value");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Basic storage operations");
            sb.AppendLine($"string {key} = \"test_key\";");
            sb.AppendLine($"string {value} = \"test_value\";");
            sb.AppendLine($"// Store a value in storage");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, {key}, {value});");
            sb.AppendLine($"// Retrieve the value from storage");
            sb.AppendLine($"string retrieved = Storage.Get(Storage.CurrentContext, {key});");

            return sb.ToString();
        }

        /// <summary>
        /// Generate storage map operations
        /// </summary>
        private static string GenerateStorageMapOperations(FragmentGenerator generator)
        {
            string mapName = generator.GenerateIdentifier("map");
            string key = generator.GenerateIdentifier("key");
            string value = generator.GenerateIdentifier("value");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Storage map operations");
            sb.AppendLine($"// Create a storage map with a prefix");
            sb.AppendLine($"StorageMap {mapName} = new StorageMap(Storage.CurrentContext, \"prefix\");");
            sb.AppendLine($"// Store a key-value pair in the map");
            sb.AppendLine($"{mapName}.Put(\"{key}\", \"value\");");
            sb.AppendLine($"// Retrieve the value from the map");
            sb.AppendLine($"string {value} = {mapName}.Get(\"{key}\");");

            return sb.ToString();
        }

        /// <summary>
        /// Generate storage find operations
        /// </summary>
        private static string GenerateStorageFindOperations(FragmentGenerator generator)
        {
            Random random = new Random();
            string prefix = generator.GenerateIdentifier("prefix");
            string iterator = generator.GenerateIdentifier("iterator");
            string count = generator.GenerateIdentifier("count");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Storage find operations");
            sb.AppendLine($"string {prefix} = \"test_\";");

            // Add some test data
            sb.AppendLine("// Add some test data to storage");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, \"{prefix}1\", \"value1\");");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, \"{prefix}2\", \"value2\");");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, \"{prefix}3\", \"value3\");");

            // Find options
            string[] findOptions = {
                "FindOptions.KeysOnly",
                "FindOptions.ValuesOnly",
                "FindOptions.RemovePrefix",
                "FindOptions.None"
            };
            string option = findOptions[random.Next(findOptions.Length)];

            sb.AppendLine("// Find items with prefix");
            sb.AppendLine($"var {iterator} = Storage.Find(Storage.CurrentContext, {prefix}, {option});");
            sb.AppendLine($"int {count} = 0;");
            sb.AppendLine("// Count the items found");
            sb.AppendLine($"while ({iterator}.Next())");
            sb.AppendLine("{");
            sb.AppendLine($"    {count}++;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generate storage context operations
        /// </summary>
        private static string GenerateStorageContextOperations(FragmentGenerator generator)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Storage context operations");
            sb.AppendLine("// Get the current storage context");
            sb.AppendLine("StorageContext context = Storage.CurrentContext;");
            sb.AppendLine("// Get a read-only context");
            sb.AppendLine("StorageContext readOnlyContext = Storage.CurrentReadOnlyContext;");
            sb.AppendLine("// Get a read-only context directly");
            sb.AppendLine("StorageContext asReadOnly = Storage.CurrentReadOnlyContext;");

            return sb.ToString();
        }

        /// <summary>
        /// Generate storage delete operations
        /// </summary>
        private static string GenerateStorageDeleteOperations(FragmentGenerator generator)
        {
            string key = generator.GenerateIdentifier("key");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// Storage delete operations");
            sb.AppendLine($"string {key} = \"test_key\";");
            sb.AppendLine("// First put something in storage");
            sb.AppendLine($"Storage.Put(Storage.CurrentContext, {key}, \"value\");");
            sb.AppendLine("// Then delete it");
            sb.AppendLine($"Storage.Delete(Storage.CurrentContext, {key});");
            sb.AppendLine("// Check if it was deleted");
            sb.AppendLine($"string deleted = Storage.Get(Storage.CurrentContext, {key});");
            sb.AppendLine("bool wasDeleted = deleted == null;");

            return sb.ToString();
        }
    }
}
