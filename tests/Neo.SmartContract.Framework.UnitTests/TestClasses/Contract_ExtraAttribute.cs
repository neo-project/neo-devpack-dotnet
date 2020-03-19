using Neo.SmartContract.Framework;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ManifestExtraAttribute("Author", "Neo")]
    [ManifestExtraAttribute("E-mail", "dev@neo.org")]
    class Contract_ExtraAttribute : SmartContract
    {
        public static object Main(string method, object[] args)
        {
            return true;
        }
    }
}
