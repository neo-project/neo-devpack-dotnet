using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo.SmartContract.Deploy.Configuration;
using Neo.SmartContract.Manifest;
using System.Text.Json;

namespace Neo.SmartContract.Deploy.Services
{
    /// <summary>
    /// Implementation of contract loading service
    /// </summary>
    public class ContractLoader : IContractLoader
    {
        private readonly ILogger<ContractLoader> _logger;
        private readonly DeploymentOptions _options;

        public ContractLoader(ILogger<ContractLoader> logger, IOptions<DeploymentOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<ContractData> LoadContractAsync(string contractName)
        {
            var contractsPath = Path.GetFullPath(_options.ContractsPath);
            var nefPath = Path.Combine(contractsPath, $"{contractName}.nef");
            var manifestPath = Path.Combine(contractsPath, $"{contractName}.manifest.json");

            if (!File.Exists(nefPath))
            {
                throw new FileNotFoundException($"NEF file not found: {nefPath}");
            }

            if (!File.Exists(manifestPath))
            {
                throw new FileNotFoundException($"Manifest file not found: {manifestPath}");
            }

            _logger.LogInformation("Loading contract: {Name}", contractName);

            var nefBytes = await File.ReadAllBytesAsync(nefPath);
            var manifestJson = await File.ReadAllTextAsync(manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);

            return new ContractData
            {
                Name = contractName,
                NefBytes = nefBytes,
                Manifest = manifest
            };
        }

        public async Task<IEnumerable<ContractData>> LoadAllContractsAsync()
        {
            var contracts = new List<ContractData>();
            var contractsPath = Path.GetFullPath(_options.ContractsPath);

            if (!Directory.Exists(contractsPath))
            {
                _logger.LogWarning("Contracts directory not found: {Path}", contractsPath);
                return contracts;
            }

            var nefFiles = Directory.GetFiles(contractsPath, "*.nef");
            var failedContracts = new List<string>();
            
            foreach (var nefFile in nefFiles)
            {
                var contractName = Path.GetFileNameWithoutExtension(nefFile);
                try
                {
                    var contract = await LoadContractAsync(contractName);
                    contracts.Add(contract);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load contract: {Name}", contractName);
                    failedContracts.Add(contractName);
                }
            }

            _logger.LogInformation("Loaded {Count} contracts successfully", contracts.Count);
            
            if (failedContracts.Any())
            {
                _logger.LogWarning("Failed to load {Count} contracts: {Names}", 
                    failedContracts.Count, string.Join(", ", failedContracts));
            }

            return contracts;
        }
    }
}