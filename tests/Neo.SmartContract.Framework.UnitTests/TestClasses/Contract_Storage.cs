using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Storage : SmartContract
    {
        // There is no main here, it can be auto generation.

        #region Byte

        public static bool TestPutByte(byte[] key, byte[] value)
        {
            var storage = new StorageMap(Storage.CurrentContext, 0x11);
            storage.Put((ByteString)key, (ByteString)value);
            return true;
        }

        public static void TestDeleteByte(byte[] key)
        {
            var storage = new StorageMap(Storage.CurrentContext, 0x11);
            storage.Delete((ByteString)key);
        }

        public static byte[] TestGetByte(byte[] key)
        {
            var context = Storage.CurrentReadOnlyContext;
            var storage = new StorageMap(context, 0x11);
            var value = storage.Get((ByteString)key);
            return (byte[])value;
        }

        public static byte[] TestOver16Bytes()
        {
            var value = new byte[] { 0x3b, 0x00, 0x32, 0x03, 0x23, 0x23, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02 };
            StorageMap storageMap = new StorageMap(Storage.CurrentContext, 0);
            storageMap.Put((ByteString)new byte[] { 0x01 }, (ByteString)value);
            return (byte[])storageMap.Get((ByteString)new byte[] { 0x01 });
        }

        #endregion

        public static bool TestPutReadOnly(byte[] key, byte[] value)
        {
            var context = Storage.CurrentContext.AsReadOnly;
            var storage = new StorageMap(context, 3);
            storage.Put((ByteString)key, (ByteString)value);
            return true;
        }

        #region Serialize

        class Value
        {
            public int Val;
        }

        public static int SerializeTest(byte[] key, int value)
        {
            var context = Storage.CurrentContext;
            var storage = new StorageMap(context, 4);
            var val = new Value() { Val = value };
            storage.PutObject(key, val);
            val = (Value)storage.GetObject(key);
            return val.Val;
        }

        #endregion

        #region Find

        public static byte[] TestFind()
        {
            var context = Storage.CurrentContext;
            Storage.Put(context, (ByteString)"key1", (ByteString)new byte[] { 0x01 });
            Storage.Put(context, (ByteString)"key2", (ByteString)new byte[] { 0x02 });
            Iterator<byte[]> iterator = (Iterator<byte[]>)Storage.Find(context, "key", FindOptions.ValuesOnly);
            iterator.Next();
            return iterator.Value;
        }

        #endregion

        #region IndexProperty

        public static bool TestIndexPut(byte[] key, byte[] value)
        {
            var storage = new StorageMap(Storage.CurrentContext, 5);
            storage[(ByteString)key] = (ByteString)value;
            return true;
        }

        public static byte[] TestIndexGet(byte[] key)
        {
            var context = Storage.CurrentReadOnlyContext;
            var storage = new StorageMap(context, 5);
            var value = storage[(ByteString)key];
            return (byte[])value;
        }

        #endregion
    }
}
