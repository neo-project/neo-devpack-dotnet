using System.Threading.Tasks;
using Neo.SmartContract.Deploy.Models;

namespace Neo.SmartContract.Deploy.Interfaces;

/// <summary>
/// Interface for contract compilation services
/// </summary>
public interface IContractCompiler
{
    /// <summary>
    /// Compile a smart contract from source code
    /// </summary>
    /// <param name="projectPath">Path to the contract project (.csproj) or source file</param>
    /// <returns>Compiled contract</returns>
    Task<CompiledContract> CompileAsync(string projectPath);

    /// <summary>
    /// Load a pre-compiled contract from NEF and manifest files
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <returns>Compiled contract</returns>
    Task<CompiledContract> LoadContractAsync(string nefPath, string manifestPath);
}