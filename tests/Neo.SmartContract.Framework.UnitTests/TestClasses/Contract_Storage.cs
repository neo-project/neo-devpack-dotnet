using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_Storage : SmartContract.Framework.SmartContract
    {
        // There is no main here, it can be auto generation.

        #region Byte

        public static bool TestPutByte(byte[] key, byte[] value)
        {
            var storage = Storage.CurrentContext.CreateMap(0xAA);
            storage.Put(key, value);
            return true;
        }

        public static void TestDeleteByte(byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(0xAA);
            storage.Delete(key);
        }

        public static byte[] TestGetByte(byte[] key)
        {
            var context = Storage.CurrentReadOnlyContext;
            var storage = context.CreateMap(0xAA);
            var value = storage.Get(key);
            return value;
        }

        public static byte[] TestOver16Bytes()
        {
            var value = new byte[] { 0x3b, 0x00, 0x32, 0x03, 0x23, 0x23, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02 };
            StorageMap storageMap = Storage.CurrentContext.CreateMap("test_map");
            storageMap.Put(new byte[] { 0x01 }, value);
            value = storageMap.Get(new byte[] { 0x01 });
            return value;
        }

        #endregion

        #region String

        public static bool TestPutString(byte[] key, byte[] value)
        {
            var prefix = "aa";
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Put(key, value);
            return true;
        }

        public static void TestDeleteString(byte[] key)
        {
            var prefix = "aa";
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Delete(key);
        }

        public static byte[] TestGetString(byte[] key)
        {
            var prefix = "aa";
            var context = Storage.CurrentReadOnlyContext;
            var storage = context.CreateMap(prefix);
            var value = storage.Get(key);
            return value;
        }

        #endregion

        #region ByteArray

        public static bool TestPutByteArray(byte[] key, byte[] value)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Put(key, value);
            return true;
        }

        public static void TestDeleteByteArray(byte[] key)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Delete(key);
        }

        public static byte[] TestGetByteArray(byte[] key)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var context = Storage.CurrentContext.AsReadOnly;
            var storage = context.CreateMap(prefix);
            var value = storage.Get(key);
            return value;
        }

        #endregion

        public static bool TestPutReadOnly(byte[] key, byte[] value)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var context = Storage.CurrentContext.AsReadOnly;
            var storage = context.CreateMap(prefix);
            storage.Put(key, value);
            return true;
        }

        #region Find

        public static byte[] TestFind()
        {
            Storage.Put("key1", new byte[] { 0x01 });
            Storage.Put("key2", new byte[] { 0x02 });
            Iterator<string, byte[]> iterator = Storage.Find("key");
            iterator.Next();
            return iterator.Value;
        }

        public static string TestFindKeys(FindOptions options)
        {
            Storage.Put("key1", new byte[] { 0x01 });
            Storage.Put("key2", new byte[] { 0x02 });
            Enumerator<string> enumerator = Storage.FindKeys("key", options);
            enumerator.Next();
            return enumerator.Value;
        }

        #endregion
    }
}
