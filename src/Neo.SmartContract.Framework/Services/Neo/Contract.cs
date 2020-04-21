namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Contract
    {
        /// <summary>
        /// Script
        /// </summary>
        public readonly byte[] Script;

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

        [Syscall("System.Contract.CreateStandardAccount")]
        public static extern void Contract_CreateStandardAccount(byte[] script);
    }
}
