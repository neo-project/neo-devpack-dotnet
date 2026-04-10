using Neo.Compiler;
using Neo.Extensions;
using Neo.SmartContract.Testing.Extensions;
using System.Text;

namespace Neo.DevPack.Fuzz.Targets;

internal sealed class TemplateProjectTarget : CompilationTargetBase
{
    private readonly TemplateManager _templateManager = new();
    private readonly string _workspaceRoot;
    private const string ProjectName = "TemplateFuzz";

    public TemplateProjectTarget(RepoLayout layout)
        : base(layout, "template-project")
    {
        _workspaceRoot = Path.Combine(Path.GetTempPath(), "Neo.Compiler", "Fuzz", "template-project");
        Directory.CreateDirectory(_workspaceRoot);
    }

    public override string Name => "fuzz_template_projects";

    public override string Description => "Generate devpack templates, patch them to the local framework project, and compile them as projects.";

    public override FuzzCaseResult Execute(byte[] input)
    {
        var template = SelectTemplate(input);
        var projectDirectory = Path.Combine(_workspaceRoot, ProjectName);
        var options = CreateOptions(input);

        try
        {
            if (Directory.Exists(projectDirectory))
            {
                Directory.Delete(projectDirectory, true);
            }

            GenerateTemplateProject(template);
            PatchTemplateProject(projectDirectory);

            var engine = CreateProjectEngine(options);
            var contexts = engine.CompileProject(Path.Combine(projectDirectory, $"{ProjectName}.csproj"));
            var textArtifacts = ReadProjectFiles(projectDirectory);

            if (contexts.Count == 0)
            {
                return new FuzzCaseResult(
                    $"{Name}:no-context:{template}",
                    $"{Name}: template={template} returned no contract contexts.",
                    TextArtifacts: textArtifacts);
            }

            if (contexts.Any(static context => !context.Success))
            {
                textArtifacts["diagnostics.txt"] = SerializeDiagnostics(contexts);
                return new FuzzCaseResult(
                    $"{Name}:diag:{template}:{Deterministic.HashHex(textArtifacts["diagnostics.txt"])}",
                    $"{Name}: template={template} diagnostics",
                    TextArtifacts: textArtifacts);
            }

            var snapshots = CaptureSnapshots(contexts, projectDirectory);
            foreach (var snapshot in snapshots)
            {
                textArtifacts[$"{snapshot.ContractName}.manifest.json"] = snapshot.ManifestJson;
                textArtifacts[$"{snapshot.ContractName}.debug.json"] = snapshot.DebugJson;
                textArtifacts[$"{snapshot.ContractName}.abi.cs"] = snapshot.InterfaceCode;
                textArtifacts[$"{snapshot.ContractName}.artifacts.cs"] = snapshot.ArtifactSource;
                textArtifacts[$"{snapshot.ContractName}.nef.txt"] = snapshot.AssemblyText;
            }

            return new FuzzCaseResult(
                $"{Name}:ok:{template}:{Deterministic.JoinFingerprints(snapshots.Select(static snapshot => snapshot.Fingerprint))}",
                $"{Name}: template={template} contracts={snapshots.Count}",
                TextArtifacts: textArtifacts,
                BinaryArtifacts: snapshots.ToDictionary(
                    static snapshot => $"{snapshot.ContractName}.nef",
                    static snapshot => snapshot.NefBytes,
                    StringComparer.Ordinal));
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException)
        {
            return new FuzzCaseResult(
                $"{Name}:failure:{template}:{Deterministic.HashHex(ex.ToString())}",
                $"{Name}: template={template} {ex.GetType().Name}: {ex.Message}",
                TextArtifacts: ReadProjectFiles(projectDirectory));
        }
        catch (Exception ex)
        {
            throw new FuzzCrashException(
                $"{Name}: unexpected template project crash for {template}",
                ex,
                ReadProjectFiles(projectDirectory));
        }
    }

    private void GenerateTemplateProject(ContractTemplate template)
    {
        var previousOut = Console.Out;
        try
        {
            Console.SetOut(TextWriter.Null);
            _templateManager.GenerateContract(
                template,
                ProjectName,
                _workspaceRoot,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["{{Author}}"] = "fuzz-author",
                    ["{{Email}}"] = "fuzz@example.com",
                    ["{{Description}}"] = $"{template} template fuzz target"
                });
        }
        finally
        {
            Console.SetOut(previousOut);
        }
    }

    private void PatchTemplateProject(string projectDirectory)
    {
        var projectPath = Path.Combine(projectDirectory, $"{ProjectName}.csproj");
        var projectText = File.ReadAllText(projectPath);
        projectText = projectText.Replace(
            "<PackageReference Include=\"Neo.SmartContract.Framework\" Version=\"3.9.0\" />",
            $"<ProjectReference Include=\"{Layout.FrameworkProjectPath}\" />",
            StringComparison.Ordinal);
        File.WriteAllText(projectPath, projectText);
    }

    private static ContractTemplate SelectTemplate(ReadOnlySpan<byte> input)
    {
        return input.Length > 0
            ? (ContractTemplate)(input[0] % Enum.GetValues<ContractTemplate>().Length)
            : ContractTemplate.Basic;
    }

    private static Dictionary<string, string> ReadProjectFiles(string projectDirectory)
    {
        if (!Directory.Exists(projectDirectory))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        return Directory.EnumerateFiles(projectDirectory, "*", SearchOption.AllDirectories)
            .Where(static path => path.EndsWith(".cs", StringComparison.Ordinal) || path.EndsWith(".csproj", StringComparison.Ordinal))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .ToDictionary(
                path => Path.GetRelativePath(projectDirectory, path),
                File.ReadAllText,
                StringComparer.Ordinal);
    }
}
