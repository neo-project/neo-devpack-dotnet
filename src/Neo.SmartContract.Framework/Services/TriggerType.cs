namespace Neo.SmartContract.Framework.Services
{
    public enum TriggerType : byte
    {
        /// <summary>
        /// Indicate that the contract is triggered by the system to execute the OnPersist method of the native contracts.
        /// </summary>
        OnPersist = 0b_0000_0001,

        /// <summary>
        /// Indicate that the contract is triggered by the system to execute the PostPersist method of the native contracts.
        /// </summary>
        PostPersist = 0b_0000_0010,

        /// <summary>
        /// Indicates that the contract is triggered by the verification of a <see cref="IVerifiable"/>.
        /// </summary>
        Verification = 0b_0010_0000,

        /// <summary>
        /// Indicates that the contract is triggered by the execution of transactions.
        /// </summary>
        Application = 0b_0100_0000,

        /// <summary>
        /// The combination of all system triggers.
        /// </summary>
        System = OnPersist | PostPersist,

        /// <summary>
        /// The combination of all triggers.
        /// </summary>
        All = OnPersist | PostPersist | Verification | Application
    }
}
