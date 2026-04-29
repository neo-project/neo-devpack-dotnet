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
public class UnitTest_RecursivePatternType
{
    [TestMethod]
    public void RecursivePattern_WithTypeClause_DoesNotMatchDifferentTypeWithSamePropertyShape()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static bool Test()
    {
        object value = new byte[] { 0x2A };
        return value is string { Length: 1 };
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<RecursivePatternTypeContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsFalse(contract.Test()!.Value);
    }

    public abstract class RecursivePatternTypeContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract bool? Test();
    }
}
