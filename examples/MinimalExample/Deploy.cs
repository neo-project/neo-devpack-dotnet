using Neo.SmartContract.Deploy;

// That's it! Just 2 lines to deploy a contract
var toolkit = new SimpleToolkit().SetNetwork("testnet");
var contractHash = await toolkit.Deploy("HelloWorld.cs");

Console.WriteLine($"Contract deployed: {contractHash}");