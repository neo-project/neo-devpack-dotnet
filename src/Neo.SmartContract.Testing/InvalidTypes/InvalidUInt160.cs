namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt160
    {
        /// <summary>
        /// Null UInt160
        /// </summary>
        public static readonly UInt160? Null = null;

        /// <summary>
        /// This will be an invalid UInt160
        /// </summary>
        public static readonly UInt160 Invalid = new();
    }
}
