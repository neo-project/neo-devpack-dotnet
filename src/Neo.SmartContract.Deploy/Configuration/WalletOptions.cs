namespace Neo.SmartContract.Deploy.Configuration
{
    /// <summary>
    /// Wallet configuration options
    /// </summary>
    public class WalletOptions
    {
        /// <summary>
        /// Path to wallet file
        /// </summary>
        public string WalletPath { get; set; } = "wallet.json";

        /// <summary>
        /// Wallet password (if not provided, will check WALLET_PASSWORD environment variable)
        /// WARNING: Do not store passwords in configuration files. Use environment variables or secure key vaults.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Default account address (if not provided, will use first account in wallet)
        /// </summary>
        public string? DefaultAccount { get; set; }
    }
}