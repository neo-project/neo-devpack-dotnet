using Neo.SmartContract.Deploy;

namespace NeoContractSolution.Deploy
{
    /// <summary>
    /// Simple deployment program that handles all contracts defined in appsettings.json
    ///
    /// Usage:
    /// - Development: dotnet run
    /// - TestNet: ENVIRONMENT=TestNet dotnet run
    /// - MainNet: ENVIRONMENT=MainNet dotnet run
    /// 
    /// Multiple Contracts:
    /// - List contracts in dependency order in appsettings.json
    /// - Use {{ContractName}} to reference previously deployed contracts
    /// - Example: "InitParams": ["{{TokenContract}}", "{{GovernanceContract}}"]
    /// - The toolkit automatically resolves contract addresses and handles dependencies
    /// </summary>
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                // The ContractDeployer automatically:
                // 1. Compiles all contracts in src/
                // 2. Deploys contracts in the order listed in configuration
                // 3. Resolves {{ContractName}} references to actual contract addresses
                // 4. Deploys new contracts or updates existing ones
                // 5. Initializes contracts with configured parameters (including contract references)
                // 6. Tracks deployments per network for future updates
                var deployer = new ContractDeployer();
                var result = await deployer.DeployAsync();

                return result ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deployment failed: {ex.Message}");
                return 1;
            }
        }
    }
}
