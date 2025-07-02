namespace Neo.SmartContract.Deploy.Exceptions;

/// <summary>
/// Base exception for all deployment-related errors
/// </summary>
public class DeploymentException : Exception
{
    /// <summary>
    /// Creates a new deployment exception
    /// </summary>
    public DeploymentException() : base()
    {
    }

    /// <summary>
    /// Creates a new deployment exception with a message
    /// </summary>
    /// <param name="message">Error message</param>
    public DeploymentException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new deployment exception with a message and inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public DeploymentException(string message, Exception innerException) : base(message, innerException)
    {
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
    public WalletException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new wallet exception with inner exception
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public WalletException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
