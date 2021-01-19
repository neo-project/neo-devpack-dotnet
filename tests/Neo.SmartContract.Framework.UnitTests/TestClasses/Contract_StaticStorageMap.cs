using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_StaticStorageMap : SmartContract.Framework.SmartContract
    {
        private static StorageMap Data = Storage.CurrentContext.CreateMap("data");
        private static readonly StorageMap ReadonlyData = Storage.CurrentContext.CreateMap("readonlydata");

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
            var Data2 = Storage.CurrentContext.CreateMap("data");
            Data2.Put(message, 3);
        }

        public static BigInteger Get2(string msg)
        {
            var Data2 = Storage.CurrentContext.CreateMap("data");
            return (BigInteger)Data2.Get(msg);
        }
    }
}
