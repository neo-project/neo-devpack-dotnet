extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ContractPermissionAttribute
{
    private const string Hash1 = "0x0000000000000000000000000000000000000001";
    private const string Hash2 = "0x0000000000000000000000000000000000000002";

    private const string ExternalAttribute = @"
namespace External
{
    public class ContractPermissionAttribute : System.Attribute
    {
        public ContractPermissionAttribute(string contract, params string[] methods) { }
    }

}
";

    private static ContractPermission[] CompilePermissions(string source)
    {
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        return context.CreateManifest().Permissions;
    }

    private static ContractPermission[] CompileInheritedPermissions(string baseAttribute, string derivedAttribute = "") =>
        CompilePermissions($$"""
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Attributes;

            {{ExternalAttribute}}

            {{baseAttribute}}
            public abstract class BaseContract : SmartContract { }

            {{derivedAttribute}}
            public class Contract : BaseContract
            {
                public static int Get() => 7;
            }
            """);

    private static void AssertHashPermission(ContractPermission permission, string hash, params string[] methods)
    {
        Assert.IsTrue(permission.Contract.IsHash);
        Assert.AreEqual(UInt160.Parse(hash), permission.Contract.Hash);
        Assert.IsFalse(permission.Methods.IsWildcard);
        CollectionAssert.AreEquivalent(methods, permission.Methods.ToArray());
    }

    [TestMethod]
    public void ExternalAttribute_IsIgnored()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
" + ExternalAttribute + @"
[External.ContractPermission(""" + Hash1 + @""", ""unexpected"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(0, permissions.Length);
    }

    [TestMethod]
    public void OfficialAttribute_IsRegistered()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission(""" + Hash1 + @""", ""expected"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(1, permissions.Length);
        AssertHashPermission(permissions[0], Hash1, "expected");
    }

    [TestMethod]
    public void OfficialAttribute_OnAbstractBase_IsInherited()
    {
        var permissions = CompileInheritedPermissions($"""[ContractPermission("{Hash1}", "inherited")]""");

        Assert.AreEqual(1, permissions.Length);
        AssertHashPermission(permissions[0], Hash1, "inherited");
    }

    [TestMethod]
    public void ExternalAttribute_OnAbstractBase_IsIgnored()
    {
        var permissions = CompileInheritedPermissions($"""[External.ContractPermission("{Hash1}", "unexpected")]""");

        Assert.AreEqual(0, permissions.Length);
    }

    [TestMethod]
    public void OfficialAttributes_OnBaseAndDerived_AreBothRegistered()
    {
        var permissions = CompileInheritedPermissions(
            $"""[ContractPermission("{Hash1}", "inherited")]""",
            $"""[ContractPermission("{Hash2}", "declared")]""");

        Assert.AreEqual(2, permissions.Length);
        AssertHashPermission(permissions.Single(p => p.Contract.Hash == UInt160.Parse(Hash1)), Hash1, "inherited");
        AssertHashPermission(permissions.Single(p => p.Contract.Hash == UInt160.Parse(Hash2)), Hash2, "declared");
    }

    [TestMethod]
    public void OfficialMetadataNameDeclaredInSource_IsIgnored()
    {
        var permissions = CompilePermissions($$"""
            using Neo.SmartContract.Framework;

            namespace Neo.SmartContract.Framework.Attributes
            {
                public sealed class ContractPermissionAttribute(string contract, params string[] methods) : System.Attribute { }
            }

            [Neo.SmartContract.Framework.Attributes.ContractPermissionAttribute("{{Hash1}}", "unexpected")]
            public class Contract : SmartContract
            {
                public static int Get() => 7;
            }
            """);

        Assert.AreEqual(0, permissions.Length);
    }

    [TestMethod]
    public void OfficialAndExternalAttributes_OnlyOfficialIsRegistered()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
" + ExternalAttribute + @"
[External.ContractPermission(""" + Hash2 + @""", ""unexpected"")]
[ContractPermission(""" + Hash1 + @""", ""expected"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(1, permissions.Length);
        AssertHashPermission(permissions[0], Hash1, "expected");
    }

    [TestMethod]
    public void ExternalAttribute_WithDifferentConstructor_DoesNotThrow()
    {
        var permissions = CompilePermissions("""
            using Neo.SmartContract.Framework;

            namespace External
            {
                public class ContractPermissionAttribute(int value) : System.Attribute { }
            }

            [External.ContractPermission(1)]
            public class Contract : SmartContract
            {
                public static int Get() => 7;
            }
            """);

        Assert.AreEqual(0, permissions.Length);
    }

    [TestMethod]
    public void OfficialAttribute_SpecificContractAndMethods()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission(""" + Hash1 + @""", ""a"", ""b"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(1, permissions.Length);
        AssertHashPermission(permissions[0], Hash1, "a", "b");
    }

    [TestMethod]
    public void OfficialAttribute_Wildcards()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission(""*"", ""*"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(1, permissions.Length);
        Assert.IsTrue(permissions[0].Contract.IsWildcard);
        Assert.IsTrue(permissions[0].Methods.IsWildcard);
    }

    [TestMethod]
    public void OfficialAttribute_WildcardMethodsForSpecificContract()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission(""" + Hash1 + @""", ""*"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(1, permissions.Length);
        Assert.IsTrue(permissions[0].Contract.IsHash);
        Assert.AreEqual(UInt160.Parse(Hash1), permissions[0].Contract.Hash);
        Assert.IsTrue(permissions[0].Methods.IsWildcard);
    }

    [TestMethod]
    public void OfficialAttribute_WildcardContractForSpecificMethod()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission(""*"", ""transfer"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(1, permissions.Length);
        Assert.IsTrue(permissions[0].Contract.IsWildcard);
        CollectionAssert.AreEqual(new[] { "transfer" }, permissions[0].Methods.ToArray());
    }

    [TestMethod]
    public void MultipleOfficialAttributes_AreAllRegistered()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

[ContractPermission(""" + Hash1 + @""", ""a"")]
[ContractPermission(""" + Hash2 + @""", ""b"")]
public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(2, permissions.Length);
        AssertHashPermission(permissions.Single(p => p.Contract.Hash == UInt160.Parse(Hash1)), Hash1, "a");
        AssertHashPermission(permissions.Single(p => p.Contract.Hash == UInt160.Parse(Hash2)), Hash2, "b");
    }

    [TestMethod]
    public void NoAttributes_ProducesNoPermissions()
    {
        var permissions = CompilePermissions(@"using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    public static int Get() => 7;
}");

        Assert.AreEqual(0, permissions.Length);
    }

    [TestMethod]
    public void SameMetadataNameInAnotherAssembly_IsIgnored()
    {
        const string source = $$"""
            extern alias external;
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Attributes;

            [external::Neo.SmartContract.Framework.Attributes.ContractPermission("{{Hash2}}", "unexpected")]
            [ContractPermission("{{Hash1}}", "expected")]
            public class Contract : SmartContract
            {
                public static int Get() => 7;
            }
            """;

        var platformReferences = CreatePlatformReferences();
        var external = CreateFrameworkFixture("ExternalPermissions", platformReferences, "external");
        var framework = MetadataReference.CreateFromFile(
            typeof(scfx::Neo.SmartContract.Framework.Attributes.ContractPermissionAttribute).Assembly.Location);
        var context = CompileWithReferences(source, [.. platformReferences, external, framework]);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var permissions = context.CreateManifest().Permissions;
        Assert.AreEqual(1, permissions.Length);
        AssertHashPermission(permissions[0], Hash1, "expected");
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void AlternateFramework_RequiresExplicitReference(bool useExplicitReference)
    {
        const string source = $$"""
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Attributes;

            [ContractPermission("{{Hash1}}", "expected")]
            public class Contract : SmartContract
            {
                public static int Get() => 7;
            }
            """;

        var platformReferences = CreatePlatformReferences();
        var framework = CreateFrameworkFixture("Neo.SmartContract.Framework", platformReferences);
        var context = CompileWithReferences(source, [.. platformReferences, framework], useExplicitReference ? framework : null);

        if (useExplicitReference)
        {
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            var permissions = context.CreateManifest().Permissions;
            Assert.AreEqual(1, permissions.Length);
            AssertHashPermission(permissions[0], Hash1, "expected");
        }
        else
        {
            Assert.IsFalse(context.Success);
            Assert.IsTrue(context.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error &&
                d.GetMessage().Contains("trusted framework reference", StringComparison.Ordinal)),
                string.Join(Environment.NewLine, context.Diagnostics));
        }
    }

    private static CompilationContext CompileWithReferences(string source, MetadataReference[] references, MetadataReference? framework = null)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(path, source);
        try
        {
            var engine = new CompilationEngine(TestHelper.CreateDefaultOptions());
            return framework is null
                ? engine.Compile([path], references).Single()
                : engine.Compile([path], references, framework).Single();
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static MetadataReference CreateFrameworkFixture(string assemblyName, MetadataReference[] references, params string[] aliases)
    {
        var compilation = CSharpCompilation.Create(assemblyName,
            [CSharpSyntaxTree.ParseText("""
                namespace Neo.SmartContract.Framework
                {
                    public abstract class SmartContract
                    {
                        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
                        public extern static void _initialize();
                    }
                }

                namespace Neo.SmartContract.Framework.Attributes
                {
                    public sealed class ContractPermissionAttribute(string contract, params string[] methods) : System.Attribute { }
                }
                """)], references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics));
        return MetadataReference.CreateFromImage(stream.ToArray(), MetadataReferenceProperties.Assembly.WithAliases(aliases));
    }

    private static MetadataReference[] CreatePlatformReferences()
    {
        var paths = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        return paths.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(path => Path.GetFileNameWithoutExtension(path) != "Neo.SmartContract.Framework")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();
    }
}
