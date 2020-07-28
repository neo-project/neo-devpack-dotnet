using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static bool ValidateAddress(byte[] address) => address.Length == 20 && address.ToBigInteger() != 0;

        private static bool IsPayable(byte[] address) => Blockchain.GetContract(address)?.IsPayable ?? true;
    }
}
