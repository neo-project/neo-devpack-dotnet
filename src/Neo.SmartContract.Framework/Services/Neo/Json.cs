namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Json
    {
        [Syscall("System.Json.Serialize")]
        public extern static string Serialize(object obj);

        [Syscall("System.Json.Deserialize")]
        public extern static object Deserialize(string json);
    }
}
