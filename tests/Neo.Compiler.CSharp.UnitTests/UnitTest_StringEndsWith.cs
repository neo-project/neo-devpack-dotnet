using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringEndsWith
{
    [TestMethod]
    public void StringEndsWith_WholeStringSuffix_ReturnsTrue()
    {
        var contract = DeployContract();

        Assert.IsTrue(contract.WholeString());
        Assert.IsFalse(contract.Mismatch());
    }

    [TestMethod]
    public void StringEndsWith_EmptySuffix_ReturnsTrue()
    {
        var contract = DeployContract();

        Assert.IsTrue(contract.EmptySuffix());
    }

    private static StringEndsWithBoundaryContract DeployContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""wholeString"")]
    public static bool WholeString() => ""Hello"".EndsWith(""Hello"");

    [DisplayName(""emptySuffix"")]
    public static bool EmptySuffix() => ""Hello"".EndsWith("""");

    [DisplayName(""mismatch"")]
    public static bool Mismatch() => ""Hello"".EndsWith(""Hell"");
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<StringEndsWithBoundaryContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class StringEndsWithBoundaryContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("wholeString")]
        public abstract bool? WholeString();

        [DisplayName("emptySuffix")]
        public abstract bool? EmptySuffix();

        [DisplayName("mismatch")]
        public abstract bool? Mismatch();
    }
}
