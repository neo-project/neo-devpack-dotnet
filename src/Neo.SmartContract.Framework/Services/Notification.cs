namespace Neo.SmartContract.Framework.Services
{
    public class Notification : IApiInterface
    {
        /// <summary>
        /// Sender script hash
        /// </summary>
        public readonly UInt160 ScriptHash;

        /// <summary>
        /// Notification's name
        /// </summary>
        public readonly string EventName;

        /// <summary>
        /// Notification's state
        /// </summary>
        public readonly object[] State;
    }
}
