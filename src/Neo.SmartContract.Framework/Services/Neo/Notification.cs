namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Notification : IApiInterface
    {
        /// <summary>
        /// Sender script hash
        /// </summary>
        public byte[] ScriptHash;

        /// <summary>
        /// Notification's state
        /// </summary>
        public object State;
    }
}
