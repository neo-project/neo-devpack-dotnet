namespace Neo.SmartContract.Framework.Services.Neo
{
    public class Contract
    {
        /// <summary>
        /// Id
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Version
        /// </summary>
        public readonly int Version;

        /// <summary>
        /// Script
        /// </summary>
        public readonly byte[] Script;

        /// <summary>
        /// Manifest
        /// </summary>
        public readonly string Manifest;

        [Syscall("System.Contract.Call")]
        public static extern object Call(UInt160 scriptHash, string method, object[] arguments);

        [Syscall("System.Contract.CallEx")]
        public static extern object CallEx(UInt160 scriptHash, string method, object[] arguments, CallFlags flag);

        [Syscall("System.Contract.MethodExists")]
        public static extern bool MethodExists(UInt160 script_hash, string method);

        [Syscall("System.Contract.Create")]
        public static extern Contract Create(byte[] script, string manifest);

        [Syscall("System.Contract.Update")]
        public static extern void Update(byte[] script, string manifest);

        [Syscall("System.Contract.Destroy")]
        public static extern void Destroy();

        [Syscall("System.Contract.GetCallFlags")]
        public static extern byte GetCallFlags();

        [Syscall("System.Contract.CreateStandardAccount")]
        public static extern UInt160 CreateStandardAccount(Cryptography.ECC.ECPoint pubKey);
    }
}
