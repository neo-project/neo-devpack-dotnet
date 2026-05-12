using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringConstructor
{
    [TestMethod]
    public void StringCharCountConstructor_ReturnsExpectedString()
    {
        var contract = DeployContract();

        Assert.AreEqual("", contract.Empty());
        Assert.AreEqual("\0", contract.NullChar());
        Assert.AreEqual("xxx", contract.Repeat());
        Assert.AreEqual("", contract.DynamicCount(0));
        Assert.AreEqual("bb", contract.DynamicCount(2));
        Assert.AreEqual("\0\0", contract.DynamicNullCount(2));
        Assert.AreEqual("AA", contract.DynamicChar(65));
        Assert.ThrowsException<TestException>(() => contract.DynamicCount(-1));
    }

    [TestMethod]
    public void StringCharCountConstructor_OverMaxItemSizeReportsDiagnostic()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("oversized")]
                public static string Oversized() => new string('x', int.MaxValue);
            }
            """);

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.InvalidArgument), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    private static StringConstructorContract DeployContract()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("empty")]
                public static string Empty() => new string('a', 0);

                [DisplayName("nullChar")]
                public static string NullChar() => new string((char)0, 1);

                [DisplayName("repeat")]
                public static string Repeat() => new string('x', 3);

                [DisplayName("dynamicCount")]
                public static string DynamicCount(int count) => new string('b', count);

                [DisplayName("dynamicNullCount")]
                public static string DynamicNullCount(int count) => new string((char)0, count);

                [DisplayName("dynamicChar")]
                public static string DynamicChar(int value) => new string((char)value, 2);
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<StringConstructorContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class StringConstructorContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("empty")]
        public abstract string? Empty();

        [DisplayName("nullChar")]
        public abstract string? NullChar();

        [DisplayName("repeat")]
        public abstract string? Repeat();

        [DisplayName("dynamicCount")]
        public abstract string? DynamicCount(int count);

        [DisplayName("dynamicNullCount")]
        public abstract string? DynamicNullCount(int count);

        [DisplayName("dynamicChar")]
        public abstract string? DynamicChar(int value);
    }
}
