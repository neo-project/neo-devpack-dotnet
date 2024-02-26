namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt256
    {
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly UInt160 Zero = UInt160.Zero;

        /// <summary>
        /// Null UInt256
        /// </summary>
        public static readonly UInt256? Null = null;

        /// <summary>
        /// This will be an invalid UInt256 (ByteString)
        /// </summary>
        public static readonly UInt256 InvalidLength = new();

        /// <summary>
        /// This will be an invalid UInt256 (Integer)
        /// </summary>
        public static readonly UInt256 InvalidType = new();
    }
}
