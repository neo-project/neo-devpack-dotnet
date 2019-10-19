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

        public static bool TestDeleteByte(byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(0xAA);
            var value = storage.Get(key);
            if (value == null) return false;
            storage.Delete(key);
            return true;
        }

        public static byte[] TestGetByte(byte[] key)
        {
            var storage = Storage.CurrentContext.CreateMap(0xAA);
            var value = storage.Get(key);
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

        public static bool TestDeleteString(byte[] key)
        {
            var prefix = "aa";
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            if (value == null) return false;
            storage.Delete(key);
            return true;
        }

        public static byte[] TestGetString(byte[] key)
        {
            var prefix = "aa";
            var storage = Storage.CurrentContext.CreateMap(prefix);
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

        public static bool TestDeleteByteArray(byte[] key)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            if (value == null) return false;
            storage.Delete(key);
            return true;
        }

        public static byte[] TestGetByteArray(byte[] key)
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var storage = Storage.CurrentContext.CreateMap(prefix);
            var value = storage.Get(key);
            return value;
        }

        #endregion
    }
}
