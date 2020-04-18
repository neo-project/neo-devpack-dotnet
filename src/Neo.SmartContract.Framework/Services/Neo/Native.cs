namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Native
    {
        [Appcall("0x9bde8f209c88dd0e7ca3bf0af0f476cdd8207789")]
        public static extern object NEO(string method, object[] arguments);

        [Appcall("0x8c23f196d8a1bfd103a9dcb1f9ccf0c611377d3b")]
        public static extern object GAS(string method, object[] arguments);

        [Appcall("0x3209d09120465bf181ced70693b897ec6ea4619a")]
        public static extern object Policy(string method, object[] arguments);

        [Appcall("0x2acbe877270a65668da64cc589e8364bca970ba9")]
        public static extern object Oracle(string method, object[] arguments);
    }
}
