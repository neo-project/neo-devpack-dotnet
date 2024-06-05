using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework.Interfaces;

namespace Neo.SmartContract.Framework.TestContracts
{
    [DisplayName(nameof(Contract_SupportedStandard11Payable))]
    [ContractDescription("<Description Here>")]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep11Payable)]
    public class Contract_SupportedStandard11Payable : SmartContract, INep11Payable
    {
        public void OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)
        {
        }
    }
}
