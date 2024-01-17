using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class SampleAttribute : System.Attribute { }

    [DisplayName("attr")]
    public abstract class A : SmartContract.Framework.SmartContract { }

    public class Contract_Attribute : A
    {
        [SampleAttribute]
        public static bool test() => true;
    }
}
