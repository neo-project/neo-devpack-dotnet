using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Contract
{
    [DisplayName("DeploymentExample.SimpleContract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [ManifestExtra("Author", "Neo Community")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractPermission("*", "*")]
    public class SimpleContract : SmartContract
    {
        // Simple counter storage
        private const byte COUNTER_KEY = 0x01;

        [DisplayName("getCounter")]
        [Safe]
        public static BigInteger GetCounter()
        {
            var value = Storage.Get(Storage.CurrentContext, new byte[] { COUNTER_KEY });
            return value?.Length > 0 ? (BigInteger)value : 0;
        }

        [DisplayName("increment")]
        public static BigInteger Increment()
        {
            var current = GetCounter();
            var newValue = current + 1;
            Storage.Put(Storage.CurrentContext, new byte[] { COUNTER_KEY }, newValue);
            return newValue;
        }

        [DisplayName("multiply")]
        [Safe]
        public static BigInteger Multiply(BigInteger a, BigInteger b)
        {
            return a * b;
        }
    }
}