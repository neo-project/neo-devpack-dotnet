using Neo;
using Neo.Compiler;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using System.Text;

namespace Neo.DevPack.Fuzz.Targets;

internal static class StructuredContractBuilder
{
    public static GeneratedSourceProject Build(ReadOnlySpan<byte> input)
    {
        var random = Deterministic.CreateRandom(input);
        var includeInterface = random.Next(2) == 0;
        var includePartialFile = random.Next(2) == 0;
        var includeLogMethod = random.Next(2) == 0;
        var contractName = "Contract";
        var interfaceConstant = 1 + random.Next(31);
        var arithmeticBase = 3 + random.Next(64);
        var arithmeticFactor = 1 + random.Next(9);
        var loopBound = 1 + random.Next(8);
        var partialBase = 5 + random.Next(32);
        var partialOffset = 1 + random.Next(17);
        var message = CreateStringLiteral(random, 5 + random.Next(12));
        var pingTag = CreateStringLiteral(random, 3 + random.Next(8));
        var touchTag = CreateStringLiteral(random, 3 + random.Next(8));

        var mainSource = new StringBuilder();
        mainSource.AppendLine("using Neo.SmartContract.Framework;");
        mainSource.AppendLine("using Neo.SmartContract.Framework.Attributes;");
        mainSource.AppendLine("using Neo.SmartContract.Framework.Services;");
        mainSource.AppendLine("using System.ComponentModel;");
        mainSource.AppendLine();

        if (includeInterface)
        {
            mainSource.AppendLine("public interface IFuzzContractValue");
            mainSource.AppendLine("{");
            mainSource.AppendLine("    [DisplayName(\"ifaceValue\")]");
            mainSource.AppendLine("    int InterfaceValue()");
            mainSource.AppendLine("    {");
            mainSource.AppendLine($"        return {interfaceConstant};");
            mainSource.AppendLine("    }");
            mainSource.AppendLine("}");
            mainSource.AppendLine();
        }

        mainSource.AppendLine($"public partial class {contractName} : SmartContract{(includeInterface ? ", IFuzzContractValue" : string.Empty)}");
        mainSource.AppendLine("{");
        mainSource.AppendLine("    [DisplayName(\"touchSeed\")]");
        mainSource.AppendLine("    public static void TouchSeed()");
        mainSource.AppendLine("    {");
        mainSource.AppendLine("        var map = new StorageMap(Storage.CurrentContext, \"fz\");");
        mainSource.AppendLine($"        map.Put(\"seed\", \"{touchTag}\");");
        mainSource.AppendLine("    }");
        mainSource.AppendLine();
        mainSource.AppendLine("    [Safe]");
        mainSource.AppendLine("    [DisplayName(\"hasSeed\")]");
        mainSource.AppendLine("    public static bool HasSeed()");
        mainSource.AppendLine("    {");
        mainSource.AppendLine("        var map = new StorageMap(Storage.CurrentReadOnlyContext, \"fz\");");
        mainSource.AppendLine("        return map.Get(\"seed\") != null;");
        mainSource.AppendLine("    }");
        mainSource.AppendLine();
        mainSource.AppendLine("    [Safe]");
        mainSource.AppendLine("    [DisplayName(\"ping\")]");
        mainSource.AppendLine("    public static string Ping()");
        mainSource.AppendLine("    {");
        mainSource.AppendLine($"        int mixed = FuzzMath.Mix({arithmeticBase}, {arithmeticFactor});");
        mainSource.AppendLine($"        return \"{message}:\" + mixed + \":{pingTag}\";");
        mainSource.AppendLine("    }");
        mainSource.AppendLine();
        mainSource.AppendLine("    [Safe]");
        mainSource.AppendLine("    [DisplayName(\"arithmetic\")]");
        mainSource.AppendLine("    public static int Arithmetic()");
        mainSource.AppendLine("    {");
        mainSource.AppendLine($"        int total = {arithmeticBase};");
        mainSource.AppendLine($"        for (int i = 0; i < {loopBound}; i++)");
        mainSource.AppendLine("        {");
        mainSource.AppendLine($"            total = FuzzMath.Mix(total, i + {arithmeticFactor});");
        mainSource.AppendLine("        }");
        mainSource.AppendLine("        if ((total & 1) == 0)");
        mainSource.AppendLine("        {");
        mainSource.AppendLine("            total += 3;");
        mainSource.AppendLine("        }");
        mainSource.AppendLine("        else");
        mainSource.AppendLine("        {");
        mainSource.AppendLine("            total -= 1;");
        mainSource.AppendLine("        }");
        mainSource.AppendLine("        return total;");
        mainSource.AppendLine("    }");
        mainSource.AppendLine();
        mainSource.AppendLine("    [Safe]");
        mainSource.AppendLine("    [DisplayName(\"flag\")]");
        mainSource.AppendLine("    public static bool Flag()");
        mainSource.AppendLine("    {");
        mainSource.AppendLine($"        return ({arithmeticBase} % 2 == 0) || ({loopBound} > 1);");
        mainSource.AppendLine("    }");

        if (includeLogMethod)
        {
            mainSource.AppendLine();
            mainSource.AppendLine("    [DisplayName(\"emitLog\")]");
            mainSource.AppendLine("    public static void EmitLog()");
            mainSource.AppendLine("    {");
            mainSource.AppendLine($"        Runtime.Log(\"{message}\");");
            mainSource.AppendLine("    }");
        }

        mainSource.AppendLine("}");

        var files = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Contract.cs"] = mainSource.ToString()
        };

