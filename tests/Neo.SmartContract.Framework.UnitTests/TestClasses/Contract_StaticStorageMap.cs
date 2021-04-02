using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_StaticStorageMap : SmartContract
    {
        private static StorageMap Data = new StorageMap(Storage.CurrentContext, "data");
        private static readonly StorageMap ReadonlyData = new StorageMap(Storage.CurrentContext, "readonlydata");

        public static void Put(string message)
        {
            Data.Put(message, 1);
        }

        public static BigInteger Get(string msg)
        {
            return (BigInteger)Data.Get(msg);
        }

        public static void PutReadonly(string message)
        {
            ReadonlyData.Put(message, 2);
        }

        public static BigInteger GetReadonly(string msg)
        {
            return (BigInteger)ReadonlyData.Get(msg);
        }

        public static void Put2(string message)
        {
            var Data2 = new StorageMap(Storage.CurrentContext, "data");
            Data2.Put(message, 3);
        }

        public static BigInteger Get2(string msg)
        {
            var Data2 = new StorageMap(Storage.CurrentContext, "data");
            return (BigInteger)Data2.Get(msg);
        }
    }
}
