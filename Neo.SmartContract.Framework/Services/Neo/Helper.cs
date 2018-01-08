using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Helper
    {
        public static StorageMap CreateMap(this StorageContext context, string prefix)
        {
            return new StorageMap
            {
                Context = context,
                Prefix = prefix
            };
        }

        public static void Delete(this StorageMap map, byte[] key)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key);
            Storage.Delete(map.Context, k);
        }

        public static void Delete(this StorageMap map, string key)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key.AsByteArray());
            Storage.Delete(map.Context, k);
        }

        public static byte[] Get(this StorageMap map, byte[] key)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key);
            return Storage.Get(map.Context, k);
        }

        public static byte[] Get(this StorageMap map, string key)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key.AsByteArray());
            return Storage.Get(map.Context, k);
        }

        public static void Put(this StorageMap map, byte[] key, byte[] value)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key);
            Storage.Put(map.Context, k, value);
        }

        public static void Put(this StorageMap map, byte[] key, BigInteger value)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key);
            Storage.Put(map.Context, k, value);
        }

        public static void Put(this StorageMap map, byte[] key, string value)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key);
            Storage.Put(map.Context, k, value);
        }

        public static void Put(this StorageMap map, string key, byte[] value)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key.AsByteArray());
            Storage.Put(map.Context, k, value);
        }

        public static void Put(this StorageMap map, string key, BigInteger value)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key.AsByteArray());
            Storage.Put(map.Context, k, value);
        }

        public static void Put(this StorageMap map, string key, string value)
        {
            byte[] k = map.Prefix.AsByteArray().Concat(new byte[] { 0 }).Concat(key.AsByteArray());
            Storage.Put(map.Context, k, value);
        }
    }
}
