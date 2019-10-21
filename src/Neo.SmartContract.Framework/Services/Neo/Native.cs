namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        [Appcall("0x43cf98eddbe047e198a3e5d57006311442a0ca15")]
        public static extern object NEO(string method, object[] arguments);

        [Appcall("0xa1760976db5fcdfab2a9930e8f6ce875b2d18225")]
        public static extern object GAS(string method, object[] arguments);

        [Appcall("0x9c5699b260bd468e2160dd5d45dfd2686bba8b77")]
        public static extern object Policy(string method, object[] arguments);
    }
}
