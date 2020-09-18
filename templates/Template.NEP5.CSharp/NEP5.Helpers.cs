using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Template.NEP5.CSharp
{
    public partial class NEP5 : SmartContract
    {
        private static bool ValidateAddress(UInt160 address) => address != UInt160.Zero;

        private static bool IsPayable(UInt160 address) => Blockchain.GetContract(address)?.IsPayable ?? true;
    }
}
