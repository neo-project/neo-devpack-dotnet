using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Helper
    {
        public static StorageMap CreateMap(this StorageContext context, byte[] prefix)
        {
            return new StorageMap
            {
                Context = context,
                Prefix = prefix
            };
        }

        public static StorageMap CreateMap(this StorageContext context, ByteString prefix)
        {
            return CreateMap(context, (byte[])prefix);
        }

        public static StorageMap CreateMap(this StorageContext context, byte prefix)
        {
            return CreateMap(context, prefix.ToByteArray());
        }

        public static void Delete(this StorageMap map, ByteString key)
        {
            byte[] k = map.Prefix.Concat(key);
            Storage.Delete(map.Context, k);
        }

        public static ByteString Get(this StorageMap map, ByteString key)
        {
            byte[] k = map.Prefix.Concat(key);
            return Storage.Get(map.Context, k);
        }

        public static void Put(this StorageMap map, ByteString key, ByteString value)
        {
            byte[] k = map.Prefix.Concat(key);
            Storage.Put(map.Context, k, value);
        }

        public static void Put(this StorageMap map, ByteString key, BigInteger value)
        {
            byte[] k = map.Prefix.Concat(key);
            Storage.Put(map.Context, k, value);
        }
    }
}
