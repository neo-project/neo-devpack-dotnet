using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_StorageMap : SmartContract.Framework.SmartContract
    {
        // There is no main here, it can be auto generation.

        #region Byte

        public static bool TestPutByte(byte prefix, byte[] key, byte[] value)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Put(key, value);
            return true;
        }

        public static bool TestDeleteByte(byte prefix, byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            var exists = value.Length > 0;
            storage.Delete(key);
            return exists;
        }

        public static byte[] TestGetByte(byte prefix, byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            return value;
        }

        #endregion

        #region String

        public static bool TestPutString(string prefix, byte[] key, byte[] value)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Put(key, value);
            return true;
        }

        public static bool TestDeleteString(string prefix, byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            var exists = value.Length > 0;
            storage.Delete(key);
            return exists;
        }

        public static byte[] TestGetString(string prefix, byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            return value;
        }

        #endregion

        #region ByteArray

        public static bool TestPutByteArray(byte[] prefix, byte[] key, byte[] value)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            storage.Put(key, value);
            return true;
        }

        public static bool TestDeleteByteArray(byte[] prefix, byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            var exists = value.Length > 0;
            storage.Delete(key);
            return exists;
        }

        public static byte[] TestGetByteArray(byte[] prefix, byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            return value;
        }

        #endregion
    }
}
