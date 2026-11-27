using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_UnloweredExternMembers
{
    [TestMethod]
    [DataRow("new System.Guid()")]
    [DataRow("new System.DateTime()")]
    [DataRow("new System.TimeSpan(5)")]
    [DataRow("new System.Nullable<int>(5)")]
    [DataRow("new System.Collections.Generic.List<int>()")]
    [DataRow("new string(new[] { 'a', 'b' })")]
    public void ExternConstructorWithoutLowering_ReportsDiagnostic(string expression)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static object Test()
                {
                    object value = {{expression}};
                    return value;
                }
            }
            """);

        AssertExternMemberDiagnostic(context, "Unknown constructor");
    }

    [TestMethod]
    public void TargetTypedExternConstructorWithoutLowering_ReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static object Test()
                {
                    System.Guid value = new();
                    return "done";
                }
            }
            """);

        AssertExternMemberDiagnostic(context, "Unknown constructor");
    }

    [TestMethod]
    [DataRow("System.Guid.Empty")]
    [DataRow("System.DateTime.MinValue")]
    [DataRow("System.TimeSpan.Zero")]
    [DataRow("System.IntPtr.Zero")]
    public void ExternStaticFieldWithoutLowering_ReportsDiagnostic(string expression)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static object Test()
                {
                    object value = {{expression}};
                    return value;
                }
            }
            """);

        AssertExternMemberDiagnostic(context, "Unknown field");
    }

    [TestMethod]
    public void ExternConstructorWithoutLowering_ReportsDiagnostic_WhenIgnored()
    {
        // The value is discarded, but the stack still has to stay balanced for the following return.
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                public static object Test()
                {
                    var value = new System.Guid();
                    return "done";
                }
            }
            """);

        AssertExternMemberDiagnostic(context, "Unknown constructor");
    }

    [TestMethod]
    public void SupportedConstructions_AndContractStaticFields_StillWork()
    {
        var contract = DeployContract();

        Assert.AreEqual(1, contract.NewObject());
        Assert.AreEqual(0, contract.NewInt());
        Assert.AreEqual(1, contract.NewList());
        Assert.AreEqual("", contract.StringEmpty());
        Assert.AreEqual("", contract.StringEmptyQualified());
        Assert.AreEqual("aaa", contract.StringRepeat());
        Assert.AreEqual(8, contract.ContractStaticField());
        Assert.AreEqual(5, contract.ContractAutoProperty());
    }

    private static void AssertExternMemberDiagnostic(CompilationContext context, string messagePrefix)
    {
        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.ExternMethod), diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.GetMessage(null).StartsWith(messagePrefix, StringComparison.Ordinal)), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    private static SupportedMembersContract DeployContract()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Helper
            {
                public static int Counter = 7;
                public static int AutoProp { get; set; } = 3;
            }

            public class Contract : SmartContract
            {
                [DisplayName("newObject")]
                public static int NewObject() => new object() is null ? 0 : 1;

                [DisplayName("newInt")]
                public static int NewInt() => new int();

                [DisplayName("newList")]
                public static int NewList()
                {
                    var list = new List<int>();
                    list.Add(1);
                    return list.Count;
                }

                [DisplayName("stringEmpty")]
                public static string StringEmpty() => string.Empty;

                [DisplayName("stringEmptyQualified")]
                public static string StringEmptyQualified() => System.String.Empty;

                [DisplayName("stringRepeat")]
                public static string StringRepeat() => new string('a', 3);

                [DisplayName("contractStaticField")]
                public static int ContractStaticField() => Helper.Counter + 1;

                [DisplayName("contractAutoProperty")]
                public static int ContractAutoProperty()
                {
                    Helper.AutoProp += 2;
                    return Helper.AutoProp;
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<SupportedMembersContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class SupportedMembersContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("newObject")]
        public abstract int NewObject();

        [DisplayName("newInt")]
        public abstract int NewInt();

        [DisplayName("newList")]
        public abstract int NewList();

        [DisplayName("stringEmpty")]
        public abstract string? StringEmpty();

        [DisplayName("stringEmptyQualified")]
        public abstract string? StringEmptyQualified();

        [DisplayName("stringRepeat")]
        public abstract string? StringRepeat();

        [DisplayName("contractStaticField")]
        public abstract int ContractStaticField();

        [DisplayName("contractAutoProperty")]
        public abstract int ContractAutoProperty();
    }
}
