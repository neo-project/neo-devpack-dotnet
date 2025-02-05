// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_CheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class CheckWitnessTests : DebugAndTestBase<Contract_CheckWitness>
    {
        [TestMethod]
        public void Test_CheckWitness()
        {
            var result = CheckWitnessAnalyzer.AnalyzeCheckWitness(NefFile, Manifest, null);
            Assert.AreEqual(result.droppedCheckWitnessResults.Count, 1);
        }
    }
}
