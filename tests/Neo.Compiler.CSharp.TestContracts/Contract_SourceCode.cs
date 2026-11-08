using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    [DisplayName("TestSourceCode")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    public class Contract_SourceCode : SmartContract.Framework.SmartContract
    {
        public static string GetVersion()
        {
            return "1.0.0";
        }
    }
}
