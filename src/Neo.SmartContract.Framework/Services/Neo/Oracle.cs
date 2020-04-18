namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Oracle
    {
        public static byte[] Get(string url, byte[] filterAddress = null, string filterMethod = null, string filterArgs = null)
        {
            return (byte[])Native.Oracle("get", new object[] { url, filterAddress, filterMethod, filterArgs });
        }

        public static byte[] Hash
        {
            get
            {
                return (byte[])Native.Oracle("getHash", new object[0]);
            }
        }
    }
}
