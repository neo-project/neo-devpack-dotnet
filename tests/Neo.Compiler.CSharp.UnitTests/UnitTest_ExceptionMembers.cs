using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ExceptionMembers
{
    [TestMethod]
    public void CaughtException_MessageAndToString_AreTheMessage()
    {
        var contract = DeployContract();

        Assert.AreEqual("boom", contract.CaughtMessage());
        Assert.AreEqual("boom", contract.CaughtToString());
        Assert.AreEqual("boom", contract.CaughtConditionalMessage());
        Assert.AreEqual("boom", contract.CaughtConditionalToString());
        Assert.AreEqual(4, contract.CaughtMessageLength());
        Assert.IsTrue(contract.CaughtMessageContains());
        Assert.AreEqual("boom!", contract.CaughtToStringConcat());
        Assert.AreEqual("arg", contract.CaughtSubclassMessage());
        Assert.AreEqual("stored", contract.StoredExceptionMessage());
        Assert.AreEqual("stored", contract.StoredExceptionToString());
        Assert.AreEqual("boom", contract.StoredExceptionThrownAndCaught());
    }

    [TestMethod]
    public void VmFault_MessageIsTheFaultText()
    {
        var contract = DeployContract();

        string message = contract.CaughtVmFaultMessage();
        Assert.IsFalse(string.IsNullOrEmpty(message));
    }

    [TestMethod]
    [DataRow("e.Source")]
    [DataRow("e.StackTrace")]
    [DataRow("e.InnerException")]
    [DataRow("e.HResult")]
    [DataRow("e.Data")]
    [DataRow("e.GetType()")]
    [DataRow("e.GetBaseException()")]
    [DataRow("e?.Source")]
    public void UnsupportedExceptionMember_ReportsDiagnostic(string expression)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;
            using System;

            public class Contract : SmartContract
            {
                public static string Test()
                {
                    try
                    {
                        throw new Exception("boom");
                    }
                    catch (Exception e)
                    {
                        object value = {{expression}};
                        return "unreachable";
                    }
                }
            }
            """);

        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString()));
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.ExternMethod), diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.GetMessage(null).StartsWith("Unknown method", StringComparison.Ordinal)), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void ContractExceptionOverride_IsNotReplaced()
    {
        // A ToString() declared by the contract itself must keep its own lowering.
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            using System;

            public class MyError : Exception
            {
                public override string ToString() => "mine";
            }

            public class Contract : SmartContract
            {
                public static string Test(MyError error)
                {
                    return error.ToString();
                }
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
    }

    private static ExceptionMembersContract DeployContract()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("caughtMessage")]
                public static string CaughtMessage()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e.Message; }
                }

                [DisplayName("caughtToString")]
                public static string CaughtToString()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e.ToString(); }
                }

                [DisplayName("caughtConditionalMessage")]
                public static string CaughtConditionalMessage()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e?.Message; }
                }

                [DisplayName("caughtConditionalToString")]
                public static string CaughtConditionalToString()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e?.ToString(); }
                }

                [DisplayName("caughtMessageLength")]
                public static int CaughtMessageLength()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e.Message.Length; }
                }

                [DisplayName("caughtMessageContains")]
                public static bool CaughtMessageContains()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e.Message.Contains("oo"); }
                }

                [DisplayName("caughtToStringConcat")]
                public static string CaughtToStringConcat()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { return e.ToString() + "!"; }
                }

                [DisplayName("caughtSubclassMessage")]
                public static string CaughtSubclassMessage()
                {
                    try { throw new ArgumentException("arg"); }
                    catch (Exception e) { return e.Message; }
                }

                [DisplayName("storedExceptionMessage")]
                public static string StoredExceptionMessage()
                {
                    Exception exception = new Exception("stored");
                    return exception.Message;
                }

                [DisplayName("storedExceptionToString")]
                public static string StoredExceptionToString()
                {
                    Exception exception = new Exception("stored");
                    return exception.ToString();
                }

                [DisplayName("storedExceptionThrownAndCaught")]
                public static string StoredExceptionThrownAndCaught()
                {
                    try { throw new Exception("boom"); }
                    catch (Exception e) { string message = e.Message; return message; }
                }

                [DisplayName("caughtVmFaultMessage")]
                public static string CaughtVmFaultMessage()
                {
                    try
                    {
                        int[] values = new int[1];
                        return "no fault: " + values[5];
                    }
                    catch (Exception e) { return e.Message; }
                }
            }
            """;

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        return engine.Deploy<ExceptionMembersContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class ExceptionMembersContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("caughtMessage")]
        public abstract string? CaughtMessage();

        [DisplayName("caughtToString")]
        public abstract string? CaughtToString();

        [DisplayName("caughtConditionalMessage")]
        public abstract string? CaughtConditionalMessage();

        [DisplayName("caughtConditionalToString")]
        public abstract string? CaughtConditionalToString();

        [DisplayName("caughtMessageLength")]
        public abstract int CaughtMessageLength();

        [DisplayName("caughtMessageContains")]
        public abstract bool CaughtMessageContains();

        [DisplayName("caughtToStringConcat")]
        public abstract string? CaughtToStringConcat();

        [DisplayName("caughtSubclassMessage")]
        public abstract string? CaughtSubclassMessage();

        [DisplayName("storedExceptionMessage")]
        public abstract string? StoredExceptionMessage();

        [DisplayName("storedExceptionToString")]
        public abstract string? StoredExceptionToString();

        [DisplayName("storedExceptionThrownAndCaught")]
        public abstract string? StoredExceptionThrownAndCaught();

        [DisplayName("caughtVmFaultMessage")]
        public abstract string? CaughtVmFaultMessage();
    }
}
