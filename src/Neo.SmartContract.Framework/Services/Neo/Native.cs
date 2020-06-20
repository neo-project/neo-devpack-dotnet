namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        [Appcall("0xde5f57d430d3dece511cf975a8d37848cb9e0525")]
        public static extern object NEO(string method, object[] arguments);

        [Appcall("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
        public static extern object GAS(string method, object[] arguments);

        [Appcall("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
        public static extern object Policy(string method, object[] arguments);

        [Appcall("0x2acbe877270a65668da64cc589e8364bca970ba9")]
        public static extern object Oracle(string method, object[] arguments);
    }
}
