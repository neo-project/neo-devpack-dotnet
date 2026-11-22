// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LambdaInstanceCapture.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_LambdaInstanceCapture
{
    [TestMethod]
    [DataRow("() => this.Value")]
    [DataRow("() => Value")]
    [DataRow("other => other.Value * 1000 + this.Value")]
    [DataRow("() => base.GetValue()")]
    public void CapturedInstanceReportsDiagnostic(string lambda)
    {
        var source = $$"""
using System;
using Neo.SmartContract.Framework;

public class BaseBox
{
    protected int GetValue() => 1;
}

public class Box : BaseBox
{
    public int Value;
    public int Read(Box other)
    {
        Func<Box, int> f = {{lambda}};
        return f(other);
    }
}

public class Contract : SmartContract
{
    public static int Test() => new Box().Read(new Box());
}
""";

        // Parameterless cases need the matching delegate type.
        if (!lambda.StartsWith("other", StringComparison.Ordinal))
            source = source.Replace("Func<Box, int> f", "Func<int> f").Replace("f(other)", "f()");

        var context = TestHelper.CompileSingleContract(source);
        var diagnostics = string.Join(Environment.NewLine, context.Diagnostics);
        Assert.IsFalse(context.Success, diagnostics);
        Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.SyntaxNotSupported), diagnostics);
        Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
    }

    [TestMethod]
    public void StaticMemberLambdaStillCompiles()
    {
        var context = TestHelper.CompileSingleContract("""
using System;
using Neo.SmartContract.Framework;

public class Contract : SmartContract
{
    private static int Value = 9;
    public static int Test()
    {
        Func<int> read = () => Value;
        return read();
    }
}
""");

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
    }
}
