using Neo.SmartContract.Framework.Attributes;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework.Interfaces;

namespace Neo.SmartContract.Framework.TestContracts
{
    [DisplayName(nameof(Contract_SupportedStandard17Payable))]
    [ContractDescription("<Description Here>")]
    [ContractAuthor("<Your Name Or Company Here>", "<Your Public Email Here>")]
    [ContractVersion("<Version String Here>")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep17Payable)]
    public class Contract_SupportedStandard17Payable : SmartContract, INep17Payable
    {
        public void OnNEP17Payment(UInt160 from, BigInteger amount, object? data = null)
        {
        }
    }
}
