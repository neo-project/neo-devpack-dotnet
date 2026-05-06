using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DefaultInterfaceImplementation
{
    [TestMethod]
    public void Contract_Exports_And_Uses_Interface_Default_Implementation()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public class Contract : SmartContract, IDefaultGreeting
{
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var manifest = context.CreateManifest();
        Assert.IsNotNull(manifest.Abi.GetMethod("greet", 0));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultGreetingContract>(context.CreateExecutable(), manifest);

        Assert.AreEqual("hello", contract.Greet());
    }

    [TestMethod]
    public void Contract_Override_Wins_Over_Interface_Default_Implementation()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public class Contract : SmartContract, IDefaultGreeting
{
    [DisplayName(""greet"")]
    public string Greet()
    {
        return ""override"";
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var manifest = context.CreateManifest();
        var greetMethods = manifest.Abi.Methods.Where(m => m.Name == "greet").ToArray();
        Assert.AreEqual(1, greetMethods.Length);

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultGreetingContract>(context.CreateExecutable(), manifest);

        Assert.AreEqual("override", contract.Greet());
    }

    [TestMethod]
    public void Contract_Uses_ExpressionBodied_Interface_Default_Implementation()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet() => ""expr"";
}

public class Contract : SmartContract, IDefaultGreeting
{
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultGreetingContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("expr", contract.Greet());
    }

    [TestMethod]
    public void Contract_Uses_Default_Interface_Property_Getter()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultValue
{
    [DisplayName(""count"")]
    int Count
    {
        get
        {
            return 42;
        }
    }
}

public class Contract : SmartContract, IDefaultValue
{
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var manifest = context.CreateManifest();
        Assert.IsNotNull(manifest.Abi.GetMethod("count", 0));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultValueContract>(context.CreateExecutable(), manifest);

        Assert.AreEqual(42, contract.Count);
    }

    [TestMethod]
    public void GetAllMembers_Includes_Default_Property_Setter_And_Skips_Abstract_Interface_Members()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultValue
{
    static int Shared() => 7;

    [DisplayName(""count"")]
    int Count
    {
        get => 42;
        set { _ = value; }
    }

    [DisplayName(""required"")]
    string Required { get; }
}

public class Contract : SmartContract, IDefaultValue
{
}";

        var contractType = CompileContractType(source);
        var members = contractType.GetAllMembers().OfType<IMethodSymbol>().ToArray();

        Assert.AreEqual(1, members.Count(m => m.MethodKind == MethodKind.PropertyGet && m.AssociatedSymbol?.Name == "Count"));
        Assert.AreEqual(1, members.Count(m => m.MethodKind == MethodKind.PropertySet && m.AssociatedSymbol?.Name == "Count"));
        Assert.IsFalse(members.Any(m => m.AssociatedSymbol?.Name == "Required"));
    }

    [TestMethod]
    public void GetAllMembers_Adds_Default_Interface_Method_When_Base_Type_Does_Not_Implement_It()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public abstract class ContractBase : SmartContract
{
}

public class Contract : ContractBase, IDefaultGreeting
{
}";

        var contractType = CompileContractType(source);
        var greetMethods = contractType.GetAllMembers().OfType<IMethodSymbol>().Where(m => m.Name == "Greet").ToArray();

        Assert.AreEqual(1, greetMethods.Length);
        Assert.IsTrue(SymbolEqualityComparer.Default.Equals(greetMethods[0], contractType.AllInterfaces.Single().GetMembers("Greet").OfType<IMethodSymbol>().Single()));
    }

    [TestMethod]
    public void GetAllMembers_Does_Not_Readd_Default_Interface_Method_When_Base_Type_Already_Provides_It()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public abstract class ContractBase : SmartContract, IDefaultGreeting
{
}

public class Contract : ContractBase
{
}";

        var contractType = CompileContractType(source);
        var greetMethods = contractType.GetAllMembers().OfType<IMethodSymbol>().Where(m => m.Name == "Greet").ToArray();

        Assert.AreEqual(1, greetMethods.Length);
        Assert.AreEqual(0, contractType.GetMembers("Greet").Length);
    }

    [TestMethod]
    public void GetAllMembers_Adds_Default_Interface_Method_For_Structs_Without_Base_Types()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface IDefaultGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""hello"";
    }
}

public struct Counter : IDefaultGreeting
{
}

public class DummyContract : SmartContract
{
}";

        var counterType = CompileContractType(source, "Counter");
        var greetMethods = counterType.GetAllMembers().OfType<IMethodSymbol>().Where(m => m.Name == "Greet").ToArray();

        Assert.AreEqual(1, greetMethods.Length);
        Assert.IsTrue(SymbolEqualityComparer.Default.Equals(greetMethods[0], counterType.AllInterfaces.Single().GetMembers("Greet").OfType<IMethodSymbol>().Single()));
    }

    [TestMethod]
    public void Contract_Class_Method_Satisfies_Two_Interface_Default_Methods_With_Same_Signature()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface ILeftGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""left"";
    }
}

public interface IRightGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""right"";
    }
}

public class Contract : SmartContract, ILeftGreeting, IRightGreeting
{
    [DisplayName(""greet"")]
    public string Greet()
    {
        return ""class"";
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var manifest = context.CreateManifest();
        var greetMethods = manifest.Abi.Methods.Where(m => m.Name == "greet").ToArray();
        Assert.AreEqual(1, greetMethods.Length);

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DefaultGreetingContract>(context.CreateExecutable(), manifest);

        Assert.AreEqual("class", contract.Greet());
    }

    [TestMethod]
    public void Contract_Fails_When_Two_Unrelated_Interfaces_Provide_Competing_Default_Methods()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public interface ILeftGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""left"";
    }
}

public interface IRightGreeting
{
    [DisplayName(""greet"")]
    string Greet()
    {
        return ""right"";
    }
}

public class Contract : SmartContract, ILeftGreeting, IRightGreeting
{
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsFalse(context.Success, "Competing unrelated default interface methods should require an explicit class implementation.");
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == "NC3003"),
            string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    private static INamedTypeSymbol CompileContractType(string sourceCode, string contractName = "Contract")
    {
        var (engine, _) = CompileSource(sourceCode);
        var contractType = engine.Compilation!.GetTypeByMetadataName(contractName);
        Assert.IsNotNull(contractType, $"Unable to find type '{contractName}' in compilation.");
        return contractType!;
    }

    private static (CompilationEngine Engine, CompilationContext[] Contexts) CompileSource(string sourceCode)
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, sourceCode);

        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };

            var engine = new CompilationEngine(options);
            var repoRoot = Syntax.SyntaxProbeLoader.GetRepositoryRoot();
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var contexts = engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile);

            return (engine, contexts.ToArray());
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    public abstract class DefaultGreetingContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("greet")]
        public abstract string? Greet();
    }

    public abstract class DefaultValueContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public abstract int? Count { [DisplayName("count")] get; }
    }
}
