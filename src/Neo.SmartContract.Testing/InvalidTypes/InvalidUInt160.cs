namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt160
    {
        /// <summary>
        /// Zero
        /// </summary>
        public static readonly UInt160 Zero = UInt160.Zero;

        /// <summary>
        /// Null UInt160
        /// </summary>
        public static readonly UInt160? Null = null;

        /// <summary>
        /// This will be an invalid UInt160 (ByteString)
        /// </summary>
        public static readonly UInt160 InvalidLength = new();

        /// <summary>
        /// This will be an invalid UInt160 (Integer)
        /// </summary>
        public static readonly UInt160 InvalidType = new();
    }
}
