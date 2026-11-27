// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CatchDiagnostics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CatchDiagnostics
{
    [TestMethod]
    public void SpecificCatchTypeIsRejected()
    {
        var context = TestHelper.CompileSingleContract("""
            using System;
            using Neo.SmartContract.Framework;
            public class Contract : SmartContract
            {
                public static int Test()
                {
                    try { throw new ArgumentException("wrong"); }
                    catch (InvalidOperationException) { return 1; }
                }
            }
            """);

        Assert.IsFalse(context.Success);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.TypedCatch),
            string.Join(Environment.NewLine, context.Diagnostics));
    }

    [TestMethod]
    [DataRow("e.Source")]
    [DataRow("e?.Source")]
    public void UnsupportedCatchVariableMemberIsRejected(string memberExpression)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using System;
            using Neo.SmartContract.Framework;
            public class Contract : SmartContract
            {
                public static string Test()
                {
                    try { throw new Exception("message"); }
                    catch (Exception e) { return {{memberExpression}}; }
                }
            }
            """);

        Assert.IsFalse(context.Success);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.ExternMethod),
            string.Join(Environment.NewLine, context.Diagnostics));
    }

    [TestMethod]
    public void CatchVariableMessageAndToStringReturnThrownMessage()
    {
        var context = TestHelper.CompileSingleContract("""
            using System;
            using Neo.SmartContract.Framework;
            public class Contract : SmartContract
            {
                public static string Test()
                {
                    try { throw new Exception("message"); }
                    catch (Exception e) { return e.Message + ":" + e.ToString() + ":" + e?.Message + ":" + e?.ToString(); }
                }
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<CatchMessageContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual("message:message:message:message", contract.Test());
    }

    [TestMethod]
    public void CatchAllAndDirectThrownValueRemainSupported()
    {
        var context = TestHelper.CompileSingleContract("""
            using System;
            using Neo.SmartContract.Framework;
            public class Contract : SmartContract
            {
                public static string Test()
                {
                    try { throw new Exception("message"); }
                    catch (Exception e) { return "caught:" + e; }
                }
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
    }

    [TestMethod]
    public void UInt160ParseCompilesWithFramework()
    {
        var context = TestHelper.CompileSingleContract("""
            using Neo.SmartContract.Framework;
            public class Contract : SmartContract
            {
                public static UInt160 Parse(string value) => UInt160.Parse(value);
            }
            """);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
    }

    public abstract class CatchMessageContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("test")]
        public abstract string? Test();
    }
}
