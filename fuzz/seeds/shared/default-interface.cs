using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IFuzzValue
{
    [DisplayName("ifaceValue")]
    int InterfaceValue()
    {
        return 7;
    }
}

public class Contract : SmartContract, IFuzzValue
{
}
