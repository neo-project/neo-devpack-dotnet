using Neo.SmartContract.Deploy.Models;

/// <summary>
/// Contract compiler interface
/// </summary>
public interface IContractCompiler
{
    /// <summary>
    /// Compile a smart contract from source code
    /// </summary>
    /// <param name="options">Compilation options</param>
    /// <returns>Compiled contract</returns>
    Task<CompiledContract> CompileAsync(CompilationOptions options);

    /// <summary>
    /// Load compiled contract from artifacts
    /// </summary>
    /// <param name="nefFilePath">Path to NEF file</param>
    /// <param name="manifestFilePath">Path to manifest file</param>
    /// <returns>Compiled contract</returns>
    Task<CompiledContract> LoadAsync(string nefFilePath, string manifestFilePath);
}
