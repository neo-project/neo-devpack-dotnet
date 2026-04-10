// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_RiscVTarget.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.Backend.RiscV;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
[TestCategory("RiscV")]
public class UnitTest_RiscVTarget
{
    [TestMethod]
    public void TestRiscVEmitter_GeneratesRustSource()
    {
        var emitter = new RiscVEmitter();
        emitter.BeginMethod("balanceOf", 1, 0);
        emitter.InitSlot(0, 1);
        emitter.Syscall(0x4a100170); // System.Storage.GetContext
        emitter.LdArg(0);
        emitter.Convert(2); // ByteString
        emitter.Syscall(0x31e85d92); // System.Storage.Get
        emitter.Convert(0); // Integer
        emitter.Ret();
        emitter.EndMethod();

        var rustSource = emitter.Builder.Build("TestContract");

        Assert.IsTrue(rustSource.Contains("fn method_balanceof(ctx: &mut Context)"));
        Assert.IsTrue(rustSource.Contains("ctx.init_slot(0, 1);"));
        Assert.IsTrue(rustSource.Contains("ctx.syscall(0x4a100170);"));
        Assert.IsTrue(rustSource.Contains("ctx.load_arg(0);"));
        Assert.IsTrue(rustSource.Contains("ctx.convert(0x02);"));
        Assert.IsTrue(rustSource.Contains("ctx.ret();"));
        Assert.IsTrue(rustSource.Contains("\"balanceOf\" => method_balanceof(ctx)"));
    }

    [TestMethod]
    public void TestRiscVEmitter_MultipleMethodDispatch()
    {
        var emitter = new RiscVEmitter();

        emitter.BeginMethod("transfer", 3, 0);
        emitter.InitSlot(0, 3);
        emitter.Ret();
        emitter.EndMethod();

        emitter.BeginMethod("balanceOf", 1, 0);
        emitter.InitSlot(0, 1);
        emitter.Ret();
        emitter.EndMethod();

        var rustSource = emitter.Builder.Build("TokenContract");

        Assert.IsTrue(rustSource.Contains("\"transfer\" => method_transfer(ctx)"));
        Assert.IsTrue(rustSource.Contains("\"balanceOf\" => method_balanceof(ctx)"));
        Assert.IsTrue(rustSource.Contains("_ => ctx.fault(\"Unknown method\")"));
    }
}
