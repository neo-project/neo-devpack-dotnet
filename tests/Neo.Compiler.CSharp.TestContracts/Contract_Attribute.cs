using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class SampleAttribute : System.Attribute { }

    [DisplayName("attr")]
    public abstract class Attr : SmartContract.Framework.SmartContract { }

    public class Contract_Attribute : Attr
    {
        [SampleAttribute]
        public static bool test() => true;
    }
}
