namespace Neo.SmartContract.Deploy.Configuration
{
    /// <summary>
    /// Neo Express configuration options
    /// </summary>
    public class NeoExpressOptions
    {
        /// <summary>
        /// Path to Neo Express configuration file
        /// </summary>
        public string ConfigFile { get; set; } = "default.neo-express";

        /// <summary>
        /// RPC endpoint URL for Neo Express
        /// </summary>
        public string RpcUrl { get; set; } = "http://localhost:10332";

        /// <summary>
        /// Seconds per block for Neo Express
        /// </summary>
        public int SecondsPerBlock { get; set; } = 1;

        /// <summary>
        /// Auto-start Neo Express if not running
        /// </summary>
        public bool AutoStart { get; set; } = true;

        /// <summary>
        /// Auto-create instance if config doesn't exist
        /// </summary>
        public bool AutoCreate { get; set; } = true;

        /// <summary>
        /// Enable checkpoint management
        /// </summary>
        public bool EnableCheckpoints { get; set; } = true;

        /// <summary>
        /// Initial GAS amount to transfer to deployment wallet
        /// </summary>
        public decimal InitialGasAmount { get; set; } = 1000;

        /// <summary>
        /// Startup wait time in milliseconds
        /// </summary>
        public int StartupWaitMs { get; set; } = 2000;

        /// <summary>
        /// Retry wait time in milliseconds
        /// </summary>
        public int RetryWaitMs { get; set; } = 1000;
    }
}