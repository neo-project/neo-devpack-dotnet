using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringCaseReceiver
{
    [TestMethod]
    public void StringCaseMethodsUseTheReceiverExpression()
    {
        var contract = DeployContract();

        Assert.AreEqual("ABC!", contract.UpperFromLocal("AbC"));
        Assert.AreEqual("abc!", contract.LowerFromExpression("AbC"));
    }

    private static StringCaseReceiverContract DeployContract()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("upperFromLocal")]
                public static string UpperFromLocal(string value)
                {
                    string local = value + "!";
                    return local.ToUpper();
                }

                [DisplayName("lowerFromExpression")]
                public static string LowerFromExpression(string value)
                {
                    return (value + "!").ToLower();
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<StringCaseReceiverContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class StringCaseReceiverContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("upperFromLocal")]
        public abstract string? UpperFromLocal(string value);

        [DisplayName("lowerFromExpression")]
        public abstract string? LowerFromExpression(string value);
    }
}
