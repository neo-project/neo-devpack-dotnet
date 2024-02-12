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
        public void LoadExportImport()
        {
            TestStorage store = new(new MemoryStore());

            // empty

            var entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 0);

            // simple object

            var json = @"{""bXlSYXdLZXk="":""dmFsdWU=""}";

            store.Import((JObject)JToken.Parse(json));
            store.Commit();

            entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 1);

            Assert.AreEqual("myRawKey", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            // prefix object

            json = @"{""bXk="":{""UmF3S2V5LTI="":""dmFsdWUtMg==""}}";

            store.Import((JObject)JToken.Parse(json));
            store.Commit();

            entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 2);

            Assert.AreEqual("myRawKey", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            Assert.AreEqual("myRawKey-2", Encoding.ASCII.GetString(entries[1].Key));
            Assert.AreEqual("value-2", Encoding.ASCII.GetString(entries[1].Value));

            // Test import

            TestStorage storeCopy = new(new MemoryStore());

            store.Commit();
            storeCopy.Import(store.Export());
            storeCopy.Commit();

            entries = storeCopy.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 2);

            Assert.AreEqual("myRawKey", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            Assert.AreEqual("myRawKey-2", Encoding.ASCII.GetString(entries[1].Key));
            Assert.AreEqual("value-2", Encoding.ASCII.GetString(entries[1].Value));
        }
    }
}
