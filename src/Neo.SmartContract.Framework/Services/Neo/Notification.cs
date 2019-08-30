namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Notification : IApiInterface
    {
        /// <summary>
        /// Sender script hash
        /// </summary>
        public readonly byte[] ScriptHash;

        /// <summary>
        /// Notification's state
        /// </summary>
        public readonly object State;
    }
}
