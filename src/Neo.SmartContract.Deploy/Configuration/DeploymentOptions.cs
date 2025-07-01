namespace Neo.SmartContract.Deploy.Configuration
{
    /// <summary>
    /// Deployment configuration options
    /// </summary>
    public class DeploymentOptions
    {
        /// <summary>
        /// Whether to wait for transaction confirmation
        /// </summary>
        public bool WaitForConfirmation { get; set; } = true;

        /// <summary>
        /// Number of retries when waiting for confirmation
        /// </summary>
        public int ConfirmationRetries { get; set; } = 30;

        /// <summary>
        /// Delay between confirmation checks in seconds
        /// </summary>
        public int ConfirmationDelaySeconds { get; set; } = 5;

        /// <summary>
        /// Path to compiled contracts directory
        /// </summary>
        public string ContractsPath { get; set; } = "contracts";

        /// <summary>
        /// Number of blocks before transaction expires (default: 100)
        /// </summary>
        public uint ValidUntilBlockOffset { get; set; } = 100;

        /// <summary>
        /// Default network fee in GAS fractions (default: 0.001 GAS = 1000000)
        /// </summary>
        public long DefaultNetworkFee { get; set; } = 1000000;

        /// <summary>
        /// Whether to use Neo Express for development
        /// </summary>
        public bool UseNeoExpress { get; set; } = false;
    }
}