namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Contract compilation options for configuring how a smart contract is compiled
/// </summary>
public class CompilationOptions
{
    /// <summary>
    /// Gets or sets the path to the contract project file (.csproj) or source file (.cs)
    /// </summary>
    /// <remarks>
    /// This should be an absolute or relative path to either:
    /// - A .csproj file for compiling an entire contract project (recommended)
    /// - A .cs source file for single-file compilation (legacy support)
    /// </remarks>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the project file path when compiling from a project
    /// </summary>
    /// <remarks>
    /// This property is preferred over SourcePath for project-based compilation.
    /// When set, SourcePath is ignored.
    /// </remarks>
    public string? ProjectPath { get; set; }

    /// <summary>
    /// Gets or sets the output directory where compiled artifacts (NEF and manifest) will be saved
    /// </summary>
    /// <remarks>
    /// If the directory doesn't exist, it will be created automatically.
    /// Default is "bin" in the current directory.
    /// </remarks>
    public string OutputDirectory { get; set; } = "bin";

    /// <summary>
    /// Gets or sets the contract name used for output files
    /// </summary>
    /// <remarks>
    /// If not specified, the name will be inferred from the source file name.
    /// This name is used for the .nef and .manifest.json output files.
    /// </remarks>
    public string? ContractName { get; set; }

    /// <summary>
    /// Gets or sets additional assembly references required for compilation
    /// </summary>
    /// <remarks>
    /// Include paths to any custom assemblies your contract depends on.
    /// Standard Neo.SmartContract.Framework references are included automatically.
    /// </remarks>
    public string[] References { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets whether to generate debug information for the contract
    /// </summary>
    /// <remarks>
    /// When true, generates debugging symbols that can be used with Neo debuggers.
    /// Recommended for development and testing. Default is true.
    /// </remarks>
    public bool GenerateDebugInfo { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to optimize the compiled contract bytecode
    /// </summary>
    /// <remarks>
    /// When true, applies optimizations to reduce contract size and execution cost.
    /// Recommended for production deployments. Default is true.
    /// </remarks>
    public bool Optimize { get; set; } = true;
}
