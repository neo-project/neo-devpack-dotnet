using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class SampleAttribute : System.Attribute { }

    [DisplayName("attr")]
    public abstract class Attr : SmartContract.Framework.SmartContract { }

    public class Contract_Attribute : Attr
    {
        [Sample]
        public static bool test() => true;
    }
}
