using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

namespace Template.NEP5.CSharp
{
    public static class AssetStorage
    {
        public static readonly string mapName = "asset";

        public static void Increase(byte[] key, BigInteger value) => Put(key, Get(key) + value);

        public static void Reduce(byte[] key, BigInteger value)
        {
            var oldValue = Get(key);
            if (oldValue == value)
                Remove(key);
            else
                Put(key, oldValue - value);
        }

        public static void Put(byte[] key, BigInteger value) => Storage.CurrentContext.CreateMap(mapName).Put(key, value);

        public static BigInteger Get(byte[] key) => Storage.CurrentContext.CreateMap(mapName).Get(key).TryToBigInteger();

        public static void Remove(byte[] key) => Storage.CurrentContext.CreateMap(mapName).Delete(key);
    }
}
