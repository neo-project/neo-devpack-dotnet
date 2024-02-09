using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Neo.Compiler
{
    public class CompilationResult
    {
        /// <summary>
        /// Nef
        /// </summary>
        public required NefFile Nef { get; init; }

        /// <summary>
        /// Contract Manifest
        /// </summary>
        public required ContractManifest Manifest { get; init; }

        /// <summary>
        /// Debug Info
        /// </summary>
        public required JObject DebugInfo { get; init; }
    }
}