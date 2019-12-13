using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.TestClasses
{
    [Features(ContractFeatures.HasStorage)]
    class Sample
    {
        public int Value;
    }

    [Features(ContractFeatures.Payable)]
    class Contract_Feature : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            var a = new Sample();
            return a.Value;
        }
    }
}
