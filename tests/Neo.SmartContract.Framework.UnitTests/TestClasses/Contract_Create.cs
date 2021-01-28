using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    [DisplayName("Contract_Create")]
    public class Contract_Create : SmartContract.Framework.SmartContract
    {
        public static int OldContract()
        {
            return 123;
        }
    }
}
