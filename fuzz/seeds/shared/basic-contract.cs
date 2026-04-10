using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName("ping")]
    public static string Ping()
    {
        return "neo";
    }
}
