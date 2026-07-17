using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_EnumTryParseContinuationAudit
{
    [TestMethod]
    public void EnumTryParseIgnoreCaseContinuesAndAssignsResult()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                private enum TestEnum
                {
                    Value1 = 1,
                    Value2 = 2
                }

                [DisplayName("test")]
                public static object[] Test(string value, bool ignoreCase)
                {
                    bool success = Enum.TryParse(typeof(TestEnum), value, ignoreCase, out object result);
                    return new object[] { success, result };
                }
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<EnumTryParseContract>(context.CreateExecutable(), context.CreateManifest());
        var result = contract.Test("value2", true);

        Assert.IsNotNull(result);
        Assert.IsTrue(((StackItem)result[0]!).GetBoolean());
        Assert.AreEqual(2, ((StackItem)result[1]!).GetInteger());

        result = contract.Test("missing", true);

        Assert.IsNotNull(result);
        Assert.IsFalse(((StackItem)result[0]!).GetBoolean());
        Assert.AreEqual(0, ((StackItem)result[1]!).GetInteger());
    }

    public abstract class EnumTryParseContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract object?[]? Test(string value, bool ignoreCase);
    }
}
