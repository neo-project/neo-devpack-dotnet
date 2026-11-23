// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT software license.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CrossActivationCapture
{
    [TestMethod]
    [DataRow("public static System.Func<int> Factory(int n) => () => n;")]
    [DataRow("public static int Recursive(int n) { System.Func<int> f = () => n; return n == 0 ? 0 : Recursive(n - 1) + f(); }")]
    [DataRow("public static int RecursiveRead(int n) { System.Func<int> f = () => n; return n == 0 ? 0 : RecursiveRead(n - 1) + n; }")]
    [DataRow("public static System.Func<int> Factory(int n) { System.Func<int> f = () => n; return f; }")]
    [DataRow("public static Neo.SmartContract.Framework.List<System.Func<int>> Factory(int n) { var list = new Neo.SmartContract.Framework.List<System.Func<int>>(); list.Add(() => n); return list; }")]
    public void CrossActivationCapture_ReportsError(string method)
    {
        var source = "using Neo.SmartContract.Framework; public class Contract : SmartContract { " + method + " }";
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsFalse(context.Success);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == "NC2014"),
            string.Join("\n", context.Diagnostics.Select(d => d.ToString())));
    }

    [TestMethod]
    public void LocalCapture_RemainsSupported()
    {
        const string source = "using Neo.SmartContract.Framework; public class Contract : SmartContract { public static int Local(int n) { System.Func<int> f = () => n; return f(); } }";
        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join("\n", context.Diagnostics.Select(d => d.ToString())));
    }
}
