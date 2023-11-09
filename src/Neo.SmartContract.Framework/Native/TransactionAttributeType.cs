namespace Neo.SmartContract.Framework.Native
{
    /// <summary>
    /// Represents the type of a <see cref="TransactionAttribute"/>.
    /// </summary>
    public enum TransactionAttributeType : byte
    {
        /// <summary>
        /// Indicates that the transaction is of high priority.
        /// </summary>
        HighPriority = 0x01,

        /// <summary>
        /// Indicates that the transaction is an oracle response.
        /// </summary>
        OracleResponse = 0x11,

        /// <summary>
        /// Indicates that the transaction is not valid before <see cref="NotValidBefore.Height"/>.
        /// </summary>
        NotValidBefore = 0x20,

        /// <summary>
        /// Indicates that the transaction conflicts with <see cref="Conflicts.Hash"/>.
        /// </summary>
        Conflicts = 0x21
    }
}
