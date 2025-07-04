using System.Threading.Tasks;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Interfaces;

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
    /// <exception cref="Neo.SmartContract.Deploy.Exceptions.CompilationException">Thrown when compilation fails</exception>
    Task<CompiledContract> CompileAsync(CompilationOptions options);

    /// <summary>
    /// Load compiled contract from artifacts
    /// </summary>
    /// <param name="nefFilePath">Path to NEF file</param>
    /// <param name="manifestFilePath">Path to manifest file</param>
    /// <returns>Compiled contract</returns>
    /// <exception cref="System.IO.FileNotFoundException">Thrown when files are not found</exception>
    /// <exception cref="System.ArgumentException">Thrown when file paths are invalid</exception>
    Task<CompiledContract> LoadAsync(string nefFilePath, string manifestFilePath);
}
