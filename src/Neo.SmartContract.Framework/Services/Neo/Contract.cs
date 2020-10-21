namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Contract
    {
        /// <summary>
        /// Script Hash
        /// </summary>
        public readonly byte[] ScriptHash;

        /// <summary>
        /// Script
        /// </summary>
        public readonly byte[] Script;

        /// <summary>
        /// Abi
        /// </summary>
        public readonly string Abi;

        /// <summary>
        /// Manifest
        /// </summary>
        public readonly string Manifest;

        /// <summary>
        /// Has storage
        /// </summary>
        public readonly bool HasStorage;

        /// <summary>
        /// Is payable
        /// </summary>
        public readonly bool IsPayable;

        [Syscall("System.Contract.Call")]
        public static extern object Call(byte[] scriptHash, string method, object[] arguments);

        [Syscall("System.Contract.CallEx")]
        public static extern object CallEx(byte[] scriptHash, string method, object[] arguments, CallFlags flag);

        [Syscall("System.Contract.Create")]
        public static extern Contract Create(byte[] script, string manifest);

        [Syscall("System.Contract.Update")]
        public static extern void Update(byte[] script, string manifest);

        [Syscall("System.Contract.Destroy")]
        public static extern void Destroy();

        [Syscall("System.Contract.GetCallFlags")]
        public static extern byte GetCallFlags();

        [Syscall("System.Contract.CreateStandardAccount")]
        public static extern byte[] CreateStandardAccount(byte[] pubKey);
    }
}
