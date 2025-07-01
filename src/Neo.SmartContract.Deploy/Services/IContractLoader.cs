using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Service for loading contract files
    /// </summary>
    public interface IContractLoader
    {
        /// <summary>
        /// Load a contract by name
        /// </summary>
        /// <param name="contractName">Name of the contract</param>
        Task<ContractData> LoadContractAsync(string contractName);

        /// <summary>
        /// Load all contracts from the contracts directory
        /// </summary>
        Task<IEnumerable<ContractData>> LoadAllContractsAsync();
    }

    /// <summary>
    /// Contract data including NEF and manifest
    /// </summary>
    public class ContractData
    {
        public string Name { get; set; } = string.Empty;
        public byte[] NefBytes { get; set; } = Array.Empty<byte>();
        public ContractManifest Manifest { get; set; } = null!;
    }
}