        var plans = new List<InvocationPlan>
        {
            new("touchSeed", System.Array.Empty<object?>()),
            new("hasSeed", System.Array.Empty<object?>()),
            new("ping", System.Array.Empty<object?>()),
            new("arithmetic", System.Array.Empty<object?>()),
            new("flag", System.Array.Empty<object?>())
        };

        if (includeInterface)
        {
            plans.Add(new InvocationPlan("ifaceValue", System.Array.Empty<object?>()));
        }

        if (includeLogMethod)
        {
            plans.Add(new InvocationPlan("emitLog", System.Array.Empty<object?>()));
        }

        if (includePartialFile)
        {
            files["Contract.Partial.cs"] =
                $$"""
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Attributes;
                using System.ComponentModel;

                public partial class {{contractName}}
                {
                    [Safe]
                    [DisplayName("extra")]
                    public static int Extra()
                    {
                        return FuzzMath.Mix({{partialBase}}, {{partialOffset}});
                    }
                }

                internal static class FuzzMath
                {
                    internal static int Mix(int left, int right)
                    {
                        if ((left & 1) == 0)
                        {
                            return left + right;
                        }

                        return left - right;
                    }
                }
                """;

            plans.Add(new InvocationPlan("extra", System.Array.Empty<object?>()));
        }
        else
        {
            files["FuzzMath.cs"] =
                """
                internal static class FuzzMath
                {
                    internal static int Mix(int left, int right)
                    {
                        if ((left & 1) == 0)
                        {
                            return left + right;
                        }

                        return left - right;
                    }
                }
                """;
        }

        return new GeneratedSourceProject(contractName, files, plans);
    }

    private static string CreateStringLiteral(Random random, int length)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        var builder = new StringBuilder(length);
        for (var index = 0; index < length; index++)
        {
            builder.Append(alphabet[random.Next(alphabet.Length)]);
        }

        return builder.ToString();
    }
}

internal sealed class StructuredCompileTarget : CompilationTargetBase
{
    public StructuredCompileTarget(RepoLayout layout)
        : base(layout, "structured-compile")
    {
    }

    public override string Name => "fuzz_structured_compile";

    public override string Description => "Generate valid smart contracts and run them through the full compiler pipeline.";

    public override FuzzCaseResult Execute(byte[] input)
    {
        var project = StructuredContractBuilder.Build(input);

        try
        {
            var contexts = CompileSources(project, CreateOptions(input));
            return SummarizeCompilation(Name, project, contexts);
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException)
        {
            return SummarizeBenignFailure(Name, ex, project);
        }
        catch (Exception ex)
        {
            throw new FuzzCrashException(
                $"{Name}: unexpected structured compile crash",
                ex,
                CreateSourceArtifacts(project));
        }
    }
}

