// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_EscapedPointerOptimization.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_EscapedPointerOptimization
{
    [TestMethod]
    public void EscapedFunctionPointerRemainsExecutable()
    {
        var context = TestHelper.CompileSingleContract("""
            using System;
            using Neo.SmartContract.Framework;

            public class Contract : SmartContract
            {
                private static int GetFive() => 5;

                public static object GetPointer() => new Func<int>(GetFive);
            }
            """);
        Assert.IsTrue(context.Success, string.Join(System.Environment.NewLine, context.Diagnostics));

        var (nef, manifest, _) = context.CreateResults();
        var getPointer = manifest.Abi.GetMethod("getPointer", 0);
        Assert.IsNotNull(getPointer);

        var engine = new TestEngine(true);
        var pointer = engine.Execute(nef.Script, getPointer.Offset) as Pointer;
        Assert.IsNotNull(pointer);

        var result = engine.Execute(nef.Script, pointer.Position);
        Assert.AreEqual(5, result.GetInteger());
    }
}
