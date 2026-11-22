using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.ComponentModel;
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

    private static InitialValueContract Deploy(string members, string getBody, CompilationOptions.OptimizationType optimization, string extraDeclarations = "")
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
        var context = TestHelper.CompileSingleContract(source, options);
        return new TestEngine(true).Deploy<InitialValueContract>(context.CreateExecutable(), context.CreateManifest());
    }

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
        var contract = DeployStatic("""[Neo.SmartContract.Framework.Attributes.Integer("43")] private static int Value;""", optimization);
        Assert.AreEqual(new BigInteger(43), contract.Get());
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
        var contract = DeployInstance("""[Neo.SmartContract.Framework.Attributes.InitialValue("500", Neo.SmartContract.Framework.ContractParameterType.Integer)] public int Value;""", optimization);
        Assert.AreEqual(new BigInteger(500), contract.Get());
    }

    public abstract class InitialValueContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("get")]
        public abstract BigInteger? Get();
    }
}
