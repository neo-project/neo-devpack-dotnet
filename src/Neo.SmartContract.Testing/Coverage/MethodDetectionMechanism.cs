namespace Neo.SmartContract.Testing.Coverage
{
    public enum MethodDetectionMechanism
    {
        /// <summary>
        /// Find RET
        /// </summary>
        FindRET,

        /// <summary>
        /// Next method
        /// </summary>
        NextMethodInAbi,

        /// <summary>
        /// It will compute the private methods
        /// </summary>
        NextMethod
    }
}
