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

    [TestMethod]
    public void StringEndsWith_ConstantSuffix()
    {
        var contract = DeployContract();

        Assert.IsTrue(contract.Utf8Suffix());
        Assert.IsFalse(contract.Utf8SuffixMismatch());
        Assert.IsTrue(contract.SingleByteSuffix());
        Assert.IsFalse(contract.SingleByteSuffixMismatch());
        Assert.IsFalse(contract.SuffixLongerThanSource());
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

    [DisplayName(""utf8Suffix"")]
    public static bool Utf8Suffix() => ""Hello 😊😭"".EndsWith(""😭"");

    [DisplayName(""utf8SuffixMismatch"")]
    public static bool Utf8SuffixMismatch() => ""Hello 😊😭"".EndsWith(""😊"");

    [DisplayName(""singleByteSuffix"")]
    public static bool SingleByteSuffix() => ""hello"".EndsWith(""o"");

    [DisplayName(""singleByteSuffixMismatch"")]
    public static bool SingleByteSuffixMismatch() => ""hello"".EndsWith(""O"");

    [DisplayName(""suffixLongerThanSource"")]
    public static bool SuffixLongerThanSource() => ""hi"".EndsWith(""hello"");
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

        [DisplayName("utf8Suffix")]
        public abstract bool? Utf8Suffix();

        [DisplayName("utf8SuffixMismatch")]
        public abstract bool? Utf8SuffixMismatch();

        [DisplayName("singleByteSuffix")]
        public abstract bool? SingleByteSuffix();

        [DisplayName("singleByteSuffixMismatch")]
        public abstract bool? SingleByteSuffixMismatch();

        [DisplayName("suffixLongerThanSource")]
        public abstract bool? SuffixLongerThanSource();
    }
}
