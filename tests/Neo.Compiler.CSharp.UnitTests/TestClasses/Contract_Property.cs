namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Property : SmartContract.Framework.SmartContract
    {
        public static string Symbol => "TokenSymbol";

        public string this[int index]
        {
            get => index.ToString();
        }

        public string testIndex(int index)
        {
            return this[index];
        }
    }
}
