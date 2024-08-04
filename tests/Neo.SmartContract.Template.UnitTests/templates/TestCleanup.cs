using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.TestingStandards;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Template.UnitTests.templates
{
    [TestClass]
    public class TestCleanup : TestCleanupBase
    {
        private static readonly Regex WhiteSpaceRegex = new("\\s");

        [AssemblyCleanup]
        public static void EnsureCoverage() => EnsureCoverageInternal(Assembly.GetExecutingAssembly()); [TestMethod]
        public void EnsureArtifactsUpToDate()
        {
            if (DebugInfos.Count > 0) return; // Maybe a UT call it

            // Define paths

            string frameworkPath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Framework/Neo.SmartContract.Framework.csproj");
            string templatePath = Path.GetFullPath("../../../../../src/Neo.SmartContract.Template/templates");
            string artifactsPath = Path.GetFullPath("../../../templates");

            // Compile

            var result = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(new CompilationSourceReferences() { Projects = [frameworkPath] },
                [
                    Path.Combine(templatePath, "neocontractnep17/Nep17Contract.cs"),
                    Path.Combine(templatePath, "neocontractoracle/OracleRequest.cs"),
                    Path.Combine(templatePath, "neocontractowner/Ownable.cs")
                ]);

            Assert.IsTrue(result.Count() == 3 && result.All(u => u.Success), "Error compiling templates");

            // Ensure Nep17

            var root = Path.GetPathRoot(templatePath) ?? "";
            (var artifact, var dbg) = CreateArtifact<Nep17ContractTemplate>(result.FirstOrDefault(p => p.ContractName == "Nep17Contract") ?? throw new InvalidOperationException(), root,
                Path.Combine(artifactsPath, "neocontractnep17/TestingArtifacts/Nep17ContractTemplate.artifacts.cs"));

            DebugInfos[typeof(Nep17ContractTemplate)] = dbg;

            // Ensure Oracle

            (artifact, dbg) = CreateArtifact<OracleRequestTemplate>(result.FirstOrDefault(p => p.ContractName == "OracleRequest") ?? throw new InvalidOperationException(), root,
                Path.Combine(artifactsPath, "neocontractoracle/TestingArtifacts/OracleRequestTemplate.artifacts.cs"));

            DebugInfos[typeof(OracleRequestTemplate)] = dbg;

            // Ensure Ownable

            (artifact, dbg) = CreateArtifact<OwnableTemplate>(result.FirstOrDefault(p => p.ContractName == "Ownable") ?? throw new InvalidOperationException(), root,
                Path.Combine(artifactsPath, "neocontractowner/TestingArtifacts/OwnableTemplate.artifacts.cs"));

            DebugInfos[typeof(OwnableTemplate)] = dbg;
        }

        private static (string artifact, NeoDebugInfo debugInfo) CreateArtifact<T>(CompilationContext context, string rootDebug, string artifactsPath)
        {
            (var nef, var manifest, var debugInfo) = context.CreateResults(rootDebug);
            var debug = NeoDebugInfo.FromDebugInfoJson(debugInfo);
            var artifact = manifest.GetArtifactsSource(typeof(T).Name, nef, generateProperties: true);

            string writtenArtifact = File.Exists(artifactsPath) ? File.ReadAllText(artifactsPath) : "";
            if (string.IsNullOrEmpty(writtenArtifact) || WhiteSpaceRegex.Replace(artifact, "") != WhiteSpaceRegex.Replace(writtenArtifact, ""))
            {
                // Uncomment to overwrite the artifact file
                File.WriteAllText(artifactsPath, artifact);
                Assert.Fail($"{typeof(T).Name} artifact was wrong");
            }

            return (artifact, debug);
        }
    }
}
