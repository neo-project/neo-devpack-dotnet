using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class SampleAttribute : System.Attribute { }

    [DisplayName("Contract_AttributeChanged")]
    public class Contract_AttributeChanged : SmartContract.Framework.SmartContract
    {
        [Sample]
        public static bool test() => true;
    }
}
