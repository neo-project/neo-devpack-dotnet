using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [Author("core-dev")]
    [Email("core@neo.org")]
    [Version("v3.6.3")]
    [Description("This is a test contract.")]
    [ManifestExtra("ExtraKey", "ExtraValue")]
    public class Contract_ManifestAttribute : SmartContract
    {
        [NoReentrant]
        public void reentrantTest(int value)
        {
            if (value == 0) return;
            if (value == 123)
            {
                reentrantTest(0);
            }
        }
    }
}
