namespace Neo.SmartContract.Framework.Services.Neo
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
        /// Script
        /// </summary>
        public readonly byte[] Script;

        /// <summary>
        /// Manifest
        /// </summary>
        public readonly string Manifest;

        [OpCode(OpCode.REVERSE3)]
        [OpCode(OpCode.PUSHINT8, "0f")] // 0x0f == 15 CallFlags.All
        [OpCode(OpCode.REVERSE4)]
        [Syscall("System.Contract.CallEx")]
        public static extern object Call(UInt160 scriptHash, string method, object[] arguments);

        [Syscall("System.Contract.CallEx")]
        public static extern object CallEx(UInt160 scriptHash, string method, object[] arguments, CallFlags flag);

        [Syscall("System.Contract.GetCallFlags")]
        public static extern byte GetCallFlags();

        [Syscall("System.Contract.CreateStandardAccount")]
        public static extern UInt160 CreateStandardAccount(Cryptography.ECC.ECPoint pubKey);
    }
}
