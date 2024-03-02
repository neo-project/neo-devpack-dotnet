namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_SequencePointInserter : SmartContract
    {
        public static int test(int a)
        {
            if (a == 1) return 23;
            return 45;
        }
    }
}
