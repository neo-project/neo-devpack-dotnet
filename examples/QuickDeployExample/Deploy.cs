using Neo.SmartContract.Deploy;

// Streamlined deployment process - configure network and deploy in two lines
var toolkit = new DeploymentToolkit().SetNetwork("testnet");
var contractHash = await toolkit.Deploy("HelloWorld.cs");

Console.WriteLine($"Contract deployed: {contractHash}");