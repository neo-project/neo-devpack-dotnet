using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC.Models;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Result returned after a successful deploy operation.
/// </summary>
public sealed record DeploymentResult(UInt160 ContractHash, UInt256 TransactionHash, Transaction Transaction);

/// <summary>
/// Result returned after sending a state-changing invocation transaction.
/// </summary>
public sealed record InvocationResult(UInt256 TransactionHash, Transaction Transaction);
