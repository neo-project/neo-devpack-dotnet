namespace Neo.SmartContract.Deploy.Configuration
{
    /// <summary>
    /// Network configuration options
    /// </summary>
    public class NetworkOptions
    {
        /// <summary>
        /// RPC endpoint URL
        /// </summary>
        public string RpcUrl { get; set; } = "http://localhost:10332";

        /// <summary>
        /// Network name (private, testnet, mainnet)
        /// </summary>
        public string Network { get; set; } = "private";

        /// <summary>
        /// Network-specific wallet configuration
        /// </summary>
        public NetworkWalletOptions? Wallet { get; set; }
    }

    /// <summary>
    /// Network-specific wallet configuration
    /// </summary>
    public class NetworkWalletOptions
    {
        /// <summary>
        /// Path to wallet file
        /// </summary>
        public string WalletPath { get; set; } = string.Empty;

        /// <summary>
        /// Wallet password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Default account address (optional)
        /// </summary>
        public string? DefaultAccount { get; set; }
    }
}