using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Persistence;
using System;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestStorageTests
    {
        [TestMethod]
        public void LoadFromJsonTest()
        {
            TestStorage store = new(new MemoryStore());

            // empty

            var entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 0);

            // simple object

            var json = @"{""a2V5"":""dmFsdWU=""}";

            store.LoadFromJson((JObject)JToken.Parse(json));
            store.Commit();

            entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 1);

            Assert.AreEqual("key", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            // prefix object

            json = @"{""bXkt"":{""a2V5"":""bXktdmFsdWU=""}}";

            store.LoadFromJson((JObject)JToken.Parse(json));
            store.Commit();

            entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 2);

            Assert.AreEqual("key", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            Assert.AreEqual("my-key", Encoding.ASCII.GetString(entries[1].Key));
            Assert.AreEqual("my-value", Encoding.ASCII.GetString(entries[1].Value));
        }
    }
}
