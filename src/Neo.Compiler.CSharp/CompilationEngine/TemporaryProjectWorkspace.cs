// Copyright (C) 2015-2026 The Neo Project.
//
// TemporaryProjectWorkspace.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler
{
    internal sealed class TemporaryProjectWorkspace
    {
        private string? _directory;
        private string? _projectPath;
        private string? _nugetConfigPath;
        private string? _referencesKey;
        private bool _cleanupRegistered;

        internal string ProjectPath =>
            _projectPath ?? throw new InvalidOperationException("Temporary project path has not been initialized.");

        internal void PrepareTransient()
        {
            Cleanup();
            _directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_directory);
            _projectPath = Path.Combine(_directory, "TempProject.csproj");
            _nugetConfigPath = Path.Combine(_directory, "nuget.config");
            WriteNuGetConfig(_nugetConfigPath);
            _referencesKey = null;
        }

        internal void EnsurePersistent(string referencesKey)
        {
            var directoryExists = _directory is not null && Directory.Exists(_directory);
            if (!directoryExists || _referencesKey != referencesKey)
            {
                Cleanup();
                _directory = Path.Combine(Path.GetTempPath(), "Neo.Compiler", "CompileSources", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(_directory);
                _projectPath = Path.Combine(_directory, "TempProject.csproj");
                _nugetConfigPath = Path.Combine(_directory, "nuget.config");
                WriteNuGetConfig(_nugetConfigPath);
                _referencesKey = referencesKey;
                RegisterCleanup();
            }
        }

        internal void WriteProject(CompilationSourceReferences references, string[] sourceFiles)
        {
            if (_directory is null)
                throw new InvalidOperationException("Temporary project directory must be initialized before writing the project file.");

            _projectPath ??= Path.Combine(_directory, "TempProject.csproj");
            _nugetConfigPath ??= Path.Combine(_directory, "nuget.config");

            File.WriteAllText(_projectPath, BuildTempProjectContent(references, sourceFiles));

            if (!File.Exists(_nugetConfigPath))
                WriteNuGetConfig(_nugetConfigPath);
        }

        internal void Cleanup()
        {
            if (_directory is null)
                return;

            try
            {
                if (Directory.Exists(_directory))
                    Directory.Delete(_directory, true);
            }
            catch (IOException)
            {
                // Best-effort cleanup; ignore IO failures.
            }
            catch (UnauthorizedAccessException)
            {
                // Best-effort cleanup; ignore permission failures.
            }

            _directory = null;
            _projectPath = null;
            _nugetConfigPath = null;
            _referencesKey = null;
        }

        internal static string BuildTempProjectContent(CompilationSourceReferences references, string[] sourceFiles)
        {
            var packages = references.Packages;
            var packageGroup = packages is null || packages.Length == 0
                ? string.Empty
                : $@"
    <ItemGroup>
        {string.Join(Environment.NewLine, packages.Select(u => $" <PackageReference Include =\"{u.packageName}\" Version=\"{u.packageVersion}\" />"))}
    </ItemGroup>";

            var projects = references.Projects;
            var projectsGroup = projects is null || projects.Length == 0
                ? string.Empty
                : $@"
    <ItemGroup>
        {string.Join(Environment.NewLine, projects.Select(u => $" <ProjectReference Include =\"{u}\"/>"))}
    </ItemGroup>";

            return $@"
<Project Sdk=""Microsoft.NET.Sdk"">

    <PropertyGroup>
        <TargetFramework>{RuntimeAssemblyResolver.CompilerTargetFrameworkMoniker}</TargetFramework>
        <LangVersion>preview</LangVersion>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <!-- Remove all Compile items from compilation -->
    <ItemGroup>
        <Compile Remove=""*.cs"" />
    </ItemGroup>

    <!-- Add specific files for compilation -->
    <ItemGroup>
        {string.Join(Environment.NewLine, sourceFiles.Select(u => $"<Compile Include=\"{Path.GetFullPath(u)}\" />"))}
    </ItemGroup>

    {packageGroup}
    {projectsGroup}

</Project>";
        }

        internal static string BuildReferencesKey(CompilationSourceReferences references)
        {
            var builder = new StringBuilder();

            if (references.Packages is { Length: > 0 })
            {
                foreach (var (packageName, packageVersion) in references.Packages
                             .OrderBy(p => p.packageName, StringComparer.Ordinal))
                {
                    builder.Append("pkg:")
                        .Append(packageName)
                        .Append('@')
                        .Append(packageVersion)
                        .Append(';');
                }
            }

            builder.Append('|');

            if (references.Projects is { Length: > 0 })
            {
                foreach (var project in references.Projects
                             .Select(Path.GetFullPath)
                             .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
                {
                    builder.Append("proj:")
                        .Append(project)
                        .Append(';');
                }
            }

            return builder.ToString();
        }

        private static void WriteNuGetConfig(string nugetConfigPath)
        {
            const string nugetConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""NuGet.org"" value=""https://api.nuget.org/v3/index.json"" protocolVersion=""3"" />
  </packageSources>
</configuration>";

            File.WriteAllText(nugetConfigPath, nugetConfigContent);
        }

        private void RegisterCleanup()
        {
            if (_cleanupRegistered)
                return;

            _cleanupRegistered = true;
            AppDomain.CurrentDomain.ProcessExit += (_, _) => Cleanup();
        }
    }
}
