using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;

namespace Template.HelloWorld
{
    [Features(ContractFeatures.HasStorage)]
    public class Contract1 : SmartContract
    {
        public static bool Main(string operation, object[] args)
        {
            Storage.Put("Hello", "World");
            return true;
        }
    }
}
