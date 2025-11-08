// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_PrivateMethod.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.SmartContract.Testing;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_PrivateMethod : DebugAndTestBase<Contract1>
    {
        [TestMethod]
        public void Test_PrivateMethod()
        {
            // Optimizer will remove this method
            Assert.IsFalse(Encoding.ASCII.GetString(Contract1.Nef.Script.Span).Contains("NEO3"));

            // Compile without optimizations

            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract1.cs").FullName;
            var results = CompilationTestHelper.CompileSource(testContractsPath, options =>
            {
                options.Debug = CompilationOptions.DebugType.Extended;
                options.CompilerVersion = "TestingEngine";
                options.Optimize = CompilationOptions.OptimizationType.None;
            });

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);

            var nef = results[0].CreateExecutable();
            Assert.IsTrue(Encoding.ASCII.GetString(nef.Script.Span).Contains("NEO3"));
        }
    }
}
