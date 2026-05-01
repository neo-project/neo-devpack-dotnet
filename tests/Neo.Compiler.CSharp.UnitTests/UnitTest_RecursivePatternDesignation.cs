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
public class UnitTest_RecursivePatternDesignation
{
    [TestMethod]
    public void RecursivePattern_Designation_AssignsCapturedValue()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""test"")]
    public static bool Test()
    {
        object value = ""A"";
        return value is string { Length: 1 } matched && matched == ""A"";
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<RecursivePatternDesignationContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsTrue(contract.Test()!.Value);
    }

    public abstract class RecursivePatternDesignationContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract bool? Test();
    }
}
