namespace Neo.SmartContract.Testing.Coverage
{
    public enum MethodDetectionMechanism
    {
        /// <summary>
        /// Find RET
        /// </summary>
        FindRET,

        /// <summary>
        /// Next method defined in Abi
        /// If there are any private method, it probably will return undesired results
        /// </summary>
        NextMethodInAbi,

        /// <summary>
        /// It will compute the private methods
        /// </summary>
        NextMethod
    }
}
