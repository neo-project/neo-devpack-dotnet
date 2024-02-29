using Neo.Cryptography.ECC;

namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidECPoint
    {
        /// <summary>
        /// Null ECPoint
        /// </summary>
        public static readonly ECPoint? Null = null;

        /// <summary>
        /// This will be an invalid ECPoint (ByteString)
        /// </summary>
        public static readonly ECPoint InvalidLength = new();

        /// <summary>
        /// This will be an invalid ECPoint (Integer)
        /// </summary>
        public static readonly ECPoint InvalidType = new();
    }
}
