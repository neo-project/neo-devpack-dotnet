using Neo.SmartContract.Deploy;

// Create toolkit - automatically loads configuration from appsettings.json
var toolkit = new DeploymentToolkit();

// Set network (mainnet, testnet, local, or custom RPC URL)
toolkit.SetNetwork("testnet");

// Deploy the contract - that's it!
var result = await toolkit.Deploy(
    "../../src/SampleToken.Contract/SampleToken.Contract.csproj",
    new object[] { await toolkit.GetDeployerAccount() } // Pass deployer as owner
);

if (result.Success)
{
    Console.WriteLine($"✅ Contract deployed at: {result.ContractHash}");
    
    // Verify deployment
    var name = await toolkit.Call<string>(result.ContractHash.ToString(), "getName");
    var totalSupply = await toolkit.Call<System.Numerics.BigInteger>(result.ContractHash.ToString(), "totalSupply");
    
    Console.WriteLine($"Token: {name}");
    Console.WriteLine($"Total Supply: {totalSupply}");
    
    // Check deployer balance
    var balance = await toolkit.GetGasBalance();
    Console.WriteLine($"Deployer GAS balance: {balance}");
}
else
{
    Console.WriteLine($"❌ Deployment failed: {result.ErrorMessage}");
}