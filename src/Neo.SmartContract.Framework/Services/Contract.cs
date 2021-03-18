namespace Neo.SmartContract.Framework.Services
{
    public class Contract
    {
        /// <summary>
        /// Id
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// UpdateCounter
        /// </summary>
        public readonly ushort UpdateCounter;

        /// <summary>
        /// Hash
        /// </summary>
        public readonly UInt160 Hash;

        /// <summary>
        /// Nef
        /// </summary>
        public readonly ByteString Nef;

        /// <summary>
        /// Manifest
        /// </summary>
        public readonly string Manifest;

        [Syscall("System.Contract.Call")]
        public static extern object Call(UInt160 scriptHash, string method, CallFlags flags, params object[] args);

        [Syscall("System.Contract.GetCallFlags")]
        public static extern byte GetCallFlags();

        [Syscall("System.Contract.CreateStandardAccount")]
        public static extern UInt160 CreateStandardAccount(Cryptography.ECC.ECPoint pubKey);

        [Syscall("System.Contract.CreateMultisigAccount")]
        public static extern UInt160 CreateMultisigAccount(int m, params Cryptography.ECC.ECPoint[] pubKey);
    }
}
