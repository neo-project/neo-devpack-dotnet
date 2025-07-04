using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Deploy.Exceptions;

/// <summary>
/// Base exception for all deployment-related errors
/// </summary>
public class DeploymentException : Exception
{
    /// <summary>
    /// Error code for categorizing exceptions
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Additional context data
    /// </summary>
    public object? Context { get; }

    /// <summary>
    /// Creates a new deployment exception
    /// </summary>
    public DeploymentException() : base()
    {
        ErrorCode = "GENERAL_ERROR";
    }

    /// <summary>
    /// Creates a new deployment exception with a message
    /// </summary>
    /// <param name="message">Error message</param>
    public DeploymentException(string message) : base(message)
    {
        ErrorCode = "GENERAL_ERROR";
    }

    /// <summary>
    /// Creates a new deployment exception with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public DeploymentException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = "GENERAL_ERROR";
    }

    /// <summary>
    /// Creates a new deployment exception with error code and context
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errorCode">Error code</param>
    /// <param name="context">Additional context</param>
    public DeploymentException(string message, string errorCode, object? context = null) : base(message)
    {
        ErrorCode = errorCode;
        Context = context;
    }

    /// <summary>
    /// Creates a new deployment exception with inner exception, error code, and context
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    /// <param name="errorCode">Error code</param>
    /// <param name="context">Additional context</param>
    public DeploymentException(string message, Exception innerException, string errorCode, object? context = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Context = context;
    }
}

/// <summary>
/// Exception thrown when contract compilation fails
/// </summary>
public class CompilationException : DeploymentException
{
    /// <summary>
    /// Gets the source file path that failed to compile
    /// </summary>
    public string SourcePath { get; }

    /// <summary>
    /// Gets the compilation errors
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Creates a new compilation exception
    /// </summary>
    /// <param name="sourcePath">Source file path</param>
    /// <param name="errors">Compilation errors</param>
    public CompilationException(string sourcePath, IEnumerable<string> errors)
        : base($"Failed to compile {sourcePath}: {string.Join("; ", errors)}")
    {
        SourcePath = sourcePath;
        Errors = errors.ToList().AsReadOnly();
    }
}

/// <summary>
/// Exception thrown when contract deployment fails
/// </summary>
public class ContractDeploymentException : DeploymentException
{
    /// <summary>
    /// Gets the contract name
    /// </summary>
    public string ContractName { get; }

    /// <summary>
    /// Creates a new contract deployment exception
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="message">Error message</param>
    public ContractDeploymentException(string contractName, string message)
        : base($"Failed to deploy contract '{contractName}': {message}")
    {
        ContractName = contractName;
    }

    /// <summary>
    /// Creates a new contract deployment exception with inner exception
    /// </summary>
    /// <param name="contractName">Contract name</param>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public ContractDeploymentException(string contractName, string message, Exception innerException)
        : base($"Failed to deploy contract '{contractName}': {message}", innerException)
    {
        ContractName = contractName;
    }
}

/// <summary>
/// Exception thrown when contract invocation fails
/// </summary>
public class ContractInvocationException : DeploymentException
{
    /// <summary>
    /// Gets the contract hash
    /// </summary>
    public UInt160 ContractHash { get; }

    /// <summary>
    /// Gets the method name
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Creates a new contract invocation exception
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="methodName">Method name</param>
    /// <param name="message">Error message</param>
    public ContractInvocationException(UInt160 contractHash, string methodName, string message)
        : base($"Failed to invoke {methodName} on contract {contractHash}: {message}")
    {
        ContractHash = contractHash;
        MethodName = methodName;
    }
}

/// <summary>
/// Exception thrown when wallet operations fail
/// </summary>
public class WalletException : DeploymentException
{
    /// <summary>
    /// Creates a new wallet exception
    /// </summary>
    /// <param name="message">Error message</param>
    public WalletException(string message) : base(message, "WALLET_ERROR")
    {
    }

    /// <summary>
    /// Creates a new wallet exception with inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public WalletException(string message, Exception innerException) : base(message, innerException, "WALLET_ERROR")
    {
    }
}

/// <summary>
/// Exception thrown when network configuration is invalid
/// </summary>
public class NetworkConfigurationException : DeploymentException
{
    /// <summary>
    /// Creates a new network configuration exception
    /// </summary>
    /// <param name="message">Error message</param>
    public NetworkConfigurationException(string message) : base(message, "NETWORK_CONFIG_ERROR")
    {
    }

    /// <summary>
    /// Creates a new network configuration exception with inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public NetworkConfigurationException(string message, Exception innerException)
        : base(message, innerException, "NETWORK_CONFIG_ERROR")
    {
    }
}

/// <summary>
/// Exception thrown when contract update fails
/// </summary>
public class ContractUpdateException : DeploymentException
{
    /// <summary>
    /// Gets the contract hash being updated
    /// </summary>
    public UInt160? ContractHash { get; }

    /// <summary>
    /// Creates a new contract update exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="contractHash">Contract hash being updated</param>
    public ContractUpdateException(string message, UInt160? contractHash = null)
        : base(message, "UPDATE_ERROR")
    {
        ContractHash = contractHash;
    }

    /// <summary>
    /// Creates a new contract update exception with inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    /// <param name="contractHash">Contract hash being updated</param>
    public ContractUpdateException(string message, Exception innerException, UInt160? contractHash = null)
        : base(message, innerException, "UPDATE_ERROR")
    {
        ContractHash = contractHash;
    }
}
