using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static bool ValidateAddress(byte[] address)
        {
            if (address.Length != 20)
                return false;
            if (address.AsBigInteger() == 0)
                return false;
            return true;
        }

        private static bool IsPayable(byte[] address)
        {
            var c = Blockchain.GetContract(address);
            return c == null || c.IsPayable;
        }
    }
}
