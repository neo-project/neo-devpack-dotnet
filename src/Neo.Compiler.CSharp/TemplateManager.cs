// Copyright (C) 2015-2025 The Neo Project.
//
// TemplateManager.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler
{
    public enum ContractTemplate
    {
        Basic,
        NEP17,
        NEP11,
        Ownable,
        Oracle
    }

    public sealed class TemplateManager
    {
        public const string DefaultFrameworkPackageVersion = "3.8.1-*";
        public const string DefaultTestSdkVersion = "17.14.1";
        public const string DefaultMSTestAdapterVersion = "3.8.0";
        public const string DefaultMSTestFrameworkVersion = "3.8.0";
        public const string DefaultCoverletCollectorVersion = "6.0.2";

        private enum ContractFeature
        {
            Basic,
            NEP17,
            NEP11,
            Ownable,
            Oracle
        }

        private static readonly IReadOnlyDictionary<string, ContractFeature> FeatureAliases =
            new Dictionary<string, ContractFeature>(StringComparer.OrdinalIgnoreCase)
            {
                ["basic"] = ContractFeature.Basic,
                ["core"] = ContractFeature.Basic,
                ["default"] = ContractFeature.Basic,
                ["nep17"] = ContractFeature.NEP17,
                ["nep-17"] = ContractFeature.NEP17,
                ["token"] = ContractFeature.NEP17,
                ["fungible"] = ContractFeature.NEP17,
                ["nep11"] = ContractFeature.NEP11,
                ["nep-11"] = ContractFeature.NEP11,
                ["nft"] = ContractFeature.NEP11,
                ["collectible"] = ContractFeature.NEP11,
                ["ownable"] = ContractFeature.Ownable,
                ["ownership"] = ContractFeature.Ownable,
                ["oracle"] = ContractFeature.Oracle,
                ["oracle-client"] = ContractFeature.Oracle,
                ["oraclefeed"] = ContractFeature.Oracle
            };

        private readonly IReadOnlyDictionary<ContractTemplate, TemplateInfo> templates;

        public TemplateManager()
        {
            var templateDict = new Dictionary<ContractTemplate, TemplateInfo>
            {
                [ContractTemplate.Basic] = new TemplateInfo(
                    "Basic",
                    "A foundational contract with simple storage, permissions, and lifecycle management.",
                    _ => new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetBasicContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }),
                [ContractTemplate.NEP17] = new TemplateInfo(
                    "NEP17",
                    "A fungible token implementation with owner controls and an initial supply mint.",
                    _ => new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetNep17ContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }),
                [ContractTemplate.NEP11] = new TemplateInfo(
                    "NEP11",
                    "A non-fungible token template with metadata storage and owner management.",
                    _ => new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetNep11ContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }),
                [ContractTemplate.Ownable] = new TemplateInfo(
                    "Ownable",
                    "A secure ownable contract for privileged administration and controlled upgrades.",
                    _ => new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetOwnableContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }),
                [ContractTemplate.Oracle] = new TemplateInfo(
                    "Oracle",
                    "Interact with the NEO oracle to persist off-chain responses securely.",
                    _ => new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetOracleContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    })
            };

            templates = templateDict;
        }

        public void GenerateContract(
            ContractTemplate template,
            string projectName,
            string outputPath,
            Dictionary<string, string>? additionalReplacements = null,
            bool includeTests = false)
        {
            if (!templates.TryGetValue(template, out var templateInfo))
            {
                throw new ArgumentException($"Unknown template: {template}", nameof(template));
            }

            var context = new TemplateContext(projectName, outputPath, additionalReplacements);
            GenerateFromTemplate(templateInfo, context, includeTests);
        }

        public IEnumerable<(ContractTemplate template, string name, string description)> GetAvailableTemplates()
        {
            return templates.Select(t => (t.Key, t.Value.Name, t.Value.Description));
        }

        public void GenerateContractFromFeatures(
            IEnumerable<string> featureNames,
            string projectName,
            string outputPath,
            Dictionary<string, string>? additionalReplacements = null,
            bool includeTests = false)
        {
            var features = NormalizeFeatures(featureNames).ToList();

            if (features.Count == 0)
            {
                features.Add(ContractFeature.Basic);
            }

            features.Sort();

            ValidateFeatureSet(features);

            var templateInfo = CreateFeatureTemplate(features);
            var context = new TemplateContext(projectName, outputPath, additionalReplacements);
            var displayFeatures = GetDisplayFeatures(features);
            string descriptor = $"feature mix ({string.Join(", ", displayFeatures.Select(GetFeatureDisplayName))})";
            GenerateFromTemplate(templateInfo, context, includeTests, descriptor);
        }

        private static void ValidateFeatureSet(IReadOnlyList<ContractFeature> features)
        {
            bool hasNep17 = features.Contains(ContractFeature.NEP17);
            bool hasNep11 = features.Contains(ContractFeature.NEP11);
            if (hasNep17 && hasNep11)
            {
                throw new ArgumentException("NEP-17 and NEP-11 features cannot be combined. Choose a single token standard.");
            }

            bool hasOracle = features.Contains(ContractFeature.Oracle);
            bool hasOwnable = features.Contains(ContractFeature.Ownable);

            if (hasOracle && hasNep11)
            {
                throw new ArgumentException("Oracle feature is not currently supported with NEP-11 templates.");
            }

            if (hasOracle && hasNep17 && !hasOwnable)
            {
                throw new ArgumentException("The NEP-17 oracle feature requires Ownable for signer management. Include the 'Ownable' feature when combining NEP17 and Oracle.");
            }
        }

        private static IReadOnlyList<ContractFeature> GetDisplayFeatures(IReadOnlyList<ContractFeature> features)
        {
            bool hasTokenBase = features.Contains(ContractFeature.NEP17) || features.Contains(ContractFeature.NEP11);
            if (!hasTokenBase)
            {
                return features;
            }

            return features.Where(f => f != ContractFeature.Basic).ToArray();
        }

        private TemplateInfo CreateFeatureTemplate(IReadOnlyList<ContractFeature> features)
        {
            bool hasNep17 = features.Contains(ContractFeature.NEP17);
            bool hasNep11 = features.Contains(ContractFeature.NEP11);
            bool hasOwnable = features.Contains(ContractFeature.Ownable);
            bool hasOracle = features.Contains(ContractFeature.Oracle);

            string contractSource = hasNep17
                ? ComposeNep17Contract(hasOwnable, hasOracle)
                : hasNep11
                    ? ComposeNep11Contract(hasOwnable, hasOracle)
                    : ComposeBasicContract(hasOwnable, hasOracle);

            var displayFeatures = GetDisplayFeatures(features);
            string featureDescription = string.Join(", ", displayFeatures.Select(GetFeatureDisplayName));
            if (string.IsNullOrEmpty(featureDescription))
            {
                featureDescription = GetFeatureDisplayName(ContractFeature.Basic);
            }

            return new TemplateInfo(
                $"Feature-based Contract ({featureDescription})",
                $"Contract generated from features: {featureDescription}.",
                _ => new Dictionary<string, string>
                {
                    ["{{ProjectName}}.cs"] = contractSource,
                    ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                });
        }

        private static string ComposeBasicContract(bool hasOwnable, bool hasOracle)
        {
            if (hasOwnable && hasOracle)
                return GetOwnableOracleTemplate();

            if (hasOwnable)
                return GetOwnableContractTemplate();

            if (hasOracle)
                return GetOracleContractTemplate();

            return GetBasicContractTemplate();
        }

        private static string ComposeNep17Contract(bool hasOwnable, bool hasOracle)
        {
            if (hasOracle)
                return GetNep17OwnableOracleTemplate();

            return GetNep17ContractTemplate();
        }

        private static string ComposeNep11Contract(bool hasOwnable, bool hasOracle)
        {
            if (hasOracle)
                throw new ArgumentException("Oracle feature is not currently supported with NEP-11 templates.");

            return hasOwnable ? GetNep11OwnableTemplate() : GetNep11ContractTemplate();
        }

        private static void WriteContractFiles(TemplateContext context, TemplateInfo templateInfo)
        {
            foreach (var (relativePath, content) in templateInfo.FilesFactory(context))
            {
                WriteFile(context.ProjectDirectory, relativePath, content, context.Replacements);
            }
        }

        private void GenerateFromTemplate(TemplateInfo templateInfo, TemplateContext context, bool includeTests, string? descriptorOverride = null)
        {
            Directory.CreateDirectory(context.ProjectDirectory);

            WriteContractFiles(context, templateInfo);
            WriteGitIgnore(context.ProjectDirectory);
            WriteDotnetToolManifest(context.ProjectDirectory, context.Replacements);

            if (includeTests)
            {
                GenerateTestProject(context);
            }

            string descriptor = descriptorOverride ?? templateInfo.Name;

            Console.WriteLine($"\nSuccessfully created {descriptor} contract '{context.ProjectName}' in {context.ProjectDirectory}");
            Console.WriteLine("\nNext steps:");
            Console.WriteLine($"  cd {context.ProjectName}");
            Console.WriteLine("  dotnet tool restore");
            Console.WriteLine($"  dotnet build {context.ProjectName}.csproj");
            Console.WriteLine($"  dotnet tool run nccs {context.ProjectName}.csproj");

            if (includeTests)
            {
                Console.WriteLine($"\nCompanion tests:");
                Console.WriteLine($"  cd ../{context.TestProjectName}");
                Console.WriteLine("  dotnet test");
            }
        }

        private static void WriteFile(
            string rootDirectory,
            string relativePath,
            string content,
            IReadOnlyDictionary<string, string> replacements)
        {
            string fileName = ReplaceTokens(relativePath, replacements);
            string fullPath = Path.Combine(rootDirectory, fileName);
            string? directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(fullPath, ReplaceTokens(content, replacements), Encoding.UTF8);
            Console.WriteLine($"Created: {fullPath}");
        }

        private static void WriteGitIgnore(string projectDirectory)
        {
            string gitignorePath = Path.Combine(projectDirectory, ".gitignore");
            if (File.Exists(gitignorePath))
            {
                return;
            }

            const string gitignore = @"bin/
obj/
*.nef
*.manifest.json
*.nefdbgnfo
";
            File.WriteAllText(gitignorePath, gitignore, Encoding.UTF8);
            Console.WriteLine($"Created: {gitignorePath}");
        }

        private static void WriteDotnetToolManifest(string projectDirectory, IReadOnlyDictionary<string, string> replacements)
        {
            string configDir = Path.Combine(projectDirectory, ".config");
            Directory.CreateDirectory(configDir);

            string manifestPath = Path.Combine(configDir, "dotnet-tools.json");
            const string manifestTemplate = @"{
  ""version"": 1,
  ""isRoot"": true,
  ""tools"": {
    ""neo.compiler.csharp"": {
      ""version"": ""{{FrameworkPackageVersion}}"",
      ""commands"": [
        ""nccs""
      ]
    }
  }
}
";

            File.WriteAllText(manifestPath, ReplaceTokens(manifestTemplate, replacements), Encoding.UTF8);
            Console.WriteLine($"Created: {manifestPath}");
        }

        private static void GenerateTestProject(TemplateContext context)
        {
            string testDirectory = Path.Combine(context.OutputPath, context.TestProjectName);
            Directory.CreateDirectory(testDirectory);

            WriteFile(testDirectory, "{{TestProjectName}}.csproj", GetTestProjectFileTemplate(), context.Replacements);
            WriteFile(testDirectory, "{{ProjectName}}Tests.cs", GetTestClassTemplate(), context.Replacements);
        }

        private static string ReplaceTokens(string content, IReadOnlyDictionary<string, string> replacements)
        {
            string result = content;
            foreach (var (token, value) in replacements)
            {
                result = result.Replace(token, value);
            }
            return result;
        }

        private static string GenerateSymbol(string projectName)
        {
            string lettersOnly = new(projectName.Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(lettersOnly))
                lettersOnly = "TOKEN";

            string symbol = lettersOnly.ToUpperInvariant();
            return symbol.Length <= 10 ? symbol : symbol[..10];
        }

        private static IReadOnlyCollection<ContractFeature> NormalizeFeatures(IEnumerable<string> featureNames)
        {
            var result = new HashSet<ContractFeature>();

            if (featureNames is null)
            {
                return Array.Empty<ContractFeature>();
            }

            foreach (var entry in featureNames)
            {
                if (string.IsNullOrWhiteSpace(entry))
                {
                    continue;
                }

                bool splitOccurred = false;
                foreach (string token in entry.Split(new[] { ',', ';', '+', '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    splitOccurred = true;
                    AddFeatureToken(token, result);
                }

                if (!splitOccurred)
                {
                    AddFeatureToken(entry, result);
                }
            }

            return result.OrderBy(f => f).ToArray();
        }

        private static void AddFeatureToken(string token, ICollection<ContractFeature> destination)
        {
            string trimmed = token.Trim();
            if (trimmed.Length == 0)
                return;

            if (!FeatureAliases.TryGetValue(trimmed, out var feature))
            {
                throw new ArgumentException($"Unknown feature '{trimmed}'. Supported features: {GetSupportedFeaturesDescription()}.");
            }

            destination.Add(feature);
        }

        private static string GetSupportedFeaturesDescription()
        {
            return string.Join(", ", Enum.GetValues(typeof(ContractFeature)).Cast<ContractFeature>().Select(GetFeatureDisplayName));
        }

        private static string GetFeatureDisplayName(ContractFeature feature) => feature switch
        {
            ContractFeature.Basic => "Basic",
            ContractFeature.NEP17 => "NEP-17",
            ContractFeature.NEP11 => "NEP-11",
            ContractFeature.Ownable => "Ownable",
            ContractFeature.Oracle => "Oracle",
            _ => feature.ToString()
        };

        private static string GetProjectFileTemplate()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""{{FrameworkPackageVersion}}"" />
  </ItemGroup>

  <PropertyGroup>
    <BaseNameArgument Condition=""'$(AssemblyName)' != ''"">--base-name $(AssemblyName)</BaseNameArgument>
    <BaseNameArgument Condition=""'$(AssemblyName)' == ''"">--base-name $(MSBuildProjectName)</BaseNameArgument>
    <NullableArgument Condition=""'$(Nullable)' != ''"">--nullable $(Nullable)</NullableArgument>
    <CheckedArgument Condition=""'$(CheckForOverflowUnderflow)' == 'true'"">--checked</CheckedArgument>
    <DebugArgument Condition=""'$(Configuration)' == 'Debug'"">-d</DebugArgument>
    <DebugArgument Condition=""'$(Configuration)' != 'Debug'""></DebugArgument>
  </PropertyGroup>

  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Message Text=""Start NeoContract converter, Source File: &quot;$(ProjectPath)&quot;"" Importance=""high"" />
    <Exec Condition=""Exists('$(MSBuildProjectDirectory).config/dotnet-tools.json')"" Command=""dotnet tool run nccs $(BaseNameArgument) $(NullableArgument) $(CheckedArgument) $(DebugArgument) &quot;$(ProjectPath)&quot;"" />
  </Target>

</Project>
";
        }

        private static string GetTestProjectFileTemplate()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version=""{{TestSdkVersion}}"" />
    <PackageReference Include=""MSTest.TestAdapter"" Version=""{{MSTestAdapterVersion}}"" />
    <PackageReference Include=""MSTest.TestFramework"" Version=""{{MSTestFrameworkVersion}}"" />
    <PackageReference Include=""coverlet.collector"" Version=""{{CoverletCollectorVersion}}"" />
    <PackageReference Include=""Neo.SmartContract.Testing"" Version=""{{FrameworkPackageVersion}}"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""../{{ProjectName}}/{{ProjectName}}.csproj"" />
  </ItemGroup>

</Project>
";
        }

        private static string GetTestClassTemplate()
        {
            return @"using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace {{Namespace}}.UnitTests;

[TestClass]
public class {{ClassName}}Tests
{
    [TestMethod]
    public void ContractScaffolded()
    {
        Assert.Inconclusive(""Add behavioural tests for global::{{Namespace}}.{{ClassName}}."");
    }
}
";
        }

        #region Contract templates

        private static string GetBasicContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private const string GreetingPrefix = ""Hello, "";
        private const string OwnerKey = ""owner"";

        [Safe]
        public static string Greet(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GreetingPrefix + ""Neo"";
            }
            return GreetingPrefix + name + ""!"";
        }

        public static void SetMessage(string key, string value)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new InvalidOperationException(""No authorization"");
            }

            Storage.Put(Storage.CurrentContext, key, value);
        }

        [Safe]
        public static string GetMessage(string key)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, key);
        }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new InvalidOperationException(""No authorization."");
            }

            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new InvalidOperationException(""No authorization."");
            }

            ContractManagement.Destroy();
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");

            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
            Storage.Put(Storage.CurrentContext, ""DeployedAt"", Runtime.Time);
        }
    }
}
";
        }

        private static string GetNep17ContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep17)]
    public class {{ClassName}} : Neo.SmartContract.Framework.Nep17Token
    {
        private static readonly byte[] OwnerKey = new byte[] { 0x01 };

        public override string Symbol { [Safe] get => ""{{Symbol}}""; }
        public override byte Decimals { [Safe] get => byte.Parse(""{{Decimals}}""); }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);
        }

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        public static new void Mint(UInt160 account, BigInteger amount)
        {
            if (!IsOwner())
            {
                throw new InvalidOperationException(""No authorization."");
            }

            ExecutionEngine.Assert(amount > 0, ""Amount must be positive."");
            Neo.SmartContract.Framework.Nep17Token.Mint(account, amount);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (!IsOwner())
            {
                throw new InvalidOperationException(""No authorization."");
            }

            ExecutionEngine.Assert(amount > 0, ""Amount must be positive."");
            Neo.SmartContract.Framework.Nep17Token.Burn(account, amount);
        }

        [Safe]
        public static bool Verify() => IsOwner();

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
            {
                throw new InvalidOperationException(""No authorization."");
            }

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""Owner must be valid."");
            Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");

            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
            Neo.SmartContract.Framework.Nep17Token.Mint(owner, {{InitialSupply}});
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
            {
                throw new InvalidOperationException(""No authorization."");
            }

            ContractManagement.Update(nefFile, manifest, data);
        }
    }
}
";
        }

        private static string GetNep11ContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [SupportedStandards(NepStandard.Nep11)]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : Nep11Token<TokenState>
    {
        private static readonly byte[] OwnerKey = new byte[] { 0x01 };
        private static readonly byte[] SequenceKey = new byte[] { 0x02 };

        public override string Symbol { [Safe] get => ""{{Symbol}}""; }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);
        }

        public static void Mint(string tokenId, TokenState tokenState)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new InvalidOperationException(""No authorization."");
            }

            Nep11Token<TokenState>.Mint(tokenId, tokenState);
        }

        public static string MintNext(TokenState tokenState)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new InvalidOperationException(""No authorization."");
            }

            StorageMap sequence = new(Storage.CurrentContext, SequenceKey);
            ByteString? current = sequence.Get(Array.Empty<byte>());
            BigInteger next = ((current is null) || current.Length == 0 ? 0 : current.ToBigInteger()) + 1;
            sequence.Put(Array.Empty<byte>(), next);

            string tokenId = next.ToString();
            Nep11Token<TokenState>.Mint(tokenId, tokenState);
            return tokenId;
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");
            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
            Storage.Put(Storage.CurrentContext, SequenceKey, 0);
        }

        protected override byte[] GetKey(string tokenId) => Helper.Concat(new byte[] { 0x10 }, tokenId);
    }

    public class TokenState : Nep11TokenState
    {
        public string? Description;
        public string? Image;
        public Map<string, object>? Properties;
    }
}
";
        }

        private static string GetNep11OwnableTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [SupportedStandards(NepStandard.Nep11)]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : Nep11Token<TokenState>
    {
        private static readonly byte[] OwnerKey = new byte[] { 0x01 };
        private static readonly byte[] SequenceKey = new byte[] { 0x02 };

        public delegate void OnSetOwnerDelegate(UInt160? previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner = default!;

        public override string Symbol { [Safe] get => ""{{Symbol}}""; }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);
        }

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        public static void SetOwner(UInt160 newOwner)
        {
            EnsureOwner();
            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""Owner must be valid."");

            UInt160 previous = GetOwner();
            Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
            OnSetOwner(previous, newOwner);
        }

        public static void Mint(string tokenId, TokenState tokenState)
        {
            EnsureOwner();
            Nep11Token<TokenState>.Mint(tokenId, tokenState);
        }

        public static string MintNext(TokenState tokenState)
        {
            EnsureOwner();

            StorageMap sequence = new(Storage.CurrentContext, SequenceKey);
            ByteString? current = sequence.Get(Array.Empty<byte>());
            BigInteger next = ((current is null) || current.Length == 0 ? BigInteger.Zero : current.ToBigInteger()) + 1;
            sequence.Put(Array.Empty<byte>(), next);

            string tokenId = next.ToString();
            Nep11Token<TokenState>.Mint(tokenId, tokenState);
            return tokenId;
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");
            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
            Storage.Put(Storage.CurrentContext, SequenceKey, 0);
            OnSetOwner(null, owner);
        }

        protected override byte[] GetKey(string tokenId) =>
            Helper.Concat(new byte[] { 0x10 }, tokenId);

        private static void EnsureOwner()
        {
            if (!IsOwner())
            {
                throw new InvalidOperationException(""No authorization."");
            }
        }
    }

    public class TokenState : Nep11TokenState
    {
        public string? Description;
        public string? Image;
        public Map<string, object>? Properties;
    }
}
";
        }

        private static string GetOwnableContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private static readonly byte[] OwnerKey = new byte[] { 0x01 };
        private static readonly byte[] AllowListPrefix = new byte[] { 0x02 };

        public delegate void OnSetOwnerDelegate(UInt160? previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner = default!;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);
        }

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        public static void SetOwner(UInt160 newOwner)
        {
            EnsureOwner();
            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""Owner must be valid."");

            UInt160 previous = GetOwner();
            Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
            OnSetOwner(previous, newOwner);
        }

        public static void AllowOperator(UInt160 account, bool allow)
        {
            EnsureOwner();
            ExecutionEngine.Assert(account.IsValid && !account.IsZero, ""Account must be valid."");

            StorageMap allowList = new(Storage.CurrentContext, AllowListPrefix);
            if (allow)
            {
                allowList.Put(account, 1);
            }
            else
            {
                allowList.Delete(account);
            }
        }

        [Safe]
        public static bool IsOperator(UInt160 account)
        {
            StorageMap allowList = new(Storage.CurrentReadOnlyContext, AllowListPrefix);
            return allowList.Get(account) != null || IsOwner();
        }

        public static void ControlledAction(string action)
        {
            if (!IsOperator(Runtime.CallingScriptHash))
            {
                throw new InvalidOperationException(""Caller is not authorized."");
            }

            Storage.Put(Storage.CurrentContext, action, Runtime.Time);
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            EnsureOwner();
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            EnsureOwner();
            ContractManagement.Destroy();
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");
            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
            OnSetOwner(null, owner);
        }

        private static void EnsureOwner()
        {
            if (!IsOwner())
            {
                throw new InvalidOperationException(""No authorization."");
            }
        }
    }
}
";
        }

        private static string GetOracleContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract, IOracle
    {
        private static readonly byte[] ResponsePrefix = new byte[] { 0x01 };
        private static readonly byte[] NoncePrefix = new byte[] { 0x02 };

        public delegate void OracleResponseDelegate(ByteString requestId, string url, OracleResponseCode code, string payload);

        [DisplayName(""OracleResponse"")]
        public static event OracleResponseDelegate OnOracleResponse = default!;

        public static ByteString RequestData(string url, string filter, object userData, long gasForResponse)
        {
            ExecutionEngine.Assert(!string.IsNullOrEmpty(url), ""URL required"");
            ExecutionEngine.Assert(gasForResponse > 0, ""Gas budget must be positive."");

            ByteString nonce = GetNextNonce();
            Oracle.Request(url, filter, nameof(OnOracleResponseCallback), nonce, gasForResponse);
            Storage.Put(Storage.CurrentContext, Helper.Concat(NoncePrefix, nonce), StdLib.Serialize(userData));
            return nonce;
        }

        public void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode responseCode, string result)
        {
            OnOracleResponseCallback(requestedUrl, (ByteString)userData, responseCode, result);
        }

        private static void OnOracleResponseCallback(string requestedUrl, ByteString requestId, OracleResponseCode responseCode, string result)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
            {
                throw new InvalidOperationException(""Unauthorized oracle callback."");
            }

            ByteString payloadKey = Helper.Concat(ResponsePrefix, requestId);
            Storage.Put(Storage.CurrentContext, payloadKey, result);
            OnOracleResponse(requestId, requestedUrl, responseCode, result);

            Storage.Delete(Storage.CurrentContext, Helper.Concat(NoncePrefix, requestId));
            Runtime.Log($""Oracle response saved for {requestedUrl}"");
        }

        [Safe]
        public static string GetResponse(ByteString requestId)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, Helper.Concat(ResponsePrefix, requestId));
        }

        private static ByteString GetNextNonce()
        {
            ByteString? current = Storage.Get(Storage.CurrentContext, NoncePrefix);
            BigInteger value = current is null ? 0 : current.ToBigInteger();
            value++;
            Storage.Put(Storage.CurrentContext, NoncePrefix, value);
            return value.ToByteArray();
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""DeployedAt"", Runtime.Time);
            }
        }
    }
}
";
        }

        private static string GetOwnableOracleTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract, IOracle
    {
        private static readonly byte[] OwnerKey = new byte[] { 0x01 };
        private static readonly byte[] ResponsePrefix = new byte[] { 0x02 };
        private static readonly byte[] RequestPrefix = new byte[] { 0x03 };

        public delegate void OracleResponseDelegate(ByteString requestId, string url, OracleResponseCode code, string payload);

        [DisplayName(""OracleResponse"")]
        public static event OracleResponseDelegate OracleResponseProcessed = default!;

        [Safe]
        public static UInt160 GetOwner() => (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);

        public static void SetOwner(UInt160 newOwner)
        {
            EnsureOwner();
            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""Owner must be valid."");
            Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
        }

        public static ByteString RequestOracleData(string url, string filter, object userData, long gasForResponse)
        {
            EnsureOwner();
            ExecutionEngine.Assert(!string.IsNullOrEmpty(url), ""URL required."");
            ExecutionEngine.Assert(gasForResponse > 0, ""Gas must be positive."");

            ByteString requestId = NextRequestId();
            Oracle.Request(url, filter, nameof(OnOracleResponseInternal), requestId, gasForResponse);
            Storage.Put(Storage.CurrentContext, Helper.Concat(RequestPrefix, requestId), StdLib.Serialize(userData));
            Runtime.Log($""Oracle request created for {url}"");
            return requestId;
        }

        public void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode responseCode, string result)
        {
            OnOracleResponseInternal(requestedUrl, (ByteString)userData, responseCode, result);
        }

        private static void OnOracleResponseInternal(string requestedUrl, ByteString requestId, OracleResponseCode responseCode, string payload)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
            {
                throw new InvalidOperationException(""Unauthorized oracle callback."");
            }

            Storage.Delete(Storage.CurrentContext, Helper.Concat(RequestPrefix, requestId));

            if (responseCode == OracleResponseCode.Success)
            {
                Storage.Put(Storage.CurrentContext, Helper.Concat(ResponsePrefix, requestId), payload);
            }

            OracleResponseProcessed(requestId, requestedUrl, responseCode, payload);
        }

        [Safe]
        public static string GetStoredResponse(ByteString requestId)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, Helper.Concat(ResponsePrefix, requestId));
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");
            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
        }

        private static void EnsureOwner()
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new InvalidOperationException(""No authorization."");
            }
        }

        private static ByteString NextRequestId()
        {
            ByteString? current = Storage.Get(Storage.CurrentContext, RequestPrefix);
            BigInteger value = (current is null || current.Length == 0) ? BigInteger.Zero : current.ToBigInteger();
            value++;
            Storage.Put(Storage.CurrentContext, RequestPrefix, value);
            return value.ToByteArray();
        }
    }
}
";
        }

        private static string GetNep17OwnableOracleTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [SupportedStandards(NepStandard.Nep17)]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : Nep17Token, IOracle
    {
        private static readonly byte[] OwnerKey = new byte[] { 0x01 };
        private static readonly byte[] ResponsePrefix = new byte[] { 0x02 };
        private static readonly byte[] NoncePrefix = new byte[] { 0x03 };

        public delegate void OracleDataDelegate(ByteString requestId, string url, string payload);

        [DisplayName(""OracleData"")]
        public static event OracleDataDelegate OracleDataReceived = default!;

        public override string Symbol { [Safe] get => ""{{Symbol}}""; }
        public override byte Decimals { [Safe] get => byte.Parse(""{{Decimals}}""); }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentReadOnlyContext, OwnerKey);
        }

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""Owner must be valid."");
            Storage.Put(Storage.CurrentContext, OwnerKey, newOwner);
        }

        public static new void Mint(UInt160 account, BigInteger amount)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");

            ExecutionEngine.Assert(amount > 0, ""Amount must be positive."");
            Nep17Token.Mint(account, amount);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");

            ExecutionEngine.Assert(amount > 0, ""Amount must be positive."");
            Nep17Token.Burn(account, amount);
        }

        public static ByteString RequestOracleQuote(string url, string filter, object userData)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");

            ByteString nonce = NextNonce();
            Oracle.Request(url, filter, nameof(OnOracleResponseHandler), nonce, long.Parse(""{{OracleGasBudget}}""));
            Storage.Put(Storage.CurrentContext, Helper.Concat(NoncePrefix, nonce), StdLib.Serialize(userData));
            Runtime.Log($""Oracle request placed for {url}"");
            return nonce;
        }

        public void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode responseCode, string result)
        {
            OnOracleResponseHandler(requestedUrl, (ByteString)userData, responseCode, result);
        }

        private static void OnOracleResponseHandler(string requestedUrl, ByteString requestId, OracleResponseCode code, string result)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
            {
                throw new InvalidOperationException(""Unauthorized oracle callback."");
            }

            if (code != OracleResponseCode.Success)
            {
                Runtime.Log($""Oracle responded with code {(byte)code}"");
                Storage.Delete(Storage.CurrentContext, Helper.Concat(NoncePrefix, requestId));
                return;
            }

            Storage.Put(Storage.CurrentContext, Helper.Concat(ResponsePrefix, requestId), result);
            OracleDataReceived(requestId, requestedUrl, result);
            Storage.Delete(Storage.CurrentContext, Helper.Concat(NoncePrefix, requestId));
        }

        [Safe]
        public static string GetOracleResult(ByteString requestId)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, Helper.Concat(ResponsePrefix, requestId));
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
                return;

            UInt160 owner = Runtime.Transaction.Sender;
            if (data != null)
            {
                owner = (UInt160)data;
            }

            ExecutionEngine.Assert(owner.IsValid && !owner.IsZero, ""Owner required"");

            Storage.Put(Storage.CurrentContext, OwnerKey, owner);
            Nep17Token.Mint(owner, {{InitialSupply}});
        }

        private static ByteString NextNonce()
        {
            ByteString? current = Storage.Get(Storage.CurrentContext, NoncePrefix);
            BigInteger nonce = (current is null || current.Length == 0) ? BigInteger.Zero : current.ToBigInteger();
            nonce++;
            Storage.Put(Storage.CurrentContext, NoncePrefix, nonce);
            return nonce.ToByteArray();
        }
    }
}
";
        }

        #endregion

        private sealed record TemplateInfo(
            string Name,
            string Description,
            Func<TemplateContext, IReadOnlyDictionary<string, string>> FilesFactory);

        private sealed class TemplateContext
        {
            public TemplateContext(string projectName, string outputPath, Dictionary<string, string>? additional)
            {
                ProjectName = projectName;
                OutputPath = outputPath;
                ProjectDirectory = Path.Combine(outputPath, projectName);
                Namespace = projectName;
                TestProjectName = projectName + ".UnitTests";
                Replacements = BuildReplacements(additional);
            }

            public string ProjectName { get; }
            public string Namespace { get; }
            public string OutputPath { get; }
            public string ProjectDirectory { get; }
            public string TestProjectName { get; }
            public IReadOnlyDictionary<string, string> Replacements { get; }

            private Dictionary<string, string> BuildReplacements(Dictionary<string, string>? additional)
            {
                var replacements = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["{{ProjectName}}"] = ProjectName,
                    ["{{Namespace}}"] = Namespace,
                    ["{{ClassName}}"] = ProjectName,
                    ["{{Author}}"] = "Author",
                    ["{{Email}}"] = "email@example.com",
                    ["{{Description}}"] = $"{ProjectName} Smart Contract",
                    ["{{Version}}"] = "1.0.0",
                    ["{{Year}}"] = DateTime.UtcNow.Year.ToString(),
                    ["{{Symbol}}"] = GenerateSymbol(ProjectName),
                    ["{{Decimals}}"] = "8",
                    ["{{InitialSupply}}"] = "1000000_00000000",
                    ["{{OracleGasBudget}}"] = "100000000",
                    ["{{FrameworkPackageVersion}}"] = DefaultFrameworkPackageVersion,
                    ["{{TestSdkVersion}}"] = DefaultTestSdkVersion,
                    ["{{MSTestAdapterVersion}}"] = DefaultMSTestAdapterVersion,
                    ["{{MSTestFrameworkVersion}}"] = DefaultMSTestFrameworkVersion,
                    ["{{CoverletCollectorVersion}}"] = DefaultCoverletCollectorVersion,
                    ["{{TestProjectName}}"] = TestProjectName
                };

                if (additional != null)
                {
                    foreach (var (key, value) in additional)
                    {
                        replacements[key] = value;
                    }
                }

                return replacements;
            }
        }
    }
}
