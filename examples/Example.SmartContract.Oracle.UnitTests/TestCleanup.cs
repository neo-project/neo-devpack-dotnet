using Neo.Compiler;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;

namespace Example.SmartContract.Oracle.UnitTests
{
    public class TestCleanup
    {
        internal static (NefFile nef, ContractManifest manifest) EnsureArtifactsUpToDateInternal()
        {
            // Define paths
            string testContractsName = System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!.Namespace!.Replace(".UnitTests", "");
            string testContractsPath = Path.GetFullPath($"../../../../{testContractsName}/{testContractsName}.csproj");
            string artifactsPath = Path.GetFullPath("../../../TestingArtifacts");
            var root = Path.GetPathRoot(testContractsPath) ?? "";

            // Compile
            var compilationContexts = new CompilationEngine(new CompilationOptions
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileProject(testContractsPath);

            // Ensure that all was well compiled
            if (!compilationContexts.All(u => u.Success))
            {
                compilationContexts.SelectMany(u => u.Diagnostics)
                    .Where(u => u.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                    .ToList().ForEach(Console.Error.WriteLine);

                Assert.Fail("Error compiling templates");
            }

            // Get all artifacts loaded in this assembly
            var result = compilationContexts.FirstOrDefault() ?? throw new ArgumentNullException($"Compilation context is null");
            // Ensure that it exists
            return CreateArtifact(result.ContractName!, result, root, Path.Combine(artifactsPath, $"{result.ContractName}.cs"));
        }

        private static (NefFile nef, ContractManifest manifest) CreateArtifact(string typeName, CompilationContext context, string rootDebug, string artifactsPath)
        {
            var (nef, manifest, _) = context.CreateResults(rootDebug);
            var artifact = manifest.GetArtifactsSource(typeName, nef, generateProperties: true);

            if (!File.Exists(artifactsPath))
            {
                var directoryPath = Path.GetDirectoryName(artifactsPath) ?? throw new ArgumentNullException($"{nameof(artifactsPath)} directory path is null");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.Create(artifactsPath).Close();
            }

            try
            {
                File.WriteAllText(artifactsPath, artifact);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return (nef, manifest);
        }
    }
}
