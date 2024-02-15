namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt256
    {
        /// <summary>
        /// Null UInt256
        /// </summary>
        public static readonly UInt256? Null = null;

        /// <summary>
        /// This will be an invalid UInt256
        /// </summary>
        public static readonly UInt256 Invalid = new();
    }
}
