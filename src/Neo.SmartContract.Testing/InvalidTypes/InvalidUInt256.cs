namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt256
    {
        /// <summary>
        /// Null UInt256
        /// </summary>
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static readonly UInt256 Null = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        /// <summary>
        /// This will be an invalid UInt256
        /// </summary>
        public static readonly UInt256 Invalid = new();
    }
}
