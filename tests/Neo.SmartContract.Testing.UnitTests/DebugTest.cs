using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Storage;
using Neo.Persistence;
using Neo.Json;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class DebugTest
    {
        [TestMethod]
        public void DebugStorageTest()
        {
            EngineStorage store = new(new MemoryStore());
            
            // Test direct store access
            store.Store.Put(Encoding.ASCII.GetBytes("testkey"), Encoding.ASCII.GetBytes("testvalue"));
            
            var entries = store.Store.Find([], SeekDirection.Forward).ToArray();
            Console.WriteLine($"Direct store entries count: {entries.Length}");
            if (entries.Length > 0)
            {
                Console.WriteLine($"Direct store key: {Encoding.ASCII.GetString(entries[0].Key)}");
                Console.WriteLine($"Direct store value: {Encoding.ASCII.GetString(entries[0].Value)}");
            }
            
            // Test import
            var json = @"{""bXlSYXdLZXk="":""dmFsdWU=""}";
            store.Import(((JObject)JToken.Parse(json)!)!);
            
            entries = store.Store.Find([], SeekDirection.Forward).ToArray();
            Console.WriteLine($"After import entries count: {entries.Length}");
            foreach (var entry in entries)
            {
                Console.WriteLine($"Key bytes: {Convert.ToHexString(entry.Key)}");
                Console.WriteLine($"Key string: {Encoding.ASCII.GetString(entry.Key)}");
                Console.WriteLine($"Value string: {Encoding.ASCII.GetString(entry.Value)}");
            }
        }
    }
}