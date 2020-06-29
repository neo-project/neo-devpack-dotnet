using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static bool ValidateAddress(byte[] address) => address.Length == 20 && address.TryToBigInteger() != 0;

        private static bool IsPayable(byte[] address) => Blockchain.GetContract(address)?.IsPayable ?? true;
    }

    public static class Helper
    {
        public static BigInteger TryToBigInteger(this byte[] value)
        {
            return value?.ToBigInteger() ?? 0;
        }
    }
}
