// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SafeAttributeIdentity.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_SafeAttributeIdentity
{
    [TestMethod]
    public void OnlyFrameworkSafeAttributeMarksAbiMembersSafe()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System;

            namespace User
            {
                [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
                public sealed class SafeAttribute : Attribute
                {
                }
            }

            public class Contract : SmartContract
            {
                [User.Safe]
                public static int CustomMethod() => 1;

                [User.Safe]
                public static int CustomProperty => 2;

                [User.Safe]
                public static int CustomWritableProperty { get; set; }

                [Neo.SmartContract.Framework.Attributes.Safe]
                public static int FrameworkMethod() => 3;

                [Neo.SmartContract.Framework.Attributes.Safe]
                public static int FrameworkProperty => 4;
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var methods = context.CreateManifest().Abi.Methods;
        Assert.IsFalse(methods.Single(method => method.Name == "customMethod").Safe);
        Assert.IsFalse(methods.Single(method => method.Name == "customProperty").Safe);
        var customWritableAccessors = methods.Where(method => method.Name == "customWritableProperty").ToArray();
        Assert.AreEqual(1, customWritableAccessors.Length);
        Assert.IsTrue(customWritableAccessors.All(method => !method.Safe));
        Assert.IsTrue(methods.Single(method => method.Name == "frameworkMethod").Safe);
        Assert.IsTrue(methods.Single(method => method.Name == "frameworkProperty").Safe);
    }

    [TestMethod]
    public void SourceDefinedFrameworkSafeAttributeDoesNotMarkAbiMembersSafe()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System;

            namespace Neo.SmartContract.Framework.Attributes
            {
                [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
                public sealed class SafeAttribute : Attribute
                {
                }
            }

            public class Contract : SmartContract
            {
                [Neo.SmartContract.Framework.Attributes.Safe]
                public static int ShadowedMethod() => 1;

                [Neo.SmartContract.Framework.Attributes.Safe]
                public static int ShadowedProperty => 2;
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var methods = context.CreateManifest().Abi.Methods;
        Assert.IsFalse(methods.Single(method => method.Name == "shadowedMethod").Safe);
        Assert.IsFalse(methods.Single(method => method.Name == "shadowedProperty").Safe);
    }

    [TestMethod]
    public void MetadataAssemblyWithFrameworkNameDoesNotMarkAbiMembersSafe()
    {
        var sourceFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(sourceFile, """
            extern alias spoof;

            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                [spoof::Neo.SmartContract.Framework.Attributes.Safe]
                public static int SpoofedMethod() => 1;

                [Neo.SmartContract.Framework.Attributes.Safe]
                public static int FrameworkMethod() => 2;
            }
            """);

        try
        {
            MetadataReference spoofFramework = CreateSpoofFrameworkReference("spoof");
            MetadataReference trustedFramework = MetadataReference.CreateFromFile(
                typeof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute).Assembly.Location);
            MetadataReference[] references =
            [
                spoofFramework,
                .. CreatePlatformReferences(),
                trustedFramework
            ];

            var engine = new CompilationEngine(TestHelper.CreateDefaultOptions());
            var context = engine.Compile([sourceFile], references).Single();

            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            var methods = context.CreateManifest().Abi.Methods;
            var spoofedMethod = context.TargetContract.GetMembers("SpoofedMethod").OfType<IMethodSymbol>().Single();
            var frameworkMethod = context.TargetContract.GetMembers("FrameworkMethod").OfType<IMethodSymbol>().Single();
            Assert.IsFalse(methods.Single(method => method.Name == "spoofedMethod").Safe,
                $"Base: {context.TargetContract.BaseType?.ContainingAssembly.Identity}; " +
                $"spoof: {spoofedMethod.GetAttributes().Single().AttributeClass?.ContainingAssembly.Identity}; " +
                $"framework: {frameworkMethod.GetAttributes().Single().AttributeClass?.ContainingAssembly.Identity}");
            Assert.IsTrue(methods.Single(method => method.Name == "frameworkMethod").Safe);
        }
        finally
        {
            File.Delete(sourceFile);
        }
    }

    [TestMethod]
    public void SpoofFrameworkContractBaseDoesNotEstablishSafeAttributeIdentity()
    {
        var sourceFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(sourceFile, """
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Attributes;

            public class Contract : SmartContract
            {
                [Safe]
                public static int SpoofedMethod() => 1;
            }
            """);

        try
        {
            MetadataReference[] references =
            [
                CreateSpoofFrameworkReference(),
                .. CreatePlatformReferences()
            ];

            var engine = new CompilationEngine(TestHelper.CreateDefaultOptions());
            var context = engine.Compile([sourceFile], references).Single();

            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            Assert.IsFalse(context.CreateManifest().Abi.Methods.Single(method => method.Name == "spoofedMethod").Safe);
        }
        finally
        {
            File.Delete(sourceFile);
        }
    }

    [TestMethod]
    public void FrameworkReferenceIdentityRejectsUntrustedMetadata()
    {
        MetadataReference compilationReference = CSharpCompilation.Create(
            assemblyName: "Unrelated",
            references: CreatePlatformReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();
        using var moduleStream = new MemoryStream();
        var moduleResult = CSharpCompilation.Create(
            assemblyName: "Unrelated.netmodule",
            references: CreatePlatformReferences(),
            options: new CSharpCompilationOptions(OutputKind.NetModule))
            .Emit(moduleStream);
        Assert.IsTrue(moduleResult.Success, string.Join(Environment.NewLine, moduleResult.Diagnostics));
        MetadataReference moduleReference = MetadataReference.CreateFromImage(
            moduleStream.ToArray(), MetadataReferenceProperties.Module);
        MetadataReference malformedReference = MetadataReference.CreateFromImage([0]);
        MetadataReference trustedReference = MetadataReference.CreateFromFile(
            typeof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute).Assembly.Location);

        Assert.IsFalse(CompilationEngine.IsTrustedFrameworkReference(compilationReference));
        Assert.IsFalse(CompilationEngine.IsTrustedFrameworkReference(moduleReference));
        Assert.IsFalse(CompilationEngine.IsTrustedFrameworkReference(malformedReference));
        Assert.IsTrue(CompilationEngine.IsTrustedFrameworkReference(trustedReference));
    }

    private static MetadataReference CreateSpoofFrameworkReference(params string[] aliases)
    {
        byte[]? publicKey = typeof(object).Assembly.GetName().GetPublicKey();
        Assert.IsNotNull(publicKey);

        CSharpCompilation spoofCompilation = CSharpCompilation.Create(
            assemblyName: "Neo.SmartContract.Framework",
            syntaxTrees:
            [
                CSharpSyntaxTree.ParseText("""
                    using System;
                    using System.Reflection;
                    using System.Runtime.CompilerServices;

                    [assembly: AssemblyVersion("99.0.0.0")]

                    namespace Neo.SmartContract.Framework
                    {
                        public abstract class SmartContract
                        {
                            [MethodImpl(MethodImplOptions.InternalCall)]
                            public extern static void _initialize();
                        }
                    }

                    namespace Neo.SmartContract.Framework.Attributes
                    {
                        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
                        public sealed class SafeAttribute : Attribute
                        {
                        }
                    }
                    """)
            ],
            references: CreatePlatformReferences(),
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                cryptoPublicKey: ImmutableArray.Create(publicKey),
                delaySign: true));

        using var stream = new MemoryStream();
        var result = spoofCompilation.Emit(stream);
        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        MetadataReferenceProperties properties = aliases.Length == 0
            ? MetadataReferenceProperties.Assembly
            : MetadataReferenceProperties.Assembly.WithAliases(aliases);
        return MetadataReference.CreateFromImage(stream.ToArray(), properties);
    }

    private static MetadataReference[] CreatePlatformReferences()
    {
        var trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        Assert.IsFalse(string.IsNullOrWhiteSpace(trustedPlatformAssemblies));
        return trustedPlatformAssemblies!
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(File.Exists)
            .Where(path => !string.Equals(
                Path.GetFileNameWithoutExtension(path),
                "Neo.SmartContract.Framework",
                StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();
    }
}
