using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Neo.Compiler;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;
using Neo.SmartContract.Framework;

string source = """
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_IntForeach : SmartContract.Framework.SmartContract
    {
        public static int IntForeach()
        {
            int[] values = new int[] { 1, 2, 3, 4 };
            var sum = 0;
            foreach (var item in values)
                sum += item;
            return sum;
        }
    }
}
""";

var tempFile = Path.Combine(Path.GetTempPath(), $"foreach_{Guid.NewGuid():N}.cs");
await File.WriteAllTextAsync(tempFile, source);

try
{
    Log("Starting compilation...");
    var contexts = CompileSource(tempFile);
    Log($"Compilation produced {contexts.Count} context(s).");

    var context = contexts.Single();
    if (context.MirModule is null)
    {
        Log("MIR module was not produced.");
        return;
    }

    var functionKey = context.MirModule.Functions.Keys.Single(k => k.Contains("IntForeach", StringComparison.Ordinal));
    var function = context.MirModule.Functions[functionKey];

    Log($"Loaded MIR function '{functionKey}' with {function.Blocks.Count} blocks.");

    RunPipelineWithDiagnostics(function);
}
finally
{
    if (File.Exists(tempFile))
        File.Delete(tempFile);
}

static IReadOnlyList<CompilationContext> CompileSource(string file)
{
    var options = new Neo.Compiler.CompilationOptions
    {
        EnableHir = true,
        Optimize = Neo.Compiler.CompilationOptions.OptimizationType.None,
        SkipContractValidation = true,
        Nullable = NullableContextOptions.Enable,
        Logger = msg => Log($"[compiler] {msg}")
    };

    var engine = new CompilationEngine(options);
    var references = BuildReferences().Select(path => MetadataReference.CreateFromFile(path)).ToArray();
    var sw = Stopwatch.StartNew();
    var contexts = engine.Compile(new[] { file }, references);
    sw.Stop();
    Log($"Compilation finished in {sw.Elapsed}.");
    return contexts;
}

static IEnumerable<string> BuildReferences()
{
    var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
    yield return Path.Combine(runtimeDir, "System.Runtime.dll");
    yield return Path.Combine(runtimeDir, "System.Runtime.InteropServices.dll");
    yield return typeof(string).Assembly.Location;
    yield return typeof(System.ComponentModel.DisplayNameAttribute).Assembly.Location;
    yield return typeof(System.Numerics.BigInteger).Assembly.Location;
    yield return typeof(SmartContract).Assembly.Location;
    yield return typeof(Neo.SmartContract.Framework.Services.Storage).Assembly.Location;
}

static void RunPipelineWithDiagnostics(MirFunction function)
{
    var passesField = typeof(MirOptimizationPipeline).GetField("s_passes", BindingFlags.NonPublic | BindingFlags.Static);
    if (passesField?.GetValue(null) is not IMirPass[] rawPasses)
    {
        Log("Unable to reflect MirOptimizationPipeline passes.");
        return;
    }

    var passes = rawPasses.Select((pass, index) => (IMirPass)new Neo.Compiler.CSharp.MirHarness.PassTracer(pass, index)).ToArray();

    var iteration = 0;
    var changedAny = false;

    do
    {
        iteration++;
        Log($"=== Iteration {iteration} ===");
        var changedIteration = false;

        foreach (var pass in passes)
        {
            var sw = Stopwatch.StartNew();
            Log($"Running {pass.GetType().Name} ...");
            bool changed;
            try
            {
                changed = pass.Run(function);
            }
            catch (Exception ex)
            {
                Log($"Pass {pass.GetType().Name} threw: {ex}");
                return;
            }
            sw.Stop();
            Log($"  Completed in {sw.Elapsed} (changed={changed}).");

            MirMemoryTokenRetargeter.Retokenize(function);
            MirPhiUtilities.PruneNonPredecessorInputs(function);

            var errors = new MirVerifier().Verify(function);
            if (errors.Count > 0)
            {
                Log($"  Verifier errors after {pass.GetType().Name}:");
                foreach (var error in errors)
                    Log($"    {error}");
                return;
            }

            if (changed)
            {
                changedIteration = true;
                changedAny = true;
            }
        }

        if (!changedIteration)
        {
            Log("No changes in iteration; pipeline complete.");
            break;
        }

        if (iteration >= 8)
        {
            Log("Stopping after 8 iterations to avoid infinite loop.");
            break;
        }
    }
    while (true);

    Log($"Pipeline finished. changedAny={changedAny}");
}

static void Log(string message)
{
    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] {message}");
}
