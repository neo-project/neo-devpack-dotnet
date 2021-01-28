using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    [DisplayName("Contract_Update")]
    public class Contract_Update : SmartContract.Framework.SmartContract
    {
        public static int NewContract()
        {
            return 124;
        }
    }
}
