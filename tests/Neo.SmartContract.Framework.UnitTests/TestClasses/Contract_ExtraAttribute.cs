namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ManifestExtra("Author", "Neo")]
    [ManifestExtra("E-mail", "dev@neo.org")]
    public class Contract_ExtraAttribute : SmartContract
    {
        public static object Main(string method, object[] args)
        {
            return true;
        }
    }
}
