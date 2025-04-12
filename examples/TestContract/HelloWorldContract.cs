using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace TestContract
{
    [DisplayName("HelloWorldContract")]
    public class HelloWorldContract : SmartContract
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        [Safe]
        public static string SayHello(string name)
        {
            return $"Hello, {name}!";
        }

        [Safe]
        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static void UpdateContract(ByteString nefFile, string manifest)
        {
            ContractManagement.Update(nefFile, manifest, null);
        }
    }
}
