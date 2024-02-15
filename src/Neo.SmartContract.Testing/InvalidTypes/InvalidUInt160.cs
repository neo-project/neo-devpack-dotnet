namespace Neo.SmartContract.Testing.InvalidTypes
{
    public class InvalidUInt160
    {
        /// <summary>
        /// Null UInt160
        /// </summary>
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public static readonly UInt160 Null = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        /// <summary>
        /// This will be an invalid UInt160
        /// </summary>
        public static readonly UInt160 Invalid = new();
    }
}