internal sealed class DeterminismTarget : CompilationTargetBase
{
    public DeterminismTarget(RepoLayout layout)
        : base(layout, "determinism")
    {
    }

    public override string Name => "fuzz_differential";

    public override string Description => "Compile the same generated contract twice and assert deterministic outputs.";

    public override FuzzCaseResult Execute(byte[] input)
    {
        var project = StructuredContractBuilder.Build(input);
        var options = CreateOptions(input);

        try
        {
            var first = CompileSources(project, options);
            var second = CompileSources(project, options);
            var firstDiagnostics = SerializeDiagnostics(first);
            var secondDiagnostics = SerializeDiagnostics(second);

            if (!string.Equals(firstDiagnostics, secondDiagnostics, StringComparison.Ordinal))
            {
                throw new FuzzCrashException(
                    $"{Name}: diagnostic mismatch between identical compilations",
                    new InvalidOperationException("Compiler diagnostics changed between runs."),
                    new Dictionary<string, string>(CreateSourceArtifacts(project), StringComparer.Ordinal)
                    {
                        ["diagnostics.first.txt"] = firstDiagnostics,
                        ["diagnostics.second.txt"] = secondDiagnostics
                    });
            }

            if (first.Any(static context => !context.Success) || second.Any(static context => !context.Success))
            {
                return new FuzzCaseResult(
                    $"{Name}:diag:{Deterministic.HashHex(firstDiagnostics)}",
                    $"{Name}: deterministic diagnostic result",
                    TextArtifacts: new Dictionary<string, string>(CreateSourceArtifacts(project), StringComparer.Ordinal)
                    {
                        ["diagnostics.txt"] = firstDiagnostics
                    });
            }

            var firstSnapshots = CaptureSnapshots(first, Layout.RootDirectory);
            var secondSnapshots = CaptureSnapshots(second, Layout.RootDirectory);

            if (firstSnapshots.Count != secondSnapshots.Count)
            {
                throw new FuzzCrashException(
                    $"{Name}: contract count mismatch between identical compilations",
                    new InvalidOperationException("Compiler returned a different number of contracts."),
                    new Dictionary<string, string>(CreateSourceArtifacts(project), StringComparer.Ordinal)
                    {
                        ["first.count.txt"] = firstSnapshots.Count.ToString(),
                        ["second.count.txt"] = secondSnapshots.Count.ToString()
                    });
            }

            for (var index = 0; index < firstSnapshots.Count; index++)
            {
                var firstSnapshot = firstSnapshots[index];
                var secondSnapshot = secondSnapshots[index];

                if (!string.Equals(firstSnapshot.Fingerprint, secondSnapshot.Fingerprint, StringComparison.Ordinal))
                {
                    throw new FuzzCrashException(
                        $"{Name}: output mismatch for contract '{firstSnapshot.ContractName}'",
                        new InvalidOperationException("Compiler produced different outputs for the same input."),
                        new Dictionary<string, string>(CreateSourceArtifacts(project), StringComparer.Ordinal)
                        {
                            [$"{firstSnapshot.ContractName}.first.manifest.json"] = firstSnapshot.ManifestJson,
                            [$"{firstSnapshot.ContractName}.second.manifest.json"] = secondSnapshot.ManifestJson,
                            [$"{firstSnapshot.ContractName}.first.debug.json"] = firstSnapshot.DebugJson,
                            [$"{firstSnapshot.ContractName}.second.debug.json"] = secondSnapshot.DebugJson,
                            [$"{firstSnapshot.ContractName}.first.nef.txt"] = firstSnapshot.AssemblyText,
                            [$"{firstSnapshot.ContractName}.second.nef.txt"] = secondSnapshot.AssemblyText,
                            [$"{firstSnapshot.ContractName}.first.abi.cs"] = firstSnapshot.InterfaceCode,
                            [$"{firstSnapshot.ContractName}.second.abi.cs"] = secondSnapshot.InterfaceCode
                        },
                        new Dictionary<string, byte[]>(StringComparer.Ordinal)
                        {
                            [$"{firstSnapshot.ContractName}.first.nef"] = firstSnapshot.NefBytes,
                            [$"{firstSnapshot.ContractName}.second.nef"] = secondSnapshot.NefBytes
                        });
                }
            }

            return new FuzzCaseResult(
                $"{Name}:ok:{Deterministic.JoinFingerprints(firstSnapshots.Select(static snapshot => snapshot.Fingerprint))}",
                $"{Name}: deterministic across {firstSnapshots.Count} contracts",
                TextArtifacts: CreateSourceArtifacts(project));
        }
        catch (FuzzCrashException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FuzzCrashException(
                $"{Name}: unexpected differential target crash",
                ex,
                CreateSourceArtifacts(project));
        }
    }
}

