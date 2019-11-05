namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Json
    {
        [Syscall("Neo.Json.Serialize")]
        public extern static string Serialize(object obj);

        [Syscall("Neo.Json.Deserialize")]
        public extern static object Deserialize(string json);
    }
}
