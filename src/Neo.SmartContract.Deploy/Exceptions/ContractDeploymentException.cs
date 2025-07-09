using System;

namespace Neo.SmartContract.Deploy.Exceptions;

/// <summary>
/// Exception thrown during contract deployment operations
/// </summary>
public class ContractDeploymentException : Exception
{
    /// <summary>
    /// Name of the contract that failed to deploy
    /// </summary>
    public string ContractName { get; }

    /// <summary>
    /// Create a new ContractDeploymentException
    /// </summary>
    /// <param name="contractName">Name of the contract</param>
    /// <param name="message">Error message</param>
    public ContractDeploymentException(string contractName, string message)
        : base(message)
    {
        ContractName = contractName;
    }

    /// <summary>
    /// Create a new ContractDeploymentException with inner exception
    /// </summary>
    /// <param name="contractName">Name of the contract</param>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public ContractDeploymentException(string contractName, string message, Exception innerException)
        : base(message, innerException)
    {
        ContractName = contractName;
    }
}
