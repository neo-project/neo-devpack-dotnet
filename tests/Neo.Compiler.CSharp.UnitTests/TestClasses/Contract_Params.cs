namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public sealed class Contract_Params : SmartContract.Framework.SmartContract
    {
        private static int Sum(params int[] args)
        {
            int sum = 0;
            foreach (int x in args)
                sum += x;
            return sum;
        }

        public static int Test()
        {
            return Sum() + Sum(1) + Sum(2, 3) + Sum(new int[] { 4, 5 });
        }
    }
}
