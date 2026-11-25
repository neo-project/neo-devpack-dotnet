using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_InitialValueAttributeIdentity
{
    private const string ExternalAttributes = """
        namespace External
        {
            public class InitialValueAttribute : System.Attribute
            {
                public InitialValueAttribute(string value) { }
                public InitialValueAttribute(int first, int second) { }
            }

            public class DerivedInitialValueAttribute(string value) : InitialValueAttribute(value)
            {
            }
        }
        """;

    // Same full name as the official attribute, but declared in the contract assembly.
    private const string ShadowedOfficialAttribute = """
        namespace Neo.SmartContract.Framework.Attributes
        {
            public class InitialValueAttribute(string value) : System.Attribute
            {
            }
        }
        """;

    private const string UnsupportedAttributes = """
        namespace Custom
        {
            using Neo.SmartContract.Framework.Attributes;

            public class IntegerAttribute : InitialValueAttribute
            {
                public IntegerAttribute(string value) : base("99", ContractParameterType.Integer) { }
                public IntegerAttribute(int value) : base("99", ContractParameterType.Integer) { }
            }

            public class OtherAttribute() : InitialValueAttribute("99", ContractParameterType.Integer);
        }
        """;

    private static CompilationContext Compile(string members, string getBody, CompilationOptions.OptimizationType optimization, string extraDeclarations = "")
    {
        var source = $$"""
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            {{ExternalAttributes}}

            {{extraDeclarations}}

            public class Contract : SmartContract
            {
                {{members}}

                [DisplayName("get")]
                public static int Get() => {{getBody}};
            }
            """;

        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        return TestHelper.CompileSingleContract(source, options);
    }

    private static InitialValueContract Deploy(string members, string getBody, CompilationOptions.OptimizationType optimization, string extraDeclarations = "")
        => Deploy(Compile(members, getBody, optimization, extraDeclarations), new TestEngine(true));

    private static InitialValueContract Deploy(CompilationContext context, TestEngine engine)
        => engine.Deploy<InitialValueContract>(context.CreateExecutable(), context.CreateManifest());

    private static InitialValueContract DeployStatic(string field, CompilationOptions.OptimizationType optimization)
        => Deploy(field, "Value", optimization);

    private static InitialValueContract DeployInstance(string field, CompilationOptions.OptimizationType optimization)
        => Deploy($$"""
            public class Position
            {
                {{field}}
                public int Deposit;
            }
            """, "new Position { Deposit = 1 }.Value", optimization);

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ExternalInitialValueAttribute_IsIgnored(CompilationOptions.OptimizationType optimization)
    {
        var contract = DeployStatic("""[External.InitialValue("42")] private static readonly int Value = 7;""", optimization);
        Assert.AreEqual(new BigInteger(7), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ExternalInitialValueAttributeWithDifferentConstructor_IsIgnored(CompilationOptions.OptimizationType optimization)
    {
        var contract = DeployStatic("""[External.InitialValue(1, 2)] private static readonly int Value = 9;""", optimization);
        Assert.AreEqual(new BigInteger(9), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void AttributeDerivedFromExternalInitialValue_IsIgnored(CompilationOptions.OptimizationType optimization)
    {
        var contract = DeployStatic("""[External.DerivedInitialValue("42")] private static readonly int Value = 8;""", optimization);
        Assert.AreEqual(new BigInteger(8), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, "Integer(\"42\")", "IntegerAttribute")]
    [DataRow(CompilationOptions.OptimizationType.Basic, "Integer(\"42\")", "IntegerAttribute")]
    [DataRow(CompilationOptions.OptimizationType.All, "Integer(\"42\")", "IntegerAttribute")]
    [DataRow(CompilationOptions.OptimizationType.None, "Integer(42)", "IntegerAttribute")]
    [DataRow(CompilationOptions.OptimizationType.Basic, "Integer(42)", "IntegerAttribute")]
    [DataRow(CompilationOptions.OptimizationType.All, "Integer(42)", "IntegerAttribute")]
    [DataRow(CompilationOptions.OptimizationType.None, "Other", "OtherAttribute")]
    [DataRow(CompilationOptions.OptimizationType.Basic, "Other", "OtherAttribute")]
    [DataRow(CompilationOptions.OptimizationType.All, "Other", "OtherAttribute")]
    public void UnsupportedDerivedAttribute_IsRejected(CompilationOptions.OptimizationType optimization, string attribute, string attributeName)
    {
        AssertUnsupported(Compile($"[Custom.{attribute}] private static int Value = 7;", "Value", optimization, UnsupportedAttributes), attributeName);
        AssertUnsupported(Compile($"public class Position {{ [Custom.{attribute}] public int Value = 7; public int Deposit; }}",
            "new Position { Deposit = 1 }.Value", optimization, UnsupportedAttributes), attributeName);
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void UnsupportedDerivedAttribute_OnOverwrittenInstanceField_IsRejected(CompilationOptions.OptimizationType optimization)
    {
        AssertUnsupported(Compile("""public class Position { [Custom.Integer("42")] public int Value; }""",
            "new Position { Value = 7 }.Value", optimization, UnsupportedAttributes), "IntegerAttribute");
    }

    private static void AssertUnsupported(CompilationContext context, string attributeName)
    {
        if (context.Success)
            Assert.Fail($"Expected NC3001, but compilation succeeded and get returned {Deploy(context, new TestEngine(true)).Get()}.");

        var errors = context.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.HasCount(1, errors, string.Join("\n", errors.Select(d => d.ToString())));
        Assert.AreEqual("NC3001", errors[0].Id);
        Assert.AreEqual($"Unsupported initial value type: {attributeName}", errors[0].GetMessage());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void OfficialFullNameDeclaredInContract_IsIgnored(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy("""[Neo.SmartContract.Framework.Attributes.InitialValue("42")] private static readonly int Value = 6;""", "Value", optimization, ShadowedOfficialAttribute);
        Assert.AreEqual(new BigInteger(6), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void OfficialInitialValueAttribute_IsApplied(CompilationOptions.OptimizationType optimization)
    {
        var contract = DeployStatic("""[Neo.SmartContract.Framework.Attributes.InitialValue("42", Neo.SmartContract.Framework.ContractParameterType.Integer)] private static int Value;""", optimization);
        Assert.AreEqual(new BigInteger(42), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void OfficialDerivedAttribute_IsApplied(CompilationOptions.OptimizationType optimization)
    {
        var engine = new TestEngine(true);
        var context = Compile("""[Neo.SmartContract.Framework.Attributes.Integer("43")] private static int Value;""", "Value", optimization);
        var contract = Deploy(context, engine);
        Assert.AreEqual(new BigInteger(43), contract.Get());
        Assert.AreEqual(270_638L, engine.FeeConsumed.Value);
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void FieldWithoutAttributes_KeepsOrdinaryInitialization(CompilationOptions.OptimizationType optimization)
    {
        var contract = DeployStatic("private static readonly int Value = 5;", optimization);
        Assert.AreEqual(new BigInteger(5), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void InstanceField_ExternalInitialValueAttribute_IsIgnored(CompilationOptions.OptimizationType optimization)
    {
        var contract = DeployInstance("""[External.InitialValue("500")] public int Value = 3;""", optimization);
        Assert.AreEqual(new BigInteger(3), contract.Get());
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void InstanceField_OfficialInitialValueAttribute_IsApplied(CompilationOptions.OptimizationType optimization)
    {
        var engine = new TestEngine(true);
        var context = Compile("""
            public class Position
            {
                [Neo.SmartContract.Framework.Attributes.InitialValue("500", ContractParameterType.Integer)] public int Value;
                public int Deposit;
            }
            """, "new Position { Deposit = 1 }.Value", optimization);
        var contract = Deploy(context, engine);
        Assert.AreEqual(new BigInteger(500), contract.Get());
        Assert.AreEqual(271_099L, engine.FeeConsumed.Value);
    }

    [TestMethod]
    public void InitialValueFromAlternateFrameworkRequiresExplicitReference()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Attributes;

            public class Contract : SmartContract
            {
                [InitialValue("42", ContractParameterType.Integer)]
                private static int Value;
                public static int Get() => Value;
            }
            """;

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(path => !string.Equals(Path.GetFileNameWithoutExtension(path),
                CompilationEngine.FrameworkAssemblyName, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();
        var frameworkCompilation = CSharpCompilation.Create(
            CompilationEngine.FrameworkAssemblyName,
            [CSharpSyntaxTree.ParseText("""
                namespace Neo.SmartContract.Framework
                {
                    public abstract class SmartContract
                    {
                        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.InternalCall)]
                        public extern static void _initialize();
                    }
                    public enum ContractParameterType : byte { Integer = 0x11 }
                }

                namespace Neo.SmartContract.Framework.Attributes
                {
                    public sealed class InitialValueAttribute(string value, ContractParameterType type) : System.Attribute;
                }
                """)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using var image = new MemoryStream();
        var emit = frameworkCompilation.Emit(image);
        Assert.IsTrue(emit.Success, string.Join(Environment.NewLine, emit.Diagnostics));
        var framework = MetadataReference.CreateFromImage(image.ToArray());

        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, source);
            MetadataReference[] allReferences = [.. references, framework];
            var engine = new CompilationEngine(TestHelper.CreateDefaultOptions());

            var unresolved = engine.Compile([path], allReferences).Single();
            Assert.IsFalse(unresolved.Success);
            Assert.IsTrue(unresolved.Diagnostics.Any(diagnostic =>
                diagnostic.Id == DiagnosticId.InvalidInitialValueType &&
                diagnostic.GetMessage().Contains("trusted framework reference", StringComparison.Ordinal)));

            var resolved = engine.Compile([path], allReferences, framework).Single();
            Assert.IsTrue(resolved.Success, string.Join(Environment.NewLine, resolved.Diagnostics));
            var contract = Deploy(resolved, new TestEngine(true));
            Assert.AreEqual(new BigInteger(42), contract.Get());
        }
        finally
        {
            File.Delete(path);
        }
    }

    public abstract class InitialValueContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("get")]
        public abstract BigInteger? Get();
    }
}
