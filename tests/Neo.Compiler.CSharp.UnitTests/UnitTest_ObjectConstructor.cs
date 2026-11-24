// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_ObjectConstructor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ObjectConstructor
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NewObject_LeavesAValueOnTheStack(CompilationOptions.OptimizationType optimization)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("local")]
                public static int Local()
                {
                    object value = new object();
                    return value == null ? 0 : 7;
                }

                [DisplayName("argument")]
                public static int Argument() => Keep(new object(), 7);

                private static int Keep(object value, int result) => result;
            }
            """;

        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(d => d.ToString())));

        var contract = new TestEngine(true).Deploy<ObjectConstructorContract>(
            context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(new BigInteger(7), contract.Local());
        Assert.AreEqual(new BigInteger(7), contract.Argument());
    }

    public abstract class ObjectConstructorContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("local")]
        public abstract BigInteger? Local();

        [DisplayName("argument")]
        public abstract BigInteger? Argument();
    }
}