internal sealed class DevpackRuntimeTarget : CompilationTargetBase
{
    public DevpackRuntimeTarget(RepoLayout layout)
        : base(layout, "devpack-runtime")
    {
    }

    public override string Name => "fuzz_devpack_runtime";

    public override string Description => "Compile generated contracts, deploy them into TestEngine, and invoke stable methods.";

    public override FuzzCaseResult Execute(byte[] input)
    {
        var project = StructuredContractBuilder.Build(input);

        try
        {
            var contexts = CompileSources(project, CreateOptions(input));
            if (contexts.Count == 0 || contexts.Any(static context => !context.Success))
            {
                return SummarizeCompilation(Name, project, contexts);
            }

            var context = contexts[0];
            var (nef, manifest, debugInfo) = context.CreateResults(Layout.RootDirectory);
            _ = context.CreateAssembly();
            var contractHash = UInt160.Parse(debugInfo["hash"]!.GetString());
            _ = ContractInterfaceGenerator.GenerateInterface(context.ContractName ?? manifest.Name, manifest, contractHash);
            _ = manifest.GetArtifactsSource(context.ContractName, nef, debugInfo: debugInfo);

            var engine = new TestEngine(true)
            {
                EnableCoverageCapture = false
            };

            var state = engine.Native.ContractManagement.Deploy(
                nef.ToArray(),
                Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)))
                ?? throw new InvalidOperationException("Contract deployment returned null state.");

            var callSummaries = new List<string>();
            foreach (var plan in project.InvocationPlans)
            {
                if (manifest.Abi.GetMethod(plan.MethodName, plan.Arguments.Length) is null)
                {
                    continue;
                }

                using var script = new ScriptBuilder();
                script.EmitDynamicCall(state.Hash, plan.MethodName, plan.Arguments);
                var result = engine.Execute(script.ToArray());
                callSummaries.Add($"{plan.MethodName}:{Describe(result)}");
            }

            var sourceArtifacts = new Dictionary<string, string>(CreateSourceArtifacts(project), StringComparer.Ordinal)
            {
                ["runtime.txt"] = string.Join(Environment.NewLine, callSummaries)
            };

            return new FuzzCaseResult(
                $"{Name}:ok:{Deterministic.HashHex(string.Join("|", callSummaries))}:{Deterministic.HashHex(nef.ToArray())}",
                $"{Name}: deploy-and-call methods={callSummaries.Count}",
                TextArtifacts: sourceArtifacts);
        }
        catch (Exception ex)
        {
            throw new FuzzCrashException(
                $"{Name}: unexpected devpack runtime crash",
                ex,
                CreateSourceArtifacts(project));
        }
    }

    private static string Describe(Neo.VM.Types.StackItem item)
    {
        return item switch
        {
            Neo.VM.Types.Null => "null",
            Neo.VM.Types.Boolean boolean => $"bool:{boolean.GetBoolean()}",
            Neo.VM.Types.Integer integer => $"int:{integer.GetInteger()}",
            Neo.VM.Types.ByteString byteString => $"bytes:{Encoding.UTF8.GetString(byteString.GetSpan())}",
            Neo.VM.Types.Buffer buffer => $"bytes:{Encoding.UTF8.GetString(buffer.GetSpan())}",
            _ => item.GetType().Name
        };
    }
}
