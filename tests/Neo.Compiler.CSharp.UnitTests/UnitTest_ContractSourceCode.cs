using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ContractSourceCode
{
    [TestMethod]
    public void ContractSourceCodePopulatesNefSource()
    {
        var context = TestHelper.CompileSingleContract("""
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
[ContractSourceCode("https://example.invalid/source")]
public class Contract : SmartContract { }
""");
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        Assert.AreEqual("https://example.invalid/source", context.CreateExecutable().Source);
    }
}
